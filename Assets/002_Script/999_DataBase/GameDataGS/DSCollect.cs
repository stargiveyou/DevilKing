﻿using UnityEngine;
using System.Collections;
using System;

public class DSCollect
{

}


public interface DSInterface
{

    void sendIntCmd(string command, int sendParam);
    void sendFloatCmd(string command, float sendParam);
    void sendStringCmd(string command, string sendParam);

    int receiveIntCmd(string cmdParam);
    float receiveFloatCmd(string cmdParam);
    string receiveStringCmd(string cmdParam);

}

public class DSFactory
{

    GameDataBase GDB;

    public DSFactory(GameDataBase _GDB)
    {
        GDB = _GDB;
    }

    public DSInterface createDS(string className)
    {
        DSInterface instance = null;

        switch (className)
        {
            case "User":
                instance = new DS_UserClass(GDB.getUserDB);
                break;
            case "Level":
                break;
            case "Stage":
                break;
            case "Trophy":
                break;
            default:
                break;
        }
        return instance;
    }

}

public class DS_UserClass : DSInterface
{
    UserDataBase DB;

    public DS_UserClass(UserDataBase _DB)
    {
        
        DB = _DB;
    }

    //Float : [Time]
    //Integer : [currentGold, totalGold, gameCount, dieCount,stageCount]
    //String : [playerName, spriteName]

    public float receiveFloatCmd(string cmdParam)
    {
        float returnValue = 0.0f;
        switch (cmdParam)
        {
            case "Time":
                returnValue = DB.getUserDB.total_time;
                break;
        }
        return returnValue;
    }
    /// <summary>
    ///  Receive Integer Value by User DB
    /// </summary>
    /// <param name="cmdParam">currentGold, totalGold, gameCount, dieCount,stageCount</param>
    /// <returns></returns>
    public int receiveIntCmd(string cmdParam)
    {
        int returnValue = 0;
        switch (cmdParam)
        {
            case "currentGold":
                returnValue = DB.getUserDB.current_player_gold;
                break;
            case "totalGold":
                returnValue = DB.getUserDB.total_player_gold;
                break;
            case "gameCount":
                returnValue = DB.getUserDB.player_game_Count;
                break;
            case "dieCount":
                returnValue = DB.getUserDB.player_die_Count;
                break;
            case "stageCount":
                returnValue = DB.getUserDB.top_stageCount;
                break;
        }
        return returnValue;
    }

    public string receiveStringCmd(string cmdParam)
    {
        string returnValue = string.Empty;
        switch (cmdParam)
        {
            case "playerName":
                returnValue = DB.getUserDB.player_name;
                break;
            case "spriteName":
                returnValue = DB.getUserDB.player_sprite_name;
                break;
        }

        return returnValue;
    }

    public void sendFloatCmd(string command, float sendParam)
    {
        UserDataBase.player_user_data_struct loadCurrentData = DB.getUserDB;
        switch (command)
        {
            case "Time":
                loadCurrentData.total_time = sendParam;
                break;
            case "deltaTime":
                loadCurrentData.total_time += sendParam;
                break;

        }
        DB.getUserDB.Copy(loadCurrentData);
    }
    /// <summary>
    /// Send Integer Value By User DB
    /// </summary>
    /// <param name="command">[currentGold, GoldUpdate, totalGold, stageCount]</param>
    /// <param name="sendParam">Integer Value</param>
    public void sendIntCmd(string command, int sendParam)
    {
    
        switch (command)
        {
            case "currentGold":
                
                break;
            case "GoldUpdate":
                DB.GoldUpdate(sendParam);
                break;
            case "totalGold":
                DB.AddGoldAmount(sendParam);
                break;
            case "stageCount":
                DB.StageUpdate(sendParam);
                break;
            case "GameStart":
                DB.GameStart();
                break;
            case "GameEnd":
                DB.PlayerDie();
                break;
        }
    }

    /// <summary>
    /// Send String Value By User DB
    /// </summary>
    /// <param name="command">[playerName, spriteName]</param>
    /// <param name="sendParam">String Value</param>
    public void sendStringCmd(string command, string sendParam)
    {
        UserDataBase.player_user_data_struct loadCurrentData = DB.getUserDB;

        switch (command)
        {
            case "playerName":
                loadCurrentData.player_name = sendParam;
                break;
            case "spriteName":
                loadCurrentData.player_sprite_name = sendParam;
                break;
        }
        DB.getUserDB.Copy(loadCurrentData);
    }


    public void GameStart()
    {
        Debug.Log("?");
    }


}

/*
public class DS_LevelClass: DSInterface
{

}

public class DS_StairClass: DSInterface
{
}
*/


