using UnityEngine;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;


//public delegate void delegateLoadObject(int stair, int floor);
public delegate void delegateLoadObject(int stair, int floor,float hp);


//Factory Method 얼추 비슷한 패턴으로
public abstract class ObjectDataBase : FileDataInterface
{

    protected string data_bin_string;
    protected object binary_Data;


    private ObjectClassEnumType enumType = ObjectClassEnumType.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataBase"/> class.
    /// </summary>
    public ObjectDataBase()
    {
        data_bin_string = string.Empty;
    }

    //public abstract ObjectDataBase CreateClass();

    /// <summary>
    /// Initialize the specified binData.
    /// </summary>
    /// <param name="binData">Bin data.</param>
    public virtual bool Initialize(string binData)
    {
        data_bin_string = binData;
        if (string.IsNullOrEmpty(binData))
        {
            binary_Data = null;
        }
        else
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream(Convert.FromBase64String(binData));
            binary_Data = bin.Deserialize(mem);
        }
        return binary_Data != null;
    }

    public abstract void SaveData();
    public abstract void LoadData(string obj_name, delegateLoadObject callback);
    public abstract void InstallMonster(int stair, int floor, string _name);
    public abstract bool UnInstallMonster(int stair, int floor, string _name);
    public abstract void UpdateMonster(string obj_name, params object[] parameter);

    /// <summary>
    /// Gets the bin data.
    /// </summary>
    /// <returns>The bin data.</returns>
    public string getBinData()
    {
        return data_bin_string;
    }

    public ObjectClassEnumType ClassType
    {
        get
        {
            return enumType;
        }
        set
        {
            enumType = value;
        }
    }

}


public class AliasObjectData : ObjectDataBase
{
    [Serializable]
    struct alias_data_struct
    {
        int stairIndex;
        int tileIndex;
        float alias_hp;
        public alias_data_struct(int stair, int tile, float hp)
        {
            alias_hp = hp;
            stairIndex = stair;
            tileIndex = tile;
        }


        public int StairIndex
        {
            get
            {
                return stairIndex;
            }
            set
            {
                stairIndex = value;
            }
        }
        public int Tileindex
        {
            get
            {
                return tileIndex;
            }
            set
            {
                tileIndex = value;
            }
        }
        public float HpValue
        {
            get
            {
                return alias_hp;
            }
            set
            {
                alias_hp = value;
            }
        }

        public bool Equals(alias_data_struct other)
        {
            return (other.stairIndex == this.stairIndex) && (other.tileIndex == this.tileIndex);
        }

        public override string ToString()
        {
            return "Stair : " + stairIndex.ToString() + "_Tile :" + tileIndex.ToString() + "_HP:" + alias_hp.ToString();
        }
    };

    private Dictionary<string, List<alias_data_struct>> alias_data_dic;

    public AliasObjectData()
    {
        ClassType = ObjectClassEnumType.AliasData;
        /*
		//Delegate 's Comparison Sorting
		alias_data_list.Sort (delegate(alias_data_struct x, alias_data_struct y) {
			
			if(x.StairIndex > y.StairIndex )	return -1;
			else if(x.StairIndex < y.StairIndex) return 1;
			else 	if(x.StairIndex > y.StairIndex )	return -1;
			else if(x.StairIndex < y.StairIndex) return 1;
			else return 0;

		});
		*/

    }

    public override bool Initialize(string binData)
    {
        base.Initialize(binData);

        try
        {
            if (!string.IsNullOrEmpty(data_bin_string))
            {
                BinaryFormatter bin_f = new BinaryFormatter();
                MemoryStream mem = new MemoryStream(Convert.FromBase64String(data_bin_string));
                alias_data_dic = (Dictionary<string, List<alias_data_struct>>)bin_f.Deserialize(mem);
            }
            else
            {
                Debug.Log("Empty Bin String // " + this.GetType().ToString());
                alias_data_dic = new Dictionary<string, List<alias_data_struct>>();
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
            return false;
        }
    }

    public override void SaveData()
    {
        BinaryFormatter bin_f = new BinaryFormatter();
        MemoryStream mem = new MemoryStream();

        bin_f.Serialize(mem, alias_data_dic);
        data_bin_string = Convert.ToBase64String(mem.GetBuffer());
    }

    public override void LoadData(string obj_name, delegateLoadObject callback)
    {
        List<alias_data_struct> load_list;
		try{
			alias_data_dic.TryGetValue (obj_name, out load_list);
			for (int i = 0; i < load_list.Count; i++) {
				callback (load_list [i].StairIndex, load_list [i].Tileindex, load_list[i].HpValue);
			}
		}
		catch(NullReferenceException nullExcep)
		{
			Debug.Log ("Null Exception Interrupt \n"+nullExcep.StackTrace);
			callback (-1, -1,0.0f);
		}

    }

    public override void InstallMonster(int stair, int floor, string _name)
    {
        alias_data_struct alias_str = new alias_data_struct(stair, floor, 0.0f);
        List<alias_data_struct> loadStructData;

        if (alias_data_dic.ContainsKey(_name))
        {

            if (alias_data_dic.TryGetValue(_name, out loadStructData))
            {
                alias_data_dic.Remove(_name);
            }
        }
        else
        {
            loadStructData = new List<alias_data_struct>();
        }

        loadStructData.Sort(delegate (alias_data_struct x, alias_data_struct y)
        {
            if (x.Tileindex < y.Tileindex) return -1;
            else if (x.Tileindex > y.Tileindex) return 1;
            else if (x.StairIndex < y.StairIndex) return -1;
            else if (x.StairIndex > y.StairIndex) return 1;
            else return 0;
        });
        loadStructData.Add(alias_str);
        alias_data_dic.Add(_name, loadStructData);
        Debug.Log("Saved " + _name + " / " + alias_str.ToString() + "/ List Count" + loadStructData.Count + "/ Dictionary Count" + alias_data_dic.Count);
    }

    public override bool UnInstallMonster(int stair, int floor, string _name)
    {
        if (alias_data_dic.Count == 0) {
            Debug.Log("Dicionary Count is 0");
            return false;
        }
        if (alias_data_dic.ContainsKey(_name))
        {
            List<alias_data_struct> loadStruct;
            int l_index = 0;
            if (alias_data_dic.TryGetValue(_name, out loadStruct))
            {
                if ((l_index = getEqalusObjectIndex(loadStruct, stair, floor)) >= 0)
                {
                    alias_data_dic.Remove(_name);
                    loadStruct.RemoveAt(l_index);
                    
                    Debug.Log("Remove " + _name + " / " + "/ List Count" + loadStruct.Count + "/ Dictionary Count" + alias_data_dic.Count);
                    if (loadStruct.Count > 0)
                    {
                        alias_data_dic.Add(_name, loadStruct);
                    }
                    return true;
                }
                Debug.Log("Not Found Index");
            }
            Debug.Log("No Data");
        }
        
        return false;
    }

    public override void UpdateMonster(string obj_name, params object[] parameter)
    {

        List<alias_data_struct> load_struct;
        int search_index;
        int stair =
            (int)parameter[0];
        int tile = (int)parameter[1];
        float hp = (float)parameter[2];
        if (alias_data_dic.TryGetValue(obj_name, out load_struct) && (search_index = getEqalusObjectIndex(load_struct, stair, tile)) >= 0)
        {
            alias_data_struct objDataStruct = load_struct[search_index];
            objDataStruct.HpValue = hp;
            load_struct.RemoveAt(search_index);
            load_struct.Insert(search_index, objDataStruct);

            alias_data_dic.Remove(obj_name);
            alias_data_dic.Add(obj_name, load_struct);

            Debug.Log("Updated " + obj_name + " / " + objDataStruct.ToString());
        }

    }

    private int getEqalusObjectIndex(List<alias_data_struct> list_struct, alias_data_struct data_struct)
    {
        int returnvalue = -1;
        for (int i = 0; i < list_struct.Count; i++)
        {
            if (list_struct[i].Equals(data_struct))
            {
                returnvalue = i; break;
            }
        }
        return returnvalue;
    }

    private int getEqalusObjectIndex(List<alias_data_struct> list_struct, int stair, int tile)
    {
        Debug.Log("List Count : " + list_struct.Count + " // stair :  " + stair.ToString() + "// tile : " + tile.ToString());
        int returnvalue = -1;
        for (int i = 0; i < list_struct.Count; i++)
        {
            if (list_struct[i].StairIndex == stair && list_struct[i].Tileindex == tile)
            {
                Debug.Log("found");
                returnvalue = i; break;
            }
        }
        return returnvalue;
    }

}

/*

//List's Comparison Class
//지정된 System.Comparison<T>을 사용하여 전체 List<T>의 요소를 정렬합니다.
class AliasDataCls : IEquatable<AliasDataCls> , IComparable<AliasDataCls>
{
	int stairIndex;
	int tileIndex;
	string alias_name;

	public AliasDataCls(string _name)
	{
		alias_name = _name;
		stairIndex = tileIndex =0;
	}
	public AliasDataCls(string _name, int stair, int tile)
	{
		alias_name= _name;
		stairIndex = stair;
		tileIndex = tile;
	}


	public bool Equals (AliasDataCls other)
	{
		throw new NotImplementedException ();
	}

	public int CompareTo (AliasDataCls other)
	{
		throw new NotImplementedException ();
	}

	public int StairIndex{
		get {
			return stairIndex;
		}
		set {
			stairIndex = value;
		}
	}
	public int Tileindex{
		get {
			return tileIndex;
		}
		set {
			tileIndex = value;
		}
	}

}
*/


public class EnemyObjectData : ObjectDataBase
{
    [Serializable]
    struct enemy_data_struct
    {
        uint unique_id;
        int stairIndex;
        float enemy_pos_x;
        float enemy_hp;

        public enemy_data_struct(uint u_id,int stair, float hp)
        {
            unique_id = u_id;
            stairIndex = stair;
            enemy_pos_x = 0.0f;
            enemy_hp = hp;
        }

        public float EnemyPos
        {
            get
            {
                return enemy_pos_x;
            }
            set
            {
                enemy_pos_x = value;
            }
        }

        public int StairIndex
        {
            get
            {
                return stairIndex;
            }
            set
            {
                stairIndex = value;
            }
        }

        public float HpValue
        {
            get
            {
                return enemy_hp;
            }
            set
            {
                enemy_hp = value;
            }
        }

        public uint UniqueID
        {
            get
            {
                return unique_id;
            }
        }

    }

    private Dictionary<string, List<enemy_data_struct>> enemy_data_dic;
    public EnemyObjectData()
    {
        ClassType = ObjectClassEnumType.EnemyData;
    }

    public override bool Initialize(string binData)
    {
        base.Initialize(binData);

        try
        {
            if (!string.IsNullOrEmpty(data_bin_string))
            {
                Debug.Log(data_bin_string);
                BinaryFormatter bin_f = new BinaryFormatter();
                MemoryStream mem = new MemoryStream(Convert.FromBase64String(data_bin_string));
                enemy_data_dic = (Dictionary<string, List<enemy_data_struct>>)bin_f.Deserialize(mem);
            }
            else
            {
                Debug.Log("Empty Bin String // " + this.GetType().ToString());
                enemy_data_dic = new Dictionary<string, List<enemy_data_struct>>();
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
            return false;
        }
    }

    public override void SaveData()
    {
        BinaryFormatter bin_f = new BinaryFormatter();
        MemoryStream mem = new MemoryStream();

        bin_f.Serialize(mem, enemy_data_dic);
        data_bin_string = Convert.ToBase64String(mem.GetBuffer());
    }

    public override void LoadData(string obj_name, delegateLoadObject callback)
    {
        List<enemy_data_struct> load_list;

        if (enemy_data_dic.TryGetValue(obj_name, out load_list))
        {
     
            for (int i = 0; i < load_list.Count; i++)
            {
				callback(load_list[i].StairIndex, (int)load_list[i].EnemyPos,load_list[i].HpValue);
            }
        }
        else
        {
            Debug.Log("Data Dictionary Not Loaded");
        }

    }

	public override void InstallMonster(int stair, int unique_id, string _name)
    {
		enemy_data_struct enemy_struct = new enemy_data_struct((uint)unique_id, stair, 0.0f);
        List<enemy_data_struct> enemy_list = null;
        if(enemy_data_dic.ContainsKey(_name) && enemy_data_dic.TryGetValue(_name, out enemy_list))
        {
            enemy_data_dic.Remove(_name);
        }
        else
        {
            enemy_list = new List<enemy_data_struct>();
        }
        
        enemy_list.Sort(delegate (enemy_data_struct x, enemy_data_struct y) {
            if (x.UniqueID > y.UniqueID) return 1;
            else if (x.UniqueID < y.UniqueID) return -1;
            else return 0;
        }
            );

        if (enemy_list != null) { 
            enemy_list.Add(enemy_struct);
            enemy_data_dic.Add(_name,enemy_list);
        }

		Debug.Log("Saved " + _name + " / " + enemy_struct.ToString() + "/ List Count" + enemy_list.Count 
			+ "/ Dictionary Count" + enemy_data_dic.Count +"\n//Unique ID : " +unique_id);
       
    }

    public override bool UnInstallMonster(int unique_id, int floor, string _name)
    {
        if (enemy_data_dic.Count == 0) return false;

        List<enemy_data_struct> load_list;

        if(enemy_data_dic.TryGetValue(_name, out load_list))
        {
            int list_index;
            if ((list_index = getObjectIndex(load_list, unique_id)) >= 0)
            {
                load_list.RemoveAt(list_index);
                if(load_list.Count ==0)
                {
                    enemy_data_dic.Remove(_name);
                }
                return true;
            }
            Debug.Log("No In List");
        }
        Debug.Log("No In Dictionary");



        return false;
    }

    public override void UpdateMonster(string obj_name, params object[] parameter)
    {
        
    }

    private int getObjectIndex(List<enemy_data_struct> list, int unique_id)
    {
        int returnValue = -1;
        
        for(int i =0; i<list.Count; i++)
        {
            if(list[i].UniqueID == unique_id)
            {
                returnValue = i;
                break;
            }
        }

        return returnValue;
    }



}

public class TrapObjectData : ObjectDataBase
{
	struct   trap_data_struct
	{
		int stair;
		int floor;
		int lastCount;

		public trap_data_struct(int stair, int floor)
		{
			this.stair = stair;
			this.floor = floor;
		
			lastCount = 0;
		}

		public int TrapCount
		{
			set {
				lastCount = value;
			}
			get{
				return lastCount;
			}
		}

		public int Tileindex
		{
			get
			{
				return floor;
			}
			set
			{
				floor = value;
			}
		}

		public int StairIndex
		{
			get
			{
				return stair;
			}
			set
			{
				stair = value;
			}
		}
	}
	//trap struct
	Dictionary<string, List<trap_data_struct>> trap_data_dic;


    public TrapObjectData()
    {
        ClassType = ObjectClassEnumType.TrapData;
    }

    public override bool Initialize(string binData)
    {
        base.Initialize(binData);

		try
		{
			if (!string.IsNullOrEmpty(data_bin_string))
			{
				Debug.Log(data_bin_string);
				BinaryFormatter bin_f = new BinaryFormatter();
				MemoryStream mem = new MemoryStream(Convert.FromBase64String(data_bin_string));
				trap_data_dic = (Dictionary<string, List<trap_data_struct>>)bin_f.Deserialize(mem);
			}
			else
			{
				Debug.Log("Empty Bin String // " + this.GetType().ToString());
				trap_data_dic = new Dictionary<string, List<trap_data_struct>>();
			}
			return true;
		}
		catch (Exception e)
		{
			Debug.Log(e.StackTrace);
			return false;
		}
    }

    public override void SaveData()
    {
        BinaryFormatter bin_f = new BinaryFormatter();
        MemoryStream mem = new MemoryStream();

		bin_f.Serialize (mem, trap_data_dic);
        data_bin_string = Convert.ToBase64String(mem.GetBuffer());
    }
    public override void LoadData(string obj_name, delegateLoadObject callback)
    {
        
		List<trap_data_struct> load_list;
		if (trap_data_dic.TryGetValue(obj_name, out load_list))
		{

			for (int i = 0; i < load_list.Count; i++)
			{
				callback(load_list[i].StairIndex, (int)load_list[i].Tileindex,load_list[i].TrapCount);
			}
		}
		else
		{
			Debug.Log("Data Dictionary Not Loaded");
		}


    }

    public override void InstallMonster(int stair, int floor, string _name)
    {
        
		trap_data_struct trap_struct = new trap_data_struct (stair, floor);
		List<trap_data_struct> trap_list = null;

		if(trap_data_dic.ContainsKey(_name) && trap_data_dic.TryGetValue(_name,out trap_list))
		{
			trap_data_dic.Remove (_name);
		}
		else
		{
			trap_list = new List<trap_data_struct> ();
		}

		trap_list.Sort (delegate(trap_data_struct x, trap_data_struct y) {
			if (x.Tileindex < y.Tileindex) return -1;
			else if (x.Tileindex > y.Tileindex) return 1;
			else if (x.StairIndex < y.StairIndex) return -1;
			else if (x.StairIndex > y.StairIndex) return 1;
			else return 0;
		});

		if (trap_list != null) {
			trap_list.Add (trap_struct);
			trap_data_dic.Add (_name, trap_list);
		}
		Debug.Log("Saved " + _name + " / " + trap_struct.ToString() + "/ List Count" + trap_list.Count 
			+ "/ Dictionary Count" + trap_data_dic.Count);
    }

    public override bool UnInstallMonster(int stair, int floor, string _name)
    {
		if (trap_data_dic.Count == 0)
			return false;

		List<trap_data_struct> load_list;

		if (trap_data_dic.TryGetValue (_name, out load_list)) {
			int list_index =0;

			if((list_index = GetObjectListIndex(load_list,stair,floor)) >-1)
			{
				load_list.RemoveAt (list_index);
				if(load_list.Count ==0 )
				{
					trap_data_dic.Remove (_name);	
				}
				return true;
			}
			Debug.Log("No In List");
		}
		Debug.Log("No In Dictionary");
		return false;
    }
    public override void UpdateMonster(string obj_name, params object[] parameter)
    {
        throw new NotImplementedException();

		//parameter [0] : stair
		//parameter [1] : floor
		//parameter [2] : count

    }

	private int GetObjectListIndex(List<trap_data_struct> list, int stair, int floor)
	{
		int returnValue = -1;

		for (int i = 0; i < list.Count; i++) {
			if (stair == list [i].StairIndex && floor == list [i].Tileindex) {
				returnValue = i;
				break;
			}
		}

		return returnValue;

	}


}

#region Creator

class DataBaseFactory
{

    public ObjectDataBase CreateDB(ObjectClassEnumType enumType)
    {

        ObjectDataBase instance = null;
        switch (enumType)
        {
            case ObjectClassEnumType.AliasData:
                instance = new AliasObjectData();
                break;
            case ObjectClassEnumType.EnemyData:
                instance = new EnemyObjectData();
                break;
            case ObjectClassEnumType.TrapData:
                instance = new TrapObjectData();
                break;
        }

        return instance;

    }
}



#endregion