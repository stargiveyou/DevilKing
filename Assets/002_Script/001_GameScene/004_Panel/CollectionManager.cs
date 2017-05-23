using UnityEngine;
using System.Collections;

public class CollectionManager : MonoBehaviour
{
    public GameObject selectPanel;
    public GameObject collectionPanel;
    public GameObject monsterInfoPanel, trapInfoPanel, infoBGPanel;
    public GameObject DexPanel;
    public GameObject trophyPanel;

    public Transform TapButtonTrs;
    public Transform PanelDisplayTrs;

    public GameObject[] Buttons;

	//    public UIAtlas[] objectAtlas;

	private GameManager GM;
///	private TrophyPanelControl Trophy;


    private enum ObjectTypeEnum
    {
        Normal,
        Boss,
        Trap,
        Enemy,
        NamedEnemy,
    }

    // Use this for initialization
    void Start()
    {
        GM = GameManager.getInstance();
		//Trophy =trophyPanel.GetComponent<TrophyPanelControl> ();


        selectPanel.SetActive(false);

        for (int i = 0; i < Buttons.Length; i++)
        {
            UIEventListener.Get(Buttons[i]).onClick -= new UIEventListener.VoidDelegate(GameButtonProcess);
            UIEventListener.Get(Buttons[i]).onClick += new UIEventListener.VoidDelegate(GameButtonProcess);
        }

        for (int i = 0; i < TapButtonTrs.childCount; i++)
        {
            UIEventListener.Get(TapButtonTrs.GetChild(i).gameObject).onClick -= new UIEventListener.VoidDelegate(TapButtonProcess);
            UIEventListener.Get(TapButtonTrs.GetChild(i).gameObject).onClick += new UIEventListener.VoidDelegate(TapButtonProcess);
        }
        StatusObjCaching();
        ObjectSet();

    }
   
    private void DisplayPanelActive(string tapName)
    {
        for (int i = 0; i < PanelDisplayTrs.childCount; i++)
        {
            PanelDisplayTrs.GetChild(i).gameObject.SetActive(false);
        }

        PanelDisplayTrs.FindChild(tapName + "Panel").gameObject.SetActive(true);

    }



	#region Collection Panel Part

    public Transform statusCollectTrs, trapstatusCollectTrs;

    private UILabel hpLabel, atkLabel, atkSpeedLabel, rangeLabel, sizeLabel;
    private UILabel trap_atkLabel, trap_countLabel, trap_InstallCountLabel, trap_atkTypeLabel;

    private UILabel monsterNameLabel, monsterLevelLabel, monsteratkTypeLabel;


    public UISprite objectDisplaySprite;
    public UILabel DialogLabel;
     void StatusObjCaching()
    {
        monsterNameLabel = infoBGPanel.transform.FindChild("Name").GetComponent<UILabel>();
        monsterLevelLabel = infoBGPanel.transform.FindChild("Level").GetComponent<UILabel>();


        hpLabel = statusCollectTrs.FindChild("HP").GetComponent<UILabel>();
        atkLabel = statusCollectTrs.FindChild("ATK").GetComponent<UILabel>();
        atkSpeedLabel = statusCollectTrs.FindChild("ATKSpeed").GetComponent<UILabel>();
        sizeLabel = statusCollectTrs.FindChild("ConsumeBlock").GetComponent<UILabel>();
        rangeLabel = statusCollectTrs.FindChild("ATKRange").GetComponent<UILabel>();
        monsteratkTypeLabel = statusCollectTrs.FindChild("ATKMethod").GetComponent<UILabel>();


        trap_atkLabel = trapstatusCollectTrs.FindChild("ATK").GetComponent<UILabel>();
        trap_countLabel = trapstatusCollectTrs.FindChild("Count").GetComponent<UILabel>();
        trap_InstallCountLabel = trapstatusCollectTrs.FindChild("InstallCount").GetComponent<UILabel>();
        trap_atkTypeLabel = trapstatusCollectTrs.FindChild("ATKMethod").GetComponent<UILabel>();

    }

	void CollectSelectActive()
	{
		Time.timeScale = 0.0f;
		selectPanel.SetActive(true);
		DisplayPanelActive("Monster");
	}


    public Transform MonsterCollectPanel, BossCollectPanel, TrapCollectPanel, NormalCollectPanel, NamedCollectPanel;

    void ObjectSet()
    {
        UIButton objectButton;
        UISprite objectSprite;
        UILabel objectLabel;
        string[] splitTrsName;
        float factor = 1;
        int monster_index = 0;

        for (int i = 0; i < MonsterCollectPanel.childCount; i++)
        {
            Transform ObjTrs = MonsterCollectPanel.GetChild(i);
            objectButton = ObjTrs.GetComponent<UIButton>();
            objectSprite = ObjTrs.FindChild("objectSprite").GetComponent<UISprite>();
            objectLabel = ObjTrs.FindChild("objectLabel").GetComponent<UILabel>();

            splitTrsName = ObjTrs.name.Split('_');

            monster_index = int.Parse(splitTrsName[1]);


            objectButton.normalSprite = GM.getMonsterName(splitTrsName[0], monster_index, true) + "_head";
            objectSprite.spriteName = GM.getMonsterName(splitTrsName[0], monster_index, true) + "_head";
            objectSprite.MakePixelPerfect();

            if (i == 5 || i == 6 || i == 9 || i == 10 || i == 15 || i == 18)
            {
                factor = 0.8f;
            }
            else if (i == 7 || i == 19)
            {
                factor = 0.7f;
            }
            else if (i == 11)
            {
                factor = 0.65f;
            }
            else
            {
                factor = 1;
            }

            objectSprite.transform.localScale *= factor;
			objectLabel.text = GM.getMonsterName(splitTrsName[0], int.Parse(splitTrsName[1]), true);

            UIEventListener.Get(ObjTrs.gameObject).onClick -= new UIEventListener.VoidDelegate(MonsterButtonProcess);
            UIEventListener.Get(ObjTrs.gameObject).onClick += new UIEventListener.VoidDelegate(MonsterButtonProcess);

        }

        for (int i = 0; i < BossCollectPanel.childCount; i++)
        {
            Transform ObjTrs = BossCollectPanel.GetChild(i);
            objectButton = ObjTrs.GetComponent<UIButton>();
            objectSprite = ObjTrs.FindChild("objectSprite").GetComponent<UISprite>();
            objectLabel = ObjTrs.FindChild("objectLabel").GetComponent<UILabel>();

            splitTrsName = ObjTrs.name.Split('_');
            objectButton.normalSprite = GM.getMonsterName(splitTrsName[0], int.Parse(splitTrsName[1]), true) + "_head";
            objectSprite.spriteName = GM.getMonsterName(splitTrsName[0], int.Parse(splitTrsName[1]), true) + "_head";
            objectSprite.MakePixelPerfect();

            if ((i > 3 && i < 6))
            {
                objectSprite.transform.localPosition = new Vector3(-10, 0, 0);
                objectSprite.transform.localScale *= 0.65f;
            }
            else if (i == 7)
            {
                objectSprite.transform.localScale *= 0.7f;
            }

            objectLabel.text = GM.getMonsterName(splitTrsName[0], int.Parse(splitTrsName[1]), false);

            UIEventListener.Get(ObjTrs.gameObject).onClick -= new UIEventListener.VoidDelegate(MonsterButtonProcess);
            UIEventListener.Get(ObjTrs.gameObject).onClick += new UIEventListener.VoidDelegate(MonsterButtonProcess);

        }

        for (int i = 0; i < TrapCollectPanel.childCount; i++)
        {
            Transform ObjTrs = TrapCollectPanel.GetChild(i);
            objectButton = ObjTrs.GetComponent<UIButton>();

            objectLabel = ObjTrs.FindChild("objectLabel").GetComponent<UILabel>();
            splitTrsName = ObjTrs.name.Split('_');
            objectLabel.text = GM.getContext("Name", splitTrsName[0], int.Parse(splitTrsName[1]));

            UIEventListener.Get(ObjTrs.gameObject).onClick -= new UIEventListener.VoidDelegate(TrapButtonProcess);
            UIEventListener.Get(ObjTrs.gameObject).onClick += new UIEventListener.VoidDelegate(TrapButtonProcess);

        }

        for (int i = 0; i < NormalCollectPanel.childCount; i++)
        {
            Transform ObjTrs = NormalCollectPanel.GetChild(i);
            objectButton = ObjTrs.GetComponent<UIButton>();
            objectSprite = ObjTrs.FindChild("objectSprite").GetComponent<UISprite>();
            objectLabel = ObjTrs.FindChild("objectLabel").GetComponent<UILabel>();

            splitTrsName = ObjTrs.name.Split('_');
            objectButton.normalSprite = GM.getMonsterName(splitTrsName[0], int.Parse(splitTrsName[1]), true) + "_head";
            objectSprite.spriteName = GM.getMonsterName(splitTrsName[0], int.Parse(splitTrsName[1]), true) + "_head";
            objectSprite.MakePixelPerfect();
            if (i == 2)
            {
                objectSprite.transform.localScale *= 0.75f;
            }
            else if (i == 4)
            {
                objectSprite.transform.localScale *= 0.6f;
            }
            objectLabel.text = GM.getMonsterName(splitTrsName[0], int.Parse(splitTrsName[1]), false);

            UIEventListener.Get(ObjTrs.gameObject).onClick -= new UIEventListener.VoidDelegate(MonsterButtonProcess);
            UIEventListener.Get(ObjTrs.gameObject).onClick += new UIEventListener.VoidDelegate(MonsterButtonProcess);

        }

        for (int i = 0; i < NamedCollectPanel.childCount; i++)
        {
            Transform ObjTrs = NamedCollectPanel.GetChild(i);
            objectButton = ObjTrs.GetComponent<UIButton>();
            objectSprite = ObjTrs.FindChild("objectSprite").GetComponent<UISprite>();
            objectLabel = ObjTrs.FindChild("objectLabel").GetComponent<UILabel>();

            splitTrsName = ObjTrs.name.Split('_');
            objectButton.normalSprite = GM.getMonsterName(splitTrsName[0], int.Parse(splitTrsName[1]), true) + "_head";
            objectSprite.spriteName = GM.getMonsterName(splitTrsName[0], int.Parse(splitTrsName[1]), true) + "_head";
            objectSprite.MakePixelPerfect();
            objectSprite.transform.localScale *= 0.65f;
            objectLabel.text = GM.getMonsterName(splitTrsName[0], int.Parse(splitTrsName[1]), false);

            UIEventListener.Get(ObjTrs.gameObject).onClick -= new UIEventListener.VoidDelegate(MonsterButtonProcess);
            UIEventListener.Get(ObjTrs.gameObject).onClick += new UIEventListener.VoidDelegate(MonsterButtonProcess);

        }

    }

    void MonsterButtonProcess(GameObject go)
    {
        PanelActivation("Monster");

        string[] splitTrsName;
        int monster_index;
        string monster_type;
        float hp, atk, speed;
        int range, size;

        hp = atk = speed = 0.0f;
        range = size = 1;

        splitTrsName = go.name.Split('_');
        monster_index = int.Parse(splitTrsName[1]);
        monster_type = splitTrsName[0];

        if (go.tag.Equals("Alias"))
        {
            int monster_unique_identify = 0;

            switch (monster_type)
            {
                case "Skeleton":
                    monster_unique_identify = 0;
                    break;
                case "SkeletonMage":
                    monster_unique_identify = 1;
                    break;
                case "Slime":
                    monster_unique_identify = 2;
                    break;
                case "Golem":
                    monster_unique_identify = 3;
                    break;
                case "Imp":
                    monster_unique_identify = 4;
                    break;
                default:
                    monster_unique_identify = 0;
                    break;
            }
            GM.setCharacterData(splitTrsName[0], monster_index, out hp, out atk, out range, out speed, out size);
            monsteratkTypeLabel.text = GM.getContext("AttackType", "Alias", monster_unique_identify);
            monsterLevelLabel.text = "Lv." + (monster_index + 1).ToString();
        }
        else if (monster_type.Equals("Normal"))
        {
            string stat_name = "Normal";
            switch (GM.getMonsterName(monster_type, monster_index, true))
            {
                case "Magician":
                    stat_name = "Archer";
                    break;
                case "Sheilder":
                    stat_name = "Shield";
                    break;
                default:
                    stat_name = "Normal";
                    break;
            }

            GM.setCharacterData(stat_name, 0, out hp, out atk, out range, out speed, out size);
            monsterLevelLabel.text = "Lv.1";
            monsteratkTypeLabel.text = GM.getContext("AttackType", monster_type, monster_index);
        }
        else
        {
            GM.setCharacterData(GM.getMonsterName(monster_type, monster_index, true), 0, out hp, out atk, out range, out speed, out size);
            monsterLevelLabel.text = "Lv.1";
            monsteratkTypeLabel.text = GM.getContext("AttackType", monster_type, monster_index);
        }

        objectDisplaySprite.spriteName = GM.getMonsterName(monster_type, monster_index, true);
        objectDisplaySprite.MakePixelPerfect();
        objectDisplaySprite.transform.localScale *= 1.5f;
        monsterNameLabel.text = GM.getMonsterName(monster_type, monster_index, false);

        DialogLabel.text = GM.getContext("DexDescript", monster_type, monster_index);


        hpLabel.text = hp.ToString();
        atkLabel.text = atk.ToString();
        atkSpeedLabel.text = speed.ToString();
        rangeLabel.text = range.ToString();
        sizeLabel.text = size.ToString();

    }
    void TrapButtonProcess(GameObject go)
    {
        PanelActivation("Trap");

        string[] splitTrsName;
        int monster_index;
        string monster_type;
        float atk;
        int lastCount, installCount;
        atk = 0.0f;
        lastCount = installCount = 0;


        splitTrsName = go.name.Split('_');
        monster_index = int.Parse(splitTrsName[1]);
        monster_type = splitTrsName[0];
        //공격력, 제한횟수, 설치카운트,
        GM.setTrapData(monster_type, monster_index, out atk, out lastCount, out installCount);

        monsterNameLabel.text = GM.getContext("Name", monster_type, monster_index);
        monsterLevelLabel.text = "Lv." + (monster_index + 1).ToString();
        objectDisplaySprite.spriteName = go.name;
        objectDisplaySprite.MakePixelPerfect();
        if (monster_type.Equals("Spike"))
        {
            objectDisplaySprite.transform.localScale *= 2;
        }

        if(monster_type.Equals("Fire"))
        {
            UISprite SubDisplaySprite = objectDisplaySprite.transform.FindChild("sub_ObjectSprite").GetComponent<UISprite>();
            SubDisplaySprite.gameObject.SetActive(true);
            if(monster_index==0 || monster_index == 1)
            {
                SubDisplaySprite.spriteName = "2_1_2_hole";
                objectDisplaySprite.transform.localPosition = new Vector3(0, -150, 0);
                SubDisplaySprite.transform.localPosition = new Vector3(0, -20, 0);
            }
            else if(monster_index == 2)
            {
                SubDisplaySprite.spriteName = "2_3_hole";
            }
            else if(monster_index == 3)
            {
                SubDisplaySprite.spriteName = "2_4_hole";
            }
            SubDisplaySprite.MakePixelPerfect();
        }


        trap_atkLabel.text = atk.ToString();
        trap_countLabel.text = lastCount.ToString();
        //trap_atkTypeLabel.text = "";
        trap_InstallCountLabel.text = installCount.ToString();
        trap_atkTypeLabel.text = GM.getContext("TrapType", monster_type);
        DialogLabel.text = GM.getContext("DexDescript", monster_type, monster_index);


    }
    void ObjectButtonProcess(GameObject go)
    {

    }

    void PanelActivation(string type)
    {
        switch (type)
        {
            case "Dex":
                DexPanel.SetActive(true);
                infoBGPanel.SetActive(false);
                monsterInfoPanel.SetActive(false);
                trapInfoPanel.SetActive(false);
                break;
            case "Monster":
                DexPanel.SetActive(false);
                infoBGPanel.SetActive(true);
                monsterInfoPanel.SetActive(true);
                trapInfoPanel.SetActive(false);
                break;
            case "Trap":
                DexPanel.SetActive(false);
                infoBGPanel.SetActive(true);
                monsterInfoPanel.SetActive(false);
                trapInfoPanel.SetActive(true);
                break;
        }
    }

	void TapButtonProcess(GameObject go)
    {
        DisplayPanelActive(go.name);
    }

    void GameButtonProcess(GameObject go)
    {

        objectDisplaySprite.transform.FindChild("sub_ObjectSprite").gameObject.SetActive(false);
        objectDisplaySprite.transform.localPosition = new Vector3(0, -180, 0);
        objectDisplaySprite.transform.FindChild("sub_ObjectSprite").localPosition = Vector3.zero;
        selectPanel.SetActive(false);
        string btn_name = go.name;
        switch (btn_name)
        {
            case "TrophyList":
                trophyPanel.SetActive(true);
                break;
            case "CharacterDex":
                collectionPanel.SetActive(true);
                PanelActivation("Dex");
                break;
            case "CancleBtn":
                Time.timeScale = 1.0f;
                break;
            case "BackBtn":
                selectPanel.SetActive(true);

                if (monsterInfoPanel.activeSelf)
                {
                    PanelActivation("Dex");
                    monsterInfoPanel.SetActive(false);
                }
                else if (trapInfoPanel.activeSelf)
                {
                    PanelActivation("Dex");
                    trapInfoPanel.SetActive(false);
                }
                else if (trophyPanel.activeSelf)
                {
                    trophyPanel.SetActive(false);
                }
                else if (collectionPanel.activeSelf)
                {
                    collectionPanel.SetActive(false);
                }
                break;
        }
    }

    public bool CloseCollection
    {
        get
        {
            bool returnValue = false;
            if (monsterInfoPanel.activeSelf)
            {
                PanelActivation("Dex");
                monsterInfoPanel.SetActive(false);
                returnValue = true;
            }
            else if (trapInfoPanel.activeSelf)
            {
                PanelActivation("Dex");
                trapInfoPanel.SetActive(false);
                returnValue = true;
            }
            else if (trophyPanel.activeSelf)
            {
                trophyPanel.SetActive(false);
                returnValue = true;
            }
            else if (collectionPanel.activeSelf)
            {
                collectionPanel.SetActive(false);
                returnValue = true;
            }
            else if(selectPanel.activeSelf)
            {
                returnValue = true;
            }

            if(returnValue)
            {
                selectPanel.SetActive(false);
                Time.timeScale = 1.0f;
            }
            return returnValue;
        }
    }

	#endregion







}

