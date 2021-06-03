using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUIManager : MonoBehaviour
{
    public static ChatUIManager instance;

   // [Header("Login Panel")]
   // public GameObject loginPanel;

    [Header("User List Panel")]
    public GameObject usersListPanel;
    //public GameObject ChatPanel;
    public Transform usersPrefabListContainer;
    public GameObject userPrefab;
    public Button listBackButton;


    [Header("Chat Inbox Panel")]
    public GameObject chatInboxPanel;
    public Button backBtn;
    
    public Text otherUserNameText;

   
    public Button chatHistoryBtn;
    
    public Transform chatContainer;
    public GameObject chatPrefab;
   
    public Color32 myTextColor;
    public Color32 otherTextColor;
  
    public InputField chatInputField;
    public Button sendChatBtn;

    private void Awake() => instance = this;
    private void Start()
    {
        sendChatBtn.interactable = false;
        //loginPanel.SetActive(true);
        usersListPanel.SetActive(false);
        chatInboxPanel.SetActive(false);

        backBtn.onClick.AddListener(OnBackButtonClick);
        chatHistoryBtn.onClick.AddListener(OnHistoryBtnClick);

        sendChatBtn.onClick.AddListener(OnClickSendButton);
        listBackButton.onClick.AddListener(OnClickListBackBtn);
        chatInputField.onValueChanged.AddListener(OnInputValuChange);
    }

    private void OnInputValuChange(string arg0)
    {
        if(string.IsNullOrEmpty(chatInputField.text))
        {
            sendChatBtn.interactable = false;
        }
        else
        {
            sendChatBtn.interactable = true;
        }
    }

    private void OnClickListBackBtn()
    {
        usersListPanel.SetActive(false);
    }

    private void OnClickSendButton()
    {
        GetComponent<SendChatBtnClick>().OnSendChatBtnClick();
    }

    private void OnHistoryBtnClick()
    {
       /* PubNubManager.instance.GetHistory(
           StaticDataManager.myUUID,
           StaticDataManager.opponentUUID,
           StaticDataManager.HistoryMsgCount);*/
    }

    private void OnBackButtonClick()
    {
        //ChatUIManager.instance.usersListPanel.SetActive(true);
        UIManager.instance.ButtonPanel.SetActive(true);
        ChatUIManager.instance.chatInboxPanel.SetActive(false);

        StaticDataManager.opponentUUID = null;
        ChatUIManager.instance.chatInputField.text = "";


        if (StaticDataManager.instance.chatType == ChatType.Globel)
        {
            UIManager.instance.globelMsgBg.SetActive(false);
            StaticDataManager.instance.globelMsg = 0;
            UIManager.instance.globelMsg.text = "";
        }
        else if (StaticDataManager.instance.chatType == ChatType.PWR_BWR)
        {
            UIManager.instance.modeMsgBg.SetActive(false);
            StaticDataManager.instance.modeMsg = 0;
            UIManager.instance.modeMsg.text = "";
        }
        else if (StaticDataManager.instance.chatType == ChatType.Clan)
        {
            UIManager.instance.clanMsgBg.SetActive(false);
            StaticDataManager.instance.clanMsg = 0;
            UIManager.instance.clanMasg.text = "";
        }

    }

    private void OnEnable()
    {
        /*if (StaticDataManager.instance.gameState == GameState.Build)
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;*/
       
        UIManager.UserStateChange += UserStateChangedCallback;
    }

    private void OnDisable()
    {
       /* if (StaticDataManager.instance.gameState == GameState.Build)
            Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;*/
       
        UIManager.UserStateChange -= UserStateChangedCallback;
    }

 /*   private void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("Token Recieved : " + token.Token);
        StaticDataManager.deviceToken = token.Token;
    }*/

    private void UserStateChangedCallback(bool success)
    {
        if (success)
        {
            //usersListPanel.SetActive(true);
            //loginPanel.SetActive(false);
            GenerateUsersList();

            PubNubManager.instance.PubNubConfig(StaticDataManager.myUUID);
        }
        else
        {
           // loginPanel.SetActive(true);
           // usersListPanel.SetActive(false);
        }
    }

    private void GenerateUsersList()
    {
        StartCoroutine(CreateUsers());
    }

    IEnumerator CreateUsers()
    {
        foreach (Transform obj in usersPrefabListContainer)
            Destroy(obj.gameObject);

        yield return new WaitForEndOfFrame();

        PopulateUserContainer();
    }

    private void PopulateUserContainer()
    {
        for (int i = 0; i < StaticDataManager.instance.staticUsersInfo.Count; i++)
        {
           // GameObject entry = Instantiate(userPrefab, usersPrefabListContainer);

            StaticUsersInfo data = new StaticUsersInfo();


            if(i==0)
            {
                data.isMine = true;
                data.userName = StaticDataManager.instance.Name;
                data.gameModeName = StaticDataManager.instance.ModeGroup;
                data.clanName = StaticDataManager.instance.ClanGroup;
                data.UUID = StaticDataManager.myUUID;

                StaticDataManager.instance.staticUsersInfo[0].isMine = data.isMine;
                StaticDataManager.instance.staticUsersInfo[0].userName = data.userName;
                StaticDataManager.instance.staticUsersInfo[0].gameModeName = data.gameModeName;
                StaticDataManager.instance.staticUsersInfo[0].clanName = data.clanName;
                StaticDataManager.instance.staticUsersInfo[0].UUID = data.UUID;
            }
            else
            {
                data.isMine = false;
                data.userName = "Gauest" + UnityEngine.Random.Range(100000, 1000000);
                data.gameModeName = "PWR";
                data.clanName = "PWR A";
                data.UUID = "UUID" + UnityEngine.Random.Range(1000000, 100000000);


                StaticDataManager.instance.staticUsersInfo[i].isMine = data.isMine;
                StaticDataManager.instance.staticUsersInfo[i].userName = data.userName;
                StaticDataManager.instance.staticUsersInfo[i].gameModeName = data.gameModeName;
                StaticDataManager.instance.staticUsersInfo[i].clanName = data.clanName;
                StaticDataManager.instance.staticUsersInfo[i].UUID = data.UUID;
            }

        }



        for (int i = 0; i < StaticDataManager.instance.staticUsersInfo.Count; i++)
        {
            GameObject entry1 = Instantiate(userPrefab, usersPrefabListContainer);

            GameObject entry = entry1.transform.GetChild(0).gameObject;

            StaticUsersInfo data = StaticDataManager.instance.staticUsersInfo[i];


            if (i == 0)
            {

                 entry.transform.SetAsFirstSibling();

                entry.GetComponent<UserData>().isMine = data.isMine;
                entry.GetComponent<UserData>().myName = data.userName;
                entry.GetComponent<UserData>().gameModeName = data.gameModeName;
                entry.GetComponent<UserData>().clanName = data.clanName;
                entry.GetComponent<UserData>().myUUID = data.UUID;

                entry.GetComponent<UserData>().Init(
                data.isMine,
                data.userName,
                data.gameModeName,
                data.clanName,
                data.UUID);


                entry.GetComponent<Button>().interactable = false;
            }
            else
            {
                entry.GetComponent<UserData>().Init(
                data.isMine,
                data.userName,
                data.gameModeName,
                data.clanName,
                data.UUID);

                 entry.GetComponent<UserData>().isMine = false;
            }

        }
         //PubNubManager.instance.PubNubConfig(StaticDataManager.myUUID);
    }
}
