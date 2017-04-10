using UnityEngine;
using System.Collections;
using System.Reflection;
public class StageController : MonoBehaviour
{
    private GameManager GM;
    public GameObject Bottom;
    public GameObject lockSprite;
    public UILabel lockTimeLabel;
    public GameObject ForceButton;
    public GameObject StairObject;
    public UILabel spikeCountLabel, fireCountLabel, stoneCountLabel;
    
    private GameObject trapLabelObj;
    private GameObject[] Tiles;
    private GameObject[] stageArrows;

    private ArrayList OnStageEnemyList;

    private int stageLevel = 1;
    private int stairNumber = 0;
    private bool[] tiles_occupy;

    private bool isAvailable;
    private UILabel stageName;


    private readonly int maxTileCount = 12;

    private int maxSpikeCount, maxFireCount, maxStoneCount;
    private int installedSpikeCount, installedFireCount, installedStoneCount;

    void Awake()
    {
        GM = GameManager.getInstance();

        OnStageEnemyList = new ArrayList();
        OnStageEnemyList.Clear();

        Tiles = new GameObject[maxTileCount];
        tiles_occupy = new bool[maxTileCount];
        stageArrows = new GameObject[maxTileCount];

        installedFireCount = installedSpikeCount = installedStoneCount = 0;
        
        for (int i = 0; i < maxTileCount; i++)
        {
            Tiles[i] = Bottom.transform.GetChild(i).gameObject;
            stageArrows[i] = Bottom.transform.GetChild(i).FindChild("TileArrow").gameObject;
            Tiles[i].name = i.ToString("0");
            if (i == 0 || i == maxTileCount - 1) continue;
            UIEventListener.Get(Tiles[i]).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
            UIEventListener.Get(Tiles[i]).onClick += new UIEventListener.VoidDelegate(ButtonProcess);
        }
        trapLabelObj = transform.FindChild("trapLabels").gameObject;
        trapLabelObj.SetActive(false);

        isAvailable = true;

    }

    void Start()
    {
        stageName = transform.FindChild("stageName").GetComponent<UILabel>();
        stageName.text = "폐허";
        //stageLevel


        UIEventListener.Get(ForceButton).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
        UIEventListener.Get(ForceButton).onClick += new UIEventListener.VoidDelegate(ButtonProcess);
        
    }


    public void TrapInstallCountInitialize(int spike, int fire, int stone)
    {
        maxSpikeCount = spike;
        maxFireCount = fire;
        maxStoneCount = stone;
        
        spikeCountLabel.text = "x" + (maxSpikeCount - installedSpikeCount).ToString("0");
        fireCountLabel.text = "x" + (maxFireCount - installedFireCount).ToString("0");
        stoneCountLabel.text = "x" + (maxStoneCount - installedStoneCount).ToString("0");
    }

    void TrapCountDisplay(int stairLevel)
    {

        if (stairLevel <= 1)
        {
            spikeCountLabel.gameObject.SetActive(true);
            fireCountLabel.gameObject.SetActive(false);
            stoneCountLabel.gameObject.SetActive(false);
        }
        else if (stairLevel <= 2)
        {
            spikeCountLabel.gameObject.SetActive(true);
            fireCountLabel.gameObject.SetActive(true);
            stoneCountLabel.gameObject.SetActive(false);
        }
        else
        {
            spikeCountLabel.gameObject.SetActive(true);
            fireCountLabel.gameObject.SetActive(true);
            stoneCountLabel.gameObject.SetActive(true);
        }
    }

    private GameObject installObj = null;
    void ButtonProcess(GameObject go)
    {
        if (go.name.Equals("UnLockButton"))
        {
            int stageIndex = int.Parse(gameObject.name.Split('_')[1]);
            int respawnPrice = GM.getPrice("BossRespawn", stageIndex);
            
            if (TempStaticMemory.gold >= respawnPrice)
            {
                ForceunLock();
                go.SetActive(false);
                TempStaticMemory.gold -= respawnPrice;
            }
            else
            {
                GM.SendToAlert(AlertDialog.NotGold, respawnPrice.ToString());
            }
        }
        else
        {
            ArrowDisappear();
            int index = 0;
            for (int i = 0; i < Tiles.Length; i++)
            {
                GameObject tile = Tiles[i];
                if (tile.Equals(go))
                {
                    index = i;
                    break;
                }
            }
            if (!isOccupyPos(index))
            {
                ArrowAppear(index);
                GM.PopUpOpen("Install", index);
                PoolCemetry(go);
            }
        }
    }

    public void addStageList(GameObject Enemy)
    {
        OnStageEnemyList.Add(Enemy);
    }
    public void removeStageList(GameObject Enemy)
    {
        OnStageEnemyList.Remove(Enemy);
    }
    private void DeathSkillReceive()
    {
        for (int i = 0; i < OnStageEnemyList.Count; i++)
        {
            Attacker ownEnemy = ((GameObject)OnStageEnemyList[i]).GetComponent<Attacker>();
            ownEnemy.SendMessage("DamageCharacter", 9999, SendMessageOptions.DontRequireReceiver);
        }
    }

    void ArrowAppear(int index)
    {
        if (index > 0 && index < 11)
        {
            trapLabelObj.SetActive(true);
            stageArrows[index].SetActive(true);
        }
    }

    void ArrowDisappear()
    {
        trapLabelObj.SetActive(false);
        for (int i = 1; i < maxTileCount - 1; i++)
        {
            stageArrows[i].SetActive(false);
        }
    }
    public void OccupyPos(int index, int size)
    {
        for (int i = index; i >= index - size + 1; i--)
        {
            tiles_occupy[i] = true;
        }
    }
    public void NonOccupyPos(int index, int size)
    {
        for (int i = index; i >= index - size + 1; i--)
        {
            tiles_occupy[i] = false;
        }

    }

    public bool isOccupyPos(int index)
    {
        return tiles_occupy[index];
    }

    #region Trap Install Part
    public void trapInstall(string trapName, bool install)
    {
        int lastCount = 0;

        switch (trapName)
        {
            case "Spike":
                if (install)
                {
                    installedSpikeCount++;
                }
                else
                {
                    installedSpikeCount--;
                }
                lastCount = maxSpikeCount - installedSpikeCount;
                if(lastCount ==0)
                {
                    spikeCountLabel.text = "";
                }
                else
                {
                    spikeCountLabel.text = "x" + lastCount.ToString("0");
                }
                
                break;
            case "Fire":
                if (install)
                {
                    installedFireCount++;
                }
                else
                {
                    installedFireCount--;
                }

                lastCount = maxFireCount - installedFireCount;
                if (lastCount == 0)
                {
                    fireCountLabel.text = "";
                }
                else
                {
                    fireCountLabel.text = "x" + lastCount.ToString("0");
                }
                break;
            case "Stone":

                if (install)
                {
                    installedStoneCount++;
                }
                else
                {
                    installedStoneCount--;
                }
                
                lastCount = maxStoneCount - installedStoneCount;
                if (lastCount == 0)
                {
                    stoneCountLabel.text = "";
                }
                else
                {
                    stoneCountLabel.text = "x" + lastCount.ToString("0");
                }
                break;
        }
    }
    public bool isTrapInstall(string trapName)
    {
        if ((trapName.Equals("Spike") && maxSpikeCount > installedSpikeCount) ||
            (trapName.Equals("Fire") && maxFireCount > installedFireCount) ||
            (trapName.Equals("Stone") && maxStoneCount > installedStoneCount))
        {
            return true;
        }
        else
        {
            return false;
        }

    }


#endregion



    void ExitGateChange()
    {
        StairObject.SetActive(true);
    }
    private PlayerCharacter stageBossMonster;
    public void setStageBossmonster()
    {
        stageBossMonster = Tiles[10].transform.GetChild(0).GetComponent<PlayerCharacter>();
        stageBossMonster.StageCntl = this;
        stageBossMonster.AliasPosNumber = 10;
        stageBossMonster.isBossMonster = true;
        stageBossMonster.SendMessage("CharacterStatus", SendMessageOptions.DontRequireReceiver);
    }
    public void updateReNewBossmonster()
    {
		Debug.Log("1 :"+GM.getContext("BossMonster", stairNumber) +" // "+ stairNumber);
        Debug.Log("2 :"+stageBossMonster.gameObject.name +" //");
        stageBossMonster.SendMessage("CharacterStatus", SendMessageOptions.DontRequireReceiver);
    }
    #region Getter Function

    public Transform StartTrs
    {
        get
        {
            return Tiles[0].transform;
        }
    }
    public Transform TrsByPos(int pos)
    {
        Transform returnPos = null;
        try
        {
            returnPos = Tiles[pos].transform;
        }
        catch
        {
            if (pos < 0)
            {
                returnPos = StartTrs;
            }
            else
            {
                returnPos = EndPos;
            }
        }

        return Tiles[pos].transform;
    }


    public Transform EndPos
    {
        get
        {
            return Tiles[11].transform;
        }
    }
    public Transform MonsterPos
    {
        get
        {
            return Tiles[10].transform;
        }
    }
    
    #endregion


    void SetName(string msg)
    {
        stageName.text = msg;
    }

    void PoolCemetry(GameObject obj)
    {
        Transform Trs = obj.transform;
        for (int i = 0; i < Trs.childCount; i++)
        {
            GameObject childObj = Trs.GetChild(i).gameObject;
            if (childObj.CompareTag("Ghost"))
            {
                childObj.GetComponent<BoxCollider>().enabled = true;
                ObjectPoolManager.getInstance.PoolObject(childObj);
                break;
            }
        }
    }

    public bool isListEmpty
    {
        get
        {
            return OnStageEnemyList.Count == 0;
        }
    }

    IEnumerator LockStage()
    {
        ForceButton.SetActive(true);
        float time = 5.0f;
        lockSprite.SetActive(true);
        isAvailable = false;
        do
        {
            time -= Time.deltaTime;
            lockTimeLabel.text = time.ToString("0.0#");
            yield return new WaitForSeconds(Time.deltaTime);
        } while (time >= 0);
        ForceButton.SetActive(false);
        lockTimeLabel.text = "0";
        isAvailable = true;
        lockSprite.SetActive(false);
        time = 0;
    }

    public void ForceunLock()
    {
        ForceButton.SetActive(false);
        StopCoroutine("LockStage");
        isAvailable = true;
        lockSprite.SetActive(false);
        if (stageBossMonster != null)
            stageBossMonster.SendMessage("ForceUnlock");
    }

    /*
    private float stageRespawnTime()
    {
        int  stageIndex = int.Parse(gameObject.name.Split('_')[1]);
    }*/

   public int StairNumber
    {
        get
        {
            return stairNumber;
        }
        set
        {
            stairNumber = value;
        }
    }
   
    public bool respawnAble
    {
        get
        {
            return isAvailable;
        }
    }


}
