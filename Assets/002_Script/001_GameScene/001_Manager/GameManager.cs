using UnityEngine;
using System;
using System.Text;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{

	#region SingleTone
	private static GameManager instance;

	public static GameManager getInstance()
	{
		if (instance == null)
		{
			instance = GameObject.Find("GM").GetComponent<GameManager>();
		}
		return instance;
	}
	#endregion

	#region PUBLIC Variable

	public GameObject Defender;
	public Camera ViewCamera;
	public GameObject TutoObject;
	public PopUpManage PopUp;
	public CollectionManager Collect;
	public UIManager UI;

	#region Debug Region
	public Font DebugFont;
	#endregion
	#endregion

	#region PRIVATE Variable

	private GameDataBase GDB;

	private AttackerController Attack_Control;
	private PlayerController Player_Control;
	private ObstacleController Obstacle_Control;

	private ObjectPoolManager Pool;
	private CSVParser _CSV;
	private JsonParser _Json;

	private bool isTutoSystem;
	private StageManager stageManager;

	#endregion

	void Awake()
	{
		/*
        int width = Screen.width;
        int height = Screen.width * 10 / 16;
        Application.targetFrameRate = 60;
        Screen.SetResolution(width, height, true);
        */

		PlayerPrefs.DeleteAll ();

		_CSV = new CSVParser();
		_Json = new JsonParser();

		GDB = GameDataBase.getDBinstance;

		//isTutoSystem = GDB.getUserDS().receiveIntCmd("gameCount") == 1;

		// Scene Init
		if (SceneManager.GetActiveScene().name.Equals("Game"))
		{
			TempStaticMemory.isOpenPopUp = false;
			TempStaticMemory.playCount = PlayerPrefs.GetInt("GameCount", 0);

			if (TempStaticMemory.playCount == 0)
			{
				//SendToUI("ShowTrophyDisplayPopUp", 0);
			}
			GameObject scripts = transform.FindChild ("ScriptCollect").gameObject;


			Attack_Control = scripts.GetComponent<AttackerController>();
			Player_Control = scripts.GetComponent<PlayerController>();
			Obstacle_Control = scripts.GetComponent<ObstacleController> ();
		}
	}

	void Start()
	{
		Screen.orientation = ScreenOrientation.Landscape;
		Pool = ObjectPoolManager.getInstance;
		stageManager = UI.s_Manager;

		//Load Prev Version Character
		if(!GDB.isNoneData)
		{
			//Player Data Load

			//Player Name ,Player Sprite Name
			Defender.GetComponent<PlayerCharacter>().SetPlayerInfo(GDB.getUserDS().receiveStringCmd("playerName"), GDB.getUserDS().receiveStringCmd("spriteName"));
			//Player Gold Data
			if (isTutoSystem)
			{
				TempStaticMemory.gold += 300;
                GDB.getUserDS().sendIntCmd("totalGold", 300);

			}
			else
			{
				TempStaticMemory.gold = GDB.getUserDS().receiveIntCmd("currentGold");
			}

			Attack_Control.LoadEnemyObject ();
			Player_Control.LoadAliasObject();
			Obstacle_Control.LoadTrapObject ();

		}
		else
		{

		}

		GameStartAllObject();
		StartCoroutine("AutoSave");
	}

	private float y = 0, dy;

	void Update()
	{
		#if UNITY_EDITOR

		if (TempStaticMemory.isOpenPopUp == false)
		{
			if (Input.GetKeyUp(KeyCode.W))
			{
				SendToUI("DisplayStair", 1);
			}
			else if (Input.GetKeyUp(KeyCode.S))
			{
				SendToUI("DisplayStair", -1);
			}
			else if (Input.GetKeyUp(KeyCode.Space))
			{
				SendToUI("DisplayStair", 0);
			}
		}


		#elif UNITY_ANDROID  || UNITY_IOS

		if(isTutoSystem) return;
		if (Input.touchCount == 0) return;


		//SingleTouch만
		if(TempStaticMemory.isOpenPopUp == false){
		Touch _touch = Input.GetTouch(0);

		if(_touch.phase == TouchPhase.Began )
		{
		y = 0;
		dy = 0;
		y = ViewCamera.ScreenToViewportPoint(_touch.position).y;

		//_Debug.Log("Unity Debugging Begin" +y+"\t");
		}
		else if(_touch.phase == TouchPhase.Moved)
		{
		dy = ViewCamera.ScreenToViewportPoint(_touch.position).y - y;
		}
		else if(_touch.phase == TouchPhase.Ended)
		{
		//_Debug.Log("Begin : " + y +" , End : "+  ViewCamera.ScreenToViewportPoint(_touch.position).y +" , Delta : " + dy);
		if(y < 0.9f){
		if(dy<0.1f && dy > -0.1f)
		{
		//_Debug.Log("Center");
		SendToUI("DisplayStair", 0);
		}
		else if(dy >0.1f)
		{
		//_Debug.Log("Up");
		SendToUI("DisplayStair", -1);
		PopUpOpen("Close");
		}
		else if (dy < -0.1f)
		{
		//_Debug.Log("Down");
		SendToUI("DisplayStair", 1);
		PopUpOpen("Close");
		}
		}

		}
		}
		#endif
	}


	private IEnumerator AutoSave()
	{
		TempStaticMemory.gold += 5000;

		PlayerPrefs.SetInt("gold", TempStaticMemory.gold);

        //GDB.getUserDS().sendIntCmd("currentGold", 5000);
        GDB.getUserDS().sendIntCmd("GoldUpdate", TempStaticMemory.gold);
        GDB.getUserDS().sendIntCmd("totalGold", 5000);
        
        PlayerPrefs.SetInt("EnemyKillCount", TempStaticMemory.enemykill);
        GDB.SaveFile();
        yield return new WaitForSeconds(1.5f);
        
		StartCoroutine("AutoSave");
	}

	#region Game Signal


	void GameStartAllObject()
	{
		UI.SendMessage("GameStart");


		if (!isTutoSystem)
		{
			TutoObject.SetActive(false);
			Attack_Control.StartCoroutine("StartRespawn");
		}
		else
		{
			TutoObject.SetActive(true);
		}

	}

	void GameEndAllObject()
	{
		PopUpOpen("GameLoseEnd");
		//UI.SendMessage("GameEnd");
		Time.timeScale = 0.0f;
	}

	void GameGoodEndProcess()
	{

	}

	void GameLoseEndProcess()
	{

	}

	#endregion


	#region Send To Other Script(Bridge)

	public void ExecuteSpeSkill(string name)
	{
		switch (name)
		{
		case "Dark":
			Attack_Control.StartCoroutine("ExecuteDarkSkill");
			break;
		case "Death":
			SendDeathSkill();
			break;
		case "Legend":
			Player_Control.StartCoroutine("ReceiveLegendSkill");
			break;
		}
	}

	void SendDeathSkill()
	{
		int displayLevel = stageManager.viewStageIndex;
		StageController viewStageCTRL = stageManager.FindRespawnByLevel(displayLevel);
		viewStageCTRL.SendMessage("DeathSkillReceive", SendMessageOptions.DontRequireReceiver);
	}
	/// <summary>
	/// return to String by Json
	/// Using title [Stage, Name, Context, LockCondition , AttackType, DexDescript]
	/// </summary>
	/// <param name="title">"Stage",...</param>
	/// <param name="param1">param1</param>
	/// <param name="param2">param2</param>
	/// <param name="param3">param3</param>
	/// <returns></returns>
	public string getContext(string title, object param1 = null, object param2 = null, object param3 = null)
	{
		string resultContext = null;
		switch (title)
		{
		case "Stage":
			resultContext = _Json.getStageName((int)param1);
			break;
		case "Name":
			resultContext = _Json.getMonsterName((string)param1, (int)param2);
			break;
		case "Context":
			resultContext = _Json.getContext((string)param1);
			break;
		case "LockCondition":
			resultContext = _Json.getLockConditionString((string)param1);
			break;
		case "BossMonster":
			resultContext = _Json.getBossMonsterName((int)param1);
			break;
		case "AttackType":
			resultContext = _Json.getMonsterType((string)param1, (int)param2);
			break;
		case "TrapType":
			resultContext = _Json.getTrapType((string)param1);
			break;
		case "DexDescript":
			resultContext = _Json.getCollectDescript((string)param1, (int)param2);
			break;
		case "Trophy":
			resultContext = _Json.getTrophyDescript ((bool)param1, (int)param2);
			break;

		}
		return resultContext;
	}
	/// <param name="type"> Normal, BossMonster, NamedMonster  </param>
	public int getMonsterNameCount(string type)
	{
		int returnCount = 0;

		returnCount = _Json.getMonsterNameCount (type);

		return returnCount;
	}

	public void PoolObject(GameObject Obj)
	{
		Pool.PoolObject(Obj);
	}
	/// <summary>
	/// UI's PopUP Open Control
	/// </summary>
	/// <param name="PopUpName">PopUp Name ["Enhance","Option","Exit","Install","Close","Collection"]</param>
	/// <param name="parameter">ETC Parameter</param>
	public void PopUpOpen(string PopUpName, object parameter = null)
	{
		switch (PopUpName)
		{
		case "Enchance":
		case "Option":
		case "Exit":
		case "GameLoseEnd":
			PopUp.SendMessage("PopUpOpen", PopUpName, SendMessageOptions.DontRequireReceiver);
			break;
		case "Install":
			if (isTutoSystem)
			{
				TempStaticMemory.isTutoProcessOn = true;
				stageManager.SendMessage("tutorialArrowAllDisAppear", (int)parameter);
			}
			PopUp.SendMessage("InstallPopUpOpen", (int)parameter);
			break;
		case "Close":
			PopUp.SendMessage("PopUpClose", SendMessageOptions.DontRequireReceiver);
			break;
		case "Collection":
			Collect.SendMessage("CollectSelectActive", SendMessageOptions.DontRequireReceiver);
			break;

		default:
			break;
		}
	}


	public void PopUpTuto()
	{
		PopUp.SendMessage("TutorialProcess", 3);
	}
	#endregion

	#region Receive Other Script
	/// <summary>
	/// Send to Function Name to UIManager.cs        
	/// </summary>
	/// <param name="msg">msg is Function Name</param>
	/// <param name="parameter">Funtion's Parameter (null or not)</param>
	public void SendToUI(string msg, object parameter = null)
	{
		if (parameter == null)
		{
			UI.SendMessage(msg, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			UI.SendMessage(msg, parameter, SendMessageOptions.DontRequireReceiver);
		}
	}

	/// <summary>
	/// Send Function Name ( + parameter) to PlayerController.cs
	/// </summary>
	/// <param name="msg">msg is Function Name</param>
	/// <param name="parameter">Funtion's Parameter (null or not)</param>
	public void SendToCM(string msg, object parameter = null)
	{

		if (parameter == null)
		{
			Player_Control.SendMessage(msg, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			Player_Control.SendMessage(msg, parameter, SendMessageOptions.DontRequireReceiver);
		}
	}


	public InstallPopUPControl installCtrl;

	public void InstallPopUPControl(string title, string obj_name = null)
	{
		switch (title)
		{
		case "Alias":
			installCtrl.SendMessage("UnLockMonster", obj_name, SendMessageOptions.DontRequireReceiver);
			break;
		case "Trap":
			installCtrl.SendMessage("UnLockTrap", obj_name, SendMessageOptions.DontRequireReceiver);
			break;
		}
	}

	public AleartPanelManager Alert;
	public void SendToAlert(AlertDialog alert, string value)
	{
		Alert.CreateAlert(alert, value);
	}

	//StageManager
	void SetHeroTrs(Transform Trs)
	{
		Defender.transform.parent = Trs;
		Defender.transform.localPosition = new Vector2(0, -10);
	}

	//Attackers
	public void SetAttackTrsByLevel(GameObject attack, int level)
	{
		int respawnLevel = level;
		StageController stageByLevel = null;

		for (; respawnLevel < TempStaticMemory.openStageCount; respawnLevel++)
		{
			stageByLevel = stageManager.FindRespawnByLevel(respawnLevel);
			if (stageByLevel.respawnAble)
			{
				break;
			}
		}

		stageByLevel.addStageList(attack);
		attack.transform.parent = stageByLevel.StartTrs;
	}

	public void ChangeAttackTrsByLevel(GameObject attack, int level)
	{
		StageController stageByLevel = stageManager.FindRespawnByLevel(level - 1);
		stageByLevel.removeStageList(attack);
		stageByLevel = stageManager.FindRespawnByLevel(level);
		stageByLevel.addStageList(attack);
		attack.transform.parent = stageByLevel.StartTrs;
	}

	public GameObject GetObjectByName(string OBJName)
	{
		return Pool.GetObject(OBJName);
	}

	#endregion

	#region DataBase Send

	/// <summary>
	/// Character Set Data
	/// </summary>
	/// <param name="name"></param>
	/// <param name="hp"></param>
	/// <param name="atk"></param>
	/// <param name="range"></param>
	/// <param name="speed"></param>
	/// <param name="size"></param>
	public void setCharacterData(string name, out float hp, out float atk, out int range, out float speed, out int size)
	{
		StatusData Data = new StatusData();
		//int level = _CSV.getMonsterLevel(name) - 1;
		int level = LoadLevelData(name)-1;

		_CSV.CharacterDataValue(name, level, ref Data);

		hp = Data.hp;
		atk = Data.atk;
		range = Data.range;
		speed = Data.speed;
		size = Data.size;

		//_Debug.Log("[ "+name + " ]  Created / HP :  " + hp + "\t Attack : " + atk + "\t Level : " + (level+1).ToString("#")    );
	}

	/// <summary>
	/// Character Set Data
	/// </summary>
	/// <param name="name"></param>
	/// <param name="level"></param>
	/// <param name="hp"></param>
	/// <param name="atk"></param>
	/// <param name="range"></param>
	/// <param name="speed"></param>
	/// <param name="size"></param>
	public void setCharacterData(string name, int level, out float hp, out float atk, out int range, out float speed, out int size)
	{
		StatusData Data = new StatusData();
		_CSV.CharacterDataValue(name, level, ref Data);

		hp = Data.hp;
		atk = Data.atk;
		range = Data.range;
		speed = Data.speed;
		size = Data.size;

		//_Debug.Log(name + "Created : \t" + hp + "\t" + atk + "\t" + level);
	}

	public int getCharacterSize(string name)
	{
		StatusData Data = new StatusData();
		int level = LoadLevelData(name);
		_CSV.CharacterDataValue(name, level, ref Data);

		return Data.size;
	}
	public void setTrapData(string name, int level, out float atk, out int count)
	{
		TrapStatusData Data = new TrapStatusData();
		_CSV.TrapDataValue(name, level, ref Data);
		atk = Data.atk;
		count = Data.count;
	}
	public int getTrapInstallCount(string name)
	{
		int level = 0;
		level = LoadLevelData (name);

		TrapStatusData Data = new TrapStatusData();
		_CSV.TrapDataValue(name, level, ref Data);

		return Data.installCount;
	}
	public void setTrapData(string name, int level, out float atk, out int count, out int install)
	{
		TrapStatusData Data = new TrapStatusData();

		_CSV.TrapDataValue(name, level, ref Data);

		atk = Data.atk;
		count = Data.count;
		install = Data.installCount;
	}

	public string getMonsterName(string monsterId, int level, bool isData)
	{
		if (isData)
		{
			return _Json.getMonsterNameData(monsterId, level);
		}
		else
		{
			return getContext("Name", monsterId, level);
		}
	}

	//New Fuction
	public void LevelUpData(string objName)
	{
		if (!_CSV.isLevelMax(objName))
		{
			GDB.getLevelDB.LevelUpData(objName);
		}
	}
	public int LoadLevelData(string objName)
	{
		return GDB.getLevelDB.getLevelData(objName);
	}
	public void LevelReplaceData(string objName, int level)
	{
		GDB.getLevelDB.setLevelData (objName, level);
	}


	public bool isMaxLevelCharacter(string _name)
	{
		return _CSV.isLevelMax(_name);
	}

	public int getPrice(string itemName)
	{
		int returnPrice = 0;

		if (itemName.Equals("Skill"))
		{
			return 5000;
		}
		else if (itemName.Equals("NameEnemyGet"))
		{

		}
		else if (itemName.Equals("Gate"))
		{
			return _CSV.getItemPrice("Enemy", LoadLevelData("Normal") - 1);
		}
		else if (itemName.Equals("EnemyGet"))
		{
			return _CSV.getItemPrice("EnemyGet", LoadLevelData("Normal") - 1);
		}
		return returnPrice;
	}

	/// <summary>
	/// get Price by String , Int
	/// Type : "Respawn", "Castle"
	/// </summary>
	/// <param name="title">CSV's Dictionary Key</param>
	/// <param name="index">CSV's Dictionary in Array Index</param>
	/// <returns></returns>
	public int getPrice(string title, int index)
	{
		return _CSV.getItemPrice(title, index);
	}

	public int getPrice(string itemName, string command, string itemType)
	{
		int returnPrice = 0;
		////_Debug.Log(itemName + "//" + command + "//" + itemType);
		if (command.Equals("install"))
		{
			/*
			if (itemType.Equals("Alias"))
			{
				returnPrice = _CSV.getItemPrice(itemName + "_Install", LoadLevelData(itemName) - 1);
			}
			else if (itemType.Equals("Obstacle"))
			{
				returnPrice = _CSV.getItemPrice(itemName + "_Install", LoadLevelData(itemName)- 1);
			}
			*/
			returnPrice = _CSV.getItemPrice(itemName + "_Install", LoadLevelData(itemName)- 1);
		}
		else if (command.Equals("enhance"))
		{
			/*
			if (itemType.Equals("Alias"))
			{
				returnPrice = _CSV.getItemPrice(itemName + "_Enhance", LoadLevelData(itemName));
			}
			else if (itemType.Equals("Obstacle"))
			{
				returnPrice = _CSV.getItemPrice (itemName + "_Enhance", LoadLevelData (itemName));
			}
			*/
			returnPrice = _CSV.getItemPrice(itemName + "_Enhance", LoadLevelData(itemName));

		}


		return returnPrice;
	}

	public void sendTrophyCondition(string command, int amount)
	{
		GDB.getTropyDB.sendTropyCommand (command, amount, UI.ShowTropyDisplayPopUp);
	}


	#endregion

	#region File Control 


	public void installObject(string obj_name, string tag, int stair, int floor, int level)
	{
		GDB.ObjectInstall (obj_name, tag, stair, floor, level);
	}

	public void unInstallObject(string obj_name, string tag, int stair, int floor)
	{
		Debug.Log("Removed mosnter : " + obj_name+"///"+tag+ "//" + stair + "//" + floor);
		GDB.ObjectUnInstall (obj_name,tag, stair, floor);
	}


	public void updateCharacterStatus(string obj_name, string obj_tag, int stair, int floor, float hp)
	{
		GDB.UpdateObjectStatus(obj_name, obj_tag, stair, floor, hp);
        
	}

	public void updateEnemyStatus(string obj_name, string tag, int stair, int uniq_id, float pos_x, float hp)
	{
//		Debug.Log("Update Enemy Status [" + obj_name + "]// " + tag + "// " + pos_x + "// " + hp);
        GDB.UpdateObjectStatus(obj_name, tag,uniq_id, stair,  pos_x, hp);
	}
    
	public void updateTrapStatus(string obj_name, string tag, int stair, int floor, int lastCount)
	{
		Debug.Log("Update Trap Status [" + obj_name + "]// " + tag + "//"+ stair +"// " + floor + "// " + lastCount);
		GDB.UpdateObjectStatus(obj_name,tag,stair,floor,lastCount);
	}


	#endregion

	//Cemetry Part
	public void CreateCemetryByPos(Transform Trs, int size)
	{
		//  Transform StageTrs = Trs.parent.parent;
		string objectName;
		switch (size)
		{
		case 1:
			objectName = "oneCemetry";
			break;
		case 2:
			objectName = "twoCemetry";
			break;
		case 3:
			objectName = "threeCemetry";
			break;
		default:
			objectName = null;
			break;
		}

		GameObject cemetry = Pool.GetObject(objectName);
		cemetry.transform.parent = Trs;
		cemetry.transform.localPosition = Vector3.up * 15;
		cemetry.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
	}

	//Ghost Part
	public void CreateGhostByPos(Transform Trs, int size)
	{
		string objectName;
		switch (size)
		{
		case 1:
			objectName = "oneGhost";
			break;
		case 2:
			objectName = "twoGhost";
			break;
		case 3:
			objectName = "threeGhost";
			break;
		default:
			objectName = null;
			break;
		}

		if (!string.IsNullOrEmpty(objectName))
		{
			GameObject cemetry = Pool.GetObject(objectName);
			cemetry.transform.parent = Trs;
			cemetry.transform.position = Trs.position;
			cemetry.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

		}
	}

	public StageController currenetStageController
	{
		get
		{
			StageController _stage = stageManager.FindRespawnByLevel(stageManager.viewStageIndex);
			return _stage;
		}
	}
	public StageController getStageController(int stageIndex)
	{
		return stageManager.FindRespawnByLevel (stageIndex);
	}


}

