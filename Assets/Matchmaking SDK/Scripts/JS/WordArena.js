
// Scheduled Tasks


handlers.AddAllPlayersToLevelLeaderboard=function(args,context)
{
    var profile = context.playerProfile;
    var request= server.UpdatePlayerStatistics(
    {
        "PlayFabId": currentPlayerId,
        "Statistics": [
          {
            "StatisticName": args.StatisticName,
            "Value": args.StartValue
          }
        ]
      });

    }


//VirtualCurrency API's


handlers.loadVirtualCurrency = function(args,context){
    //currentPlayerId="C3DC61C584968086";
    var currentCurrency = server.GetUserInventory({
        PlayFabId: currentPlayerId
    });
    
    //var currencyResponse = JSON.parse(JSON.stringify(currentCurrency.VirtualCurrency));
    return JSON.stringify(currentCurrency); /*{response: currencyResponse}*/;

    
};

handlers.DeductLives = function(args,context){
    var subtractDarts = server.SubtractUserVirtualCurrency({
        Amount: 1,
        PlayFabId: currentPlayerId,
        VirtualCurrency: "LF"
    });
    
    return subtractDarts;
};



//////MatchMaking API'S

//Updates the statistics and data of player in the backend
handlers.OnPVPFinish = function(args, context)
{
    try{
        let matchResult = args.result; //result of match (Win, Lose)
        let leaderboardCategoryName = args.leaderboardCategory; //name of the category the player belongs to
        let masterLeaderboardName = args.masterLeaderboardName; //name of the unfiltered leaderboard of the player
        let playId = args.playerId;
        log.info("Entry");
        const regularPts = 10;

        let PlayerPVPStatistics = server.GetPlayerStatistics({
            PlayFabId: playId,
            StatisticNames: [ leaderboardCategoryName, masterLeaderboardName ]
        });

        //Find Statistics
        let LeaderboardCategoryStat = PlayerPVPStatistics.Statistics.find(statistic => statistic.StatisticName == leaderboardCategoryName);
        let MasterLeaderboardStat   = PlayerPVPStatistics.Statistics.find(statistic => statistic.StatisticName == masterLeaderboardName);
        log.info("done finding stats");
        //get statistic values
        let leaderboardCategoryPoints = LeaderboardCategoryStat == undefined ? 0 : LeaderboardCategoryStat.Value;
        let masterLeaderboardPoints = MasterLeaderboardStat == undefined ? 0 : MasterLeaderboardStat.Value;
        
        let previousPlayerRank = GetRank(masterLeaderboardPoints);
        //Check match result
        if(matchResult == "Win")
        {
            leaderboardCategoryPoints += regularPts;
            masterLeaderboardPoints += regularPts;
        }else
        {
            let expectedPts = leaderboardCategoryPoints - regularPts;
            if(expectedPts < 0)
            {
                leaderboardCategoryPoints = 0;
                masterLeaderboardPoints = 0;
            }else{
                leaderboardCategoryPoints -= regularPts;
                masterLeaderboardPoints -= regularPts;
            }
        }

        let currentPlayerRank = GetRank(masterLeaderboardPoints);

        if(currentPlayerRank != previousPlayerRank){
            //set the old and new leaderboard stat names
            let previousLeaderboardCategoryName = leaderboardCategoryName;
            let rankCharIdx = leaderboardCategoryName.length - 1;
            let newLeaderboardCategoryName = leaderboardCategoryName.replaceAt(rankCharIdx, String(currentPlayerRank));
            server.UpdatePlayerStatistics({
                PlayFabId: playId,
                Statistics: [
                    {
                        //set the previous category to 0
                        StatisticName: previousLeaderboardCategoryName,
                        Value: 0
                    },
                    {
                        //set current points to the new leaderboard category
                        StatisticName: newLeaderboardCategoryName,
                        Value: leaderboardCategoryPoints
                    },
                    {
                        StatisticName: masterLeaderboardName,
                        Value: masterLeaderboardPoints
                    }
                ]
            });            
        }else
        {
            //Update statistics
            server.UpdatePlayerStatistics({
                PlayFabId: playId,
                Statistics: [
                    {
                        StatisticName: leaderboardCategoryName,
                        Value: leaderboardCategoryPoints
                    },
                    {
                        StatisticName: masterLeaderboardName,
                        Value: masterLeaderboardPoints
                    }
                ]
            });
        }
        log.info("5 play id:" + playId);
        var result = {
            /*
            CurrentGainedMMR: GainedMMR,
            CurrentMMRWeekly: PlayerMMRWeekly,
            CurrentMMRMonthly: PlayerMMRMonthly,
            CurrentWinStreak: PlayerWinStreak,
            CurrentWinStreakHistorical: PlayerWinStreakHistorical,
            CurrentBattles: PlayerBattles,
            CurrentWins: PlayerWins,
            CurrentLosses: PlayerLosses,
            CurrentMaxDamage: PlayerMaxDamage,
            CurrentBonusMMR: bonusRevengeMMR
            */
           MasterLeaderBoardMMR: masterLeaderboardPoints,
           LeaderboardCategoryPoints: leaderboardCategoryPoints
        }

        return result;
    }catch(err){
        LogError({
            "EventName" : "GetPVPData_error",
            "Error":err
        });

        return err.apiErrorInfo.apiError;
    }
}

//Fetches the PVP data of the player
handlers.GetPVPData = function(args, context)
{
    try{
        const key = "pvpArenaKey";
        let playerData =  GetUserData(key)[key];
        let userData = typeof playerData === "undefined" ? GetServerData(key)[key] : playerData.Value;
        return userData;
    }catch(err){
        LogError({
            "EventName" : "GetPVPData_error",
            "Error":err
        });

        return err.apiErrorInfo.apiError;
    }
}

//Sets the remote pvp data with the local data
handlers.SetPVPData = function(args, context){
    try{
        const key = "pvpArenaKey";

        let playerData =  GetUserData(key)[key];
        let userData = typeof playerData === "undefined" ? GetServerData(key)[key] : playerData.Value;
        let userParsedData = JSON.parse(userData);
        userParsedData.lastOpponentPlayfabID = args.lastOpponentPlayfabID;
        //SetSpecificUserData(args.playerId, key, JSON.stringify(userParsedData))
        SetUserData("pvpArenaKey", JSON.stringify(userParsedData));
    }catch(err){
        LogError({
            "EventName" : "GetSetData_error",
            "Error":err
        });

        return err.apiErrorInfo.apiError;
    }
}

//Methods taken straight from Murasaki7 Arena PVP

//Method is called as a scheduled task. It places players 1 rank down to keep PvP competitive.
handlers.ResetArena = function(args){
    try{
        const Statistics = server.GetPlayerStatistics({
            "PlayFabId": currentPlayerId,
            "StatisticNames": [ "ArenaMMR_Weekly", "Mmr", "xpTotalKey"]
        }).Statistics;
        
        let xp = Statistics.find(statistic => statistic.StatisticName === "xpTotalKey").Value;
        let currentLevel = GetLevel(xp);

        if(currentLevel < 10)
        {
            log.info("Player below required level! XP:" + xp + "Player Level:" + currentLevel);
            return;
        }

        const SavedTotalMmr = Statistics.find(statistic => statistic.StatisticName == "Mmr");

        //var CurrentWeeklyMMR = SavedWeeklyMmr == undefined ? 0 : SavedWeeklyMmr.Value;
        var CurrentTotalMMR = SavedTotalMmr == undefined ? 0 : SavedTotalMmr.Value;

        var rank = GetRank(CurrentTotalMMR);
        var newRank = (rank - 1) > 0 ? (rank - 1) : 0;
        var newMMR = GetMinimumPointsByRank(newRank); //Get the minimum MMR of the rank below my current rank
        var previousRank = "ArenaMMR_Rank_" + rank;
        var newRankStatistic = "ArenaMMR_Rank_" + newRank;

        if(previousRank == newRankStatistic)
        {
            var result = server.UpdatePlayerStatistics({
                "PlayFabId": currentPlayerId,
                "Statistics": [
                    {
                        "StatisticName" : newRankStatistic,
                        "Value" : newMMR
                    },
                    {
                        "StatisticName": "ArenaMMR_Weekly",
                        "Value": newMMR
                    },
                    {
                        "StatisticName": "Mmr",
                        "Value": newMMR
                    },
                    {
                        "StatisticName": "pvp_wins",
                        "Value": 0
                    },
                    {
                        "StatisticName": "pvp_losses",
                        "Value": 0
                    },
                    {
                        "StatisticName": "ArenaWinStreak",
                        "Value": 0
                    },
                    {
                        "StatisticName": "pvp_streak",
                        "Value": 0
                    }
                ]
            });
        }else{
            var result = server.UpdatePlayerStatistics({
                "PlayFabId": currentPlayerId,
                "Statistics": [
                    {
                        "StatisticName" : previousRank,
                        "Value": 0
                    },
                    {
                        "StatisticName" : newRankStatistic,
                        "Value" : newMMR
                    },
                    {
                        "StatisticName": "ArenaMMR_Weekly",
                        "Value": newMMR
                    },
                    {
                        "StatisticName": "Mmr",
                        "Value": newMMR
                    },
                    {
                        "StatisticName": "pvp_wins",
                        "Value": 0
                    },
                    {
                        "StatisticName": "pvp_losses",
                        "Value": 0
                    },
                    {
                        "StatisticName": "ArenaWinStreak",
                        "Value": 0
                    }
                ]
            });
        }
    }catch(err){
        LogError({
            "EventName" : "WeeklyReset_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

// Using a numerical representation for arena title
function GetRank(mmr) 
{
    if(mmr >= 100 && mmr < 399)
    {
        return 1;
    }
    else if(mmr >= 400 && mmr < 799)
    {
        return 2;
    }
    else if(mmr >= 800 && mmr < 1199)
    {
        return 3;
    }
    else if(mmr >= 1200 && mmr < 1599)
    {
        return 4;
    }
    else if(mmr >= 1600)
    {
        return 5;
    }else
        return 0;
}

//Minimum points of
function GetMinimumPointsByRank(rank)
{
    switch(rank)
    {
        case 1:
            return 100;
        case 2:
            return 400;
        case 3:
            return 800;
        case 4:
            return 1200;
        case 5:
            return 1600;
        default:
            return 0;
    }
}
String.prototype.replaceAt = function(index, replacement) {
    return this.substr(0, index) + replacement + this.substr(index + replacement.length);
}

//////Revenge API'S

handlers.GetRevengeData = function(args, context){
    try{
        const key = "pvpRevengeData";
        let playerData =  GetUserData(key)[key];
        let userData = typeof playerData === "undefined" ? GetServerData(key)[key] : playerData.Value;
        return userData;
    }catch(err){
        LogError({
            "EventName" : "GetRevengeData_error",
            "Error":err
        });

        return err.apiErrorInfo.apiError;
    }
}

handlers.UpdateRevengeList = function(args, context){
    try{
        let revengeKey = "pvpRevengeData";

        if(args.didWin){
            UpdateRevengeData(args.OpponentPlayFabId, args.PlayerPlayFabId, revengeKey, true); //remove enemy from my revenge list
            UpdateRevengeData(args.PlayerPlayFabId, args.OpponentPlayFabId, revengeKey, false); //add player to enemy revenge list
        }else{
            UpdateRevengeData(args.OpponentPlayFabId, args.PlayerPlayFabId, revengeKey, false); //add enemy from my revenge list
            UpdateRevengeData(args.PlayerPlayFabId, args.OpponentPlayFabId, revengeKey, true); //remove player to enemy revenge list
        }
    }catch(err){
        LogError({
            "EventName" : "AddPlayerToRevengeList_error",
            "Error":err
        });
        return err.apiErrorInfo.apiError;
    }
}


function UpdateRevengeData(offensivePlayerId, defensivePlayerId, revengeKey, isRemoving)
{
    let savedRevengePlayerData = GetSpecificUserData(revengeKey, defensivePlayerId);//GetUserData(revengeKey)[revengeKey];
    let currentRevengePlayerData = savedRevengePlayerData.hasOwnProperty(revengeKey) ? GetServerData(revengeKey)[revengeKey] : savedRevengePlayerData.Value;
    let currentParsedData = JSON.parse(currentRevengePlayerData);
    let revengeCollection = currentParsedData.revengeIDs;
    //dupe check
    if(isRemoving == false)
    {
        let foundDuplicate = false;
        for(playerID in revengeCollection)
        {
            //currentID = revengeCollection[playerID];
            if(offensivePlayerId === playerID)
            {
                foundDuplicate = true;
                break;
            }
        }
    if(foundDuplicate == false)
            revengeCollection.push(offensivePlayerId);
    }else{
        revengeCollection = revengeCollection.filter(idToRemove => idToRemove !== offensivePlayerId);
    }

    currentParsedData.revengeIDs = revengeCollection;
    SetSpecificUserData(defensivePlayerId, revengeKey, JSON.stringify(currentParsedData));
}


///// Helper Functions 


function GetServerData(keys)
{
    try
    {
        var itemData = server.GetTitleData(
        {
            Keys  : keys
        });

        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetServerData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

function SetServerData(key, data)
{
    try
    {
        var itemData = server.SetTitleData(
        {
            Key : key,
            Value: data
        });
    }
    catch(err)
    {
        LogError({
            "EventName" : "SetServerData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

function GetServerInternalData(keys)
{
    try
    {
        var itemData = server.GetTitleInternalData(
        {
            Keys : keys
        });

        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetServerData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}
function GetSpecificUserInternalData(keys, playerId)
{
    try
    {
        var itemData = server.GetTitleInternalData(
        {
            PlayFabId : playerId,
            Keys  : keys
        });
        log.info("specificuserdata")
        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetUserData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}
function SetServerInternalData(key, data)
{
    try
    {
        var itemData = server.SetTitleInternalData(
        {
            Key : key,
            Value: data
        });


    }
    catch(err)
    {
        LogError({
            "EventName" : "GetServerData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

function SetSpecificUserData(specificPlayerId,key, data)
{
    var dataToSend = {}
    dataToSend[key] = data;
        try
        {
            var itemData = server.UpdateUserData(
            {
                PlayFabId : specificPlayerId,
                Data: dataToSend
            });
            log.info("set specific user!");
        }
        catch(err)
        {
            LogError({
                "EventName" : "SetSpecificUserData_error",
                "Error" : err
            });

            return err.apiErrorInfo.apiError;
        }
}

function GetSpecificUserData(keys, playerId)
{
    try
    {
        var itemData = server.GetUserData(
        {
            PlayFabId : playerId,
            Keys  : keys
        });
        log.info("specificuserdata")
        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetUserData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

function GetUserData(keys)
{
    try
    {
        var itemData = server.GetUserData(
        {
            PlayFabId : currentPlayerId,
            Keys  : keys
        });

        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetUserData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

