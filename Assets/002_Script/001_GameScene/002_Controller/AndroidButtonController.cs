using UnityEngine;
using System.Collections;

public class AndroidButtonController : MonoBehaviour
{
    public PopUpManage Popup;
    public CollectionManager Collect;
    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android && Input.GetKeyUp(KeyCode.Escape))
        {
            if(!(Popup.ClosePopUp || Collect.CloseCollection))
            {
                Popup.SendMessage("PopUpOpen", "Exit");
            }
        }
    }
}
