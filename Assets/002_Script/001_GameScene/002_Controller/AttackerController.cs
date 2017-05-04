using UnityEngine;
using System.Collections;

public class AttackerController : MonoBehaviour
{

	private int attack_Create_Unique_id= 0;

    public GameObject Empty_Attacker;

    public UIAtlas NormalMonsterAtlas;
    public UIAtlas[] NamedMonsterAtlas;

    private ArrayList AttackerList;
    private GameManager GM;
	private GameDataBase GDB;


    private bool isFever = false;
    float e_respawn_time = 0.0f;
    // Use this for initialization
    void Start()
    {
		GDB = GameDataBase.getDBinstance;
        GM = GameManager.getInstance();

        AttackerList = new ArrayList();

    }
	private string[] Attackers_name = { "Bastard", "Hat", "Magician", "SwordKnight", "Sheilder" };
	//private string[] Special_Attackers_Name = { "JigRinde", "Leonheart","Ainhert","Darkflame" };

	private string[] Special_Attackers_Name = { "JigRinde",
		"Ainhert",
		"Leonheart",
		"Darkflame",
		"Ishark",
		"Rebelika",
		"Kastral",
		"Illene",
		"Gunt",
		"Mrpenguin" };
	

	public void LoadEnemyObject(string obj_name = null)
	{
		attack_Create_Unique_id = GDB.getUserDS ().receiveIntCmd ("EnemyUniqID");
		Attacker _attackerCtrl;
		for (int  i = 0; i < Attackers_name.Length; i++) {
			GDB.getEnemyObjectDB.LoadData (Attackers_name [i], delegate(int stair, int floor, float hp, int level) {
				if( stair > -1)
				{
					Debug.Log(Attackers_name[i] + " Created // Stair : " + " // Enemy Pos " + floor + " // HP Value : " + hp+" // Level : "+ level);

                    GameObject AttackCharacter = Instantiate(Empty_Attacker) as GameObject;
                    AttackCharacter.name = Attackers_name[i];
                    AttackCharacter.transform.FindChild("BodyContainer").FindChild("Body").GetComponent<UISprite>().atlas = NormalMonsterAtlas;
                    AttackCharacter.tag = "Enemy";

                    _attackerCtrl = AttackCharacter.GetComponent<Attacker>();
                    _attackerCtrl.setUniqueID = stair / 100;

                    _attackerCtrl.control = this.GetComponent<AttackerController>();
			        AttackerList.Add(AttackCharacter); 
					GM.getStageController(stair%100).addStageList(AttackCharacter);

					AttackCharacter.transform.parent = GM.getStageController(stair%100).StartTrs;

					_attackerCtrl.CharacterSetting(level);
					_attackerCtrl.CharacterPositionSet(floor);
					_attackerCtrl.UpdateCharacterHP((int)hp);

						
				}
			});
		}

		for (int i = 0; i < Special_Attackers_Name.Length; i++) {
			GDB.getEnemyObjectDB.LoadData (Special_Attackers_Name [i], delegate(int stair, int floor, float hp, int level) {
				
				if(stair >-1)
				{
                    Debug.Log(Special_Attackers_Name[i] + " Created // Stair : " + stair + " // Enemy Pos " + floor + " // HP Value : " + hp);

                    GameObject AttackNamedCharacter = Instantiate(Empty_Attacker) as GameObject;
					AttackNamedCharacter.name = Special_Attackers_Name[i];
                    AttackNamedCharacter.transform.FindChild("BodyContainer").FindChild("Body").GetComponent<UISprite>().atlas = getNamedAtlas(Special_Attackers_Name[i]);
                    AttackNamedCharacter.tag = "SuperEnemy";

					_attackerCtrl= AttackNamedCharacter.GetComponent<Attacker>();
                    _attackerCtrl.setUniqueID = stair / 100;
                    _attackerCtrl.bossmonsterLabelSet(GM.getContext("Name","BossMonster", Special_Attackers_Name[i]));
					_attackerCtrl.control = this.GetComponent<AttackerController>();
					AttackerList.Add(AttackNamedCharacter);
					GM.getStageController(stair%100).addStageList(AttackNamedCharacter);

					AttackNamedCharacter.transform.parent = GM.getStageController(stair%100).StartTrs;
                    Debug.Log("Floor : " + floor);
					_attackerCtrl.CharacterSetting(level);
					_attackerCtrl.CharacterPositionSet(floor);
					_attackerCtrl.UpdateCharacterHP((int)hp);
				}
			});
		}
			
	
	}

    IEnumerator StartRespawn()
    {
        RespawnEnemy();

        if (!isFever)
        {
            int enemy_level = GameManager.getInstance().LoadLevelData("Normal");

            switch (enemy_level)
            {
                case 0:
                case 1:
                    e_respawn_time = 5.0f;
                    break;
                case 2:
                case 3:
                    e_respawn_time = 4.0f;
                    break;
                case 4:
                case 5:
                    e_respawn_time = 3.0f;
                    break;
                case 6:
                case 7:
                    e_respawn_time = 2.0f;
                    break;
            }
        }
        //yield return new WaitForSeconds(e_respawn_time);
        yield return new WaitForSeconds(3.0f);
        StartCoroutine("StartRespawn");
    }
    
    private StatusData repawnStatus;
    
    void RespawnEnemy()
    {
        bool isSpecial = UnityEngine.Random.Range(0, 100) < 5 ? true : false;

        int rand_Enemy = Random.Range(0, Attackers_name.Length);
		string create_enemy_sprite_name = Attackers_name[rand_Enemy];
       
        GameObject AttackCharacter = Instantiate(Empty_Attacker) as GameObject;
        Attacker AttackerCtrl = AttackCharacter.GetComponent<Attacker>();

        //GM Send Object Data Base To Create Install Enemy
        ++attack_Create_Unique_id;
        AttackerCtrl.setUniqueID = attack_Create_Unique_id;

        if (isSpecial)
        {
            int specialNameLevel = 0;
            switch (GM.LoadLevelData("Normal"))
            {
                case 1:
                    specialNameLevel = 2;
                    break;
                case 2:
                    specialNameLevel = 4;
                    break;
                case 3:
                    specialNameLevel = 6;
                    break;
                case 4:
                    specialNameLevel = 7;
                    break;
                case 5:
                    specialNameLevel = 8;
                    break;
                default:
                    specialNameLevel = 9;
                    break;
            }

            rand_Enemy = Random.Range(0, specialNameLevel);
            create_enemy_sprite_name = Special_Attackers_Name[rand_Enemy];
            AttackCharacter.transform.FindChild("BodyContainer").FindChild("Body").GetComponent<UISprite>().atlas = getNamedAtlas(create_enemy_sprite_name);
        }
        else
        {
            AttackCharacter.transform.FindChild("BodyContainer").FindChild("Body").GetComponent<UISprite>().atlas = NormalMonsterAtlas;
        }

        AttackCharacter.name = create_enemy_sprite_name;
		if (isSpecial) {
			AttackCharacter.tag = "SuperEnemy";
            AttackerCtrl.bossmonsterLabelSet (GM.getContext ("Name", "BossMonster", rand_Enemy));
		} else {
			AttackCharacter.tag = "Enemy";
		}


        AttackerCtrl.control = this.GetComponent<AttackerController>();

        AttackerList.Add(AttackCharacter);
		GM.installObject (AttackCharacter.name, AttackCharacter.tag,  0,attack_Create_Unique_id,GM.LoadLevelData("Normal"));
		GDB.getUserDS ().sendIntCmd ("EnemyCreate",1);


		AttackerCtrl.CharacterSetting();
        AttackerCtrl.CharacterPositionSet(0);
    }

    private string getCreateNamedMonsterName()
    {
        return "";
    }

    public void DeleteListByObject(GameObject obj)
    {
        AttackerList.Remove(obj);
    }

    private IEnumerator ExecuteDarkSkill()
    {
        yield return null;

        for (int i = 0; i < AttackerList.Count; i++)
        {
            Attacker DeleteEnemy = ((GameObject)AttackerList[i]).GetComponent<Attacker>();
            DeleteEnemy.SendMessage("DarkSkillReceive");
        }
    }

    private IEnumerator FeverMode(int enemy_level)
    {
        GM.SendToUI("FeverDisplay", true);
        isFever = true;
        float duringTime = 0.0f;
        switch (enemy_level)
        {
            case 0:
            case 1:
                duringTime = 4.0f;
                break;
            case 2:
            case 3:
                duringTime = 5.0f;
                break;
            case 4:
            case 5:
                duringTime = 6.0f;
                break;
            case 6:
                duringTime = 7.0f;
                break;
            default:
                duringTime = 0.0f;
                break;
        }
        yield return new WaitForSeconds(duringTime);
        GM.SendToUI("FeverDisplay", false);
        isFever = false;
    }

    private UIAtlas getNamedAtlas(string NamedAttackerName)
    {
        int index = 0;
        switch (NamedAttackerName)
        {
            case "JigRinde":
            case "Ainhert":
            case "Leonheart":
                index = 0;
                break;
            case "Darkflame":
            case "Ishark":
            case "Rebelika":
                index = 1;
                break;
            case "Kastral":
            case "Illene":
            case "Gunt":
                index = 2;
                break;
            case "Mrpenguin":
                index = 3;
                break;
            default:
                index = 0;
                break;
        }
        return NamedMonsterAtlas[index];
    }


    void FixedUpdate()
    {
        if (isFever)
            TempStaticMemory.enemykill = 0;

    }


}
