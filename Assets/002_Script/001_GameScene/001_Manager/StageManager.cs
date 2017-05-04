﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class StageManager : MonoBehaviour
{
    private GameManager GM;

    private const string StageAddr = "001_Prefab/Game/Stage";

    private UIGrid StageGrid;

    private int displayStage = 0;
    private int opendedStage = 0;
    // private ArrayList Stages;

    private List<StageController> stageCtrlList;
    private List<string> StageNameList;

    public UIAtlas FinalMap1, FinalMap2;

    private void Awake()
    {
        stageCtrlList = new List<StageController>();

        StageController stageCtrl = null;
        StageGrid = transform.FindChild("GRID").GetComponent<UIGrid>();

        int length = StageGrid.transform.childCount;
        int stairLevel = PlayerPrefs.GetInt("TopStage", 1);
        for (int i = 0; i < length; i++)
        {
            GameObject _Stage = StageGrid.transform.GetChild(i).gameObject;
            _Stage.name = "Stage_" + i;
            stageCtrl = _Stage.GetComponent<StageController>();
            stageCtrl.StairNumber = i;
            stageCtrl.SendMessage("TrapCountDisplay", stairLevel, SendMessageOptions.DontRequireReceiver);
            stageCtrlList.Add(stageCtrl);
        }

    }
    void Start()
    {
        GM = GameManager.getInstance();

        AllStageTrapCountSet();
    }

    private string[] namedCharacterName
        = {
        "Kelberos","Gripon","Cyclopse","Basilisk",
        "Minotauros","Kimera","Archedemon","Dragon","Death"
    };

    void CreateUpStair()
    {
        CreateBossMonsterByLevel(opendedStage);
        //Save
        char[] stageCurrentData = PlayerPrefs.GetString("castleLevel", TempStaticMemory.initStageLevel).ToCharArray();
        stageCurrentData[opendedStage] = '3';

        if (opendedStage < stageCurrentData.Length - 1)
        {
            GameObject Upstair = stageCtrlList[++opendedStage].gameObject;

            int Random_ruin_number = UnityEngine.Random.Range(1, 4);
            UISprite StageBackImg = Upstair.transform.FindChild("Sprite").GetComponent<UISprite>();
            int Random_normal_num = UnityEngine.Random.Range(1, 3);
            StageBackImg.spriteName = "Normal_" + Random_normal_num.ToString("#");
            StageNameList.Add(StageBackImg.spriteName);

            PlayerPrefs.SetInt("TopStage", opendedStage + 1);
            TempStaticMemory.openStageCount = opendedStage + 1;
            stageCurrentData[opendedStage] = '2';
            stageCtrlList[opendedStage].ForceunLock();

            ObjectUpdate(opendedStage + 1);

            PlayerPrefs.SetString("castleLevel", new string(stageCurrentData));
            SaveStageNameList();
        }
    }
    void RemoveDownStair()
    {
        if (opendedStage > 0)
        {
            char[] stageCurrentData = PlayerPrefs.GetString("castleLevel", TempStaticMemory.initStageLevel).ToCharArray();
            stageCurrentData[opendedStage] = '0';
            PlayerPrefs.SetInt("TopStage", opendedStage - 1);
            TempStaticMemory.openStageCount = opendedStage - 1;
            PlayerPrefs.SetString("castleLevel", new string(stageCurrentData));
            SaveStageNameList();
        }
    }
    void ObjectUpdate(int stairLevel)
    {
        for (int i = 0; i < stageCtrlList.Count; i++)
        {
            stageCtrlList[i].SendMessage("TrapCountDisplay", stairLevel, SendMessageOptions.DontRequireReceiver);
        }
    }

    void EnhanceStageLevel(int stage)
    {
		char[] stageData = PlayerPrefs.GetString("castleLevel", TempStaticMemory.initStageLevel).ToCharArray();
        UISprite StageBackImg = stageCtrlList[stage - 1].transform.FindChild("Sprite").GetComponent<UISprite>();
        char stageCurrentData = stageData[stage - 1];
        Debug.Log(stage + " is Enhance and data" + stageCurrentData);
        if (stageCurrentData == '1')
        {
            //Normal
            int Random_normal_num = UnityEngine.Random.Range(1, 3);
            StageBackImg.spriteName = "Normal_" + Random_normal_num.ToString("#");
            StageNameList[stage - 1] = "Normal_" + Random_normal_num.ToString("#");
            stageCtrlList[stage - 1].SendMessage("SetName", "폐허");
            stageData[stage - 1] = '2';
        }
        else if (stageData[stage - 1] == '2')
        {
            if (stage <= 8)
            {
                StageBackImg.atlas = FinalMap1;
            }
            else
            {
                StageBackImg.atlas = FinalMap2;
            }
            stageCtrlList[stage - 1].SendMessage("SetName", GM.getContext("Stage", stage - 1));
            StageBackImg.spriteName = "Stage" + stage.ToString("#");
            StageNameList[stage - 1] = "Stage" + stage.ToString("#");
            stageData[stage - 1] = '3';
        }
        SaveStageNameList();
        PlayerPrefs.SetString("castleLevel", new string(stageData));
    }



    /// <summary>
    /// UnForce Stage , new BossMonster floor Set , old BossMonster LevelUP
    /// </summary>
    /// <param name="level"></param>
    void CreateBossMonsterByLevel(int level)
    {
        Debug.Log("Prev Current Stage" + level);
        stageCtrlList[level].SendMessage("ExitGateChange");

        Transform moveObjectTrs = null;
        PlayerCharacter objectControl = null;
        GM = GameManager.getInstance();
        if (level < 9)
        {
            int monsterCounter = level < 9 ? level : 8;
            Debug.Log(this.GetType() + "// stage Level "+ level.ToString("0"));
			for (int i = 0; i <=monsterCounter ; i++)
            {
                //GM.LevelUpData(GM.getContext("BossMonster", i), GM.LoadLevelData(GM.getContext("BossMonster", i)) + 1);
				GM.LevelReplaceData(GM.getContext("BossMonster", i), GM.LoadLevelData(GM.getContext("BossMonster", i)) + 1);
            }

            GameObject NamedMonster = Instantiate(Resources.Load("001_Prefab/001_Character/Boss/" + GM.getContext("BossMonster", level))) as GameObject;
            NamedMonster.name = GM.getContext("BossMonster", level);

            NamedMonster.transform.parent = stageCtrlList[level].MonsterPos;
            NamedMonster.transform.SetAsFirstSibling();
            
            stageCtrlList[level].setStageBossmonster();
            int characterSize = GM.getCharacterSize(NamedMonster.name);

            for (int i = 9; i >= 10 - characterSize; i--)
            {
                if (stageCtrlList[level].isOccupyPos(i))
                {
                    stageCtrlList[level].NonOccupyPos(i, characterSize);
                    moveObjectTrs = stageCtrlList[level].Bottom.transform.GetChild(i).GetChild(0);
                    moveObjectTrs.parent = stageCtrlList[level + 1].Bottom.transform.GetChild(i);
                    moveObjectTrs.SetAsFirstSibling();
                    objectControl = moveObjectTrs.GetComponent<PlayerCharacter>();
					if (objectControl != null) {
						objectControl.StageCntl = stageCtrlList[level + 1];
						objectControl.SendMessage("CharacterStatus", moveObjectTrs.name, SendMessageOptions.DontRequireReceiver);
					}
                    
                    if (moveObjectTrs != null)
                    {
                        Debug.Log("Move");
                    }
                }
            }

			for (int i = 0; i < monsterCounter-1; i++)
			{
				stageCtrlList[level].updateReNewBossmonster();
			}
            moveObjectTrs = null;
            objectControl = null;
            
            GM.SendMessage("SetHeroTrs", stageCtrlList[level + 1].EndPos);
        }
        else
        {
            for (int i = 0; i <= 9; i++)
            {
				GM.LevelReplaceData(GM.getContext("BossMonster", i), GM.LoadLevelData(GM.getContext("BossMonster", i)) + 1);
            }
        }
    }

    /// <summary>
    /// First Game , Load BossMonster Data and Player Set Floor
    /// </summary>
    /// <param name="level"></param>
    void LoadBossMonsterByLevel(int level)
    {
        Debug.Log("Load Current Stage");

        stageCtrlList[level].SendMessage("ExitGateChange");
      
        int monsterCounter = level < 9 ? level : 8;
        
        GameObject NamedMonster = Instantiate(Resources.Load("001_Prefab/001_Character/Boss/" + GM.getContext("BossMonster", level))) as GameObject;
        NamedMonster.name = GM.getContext("BossMonster", level);

        NamedMonster.transform.parent = stageCtrlList[level].MonsterPos;
        NamedMonster.transform.SetAsFirstSibling();
        
      
        int characterSize = GM.getCharacterSize(NamedMonster.name);
        stageCtrlList[level].setStageBossmonster();
        GM.SendMessage("SetHeroTrs", stageCtrlList[level + 1].EndPos);

    }


    void AllStageTrapCountSet()
    {
        int spikeInstall, stoneInstall, fireInstall;
        spikeInstall = GM.getTrapInstallCount("Spike");
        fireInstall = GM.getTrapInstallCount("Fire");
        stoneInstall = GM.getTrapInstallCount("Stone");
        for (int i = 0; i < stageCtrlList.Count; i++)
        {
            stageCtrlList[i].TrapInstallCountInitialize(spikeInstall, fireInstall, stoneInstall);
        }
    }


    // Use this for initialization
    void GameStartRecieveSignal()
    {
        string StageData = PlayerPrefs.GetString("castleName");

        if (TempStaticMemory.gameCount == 1)
        {
            stageCtrlList[0].SendMessage("SetName", "Prologue");
        }

        if (string.IsNullOrEmpty(StageData))
        {
            Debug.Log("First Data");
            TempStaticMemory.openStageCount = 1;
            StageNameList = new List<string>();
            PlayerPrefs.SetInt("TopStage", 1);
            for (int i = 0; i < 10; i++)
            {
                UISprite StageBackImg = stageCtrlList[i].transform.FindChild("Sprite").GetComponent<UISprite>();
                int Random_ruin_number = UnityEngine.Random.Range(1, 4);
                StageBackImg.spriteName = "Ruin_" + Random_ruin_number.ToString("#");
                StageNameList.Add(StageBackImg.spriteName);
            }
            SaveStageNameList();
        }
        else
        {

            char[] stageCurrentData = PlayerPrefs.GetString("castleLevel", TempStaticMemory.initStageLevel).ToCharArray();
            LoadStageNameList();
            TempStaticMemory.openStageCount = PlayerPrefs.GetInt("TopStage", 1);
            opendedStage = PlayerPrefs.GetInt("TopStage", 1) - 1;
            //Load
            for (int i = 0; i < opendedStage; i++) LoadBossMonsterByLevel(i);

            for (int i = 0; i < 10; i++)
            {
                UISprite StageBackImg = stageCtrlList[i].transform.FindChild("Sprite").GetComponent<UISprite>();
                if (stageCurrentData[i] > '0')
                {
                    stageCtrlList[i].ForceunLock();
                }

                if (stageCurrentData[i] == '3')
                {
                    if (i < 8)
                    {
                        StageBackImg.atlas = FinalMap1;
                    }
                    else
                    {
                        StageBackImg.atlas = FinalMap2;
                    }
                    stageCtrlList[i].SendMessage("SetName", GM.getContext("Stage", i));
                }
                StageBackImg.spriteName = StageNameList[i];
            }
        }
        Transform Trs = stageCtrlList[opendedStage].EndPos;
        GM.SendMessage("SetHeroTrs", Trs);
    }

    void tutorialArrowAppear()
    {
        for (int i = 1; i < 11; i++)
        {
            if (!stageCtrlList[0].isOccupyPos(i))
            {
                stageCtrlList[0].SendMessage("ArrowAppear", i, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    void tutorialArrowAllDisAppear(int clickedIndex)
    {
        stageCtrlList[0].SendMessage("ArrowDisappear", SendMessageOptions.DontRequireReceiver);
        stageCtrlList[0].SendMessage("ArrowAppear", clickedIndex, SendMessageOptions.DontRequireReceiver);
    }


    void GameEndReceiveSignal()
    {
        char[] stageCurrentData = PlayerPrefs.GetString("castleLevel", TempStaticMemory.initStageLevel).ToCharArray();
        LoadStageNameList();
        stageCurrentData[TempStaticMemory.openStageCount - 1] = '0';
        TempStaticMemory.openStageCount--;
        PlayerPrefs.SetInt("TopStage", TempStaticMemory.openStageCount);

    }

    void MoveStageAndFocus(int value)
    {
        int nextStage = displayStage + value;
        int firstStage = 0;
        int lastStage = stageCtrlList.Count - 1;

        GameObject ViewStage = null;

        if (nextStage < 0)
        {
            displayStage = firstStage;
            Debug.Log("First Stage Center");
        }
        else if (nextStage >= stageCtrlList.Count)
        {
            displayStage = lastStage;
            Debug.Log("Last Stage Center");
        }
        else
        {
            displayStage = nextStage;
        }

        ViewStage = stageCtrlList[displayStage].gameObject;
        ViewStage.GetComponent<UICenterOnClick>().CenterOn();

    }

    void SaveStageNameList()
    {
        BinaryFormatter B_Fomatter = new BinaryFormatter();
        MemoryStream M_Stream = new MemoryStream();

        B_Fomatter.Serialize(M_Stream, StageNameList);

        PlayerPrefs.SetString("castleName", Convert.ToBase64String(M_Stream.GetBuffer()));
    }

    void LoadStageNameList()
    {
        string StageData = PlayerPrefs.GetString("castleName");

        BinaryFormatter B_Fomatter = new BinaryFormatter();
        MemoryStream M_Stream = new MemoryStream(Convert.FromBase64String(StageData));

        StageNameList = (List<string>)B_Fomatter.Deserialize(M_Stream);
    }

    // Update is called once per frame

    public StageController FindRespawnByLevel(int level)
    {
        return stageCtrlList[level];
    }

    public int viewStageIndex
    {
        get
        {
            return displayStage;
        }
    }
}
