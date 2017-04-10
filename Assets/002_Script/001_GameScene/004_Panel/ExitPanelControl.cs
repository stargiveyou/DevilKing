using UnityEngine;
using System.Collections;

public class ExitPanelControl : MonoBehaviour {
    public GameObject okBtn, closeBtn;
    private GameManager GM;
    void Start()
    {
        GM = GameManager.getInstance();
        UIEventListener.Get(okBtn).onClick += new UIEventListener.VoidDelegate(Process => {
            Application.Quit();
        });
        UIEventListener.Get(closeBtn).onClick += new UIEventListener.VoidDelegate(Process => {
            GM.PopUpOpen("Close");
            this.gameObject.SetActive(false);
            Time.timeScale = 1.0f;
        });
    }
}
