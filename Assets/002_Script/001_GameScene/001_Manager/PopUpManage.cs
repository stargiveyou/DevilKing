using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

public class PopUpManage : MonoBehaviour
{

    public GameObject ButtonCollect;
    public Transform CancleButtonTrs;
    public Transform ArrowTrs;
    GameObject Display;
    private GameObject EnhancePanel;
    private GameObject OptionPanel;
    private GameObject GameEndPanel;
    private GameObject ExitPanel;
    private InstallPopUPControl installControl;
    
    public GameObject BottomMainButton;
    private GameObject BottomInstallButton;
    
    private UISprite endPanelSprite;

    private bool isTutoSystem;

    // Use this for initialization

    void Start()
    {

        isTutoSystem = PlayerPrefs.GetInt("GameCount", 0) == 1;

        EnhancePanel = transform.FindChild("EnchancePopUp").gameObject;
        OptionPanel = transform.FindChild("OptionPanel").gameObject;
        ExitPanel = transform.FindChild("ExitPanel").gameObject;
        BottomInstallButton = transform.FindChild("InstallPopUp").gameObject;
        GameEndPanel = transform.FindChild("GameEndPanel").gameObject;
        installControl = BottomInstallButton.GetComponent<InstallPopUPControl>();

        //Game End Panel Initialize
        endPanelSprite = GameEndPanel.transform.FindChild("panelSprite").GetComponent<UISprite>();
        endPanelSprite.spriteName = PlayerPrefs.GetString("PlayerSprite","King")+"_dead";
        endPanelSprite.MakePixelPerfect();
        endPanelSprite.transform.localScale = new Vector3(2, 2, 2);

    }

    private void InstallPopUpOpen(int number)
    {
        TempStaticMemory.isOpenPopUp = true;
        BottomInstallButton.SetActive(true);
        installControl.installTile = number;
        installControl.SendMessage("RefreshSmallButton", SendMessageOptions.DontRequireReceiver);
        BottomMainButton.SetActive(false);
        
    }
    
    void PopUpOpen(string popName)
    {
        Time.timeScale = 0.0f;

        TempStaticMemory.isOpenPopUp = true;
        if (popName.Equals("Enchance"))
        {
            if (isTutoSystem)
            {
                TutorialProcess(1);
            }
            
            EnhancePanel.SetActive(true);
            ButtonCollect = EnhancePanel.transform.FindChild("EnchanceButtons").gameObject;
        }
        else if (popName.Equals("Option"))
        {
            OptionPanel.SetActive(true);
            ButtonCollect = OptionPanel.transform.FindChild("OptionButtons").gameObject;
        }
        else if(popName.Equals("GameLoseEnd"))
        {
            GameEndPanel.SetActive(true);
            ButtonCollect = GameEndPanel.transform.FindChild("EndButtons").gameObject;
        }
        else if (popName.Equals("Exit"))
        {
			if (!isTutoSystem)
			{
				ExitPanel.SetActive(true);
			}
        }

        if (ButtonCollect != null)
        {
            int length = ButtonCollect.transform.childCount;
            for (int i = 0; i < length; i++)
            {
                UIEventListener.Get(ButtonCollect.transform.GetChild(i).gameObject).onClick -= new UIEventListener.VoidDelegate(PopUpButtonProcess);
                UIEventListener.Get(ButtonCollect.transform.GetChild(i).gameObject).onClick += new UIEventListener.VoidDelegate(PopUpButtonProcess);
            }
        }

    }

    void PopUpButtonProcess(GameObject go)
    {
        string _button_name = go.name;

        switch (_button_name)
        {
            case "Cancle":
                if (!isTutoSystem) {
                    CancleButtonTrs.localPosition = new Vector3(711, 600, 0);
                    if (Display != null)
                    {
                        Display.SetActive(false);
                        Display = null;
                    }
                    else
                    {
                        go.transform.parent.parent.gameObject.SetActive(false);
                        Time.timeScale = 1.0f;
                    }
                }
                TempStaticMemory.isOpenPopUp = false;
                break;
            case "Castle":
                if (isTutoSystem)
                {
                    TutorialProcess(2);
                }
                //CancleButtonTrs.localPosition = new Vector3(711, 580, 0);
				CancleButtonTrs.localPosition = new Vector3(711, 625, 0);
                Display = transform.FindChild(_button_name + "EnchancePopUP").gameObject;
                Display.SetActive(true);
                Display.GetComponent<CastleEnhanceManager>().SendMessage("initCastleButtonData", SendMessageOptions.DontRequireReceiver);
                break;
            case "Monster":
                //case "Trap":
                //case "Monster_Trap":
                //case "NamedMonster":
                CancleButtonTrs.localPosition = new Vector3(711, 660, 0);
                Display = transform.FindChild(_button_name + "EnchancePopUP").gameObject;
                Display.SetActive(true);
                break;
            case "EndCloseBtn":
                Time.timeScale = 1.0f;
                SceneManager.LoadScene("Game");

                break;
            default:
                break;

        }
    }

    private void TutorialProcess(int index)
    {
        switch(index )
        {
            case 1:
                ArrowTrs.localPosition = new Vector2(0, 0);
                break;
            case 2:
                ArrowTrs.localPosition = new Vector2(575, 50);
                break;
            case 3:
                ArrowTrs.gameObject.SetActive(false);
                if(Display !=null)
                {
                    Display.SetActive(false);
                    Display = null;
                }
                Time.timeScale = 1.0f;
                transform.FindChild("EnchancePopUp").gameObject.SetActive(false);

                TempStaticMemory.isTutoProcessOn = true;

                break;
        }
    }



    void PopUpClose()
    {
        CancleButtonTrs.localPosition = new Vector3(711, 580, 0);
        int length = transform.childCount;
        for (int i = 0; i < length; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        Time.timeScale = 1.0f;
        BottomMainButton.SetActive(true);
        BottomInstallButton.SetActive(false);
        StartCoroutine("delayStop");
    }
    IEnumerator delayStop()
    {
        yield return new WaitForSeconds(0.3f);
        TempStaticMemory.isOpenPopUp = false;
    }
    
    public bool ClosePopUp
    {
        get
        {
            if (EnhancePanel.activeSelf || OptionPanel.activeSelf || ExitPanel.activeSelf)
            {
                CancleButtonTrs.localPosition = new Vector3(711, 580, 0);
                if (Display != null)
                {
                    Display.SetActive(false);
                    Display = null;
                }
                else
                {
                    PopUpClose();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /*
    void LateUpdate()
    {
        if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
        {

            if (EnhancePanel.activeSelf || OptionPanel.activeSelf || ExitPanel.activeSelf)
            {
                CancleButtonTrs.localPosition = new Vector3(711, 580, 0);
                if (Display != null)
                {
                    Display.SetActive(false);
                    Display = null;
                }
                else
                { 
                PopUpClose();
                }
            }
            else
            {
                PopUpOpen("Exit");
            }
        }
    }*/
}
