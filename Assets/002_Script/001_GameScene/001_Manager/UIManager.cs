    using UnityEngine;
using System.Collections;


public class UIManager : MonoBehaviour
{
    private GameManager GM;

    public StageManager s_Manager;
    // public GameObject PopUPObject;
    public TrophyDisplayPanelManager TrophyDisplayPopUP;
    public Transform FeverPanel;

    private UISprite feverGageSprite;
    private UILabel feverLabel;

    void Start()
    {
        GM = GameManager.getInstance();
        //Move To GameStart()
        
        //TempStaticMemory.enemykill = PlayerPrefs.GetInt("EnemyKillCount", 0);

        feverGageSprite = FeverPanel.FindChild("feverGage").GetComponent<UISprite>();
        feverLabel = FeverPanel.FindChild("feverLabel").GetComponent<UILabel>();
        goldSprite.transform.localPosition = Vector3.left * 100;
        UpdateMaxFeverCount();

        StartCoroutine("GoldLabelUpdate");
        StartCoroutine("UpdateFeverGageCoroutine");
    }

    void GameStart()
    {
        s_Manager.SendMessage("GameStartRecieveSignal");
    }
    void GameEnd()
    {
        //s_Manager.SendMessage("RemoveDownStair");
        //s_Manager.SendMessage("GameEndRecieveSignal");
    }

    void CreateStair()
    {
        s_Manager.SendMessage("CreateUpStair", SendMessageOptions.DontRequireReceiver);
    }

    void EnhanceStair(int number)
    {
        Debug.Log("Receive Message Enhance stair");
        s_Manager.SendMessage("EnhanceStageLevel", number, SendMessageOptions.DontRequireReceiver);
    }

    void DisplayStair(int value)
    {
        s_Manager.SendMessage("MoveStageAndFocus", value);
    }
    
    void ShowTrophyDisplayPopUp(int index)
    {
        TrophyDisplayPopUP.gameObject.SetActive(true);
        string spriteName = "", context = "";
        TrophySpriteNameByIndex(index, out spriteName, out context);
        TrophyDisplayPopUP.SetTrophy(spriteName, context);
    }

    private void TrophySpriteNameByIndex(int index, out string spritename, out string context)
    {
        switch (index)
        {
            case 0:
                spritename = "welcome";
                context = "웰컴 투 마왕성";
                break;
            default:
                spritename = "1";
                context = "2";
                break;
        }
    }


    public Transform stageStartTrs;
    private Vector3 stageEndPos;

    public Vector3 defendHeroPos
    {
        get
        {
            return stageEndPos;
        }
        set
        {
            stageEndPos = value;
        }
    }
    public UISprite goldSprite;
    public UILabel goldLabel;
    private const int delta_sprite_x = 30;
    IEnumerator GoldLabelUpdate()
    {
        int decimalIndex = 0;

        int currentGold = TempStaticMemory.gold;

        while (currentGold != 0)
        {
            currentGold /= 10;
            decimalIndex++;
        }
        
        goldLabel.text = TempStaticMemory.gold.ToString("0");
        goldSprite.transform.localPosition = new Vector3(-100 - (delta_sprite_x * decimalIndex), 0, 0);
        yield return new WaitForSeconds(0.2f);
        StartCoroutine("GoldLabelUpdate");
    }
    
    IEnumerator UpdateFeverGageCoroutine()
    {
        feverGageSprite.fillAmount = (float)TempStaticMemory.enemykill / maxFeverCount;
        yield return new WaitForFixedUpdate();
        StartCoroutine("UpdateFeverGageCoroutine");
    }
    
    private int[] maxFeverCountList = { 30, 40, 50, 70 };
    private int maxFeverCount;
    void UpdateMaxFeverCount()
    {
        int enemy_level = GM.LoadMonsterLevelData("Normal");
        int enemy_index = 0;
        switch(enemy_level)
        {
            case 0: case 1:
                enemy_index = 0;
                break;
            case 2: case 3:
                enemy_index = 1;
                break;
            case 4: case 5:
                enemy_index = 2;
                break;
            case 6:
                enemy_index = 3;
                break;
        }
        maxFeverCount = maxFeverCountList[enemy_index];
    }

    void FeverDisplay(bool isStart)
    {
        if(isStart)
        {
            feverLabel.text = "FEVER";
        }
        else
        {
            feverLabel.text = "";
        }
    }

}
