using UnityEngine;
using System.Collections;

public class ObstacleController : MonoBehaviour {

	private const string aliasAddress = "001_Prefab/001_Character/Normal/";
	private const string trapAddress = "001_Prefab/002_Trap/";

	private GameDataBase GDB;
	private GameManager GM;
	/*
	void  Awake()
	{

	}
	void OnEnable()
	{
		if (GDB == null) {
			GDB = GameDataBase.getDBinstance;
		}
	}
	*/

	private string[] obstacleNames = { "Spike", "Fire", "Stone" };


	// Use this for initialization
	void Start () {
		GDB = GameDataBase.getDBinstance;
		GM = GameManager.getInstance ();

	}

	public void LoadTrapObject()
	{

		string trapName = string.Empty;

		for (int i = 0; i < 3; i++) {
			trapName = obstacleNames [i];
			GDB.getTrapObjectDB.LoadData (trapName, delegate(int stair, int floor, float count, int level) {

				GameObject newObstacleObject = Instantiate(Resources.Load(trapAddress + trapName)) as GameObject ;
				newObstacleObject.name = trapName;
				ObstacleCharacter TrapCtrl = newObstacleObject.GetComponent<ObstacleCharacter>();

				newObstacleObject.transform.parent = 	GM.getStageController(stair).TrsByPos(floor);
				newObstacleObject.transform.SetAsFirstSibling();

				//TrapCtrl.InitializeTrap((int)count);



			});

		}
	
	}

	// Update is called once per frame
	void Update () {
	
	}
}
