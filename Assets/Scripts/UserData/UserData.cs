using UnityEngine;
using UnityEngine.UI;

public class UserData : MonoBehaviour
{
    public bool isMine;
    public string myName;
    public string gameModeName;
    public string clanName;
    public string myUUID;


    //public Image statusIcon;
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


        myNameText.text = myName;
        GameModeNameText.text = gameModeName;
        ClanNameText.text = this.clanName;
        myUUIDText.text = myUUID;

        GetComponent<Button>().onClick.AddListener(GetComponent<ChatTypeBtnClick>().ClickOnChatType);
    }
}