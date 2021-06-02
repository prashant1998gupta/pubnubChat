using System;
using PubNubAPI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChatInboxManager : MonoBehaviour
{
    public static ChatInboxManager instance;
    public Color32 myMessage, otherMessage;

    private void Awake() => instance = this;

    private void OnEnable()
    {
        PubNubManager.FetchAllMessageState += FetchAllMessageCallback;
        PubNubManager.HistoryMessageState += HistoryMessageCallback;
        PubNubManager.MessageSent += CreateChat;
        PubNubManager.MessageRecieve += CreateChat;

       // PubNubManager.MessageSentForGroups += CreateChatForGroup;
       // PubNubManager.MessageRecieveForGroup += CreateChatForGroup;
    }
    private void OnDisable()
    {
        PubNubManager.FetchAllMessageState -= FetchAllMessageCallback;
        PubNubManager.HistoryMessageState -= HistoryMessageCallback;
        PubNubManager.MessageSent -= CreateChat;
        PubNubManager.MessageRecieve -= CreateChat;

       // PubNubManager.MessageSentForGroups -= CreateChatForGroup;
       // PubNubManager.MessageRecieveForGroup -= CreateChatForGroup;
    }

    private void FetchAllMessageCallback(bool state, List<PNMessageResult> AllChatMessages)
    {
        if (state)
        {
            if (AllChatMessages.Count > 0)
            {
                //Debug.Log("Message Fetched...." + AllChatMessages.Count);
                AllChatMessages.Sort((MsgRessult1, MsgRessult2) => MsgRessult1.Timetoken.CompareTo(MsgRessult2.Timetoken));

                foreach (PNMessageResult pnMessageResult in AllChatMessages)
                {
                    //Debug.Log("message list : " + pnMessageResult.Payload.ToString());
                    // Format data into readable format
                    Dictionary<string, object> payload = (Dictionary<string, object>)pnMessageResult.Payload;

                    /*foreach (KeyValuePair<string, object> item in payload)
                        Debug.Log("Key : " + item.Key + ", Value : " + item.Value);*/
                    if(StaticDataManager.instance.chatType == ChatType.Privete)
                    {

                        CreateChat(true, payload, pnMessageResult.Timetoken, false);
                    }
                    else
                    {
                         CreateChat(true, payload, pnMessageResult.Timetoken, false);
                        //CreateChatForGroup(true, payload, pnMessageResult.Timetoken, false);
                    }

                }
            }
        }
        else
        {
            Debug.LogError("Unabe to fetch messeges...");
        }
    }

    private void CreateChatForGroup(bool state, Dictionary<string, object> payLoad, long timeToken, bool isHistory)
    {
        if (state)
        {
            string[] IDs;
            bool isMine = false;
            IDs = payLoad["chatID"].ToString().Split('_');
            if (IDs[0] == StaticDataManager.myUUID)
            {
                isMine = true;
            }
            else
            {
                isMine = false;
            }

            ChatUIManager.instance.chatContainer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            //Debug.Log("In here");
            double messageTimestamp = timeToken / 10000000;
            string messageTime = "";
            //Debug.Log("Time : " + messageTimestamp);

            if (timeToken <= 0)
                messageTime = DateTime.Now.ToString("hh:mm tt");
            else
                messageTime = UnixTimeStampToDateTime(messageTimestamp);

            GameObject chatBox = Instantiate(
                      ChatUIManager.instance.chatPrefab,
                      ChatUIManager.instance.chatContainer);

           


            if (isMine)
                {
                    //Debug.Log("is mine");
                    chatBox.GetComponent<Image>().color = myMessage;
                    chatBox.GetComponent<MessageData>().myName.alignment = (TextAnchor)TextAlignment.Right;
                }
                else
                {
                    //Debug.Log("is other");
                    chatBox.GetComponent<Image>().color = otherMessage;
                    chatBox.GetComponent<MessageData>().myName.alignment = (TextAnchor)TextAlignment.Left;
                }

            /*Debug.Log("Payload data : " + payLoad["username"].ToString() +
                "\n" + payLoad["text"].ToString() +
                "\n" + messageTime);*/



            chatBox.GetComponent<MessageData>().myName.text = payLoad["username"].ToString();
            chatBox.GetComponent<MessageData>().message.text = payLoad["text"].ToString();
            chatBox.GetComponent<MessageData>().time.text = messageTime;

        }
        else
        {
            Debug.LogError("Failed to send message..");
        }
    }


    private void CreateChat(bool state, Dictionary<string, object> payLoad, long timeToken, bool isHistory)
    {
        if (state)
        {
            string[] IDs;
            string otherUUID;
            bool isMine = false;
            IDs = payLoad["chatID"].ToString().Split('_');
            if (IDs[0] == StaticDataManager.myUUID)
            {
                isMine = true;
                otherUUID = IDs[1];
                Debug.Log($"this is my uuid {otherUUID}");
            }
            else
            {
                isMine = false;
                otherUUID = IDs[0];
                Debug.Log($"this is my uuid {otherUUID}");
            }

            if (otherUUID == StaticDataManager.currentUserUUID)
            {
                //Debug.Log("In here");
                double messageTimestamp = timeToken / 10000000;
                string messageTime = "";
                //Debug.Log("Time : " + messageTimestamp);

                if (timeToken <= 0)
                    messageTime = DateTime.Now.ToString("hh:mm tt");
                else
                    messageTime = UnixTimeStampToDateTime(messageTimestamp);

                GameObject chatBox = Instantiate(
                      ChatUIManager.instance.chatPrefab,
                      ChatUIManager.instance.chatContainer);
                //Debug.Log("Box gen");

                if (isHistory)
                {
                    //Debug.Log("is History");
                    chatBox.transform.SetAsFirstSibling();
                }

                ChatUIManager.instance.chatContainer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                if (isMine)
                {
                    //Debug.Log("is mine");
                    chatBox.GetComponent<Image>().color = myMessage;
                    chatBox.GetComponent<MessageData>().myName.alignment = (TextAnchor)TextAlignment.Right;
                }
                else
                {
                    //Debug.Log("is other");
                    chatBox.GetComponent<Image>().color = otherMessage;
                    chatBox.GetComponent<MessageData>().myName.alignment = (TextAnchor)TextAlignment.Left;
                }

                Debug.Log("Payload data : " + payLoad["username"].ToString() +
                    "\n" + payLoad["text"].ToString() +
                    "\n" + messageTime);

                chatBox.GetComponent<MessageData>().myName.text = payLoad["username"].ToString();
                chatBox.GetComponent<MessageData>().message.text = payLoad["text"].ToString();
                chatBox.GetComponent<MessageData>().time.text = messageTime;
            }
        }
        else
        {
            Debug.LogError("Failed to send message..");
        }
    }


    private void HistoryMessageCallback(bool state, List<PNHistoryItemResult> result)
    {
        if (state)
        {
            result.Reverse();
            foreach (PNHistoryItemResult histItem in result)
            {
                PNMessageResult tempResult = new PNMessageResult("", "", histItem.Entry,
                   histItem.Timetoken, histItem.Timetoken, null, "");

                Dictionary<string, object> chatmessage = (Dictionary<string, object>)tempResult.Payload;

                StaticDataManager.AllChatMessages.Insert(0, tempResult);


                if(StaticDataManager.instance.chatType == ChatType.Privete)
                {
                    CreateChat(true, chatmessage, histItem.Timetoken, true);
                }
                else
                {
                    CreateChat(true, chatmessage, histItem.Timetoken, true);
                    //CreateChatForGroup(true, chatmessage, histItem.Timetoken, true);
                }

            }
        }
        else
            Debug.LogError("Failed to get History");
    }

    private static string UnixTimeStampToDateTime(double unixTimeStamp)
    {
        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();

        //Debug.LogError("Time in UI :- " + dtDateTime.ToString("hh:mm tt"));

        return dtDateTime.ToString("hh:mm tt");
    }
}