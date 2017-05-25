using UnityEngine;
using System.Collections;

public class MonsterEnhanceManager : MonoBehaviour
{

    private GameManager GM;

    public GameObject ScrollGrid;
    public GameObject currentDisplayPanel, nextDisplayPanel;
    private GameObject DisplayPanel, BuyPanel;
    private GameObject BuyProcessPanel, NotCompletePanel;

    public GameObject EnhanceButton, UnlockButton, CancleButton;

	[HideInInspector]
	public Transform mainCancleButton;

    [HideInInspector]
    public UISprite clickedSprite, nextLevelSprite;
    [HideInInspector]
    public UILabel clickedMonsterLabel, nextMonsterNameLabel;
    [HideInInspector]
    public UILabel clickedLabel, nextLevelLabel, buyDescriptLabel;

    public UISprite unlockObjectSprite;

    public UILabel priceLabel;
    private string character_name;

    public UIAtlas characterAtlas, trapAtlas;
    
    private void Awake()
    {
        DisplayPanel = transform.FindChild("DisplayInfomation").gameObject;
        BuyPanel = transform.FindChild("UnLockPanel").gameObject;
    }

    private void OnEnable()
    {
        GM = GameManager.getInstance();
        InfoDisplay("Alias", "Skeleton");
    }
    // Use this for initialization
    void Start()
    {
        
        // InitializeList();
        GameObject Items, EnhanceLock;
        int length = ScrollGrid.transform.childCount;
        int level = 0;
        
        BuyProcessPanel = BuyPanel.transform.FindChild("BuyProcessPanel").gameObject;
        NotCompletePanel = BuyPanel.transform.FindChild("NotCompletePanel").gameObject;

        UIEventListener.Get(ScrollGrid.transform.GetChild(0).gameObject).onClick += new UIEventListener.VoidDelegate(BackProcess => {
			mainCancleButton.localPosition = new Vector3(711, 580, 0);
            this.gameObject.SetActive(false);
        });


        for (int i = 1; i < length; i++)
        {
            Items = ScrollGrid.transform.GetChild(i).gameObject;
            EnhanceLock = Items.transform.FindChild("EnhanceLocked").gameObject;

            if (Items.tag.Equals("Alias"))
            {
                level = GM.LoadLevelData(Items.name);
                
                if (level == 0)
                {
                    EnhanceLock.SetActive(true);
                    UIEventListener.Get(Items).onClick -= new UIEventListener.VoidDelegate(MonsterBuyProcess);
                    UIEventListener.Get(Items).onClick += new UIEventListener.VoidDelegate(MonsterBuyProcess);
                }
                else
                {
                    EnhanceLock.SetActive(false);
                    UIEventListener.Get(Items).onClick -= new UIEventListener.VoidDelegate(InfoDisplayProcess);
                    UIEventListener.Get(Items).onClick += new UIEventListener.VoidDelegate(InfoDisplayProcess);
                }
            }
            else if (Items.tag.Equals("Obstacle"))
            {
				level = GM.LoadLevelData(Items.name);
                if (level == 0)
                {
                    EnhanceLock.SetActive(true);
                    UIEventListener.Get(Items).onClick -= new UIEventListener.VoidDelegate(MonsterBuyProcess);
                    UIEventListener.Get(Items).onClick += new UIEventListener.VoidDelegate(MonsterBuyProcess);
                }
                else
                {
                    EnhanceLock.SetActive(false);
                    UIEventListener.Get(Items).onClick -= new UIEventListener.VoidDelegate(InfoDisplayProcess);
                    UIEventListener.Get(Items).onClick += new UIEventListener.VoidDelegate(InfoDisplayProcess);
                }
            }

        }

		InfoDisplayProcess (ScrollGrid.transform.GetChild (1).gameObject);

    }

    void InfoDisplayProcess(GameObject go)
    {
        
        BuyPanel.SetActive(false);
        DisplayPanel.SetActive(true);
        int enhanceLevel = 0;

        InfoDisplay(go.tag, go.name);
        upgradeObject = go;
    
        UIEventListener.Get(EnhanceButton).onClick -= new UIEventListener.VoidDelegate(UpgradeProcess);
        UIEventListener.Get(EnhanceButton).onClick += new UIEventListener.VoidDelegate(UpgradeProcess);
        
    }

    void InfoDisplay(string title, string objName)
    {
        DisplayPanel.SetActive(true);

        EnhanceButton.SetActive(true);
        nextDisplayPanel.SetActive(true);

        if (title.Equals("Alias"))
        {
            int monster_level = GM.LoadLevelData(objName);

            float atk, hp, speed;
            int size, range;
            GM.setCharacterData(objName, out hp, out atk, out range, out speed, out size);
          
            clickedSprite.atlas = characterAtlas;
            nextLevelSprite.atlas = characterAtlas;

            character_name = objName;
            
            clickedSprite.spriteName = GM.getMonsterName(objName, monster_level-1, true); ;
            clickedSprite.MakePixelPerfect();
         
            priceLabel.text = GM.getPrice(objName, "enhance",title).ToString();

            clickedMonsterLabel.text = GM.getMonsterName(objName, monster_level-1,false);
           
            if(GM.isMaxLevelCharacter(objName))
            {
                EnhanceButton.SetActive(false);
                nextDisplayPanel.SetActive(false);

                currentDisplayPanel.transform.localPosition = new Vector3(0, 125);

                clickedLabel.text =
                     "Level : MAX" + "\n" +
                     "hp : " + hp.ToString("#") + "\n" +
                     "공격력 : " + atk.ToString("#") + "\n" +
                    "공격속도 : " + speed.ToString("#") + "\n" +
                    "공격범위 : " + range.ToString("#") + "\n" +
                    "캐릭터크기 : " + size.ToString("#");
                priceLabel.gameObject.SetActive(false);
            }
            else
            {
                currentDisplayPanel.transform.localPosition = new Vector3(-539, 125);

                priceLabel.gameObject.SetActive(true);
                clickedLabel.text =
                "Level : " + monster_level + "\n" +
                "hp : " + hp.ToString("#") + "\n" +
                "공격력 : " + atk.ToString("#") + "\n" +
               "공격속도 : " + speed.ToString("#") + "\n" +
               "공격범위 : " + range.ToString("#") + "\n" +
               "캐릭터크기 : " + size.ToString("#");
                
                GM.setCharacterData(objName, monster_level, out hp, out atk, out range, out speed, out size);
            nextLevelSprite.spriteName = GM.getMonsterName(objName, monster_level, true);
            nextLevelSprite.MakePixelPerfect();
            nextMonsterNameLabel.text = GM.getMonsterName(objName, monster_level, false);
            nextLevelLabel.text = 
                "Level : " + (monster_level + 1) + "\n" +
                "hp : " + hp.ToString("#") + "\n" +
                "공격력 : " + atk.ToString("#") + "\n" +
               "공격속도 : " + speed.ToString("#") + "\n" +
               "공격범위 : " + range.ToString("#") + "\n" +
               "캐릭터크기 : " + size.ToString("#");
            }
        }
        else if(title.Equals("Obstacle"))
        {

			int trap_level = GM.LoadLevelData(objName);

            float atk = 0;
            int count = 0, installCount = 0;

			clickedSprite.atlas = trapAtlas;
			nextLevelSprite.atlas = trapAtlas;

            GM.setTrapData(objName, trap_level, out atk, out count, out installCount);
            
            clickedSprite.spriteName = objName + "_" + (trap_level-1).ToString("0");
            clickedMonsterLabel.text = GM.getContext("Name",objName,trap_level-1);
            priceLabel.text = GM.getPrice(objName, "enhance",title).ToString();
            
			if (trap_level > 3) {
				currentDisplayPanel.transform.localPosition = new Vector3 (0, 125);

				EnhanceButton.SetActive (false);
				nextDisplayPanel.SetActive (false);
				priceLabel.gameObject.SetActive (false);
				clickedLabel.text =
					"Level : MAX" + "\n" +
				"공격력 : " + atk.ToString ("#") + "\n" +
				"유지 개수 : " + count.ToString ("#") + "\n" +
				"층 당 설치 가능 개수 : " + installCount.ToString ("#");
			} else {
				
				nextLevelSprite.spriteName = objName + "_" + (trap_level).ToString("#");
				nextMonsterNameLabel.text = GM.getContext("Name", objName, trap_level);

				currentDisplayPanel.transform.localPosition = new Vector3(-539, 125);

				priceLabel.gameObject.SetActive(true);
				clickedLabel.text =
					"Level : " + trap_level + "\n" +
					"공격력 : " + atk.ToString("#") + "\n" +
					"유지 개수 : " + count.ToString("#") + "\n" +
					"층 당 설치 가능 개수 : " + installCount.ToString("#");

				GM.setTrapData(objName, trap_level + 1, out atk, out count, out installCount);
				nextLevelLabel.text =
					"Level : " + (trap_level + 1).ToString("#") + "\n" +
					"공격력 : " + atk.ToString("#") + "\n" +
					"유지 개수 : " + count.ToString("#") + "\n" +
					"층 당 설치 가능 개수 : " + installCount.ToString("#");
			}

			clickedSprite.MakePixelPerfect();
			nextLevelSprite.MakePixelPerfect();

        }

    }

    private GameObject upgradeObject = null;

    void UpgradeProcess(GameObject go)
    {
		#if EditorDebug
		Debug.Log (this.GetType().ToString() +" // UpgradeProcess, UpgradeName : "+ upgradeObject.name +"// go Name : " +go.name); 
		#endif
        int price = GM.getPrice(upgradeObject.name, "enhance", upgradeObject.tag);
        
        if (price <= TempStaticMemory.gold)
        {
            TempStaticMemory.gold -= GM.getPrice(upgradeObject.name, "enhance", upgradeObject.tag);
            int enhanceLevel = 0;
            if (upgradeObject.tag.Equals("Alias"))
            {
				Debug.Log (upgradeObject.name);
                enhanceLevel = GM.LoadLevelData(upgradeObject.name);
                GM.LevelUpData(upgradeObject.name);
				GM.LevelUpData ("Monster");

				GM.sendTrophyCondition (upgradeObject.name, enhanceLevel + 1);
				GM.sendTrophyCondition ("MonsterEnhance", GM.LoadLevelData ("Monster"));

            }
            else if (upgradeObject.tag.Equals("Obstacle"))
            {
                //enhanceLevel = GM.LoadTrapLevelData(GM.trapIndex(upgradeObject.name));
                //GM.SaveTrapLevelData(GM.trapIndex(upgradeObject.name));
				enhanceLevel = GM.LoadLevelData(upgradeObject.name);
				GM.LevelUpData(upgradeObject.name);
				GM.LevelUpData ("Trap");

				GM.sendTrophyCondition (upgradeObject.name, enhanceLevel+1);
				GM.sendTrophyCondition ("TrapEnhance", GM.LoadLevelData ("Trap"));
            }

			Debug.Log("Enhance Level plus:" + enhanceLevel +" last gold: "+TempStaticMemory.gold);

            InfoDisplay(upgradeObject.tag, upgradeObject.name);
        }
        else
        {
            Debug.Log("Not Enough Gold");
            GM.SendToAlert(AlertDialog.NotGold, price.ToString("0"));
        }

    }
    private Vector2 unlockPos = new Vector2(477, 642);
    private Vector2 lockpos = new Vector2(-59, 642);
    
    void MonsterBuyProcess(GameObject go)
    {
		NotCompletePanel.SetActive(false);
		BuyProcessPanel.SetActive(false);
		CancleButton.SetActive(true);
			
   
        if (go.tag.Equals("Alias"))
        {
            unlockObjectSprite.atlas = characterAtlas;
            unlockObjectSprite.spriteName = GM.getMonsterName(go.name, GM.LoadLevelData(go.name), true);
        }
        else if (go.tag.Equals("Obstacle"))
        {
            unlockObjectSprite.atlas = trapAtlas;
			unlockObjectSprite.spriteName = go.name + "_" + GM.LoadLevelData(go.name);
        }
        unlockObjectSprite.MakePixelPerfect();
        unlockObjectSprite.transform.localPosition = unlockPos;

        int peekPrice = GM.getPrice(go.name, "enhance", go.tag);
        
        if (peekPrice > TempStaticMemory.gold)
        {
            GM.SendToAlert(AlertDialog.NotGold, peekPrice.ToString("0"));
        }
        else  if (!isBuyCharacter(go.name))
        {

//            DisplayPanel.SetActive(false);
            BuyPanel.SetActive(true);
            NotCompletePanel.SetActive(true);
            NotCompletePanel.transform.FindChild("ContextLabel").GetComponent<UILabel>().text
               = GM.getContext("Context", "NotComplete");
            NotCompletePanel.transform.FindChild("NeedConditionLabel").GetComponent<UILabel>().text
              = GM.getContext("LockCondition", go.name);

            UIEventListener.Get(CancleButton).onClick += new UIEventListener.VoidDelegate(process =>
            {
                OnDisableUnLockPanel();
            });
        }
        else
        {
            DisplayPanel.SetActive(false);
            int  range,size;
            float hp, atk, speed; 
            unlockObjectSprite.transform.localPosition = lockpos;
            UnlockButton.transform.FindChild("Label").GetComponent<UILabel>().text
                 = "해금 : " + peekPrice + "G";
            GM.setCharacterData(go.name, out hp, out atk, out range, out speed, out size);
            //"Level : " + GM.LoadLevelData(go.name) + "\n" +
           buyDescriptLabel.text =
                 "hp : " + hp.ToString("#") + "\n" +
                 "공격력 : " + atk.ToString("#") + "\n" +
                "공격속도 : " + speed.ToString("#") + "\n" +
                "공격범위 : " + range.ToString("#") + "\n" +
                "캐릭터크기 : " + size.ToString("#");

            BuyPanel.SetActive(true);
            CancleButton.SetActive(false);
            BuyProcessPanel.SetActive(true);
            upgradeObject = go;
                    UIEventListener.Get(UnlockButton).onClick +=
                new UIEventListener.VoidDelegate(buybuttonProcess);
           
        }
    }

    private void buybuttonProcess(GameObject go)
    {
        int peekPrice = GM.getPrice(upgradeObject.name, "enhance", upgradeObject.tag);
        int monster_level = GM.LoadLevelData(upgradeObject.name);
        upgradeObject.transform.FindChild("EnhanceLocked").gameObject.SetActive(false);

        TempStaticMemory.gold -= peekPrice;
		/*
        if (upgradeObject.tag.Equals("Alias"))
        {
            //GM.LevelUpData(upgradeObject.name, GM.LoadLevelData(upgradeObject.name) + 1);
			GM.LevelUpData(upgradeObject.name);
			//MonsterEnhance +1
        }
        else if (upgradeObject.tag.Equals("Obstacle"))
        {
			GM.LevelUpData(upgradeObject.name);
        }
*/
		GM.LevelUpData(upgradeObject.name);

        UIEventListener.Get(upgradeObject).onClick -= new UIEventListener.VoidDelegate(MonsterBuyProcess);
        UIEventListener.Get(upgradeObject).onClick += new UIEventListener.VoidDelegate(InfoDisplayProcess);

        OnDisableUnLockPanel();
        InfoDisplay(upgradeObject.tag, upgradeObject.name);
        //upgradeObject = null;
    }
    
    private void size_pos_reset(Transform trs)
    {
        string trs_name = trs.name;

        switch(trs_name)
        {
            default:
                break;
        }
    }



    void OnDisableUnLockPanel()
    {
    //    NotGoldPanel.SetActive(false);
        NotCompletePanel.SetActive(false);
        BuyProcessPanel.SetActive(false);

        BuyPanel.SetActive(false);

    }

    bool isBuyCharacter(string itemName)
    {
        //int topStage = PlayerPrefs.GetInt("TopStage", 1);
		int topStage = GM.getUserIntData ("stageCount");

        if ((topStage > 1 && itemName.Equals("SkeletonMage")) ||
            (topStage > 2 && itemName.Equals("Golem")) ||
            (topStage > 3 && itemName.Equals("Slime")) ||
            (topStage > 5 && itemName.Equals("Imp")))
        {
            return true;
        }
        else if ((topStage > 2 && itemName.Equals("Fire")) ||
            (topStage > 4 && itemName.Equals("Stone")))
        {
            return true;
        }
        else

            return false;
    }

    void OnDisable()
    {
        DisplayPanel.SetActive(false);
    }

}
