using UnityEngine;
using System.Collections;

public class TrophyDisplayPanelManager : MonoBehaviour {

    public UISprite trophysprite;
    public UILabel trophyContext;
    public GameObject Cancle;
	public GameObject PopUP;
	public UIAtlas[] tropysAtlas;


    void Start()
    {
        UIEventListener.Get(Cancle).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
        UIEventListener.Get(Cancle).onClick += new UIEventListener.VoidDelegate(ButtonProcess);
    }
	/*
    public void SetTrophy(string spritename, string context)
    {
        trophysprite.spriteName = spritename;
        trophyContext.text = context;
		PopUP.SetActive (true);
		Time.timeScale = 0.0f;
    }
    */
	

	public void SetTropy(int index, string spritename, string context)
	{
		
		Resources.UnloadUnusedAssets();

		UIAtlas selectedAtlas = tropysAtlas [(index-1) / 16];
		trophysprite.atlas = selectedAtlas;
		trophysprite.spriteName = spritename;
		trophyContext.text = context;
		PopUP.SetActive (true);
		Time.timeScale = 0.0f;
	}

    void ButtonProcess(GameObject go)
    {
		Time.timeScale = 1.0f;
		PopUP.SetActive (false);
    }
}
