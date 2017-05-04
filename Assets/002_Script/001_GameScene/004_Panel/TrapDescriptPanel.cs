using UnityEngine;
using System.Collections;

public class TrapDescriptPanel : MonoBehaviour
{


    private GameManager GM;

    private string trapObjName;
    public GameObject EnhanceButton;

	delegate void DisplayFunction(int level);


    // Use this for initialization
    void Start()
    {
        GM = GameManager.getInstance();
        UIEventListener.Get(EnhanceButton).onClick -= new UIEventListener.VoidDelegate(TrapButtonProcess);
        UIEventListener.Get(EnhanceButton).onClick += new UIEventListener.VoidDelegate(TrapButtonProcess);
        trapSpriteCollider = trapSprite.GetComponent<BoxCollider>();

    }

    void TrapDisplaySetting(string name)
    {
		DisplayFunction Display;
        trapObjName = name;
        switch (name)
        {
		case "Spike":
			//SpikeDisplay (GM.LoadLevelData (name));
			Display = SpikeDisplay;
                break;
		case "Fire":
			//FireDisplay (GM.LoadLevelData (name));
			Display = FireDisplay;
                break;
		case "Stone":
			//StoneDisplay (GM.LoadLevelData (name));
			Display = StoneDisplay;
                break;
		default:
			Display = null;
			break;
        }

		if (Display != null) {
			Display(GM.LoadLevelData(name));
		}

    }

    public UISprite trapSprite;
    public UILabel trapName, trapDescript;
    private BoxCollider trapSpriteCollider;
    #region Display Function

    void SpikeDisplay(int level)
    {
        trapName.text = "스파이크 함정";
        Debug.Log("Spike Level " + level);
        switch (level)
        {
            case 1:
                trapSprite.spriteName = "spike_1";
                break;
            case 2:
                trapSprite.spriteName = "spike_2";
                break;
            case 3:
                trapSprite.spriteName = "spike_3";
                break;
            case 4:
                trapSprite.spriteName = "spike_4";
                break;
        }
        trapSprite.MakePixelPerfect();
        // trapSprite.transform.localScale = new Vector3(2, 2, 2);
        trapSpriteCollider.center = new Vector3(0, trapSprite.height / 2, 0);
        trapSpriteCollider.size = trapSprite.localSize;
    }
    void FireDisplay(int level)
    {
        trapName.text = "화염 함정";
        Debug.Log("Fire Level " + level);
        switch (level)
        {
            case 1:
                trapSprite.spriteName = "2-1_SpitFire_1";
                break;
            case 2:
                trapSprite.spriteName = "2-2_FireBall_1";
                break;
            case 3:
                trapSprite.spriteName = "2-3_FireWall_1";
                break;
            case 4:
                trapSprite.spriteName = "2-4_FlameSword_1";
                break;
        }
        trapSprite.MakePixelPerfect();
        //   trapSprite.transform.localScale = new Vector3(2, 2, 2);
        trapSpriteCollider.center = new Vector3(0, trapSprite.height / 2, 0);
        trapSpriteCollider.size = trapSprite.localSize;
    }
    void StoneDisplay(int level)
    {
        trapName.text = "낙석 함정";
        Debug.Log("Stone Level " + level);
        switch (level)
        {
            case 1:
                trapSprite.spriteName = "3-1_StonFall_Ready";
                break;
            case 2:
                trapSprite.spriteName = "3-2_SquareStone_Ready";
                break;
            case 3:
                trapSprite.spriteName = "3-3_SoneLance_Ready";
                break;
            case 4:
                trapSprite.spriteName = "3-4_StonePist_Ready";
                break;
        }

        trapSprite.MakePixelPerfect();
        //trapSprite.transform.localScale = new Vector3(2, 2, 2);
        trapSpriteCollider.center = new Vector3(0, trapSprite.height / 2, 0);
        trapSpriteCollider.size = trapSprite.localSize;

    }

    #endregion


    void TrapButtonProcess(GameObject go)
    {
        if (go.name.Equals("EnhanceTrap"))
        {
            int index = -1;
          /*  switch (trapObjName)
            {
                case "Fire":
                    index = 1;
                    break;
                case "Spike":
                    index = 0;
                    break;
                case "Stone":
                    index = 2;
                    break;
                default:
                    index = -1;
                    break;
            }
		*/
			//if (index != -1 && GM.LoadLevelData(trapObjName) <= 4)
			if( GM.LoadLevelData(trapObjName)  != -1&&GM.LoadLevelData(trapObjName) <=4 )
            {
				GM.LevelUpData(trapObjName);
				Debug.Log("Save Trap Data : " + "    " + trapObjName + "   " + GM.LoadLevelData(trapObjName));
            }
            else
            {
                Debug.Log("Error or Full Enhance");
            }
            
            TrapDisplaySetting(trapObjName);
        }
    }

}
