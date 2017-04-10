using UnityEngine;
using System.Collections;

using System;
using System.IO;



#region Sturct Region

[Serializable]
struct player_character_data
{
    public string player_name;
    public string player_sprite_name;
    public int player_gold;
    public float time;
    
    public bool isNoneData()
    {
        return string.IsNullOrEmpty(player_name) && string.IsNullOrEmpty(player_sprite_name);
    }
}

#endregion


public enum ObjectEnumType
{
    None = 0,
    Alias = 1,
    Boss,
    Trap,
    EnemyNormal,
    EnemyNamed,
    End,
}

public enum ClassEnumType
{
    None = -1,
    UserData,
    StageData,
	TropyData,
    End,
}

public enum ObjectClassEnumType 
{
	None = -1,
	AliasData,
	EnemyData,
	TrapData,
	End,
}



public interface FileDataInterface
{   
	
	bool Initialize(string binData);
	string getBinData();   
	void SaveData();

	/*

    void setIntData(string cmdArg, int value);
    void setStringData(string cmdArg, string value);
    void setFloatData(string cmdArg, float value);

    int getIntData(string cmdArg);
    string getStringData(string cmdArg);
    float getFloatData(string cmdArg);
    
    */

}




