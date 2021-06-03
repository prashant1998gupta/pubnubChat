using PubNubAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Testing, Build };
public enum UserState { LoggedOut, LoggedIn };
public enum PubNubState { none, Joined };

public enum ChatType {none , Globel , PWR_BWR , Clan , Privete}

[Serializable]
public class StaticUsersInfo
{
    public bool isMine;
    public string userName;
    public string gameModeName;
    public string clanName;
    public string UUID;
}

public class StaticDataManager : MonoBehaviour
{
    public static StaticDataManager instance;

    #region PubNub Keys
    public const string pubNubPublishKey = "pub-c-f425597d-dc0f-48d7-b5e2-2e4595f9317c";
    public const string pubNubSubscribeKey = "sub-c-8662494a-bd1a-11eb-9c3c-fe487e55b6a4";
    public const string pubNubSecretKey = "sec-c-MWNkMjI4MzgtMjJhYS00ZDcyLTlkZmMtODM1M2FlMGZkMzAz";
    #endregion

    public GameState gameState = GameState.Testing;
    public UserState userState = UserState.LoggedOut;
    public PubNubState pubNubState = PubNubState.none;

    public ChatType chatType = ChatType.none;


    public string ModeGroup;
    public string ClanGroup;
    public string Name;

    public  string globelChannel;
    public  string modeChannel;
    public  string clanChannel;
    public  string privateChannel;


    [Header("Static Users Info")]
    public List<StaticUsersInfo> staticUsersInfo = new List<StaticUsersInfo>(20);

    [Header("My Details")]
    public static string deviceToken;
    public static string myUUID;
    public static string myName;

    public static bool IsMineMsg = false;


    public int clanMsg;
    public int modeMsg;
    public int globelMsg;

    [Header("OtherUserDetails")]
    public static string opponentUUID;

    public static ushort maxMessagesToDisplay = 3;
    public static ushort HistoryMsgCount = 2;

    public static List<PNMessageResult> AllChatMessages;

    private void Awake() => instance = this;

    private void Start()
    {
        // UIManager.OnUserSummitButton += CallAfterSummit;
        if (PlayerPrefs.GetString("UUID") == "")
        {
            myUUID = "UUID" + UnityEngine.Random.Range(10000, 100000);
            PlayerPrefs.SetString("UUID", myUUID);
        }
        else
        {
            myUUID = PlayerPrefs.GetString("UUID");
        }

        Debug.Log(myUUID);

    }

    public void CallAfterSummit()
    {
        if (PlayerPrefs.GetString("name") == "" && PlayerPrefs.GetString("mode") == "" && PlayerPrefs.GetString("clan") == "")
        {
            PlayerPrefs.SetString("name", Name);
            PlayerPrefs.SetString("mode", ModeGroup);
            PlayerPrefs.SetString("clan", ClanGroup);
        }

      

    }
}