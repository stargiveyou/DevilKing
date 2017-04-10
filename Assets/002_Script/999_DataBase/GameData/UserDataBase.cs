using UnityEngine;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;

using Random = UnityEngine.Random;


public class UserDataBase : FileDataInterface
{
	[Serializable]
	public struct player_user_data_struct
	{
		public string player_name;
		public string player_sprite_name;
		public int current_player_gold;
		public int total_player_gold;
		public int top_stageCount;
		public int player_game_Count;
		public int player_die_Count;

		public float total_time;

		public bool isNoneData()
		{
			return string.IsNullOrEmpty(player_name) && string.IsNullOrEmpty(player_sprite_name);
		}

		public void Copy(player_user_data_struct data)
		{
			this.player_name = data.player_name;
			this.player_sprite_name = data.player_sprite_name;
			this.current_player_gold = data.current_player_gold;
			this.total_player_gold = data.total_player_gold;
			this.top_stageCount = data.top_stageCount;
			this.total_time = data.total_time;
			this.player_game_Count = data.player_game_Count;
			this.player_die_Count = data.player_die_Count;
		}
	}

	private player_user_data_struct data_struct;

	private MemoryStream Mem;
	private BinaryFormatter bin;
	private string thisBinData;
	private ClassEnumType thisClassEnum;

	public UserDataBase()
	{
		thisClassEnum = ClassEnumType.UserData;
	}

	public bool Initialize(string binData = null)
	{
		bin = new BinaryFormatter();
        
		if (string.IsNullOrEmpty(binData))
		{
            Debug.Log("Init User Data");
            return false;
		}
		else
		{
            Debug.Log("Loaded User Data");
			loadUserData(binData);
		}
		return true;
	}

	public void setUserData(string playerName, string playerSpriteName)
	{
		data_struct.player_name = playerName;
		data_struct.player_sprite_name = playerSpriteName;
		data_struct.top_stageCount = 1;
		data_struct.total_player_gold = data_struct.current_player_gold = 0;
		data_struct.player_game_Count = 0;
		data_struct.player_die_Count = 0;
		data_struct.total_time = 0.0f;

		Debug.Log(this.GetType() + "// Created user PlayerName : " + playerName + "_ PlayerSpriteName : " + playerSpriteName);

	}

	private void loadUserData(string binData)
	{
		player_user_data_struct originData;

		Mem = new MemoryStream(Convert.FromBase64String(binData));
		originData = (player_user_data_struct)bin.Deserialize(Mem);

		data_struct.Copy(originData);
	}

	public string getBinData()
	{
		string binData = null;
		try { 
			Mem = new MemoryStream();
			bin.Serialize(Mem, data_struct);
			binData = Convert.ToBase64String(Mem.GetBuffer());
		}
		catch(Exception e)
		{
			Debug.Log(e.StackTrace);
		}
		return binData;
	}

    #region Set Function

    public void GameStart()
	{
		data_struct.player_game_Count++;
	}
	public void PlayerDie()
	{
		data_struct.player_die_Count++;
	}
    public void GoldUpdate(int gold)
    {
        data_struct.current_player_gold = gold;
    }
    public void AddGoldAmount(int gold)
    {
        data_struct.total_player_gold += gold;
    }
    public void StageUpdate(int stage)
    {
        data_struct.top_stageCount = stage;
    }



    #endregion


    public void SaveData ()
	{
		throw new NotImplementedException ();
	}

    public player_user_data_struct getUserDB
    {
        get
        {
            return data_struct;
        }
    }
    
	public bool isDataEmpty
	{
		get
		{
			return data_struct.isNoneData();
		}
	}
}
