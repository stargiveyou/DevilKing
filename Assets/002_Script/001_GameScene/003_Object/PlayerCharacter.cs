using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class PlayerCharacter : MonoBehaviour
{

	private GameManager GM;

	List<string> AnimspriteList = new List<string>();
	private StageController _stageControl;

	//private int _atk, _hp, _speed;
	private float atk, hp, initHp, speed;
	private int range, c_size;
	private bool bossCheck = false;
	private const float fix_floor_monster_size = 1.0f;


	private BoxCollider thisBoxCol;

	public GameObject Attackobj;
	private Weapon PlayerWeapon;

	public UISprite thisSprite;
	private string initSpriteName;

	public UIProgressBar hpBar;
	private string thisTag;
	private bool oneAttacked = true;
	private UILabel characterName;
	// Use this for initialization

	void Start()
	{
		GM = GameManager.getInstance();
		touchedEnemy = new List<Attacker>();

		Attackobj.SetActive(false);
		oneAttacked = true;

		if (!isPlayer)
		{
			if (!isBossMonster)
			{
				int monster_level = GM.LoadMonsterLevelData(this.gameObject.name) - 1;
				string monster_name = GM.getMonsterName(this.gameObject.name, monster_level, true);
				RebuildList("attack_" + monster_name + "_");
			}
			else
			{
				RebuildList("attack_" + this.gameObject.name + "_");
			}
		}

		initSpriteName = thisSprite.spriteName;

		GM.SendToCM("AddPlayerList", this.gameObject);
	}

	private bool dummyAttack = true;
	void ReceivedGameStartSignal()
	{

	}

	public void SetPlayerInfo(string playerName, string playerSpriteType)
	{
		transform.FindChild("NameLabel").GetComponent<UILabel>().text = playerName;
		RebuildList("attack_" + playerSpriteType + "_");
		thisSprite.spriteName = playerSpriteType;
		thisSprite.MakePixelPerfect();
		initSpriteName = thisSprite.spriteName; 
		CharacterStatus();
	}

	void CharacterStatus()
	{
		characterName = transform.FindChild("NameLabel").GetComponent<UILabel>();
		thisBoxCol = GetComponent<BoxCollider>();

		if (GM == null)
		{
			GM = GameManager.getInstance();
		}

		GM.setCharacterData(this.gameObject.name, out initHp, out atk, out range, out speed, out c_size);

		#if UNITY_EDITOR

		if(strBuilder.Length > 0)
			strBuilder.Remove(0, strBuilder.Length);

		strBuilder.AppendLine("[ " + this.gameObject.name + " ]");
		strBuilder.AppendLine("Level\t:\t" + GM.LoadMonsterLevelData(this.gameObject.name));

		strBuilder.AppendLine("Attack\t:\t" + atk);
		strBuilder.AppendLine("Range\t:\t" + range);
		strBuilder.AppendLine("Speed\t:\t" + speed);
		strBuilder.AppendLine("Character Size\t:\t" + c_size);
		strBuilder.AppendLine("Init HP\t:\t" + initHp);

		if(DebugDisplayLabel == null && DebugHPLabel == null)
		{
			CharacterDataInfoDebugDisplay(strBuilder.ToString());
		}
		else
		{
			CharacterDataInfoDebugUpdate(strBuilder.ToString());
		}
		#endif

		PlayerWeapon = Attackobj.GetComponent<Weapon>();
		hp = initHp;

		PlayerWeapon.damage = atk;

		transform.localPosition = Vector3.zero;
		transform.localScale = new Vector3(fix_floor_monster_size, fix_floor_monster_size, fix_floor_monster_size);

		thisTag = this.gameObject.tag;

		if (!isPlayer)
		{
			if (!isBossMonster)
			{
				characterName.gameObject.SetActive(false);
				StageCntl.OccupyPos(AliasPos, c_size);
			}
			else
			{
				characterName.text = this.gameObject.name;
				StageCntl.OccupyPos(AliasPos-1, c_size);
			}
		}  
	}

	IEnumerator AttackProcess()
	{
		if (dummyAttack)
		{
			dummyAttack = false;
			yield return new WaitForSeconds(0.5f);
			yield return StartCoroutine("StartSpriteAnimation");
			yield return new WaitForSeconds(1.0f);
			dummyAttack = true;
		}
	}

	IEnumerator StartSpriteAnimation()
	{
		if (this.gameObject.name.Equals("SkeletonMage"))
		{
			yield return new WaitForSeconds(2.0f);
		}
		else if (this.gameObject.name.Equals("Imp"))
		{
			yield return new WaitForSeconds(1.0f);
		}

		int mIndex = 0;
		int middle_mIndex = AnimspriteList.Count / 2;
		for (mIndex = 0; mIndex < AnimspriteList.Count; mIndex++)
		{
			yield return new WaitForEndOfFrame();
			thisSprite.spriteName = AnimspriteList[mIndex];
			thisSprite.MakePixelPerfect();

			if (mIndex == middle_mIndex - 1)
			{
				Attackobj.SetActive(true);
				PlayerWeapon.SendMessage("ShootSpell");
			}
			yield return new WaitForSeconds(0.1f);

			/*
            if (this.gameObject.name.Equals("Skeleton") || this.gameObject.name.Equals("SkeletonMage"))
            {
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
            }
            */

			if (mIndex == middle_mIndex)
			{
				Attackobj.SetActive(false);
			}
		}
		//Attack Function

		thisSprite.spriteName = initSpriteName;
		thisSprite.MakePixelPerfect();

		//After Delay, Change Origin Mode
	}

	IEnumerator AttackedBlink()
	{
		oneAttacked = false;
		int count = 5;
		do
		{
			thisSprite.alpha = 0.0f;
			yield return new WaitForSeconds(0.1f);
			thisSprite.alpha = 1.0f;
			yield return new WaitForSeconds(0.1f);
		} while (count-- > 0);

		oneAttacked = true;
	}

	private List<Attacker> touchedEnemy;

	void OnTriggerEnter(Collider hit)
	{
		string tag = hit.gameObject.tag;

		if (hit.gameObject.tag.Equals("Enemy") || hit.gameObject.tag.Equals("SuperEnemy"))
		{
			/*
            Attacker attacker = hit.transform.parent.GetComponent<Attacker>();
            if (!touchedEnemy.Contains(attacker))
            {
                touchedEnemy.Add(attacker);
            }
            */
		}

		if (tag.Equals("E_Attack"))
		{
			hp -= hit.gameObject.GetComponent<Weapon>().damage;

			#if UNITY_EDITOR
			DebugHPLabel.text = hp.ToString("#.##");
			#endif
			if (oneAttacked)
			{
				StartCoroutine("AttackedBlink");
			}

			if (hp <= 0)
			{
				/*
                for (int i = 0; i < touchedEnemy.Count; i++)
                {
                    if (touchedEnemy[i].gameObject.activeSelf)
                    {
                        touchedEnemy[i].SendMessage("ForceMove");
                    }
                }
                touchedEnemy.Clear();
                */

				if (isPlayer)
				{
					GM.SendMessage("GameEndAllObject", SendMessageOptions.DontRequireReceiver);
				}
				else if (bossCheck)
				{
					StartCoroutine("DelayRespawn");
					hp = initHp;
					Debug.Log(hp + "Respawn HP");
				}
				else
				{
					GM.unInstallObject(this.gameObject.name, thisTag, StageCntl.StairNumber, int.Parse(this.transform.parent.name));
					GM.SendToCM("RemovePlayerList", this.gameObject);
					//if (thisTag.Equals("Alias") || isBossMonster)
					if (thisTag.Equals("Alias"))
					{
						GM.CreateCemetryByPos(transform.parent, c_size);
						_stageControl.NonOccupyPos(AliasPos, c_size);
						_stageControl = null;
						transform.localPosition = new Vector3(1000, 1000);
						this.gameObject.SetActive(false);
					}
				}
			}
			else
			{
				if (!isPlayer) {
					GM.updateCharacterStatus (this.gameObject.name, thisTag, StageCntl.StairNumber, int.Parse (this.transform.parent.name), hp);
				}
			}
			hpBar.value = (hp / initHp);
		}
	}

	void OnTriggerStay(Collider hit)
	{
		if (hit.gameObject.tag.Equals("Enemy") || hit.gameObject.tag.Equals("SuperEnemy"))
		{
			StartCoroutine("AttackProcess");
		}
	}

	void OnTriggerExit(Collider hit)
	{
		/*
        if (hit.gameObject.tag.Equals("Enemy") || hit.gameObject.tag.Equals("SuperEnemy"))
        {
            Attacker attacker = hit.transform.parent.GetComponent<Attacker>();
            if(touchedEnemy.Contains(attacker))
            {
                touchedEnemy.Remove(attacker);
            }
        }
        */
	}

	void ForceUnlock()
	{
		StopCoroutine("DelayRespawn");
		oneAttacked = true;
		hpBar.gameObject.SetActive(true);
		thisBoxCol.enabled = true;
		thisSprite.spriteName = gameObject.name;
		thisSprite.MakePixelPerfect();
		dummyAttack = true;
		thisSprite.MakePixelPerfect();
	}

	IEnumerator DelayRespawn()
	{
		dummyAttack = false;

		StopCoroutine("AttackedBlink");
		StopCoroutine("AttackProcess");
		StopCoroutine("StartSpriteAnimation");

		thisSprite.spriteName = gameObject.name + "_retire";
		thisSprite.color = Color.white;
		hpBar.gameObject.SetActive(false);
		thisSprite.MakePixelPerfect();
		Attackobj.SetActive(false);
		thisBoxCol.enabled = false;
		yield return _stageControl.StartCoroutine("LockStage");

		oneAttacked = true;
		hpBar.gameObject.SetActive(true);
		thisBoxCol.enabled = true;
		thisSprite.spriteName = gameObject.name;
		thisSprite.MakePixelPerfect();
		dummyAttack = true;

	}


	void ReceiveLegendSkill()
	{
		hp += initHp * 0.3f;
		if (hp > initHp)
		{
			hp = initHp;
		}
		hpBar.value = hp;
	}



	private void RebuildList(string mPrefix)
	{
		if (thisSprite == null) thisSprite = GetComponent<UISprite>();
		AnimspriteList.Clear();

		if (thisSprite != null && thisSprite.atlas != null)
		{
			List<UISpriteData> spriteData = thisSprite.atlas.spriteList;

			for (int i = 0, imax = spriteData.Count; i < imax; ++i)
			{
				UISpriteData sprite = spriteData[i];

				if (string.IsNullOrEmpty(mPrefix) || sprite.name.StartsWith(mPrefix))
				{
					AnimspriteList.Add(sprite.name);
				}
			}
			AnimspriteList.Sort();
		}
	}


	int AliasPos;

	public int AliasPosNumber
	{
		set
		{
			AliasPos = value;
		}
		get
		{
			return AliasPos;
		}
	}

	public StageController StageCntl
	{
		get
		{
			return _stageControl;
		}
		set
		{
			_stageControl = value;
		}
	}

	void ForceDisplayHP()
	{
		Debug.Log("Install");
		hpBar.gameObject.SetActive(true);
	}

	private bool isPlayer
	{
		get
		{
			return this.gameObject.tag.Equals("Player") && this.gameObject.name.Equals("Player");
		}
	}
	public int getCharacterSize
	{
		get
		{

			return c_size;
		}
	}

	public bool isBossMonster
	{
		get
		{
			return bossCheck;
		}
		set
		{
			bossCheck = value;
		}
	}

	public void setArcher()
	{
		PlayerWeapon.Archery = true;
	}

	public float CurrentHP
	{
		get
		{
			return hp;
		}
		set {
			hp = value;
		}
	}

	#if UNITY_EDITOR

	private StringBuilder strBuilder = new StringBuilder();
	private UILabel DebugDisplayLabel,DebugHPLabel;
	void CharacterDataInfoDebugUpdate(string displayInfoContext)
	{        
		DebugDisplayLabel.text = displayInfoContext;
		DebugDisplayLabel.MakePixelPerfect();
		DebugDisplayLabel.transform.localPosition = new Vector2(0, 600);
		DebugHPLabel.transform.localPosition = new Vector2(0, 600 - (DebugDisplayLabel.height / 2));
	}
	void CharacterDataInfoDebugDisplay(string context = "")
	{
		GameObject labelObj = new GameObject();
		GameObject labelObj2 = new GameObject();
		labelObj.name = "CharacterStatus";
		labelObj2.name = "CharacterHP";

		labelObj.transform.parent = this.transform;
		labelObj2.transform.parent = this.transform;

		labelObj.transform.localPosition = new Vector2(0, 600);
		labelObj.transform.localScale = new Vector3(1, 1, 1);

		DebugDisplayLabel = labelObj.AddComponent<UILabel>();
		DebugDisplayLabel.text =context;
		DebugDisplayLabel.trueTypeFont = GM.DebugFont;

		DebugDisplayLabel.fontSize = 35;
		DebugDisplayLabel.color = Color.black;
		DebugDisplayLabel.MakePixelPerfect();


		DebugHPLabel = labelObj2.AddComponent<UILabel>();
		DebugHPLabel.trueTypeFont = GM.DebugFont;
		labelObj2.transform.localPosition = new Vector2(0, 600 - ((DebugDisplayLabel.height) / 2));

		DebugHPLabel.fontSize = 30;
		DebugHPLabel.text = initHp.ToString();
		DebugHPLabel.color = Color.black;
		DebugHPLabel.MakePixelPerfect();

	}

	#endif

}
