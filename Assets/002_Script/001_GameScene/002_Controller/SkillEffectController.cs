using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SkillEffectController : MonoBehaviour {

	public GameObject EffectObject;
	public UIAtlas []effectAtlas;

	[SerializeField]
	private float frame =30;


	private UISprite thisEffectSprite;

	private List<string> death_Effect_spriteName_list;
	private List<string> dark_Effect_spriteName_list;
	private List<string> legend_Effect_spriteName_list;

	void Start () {

		thisEffectSprite = EffectObject.transform.FindChild ("Effect").GetComponent<UISprite> ();

		EffectObject.SetActive (false);

		death_Effect_spriteName_list = new List<string> ();
		dark_Effect_spriteName_list = new List<string> ();
		legend_Effect_spriteName_list = new List<string> ();


	}

	// 0 : Dark
	// 1 : Death
	// 2 : Legend


	public void DisplaySkillEffect(string skillName)
	{
		
		switch (skillName)
		{
		case "Dark":
			StartCoroutine ("EffectAppear",0);
			break;
		case "Death":
			StartCoroutine ("EffectAppear",1);
			break;
		case "Legend":
			StartCoroutine ("EffectAppear",2);
			break;
		default:
			
			break;
		}
	}

	IEnumerator EffectAppear(int index)
	{
		UIAtlas atlas = effectAtlas [index];
		thisEffectSprite.atlas = atlas;

		int list_index = 0;
		EffectObject.SetActive (true);
		do {
			
			thisEffectSprite.spriteName = atlas.spriteList[index].name;
			thisEffectSprite.MakePixelPerfect();

			yield return new WaitForSeconds(1/frame);

		} while(++index < atlas.spriteList.Count);

		EffectObject.SetActive (false);

	}


	// Update is called once per frame
	void Update () {
	
	}
}
