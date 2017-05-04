using UnityEngine;

using System;

using System.Collections;
using System .Collections.Generic;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LevelDataBase : FileDataInterface {
	
	Hashtable lvTable ;

	private string binData;
	private bool isLevelDataCreated;
	public bool Initialize (string binData = null)
	{

		if (string.IsNullOrEmpty (binData)) {
			lvTable = new Hashtable ();
			isLevelDataCreated = true;
		} else {
			BinaryFormatter B_Fomatter = new BinaryFormatter ();
			MemoryStream M_Stream = new MemoryStream (Convert.FromBase64String (binData));
			lvTable = (Hashtable)B_Fomatter.Deserialize (M_Stream);
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

	public int getLevelData(string obj_name)
	{
		return (int)lvTable [obj_name];
	}

	public void LevelUpData(string obj_name)
	{
		int level = (int)lvTable [obj_name];
		lvTable [obj_name] = ++level;
	}


	public bool isDataEmpty
	{
		get {
			return isLevelDataCreated;
		}
	}


}

