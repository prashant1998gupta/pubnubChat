using UnityEngine;
using UnityEngine.UI;


namespace ChatPackage
{ 
public class UserData : MonoBehaviour
{
    public bool isMine;
    public string myName;
    public string gameModeName;
    public string clanName;
    public string myUUID;
    public int totalMsgis;

    public Image profileImaga;
    public Image statusIcon;
    public Text totalMsg;
    public Text myNameText;
    public Text GameModeNameText;
    public Text ClanNameText;
    public Text myUUIDText;

    public void Init(bool isMine ,string name, string modeName , string clanName ,  string UUID)
    {
        this.isMine = isMine;
        myName = name;
        gameModeName = modeName;
        this.clanName = clanName;
        myUUID = UUID;
        //totalMsgis = totalMsg;


        myNameText.text = myName;
        GameModeNameText.text = gameModeName;
        ClanNameText.text = this.clanName;
        myUUIDText.text = myUUID;
        this.totalMsg.text = totalMsgis.ToString();

        GetComponent<Button>().onClick.AddListener(GetComponent<ChatTypeBtnClick>().ClickOnChatType);
    }
}
}