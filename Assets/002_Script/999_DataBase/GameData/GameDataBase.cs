using UnityEngine;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;

using Random = UnityEngine.Random;


public class GameDataBase
{
	//SingleTon

	private static GameDataBase instance;

	public static GameDataBase getDBinstance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameDataBase();
			}
			return instance;
		}
	}

	BinaryFormatter binform;
	MemoryStream mem;
	FileStream fileStream;


	DataContainerClass dataCls;

	private readonly string file_path = Application.persistentDataPath + "/data.bin";


	//FileDataInterface[] FileData = new FileDataInterface[(int)ClassEnumType.End];

	UserDataBase userDB = new UserDataBase();
	LevelDataBase levelDB = new LevelDataBase();

	StageDataBase stageDB = new StageDataBase();



    DSFactory ds_factory;

    /// <summary>
    /// The object datas.
    /// Alias, Boss, Enemy, Named, Trap
    /// </summary>
    ObjectDataBase[] ObjectDatas = new ObjectDataBase[ (int)ObjectClassEnumType.End] ;

	public GameDataBase()
	{
        //DataRemove();
        binform = new BinaryFormatter();
		DataBaseFactory factory = new DataBaseFactory ();
        
        if (!LoadFile())
        {
            dataCls = new DataContainerClass();
            userDB.Initialize();
            levelDB.Initialize();
            //stageDB.Initialize();
            //achivDB.Initialize();
        }
   
        for (ObjectClassEnumType type = ObjectClassEnumType.None + 1; type < ObjectClassEnumType.End; type++)
		{
			ObjectDatas[(int)type] = factory.CreateDB(type);
			ObjectDatas[(int)type].Initialize(dataCls.GetInstallBinData(type));
		}

		factory = null;
        ds_factory = new DSFactory(this);

    }

	public void CreatePlayerData(string player_Name, string player_sprite_name)
	{
		FileDataInterface testUSerDB = new UserDataBase();
		userDB.setUserData(player_Name, player_sprite_name);
		//  userDB.SaveData();
	}

	/* public void CreatePlayerData(string player_Name, string player_sprite_name)
      {
          player_character_data p_data;

          p_data.player_name = player_Name;
          p_data.player_sprite_name = player_sprite_name;
          p_data.player_gold = 0;
          p_data.time = 0.0f;

          MemoryStream p_mem = new MemoryStream();

          binform.Serialize(p_mem, p_data);
          dataCls.PlayerBinData = Convert.ToBase64String(p_mem.GetBuffer());
      }
      */


	public void ObjectInstall(string objectName, string objectTag ,int stair, int floor)
	{
		ObjectClassEnumType sendClassType = ObjectClassEnumType.None;

		switch (objectTag) {
		case "Alias":
			sendClassType = ObjectClassEnumType.AliasData;
			break;
		case "Enemy":
			sendClassType = ObjectClassEnumType.EnemyData;
			break;
		case "Trap":
			sendClassType = ObjectClassEnumType.TrapData;
			break;
		}

		ObjectDatas [(int)sendClassType].InstallMonster (stair, floor, objectName);
	}

	public void ObjectUnInstall(string objectName, string objectTag, int stair, int floor)
	{
		ObjectClassEnumType sendClassType = ObjectClassEnumType.None;

		switch (objectTag) {
		case "Alias":
			sendClassType = ObjectClassEnumType.AliasData;
			break;		
		case "Enemy":
			sendClassType = ObjectClassEnumType.EnemyData;
			break;
		case "Trap":
			sendClassType = ObjectClassEnumType.TrapData;
			break;
		}

		if (ObjectDatas [(int)sendClassType].UnInstallMonster (stair, floor, objectName)) {
			Debug.Log ("Success Good");
		} else {
			throw  new System.NotImplementedException();
		}

	}

	public void UpdateObjectStatus( string obj_name, string objectTag, params object[] status)
	{
		Debug.Log("Tag : " + objectTag + " // Object : "+obj_name) ;
		ObjectClassEnumType sendClassType = ObjectClassEnumType.None;
		switch (objectTag)
		{
		case "Alias":
		case "Player":
			sendClassType = ObjectClassEnumType.AliasData;  
			break;
		case "Enemy":
			sendClassType = ObjectClassEnumType.EnemyData;
			break;
		case "Trap":
			sendClassType = ObjectClassEnumType.TrapData;
			break;
		}
		ObjectDatas[(int)sendClassType].UpdateMonster(obj_name, status);
	}

	#region ObjectDataBase Getter 

	public AliasObjectData getAliasObjectDB
	{
		get {
			return (AliasObjectData)ObjectDatas [(int)ObjectClassEnumType.AliasData];
		}
	}
	public EnemyObjectData getEnemyObjectDB
	{
		get {
			return (EnemyObjectData)ObjectDatas [(int)ObjectClassEnumType.EnemyData];
		}
	}
	public TrapObjectData getTrapObjectDB
	{
		get {
			return (TrapObjectData)ObjectDatas [(int)ObjectClassEnumType.TrapData];
		}
	}

	#endregion

	#region FILE I/O

	public void DataRemove()
	{
		if (File.Exists(file_path))
		{
			try
			{
				File.Delete(file_path);
				PlayerPrefs.DeleteAll();
			}
			catch (Exception e)
			{
				Debug.Log("StackTrace " + e.StackTrace);
			}
			finally
			{
				Debug.Log("Remove File and Reset Player Data");
			}
		}
	}

	public void SaveFile()
	{
		//Class Data Save
		userDB.SaveData();
		levelDB.SaveData ();

		dataCls.PlayerBinData = userDB.getBinData();
		dataCls.LevelBinData = levelDB.getBinData ();

		/* Prev (
        dataCls.InstallBinData = installData.getBinData();
        )
         */


		for(ObjectClassEnumType type = ObjectClassEnumType.None +1; type < ObjectClassEnumType.End;type++)
		{
			ObjectDatas[(int)type].SaveData();
			dataCls.SetBinInstallData(ObjectDatas[(int)type].getBinData(),type);
		}


		if (File.Exists(file_path))
		{
			try
			{
				fileStream = new FileStream(file_path, FileMode.Truncate);
				binform.Serialize(fileStream, dataCls);
			}
			catch (ArgumentException AE)
			{
				Debug.Log(AE.StackTrace);
			}
			finally
			{
				fileStream.Close();
			}
		}
		else
		{
			using (fileStream = File.Create(file_path))
			{
				try
				{
					binform.Serialize(fileStream, dataCls);
				}
				catch (Exception e)
				{
					Debug.Log(e.Source + "/" + e.StackTrace);
				}
				finally
				{
					fileStream.Close();
				}
			}
		}
	}

	public bool LoadFile()
	{
		if (!File.Exists(file_path))
		{
			Debug.Log("File No Exist");
			return false;
		}
		else
		{
			Debug.Log("File Loaded");
			fileStream = new FileStream(file_path, FileMode.Open, FileAccess.Read, FileShare.Read);
			dataCls = (DataContainerClass)binform.Deserialize(fileStream);
            
			userDB.Initialize(dataCls.PlayerBinData);
			levelDB.Initialize (dataCls.LevelBinData);

			//ObjectLevelData.LoadData(dataCls.LevelBinData);

			fileStream.Close();

			return true;
		}

	}

	#endregion

	public bool isNoneData
	{
		get
		{
			return userDB.isDataEmpty;
		}
	}
	public bool isNoneLevelData
	{
		get
		{
			return levelDB.isDataEmpty;
		}
	}


    public DSInterface getUserDS()
    {
        return ds_factory.createDS("User");
    }
	public DSInterface getLevelDS()
	{
		return ds_factory.createDS ("Level");
	}


    public int LoadLevelData(string obj_name)
	{
		//  return ObjectLevelData.loadLevelData(obj_name);
		return 0;
	}

	public void LoadMonsterInstallData(string name, int index, out int stair, out int floor)
	{
		stair = 0;
		floor = 0;
		//installData.getMonsterInstallPos(name, index, out stair, out floor);   
	}

	public int getInstalledMonsterCount( string monster_name)
	{
		//return installData.getMonsterPos(monster_name).Count;
		return 0;
	}

	public string getInstallPosResult
	{
		get
		{
			// return installData.ToString();
			return string.Empty;
		}
	}

	#region Data Save Function

	public void SaveLevelData(string obj_name, int level)
	{
		// ObjectLevelData.saveLevelData(obj_name, level);
	}

	public UserDataBase getUserDB
	{
		get
		{
			return userDB;
		}
	}
	#endregion


}

/*
#region Install Level Data

[Serializable]
class ObjectInstallData
{
    [Serializable]
    public struct object_pos_struct
    {
        int stairNumber;
        int floorIndex;

        public object_pos_struct(int stair_number, int floor_Index)
        {
            this.stairNumber = stair_number;
            this.floorIndex = floor_Index;
        }

        public int getStair()
        {
            return stairNumber;
        }
        public int getFloor()
        {
            return floorIndex;
        }

        public bool Equals(object_pos_struct obj)
        {
            if (obj.getFloor() == floorIndex && obj.getStair() == stairNumber)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    private Dictionary<string, List<object_pos_struct>> stageInstalledDic;

    BinaryFormatter bin;
    MemoryStream mem;

    public ObjectInstallData()
    {
        stageInstalledDic = new Dictionary<string, List<object_pos_struct>>();
        bin = new BinaryFormatter();
    }

    public void LoadData(string data)
    {
        if(string.IsNullOrEmpty(data))
        {
            stageInstalledDic = new Dictionary<string, List<object_pos_struct>>();
        }
        else
        { 
        mem = new MemoryStream(Convert.FromBase64String(data));
        stageInstalledDic = (Dictionary<string, List<object_pos_struct>>)bin.Deserialize(mem);
        }
    }


    public virtual void insert_pos_data(int stair, int floor, string name)
    {
        object_pos_struct pos_struct = new object_pos_struct(stair, floor);

        if (!stageInstalledDic.ContainsKey(name))
        {
            List<object_pos_struct> pos_struct_list = new List<object_pos_struct>();
            pos_struct_list.Add(pos_struct);
            stageInstalledDic.Add(name, pos_struct_list);
        }
        else
        {
            List<object_pos_struct> pos_struct_list;
            stageInstalledDic.TryGetValue(name, out pos_struct_list);
            if (pos_struct_list != null)
            {
                pos_struct_list.Add(pos_struct);
            }
            stageInstalledDic.Remove(name);
            stageInstalledDic.Add(name, pos_struct_list);
        }
    }

    public bool remove_pos_data(string name, int stair, int floor)
    {
        object_pos_struct compare_sturct = new object_pos_struct(stair, floor);
        if (!stageInstalledDic.ContainsKey(name))
        {
            Debug.Log("Not InSide");
            return false;
        }
        else
        {
            foreach (KeyValuePair<string, List<object_pos_struct>> kvp in stageInstalledDic)
            {
                if (kvp.Key.Equals(name))
                {
                    List<object_pos_struct> pos_list = kvp.Value;
                    for (int i = 0; i < pos_list.Count; i++)
                    {
                        if (pos_list[i].Equals(compare_sturct))
                        {
                            pos_list.RemoveAt(i);
                            return true;
                        }
                    }

                }
            }

        }
        return false;
    }

    
    public List<object_pos_struct> getMonsterPos(string obj_name)
    {
        List<object_pos_struct> list = null;
        if(stageInstalledDic.ContainsKey(obj_name))
        {
            stageInstalledDic.TryGetValue(obj_name, out list);
        }
        return list;
    }
    
    public void getMonsterInstallPos(string name, int index, out int stair, out int floor)
    {
        if (stageInstalledDic.ContainsKey(name))
        {
            List<object_pos_struct> pos_list;
            stageInstalledDic.TryGetValue(name, out pos_list);

            object_pos_struct pos_struct = pos_list[index];

            stair = pos_struct.getStair();
            floor = pos_struct.getFloor();
        }
        else
        {
            stair = floor = 0;
        }
    }


    public List<string> getDictionaryKeys()
    {
        List<string> monsterNames = new List<string>();
        foreach (string keys in stageInstalledDic.Keys)
        {
            monsterNames.Add(keys);
        }
        return monsterNames;
    }

    public virtual string getBinData()
    {
        mem = new MemoryStream();
        bin.Serialize(mem, stageInstalledDic);
        return Convert.ToBase64String(mem.GetBuffer());
    }

    public override string ToString()
    {
        if (stageInstalledDic.Count > 0)
        {
            StringBuilder str_builder = new StringBuilder();

            foreach (KeyValuePair<string, List<object_pos_struct>> kvp in stageInstalledDic)
            {
                str_builder.AppendLine("Saved Monster Name : " +kvp.Key);
                str_builder.AppendLine("Save Install Pos: ");
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    str_builder.AppendLine("Stair : " + kvp.Value[i].getStair() + "/  Floor" + kvp.Value[i].getFloor());
                }
            }
            return str_builder.ToString();
        }
        else
        {
            return base.ToString();
        }
    }
}
/*
[Serializable]
class AliasMonsterData : ObjectInstallData
{

    private string[] name_Array = {"Skeleton","SkeletonMage","Slime","Imp","Golem" };

    public AliasMonsterData()
    {
        base.enumType = ObjectEnumType.Alias;
    }
    public override void insert_pos_data(int stair, int floor, string name)
    {
        base.insert_pos_data(stair, floor, name);
    }
    
}
[Serializable]
class TrapData : ObjectInstallData
{
    public TrapData()
    {
        base.enumType = ObjectEnumType.Trap;
    }
    public override void insert_pos_data(int stair, int floor, string name)
    {
        base.insert_pos_data(stair, floor, name);

    }
}
[Serializable]
class EnemyMonsterData : ObjectInstallData
{
    public EnemyMonsterData()
    {
        base.enumType = ObjectEnumType.EnemyNormal;
    }
    public override void insert_pos_data(int stair, int floor, string name)
    {
        base.insert_pos_data(stair, floor, name);

    }
}
[Serializable]
class BossMonsterData : ObjectInstallData
{
    public BossMonsterData()
    {
        base.enumType = ObjectEnumType.Boss;
    }
    public override void insert_pos_data(int stair, int floor, string name)
    {
        base.insert_pos_data(stair, floor, name);

    }
}
[Serializable]
class EnemyNamedData : ObjectInstallData
{
    public EnemyNamedData()
    {
        base.enumType = ObjectEnumType.EnemyNamed;
    }
    public override void insert_pos_data(int stair, int floor, string name)
    {
        base.insert_pos_data(stair, floor, name);

    }
    public override string getBinData()
    {
        return base.getBinData();
    }
}



#endregion
*/



/*
class AchivementData
{
	private int game_count;
	private int die_count;

	private int total_gold;
	private int installCount;

	struct install_struct
	{

	}

	private int deleteCount;


	public AchivementData ()
	{

	}
}
*/

/*
#region Level Data Class 


[Serializable]
class LevelDataClass
{
	protected JsonParser json;
	protected ObjectEnumType enumType = ObjectEnumType.None;
	protected BinaryFormatter bin;
	protected MemoryStream mem;
	protected Dictionary<string, int> levelDataDic;

	public LevelDataClass()
	{
		json = new JsonParser();
		bin = new BinaryFormatter();
		levelDataDic = new Dictionary<string, int>();
	}
	public void LoadData(string data)
	{
		if (string.IsNullOrEmpty(data))
		{
			levelDataDic = new Dictionary<string, int>();
		}
		else
		{
			mem = new MemoryStream(Convert.FromBase64String(data));
			levelDataDic = (Dictionary<string, int>)bin.Deserialize(mem);
		}
	}

	public string getBinData()
	{
		mem = new MemoryStream();
		bin.Serialize(mem, levelDataDic);
		return Convert.ToBase64String(mem.GetBuffer());
	}


	public void saveLevelData(string obj_name, int level)
	{
		Debug.Log(obj_name + "/" + level.ToString("#") + " Saved");
		if (levelDataDic.ContainsKey(obj_name))
		{
			levelDataDic.Remove(obj_name);
		}
		levelDataDic.Add(obj_name, level);
	}

	public int loadLevelData(string obj_name)
	{
		int obj_level = 0;
		levelDataDic.TryGetValue(obj_name, out obj_level);
		return obj_level;
	}

}

#endregion
*/

[Serializable]
class DataContainerClass
{
	private string playerData;

	private string aliasInstallData;
	private string enemyInstallData;
	private string trapInstallData;

	private string levelData;

	private string stageData;

	private string achivData;



	public string PlayerBinData
	{
		get
		{
			return playerData;
		}
		set
		{
			playerData = value;
		}
	}
	public string LevelBinData
	{
		get
		{
			return levelData;
		}
		set
		{
			levelData = value;
		}
	}

	public string StageBinData
	{
		get
		{
			return stageData;
		}
		set
		{
			stageData = value;
		}
	}


	public string AchivBinData
	{
		get
		{
			return achivData;
		}
		set
		{
			achivData = value;
		}
	}

	public void SetBinInstallData(string data, ObjectClassEnumType type)
	{
		/*
         private string aliasInstallData;
    private string enemyInstallData;
    private string trapInstallData;
         */
		switch(type)
		{
		case ObjectClassEnumType.AliasData:
			aliasInstallData = data;
			break;
		case ObjectClassEnumType.EnemyData:
			enemyInstallData = data;
			break;
		case ObjectClassEnumType.TrapData:
			trapInstallData = data;
			break;
		}
	}

	public string GetInstallBinData(ObjectClassEnumType type)
	{
		string returnValue = string.Empty;
		switch(type)
		{
		case ObjectClassEnumType.AliasData:
			returnValue = aliasInstallData;
			break;
		case ObjectClassEnumType.EnemyData:
			returnValue = enemyInstallData;
			break;
		case ObjectClassEnumType.TrapData:
			returnValue = trapInstallData;

			break;
		}
		return returnValue;
	}

}



