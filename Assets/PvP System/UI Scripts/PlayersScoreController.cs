using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGGamesCore.Matchmaker;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;


/// <summary>
/// This class contols the flow of PVP 
/// </summary>
public class PlayersScoreController : MonoBehaviour
{
    //public IsHiddenObject isHiddenObject;
    /// <summary>
    /// Static instance of PlayersScoreController class
    /// </summary>
    public static PlayersScoreController Instance;
    /// <summary>
    /// Player 1 PlayerInstance refernce 
    /// </summary>
    [SerializeField]
    PlayerInstance player1Instance;
    /// <summary>
    /// Player 2 PlayerInstance refernce 
    /// </summary>
    [SerializeField]
    PlayerInstance player2Instance;
    /// <summary>
    /// Debug local player win
    /// </summary>
    public bool hasWon = false;
    /// <summary>
    /// Boolean to control the flow. Tells if currently match is ongoing or not
    /// </summary>
    public bool MatchInProgress = false;
    /// <summary>
    /// List of possible puzzle types
    /// </summary>
    /// <typeparam name="PuzzleType"></typeparam>
    /// <returns></returns>
    public List<PuzzleType> Puzzles = new List<PuzzleType>();
    /// <summary>
    /// Current on going level object
    /// </summary>
    [Header("Level To Be Played")]
    public LevelClass levelObject;
    /// <summary>
    /// Current on going city stack
    /// </summary>
    /// <returns></returns>
    private StackClass CurrentCityStack = new StackClass();
    /// <summary>
    /// Current on going city stack
    /// </summary>
    /// <returns></returns>

    private StackSystem CurrentGeneralStack = new StackSystem();
    // public PVPEnums.GameStatus GameStatus { get => _gameStatus; set => _gameStatus = value; }

    /// <summary>
    /// Static method used to fetch PuzzleType based on available PuzzleType objects list
    /// </summary>
    /// <param name="items">List of PuzzleType to fetch from</param>
    /// <returns></returns>
    public static PuzzleType SelectPuzzle(List<PuzzleType> items)
    {
        // Calculate the summa of all portions.
        int poolSize = 0;
        for (int i = 0; i < items.Count; i++)
        {
            poolSize += items[i].percentage;
        }

        // Get a random integer from 0 to PoolSize.
        System.Random rnd = new System.Random();
        int randomNumber = rnd.Next(0, poolSize) + 1;

        // Detect the item, which corresponds to current random number.
        int accumulatedProbability = 0;
        for (int i = 0; i < items.Count; i++)
        {
            accumulatedProbability += items[i].percentage;
            if (randomNumber <= accumulatedProbability)
                return items[i];
        }
        return null;    // this code will never come while you use this programm right :)
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    /// <summary>
    /// Call this when a player has won to set its WinTime field
    /// </summary>

    public void CallThisPlayerWinTimeRPC()
    {
        if (player1Instance.realTimePVP == null || player2Instance.realTimePVP == null)
            return;
        if (player1Instance.realTimePVP.pv.Owner.IsLocal)
        {
            player1Instance.realTimePVP.SetWinTimeRPCCaller();
        }
        else
        {
            player2Instance.realTimePVP.SetWinTimeRPCCaller();
        }
    }

    /// <summary>
    /// Collect both win times in case of simultaneous win and decide winner based on that
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetBothPlayerWinTime()
    {

        //  isHiddenObject = FindObjectOfType<IsHiddenObject>();
        // if (isHiddenObject == null)
        // {
        //     Debug.Log("No isHidden Object Found");
        // }

        yield return new WaitForSeconds(1f);

        if (player1Instance.realTimePVP.pv.Owner.IsLocal)
        {
            if (player1Instance.realTimePVP.GetServerTime() < player2Instance.realTimePVP.GetServerTime())
            {  //PhotonManager.Instance.LeaveRoom();
                hasWon = true;
                //    isHiddenObject.FindAllAnimationCaller();

            }
            if (player1Instance.realTimePVP.GetServerTime() > player2Instance.realTimePVP.GetServerTime())
            {//PhotonManager.Instance.LeaveRoom();
             //    AiModeManager.Instance.ThisPlayerLose();
            }
        }
        else
        {
            if (player2Instance.realTimePVP.GetServerTime() < player1Instance.realTimePVP.GetServerTime())
            {//PhotonManager.Instance.LeaveRoom();
                hasWon = true;
                // isHiddenObject.FindAllAnimationCaller();
            }
            if (player2Instance.realTimePVP.GetServerTime() > player1Instance.realTimePVP.GetServerTime())
            {  // PhotonManager.Instance.LeaveRoom();
               //AiModeManager.Instance.ThisPlayerLose();

            }
        }
    }

    /// <summary>
    /// Add difference to differences found if it hasnt been added already. Not used in original flow
    /// </summary>
    /// <param name="realTimePVP"></param>
    /// <param name="difference"></param>
    public void CheckAndAddDifference(RealTimePVP realTimePVP, int difference)
    {
        if (realTimePVP == player1Instance.realTimePVP)
        {
            if (player1Instance.PlayerFoundDifferences.Contains(difference))
                return;
            else
                player1Instance.PlayerFoundDifferences.Add(difference);

        }
        else if (realTimePVP == player2Instance.realTimePVP)
        {
            if (player2Instance.PlayerFoundDifferences.Contains(difference))
                return;
            else
                player2Instance.PlayerFoundDifferences.Add(difference);

        }
    }

    /// <summary>
    /// Set realTimePVP of player1 and player2 instances on the basis of player's current state.
    /// If the are already set, it make a call to start a coroutine of starting a match.
    /// </summary>
    /// <param name="realTimePVP"></param>
    public void SetupInstance(RealTimePVP realTimePVP)
    {
        switch (realTimePVP.playerCurrentState)
        {
            case PVPEnums.PlayerCurrentState.Player1:
                {
                    player1Instance.realTimePVP = realTimePVP;
                    //  realTimePVP.playerScorePanelController = player1Instance.playerScorePanelController;
                    //   player1Instance.playerScorePanelController.realTimePVP = realTimePVP;
                }
                break;
            case PVPEnums.PlayerCurrentState.Player2:
                {
                    player2Instance.realTimePVP = realTimePVP;
                    //   realTimePVP.playerScorePanelController = player2Instance.playerScorePanelController;
                    //s player2Instance.playerScorePanelController.realTimePVP = realTimePVP;
                }
                break;
        }
        if (player1Instance.realTimePVP != null && player2Instance.realTimePVP != null)
        {
            //GamePanel.SetActive(true);
            // OnMatchStart(testPoint);

            // This Is From Where Match Will Start
            StartCoroutine(StartMatchIfImagesExists());
        }
    }


    /// <summary> 
    /// Run in loop and do noting until the level's image is downloaded.
    /// If image is downloaded, turn on the match progress toggle and call OnMatchFoundFromPVP on the basis of local and remote instance.
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartMatchIfImagesExists()
    {

        // New Level Genereation Will Be Here
        //int levelToBePlayed= 
        yield return new WaitForSeconds(2f);
        LevelGenerationBasedOnLevelsStack(); //New Way
                                             // LevelDownloadManager.Instance.CheckIfImageAvailable(PhotonManager.Instance.LevelNumber);//Old Way

        // while (!LevelDownloadManager.Instance.isRendomLevelImageDownloaded)
        // {
        //     if (!MatchmakingSystem.Instance.notStuck)
        //     {
        //         MatchmakingSystem.Instance.notStuck = true;


        //    
        //     yield return new WaitForSeconds(.05f);

        // }


        MatchInProgressToggle(true);

        if (player1Instance.realTimePVP.pv.Owner.IsLocal)
        {
            MatchmakingSystem.Instance.OnMatchFoundFromPVP(player2Instance.realTimePVP.PlayerDataForSyncing);
        }
        else
        {
            MatchmakingSystem.Instance.OnMatchFoundFromPVP(player1Instance.realTimePVP.PlayerDataForSyncing);
        }
    }


    /// <summary>
    /// Puzzles setup based on CityClass object
    /// </summary>
    /// <param name="CityData">CityData object based on which puzzles types need to be setup</param>
    public void SetUpPuzzles(CityClass CityData)
    {
        Puzzles.Clear();

        Puzzles.Add(new PuzzleType(CityData.Probability, PVPEnums.PuzzleTypeEnums.City, "City"));
        foreach (GeneralforeignClass general in CityData.GeneralTypesIncluded)
        {
            Puzzles.Add(new PuzzleType(general.Probability, PVPEnums.PuzzleTypeEnums.General, general.GeneralName));
        }
    }

    /// <summary>
    /// Level generation from stack for playing against AI
    /// </summary>
    public void LevelGenerationBasedOnLevelsStackVSAI()    //Level Generation VS AI
    {
        CurrentCityStack = PlayerLevelsDataManager.Instance.cityStackSystem.GetStack(PhotonManager.Instance.currentArena);
        // TO DO : Filter list from levels against AI
        CurrentCityStack.LevelsUnPlayed = CurrentCityStack.LevelsUnPlayed.Where(x => ArenaAndMatchMakingBridge.Instance.levelsToBeFiltered.LevelsToBeFiltered.All(y => y != x)).ToList();
        CurrentGeneralStack.stacks = PlayerLevelsDataManager.Instance.generalStackSystem.
        GetStack(ArenaAndMatchMakingBridge.Instance.
        GetGeneralList(CurrentCityStack.name));
        if (CurrentCityStack == null)
        {
            CurrentCityStack = new StackClass("London", "City");
        }
        int levelToBePlayed = -1;
        PuzzleType SelectedPuzzle = new PuzzleType();
        SelectedPuzzle = SelectPuzzle(Puzzles);
        switch (SelectedPuzzle.puzzleType)
        {
            case PVPEnums.PuzzleTypeEnums.City:
                {
                    levelToBePlayed = LevelGenerationBasedOnLevelsStack(CurrentCityStack.LevelsUnPlayed);
                    levelObject = new LevelClass(levelToBePlayed, "City", PhotonManager.Instance.currentArena);
                }
                break;
            case PVPEnums.PuzzleTypeEnums.General:
                {
                    // levelToBePlayed = LevelGenerationBasedOnLevelsStack(player1Instance.realTimePVP.GetSpecificGeneralStack(SelectedPuzzle.typeInString).LevelsUnPlayed,
                    //    player2Instance.realTimePVP.GetSpecificGeneralStack(SelectedPuzzle.typeInString).LevelsUnPlayed);
                    levelToBePlayed = LevelGenerationBasedOnLevelsStack(CurrentGeneralStack.stacks.Find((ob) => ob.name == SelectedPuzzle.typeInString).LevelsUnPlayed);
                    levelObject = new LevelClass(levelToBePlayed, "General", SelectedPuzzle.typeInString);
                }
                break;
        }
        ArenaAndMatchMakingBridge.Instance.PrintDebugOnNextLine("PlayersScoreController FOR AI" + levelObject.levelType + "Level Number " + levelObject.levelNumber);
    }


    /// <summary>
    /// Level generation from stack for playing agains real player
    /// </summary>
    public void LevelGenerationBasedOnLevelsStack()  //Level Generation VS Player
    {
        bool IsGeneral = false;
        if ((player1Instance.realTimePVP.pv.Owner.IsMasterClient && player1Instance.realTimePVP.pv.Owner.IsLocal)
        || player2Instance.realTimePVP.pv.Owner.IsMasterClient && player2Instance.realTimePVP.pv.Owner.IsLocal)
        {
            int levelToBePlayed = -1;
            PuzzleType SelectedPuzzle = new PuzzleType();
            SelectedPuzzle = SelectPuzzle(Puzzles);
            switch (SelectedPuzzle.puzzleType)
            {
                case PVPEnums.PuzzleTypeEnums.City:
                    {
                        levelToBePlayed = LevelGenerationBasedOnLevelsStack(player1Instance.realTimePVP.CurrentCityStack.LevelsUnPlayed,
                            player2Instance.realTimePVP.CurrentCityStack.LevelsUnPlayed, false);
                        levelObject = new LevelClass(levelToBePlayed, "City", PhotonManager.Instance.currentArena);
                        CallStartMatchRPC(levelObject);

                    }
                    break;

                case PVPEnums.PuzzleTypeEnums.General:
                    {
                        levelToBePlayed = LevelGenerationBasedOnLevelsStack(player1Instance.realTimePVP.GetSpecificGeneralStack(SelectedPuzzle.typeInString).LevelsUnPlayed,
                           player2Instance.realTimePVP.GetSpecificGeneralStack(SelectedPuzzle.typeInString).LevelsUnPlayed, true);
                        if (levelToBePlayed == -1)
                        {
                            levelToBePlayed = LevelGenerationBasedOnLevelsStack(player1Instance.realTimePVP.CurrentCityStack.LevelsUnPlayed,
                           player2Instance.realTimePVP.CurrentCityStack.LevelsUnPlayed, false);
                            levelObject = new LevelClass(levelToBePlayed, "City", PhotonManager.Instance.currentArena);
                        }
                        else
                        {
                            levelObject = new LevelClass(levelToBePlayed, "General", SelectedPuzzle.typeInString);
                        }


                        CallStartMatchRPC(levelObject);

                    }
                    break;
            }
            ArenaAndMatchMakingBridge.Instance.PrintDebugOnNextLine("PlayersScoreController " + levelObject.levelType + "Level Number " + levelObject.levelNumber);
        }
    }

    /// <summary>
    /// Fetch random level from Unplayed levels for playing against AI
    /// </summary>
    /// <param name="Player1UnplayedStack">Stack from from level needs to be fetched</param>
    /// <returns></returns>
    public int LevelGenerationBasedOnLevelsStack(List<int> Player1UnplayedStack)
    {

        int levelToBePlayed = -1;
        // totalUnplayedLevels.ToArray();
        if (Player1UnplayedStack.Count > 0)
        {
            levelToBePlayed = Player1UnplayedStack[UnityEngine.Random.Range(0, Player1UnplayedStack.Count)];
        }
        return levelToBePlayed;

    }

    /// <summary>
    /// Fetch random level from inner join of both players' unplayed stacks
    /// </summary>
    /// <param name="Player1UnplayedStack"></param>
    /// <param name="Player2UnplayedStack"></param>
    /// <param name="IsGeneral"></param>
    /// <returns></returns>
    public int LevelGenerationBasedOnLevelsStack(List<int> Player1UnplayedStack, List<int> Player2UnplayedStack, bool IsGeneral)
    {

        int levelToBePlayed = -1;
        List<int> totalUnplayedLevels = Player1UnplayedStack.Intersect(Player2UnplayedStack).ToList();
        // totalUnplayedLevels.ToArray();
        if (totalUnplayedLevels.Count > 0)
        {
            levelToBePlayed = totalUnplayedLevels[UnityEngine.Random.Range(0, totalUnplayedLevels.Count)];
        }
        else
        {
            if (Player1UnplayedStack.Count == 0 && Player2UnplayedStack.Count == 0)
            {
                if (IsGeneral)
                {
                    levelToBePlayed = -1;
                }
            }
            // TO DO: IF both stacks count is zero 

            if (Player1UnplayedStack.Count >= Player2UnplayedStack.Count)
            {
                levelToBePlayed = Player1UnplayedStack[UnityEngine.Random.Range(0, Player1UnplayedStack.Count)];
            }
            else
            {

                levelToBePlayed = Player2UnplayedStack[UnityEngine.Random.Range(0, Player2UnplayedStack.Count)];
            }
        }
        return levelToBePlayed;

    }

    /// <summary>
    /// Toggle AI and MatchInProgress states
    /// </summary>
    /// <param name="toggle"></param>
    public void MatchInProgressToggle(bool toggle)
    {

        // _gameStatus = toggle ? PVPEnums.GameStatus.Playing : _gameStatus;
        //WinGameManager.Instance.isAiStop = MatchInProgress = toggle; //Stop AI  // this line was being used in original flow 
    }


    /// <summary>
    /// Call for clearing custom properties of the PlayerInstance which is local
    /// </summary>
    public void ClearCustomPropertiesOfPlayers()
    {
        if (player1Instance.realTimePVP != null)
        {
            player1Instance.realTimePVP.ClearCustomProperties();
        }
        if (player2Instance.realTimePVP != null)
        {
            player2Instance.realTimePVP.ClearCustomProperties();
        }
    }

    /// <summary>
    /// Call the EmoteRPC function on the basis of local and remote instance.
    /// </summary>
    /// <param name="index"></param>
    public void CallEmoteRPC(int index)
    {
        if (!MatchInProgress)
        {
            return;
        }
        if (player1Instance.realTimePVP == null || player2Instance.realTimePVP == null)
            return;
        if (player1Instance.realTimePVP.pv.Owner.IsLocal)
        {
            player1Instance.realTimePVP.EmoteRPC(index);
        }
        else
        {
            player2Instance.realTimePVP.EmoteRPC(index);
        }
    }



    /// <summary>
    /// Send message RPC
    /// </summary>
    /// <param name="message"></param>
    public void CallMessageRPC(string message)
    {
        if (!MatchInProgress)
        {
            return;
        }
        if (player1Instance.realTimePVP == null || player2Instance.realTimePVP == null)
            return;
        if (player1Instance.realTimePVP.pv.Owner.IsLocal)
        {
            player1Instance.realTimePVP.MessageRPC(message);
        }
        else
        {
            player2Instance.realTimePVP.MessageRPC(message);
        }
    }

    /// <summary>
    /// Call the IncreasePointsRPCCaller function on the basis of local and remote instance.
    /// </summary>
    /// <param name="index"></param>
    public void CallPointRPC(int index)
    {
        if (player1Instance.realTimePVP == null || player2Instance.realTimePVP == null)
            return;
        if (player1Instance.realTimePVP.pv.Owner.IsLocal)
        {
            player1Instance.realTimePVP.IncreasePointsRPCCaller(index);
        }
        else
        {

            player2Instance.realTimePVP.IncreasePointsRPCCaller(index);
        }
    }


    /// <summary>
    /// Freeze other player rpc caller
    /// </summary>
    public void CallFreezeRPC()
    {
        if (player1Instance.realTimePVP == null || player2Instance.realTimePVP == null)
            return;
        if (player1Instance.realTimePVP.pv.Owner.IsLocal)
        {
            Debug.Log("CallFreezeRPC 2");
            player2Instance.realTimePVP.FreezeRPCCaller();
        }
        else
        {
            Debug.Log("CallFreezeRPC 1");

            player1Instance.realTimePVP.FreezeRPCCaller();
        }
    }


    /// <summary>
    /// Let other instance of the player know to start the match with LevelClass object 
    /// </summary>
    /// <param name="levelObject">LevelClass object which contains necessary data about the level</param>
    public void CallStartMatchRPC(LevelClass levelObject)
    {
        if (player1Instance.realTimePVP == null || player2Instance.realTimePVP == null)
            return;
        if (player1Instance.realTimePVP.pv.Owner.IsLocal)
        {
            Debug.Log("StartMatchRPC 2");
            player2Instance.realTimePVP.StartMatchRPCCaller(levelObject.levelNumber, levelObject.levelType, levelObject.StackName);
        }
        else
        {
            Debug.Log("StartMatchRPC 1");

            player1Instance.realTimePVP.StartMatchRPCCaller(levelObject.levelNumber, levelObject.levelType, levelObject.StackName);
        }
    }
}
