using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour
{

    private GameManager GM;
    public SkillBuyPanelController SkillBuyControl;
    void Awake()
    {
        GM = GameManager.getInstance();
    }
    // Use this for initialization
    void Start()
    {
        SKillLockProcess();
        ButtonActive();
    }

  public  void TutorialBtnControl(int index =0)
    {
        GameObject ButtonObj = null;
        switch (index)
        {
            case 0:
                int Button_count = transform.childCount;

                for (int i = 0; i < Button_count; i++)
                {
                    ButtonObj = transform.GetChild(i).gameObject;
                    string buttonName = ButtonObj.name;
                    if (buttonName.Contains("Obstacle") || buttonName.Contains("Alias"))
                    {
                        UIEventListener.Get(ButtonObj).onClick -= new UIEventListener.VoidDelegate(InstallButtonProcess);
                    }
                    else if (ButtonObj.tag.Equals("Skill"))
                    {
                        UIEventListener.Get(ButtonObj).onClick -= new UIEventListener.VoidDelegate(SkillButtonProcess);
                    }
                    else
                    {
                        UIEventListener.Get(ButtonObj).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
                    }
                }
                break;
            case 1:

                ButtonObj = transform.FindChild("Enchance").gameObject;
                UIEventListener.Get(ButtonObj).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
                UIEventListener.Get(ButtonObj).onClick += new UIEventListener.VoidDelegate(ButtonProcess);

                break;
            case 2:
                ButtonActive();
                break;
            default:
                break;
        }
        
   }

    void ButtonActive()
    {
        int Button_count = transform.childCount;

        for (int i = 0; i < Button_count; i++)
        {
            GameObject ButtonObj = transform.GetChild(i).gameObject;
            string buttonName = ButtonObj.name;
            if (buttonName.Contains("Obstacle") || buttonName.Contains("Alias"))
            {
                UIEventListener.Get(ButtonObj).onClick -= new UIEventListener.VoidDelegate(InstallButtonProcess);
                UIEventListener.Get(ButtonObj).onClick += new UIEventListener.VoidDelegate(InstallButtonProcess);
            }
            else if (ButtonObj.tag.Equals("Skill"))
            {
                UIEventListener.Get(ButtonObj).onClick -= new UIEventListener.VoidDelegate(SkillButtonProcess);
                UIEventListener.Get(ButtonObj).onClick += new UIEventListener.VoidDelegate(SkillButtonProcess);
            }
            else
            {
                UIEventListener.Get(ButtonObj).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
                UIEventListener.Get(ButtonObj).onClick += new UIEventListener.VoidDelegate(ButtonProcess);
            }
        }
    }

    void SKillLockProcess()
    {
        int skillLockLevel = PlayerPrefs.GetInt("SkillLock", 000);

        GameObject dark_lock_button = transform.FindChild("Dark").FindChild("Locked").gameObject;
        GameObject death_lock_button = transform.FindChild("Death").FindChild("Locked").gameObject;
        GameObject legend_lock_button = transform.FindChild("Legend").FindChild("Locked").gameObject;

        dark_lock_button.SetActive((skillLockLevel % 10 == 0));
        death_lock_button.SetActive((skillLockLevel / 10) % 10 == 0);
        legend_lock_button.SetActive(skillLockLevel / 100 == 0);    
    }
    
    void SkillButtonProcess(GameObject go)
    {
        if(go.transform.FindChild("Locked").gameObject.activeSelf)
        {
            int index = 0;
            
            switch (go.name)
            {
                case "Dark":
                    index = 1;
                    break;
                case "Death":
                    index = 2;
                    break;
                case "Legend":
                    index = 3;
                    break;
            }
            SkillBuyControl.gameObject.SetActive(true);
            SkillBuyControl.SendMessage("SkillBuyPopUpCreate", index);
        }
        else {
			Debug.Log("Act Skill : " + go.name);
			GM.ExecuteSpeSkill(go.name);
            StartCoroutine("DelayUnLock", go);
        }
    }
    void SkillUnLock(int index)
    {
        GameObject lockObj = null;
        GameObject skillBtn = null;
        //Dark Death Legend

        Debug.Log(index + "/// SkillUnLock");
        switch(index)
        {
            case 1:
                skillBtn = transform.FindChild("Dark").gameObject;
                break;
            case 2:
                skillBtn = transform.FindChild("Death").gameObject;
                break;
            case 3:
                skillBtn = transform.FindChild("Legend").gameObject;
                break;
            default:
                break;
        }

        
        if (skillBtn != null)
        {
            skillBtn.transform.FindChild("Locked").gameObject.SetActive(false);
        }

    }
    IEnumerator DelayUnLock(GameObject button)
    {
        float time_delta = (float)1 / 180 ;
        
        UISprite lock_Sprite = button.transform.FindChild("Locked").GetComponent<UISprite>();
        UIEventListener.Get(button).onClick -= new UIEventListener.VoidDelegate(SkillButtonProcess);
        lock_Sprite.gameObject.SetActive(true);
        float fillAmount = 1.0f;
        lock_Sprite.fillAmount = 1.0f;

        do
        {
            fillAmount -= time_delta;
            yield return new WaitForFixedUpdate();
            lock_Sprite.fillAmount = fillAmount;

        } while (fillAmount >= 0);
        fillAmount = 0.0f;
        lock_Sprite.fillAmount = 0.0f;
        lock_Sprite.gameObject.SetActive(false);
        UIEventListener.Get(button).onClick += new UIEventListener.VoidDelegate(SkillButtonProcess);

    }
    
    void InstallButtonProcess(GameObject go)
    {
        switch (go.name)
        {
            case "ObstacleTest":
            case "AliasTest":
                GameObject obstacle = Instantiate(go.transform.GetChild(0).gameObject) as GameObject;
                obstacle.SetActive(false);
  //              GM.PrepareInstall(obstacle);
                transform.parent.parent.gameObject.SetActive(false);
                break;

            default:
                break;
        }
    }
    
    void ButtonProcess(GameObject go)
    {

        switch (go.name)
        {
            case "CreateStair":
                GM.SendToUI("CreateStair");
                break;
             case "Collection":
                GM.PopUpOpen("Collection");
                break;
             case "Option":
            case "Enchance":
                GM.PopUpOpen(go.name);
                break;            
            default:
                break;
        }
    }


}
