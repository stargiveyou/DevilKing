    using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private ArrayList AliasPlayerList;
	private GameDataBase GDB;
	private GameManager GM;

	void Awake()
    {
		AliasPlayerList = new ArrayList();
    }

	void Start()
	{
		
	}

	private string[] alias_type_strings = 	 {"Skeleton","SkeletonMage","Slime","Golem","Imp", "BossMonster"};

	private const string aliasAddress = "001_Prefab/001_Character/Normal/";
	private const string trapAddress = "001_Prefab/002_Trap/";

	public void LoadAliasObject()
	{
		GM = GameManager.getInstance();
		GDB = GameDataBase.getDBinstance;

		string aliasName = string.Empty;

		for (int i = 0; i < alias_type_strings.Length; i++) {
			for (int alias_count = 0; alias_count <	GM.getMonsterNameCount (alias_type_strings [i]); alias_count++) {
				aliasName = GM.getMonsterName (alias_type_strings [i], alias_count, true);
				GDB.getAliasObjectDB.LoadData (GM.getMonsterName (alias_type_strings [i], alias_count, true), delegate(int stair, int floor, float hp) {
                    
					if(stair != -1)
					{
                        Debug.Log(aliasName + " Created // Stair : " + stair + " // Enemy Pos " + floor + " // HP Value : " + hp);
                        
                        //Resources.Load
                        GameObject CreateMonster = Instantiate(Resources.Load(aliasAddress+ aliasName)) as GameObject;
						CreateMonster.name = aliasName;
					//Alias create
						PlayerCharacter CreateMonsterInfo = CreateMonster.GetComponent<PlayerCharacter>();

						CreateMonsterInfo.StageCntl = GM.getStageController(stair);
						CreateMonster.transform.parent = GM.getStageController(stair).TrsByPos(floor);
						CreateMonster.transform.SetAsFirstSibling();

						CreateMonsterInfo.AliasPosNumber = floor;
						CreateMonsterInfo.isBossMonster = false;
						CreateMonsterInfo.SendMessage("CharacterStatus",SendMessageOptions.DontRequireReceiver);

						//HP Value Set
						CreateMonsterInfo.CurrentHP = hp;

					}
				});
			}
		}
	}

    void AddPlayerList(GameObject player)
    {
        if(!AliasPlayerList.Contains(player))
        AliasPlayerList.Add(player);
    }

    void RemovePlayerList(GameObject player)
    {
        AliasPlayerList.Remove(player);
    }

    IEnumerator ReceiveLegendSkill()
    {
        yield return null;
        for(int i =0; i<AliasPlayerList.Count; i++)
        {
            PlayerCharacter player = ((GameObject)AliasPlayerList[i]).GetComponent<PlayerCharacter>();
            player.SendMessage("ReceiveLegendSkill");
        }
    }



    
}
