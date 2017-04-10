using UnityEngine;
using System.Collections;

public class TrophyDisplayPanelManager : MonoBehaviour {

    public UISprite trophysprite;
    public UILabel trophyContext;
    public GameObject Cancle;

    void Start()
    {
        UIEventListener.Get(Cancle).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
        UIEventListener.Get(Cancle).onClick += new UIEventListener.VoidDelegate(ButtonProcess);
    }
    public void SetTrophy(string spritename, string context)
    {
        trophysprite.spriteName = spritename;
        trophyContext.text = context;
    }

    void ButtonProcess(GameObject go)
    {
        this.gameObject.SetActive(false);
    }
}
