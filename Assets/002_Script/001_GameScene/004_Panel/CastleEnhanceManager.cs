using UnityEngine;
using System.Collections;
using System;
public class CastleEnhanceManager : MonoBehaviour
{
    
    private GameManager GM;
	private LevelDataBase LevelDB;


    //private char[] castleData;
    private int level;
    private ArrayList AbleButtonList;
    private UILabel DoorGoldLabel;
    private UILabel EnemyLevelLabel;

    public GameObject CastleGrid;
    public UILabel GateLevelLabel;
    
	void Awake()
	{
		AbleButtonList = new ArrayList();
		EnemyLevelLabel = CastleGrid.transform.FindChild("Door").FindChild("EnemyDescript").GetComponent<UILabel>();
	}

    // Use this for initialization
    void Start()
    {
		
    }
    void initCastleButtonData()
    {
		GM = GameManager.getInstance();
		LevelDB = GameDataBase.getDBinstance.getLevelDB;


        GameObject EnhanceButton = null;
        UILabel ButtonDescript = null;
        UILabel goldLabel = null, descriptLabel = null;
        int enemyLevelCount = GM.LoadLevelData("Normal");
        EnemyLevelLabel.text = "적 용사 레벨 : " + enemyLevelCount.ToString();

        GateLevelLabel.text = "Lv " + enemyLevelCount.ToString();
        Transform DoorBoard = CastleGrid.transform.FindChild("Door");
        DoorGoldLabel = DoorBoard.transform.FindChild("Enhance").FindChild("CastleGoldLabel").GetComponent<UILabel>();

		UIEventListener.Get(DoorBoard.transform.FindChild("Enhance").gameObject).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
        UIEventListener.Get(DoorBoard.transform.FindChild("Enhance").gameObject).onClick += new UIEventListener.VoidDelegate(ButtonProcess);

        DoorGoldLabel.text = GM.getPrice("Gate").ToString();
        
		for (int i = 0; i < 10; i++)
        {
            Transform castleBoard = CastleGrid.transform.FindChild((i + 1).ToString());
            descriptLabel = castleBoard.transform.FindChild("StageContext").GetComponent<UILabel>();

            EnhanceButton = castleBoard.transform.FindChild("Enhance").gameObject;
            goldLabel = EnhanceButton.transform.FindChild("CastleGoldLabel").GetComponent<UILabel>();
            
            castleBoard.transform.FindChild("Locked").gameObject.SetActive(false);           
            ButtonDescript = EnhanceButton.transform.FindChild("Descript").GetComponent<UILabel>();

			int data = GM.LoadLevelData (i);
			if (data == 3) {
				EnhanceButton.SetActive(false);
			} else {
				EnhanceButton.SetActive(true);

				goldLabel.text = GM.getPrice("Castle", i).ToString();
				UIEventListener.Get(EnhanceButton).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
				UIEventListener.Get(EnhanceButton).onClick += new UIEventListener.VoidDelegate(ButtonProcess);

				AbleButtonList.Add(EnhanceButton);
				ButtonDescript.text = "강화";
			}
			if (data == 0) {
				castleBoard.transform.FindChild("Locked").gameObject.SetActive(true);
			}

            descriptLabel.text = GM.getContext("Context", "StageEnhance") + "  [  " + GM.getContext("Stage", i) + "  ]\n"
                + (i + 1).ToString() + GM.getContext("Context", "StageUnLock");
			
        }
    }
    
    void ButtonProcess(GameObject Go)
    {
        string ButtonParentName = Go.transform.parent.name;
        int floor = 0;
        if (ButtonParentName.Equals("Door"))
        {
            Debug.Log(GM.getPrice("Gate") + "Gate Gold");
            if (!GM.isMaxLevelCharacter("Normal"))
            {
                //Normal, Shield, Archer
                if (TempStaticMemory.gold >= GM.getPrice("Gate"))
                {
                    Debug.Log("Level Up Enemy Character");
                    TempStaticMemory.gold -= GM.getPrice("Gate");

                    int monsterLev = GM.LoadLevelData("Normal");
                    
                    monsterLev++;

                    GM.LevelUpData("Normal");
                    GM.LevelUpData("Shield");
                    GM.LevelUpData("Archer");

                    GM.SendToUI("UpdateMaxFeverCount");

					GM.sendTrophyCondition ("EnemyLevel", monsterLev);

                    EnemyLevelLabel.text = "적 용사 레벨 : " + (GM.LoadLevelData("Normal")).ToString();

					/*New Function
					GM.LevelUpData ("Normal");
					GM.LevelUpData ("Shield");
					GM.LevelUpData ("Archer");

					GateLevelLabel.text = "Lv " + (GM.LoadLevelData("Normal")).ToString();

					*/

                    DoorGoldLabel.text = GM.getPrice("Gate").ToString();
                    GateLevelLabel.text = "Lv " + (GM.LoadLevelData("Normal")).ToString();

                }
                else
                {
                    GM.SendToAlert(AlertDialog.NotGold, (GM.getPrice("Gate")).ToString("0"));
                }
            }
            else
            {
                EnemyLevelLabel.text = "적 용사 레벨 : MAX";
                Go.SetActive(false);
            }
        }
        else
        {
            floor = Convert.ToInt32(Go.transform.parent.name);
            int price = GM.getPrice("Castle", int.Parse(Go.transform.parent.name)-1);

            if (TempStaticMemory.gold >= price)
            {
				int data = GM.LoadLevelData (floor - 1);
				GM.SendToUI ("EnhanceStair", floor);
				if (GM.getUserIntData ("gameCount") == 1) {
					TempStaticMemory.isTutoProcessOn = true;
					if (data == 1) {
						GM.PopUpTuto ();
					}
				}
				if (data == 2) {
					UIEventListener.Get(Go).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
					//Create Stairs
					GM.SendToUI("CreateStair");

					Go.SetActive(false);
					AbleButtonList.Remove(Go);
					if (floor <= 10) {
						NextStageEnhanceAppear (floor);
						GM.sendTrophyCondition ("Stage", floor);
					} else {
						GM.sendTrophyCondition ("Comeplete", 0);
					}
				}

				GameDataBase.getDBinstance.SaveFile ();
                TempStaticMemory.gold -= price;

            }
            else
            {
                GM.SendToAlert(AlertDialog.NotGold, price.ToString("0"));
            }
        }

    }
    
    private void NextStageEnhanceAppear(int number)
    {
        if(number < 10) { 
        GameObject NextButton = CastleGrid.transform.FindChild((number + 1).ToString()).gameObject;
        NextButton.transform.FindChild("Locked").gameObject.SetActive(false);
        UIEventListener.Get(NextButton.transform.FindChild("Enhance").gameObject).onClick += new UIEventListener.VoidDelegate(ButtonProcess);
        }
    }


    void OnDisable()
    {
        for (int i = 0; i < AbleButtonList.Count; i++)
        {
            GameObject button = (GameObject)AbleButtonList[i];
            UIEventListener.Get(button).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
        }
    }

}
