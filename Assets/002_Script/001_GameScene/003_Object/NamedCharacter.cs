using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class NamedCharacter : MonoBehaviour {

    List<string> AnimspriteList = new List<string>();
    
    public UISprite mSprite;
    public GameObject AttackObj;

    private GameManager GM;
    private StageController _stageControl;
    private float atk, hp, initHp, speed;
    private int range, c_size;

    void Start () {
        GM = GameManager.getInstance();
        GM.setCharacterData(this.gameObject.name, out initHp, out atk,out range, out speed, out c_size);
        
    }

    /// <summary>
    /// Rebuild the sprite list after changing the sprite name.
    /// </summary>

    void RebuildList(string mPrefix)
    {
        
        if (mSprite == null) mSprite = GetComponent<UISprite>();
        AnimspriteList.Clear();

        if(mSprite != null && mSprite.atlas != null)
        {
            List<UISpriteData> spriteData = mSprite.atlas.spriteList;

            for(int i =0 , imax = spriteData.Count; i<imax; ++i)
            {
                UISpriteData sprite = spriteData[i];

                if(string.IsNullOrEmpty(mPrefix) || sprite.name.StartsWith(mPrefix))
                {
                    AnimspriteList.Add(sprite.name);
                }
            }
            AnimspriteList.Sort();
        }
    }
    
    void OnTriggerEnter(Collider hit)
    {
        string hit_tagName = hit.gameObject.tag;
        if (hit_tagName.Equals("Enemy") || hit_tagName.Equals("SuperEnemy"))
        {
            
        }
    }
    private bool AttackOnce =true;
    void OnTriggerStay(Collider hit)
    {
        string hit_tagName = hit.gameObject.tag;
        if ((hit_tagName.Equals("Enemy") || hit_tagName.Equals("SuperEnemy") )&& AttackOnce)
        {
           StartCoroutine("StartAttack");
        }
        
    }
    
    //Animation Coroutine Test

    IEnumerator StartAttack()
    {
        AttackOnce = false;
        yield return new WaitForSeconds(0.5f);
        AttackObj.SetActive(true);
        yield return StartCoroutine("StartSpriteAnimation");
   
   
        yield return new WaitForSeconds(3.0f);
        AttackOnce = true;
        
    }

    IEnumerator StartSpriteAnimation()
    {
        int mIndex = 0;
        do
        {
            mSprite.spriteName = AnimspriteList[mIndex];
            mSprite.MakePixelPerfect();
            
            yield return new WaitForSeconds(0.1f);
        }
        while (++mIndex < AnimspriteList.Count );

        //Attack Function
        yield return new WaitForSeconds(0.15f);
        AttackObj.SetActive(false);
        //After Delay, Change Origin Mode
        mSprite.spriteName = this.gameObject.name;
        mSprite.MakePixelPerfect();

    }
    
}
