using UnityEngine;
using UnityEngine.UI;

public class UserData : MonoBehaviour
{
    public bool isMine;
    public string myName;
    public string myUUID;
    public Image statusIcon;
    public Text myNameText;
    public Text myUUIDText;

    public void Init(string name, string UUID)
    {
        myName = name;
        myUUID = UUID;

        myNameText.text = myName;
        myUUIDText.text = myUUID;

        GetComponent<Button>().onClick.AddListener(GetComponent<ChatTypeBtnClick>().ClickOnChatType);
    }
}