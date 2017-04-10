using UnityEngine;
using System.Collections;

public class TrapButton : MonoBehaviour {

    private GameManager GM;

	// Use this for initialization
	void Start () {
        int c_length = transform.childCount;
        GM = GameManager.getInstance();
        for(int i =0; i<c_length; i++)
        {
            UIEventListener.Get(transform.GetChild(i).gameObject).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
            UIEventListener.Get(transform.GetChild(i).gameObject).onClick += new UIEventListener.VoidDelegate(ButtonProcess);
        }
	}
	
    void ButtonProcess(GameObject go)
    {  
        string obj_name = go.name;

        GameObject TrapObj = Instantiate(go.transform.FindChild("Background").gameObject) as GameObject;
        TrapObj.name = obj_name;
        UISprite TrapSprite = TrapObj.GetComponent<UISprite>();
        TrapSprite.color = Color.white;
        TrapObj.AddComponent<ObstacleCharacter>();
        TrapObj.SetActive(false);
//      GM.PrepareInstall(TrapObj);
    }


}
