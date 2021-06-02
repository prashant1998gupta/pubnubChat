using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;


    public static event Action<bool> UserStateChange;
    public static event Action OnUserSummitButton;

    public ChatTypeBtnClick chatTypeBtnClick;

    public GameObject MainPanel;
    public Toggle PWR;
    public Toggle BWR;

    public GameObject PWRChackMark;
    public GameObject BWRChackMark;
    public InputField playerName;

    public Dropdown PWRClan;
    public Dropdown BWRClan;

    public Button summit;
    public Text info;

   
  

    public GameObject ButtonPanel;

    public Button Globel;
    public Button PWR_BWR;
    public Button Clan;


    public Button logOut;

   // public Action O

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }


    public void Reset()
    {
        BWRChackMark.SetActive(false);
        PWRChackMark.SetActive(false);
        info.gameObject.SetActive(false);

       StaticDataManager.instance.ClanGroup = "";
        StaticDataManager.instance.ModeGroup = "";
        playerName.text = "";
       /* PWRClan.ClearOptions();
        BWRClan.ClearOptions();*/

        PWRClan.interactable = false;
        BWRClan.interactable = false;
    }

    private void Start()
    {
        BWRChackMark.SetActive(false);
        PWRChackMark.SetActive(false);
        info.gameObject.SetActive(false);

        StaticDataManager.instance.ClanGroup = "";
        StaticDataManager.instance.ModeGroup = "";



        PWRClan.interactable = false;
        BWRClan.interactable = false;

        // BWRClan.gameObject.SetActive(false);

        PWR.onValueChanged.AddListener(OnPWRSelect);
        BWR.onValueChanged.AddListener(OnBWRSelect);

        PWRClan.onValueChanged.AddListener(OnSelectPWRClan);
        BWRClan.onValueChanged.AddListener(OnSelectBWRClan);

        summit.onClick.AddListener(OnSummitButtonClick);
        logOut.onClick.AddListener(OnLogOut);

        PWR_BWR.onClick.AddListener(OnClickPWR_BWE);
        Globel.onClick.AddListener(OnClickGlobel);
        Clan.onClick.AddListener(OnClickClan);

    }

  

    private void OnSelectPWRClan(int arg0)
    {
        StaticDataManager.instance.ClanGroup = PWRClan.options[arg0].text;
        Clan.GetComponentInChildren<Text>().text = StaticDataManager.instance.ClanGroup;


        if (StaticDataManager.instance.ClanGroup == "Choose Clan")
        {
            StaticDataManager.instance.ClanGroup = "";
        }
        else
        {
           
            info.gameObject.SetActive(false);
        }
    }

    private void OnSelectBWRClan(int arg0)
    {
        StaticDataManager.instance.ClanGroup = BWRClan.options[arg0].text;
        Clan.GetComponentInChildren<Text>().text = StaticDataManager.instance.ClanGroup;

        if (StaticDataManager.instance.ClanGroup == "Choose Clan")
        {
            StaticDataManager.instance.ClanGroup = "";
        }
        else
        {

            info.gameObject.SetActive(false);
        }
    }

    private void OnPWRSelect(bool arg0)
    {
        PWR.isOn = true;
        PWRChackMark.SetActive(true);
        BWRChackMark.SetActive(false);

        PWRClan.interactable = true;
        PWRClan.gameObject.SetActive(true);
        BWRClan.gameObject.SetActive(false);

        StaticDataManager.instance.ModeGroup = "PWR";
        PWR_BWR.GetComponentInChildren<Text>().text = StaticDataManager.instance.ModeGroup;
        StaticDataManager.instance.ClanGroup = "";
        info.gameObject.SetActive(false);
    }

    private void OnBWRSelect(bool arg0)
    {
        BWR.isOn = true;
        PWRChackMark.SetActive(false);
        BWRChackMark.SetActive(true);

        BWRClan.interactable = true;
        PWRClan.gameObject.SetActive(false);
        BWRClan.gameObject.SetActive(true);

        StaticDataManager.instance.ModeGroup = "BWR";
        PWR_BWR.GetComponentInChildren<Text>().text = StaticDataManager.instance.ModeGroup;
        StaticDataManager.instance.ClanGroup = "";
        info.gameObject.SetActive(false);
    }

    private void OnSummitButtonClick()
    {


        if((string.IsNullOrEmpty(StaticDataManager.instance.ModeGroup.Trim())) || (string.IsNullOrEmpty(StaticDataManager.instance.ClanGroup.Trim())) || (string.IsNullOrEmpty(playerName.text.Trim())))
        {
            info.gameObject.SetActive(true);
            info.text = "plz select Mode and clan";
            return;
        }
        else
        {
            ButtonPanel.SetActive(true);
            MainPanel.SetActive(false);
            UserStateChange(true);
            StaticDataManager.instance.Name = playerName.text;
            StaticDataManager.myName = playerName.text;
        }

        //OnUserSummitButton();
    }


    private void OnLogOut()
    {
        ButtonPanel.SetActive(false);
        MainPanel.SetActive(true);
        Reset();
        UserStateChange(false);
    }




    private void OnClickClan()
    {
       // UserStateChange(true);
        StaticDataManager.instance.chatType = ChatType.Clan;
        chatTypeBtnClick.ClickOnChatType();

    }
    private void OnClickGlobel()
    {
      //  UserStateChange(true);
        StaticDataManager.instance.chatType = ChatType.Globel;
        chatTypeBtnClick.ClickOnChatType();
    }

    private void OnClickPWR_BWE()
    {
       // UserStateChange(true);
        StaticDataManager.instance.chatType = ChatType.PWR_BWR;
        chatTypeBtnClick.ClickOnChatType();
    }
}
