using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;
using PlayFab.GroupsModels;

public class UiController : MonoBehaviour
{
    #region UIComponents
    public static UiController Instance;
    #region CreateGroupInformationUI
    public InputField groupName;
    public Image iconImage;
    public InputField groupDescription;
    #endregion
    public InputField username;
    public InputField password;
    public InputField GroupUsername;
    public Text apiResponse;
    public VerticalScrollSnap verticalScrollSnapMessages;
    public VerticalScrollSnap verticalScrollSnapLivesReceived;
    public VerticalScrollSnap verticalScrollSnapLifeMessages;

    public Text coinsText;
    public Text livesText;


    public Text spinWheelsText;
    public Text timersText;
    public Text hintsText;


    public Text b1;
    public Text b2;
    public Text b3;


    public Image ExpBar;
    public Text ExpText;
    public Text requiredExpText;

    public Text LevelText;


    #endregion
    #region Panels and Buttons
    public GameObject buttonPrefab;
    public GameObject groupInfoPanel;
    public GameObject memberInfoPanel;
    public GameObject groupInfoPanelParent;
    public GameObject groupapplicationPanel;
    public GameObject groupapplicationPanelParent;
    public GameObject PromotionPanel;
    public GameObject groupsListPanel;
    public GameObject groupOptionsPanel;


    public GameObject spinWheelItemsParent;
    public GameObject SpinWheelItem;

    public GameObject loginPanel;
    public GameObject membersPanel;
    public GameObject applyButton;
    public GameObject makeSubLeader;
    public GameObject makeLeader;
    public GameObject msgPanel;
    public GameObject msgPanelParent;
    public GameObject lifeReceivedPanel;
    public GameObject lifeReceivedPanelParent;
    public GameObject lifeRequestPanel;
    public GameObject lifeRequestPanelParent;

    #endregion


    /// <summary>
    /// Group Creation From Button Press
    /// </summary>
    /// 
    public Toggle isGroupOpenToggle;
    public GameObject setUserNamePanel;
    public Text SetUserNameErrorMessage;
    public Text newUserName;




    internal void showSetUserNamePanelForAndroidUser()
    {

        setUserNamePanel.SetActive(true);


    }


    public void TurnOffSetUserNamePanelForAndroidUser()
    {
        setUserNamePanel.SetActive(false);
    }


    public void showError(string errorMessage)
    {
        SetUserNameErrorMessage.text = "";
        SetUserNameErrorMessage.text = errorMessage + " TryAgain";
    }

    public void UpdateLevelSystemUI(string level, int exp, int requiredExp)
    {

        LevelText.text = "Level: " + level;
        ExpText.text = "Exp: " + exp.ToString();
        requiredExpText.text = exp.ToString() + "/" + requiredExp.ToString();
        ExpBar.fillAmount = (float)exp / (float)requiredExp;



    }

    public void setUsernameFromButton()
    {
        PlayFabManager.Instance.setUsername(newUserName.text);
        PlayFabManager.Instance.SetUserNameOnPlayFab();

    }
    public void CreateGroupOnButtonPress()
    {
        ClanSystemController.Instance.CreateGroup(groupName.text);

        ClanSystemController.Instance.playFabClanData.groupInformation.setDescription(groupDescription.text);
        ClanSystemController.Instance.playFabClanData.groupInformation.setIsOpen(isGroupOpenToggle.isOn);
        if (RequiredLevel.text != "")
        {
            ClanSystemController.Instance.playFabClanData.groupInformation.setRequiredLevel(int.Parse(RequiredLevel.text));
        }
        else
        {
            ClanSystemController.Instance.playFabClanData.groupInformation.setRequiredLevel();

        }

    }
    public Text RequiredLevel;
    public void setGoupIcon(string groupIcon)
    {

        ClanSystemController.Instance.playFabClanData.groupInformation.setImageId(groupIcon);
        iconImage.sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;


    }
    public void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// Buttons Instantiation for groups
    /// </summary>
    /// <param name="groupsData"></param>
    public void CreateButtonsBaseOnList(GroupsData groupsData)
    {
        foreach (Groups groups in groupsData.Groups)
        {
            GameObject currrentButton = Instantiate(buttonPrefab, groupsListPanel.transform);
            currrentButton.GetComponentInChildren<Text>().text = currrentButton.GetComponentInChildren<Text>().text + groups.GroupName;
            currrentButton.GetComponent<Button>().onClick.AddListener(() => {
                ClanSystemController.Instance.ListGroupMembers(groups.Group);

                ClanSystemController.Instance.HasPlayerAppliedAlready(groups.Group.Id, applyButton);


            }
            );

        }
        groupsListPanel.SetActive(true);
    }

    public void SetText(string result)
    {
        apiResponse.text = result;
        membersPanel.SetActive(true);
    }

    public void LoginWithPlayfab()
    {
        PlayFabManager.Instance.LoginWithPlayFab(username.text, password.text);
    }
    public void LoginWithDevice()
    {
        PlayFabManager.Instance.LoginGuest();
    }
    public void OpenGroupOptions()
    {
        loginPanel.SetActive(false);
        groupOptionsPanel.SetActive(true);
    }
    public void ApplyToGroup()
    {
        PlayFabManager.Instance.currentMember.setGroupUsername(GroupUsername.text);
        if (GroupUsername.text.Length < ClanSystemController.Instance.GetUserNameLimit() && ClanSystemController.Instance.GetApplyWithName())
        {
            Debug.Log("small username");
            return;
        }
        ClanSystemController.Instance.ApplyToGroup();

    }
    public void PopulateGroupOfCurrentPlayer()
    {
        Debug.Log("herehere");
        GroupsData PlayerGroupData = ClanSystemController.Instance.playFabClanData.thisPlayerGroupData;
        if (PlayerGroupData.Groups.Count > 0)
            groupInfoPanelParent.SetActive(true);
        else
            return;

        foreach (Groups groups in PlayerGroupData.Groups)
        {
            GameObject CurrentPanel = Instantiate(groupInfoPanel, groupInfoPanelParent.transform);
            CurrentPanel.transform.Find("Group Name").gameObject.GetComponent<Text>().text = groups.GroupName;
            //CurrentPanel.transform.Find("Role").gameObject.GetComponent<Text>().text = groups.Roles[0].RoleName;
            CurrentPanel.transform.Find("GroupDescription").gameObject.GetComponent<Text>().text = ClanSystemController.Instance.groupInfo.getDescription();
            CurrentPanel.transform.Find("GroupIcon").GetChild(int.Parse(ClanSystemController.Instance.groupInfo.getImageId())).gameObject.SetActive(true);

            if (PlayFabManager.Instance.currentMember.MemberRole.RoleId == "leader")
            {

                GameObject applicationsButton = CurrentPanel.transform.Find("ListGroupApplications-bttn").gameObject;
                applicationsButton.SetActive(true);
                applicationsButton.GetComponent<Button>().onClick.AddListener(

                    () => { ClanSystemController.Instance.ListApplicationsForThisGroup(groups.Group.Id); }
                    );
            }

            GameObject leaveGroupButton = CurrentPanel.transform.Find("Leave-bttn").gameObject;
            leaveGroupButton.GetComponent<Button>().onClick.AddListener(

                () => { ClanSystemController.Instance.leaveGroup(groups.Group.Id, PlayFabManager.Instance.currentMember.Id); }
                );







        }

        PopulateGroupMembers();
    }
    public void PopulateGroupApplications(GroupApplications currentGroupApplications)
    {
        if (currentGroupApplications.Applications.Count > 0)
            groupapplicationPanelParent.SetActive(true);

        foreach (PlayerApplication application in currentGroupApplications.Applications)
        {
            GameObject currentApplication = Instantiate(groupapplicationPanel, groupapplicationPanelParent.transform);
            currentApplication.transform.Find("Player Name").gameObject.GetComponent<Text>().text = application.Entity.Key.GetGroupUsername();
            currentApplication.transform.Find("PlayerLevel").gameObject.GetComponent<Text>().text = application.Entity.Key.GetLevel();

            currentApplication.transform.Find("Accept Application-bttn").GetComponent<Button>().onClick.AddListener(() =>
            {
                ClanSystemController.Instance.AcceptGroupApplication(application.Group.Id, application.Entity.Key.Id);
                ClanSystemController.Instance.AddMemberToSharedGroupObject(PlayFabManager.Instance.currentMember.MemberGroup.Id, application.Entity.Lineage.master_player_account.Id);
                ChatManager.Instance.GroupUpdate(application.Entity.Key.GetGroupUsername(), GroupUpdateType.UpdateType.Joined);


            });
            currentApplication.transform.Find("Reject Application-bttn").GetComponent<Button>().onClick.AddListener(() =>
            {
                /*              ClanSystemController.Instance.AcceptGroupApplication(application.Group.Id, application.Entity.Key.Id);
                                  ClanSystemController.Instance.AddMemberToSharedGroupObject(PlayFabManager.Instance.currentMember.MemberGroup.Id, application.Entity.Lineage.master_player_account.Id);
                */
                ClanSystemController.Instance.RejectGroupApplication(application.Group.Id, application.Entity.Key.Id);

            });




        }
    }

    public void PopulateGroupMembers()
    {
        foreach (Member member in ClanSystemController.Instance.playFabClanData.allMembersInSelectedGroup.Members)
        {
            foreach (MemberClass memberClass in member.Members)
            {
                if (memberClass.Key.Id != PlayFabManager.Instance.PlayFabTitleId)
                {
                    GameObject currenMemberInfoPanel = Instantiate(memberInfoPanel, groupInfoPanelParent.transform);
                    currenMemberInfoPanel.transform.Find("PlayerName").gameObject.GetComponent<Text>().text = memberClass.Key.MemberPersonalData.GroupUsername;
                    currenMemberInfoPanel.transform.Find("Role").gameObject.GetComponent<Text>().text = member.RoleName;
                    currenMemberInfoPanel.transform.Find("PlayerLevel").gameObject.GetComponent<Text>().text = memberClass.Key.MemberPersonalData.Level.ToString();
                    if (PlayFabManager.Instance.currentMember.MemberRole.RoleId == "leader")
                    {
                        currenMemberInfoPanel.transform.Find("Kick-bttn").gameObject.SetActive(true);
                        currenMemberInfoPanel.transform.Find("Promote-bttn").gameObject.SetActive(true);
                        currenMemberInfoPanel.transform.Find("Promote-bttn").gameObject.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            PromotionPanel.SetActive(true);
                            Debug.Log(member.RoleId);
                            ClanSystemController.Instance.PopulatePromotionPlayer(memberClass.Key.Id, new Role(member.RoleId, member.RoleName), PlayFabManager.Instance.currentMember.MemberGroup);
                        });
                        currenMemberInfoPanel.transform.Find("Kick-bttn").gameObject.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            ClanSystemController.Instance.BanMemberFromGroup(PlayFabManager.Instance.currentMember.MemberGroup.Id, memberClass.Key.Id);
                            ChatManager.Instance.GroupUpdate(memberClass.Key.MemberPersonalData.GroupUsername, GroupUpdateType.UpdateType.Kicked);
                        });
                    }
                }
            }
        }
    }
    public void showMessage(MessageItem msg)
    {
        GameObject currentMsgPanel = Instantiate(msgPanel, msgPanelParent.transform);
        currentMsgPanel.transform.Find("Username").GetComponent<Text>().text = msg.senderName;
        currentMsgPanel.transform.Find("Message").GetComponent<Text>().text = msg.message;
        verticalScrollSnapMessages.AddChild(currentMsgPanel);
    }
    public void ShowAllPreviousMsgs()
    {
        foreach (MessageItem msg in ChatManager.Instance.DownloadedMessages.Messages)
        {
            GameObject currentMsgPanel = Instantiate(msgPanel, msgPanelParent.transform);
            currentMsgPanel.transform.Find("Username").GetComponent<Text>().text = msg.senderName;
            currentMsgPanel.transform.Find("Message").GetComponent<Text>().text = msg.message;
            verticalScrollSnapMessages.AddChild(currentMsgPanel);

        }
    }
    public void RequestLife()
    {
        // TradeSystemController.Instance.CreateEnergyRequest(PlayFabManager.Instance.currentMember.MemberGroup.Id, PlayFabManager.Instance.PlayFabUserId);
        TradeSystemController.Instance.CreateLifeRequest(PlayFabManager.Instance.currentMember.MemberGroup.Id, PlayFabManager.Instance.PlayFabUserId);


    }
    public void UpdateCurrency()
    {
        coinsText.text = "Coins " + PlayFabManager.Instance.getCoins();
        livesText.text = "Lives" + PlayFabManager.Instance.getLives();

        spinWheelsText.text = "Spin Wheels " + PlayFabManager.Instance.getSpinWheels();
        timersText.text = "Timers " + PlayFabManager.Instance.getTimer();
        hintsText.text = "Hints " + PlayFabManager.Instance.getHint();
        b1.text = "Bonus Life 2hr " + PlayFabManager.Instance.getBonusLife1();
        b2.text = "Bonus Life 6hr " + PlayFabManager.Instance.getBonusLife2();
        b3.text = "Bonus Life 24hr " + PlayFabManager.Instance.getBonusLife3();
    }
    public Text timerText;
    public void Update()
    {
        ///   Debug.Log("exs");
        timerText.text = PlayFabManager.Instance.timeToDisplay;
    }
    public void GetUserLifeMessages()
    {
        UpdateEnergyMessages();
    }
    public void UpdateEnergyMessages()
    {
        removeAllFromSnap(verticalScrollSnapLivesReceived);

        foreach (InventoryMessageItem inventoryMessageItem in TradeSystemController.Instance.inventoryMessages.messages)
        {
            GameObject currentLifeReceivedPanel = Instantiate(lifeReceivedPanel, lifeReceivedPanelParent.transform);
            currentLifeReceivedPanel.transform.Find("Text").gameObject.GetComponent<Text>().text = inventoryMessageItem.senderName + " " + inventoryMessageItem.message;

            currentLifeReceivedPanel.transform.Find("GetLife").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                //TradeSystemController.Instance.ConsumeEnergy();

                TradeSystemController.Instance.ConsumeLife();


                TradeSystemController.Instance.messageNumber = inventoryMessageItem.messageNumber;

            });

            verticalScrollSnapLivesReceived.AddChild(currentLifeReceivedPanel);
        }
    }
    public void GetLifeRequests()
    {
        TradeSystemController.Instance.GetLifeRequests(PlayFabManager.Instance.currentMember.MemberGroup.Id);

        TradeSystemController.Instance.GetUserLifeMessages();
    }
    public void updateUiOnSingleLifeRequest(LifeRequestItem lifeRequestItem)
    {
        GameObject currentLifeRequestPanel = Instantiate(lifeRequestPanel, lifeRequestPanelParent.transform);
        currentLifeRequestPanel.transform.Find("LivesRemaining").
        gameObject.GetComponent<Text>().text = lifeRequestItem.lifeReceived + "/" + "5";
        currentLifeRequestPanel.transform.Find("Info").gameObject.GetComponent<Text>().text = "";
        currentLifeRequestPanel.transform.Find("Info").gameObject.GetComponent<Text>().text = lifeRequestItem.name + " " + "Asked for Lives";
        if (lifeRequestItem.lifeReceived < 5 && (!lifeRequestItem.unallowedMembers.Exists((obj) => obj == PlayFabManager.Instance.PlayFabUserId)))
        {
            currentLifeRequestPanel.transform.Find("GiveLife").
            gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {

                TradeSystemController.Instance.
                //   AwardEnergyContainer    (lifeRequestItem.playfabID, PlayFabManager.Instance.currentMember.MemberPersonalData.GroupUsername, PlayFabManager.Instance.currentMember.MemberGroup.Id);
                AwardLifeContainer(lifeRequestItem.playfabID, PlayFabManager.Instance.currentMember.MemberPersonalData.GroupUsername, PlayFabManager.Instance.currentMember.MemberGroup.Id);

                /*
                                TradeSystemController.Instance.GetEnergyRequests(PlayFabManager.Instance.currentMember.MemberGroup.Id);
                                */
                TradeSystemController.Instance.GetLifeRequests(PlayFabManager.Instance.currentMember.MemberGroup.Id);


            });
        }
        else
        {

            currentLifeRequestPanel.transform.Find("GiveLife").
            gameObject.GetComponent<Button>().interactable = false;
        }

        verticalScrollSnapLifeMessages.AddChild(currentLifeRequestPanel);
    }
    public void showAllRequestsOnPanelOpen()
    {
        removeAllFromSnap(verticalScrollSnapLifeMessages);
        foreach (LifeRequestItem life in TradeSystemController.Instance.lifeResponse.lifeRequests)
        {
            updateUiOnSingleLifeRequest(life);
        }
    }
    public void OnSignOut()
    {
        coinsText.text = "";
        livesText.text = "";

        spinWheelsText.text = "";
        timersText.text = "";
        hintsText.text = "";

    }
    public void removeAllFromSnap(VerticalScrollSnap verticalScrollSnap)
    {
        GameObject[] gb;
        verticalScrollSnap.RemoveAllChildren(out gb);
    }

    public void PopulateSpinWheelItems()
    {
        foreach (Transform t in spinWheelItemsParent.transform)
        {
            Destroy(t.gameObject);

        }

        GameObject currentSpinWheelItem;
        foreach (CloudResponse.SpinWheel.Node node in SpinWheelController.Instance.spinWheel.Nodes)
        {

            currentSpinWheelItem = Instantiate(SpinWheelItem, spinWheelItemsParent.transform);
            currentSpinWheelItem.transform.Find("Text").gameObject.GetComponent<Text>().text = node.ResultItem;
            //currentSpinWheelItem.transform.Find();




        }




    }
    [ContextMenu("Stop Spin")]
    public void stopSpin()
    {
        iskeepSpining = true;

    }
    bool iskeepSpining = false;
    public GameObject spinWheelPanel;
    public void startTheSpin()
    {

        if (PlayFabManager.Instance.getSpinWheels() < 1)
            return;

        spinWheelPanel.SetActive(true);
        iskeepSpining = true;
        StartCoroutine(SpinTheWheel());
        StartCoroutine(EvaluateTheWheel());

    }

    public IEnumerator SpinTheWheel()
    {

        int totalCount = spinWheelItemsParent.transform.childCount;

        int startItem = Random.Range(0, totalCount);
        while (iskeepSpining && totalCount > 0)
        {
            foreach (Transform t in spinWheelItemsParent.transform)
            {
                t.gameObject.GetComponent<Image>().enabled = false;
            }
            spinWheelItemsParent.transform.GetChild(startItem).gameObject.GetComponent<Image>().enabled = true;
            yield return new WaitForSeconds(0.10f);
            startItem++;
            if (startItem >= totalCount)
                startItem = 0;

        }




    }

    public IEnumerator EvaluateTheWheel()
    {

        yield return new WaitForSeconds(5f);
        SpinWheelController.Instance.EvaluateSpinWheel();


    }
    public void StopTheWheel(int selectedItem)
    {
        iskeepSpining = false;
        foreach (Transform t in spinWheelItemsParent.transform)
        {
            t.gameObject.GetComponent<Image>().enabled = false;
        }
        spinWheelItemsParent.transform.GetChild(selectedItem).gameObject.GetComponent<Image>().enabled = true;



    }
    public Text eventText;
    public Text eventDesc;
    public Text eventTitle;

    public void updateEventUI(string totalObjectives, string currentObjectives, string title, string desc)
    {

        eventText.text = currentObjectives + "/" + totalObjectives;
        eventTitle.text = title;
        eventDesc.text = desc;

    }


    public GameObject TournamentPanel;
    public GameObject singlePlayerPanelParent;
    public GameObject singlePlayerPanel;
    public VerticalScrollSnap verticalScrollSnapPersonalTournament;

    public void populatePersonalTournament()
    {

        removeAllFromSnap(verticalScrollSnapPersonalTournament);

        for (int i = 0; i < TournamentSystem.Instace.personalTournament.singlePlayerData.Count; i++)
        {
            GameObject currentPlayerData = Instantiate(singlePlayerPanel, singlePlayerPanelParent.transform);
            currentPlayerData.transform.Find("Rank").gameObject.GetComponent<Text>().text = (i + 1).ToString();
            currentPlayerData.transform.Find("Name").gameObject.GetComponent<Text>().text = TournamentSystem.Instace.personalTournament.singlePlayerData[i].DisplayName;
            currentPlayerData.transform.Find("Value").gameObject.GetComponent<Text>().text = TournamentSystem.Instace.personalTournament.singlePlayerData[i].Value.ToString();
            verticalScrollSnapPersonalTournament.AddChild(currentPlayerData);
        }


    }
    public void refreshLeaderboard()
    {
        TournamentSystem.Instace.getPersonalTournament();

    }



    public GameObject groupTournamentPanel;
    public GameObject groupPlayerPanelParent;
    public VerticalScrollSnap verticalScrollSnapGroupTournament;

    public void populateGroupTournament()
    {

        removeAllFromSnap(verticalScrollSnapGroupTournament);

        for (int i = 0; i < TournamentSystem.Instace.teamEvent.PlayerStats.Count; i++)
        {
            GameObject currentPlayerData = Instantiate(singlePlayerPanel, groupPlayerPanelParent.transform);
            currentPlayerData.transform.Find("Rank").gameObject.GetComponent<Text>().text = (i + 1).ToString();
            currentPlayerData.transform.Find("Name").gameObject.GetComponent<Text>().text = TournamentSystem.Instace.teamEvent.PlayerStats[i].GroupUsername;
            currentPlayerData.transform.Find("Value").gameObject.GetComponent<Text>().text = TournamentSystem.Instace.teamEvent.PlayerStats[i].Stars.ToString();
            verticalScrollSnapGroupTournament.AddChild(currentPlayerData);
        }


    }
    public void refreshGroupLeaderboard()
    {
        TournamentSystem.Instace.GetTeamEvent();
    }

    [Header("Inbox UI")]
    public GameObject ClaimGiftButton;
    public Text InboxTextDescription;
    public Text MessageCount;
    public GameObject InboxPanelParent;
    public GameObject GiftPanel;
    public void RefreshInbox(GiftsResponse inboxResponse)
    {
        foreach (Transform t in InboxPanelParent.transform)
        {
            Destroy(t.gameObject);
        }

        MessageCount.text = inboxResponse.Gifts.Count.ToString();
        GameObject gb = null;
        foreach (Gift gf in inboxResponse.Gifts)
        {
            gb = Instantiate(GiftPanel, InboxPanelParent.transform);
            gb.GetComponent<GiftPanel>().SetMessage(gf.Message);

            Button bt = gb.AddComponent<Button>();
            bt.onClick.AddListener(() => {
                InboxSystemController.Instance.OpenCoinsGift(gf.InstanceId);
            });
        }        
    }
}