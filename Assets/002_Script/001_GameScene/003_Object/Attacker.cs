using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Debug
using System.Text;
public class Attacker : MonoBehaviour
{

	private GameManager GM;

	private int unique_id =0;
    private float atk, hp, initHp, initSpeed, speed, atkSpeed;
    private int enemy_level = 0, size, range;
    private int currentStair = 0;
    private float localposY;

    private UISprite thisSprite;
    private BoxCollider bodyCollider;
    private List<string> moveSpriteNameLists, attackSpriteNameLists;
	private UILabel attackerNameLabel;

    private AttackerController Controller;

    private GameObject CharacterBody;

    private UIProgressBar hpBar;
    

    public GameObject Attackobj;
    private GameObject healObj;

    private bool isAttackable = true;
    private bool oneAttacked = true;

    private bool isEnemyFace = false;
    private bool isEnemyHeal = false;

    private ObstacleCharacter obs_Object = null;
    private string stat_name = "";

    // Use this for initialization
	void Awake()
	{
        moveSpriteNameLists = new List<string>();
        attackSpriteNameLists = new List<string>();
		GM = GameManager.getInstance();
    }
    void OnEnable()
    {
        currentStair = 0;

        attackerNameLabel = transform.FindChild("attacker_name").GetComponent<UILabel>();

        CharacterBody = transform.FindChild("BodyContainer").gameObject;
        bodyCollider = CharacterBody.GetComponent<BoxCollider>();

        thisSprite = CharacterBody.GetComponent<UISprite>();
        healObj = transform.FindChild("Heal").gameObject;
        hpBar = transform.FindChild("HPBar").GetComponent<UIProgressBar>();


    }
    void Start()
    {
      
        localposY = transform.localPosition.y;

    //    CharacterSetting();
        Create_Move_Attack_List(gameObject.name + "_");

        StartCoroutine("MoveSpriteAnimation");
		StartCoroutine ("EnemySaveCoroutine");
    }

    private Vector3 swordAttackPos = new Vector3(-50f, 0, 0);
    private Vector3 swordAttackSize = new Vector3(230f, 50, 100);

    private Vector3 archeryAttackPos = new Vector3(-185, 0, 0);
    private Vector3 archeryAttackSize = new Vector3(600, 100, 100);

	IEnumerator EnemySaveCoroutine()
	{
		yield return new WaitForSeconds (1.0f);
		GM.updateEnemyStatus(gameObject.name,gameObject.tag,currentStair,unique_id,transform.localPosition.x,hp);
		StartCoroutine ("EnemySaveCoroutine");
	}

	public void CharacterSetting(int level =0)
    {
        UISprite AttackSprite = CharacterBody.transform.FindChild("Body").GetComponent<UISprite>();
        BoxCollider AttackCollider = Attackobj.GetComponent<BoxCollider>();
		attackerNameLabel.text = string.Empty;

        Attackobj.GetComponent<UISprite>().enabled = false;
        string _name = gameObject.name;
        
        AttackCollider.center = swordAttackPos;
        AttackCollider.size = swordAttackSize;

        if (gameObject.tag.Equals("Enemy"))
        {
            stat_name = "Normal";
            switch (_name)
            {
                case "Magician":
                    stat_name = "Archer";
                    break;
                case "Sheilder":
                    stat_name = "Shield";
                    break;
                default:
                    stat_name = "Normal";
                    break;
            }
        }
        else if (gameObject.tag.Equals("SuperEnemy"))
        {
            stat_name = gameObject.name;
			attackerNameLabel.text = gameObject.name;
        }

        AttackSprite.spriteName = gameObject.name;
        



        /*
        if (gameObject.tag.Equals("SuperEnemy"))
        {
            transform.localScale *= 1.2f;
        }
        */
//        CharacterPositionSet();
		if (level == 0) {
			GM.setCharacterData (stat_name, out hp, out atk, out range, out atkSpeed, out size);
		} else {
			GM.setCharacterData (stat_name,level, out hp, out atk, out range, out atkSpeed, out size);
		}
        
#if UNITY_EDITOR

        strBuilder.AppendLine("[ " + this.gameObject.name + " ]");
		strBuilder.AppendLine("Level\t:\t" + (GM.LoadLevelData(stat_name)));
        strBuilder.AppendLine("Attack\t:\t" + atk);
        strBuilder.AppendLine("Range\t:\t" + range);
        strBuilder.AppendLine("Speed\t:\t" + speed);
        strBuilder.AppendLine("Character Size\t:\t" + size);
        strBuilder.AppendLine("Init HP\t:\t" + hp);

        CharacterDataInfoDebugDisplay(strBuilder.ToString());
#endif

        if (stat_name.Equals("Illene"))
        {
            Attackobj.tag = "Untagged";
            CharacterBody.transform.localPosition = new Vector3(-103, -35, 0);
            CharacterBody.GetComponent<UISprite>().pivot = UIWidget.Pivot.Bottom;
            bodyCollider.center = new Vector2(40, 117.3f);
            healObj.GetComponent<AttackerHealGS>().SetHealAmount(atk);
            StartCoroutine("HealSkillAuto");
        }
        else
        {
            Attackobj.tag = "E_Attack";
            bodyCollider.center = new Vector2(153, 117.3f);
        }

        initHp = hp;
        initSpeed = speed = 0.3f;
        enemy_level = GM.LoadLevelData(stat_name);
        Attackobj.GetComponent<Weapon>().damage = atk;
    }

	void CharacterPositionSet()
	{	
		float character_pos_Y = 0;
		BoxCollider AttackCollider = Attackobj.GetComponent<BoxCollider>();
		switch (this.gameObject.name)
		{
		case "Magician":
			AttackCollider.center = archeryAttackPos;
			AttackCollider.size = archeryAttackSize;
			Attackobj.GetComponent<UISprite>().enabled = true;
			break;
		case "Sheilder":
			character_pos_Y = 10.0f;
			break;
		case "AxeKnight":
			character_pos_Y = 10.0f;
			break;
		case "Illene":

			break;
		default:
			character_pos_Y = 0.0f;
			break;
		}

		transform.localPosition = new Vector3(0, character_pos_Y, 0);
		transform.localScale = new Vector3(1, 1, 1);

	}
	public void CharacterPositionSet(float x)
	{
        if(x == 0)
        {
            if(GM == null)
            {
                GM = GameManager.getInstance();
            }
			GM.SetAttackTrsByLevel(this.gameObject, currentStair);    
        }

		float character_pos_Y = 0;
		BoxCollider AttackCollider = Attackobj.GetComponent<BoxCollider>();
		switch (this.gameObject.name)
		{
		case "Magician":
			AttackCollider.center = archeryAttackPos;
			AttackCollider.size = archeryAttackSize;
			Attackobj.GetComponent<UISprite>().enabled = true;
			break;
		case "Sheilder":
			character_pos_Y = 10.0f;
			break;
		case "AxeKnight":
			character_pos_Y = 10.0f;
			break;
		case "Illene":

			break;
		default:
			character_pos_Y = 0.0f;
			break;
		}
       
		transform.localPosition = new Vector3(x, character_pos_Y, 0);
		transform.localScale = new Vector3(1, 1, 1);
	}

	public void bossmonsterLabelSet(string nameStr)
	{
		attackerNameLabel.text = string.Empty;
		attackerNameLabel.text = nameStr;
	}

    void Create_Move_Attack_List(string prefixName)
    {
        if (thisSprite == null) thisSprite = this.transform.FindChild("BodyContainer").FindChild("Body").GetComponent<UISprite>();
        moveSpriteNameLists.Clear();
        attackSpriteNameLists.Clear();

        if (thisSprite != null && thisSprite.atlas != null)
        {
            List<UISpriteData> spriteData = thisSprite.atlas.spriteList;

            for (int i = 0, imax = spriteData.Count; i < imax; ++i)
            {
                UISpriteData sprite = spriteData[i];
                if (string.IsNullOrEmpty("attack_" + prefixName) || sprite.name.StartsWith("attack_" + prefixName))
                {
                    attackSpriteNameLists.Add(sprite.name);
                }
                else if (string.IsNullOrEmpty(prefixName) || sprite.name.StartsWith(prefixName))
                {
                    moveSpriteNameLists.Add(sprite.name);
                }

            }
            moveSpriteNameLists.Sort();
            attackSpriteNameLists.Sort();
        }
    }

    public void UpdateCharacterHP(int lastHP)
    {
        hp = lastHP;

		#if UNITY_EDITOR
        if(DebugHPLabel!=null)
		DebugHPLabel.text = hp.ToString();
		#endif

        hpBar.value = (hp / initHp);
    }
 
    void OnTriggerEnter(Collider hit)
    {
        string hit_tag = hit.gameObject.tag;

        if (hit_tag.Equals("Exit"))
        {
            gameObject.SetActive(false);
        }
        else if (hit_tag.Equals("P_Attack"))
        {
            Weapon weaponStat = hit.gameObject.GetComponent<Weapon>();

            if (weaponStat.Archery && stat_name.Equals("Kastral"))
            {

            }
            else
            {
                float damage = weaponStat.damage;
                DamageCharacter(damage);
            }
        }
        else if (hit_tag.Equals("Obstacle") && !stat_name.Equals("Ishark"))
        {
            float damage = 0;
            if (hit.gameObject.name.Equals("Effect"))
            {
                obs_Object = hit.transform.parent.GetComponent<ObstacleCharacter>();
                DamageByObstacleFunc(obs_Object.obs_damage);
            }
            else if (hit.gameObject.name.Equals("Spike"))
            {
                speed = initSpeed * 0.7f;
                obs_Object = hit.gameObject.GetComponent<ObstacleCharacter>();
                StartCoroutine("DamageByObstacle", obs_Object.obs_damage);
            }
        }
        else if (hit_tag.Equals("Ghost") || hit_tag.Equals("Starter"))
        {
            if (hit_tag.Equals("Ghost"))
            {
                hit.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
            isEnemyFace = false;
            StopCoroutine("AttackProcess");
            StartCoroutine("MoveSpriteAnimation");
        }
        else if (hit_tag.Equals("Stair"))
        {
            GM.ChangeAttackTrsByLevel(this.gameObject, ++currentStair);
			//Update Pos
			GM.updateEnemyStatus(gameObject.name,gameObject.tag,currentStair,unique_id,0.0f,hp);
			CharacterPositionSet();
        }

        if ((hit_tag.Equals("Alias") || hit_tag.Equals("Player")))
        {
            //   StartCoroutine("KnockBack");
            isEnemyFace = true;
			StopCoroutine("MoveSpriteAnimation");
            if (!stat_name.Equals("Illene"))
            {
                StartCoroutine("AttackProcess");
            }
        }


		if(hit_tag.Equals("Enemy") || hit_tag.Equals("SuperEnemy"))
		{
		//	StopCoroutine("MoveSpriteAnimation");
			Attacker otherAttacker = hit.transform.parent.GetComponent<Attacker> ();
			//Debug.Log ("??"+otherAttacker.name + "/"+stat_name.ToString()+"/"+otherAttacker.getStatName);
			if (stat_name.Equals("Archer") && !otherAttacker.getStatName.Equals ("Archer") && !isEnemyFace) {
//				Debug.Log ("Must Be Stop");
				isEnemyFace = true;
				StopCoroutine("MoveSpriteAnimation");
				StartCoroutine("AttackProcess");
			}
		}
        if (hit.gameObject.name.Equals("Heal"))
        {
            DamageCharacter(-1 * hit.gameObject.GetComponent<AttackerHealGS>().GetHealAmount);

        }
    }

    void OnTriggerExit(Collider hit)
    {
        string hit_tag = hit.gameObject.tag;

		if (isEnemyFace) { 
            isEnemyFace = false;
            isAttackable = true;

            StopCoroutine("AttackProcess");
            StopCoroutine("StartSpriteAnimation");
            StartCoroutine("MoveSpriteAnimation");
        }
    }

    IEnumerator MoveSpriteAnimation()
    {
        thisSprite.spriteName = moveSpriteNameLists[0];
        int mIndex = 0;
        for (mIndex = 0; mIndex < moveSpriteNameLists.Count; mIndex++)
        {
            thisSprite.spriteName = moveSpriteNameLists[mIndex];
            thisSprite.MakePixelPerfect();
            yield return new WaitForSeconds(0.2f);
        }

        StartCoroutine("MoveSpriteAnimation");
    }
    IEnumerator AttackProcess()
    {
        if (isAttackable)
        {
            isAttackable = false;
            yield return new WaitForSeconds(0.75f);
            yield return StartCoroutine("StartSpriteAnimation");
            yield return new WaitForSeconds(1.0f);
            isAttackable = true;
            StartCoroutine("AttackProcess");
        }
    }

    IEnumerator StartSpriteAnimation()
    {
        int mIndex = 0;
        if (attackSpriteNameLists.Count == 0)
        {
            if (moveSpriteNameLists.Count == 0)
            {
                Debug.Log("move Sprite List Null");
            }
            thisSprite.spriteName = moveSpriteNameLists[0];
        }
        else
        {
            thisSprite.spriteName = attackSpriteNameLists[0];
        }

        for (mIndex = 0; mIndex < attackSpriteNameLists.Count; mIndex++)
        {
            yield return new WaitForEndOfFrame();
            thisSprite.spriteName = attackSpriteNameLists[mIndex];
            thisSprite.MakePixelPerfect();
            if (mIndex == attackSpriteNameLists.Count / 2)
            {
                Attackobj.SetActive(true);

                yield return new WaitForEndOfFrame();
                Attackobj.SetActive(false);
            }
        }

        yield return new WaitForSeconds(0.5f * atkSpeed);
        if (attackSpriteNameLists.Count == 0)
        {
            thisSprite.spriteName = moveSpriteNameLists[0];
        }
        else
        {
            thisSprite.spriteName = attackSpriteNameLists[0];
        }
        thisSprite.MakePixelPerfect();
        //After Delay, Change Origin Mode

    }


    IEnumerator AttackedBlink()
    {
        oneAttacked = false;
        int count = 6;
        do
        {
            thisSprite.alpha = 0.0f;
            yield return new WaitForSeconds(0.05f);
            thisSprite.alpha = 1.0f;
            yield return new WaitForSeconds(0.05f);

        } while (count-- > 0);
        oneAttacked = true;
    }
    void DamageByObstacleFunc(float damage)
    {

        if (obs_Object != null && oneAttacked)
        {
            obs_Object.SendMessage("Discount", SendMessageOptions.DontRequireReceiver);
            DamageCharacter(damage);
        }
    }
    IEnumerator DamageByObstacle(float damage)
    {
        if (obs_Object != null && oneAttacked)
        {
            obs_Object.SendMessage("Discount", SendMessageOptions.DontRequireReceiver);
            DamageCharacter(damage);
        }
        yield return new WaitForSeconds(0.4f);

    }

    void DamageCharacter(float damage)
    {

        hp -= damage;
#if UNITY_EDITOR
        if(DebugHPLabel != null)
        DebugHPLabel.text = hp.ToString();
#endif

        if (hp <= 0)
        {
            int feverNeedCount = 30;
            int getGoldAmount = 0;
            GM.CreateGhostByPos(this.transform, 1);

            getGoldAmount = GM.getPrice("EnemyGet");

            switch (enemy_level)
            {
                case 0:
                case 1:
                    feverNeedCount = 30;
                    break;
                case 2:
                case 3:
                    feverNeedCount = 40;
                    break;
                case 4:
                case 5:
                    feverNeedCount = 50;
                    break;
                case 6:
                    feverNeedCount = 70;
                    break;

            }

			StopCoroutine ("EnemySaveCoroutine");

			GM.unInstallObject (this.gameObject.name, this.gameObject.tag, unique_id, currentStair);

            TempStaticMemory.gold += getGoldAmount;
			GM.setUserData ("TotalGold", getGoldAmount);

            TempStaticMemory.enemykill++;

			GM.setUserData("EnemyKill");
			GM.sendTrophyCondition ("EnemyKill", GM.getUserIntData ("enemyKillCount"));

			if (TempStaticMemory.enemykill== feverNeedCount)
            {
                Controller.SendMessage("FeverMode", enemy_level);
            }

            Controller.DeleteListByObject(this.gameObject);

            gameObject.SetActive(false);
            hpBar.value = 1.0f;
            initHp = hp = 3;

        }
        else if (hp >= initHp)
        {
            hp = initHp;
			GM.updateEnemyStatus(gameObject.name,gameObject.tag,currentStair,unique_id,transform.localPosition.x,hp);
            hpBar.value = (hp / initHp);
        }
        else
        {
            hpBar.value = (hp / initHp);
			GM.updateEnemyStatus(gameObject.name,gameObject.tag,currentStair,unique_id,transform.localPosition.x,hp);
            StartCoroutine("AttackedBlink");
        }
    }

    void DarkSkillReceive()
    {
        DamageCharacter(initHp * 0.3f);
    }

    IEnumerator HealSkillAuto()
    {
        isEnemyHeal = true;
        StopCoroutine("MoveSpriteAnimation");
        healObj.SetActive(true);
        for (int mIndex = 0; mIndex < attackSpriteNameLists.Count; mIndex++)
        {
            yield return new WaitForEndOfFrame();
            thisSprite.spriteName = attackSpriteNameLists[mIndex];
            thisSprite.MakePixelPerfect();
            yield return new WaitForSeconds(0.1f);
        }
        healObj.SetActive(false);
        isEnemyHeal = false;
        if (!isEnemyFace)
        {
            StartCoroutine("MoveSpriteAnimation");
        }
        yield return new WaitForSeconds(4.0f);
        StartCoroutine("HealSkillAuto");

    }

    #region Test
    public bool isKnockBack;


    IEnumerator KnockBack()
    {
        isEnemyFace = true;
        yield return StartCoroutine("BackMoving");
        yield return new WaitForSeconds(1.0f);
        isEnemyFace = false;
    }
    private float distanceX = 3.0f;
    IEnumerator BackMoving()
    {
        StartCoroutine("JumpBackMoving");
        float dix = 2.5f * (1 + testValue);
        do
        {
            transform.Translate(Vector3.left * dix * Time.deltaTime);
            dix -= 0.1f * (1 + testValue);
            yield return new WaitForFixedUpdate();
        } while (dix >= 0.0f);

    }
    public float testValue;
    IEnumerator JumpBackMoving()
    {

        float diy = 1.5f * (1 + testValue);
        do
        {
            transform.Translate(Vector3.up * diy * Time.deltaTime);
            diy -= 0.3f * (1 + testValue);
            yield return new WaitForFixedUpdate();
        } while (diy >= 0.0f);
        yield return null;

        do
        {
            diy = transform.localPosition.y;
            transform.Translate(Vector3.down * 0.98f * (1 + testValue) * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        } while (diy > localposY + 0.98f);
        Vector3 pos = transform.localPosition;
        pos.y = localposY;
        transform.localPosition = pos;
    }

    #endregion

	/*

    void ForceMove() //??
    {
        StopCoroutine("AttackProcess");
        //StopCoroutine("StartSpriteAnimation");
        Attackobj.SetActive(false);

        Debug.Log("Force Use//" + this.gameObject.name);
        thisSprite.spriteName = moveSpriteNameLists[0];
        thisSprite.MakePixelPerfect();
        
        StartCoroutine("MoveSpriteAnimation");
        isAttackable = true;
    }

*/


    void FixedUpdate()
    {
        if (TempStaticMemory.isGamePlaying && !isEnemyFace && !isEnemyHeal)
        {

            transform.Translate(Vector3.right * Time.fixedDeltaTime * speed);
        }
        if (isKnockBack)
        {
            StartCoroutine("KnockBack");
            isKnockBack = false;
        }

		//

    }
    
    public bool PlayerFaced
    {
        get
        {
            return isEnemyFace;
        }
    }

    public AttackerController control
    {
        set
        {
            Controller = value;
        }
    }

	public int setUniqueID
	{
		set
		{
			unique_id = value;
		}
		private get {
			return unique_id;
		}
	}

	public string getStatName
	{
		get {
			return stat_name;
		}
	}

#if UNITY_EDITOR
    private StringBuilder strBuilder = new StringBuilder();
    private UILabel DebugDisplayLabel, DebugHPLabel;

    void CharacterDataInfoDebugDisplay(string context = "")
    {
        GameObject labelObj = new GameObject();
        GameObject labelObj2 = new GameObject();
        labelObj.name = "EnemyStatus";
        labelObj2.name = "EnemyHP";

        labelObj.transform.parent = this.transform;
        labelObj2.transform.parent = this.transform;

        labelObj.transform.localPosition = new Vector2(0, 500);
        labelObj.transform.localScale = new Vector3(1, 1, 1);

        DebugDisplayLabel = labelObj.AddComponent<UILabel>();
        DebugDisplayLabel.text = context;
        DebugDisplayLabel.trueTypeFont = GM.DebugFont;

        DebugDisplayLabel.fontSize = 35;
        DebugDisplayLabel.color = Color.black;
        DebugDisplayLabel.MakePixelPerfect();


        DebugHPLabel = labelObj2.AddComponent<UILabel>();
        DebugHPLabel.trueTypeFont = GM.DebugFont;
        labelObj2.transform.localPosition = new Vector2(0, 500 - ((DebugDisplayLabel.height) / 2));

        DebugHPLabel.fontSize = 30;
        DebugHPLabel.text = hp.ToString();
        DebugHPLabel.color = Color.black;
        DebugHPLabel.MakePixelPerfect();

    }
#endif


}
