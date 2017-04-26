using LitJson;

using UnityEngine;
using System.Collections;

using System;
using System.Runtime.Serialization.Formatters.Binary;

using System.IO;
using System.Collections.Generic;

public struct StatusData
{
    public float hp;
    public float atk;
    public float speed;
    public int range;
    public int size;
}


[Serializable]
public struct TrapStatusData
{
    public float atk;
    public int count;
    public int installCount;
}


[Serializable]
public class MonsterLevData
{
    private string name;
    private int level;

    public MonsterLevData(string name)
    {
        this.name = name;
        this.level = 0;
    }

    public string getName
    {
        get
        {
            return name;
        }
    }
    public int LevelValue
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
        }
    }

}


public class JsonParser
{
    private string languageCode;
    private JsonData json;
    bool isReady = false;
    public JsonParser()
    {
        TextAsset asset;
        asset = Resources.Load("006_Database/JsonFileData/JsonText") as TextAsset;
        json = JsonMapper.ToObject(asset.text);

        languageCode = Application.systemLanguage.ToString();

        languageCode = "ko";
        isReady = true;
    }

    public string getStageName(int num)// Stage Name Part
    {
        return json[languageCode]["Name"]["Stage"][num].ToString();
    }
    public string getMonsterName(string monsterId, int level)
    {
        return json[languageCode]["Name"][monsterId][level].ToString();
    }
    public string getMonsterNameData(string monsterId, int level)
    {
        return json[monsterId][level].ToString();
    }
    public string getContext(string contextID)
    {
        return json[languageCode]["Context"][contextID].ToString();
    }
    public string getMonsterType(string monsterType, int index)
    {
        return json[languageCode]["AttackType"][monsterType][index].ToString();
    }
    public string getTrapType(string trapType)
    {
        return json[languageCode]["AttackType"][trapType].ToString();
    }
    public string getLockConditionString(string monsterID)
    {
        return json[languageCode]["LockCondition"][monsterID].ToString();
    }
    public string getBossMonsterName(int index)
    {
        return json["BossMonster"][index].ToString();
    }
    public string getCollectDescript(string monsterID, int index)
    {
        return json[languageCode]["MonsterDescript"][monsterID][index].ToString();
    }

    /// <summary>
    /// Return MonsterName's Count 
    /// </summary>
    /// <param name="switchStr"> Normal, BossMonster, NamedMonster  </param>
    /// <returns></returns>
    public int getMonsterNameCount(string switchStr)
    {
        int length = -1;

        length = json[switchStr].Count;

        return length;
    }


    public bool PrepareComplete
    {
        get
        {
            return isReady;
        }
    }

    ~JsonParser()
    {
        json = null;
    }

}

public class CSVParser
{
    private TextAsset asset;
    private Dictionary<string, StatusData[]> monster_datas;
    private Dictionary<string, TrapStatusData[]> trap_datas;
    private Dictionary<string, int[]> price_datas;
    private List<MonsterLevData> monster_level;
    private const char lineSperator = '\n';
    private const char fieldSeperator = ',';

    public TextAsset monster_dataFile, trap_dataFile, price_dataFile;

    private bool isReady = false;

    private GameDataBase GDB;


    public CSVParser()
    {
        monster_datas = new Dictionary<string, StatusData[]>();
        trap_datas = new Dictionary<string, TrapStatusData[]>();
        price_datas = new Dictionary<string, int[]>();
        GDB = GameDataBase.getDBinstance;
        ReadAndParse();
    }

    void ReadAndParse()
    {
        monster_dataFile = Resources.Load("006_Database/CSVFileData/Monster") as TextAsset;
        string dataText = monster_dataFile.text;

        string[] lines = dataText.Split(lineSperator);

        string name = "", type = "";
        int levelRange = 0;
        StatusData[] CharacterData;


        #region Monster Data

        monster_level = getListFromData();

		for (int i = 0; i < lines.Length - 1; i++)
        {
            string[] fields = lines[i].Split(fieldSeperator);
            name = fields[0];


            if (!monster_level.Equals(name))
            {
                MonsterLevData levelData = new MonsterLevData(name);
                monster_level.Add(levelData);
            }

            if (GDB.isNoneLevelData)
            {
                GDB.getLevelDS().sendStringCmd("Create", name);
            }

            int checkRange = 1;
            while (fields[++checkRange] != "" && checkRange < fields.Length - 1) ;
            levelRange = checkRange / 5;
            CharacterData = new StatusData[levelRange];
            for (int fieldIndex = 1; fieldIndex < checkRange; fieldIndex += 5)
            {
                int index = (fieldIndex - 1) / 5;
                CharacterData[index] = new StatusData();
                CharacterData[index].hp = float.Parse(fields[fieldIndex + 0]);
                CharacterData[index].atk = float.Parse(fields[fieldIndex + 1]);
                CharacterData[index].range = int.Parse(fields[fieldIndex + 2]);
                CharacterData[index].speed = float.Parse(fields[fieldIndex + 3]);
                CharacterData[index].size = int.Parse(fields[fieldIndex + 4]);
            }
            monster_datas.Add(name, CharacterData);
        }

        if (GDB.isNoneLevelData)
        {
            //Player
            SetMonsterLevel("Player", 1);
            //Alias Normal
            SetMonsterLevel("Skeleton", 1);
            //Enemy Special
            SetMonsterLevel("JigRinde", 1);
            SetMonsterLevel("Ainhert", 1);
            SetMonsterLevel("Leonheart", 1);
            //Enemy Normal
            SetMonsterLevel("Normal", 1);
            SetMonsterLevel("Shield", 1);
            SetMonsterLevel("Archer", 1);
        }

        #endregion


        #region Trap
        monster_dataFile = Resources.Load("006_Database/CSVFileData/Trap") as TextAsset;
        dataText = monster_dataFile.text;
        TrapStatusData[] TrapDatas;
        lines = dataText.Split(lineSperator);

        for (int i = 0; i < lines.Length - 1; i++)
        {
            string[] fields = lines[i].Split(fieldSeperator);
            name = fields[0];
            int checkRange = 1;

            if (GDB.isNoneLevelData)
            {
                GDB.getLevelDS().sendStringCmd("Create", name);
            }

            while (fields[++checkRange] != "" && checkRange < fields.Length - 1) ;

            levelRange = checkRange / 3;

            TrapDatas = new TrapStatusData[levelRange];

            for (int fieldIndex = 1; fieldIndex < checkRange; fieldIndex += 3)
            {
                int index = (fieldIndex - 1) / 3;
                TrapDatas[index] = new TrapStatusData();
                TrapDatas[index].atk = float.Parse(fields[fieldIndex + 0].ToString());
                TrapDatas[index].count = int.Parse(fields[fieldIndex + 1].ToString());
                TrapDatas[index].installCount = int.Parse(fields[fieldIndex + 2].ToString());
            }
            trap_datas.Add(name, TrapDatas);
        }

        #endregion

        #region Price

        price_dataFile = Resources.Load("006_Database/CSVFileData/Price") as TextAsset;
        dataText = price_dataFile.text;
        lines = dataText.Split(lineSperator);

        int[] priceArray;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(fieldSeperator);
            name = fields[0];
            int checkRange = 1;
            while (fields[++checkRange] != "" && checkRange < fields.Length - 1) ;
            priceArray = new int[checkRange];
            TrapDatas = new TrapStatusData[levelRange];

            for (int fieldIndex = 1; fieldIndex < checkRange; fieldIndex++)
            {
                priceArray[fieldIndex - 1] = int.Parse(fields[fieldIndex]);
            }
            price_datas.Add(name, priceArray);
        }
        #endregion

        #region Trophy

        int tropy_list_count = 0;
        if ((tropy_list_count = GDB.getTropyDB.currentTropyListCount) == 0)
        {
            monster_dataFile = Resources.Load("006_Database/CSVFileData/Trophy") as TextAsset;
            dataText = monster_dataFile.text;

            lines = dataText.Split(lineSperator);

            for (int i = tropy_list_count + 1; i < lines.Length; i++)
            {
                string[] fields = lines[i].Split(fieldSeperator);

                // 0 : index
                // 1 : Condition
                // 2 : Amount 
                // 3 : spriteName
                GDB.getTropyDB.CreateTropyData(int.Parse(fields[0]), string.Empty, fields[1], int.Parse(fields[2]));

            }

        }
        #endregion;

        monster_dataFile = trap_dataFile = price_dataFile = null;

		#region Trophy

		monster_dataFile = Resources.Load("006_Database/CSVFileData/Trophy") as TextAsset;
		dataText = monster_dataFile.text;

		lines = dataText.Split(lineSperator);

		for(int i =1; i< lines.Length;i++)
		{
			string[] fields = lines[i].Split(fieldSeperator);

			// 0 : index
			// 1 : Condition
			// 2 : Amount


		}

		#endregion;

		monster_dataFile = trap_dataFile = price_dataFile = null;
        isReady = true;
    }

    public void CharacterDataValue(string name, int level, ref StatusData data)
    {
        if (monster_datas.Count > 0 && monster_datas.ContainsKey(name))
        {
            try
            {
                data = monster_datas[name][level];
            }
            catch (IndexOutOfRangeException ie)
            {
                int length = monster_datas[name].Length - 1;
                data = monster_datas[name][length];
            }
        }
    }

    public bool isLevelMax(string name)
    {
        return (monster_datas[name].Length - 1) < getMonsterLevel(name);
    }

    public bool TrapDataValue(string name, int level, ref TrapStatusData data)
    {

        if (trap_datas.Count > 0 && trap_datas.ContainsKey(name))
        {
            try
            {
                data = trap_datas[name][level];
            }
            catch (IndexOutOfRangeException IORE)
            {
                data = trap_datas[name][3];
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    private List<MonsterLevData> getListFromData()
    {
        List<MonsterLevData> DataList = null;

        string m_levelData = PlayerPrefs.GetString("CharacterLevel");
        //string m_levelData2 = GameDataBase.getDBinstance.getLevelDS().
        if (!string.IsNullOrEmpty(m_levelData))
        {
            BinaryFormatter B_Fomatter = new BinaryFormatter();
            MemoryStream M_Stream = new MemoryStream(Convert.FromBase64String(m_levelData));
            DataList = (List<MonsterLevData>)B_Fomatter.Deserialize(M_Stream);
        }
        else
        {
            DataList = new List<MonsterLevData>();
        }
        return DataList;
    }
    private void SaveListToData()
    {
        BinaryFormatter B_Fomatter = new BinaryFormatter();
        MemoryStream M_Stream = new MemoryStream();

        B_Fomatter.Serialize(M_Stream, monster_level);

        PlayerPrefs.SetString("CharacterLevel", Convert.ToBase64String(M_Stream.GetBuffer()));

    }

    /* PlayerPrefs Data R/W

private List<MonsterLevData> getListFromData()
    {
		List<MonsterLevData> DataList = null;

        string m_levelData = PlayerPrefs.GetString("CharacterLevel");
		if (!string.IsNullOrEmpty (m_levelData)) {
			BinaryFormatter B_Fomatter = new BinaryFormatter ();
			MemoryStream M_Stream = new MemoryStream (Convert.FromBase64String (m_levelData));
			DataList = (List<MonsterLevData>)B_Fomatter.Deserialize (M_Stream);
		} else {
			DataList = new List<MonsterLevData> ();
		}
		return DataList;
    }
    private void SaveListToData( )
    {
        BinaryFormatter B_Fomatter = new BinaryFormatter();
        MemoryStream M_Stream = new MemoryStream();

        B_Fomatter.Serialize(M_Stream, monster_level);

        PlayerPrefs.SetString("CharacterLevel", Convert.ToBase64String(M_Stream.GetBuffer()));

    }
	*/


    public int getMonsterLevel(string name)
    {
        int length = monster_level.Count;
        for (int i = 0; i < length; i++)
        {
            if (name.Equals(monster_level[i].getName))
            {
                return monster_level[i].LevelValue;
            }
        }
        return 0;
        //return 1;
    }

    public void SetMonsterLevel(string name, int level)
    {
        int length = monster_level.Count;
        Debug.Log("SetMonster Level : " + name);
        for (int i = 0; i < length; i++)
        {
            if (name.Equals(monster_level[i].getName))
            {
                monster_level[i].LevelValue = level;
                SaveListToData();
                break;
            }
        }
    }

    public int getItemPrice(string item_name, int level)
    {
        int[] resultArray = null;

        if (price_datas.ContainsKey(item_name))
        {
            price_datas.TryGetValue(item_name, out resultArray);
        }

        if (resultArray != null)
        {
            return resultArray[level];
        }
        else
        {
            return 0;
        }
    }

    ~CSVParser()
    {
        asset = null;
        monster_datas = null;
    }

    public bool PrepareComplete
    {
        get
        {
            return isReady;
        }
    }

}
