﻿using UnityEngine;

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
		public int enemy_kill_Count;
		public float total_time;

		public int aliasEnemyCreateCount;

		public bool DeathSkillOpen;
		public bool LegendSkillOpen;
		public bool DarkSkillOpen;



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
			this.enemy_kill_Count = data.enemy_kill_Count;
			this.DeathSkillOpen = DeathSkillOpen;
			this.LegendSkillOpen = LegendSkillOpen;
			this.DarkSkillOpen = DarkSkillOpen;
		}

		public override string ToString ()
		{
			return string.Format ("[Player Struct \n Name : {0}, Current Gold : {1}, Current Stage :{2}]",this.player_name,this.current_player_gold,this.top_stageCount);
		}
	}

	private player_user_data_struct data_struct;

	private MemoryStream Mem;
	private BinaryFormatter bin;
	private string thisBinData;
	private ClassEnumType thisClassEnum;

	private const int maxStairCount = 10;



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
		data_struct.enemy_kill_Count = 0;
		data_struct.player_game_Count = 0;
		data_struct.player_die_Count = 0;
		data_struct.total_time = 0.0f;
		data_struct.LegendSkillOpen = data_struct.DarkSkillOpen = data_struct.DeathSkillOpen = false;

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
		return thisBinData;
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

	public void PlayerSkillBuy(char s_name)
	{
		switch (s_name) {
		case 'l':
			data_struct.LegendSkillOpen = true;
			break;
		case 'd':data_struct.DarkSkillOpen = true;
			break;
		case 'D':
			data_struct.DeathSkillOpen = true;
			break;

		}
	}

	public void StageUpdate(bool isUp)
	{
		if (isUp) {
			++data_struct.top_stageCount;
		} else {
			int currentStage = data_struct.top_stageCount;
			data_struct.top_stageCount = Mathf.Max (1, --currentStage);
		}
	}

	public void UpdateTime(float time)
	{
		data_struct.total_time = time;
	}
	public void AddTimeAmount(float timeAmount)
	{
		data_struct.total_time += timeAmount;
	}

	public void RespawnEnemy()
	{
		data_struct.aliasEnemyCreateCount++;
	}
	public void EnemyKill()
	{
		data_struct.enemy_kill_Count++;
	}


    #endregion


	#region Get Function


	public string PlayerName
	{
		set{
			data_struct.player_name = value;
		}
		get {
			return data_struct.player_name;
		}
	}

	public string PlayerSpriteName
	{
		set {
			data_struct.player_sprite_name = value;
		}
		get {
			return data_struct.player_sprite_name;
		}
	}

	public int getCurrentGold{
		get {
			return data_struct.current_player_gold;
		}
	}

	public int getTotalGold{
		get
		{
			return data_struct.total_player_gold;
		}
	}

	public int getGameCount{
		get {
			return data_struct.player_game_Count;
		}
	}

	public int getDieCount{
		get
		{
			return data_struct.player_die_Count;
		}
	}

	public int getTopStageCount
	{
		get {
			return data_struct.top_stageCount;
		}
	}
	public int getEnemyID{
		get {
			return data_struct.aliasEnemyCreateCount;
		}
	}
	public int getEnemyKillCount
	{
		get {
			return data_struct.enemy_kill_Count;
		}
	}
	public int getMaxStairLength
	{
		get
		{
			return 0;
		}
	}

	public float getTotalTime{
		get
		{
			return data_struct.total_time;
		}
	}

	/*
	public int getPlayerIntData(string cmd)
	{
		int returnValue = 0;
		switch (cmd) {
		case "CurrentGold":
			returnValue = data_struct.current_player_gold;
			break;
		case "TotalGold":
			returnValue = data_struct.total_player_gold;
			break;
		case "gameCount":
			returnValue = data_struct.player_game_Count;
			break;
		case "dieCount":
			returnValue =data_struct.player_die_Count;
			break;
		case "stageCount":
			returnValue = data_struct.top_stageCount;
			break;
		case "enemyUniqueID":
			returnValue = data_struct.aliasEnemyCreateCount;
			break;
		}
		return returnValue;
	}
	*/


	public float getPlayerFloatData(string cmd)
	{
		float returnValue = 0;
		switch (cmd) {
		case "TotalTime":
			returnValue = data_struct.total_time;
			break;
		}

		return returnValue;
	}

	public bool isSkillOpen(char s_name)
	{
		bool returnValue = false;
		switch (s_name) {
		case 'l':
			returnValue =  data_struct.LegendSkillOpen;
			break;
		case 'd':
			returnValue = data_struct.DarkSkillOpen;
			break;
		case 'D':
			returnValue = data_struct.DeathSkillOpen;
			break;
		}
		return returnValue;
	}
	#endregion


    public void SaveData ()
	{
		try { 
			Mem = new MemoryStream();
			bin.Serialize(Mem, data_struct);
			thisBinData = Convert.ToBase64String(Mem.GetBuffer());
		}
		catch(Exception e)
		{
            #if EditorDebug
			Debug.Log(e.StackTrace);
            #endif
		}
		finally{
            #if EditorDebug
			Debug.Log("Save Data Binary File");
            #endif
		}
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
