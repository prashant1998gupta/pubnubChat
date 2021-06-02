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

   // [Header("User List Panel")]
   // public GameObject usersListPanel;
    //public GameObject ChatPanel;
   // public Transform usersPrefabListContainer;
   // public GameObject userPrefab;

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
        //loginPanel.SetActive(true);
        //usersListPanel.SetActive(false);
        //chatInboxPanel.SetActive(false);

        backBtn.onClick.AddListener(OnBackButtonClick);
        chatHistoryBtn.onClick.AddListener(OnHistoryBtnClick);

        sendChatBtn.onClick.AddListener(OnClickSendButton);
    }

    private void OnClickSendButton()
    {
        GetComponent<SendChatBtnClick>().OnSendChatBtnClick();
    }

    private void OnHistoryBtnClick()
    {
       /* PubNubManager.instance.GetHistory(
           StaticDataManager.myUUID,
           StaticDataManager.currentUserUUID,
           StaticDataManager.HistoryMsgCount);*/
    }

    private void OnBackButtonClick()
    {
        //ChatUIManager.instance.usersListPanel.SetActive(true);
        UIManager.instance.ButtonPanel.SetActive(true);
        ChatUIManager.instance.chatInboxPanel.SetActive(false);

        StaticDataManager.currentUserUUID = null;
        ChatUIManager.instance.chatInputField.text = "";
    }

    private void OnEnable()
    {
        /*if (StaticDataManager.instance.gameState == GameState.Build)
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;*/
        UIManager.UserStateChange += UserStateChangedCallback;
        UIManager.UserStateChange += UserStateChangedCallback;
    }

    private void OnDisable()
    {
       /* if (StaticDataManager.instance.gameState == GameState.Build)
            Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;*/
        UIManager.UserStateChange -= UserStateChangedCallback;
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
            // GenerateUsersList();

            PubNubManager.instance.PubNubConfig(StaticDataManager.myUUID);

        }
        else
        {
           // loginPanel.SetActive(true);
           // usersListPanel.SetActive(false);
        }
    }

  /*  private void GenerateUsersList()
    {
        StartCoroutine(CreateUsers());
    }*/

   /* IEnumerator CreateUsers()
    {
        foreach (Transform obj in usersPrefabListContainer)
            Destroy(obj.gameObject);

        yield return new WaitForEndOfFrame();

        PopulateUserContainer();
    }*/

  /*  private void PopulateUserContainer()
    {
        for (int i = 0; i < StaticDataManager.instance.staticUsersInfo.Count; i++)
        {
            GameObject entry = Instantiate(userPrefab, usersPrefabListContainer);

            StaticUsersInfo data = StaticDataManager.instance.staticUsersInfo[i];

            entry.GetComponent<UserData>().Init(
                data.userName,
                data.UUID);

            if (data.isMine)
            {
                entry.transform.SetAsFirstSibling();
                StaticDataManager.myName = data.userName;
                StaticDataManager.myUUID = data.UUID;
                entry.GetComponent<UserData>().isMine = true;
                entry.GetComponent<Button>().interactable = false;
            }
            else
                entry.GetComponent<UserData>().isMine = false;
        }

        PubNubManager.instance.PubNubConfig(StaticDataManager.myUUID);
    }*/
}
