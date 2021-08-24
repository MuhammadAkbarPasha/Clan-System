
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

