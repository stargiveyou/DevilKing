using UnityEngine;
using System.Collections;

public class TrapEnhanceManager : MonoBehaviour {

    public TrapDescriptPanel DescriptPanel;

    // Use this for initialization
    
    void Start()
    {
        Transform TrapButtons = transform.FindChild("TrapButton");
        
        for (int i = 0; i < TrapButtons.childCount; i++)
        {
            UIEventListener.Get(TrapButtons.GetChild(i).gameObject).onClick -= new UIEventListener.VoidDelegate(DescriptShowProcess);
            UIEventListener.Get(TrapButtons.GetChild(i).gameObject).onClick += new UIEventListener.VoidDelegate(DescriptShowProcess);
        }
        
    }

    void DescriptShowProcess(GameObject go)
    {
        DescriptPanel.gameObject.SetActive(true);
        DescriptPanel.SendMessage("TrapDisplaySetting", go.name, SendMessageOptions.DontRequireReceiver);
    }
    
    void TrapButtonProcess(GameObject go)
    {
        Debug.Log("Clickk");
    }

    void OnDisable()
    {
        DescriptPanel.gameObject.SetActive(false);
    }
}

