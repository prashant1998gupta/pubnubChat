using UnityEngine;

namespace ChatPackage
{ 
public class ChatTypeBtnClick : MonoBehaviour
{
    
    public void ClickOnChatType()
    {
        foreach (Transform obj in ChatUIManager.instance.chatContainer)
            Destroy(obj.gameObject);

        ChatUIManager.instance.chatInboxPanel.SetActive(true);

        if(StaticDataManager.instance.chatType == ChatType.Globel || StaticDataManager.instance.chatType == ChatType.PWR_BWR || StaticDataManager.instance.chatType== ChatType.Clan)
        {
           UIManager.instance.ButtonPanel.SetActive(false);
            ChatUIManager.instance.otherUserNameText.text = StaticDataManager.instance.chatType.ToString();
            if (StaticDataManager.instance.chatType == ChatType.Globel)
            {
                PubNubManager.instance.FetchAllMsgForGroups(
                    StaticDataManager.myUUID, 
                    StaticDataManager.instance.globelChannel,
                    StaticDataManager.maxMessagesToDisplay);
            }

            else if (StaticDataManager.instance.chatType == ChatType.PWR_BWR)
            {
                PubNubManager.instance.FetchAllMsgForGroups(
                    StaticDataManager.myUUID,
                    StaticDataManager.instance.modeChannel,
                    StaticDataManager.maxMessagesToDisplay);
            }

            else if (StaticDataManager.instance.chatType == ChatType.Clan)
            {
                PubNubManager.instance.FetchAllMsgForGroups(
                    StaticDataManager.myUUID,
                    StaticDataManager.instance.clanChannel,
                    StaticDataManager.maxMessagesToDisplay);
            }
        }
        else if(StaticDataManager.instance.chatType == ChatType.Privete)
        {
            // ChatUIManager.instance.usersListPanel.SetActive(false);

            ChatUIManager.instance.otherUserNameText.text = GetComponent<UserData>().myName;
            StaticDataManager.opponentUUID = GetComponent<UserData>().myUUID;

            PubNubManager.instance.FetchAllMsgPrivate(
                StaticDataManager.myUUID,
                StaticDataManager.opponentUUID,
                StaticDataManager.maxMessagesToDisplay);
        }

      
    }
}

}
