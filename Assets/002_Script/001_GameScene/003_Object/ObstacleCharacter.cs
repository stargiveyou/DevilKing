using UnityEngine;
using System.Collections;

public class ObstacleCharacter : MonoBehaviour
{
    private float damage = 0.05f;
    private int count, installCount;
    private int initCount;
    private GameManager GM;
    private UISprite thisSprite, effectSprite;
   
    private StageController _stageControl;

    private const string fire_hole = "2-0_SpitFire1~2Hole";
	private int currentStair, currentFloor;

	void Awake()
	{

		thisSprite = GetComponent<UISprite>();
	}
	// Use this for initialization
    void Start()
    {
    
	}

	public void Initialize()
	{
		GM = GameManager.getInstance();
		string _name = this.gameObject.name;
		int level = GM.LoadTrapLevelData (GM.trapIndex (_name));
		GM.setTrapData(_name, level, out damage, out count, out installCount);
		switch (_name)
		{
		case "Fire":
			FireObstacleProcess(level);
			break;
		case "Stone":
			StoneObstacleProcess( level);
			transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
			break;
		case "Spike":
			SpikeObstacleProcess(level);
			break;
		default:
			break;
		}
		GM.updateTrapStatus (_name, this.gameObject.tag, currentStair, currentFloor, count);
		initCount = count;

	}

	public void Initialize(int level, int prevInstalledCount)
	{
		GM = GameManager.getInstance();
		string _name = this.gameObject.name;
		GM.setTrapData(_name, level, out damage, out count, out installCount);
		initCount = count;
		count = prevInstalledCount;	
		switch (_name)
		{
		case "Fire":
			FireObstacleProcess(level);
			break;
		case "Stone":
			StoneObstacleProcess(level);
			transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
			break;
		case "Spike":
			SpikeObstacleProcess(level);
			break;
		default:
			break;
		}
	}



	void SpikeObstacleProcess(int level)
    {
        thisSprite = GetComponent<UISprite>();
        transform.localPosition = Vector3.up * 10;
        this.gameObject.tag = "Obstacle";
        
        switch (level)
        {
            case 1:
                thisSprite.spriteName = "spike_1";
                break;
            case 2:
                thisSprite.spriteName = "spike_2";
                break;
            case 3:
                thisSprite.spriteName = "spike_3";
                break;
            case 4:
                thisSprite.spriteName = "spike_4";
                break;
            default:
                break;
        }
        thisSprite.MakePixelPerfect();
    }

	void FireObstacleProcess(int level)
    {
        thisSprite = GetComponent<UISprite>();
        transform.localPosition = Vector3.up * 30;
        effectSprite = transform.FindChild("Effect").GetComponent<UISprite>();
        Debug.Log(effectSprite.name+"  "+ effectSprite.tag);
        //int level = PlayerPrefs.GetInt("Obstacle_Level", 1);
        effectSprite.tag = "Obstacle";
        
        thisSprite.MakePixelPerfect();

        switch (level)
        {
            case 1:
            case 2:
                thisSprite.spriteName = "2-0_SpitFire1~2Hole";
                break;
            case 3:
                thisSprite.spriteName = "2-3-0FireWallHole";
                break;
            case 4:
                thisSprite.spriteName = "2-4-0FlameSwordHole";
                break;
            default:
                break;
        }
        thisSprite.MakePixelPerfect();

        StartCoroutine("FireObstacleAnimation", level);
    }

    IEnumerator FireObstacleAnimation(int level)
    {
        
        yield return new WaitForSeconds(2.0f);
        effectSprite.gameObject.SetActive(true);
        switch (level)
        {
            case 1:
                effectSprite.spriteName = "2-1_SpitFire_1";
                effectSprite.MakePixelPerfect();
                effectSprite.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                yield return new WaitForSeconds(0.3f);
                effectSprite.spriteName = "2-1_SpitFire_2";
                yield return new WaitForSeconds(0.3f);
                break;
            case 2:
                effectSprite.spriteName = "2-2_FireBall_1";
                effectSprite.MakePixelPerfect();
                effectSprite.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                yield return new WaitForSeconds(0.3f);
                effectSprite.spriteName = "2-2_FireBall_2";
                yield return new WaitForSeconds(0.3f);
                break;
            case 3:
                effectSprite.spriteName = "2-3_FireWall_1";
                effectSprite.MakePixelPerfect();
                yield return new WaitForSeconds(0.3f);
                effectSprite.spriteName = "2-3_FireWall_2";
                yield return new WaitForSeconds(0.3f);
                break;
            case 4:
                effectSprite.spriteName = "2-4_FlameSword_1";
                effectSprite.MakePixelPerfect();
                yield return new WaitForSeconds(0.3f);
                effectSprite.spriteName = "2-4_FlameSword_2";
                yield return new WaitForSeconds(0.3f);
                break;
            default:
                break;
        }
        effectSprite.gameObject.SetActive(false);
        StartCoroutine("FireObstacleAnimation", level);
    }

    string ready_spriteName, attack_spriteName;


	void StoneObstacleProcess(int level)
    {
        thisSprite = GetComponent<UISprite>();
        effectSprite = transform.FindChild("Effect").GetComponent<UISprite>();
        effectSprite.tag = "Obstacle";
        transform.localPosition = Vector3.up * 180;
        
        thisSprite.spriteName = "3-0StonFallHole";
        thisSprite.MakePixelPerfect();
        switch (level)
        {
            case 1:
                ready_spriteName = "3-1_StonFall_Ready";
                attack_spriteName = "3-1_StonFall_Attack";
                break;
            case 2:
                ready_spriteName = "3-2_SquareSton_Ready";
                attack_spriteName = "3-2_SquareSton_Attack";
                break;
            case 3:
                ready_spriteName = "3-3_SoneLance_Ready";
                attack_spriteName = "3-3_SoneLance_Attack";
                break;
            case 4:
                ready_spriteName = "3-4_StonePist_Ready";
                attack_spriteName = "3-4_StonePist_Attack";
                break;
        }

        StartCoroutine("StoneObstacleAnimation", level);


        //3-1_StonFall_Attack
        //3-2_SquareSton_Ready
        //3-2_SquareSton_Attack
        //3-3_SoneLance_Ready
        //3-3_SoneLance_Attack
        //3-4_StonePist_Ready
        //3-4_StonePist_Attack


    }

    IEnumerator StoneObstacleAnimation(int level)
    {
        yield return new WaitForSeconds(4.0f);
        float stone_y = 280.0f;
        effectSprite.gameObject.SetActive(true);
        thisSprite.enabled = false;
        effectSprite.GetComponent<BoxCollider>().enabled = false;
        effectSprite.spriteName = ready_spriteName;
        effectSprite.MakePixelPerfect();
        effectSprite.transform.localScale = new Vector3(2f, 2f, 2f);
        effectSprite.transform.localPosition = Vector3.up * 240;
        do
        {
            stone_y -= 20;
            yield return new WaitForFixedUpdate();
            effectSprite.transform.localPosition = Vector3.up * stone_y;

        } while (stone_y >= -20);
        //effectSprite.transform.localPosition = Vector3.zero;

        effectSprite.GetComponent<BoxCollider>().enabled = true;

        effectSprite.spriteName = attack_spriteName;
        effectSprite.MakePixelPerfect();
        effectSprite.transform.localScale = new Vector3(2f, 2f, 2f);
        yield return new WaitForSeconds(0.3f);
        effectSprite.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.6f);
        thisSprite.enabled = true;


        StartCoroutine("StoneObstacleAnimation", level);
    }

    public float obs_damage
    {
        get
        {
            return damage;
        }
    }
    public StageController StageCntl
    {
        get
        {
            return _stageControl;
        }
        set
        {
            _stageControl = value;
        }
    }
    public int setTrapPos
    {
        set
        {
			currentFloor = value;
        }
    }
	public int setTrapStair
	{
		set
		{
			currentStair = value;
		}
	}

    void Discount()
    {
        count--;
        Debug.Log("Obstacle Count : " + count);
        if (count == 0)
        {
            count = initCount;
			_stageControl.NonOccupyPos(currentFloor, 1);

			GM.unInstallObject (this.gameObject.name, this.gameObject.tag, currentStair, currentFloor);

            _stageControl.trapInstall(gameObject.name, false);
            gameObject.SetActive(false);
			Destroy (gameObject);
        }
    }


}
