using UnityEngine;
using System.Collections;

public class SkillBuyPanelController : MonoBehaviour {

    public ButtonController btnController;
    public GameObject DarkSkillPopup, DeathSkillPopup, LegendSkillPopup;

    public GameObject buyButton, closeButton;
    private GameManager GM;

    private int ready_unlock_skillIndex =-1;


    void Awake()
    {
        GM = GameManager.getInstance();
    }
	// Use this for initialization
	void Start () {
        UIEventListener.Get(closeButton).onClick += new UIEventListener.VoidDelegate(Process => {
            AllPopupDisable();
            this.gameObject.SetActive(false);
            Time.timeScale = 1.0f;
        });
	}
    void AllPopupDisable()
    {
        DarkSkillPopup.SetActive(false);
        DeathSkillPopup.SetActive(false);
        LegendSkillPopup.SetActive(false);
    }
    
    void SkillBuyPopUpCreate(int index)
    {
        TempStaticMemory.GamePause();
        AllPopupDisable();
    
        Debug.Log(index+"   index Test");
      
        ready_unlock_skillIndex = index;

        switch (ready_unlock_skillIndex)
        {
            case 1:
                DarkSkillPopup.SetActive(true);
                break;
            case 2:
                DeathSkillPopup.SetActive(true);
                
                break;
            case 3:
                LegendSkillPopup.SetActive(true);
              
                break;
        }
        UIEventListener.Get(buyButton).onClick += new UIEventListener.VoidDelegate(UnLockButtonProcess);

    }	
    
    private void UnLockButtonProcess(GameObject go)
    {

        int currentLevel = PlayerPrefs.GetInt("SkillLock", 000);
        int price = GM.getPrice("Skill");

        switch (ready_unlock_skillIndex)
        {
            case 1:
             
                    if (TempStaticMemory.gold >= price)
                    {
                        currentLevel += 1;
                        TempStaticMemory.gold -= price;
                        btnController.SendMessage("SkillUnLock", 1, SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        GM.SendToAlert(AlertDialog.NotGold, price.ToString("0"));
                    }
                    TempStaticMemory.GameResume();
                break;
            case 2:
             
                    if (TempStaticMemory.gold >= price)
                    {
                        currentLevel += 10;
                        TempStaticMemory.gold -= price;
                        btnController.SendMessage("SkillUnLock", 2, SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        GM.SendToAlert(AlertDialog.NotGold, price.ToString("0"));
                    }
                break;
            case 3:
                LegendSkillPopup.SetActive(true);
                
                    if (TempStaticMemory.gold >= price)
                    {
                        currentLevel += 100;
                        AllPopupDisable();
                        TempStaticMemory.gold -= price;
                        
                        btnController.SendMessage("SkillUnLock", 3, SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        GM.SendToAlert(AlertDialog.NotGold, price.ToString("0"));
                    }

                break;
        }
        AllPopupDisable();
        TempStaticMemory.GameResume();
        PlayerPrefs.GetInt("SkillLock", currentLevel);

        ready_unlock_skillIndex = -1;
        UIEventListener.Get(buyButton).onClick -= new UIEventListener.VoidDelegate(UnLockButtonProcess);

        this.gameObject.SetActive(false);
        
    }

}
