using System;
using PubNubAPI;
using UnityEngine;
using System.Collections.Generic;
//using Pathfinding.Serialization.JsonFx;
//using Pathfinding.Serialization.JsonFx;

namespace ChatPackage
{ 

public class PubNubManager : MonoBehaviour
{
    public static PubNubManager instance;
    public PubNub pubnub;

    #region Delegate
    public static event Action<bool, List<PNMessageResult>> FetchAllMessageState = delegate { };
    public static event Action<bool, List<PNHistoryItemResult>> HistoryMessageState = delegate { };
    public static event Action<bool, Dictionary<string, object>, long, bool> MessageSent = delegate { }; // success, chat payload, isHistory
    public static event Action<bool, Dictionary<string, object>, long, bool> MessageRecieve = delegate { }; // success, chat payload, isHistory

    /* public static event Action<bool, Dictionary<string, object>, long, bool> MessageSentForGroups = delegate { }; // success, chat payload, isHistory
     public static event Action<bool, Dictionary<string, object>, long, bool> MessageRecieveForGroup = delegate { }; // success, chat payload, isHistory*/

    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }



    private void OnEnable()
    {
        UIManager.UserStateChange += UserStateChangedCallback;
    }

    private void OnDisable()
    {
        UIManager.UserStateChange -= UserStateChangedCallback;
    }

    private void UserStateChangedCallback(bool success)
    {
        if (!success)
        {
            StaticDataManager.instance.pubNubState = PubNubState.none;
            if (StaticDataManager.myUUID != null)
                UnsubscribeChannel(StaticDataManager.myUUID);

            if (pubnub != null)
            {
                pubnub.CleanUp();
                pubnub.SubscribeCallback -= SubscribeCallbackHandler;
                pubnub = null;
            }
            StaticDataManager.myName = null;
            StaticDataManager.myUUID = null;

        }
    }

    public void PubNubConfig(string UUID)
    {
        Debug.Log("PubNub Config()");
        StaticDataManager.instance.pubNubState = PubNubState.Joined;

        PNConfiguration pnConfiguration = new PNConfiguration
        {
            PublishKey = StaticDataManager.pubNubPublishKey,
            SubscribeKey = StaticDataManager.pubNubSubscribeKey,
            SecretKey = StaticDataManager.pubNubSecretKey,
            LogVerbosity = PNLogVerbosity.BODY,
            UUID = UUID
        };
        pubnub = new PubNub(pnConfiguration, this.gameObject);
        // SubscribeToGroups();
        SubscribeToMyselfAndGroup(UUID);

        pubnub.SubscribeCallback += SubscribeCallbackHandler;
    }

    void SubscribeCallbackHandler(object sender, EventArgs e)
    {
        SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;
        // Debug.LogWarning("Affacted Channel : " + JsonWriter.Serialize(mea.Status.AffectedChannels));

        if (mea.Status != null)
        {
            switch (mea.Status.Category)
            {
                case PNStatusCategory.PNUnexpectedDisconnectCategory:
                case PNStatusCategory.PNTimeoutCategory:
                    // handle publish
                    break;
            }
        }
        if (mea.MessageResult != null)
        {
            Debug.Log("Chnl : " + mea.MessageResult.Channel + "      " + StaticDataManager.IsMineMsg);
            Dictionary<string, object> chatPayload = (Dictionary<string, object>)mea.MessageResult.Payload;


            /* foreach (var item in chatPayload)
             {
                 Debug.Log("Key : " + item.Key + " Value : " + item.Value);
             }*/
            if (!StaticDataManager.IsMineMsg)
            {
                Debug.LogError($"this is not my sms {StaticDataManager.IsMineMsg}");


                if ((mea.MessageResult.Channel != GetChannelNameGloble() && mea.MessageResult.Channel != GetChannelNamePWR_BWR() && mea.MessageResult.Channel != GetChannelNameClan()))
                    {
                    string chatID = chatPayload["chatID"].ToString();
                    string[] PrivateUUIDOther = chatID.Split('_');

                    bool uerExits = true;

                    for (int i = 0; i < StaticDataManager.instance.staticUsersInfo.Count; i++)
                    {
                        if (StaticDataManager.instance.staticUsersInfo[i].UUID == chatPayload["uuid"].ToString())//chatID
                        {
                            uerExits = true;
                            Debug.Log($"use alredy exits with uuid {PrivateUUIDOther[0]} and other is {chatPayload["uuid"].ToString()}  thfidfifi{ChatUIManager.instance.chatContainer.name}");
                           GameObject ExitUserReceiveMsg = ChatUIManager.instance.usersPrefabListContainer.Find(chatPayload["uuid"].ToString()).gameObject;
                            //GameObject ExitUserReceiveMsg = ChatUIManager.instance.chatContainer.p

                            Debug.Log(ExitUserReceiveMsg);
                            Debug.Log(ExitUserReceiveMsg.name);
                            ExitUserReceiveMsg.transform.SetSiblingIndex(1);

                            GameObject entry = ExitUserReceiveMsg.transform.GetChild(0).gameObject;
                            entry.GetComponent<UserData>().statusIcon.gameObject.SetActive(true);
                            entry.GetComponent<UserData>().totalMsgis++;

                            Debug.Log(entry.GetComponent<UserData>().totalMsgis);

                           
                                if (entry.GetComponent<UserData>().totalMsgis <= 99)
                                {
                                   entry.GetComponent<UserData>().totalMsg.text = entry.GetComponent<UserData>().totalMsgis.ToString();
                                }
                                else
                                {
                                  entry.GetComponent<UserData>().totalMsg.text = "99+";
                                }
                            

                            break;
                        }
                        else
                        {
                            Debug.Log($"use not exits with uuid {PrivateUUIDOther[0]}");
                            uerExits = false;
                        }
                    }

                    if (uerExits == false)// && (mea.MessageResult.Channel != GetChannelNameGloble() && mea.MessageResult.Channel != GetChannelNamePWR_BWR() && mea.MessageResult.Channel != GetChannelNameClan()))
                    {

                        Debug.Log($"new user add to private list");

                        GameObject entry1 = Instantiate(ChatUIManager.instance.userPrefab, ChatUIManager.instance.usersPrefabListContainer);

                        GameObject entry = entry1.transform.GetChild(0).gameObject;


                        entry1.transform.SetSiblingIndex(1);

                        entry.GetComponent<UserData>().isMine = false;
                        entry.GetComponent<UserData>().myName = chatPayload["username"].ToString();
                        entry.GetComponent<UserData>().gameModeName = "PWR";
                        entry.GetComponent<UserData>().clanName = "PWR";
                        entry.GetComponent<UserData>().myUUID = chatPayload["uuid"].ToString();
                        entry.GetComponent<UserData>().statusIcon.gameObject.SetActive(true);
                        entry.GetComponent<UserData>().totalMsgis++;

                        StaticUsersInfo addList = new StaticUsersInfo();
                        addList.isMine = false;
                        addList.userName = chatPayload["username"].ToString();
                        addList.gameModeName = "PWR";
                        addList.clanName = "PWR_A";
                        addList.UUID = chatPayload["uuid"].ToString();

                        StaticDataManager.instance.staticUsersInfo.Add(addList);



                        entry.GetComponent<UserData>().Init(
                               false,
                                chatPayload["username"].ToString(),
                                "PWR",
                               "PWR",
                               chatPayload["uuid"].ToString());

                        entry1.name = chatPayload["uuid"].ToString();


                    }
                }
               else if (mea.MessageResult.Channel == GetChannelNameGloble())
                {
                    UIManager.instance.globelMsgBg.SetActive(true);
                    StaticDataManager.instance.globelMsg++;
                    if (StaticDataManager.instance.globelMsg <= 99)
                    {
                        UIManager.instance.globelMsg.text = StaticDataManager.instance.globelMsg.ToString();
                    }
                    else
                    {
                        UIManager.instance.globelMsg.text = "99+";
                    }
                }
                else if (mea.MessageResult.Channel == GetChannelNamePWR_BWR())
                {
                    UIManager.instance.modeMsgBg.SetActive(true);
                    StaticDataManager.instance.modeMsg++;
                    if (StaticDataManager.instance.modeMsg <= 99)
                    {
                        UIManager.instance.modeMsg.text = StaticDataManager.instance.modeMsg.ToString();
                    }
                    else
                    {
                        UIManager.instance.modeMsg.text = "99+";
                    }
                }
                else if (mea.MessageResult.Channel == GetChannelNameClan())
                {
                    UIManager.instance.clanMsgBg.SetActive(true);
                    StaticDataManager.instance.clanMsg++;
                    if (StaticDataManager.instance.clanMsg <= 99)
                    {
                        UIManager.instance.clanMasg.text = StaticDataManager.instance.clanMsg.ToString();
                    }
                    else
                    {
                        UIManager.instance.clanMasg.text = "99+";
                    }
                }
                MessageRecieve(true, chatPayload, mea.MessageResult.Timetoken, false);


            }
            else if (StaticDataManager.IsMineMsg)
            {
                Debug.LogError($"this my massage sms {StaticDataManager.IsMineMsg}");
                StaticDataManager.IsMineMsg = false;
            }
            /* else if(StaticDataManager.instance.chatType == ChatType.Privete)
             {
                 MessageRecieve(true, chatPayload, mea.MessageResult.Timetoken, false);
             }*/
        }
        if (mea.PresenceEventResult != null)
        {
            Debug.Log($"SubscribeCallback in presence { mea.PresenceEventResult.Channel } { mea.PresenceEventResult.Occupancy } { mea.PresenceEventResult.Event}");
        }
        if (mea.SignalEventResult != null)
        {
            Debug.Log("SubscribeCallback in SignalEventResult" + mea.SignalEventResult.Channel + mea.SignalEventResult.Payload);
        }

        if (mea.MessageActionsEventResult != null)
        {
            Debug.Log(mea.MessageActionsEventResult.Channel);
            if (mea.MessageActionsEventResult.Data != null)
            {
                Debug.Log(mea.MessageActionsEventResult.Data.ActionTimetoken);
                Debug.Log(mea.MessageActionsEventResult.Data.ActionType);
                Debug.Log(mea.MessageActionsEventResult.Data.ActionValue);
                Debug.Log(mea.MessageActionsEventResult.Data.MessageTimetoken);
                Debug.Log(mea.MessageActionsEventResult.Data.UUID);
            }
            Debug.Log(mea.MessageActionsEventResult.MessageActionsEvent);
            Debug.Log(mea.MessageActionsEventResult.Subscription);
        }
    }

    private void SubscribeToMyselfAndGroup(string UUID)
    {
        pubnub.Subscribe()
            .Channels(new List<string> { GetChannelNamePrivate(UUID), GetChannelNameGloble(), GetChannelNamePWR_BWR(), GetChannelNameClan() })
            .WithPresence()
            .Execute();

        /* if (StaticDataManager.instance.gameState == GameState.Testing)
             StaticDataManager.deviceToken = "dvKaZa_7SLm_sISchFAxfz:APA91bEJLJHziSXGN0t_Z5EO3VSM2pAco3XCy0XHsU2VWy0Yukw70UPpLuqAumEYxMNSI0rTTWHKox_Ss7Fchik41_WyKi5mDh8QRJXejaXV-lVUd45cjl5ypjWYCYxPJLi7zmWxG3R0";*/

        //AddPushToChannels(UUID, StaticDataManager.deviceToken);
    }

    #region chanelName

    private string GetChannelNamePrivate(string uuid)
    {
        StaticDataManager.instance.privateChannel = "channel_" + uuid;

        return StaticDataManager.instance.privateChannel;

    }

    private string GetChannelNameGloble()
    {
        StaticDataManager.instance.globelChannel = "channel_Globle";
        return StaticDataManager.instance.globelChannel;
    }

    private string GetChannelNamePWR_BWR()
    {
        StaticDataManager.instance.modeChannel = "channel_" + StaticDataManager.instance.ModeGroup;
        return StaticDataManager.instance.modeChannel;
    }

    private string GetChannelNameClan()
    {
        StaticDataManager.instance.clanChannel = "channel_" + StaticDataManager.instance.ClanGroup;
        return StaticDataManager.instance.clanChannel;
    }

    #endregion


    private string GetHybridChannelName(string id1, string id2)
    {
        string hybridChnl = "";
        int i = 0;

        while (true)
        {
            if (id1.ToUpper()[i] != id2.ToUpper()[i])
            {
                if (id1.ToUpper()[i] < id2.ToUpper()[i])
                    hybridChnl = "channel_" + id1 + "_" + id2;
                else
                    hybridChnl = "channel_" + id2 + "_" + id1;
                break;
            }
            i++;
        }
        return hybridChnl;
    }

    public void AddPushToChannels(string UUID, string deviceID)
    {
        pubnub.AddPushNotificationsOnChannels()
              .PushType(PNPushType.GCM)
            .Channels(new List<string> { GetChannelNamePrivate(UUID) })
              .DeviceID(deviceID)
              .Async((result, status) =>
              {
                  if (status.Error)
                  {
                      Debug.LogError(string.Format(
                          "in AddPush Error: {0}, {1}, {2}",
                          status.StatusCode,
                          status.ErrorData.Info,
                          status.Category
                      ));
                  }
                  else
                      Debug.Log("Notification Configured.");
              });
    }

    #region FetachAll Msg
    public void FetchAllMsgForGroups(string id1, string chanelName, ushort numberOfMesseges)
    {
        StaticDataManager.AllChatMessages = new List<PNMessageResult>();

        //string fetchChennal = GetHybridChannelName(id1, id2);

        pubnub.FetchMessages()
               .Channels(new List<string> { chanelName })
               .Count(numberOfMesseges)
               .Async((result, status) =>
               {
                   if (status.Error)
                   {
                       Debug.Log(string.Format(
                           " FetchMessages Error: {0}, {1}, {2}",
                           status.StatusCode,
                           status.ErrorData.Info,
                           status.Category));

                       FetchAllMessageState?.Invoke(false, null);
                   }
                   else
                   {
                       foreach (KeyValuePair<string, List<PNMessageResult>> kvp in result.Channels)
                       {
                           foreach (PNMessageResult pnMessageResult in kvp.Value)
                           {
                               //Debug.LogWarning("Adding user msg : " + pnMessageResult.Payload);
                               StaticDataManager.AllChatMessages.Add(pnMessageResult);
                           }
                       }
                       Debug.Log("Fetch msg count : " + StaticDataManager.AllChatMessages.Count);
                       FetchAllMessageState?.Invoke(true, StaticDataManager.AllChatMessages);
                   }
               });
    }

    public void FetchAllMsgPrivate(string id1, string id2, ushort numberOfMesseges)
    {
        StaticDataManager.AllChatMessages = new List<PNMessageResult>();

        string fetchChennal = GetHybridChannelName(id1, id2);

        pubnub.FetchMessages()
               .Channels(new List<string> { fetchChennal })
               .Count(numberOfMesseges)
               .Async((result, status) =>
               {
                   if (status.Error)
                   {
                       Debug.Log(string.Format(
                           " FetchMessages Error: {0}, {1}, {2}",
                           status.StatusCode,
                           status.ErrorData.Info,
                           status.Category));

                       FetchAllMessageState(false, null);
                   }
                   else
                   {
                       foreach (KeyValuePair<string, List<PNMessageResult>> kvp in result.Channels)
                       {
                           foreach (PNMessageResult pnMessageResult in kvp.Value)
                           {
                               //Debug.LogWarning("Adding user msg : " + pnMessageResult.Payload);
                               StaticDataManager.AllChatMessages.Add(pnMessageResult);
                           }
                       }
                       Debug.Log("Fetch msg count : " + StaticDataManager.AllChatMessages.Count);
                       FetchAllMessageState(true, StaticDataManager.AllChatMessages);
                   }
               });
    }
    #endregion

    public void SendMessageCallForGroup(string UUID_1, string chanelName, Dictionary<string, object> payload)
    {

        pubnub.Publish()
            .Channel(chanelName)
            .Message(payload)
            .Async((result, status) =>
            {
                if (status.Error)
                {
                    Debug.Log(status.Error);
                    Debug.Log(status.ErrorData.Info);
                    MessageSent(false, null, 0, false);
                    //MessageSentForGroups(false, null, 0, false);
                }
                else
                {
                    Debug.Log("Message Sent..." + result.Timetoken);
                    MessageSent(true, payload, result.Timetoken, false);
                    //MessageSentForGroups(true, payload, result.Timetoken, false);
                }
            });
        ChatUIManager.instance.chatInputField.text = "";
        //StaticDataManager.IsMineMsg = false;
    }


    public void SendMessageCall(string UUID_1, string UUID_2, Dictionary<string, object> payload)
    {
        /*foreach (KeyValuePair<string, object> item in payload)
            Debug.Log("Key : " + item.Key + ", Value : " + item.Value);

        Debug.Log("Payload key count : " + payload.Keys.Count);*/

        //string payload = JsonUtility.ToJson(publishMessage);

        Debug.Log(UUID_1 + ",\n" + UUID_2 + ",\n" + payload);

        pubnub.Publish()
            .Channel(GetChannelNamePrivate(StaticDataManager.opponentUUID))
            .Message(payload)
            .Async((result, status) =>
            {
                if (status.Error)
                {
                    Debug.Log(status.Error);
                    Debug.Log(status.ErrorData.Info);
                    MessageSent(false, null, 0, false);
                }
                else
                {
                    Debug.Log("Message Sent..." + result.Timetoken);
                    MessageSent(true, payload, result.Timetoken, false);
                }
            });

        pubnub.Publish()
            .Channel(GetHybridChannelName(StaticDataManager.myUUID, StaticDataManager.opponentUUID))
            .Message(payload)
            .Async((result, status) =>
            {
                if (status.Error)
                {
                    Debug.Log(status.Error);
                    Debug.Log(status.ErrorData.Info);
                }
            });

        ChatUIManager.instance.chatInputField.text = "";
        StaticDataManager.IsMineMsg = false;
    }

    public void GetHistory(string id1, string id2, ushort numberOfMessage)
    {
        if (StaticDataManager.AllChatMessages.Count > 0)
        {
            string historyChannel = GetHybridChannelName(id1, id2);
            pubnub.History()
                .Channel(historyChannel)
                .IncludeTimetoken(true)
                .Count(numberOfMessage)
                .Start(StaticDataManager.AllChatMessages[0].Timetoken)
                .Async((result, status) =>
                {
                    if (status.Error)
                    {
                        Debug.LogError(string.Format(" History Error: {0}, {1}, {2}",
                            status.StatusCode,
                            status.ErrorData,
                            status.Category));
                        HistoryMessageState(false, result.Messages);
                    }
                    else
                    {
                        HistoryMessageState(true, result.Messages);
                    }
                });
        }
    }

    void UnsubscribeChannel(string id) { }
}

}