using UnityEngine;
using System.Collections;

public class AleartPanelManager : MonoBehaviour {

    public GameObject NotGoldPanel ;
   
    [HideInInspector]
    public GameObject BGPanel,CloseButton;
    private GameManager GM;

    void Awake()
    {
        GM = GameManager.getInstance();
    }

  
    public void CreateAlert(AlertDialog alert, string value)
    {
        UILabel contextLabel = null;
        BGPanel.SetActive(true);
        Time.timeScale = 0.0f;
        GameObject openPopup = null;
        switch(alert)
        {
            case AlertDialog.NotGold:
                NotGoldPanel.transform.FindChild("NeedGoldLabel").GetComponent<UILabel>().text = value;
                openPopup = NotGoldPanel;
                break;
            case AlertDialog.NotCondition:
                
                break;
            case AlertDialog.UnLock:
                
                break;
        }
        if(openPopup != null) { 
        openPopup.SetActive(true);
        CloseButton.SetActive(true);
        UIEventListener.Get(CloseButton).onClick += new UIEventListener.VoidDelegate(Process => {
            BGPanel.SetActive(false);
            openPopup.SetActive(false);
            CloseButton.SetActive(false);
           // Time.timeScale = 1.0f;
        });
        }
    }
    
}
