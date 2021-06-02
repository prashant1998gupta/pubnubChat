using PubNubAPI;
using System.Collections.Generic;
using UnityEngine;

public class SendChatBtnClick : MonoBehaviour
{
    
    public void OnSendChatBtnClick()
    {
        CreatePushPayloadHelper cpph = new CreatePushPayloadHelper();

        /*  Dictionary<string, object> notification = new Dictionary<string, object>(){
                  {"title", StaticDataManager.myName},
                  {"body", ChatUIManager.instance.chatInputField.text},
              };

          PNFCMData fcm = new PNFCMData
          {
              Custom = new Dictionary<string, object>(){
                  {"notification", notification}
              },
          };*/

        Dictionary<string, object> commonPayload = new Dictionary<string, object>()
        {
            {"uuid", StaticDataManager.myUUID},
            {"username", StaticDataManager.myName},
            {"text", ChatUIManager.instance.chatInputField.text},
            {"chatID", StaticDataManager.myUUID + "_" + StaticDataManager.currentUserUUID}
        };

        // Dictionary<string, object> payload = cpph.SetFCMPayload(fcm).SetCommonPayload(commonPayload).BuildPayload();
        Dictionary<string, object> payload = cpph.SetCommonPayload(commonPayload).BuildPayload();


        if (StaticDataManager.instance.chatType == ChatType.Globel)
        {
            PubNubManager.instance.SendMessageCallForGroup(
                StaticDataManager.myUUID,
                StaticDataManager.instance.globelChannel,
                payload);
        }
        else if (StaticDataManager.instance.chatType == ChatType.PWR_BWR)
        {
            PubNubManager.instance.SendMessageCallForGroup(
                StaticDataManager.myUUID,
                StaticDataManager.instance.modeChannel,
                payload);
        }
        else if (StaticDataManager.instance.chatType == ChatType.Clan)
        {
            PubNubManager.instance.SendMessageCallForGroup(
                StaticDataManager.myUUID,
                StaticDataManager.instance.clanChannel,
                payload);
        }
        else if (StaticDataManager.instance.chatType == ChatType.Privete)
        {
            PubNubManager.instance.SendMessageCall(
          StaticDataManager.myUUID,
          StaticDataManager.currentUserUUID,
          payload);
        }

       


      
    }
}