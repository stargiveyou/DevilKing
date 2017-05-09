using UnityEngine;

using System;

using System.Collections;
using System .Collections.Generic;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LevelDataBase : FileDataInterface {
	
	Hashtable lvTable ;

	private string binData;

	private char[] castleLevData;
	private int[] _castleLevData;

	private bool isLevelDataCreated;
	public bool Initialize (string binData = null)
	{

		if (string.IsNullOrEmpty (binData)) {
			
			lvTable = new Hashtable ();

			_castleLevData = new int[10];
			for (int i = 0; i < 10; i++) {
				_castleLevData [i] = 0;
			}
			_castleLevData [0] = 1;
			lvTable.Add ("Stage", _castleLevData);

			isLevelDataCreated = true;

		} else {
			BinaryFormatter B_Fomatter = new BinaryFormatter ();
			MemoryStream M_Stream = new MemoryStream (Convert.FromBase64String (binData));
			lvTable = (Hashtable)B_Fomatter.Deserialize (M_Stream);
			_castleLevData = (int[])lvTable ["Stage"];
			isLevelDataCreated = false;
		}
		return true;
	}

	public string getBinData ()
	{
		return binData;
	}

	public void SaveData ()
	{
		try{
		BinaryFormatter B_Fomatter = new BinaryFormatter ();
		MemoryStream M_Stream = new MemoryStream ();

		B_Fomatter.Serialize (M_Stream, lvTable);

		binData = Convert.ToBase64String (M_Stream.GetBuffer ());
		}
		catch(Exception e) {

		}
		finally{
			
            /*
            #if UNITY_EDITOR

			Debug.Log(this.GetType().ToString()+"// Save BInary File");

			#endif
            */
		}

	}

	//Create Hash Table .
	public void createLevelData(string _name)
	{
		if (!lvTable.Contains (_name)) 
		{
			lvTable.Add (_name, 0);
		}
	}


	public void setLevelData(string obj_name, int level)
	{
		lvTable [obj_name] = level;
	}
	public void setLevelStairData(int stair, int level)
	{
		_castleLevData [stair] = level;
		lvTable ["Stage"] = _castleLevData;
	}
	public int getLevelData(string obj_name)
	{
		if (lvTable.ContainsKey (obj_name))
			return (int)lvTable [obj_name];
		else
			return -1;
	}

	public void LevelUpData(string obj_name)
	{
		int level = (int)lvTable [obj_name];
		Debug.Log("Level up : "+ obj_name + "prev Level : "+ level);
		lvTable [obj_name] = ++level;
		Debug.Log("Level up : "+ obj_name + "current Level : "+ level);
	}

	public void LevelUpStairData(int stair)
	{
		if (_castleLevData [stair] < 3) {
			int levData = _castleLevData [stair];
			_castleLevData [stair] = ++levData;

			lvTable.Remove("Stage");
			lvTable.Add ("Stage", _castleLevData);
		}
	}


	public int getLevelDataStair(int stair)
	{
		return _castleLevData [stair];
	}


	public bool isDataEmpty
	{
		get {
			if (isLevelDataCreated) {
				Debug.Log ("LevelDatabase Initialize");
			}
			return isLevelDataCreated;
		}
	}


}

