using UnityEngine;
using System.Collections;

public class InstallPopUPControl : MonoBehaviour
{

    public Transform Buttons;
    private GameManager GM;
    private const string prefabAddress = "001_Prefab/001_Character/";

    public delegate void Process(GameObject go);


    private void OnEnable()
    {
      
        RefreshSmallButton();
    }
    // Use this for initialization
    void Start()
    {
	    GameObject  smallButton = Buttons.GetChild(0).gameObject;

        UIEventListener.Get(smallButton).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
        UIEventListener.Get(smallButton).onClick += new UIEventListener.VoidDelegate(ButtonProcess);

    }

    public void RefreshSmallButton()
    {
        if (GM == null)
        {
            GM = GameManager.getInstance();
        }
        UILabel monsterLabel = null;
        GameObject smallButton, LockObj;
        UISprite monsterButtonSprite;
        UIButton monsterButton;
        string spriteName;
        int itemLevel = 0;
        int monsterCount = 0;


        int childCount = Buttons.childCount;
      
        StageController _currentStageCtrl = GM.currenetStageController;

        for (int i = 0; i < childCount; i++)
        {
            smallButton = Buttons.GetChild(i).gameObject;

            if (smallButton.tag.Equals("Alias"))
            {
                monsterButton = smallButton.GetComponent<UIButton>();
                LockObj = smallButton.transform.FindChild("InstallLock").gameObject;
                monsterLabel = smallButton.transform.FindChild("monsterPrice").GetComponent<UILabel>();
                monsterLabel.text = "";
                
                itemLevel = GM.LoadMonsterLevelData(smallButton.name);
                monsterCount = GM.getCharacterSize(smallButton.name);
                
                if ((itemLevel == 0) ||
                    installPos != -1 && _currentStageCtrl.isOccupyPos(installPos - 1) && _currentStageCtrl.isOccupyPos(installPos + 1) && monsterCount == 2)
                {
                    LockObj.SetActive(true);
                    UIEventListener.Get(Buttons.GetChild(i).gameObject).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
                }
                
                else
                {
                    spriteName = GM.getMonsterName(smallButton.name, itemLevel - 1, true) + "_head";
                    LockObj.SetActive(false);
                    monsterLabel.text = GM.getPrice(smallButton.name, "install", smallButton.tag).ToString("##") + " G";
                    monsterButtonSprite = smallButton.transform.FindChild("Background").GetComponent<UISprite>();
                    monsterButtonSprite.spriteName = spriteName;
                    monsterButton.normalSprite = spriteName;
                    monsterButtonSprite.MakePixelPerfect();
                    monsterButtonSprite = smallButton.transform.FindChild("InstallLock").GetComponent<UISprite>();
                    monsterButtonSprite.spriteName = spriteName;
                    monsterButtonSprite.MakePixelPerfect();
                    UIEventListener.Get(Buttons.GetChild(i).gameObject).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
                    UIEventListener.Get(Buttons.GetChild(i).gameObject).onClick += new UIEventListener.VoidDelegate(ButtonProcess);
                }
            }
            else if (smallButton.tag.Equals("Obstacle"))
            {
                LockObj = smallButton.transform.FindChild("InstallLock").gameObject;
                monsterLabel = smallButton.transform.FindChild("monsterPrice").GetComponent<UILabel>();
                monsterLabel.text = "";
                itemLevel = GM.LoadTrapLevelData(i - 6);

                if (itemLevel == 0 || !_currentStageCtrl.isTrapInstall(smallButton.name))
                {
                    LockObj.SetActive(true);
                    UIEventListener.Get(Buttons.GetChild(i).gameObject).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
                }
                else
                {
                    LockObj.SetActive(false);
                    monsterLabel.text = GM.getPrice(smallButton.name, "install", smallButton.tag).ToString("00") + " G";
                    UIEventListener.Get(Buttons.GetChild(i).gameObject).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
                    UIEventListener.Get(Buttons.GetChild(i).gameObject).onClick += new UIEventListener.VoidDelegate(ButtonProcess);
                }
            }
            else
            {
                UIEventListener.Get(Buttons.GetChild(i).gameObject).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
                UIEventListener.Get(Buttons.GetChild(i).gameObject).onClick += new UIEventListener.VoidDelegate(ButtonProcess);
            }

        }
    }

    int installPos = -1;

    public int installTile
    {
        set
        {
            installPos = value;
        }
    }

    private const string aliasAddress = "001_Prefab/001_Character/Normal/";
    private const string trapAddress = "001_Prefab/002_Trap/";
    void ButtonProcess(GameObject go)
    {
        StageController _currentStageCtrl = GM.currenetStageController;
        if (go.name.Equals("Back"))
        {
            GM.PopUpOpen("Close");
        }
        else
        {   
            if (go.tag.Equals("Alias"))
            {
                string mosnter_type =go.name;
                if (TempStaticMemory.gold < GM.getPrice(go.name, "install", go.tag))
                {
                    GM.SendToAlert(AlertDialog.NotGold, (GM.getPrice(go.name, "install", go.tag)).ToString("0"));
                }
                else
                {
					string current_monster_name = GM.getMonsterName(mosnter_type, GM.LoadMonsterLevelData(mosnter_type) -1, true);
                    
					GameObject CreateMonster = Instantiate(Resources.Load(aliasAddress +current_monster_name)) as GameObject;
					CreateMonster.name =go.name;
                    PlayerCharacter CreateMonsterInfo = CreateMonster.GetComponent<PlayerCharacter>();
              
					GM.installObject (go.name, go.tag, _currentStageCtrl.StairNumber, installPos, GM.LoadMonsterLevelData(current_monster_name));
                    CreateMonsterInfo.StageCntl = _currentStageCtrl;
                    CreateMonster.transform.parent = _currentStageCtrl.TrsByPos(installPos);
                    CreateMonster.transform.SetAsFirstSibling();
                    CreateMonsterInfo.AliasPosNumber = installPos;
                    CreateMonsterInfo.isBossMonster = false;
					CreateMonsterInfo.CharacterStatus ();

                    TempStaticMemory.gold -= GM.getPrice(go.name, "install", go.tag);
                 
                    if (PlayerPrefs.GetInt("GameCount", 0) == 1)
                    {
                        TempStaticMemory.isTutoProcessOn = true;
                    }
                }
            }
            else if (go.tag.Equals("Obstacle"))
            {
                if (!_currentStageCtrl.isTrapInstall(go.name))
                {

                }
                else if (TempStaticMemory.gold < GM.getPrice(go.name, "install", go.tag))
                {
                    GM.SendToAlert(AlertDialog.NotGold, (GM.getPrice(go.name, "install", go.tag)).ToString("0"));
                }
                else
                {
                    //실제로 함정 사는 부분
                    GameObject CreateTrap = Instantiate(Resources.Load(trapAddress + go.name)) as GameObject;
                    CreateTrap.name = go.name;
                    ObstacleCharacter TrapControl = CreateTrap.GetComponent<ObstacleCharacter>();
                    
                    TrapControl.StageCntl = _currentStageCtrl;
                    CreateTrap.transform.parent = _currentStageCtrl.TrsByPos(installPos);
                    CreateTrap.transform.SetAsFirstSibling();

					TrapControl.setTrapStair = _currentStageCtrl.StairNumber;
					TrapControl.setTrapPos = installPos;

					TrapControl.Initialize ();

                    _currentStageCtrl.OccupyPos(installPos, 1);
                    _currentStageCtrl.trapInstall(go.name,true);

					GM.installObject (go.name, go.tag, _currentStageCtrl.StairNumber, installPos,GM.LoadTrapLevelData(GM.trapIndex(go.name)));

                    TempStaticMemory.gold -= GM.getPrice(go.name, "install", go.tag);
                    
                    if (PlayerPrefs.GetInt("GameCount", 0) == 1)
                    {
                        TempStaticMemory.isTutoProcessOn = true;
                    }
                }
            }
            GM.PopUpOpen("Close");
        }
        _currentStageCtrl.SendMessage("ArrowDisappear", SendMessageOptions.DontRequireReceiver);
        Time.timeScale = 1.0f;
        installPos = -1;
    }

    void UnLockButton(string button_name)
    {
        GameObject objButton = Buttons.FindChild(button_name).gameObject;
        objButton.transform.FindChild("InstallLock").gameObject.SetActive(false);
        UIEventListener.Get(objButton).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
        UIEventListener.Get(objButton).onClick += new UIEventListener.VoidDelegate(ButtonProcess);
    }
}
