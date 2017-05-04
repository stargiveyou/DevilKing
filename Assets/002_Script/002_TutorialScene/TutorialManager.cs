using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;


public class TutorialManager : MonoBehaviour
{
    private GameDataBase GDB;

    public Transform CameraTrs;

    public GameObject LoginPanel;
    public GameObject LoadingPanel;
    public GameObject tutorialPanel;


    public Transform ButtonTrs;
    public UISpriteAnimation KingAnimation, QueenAnimation;
    public GameObject EnterButton;
    public UILabel selectContext;
    public UIInput inputObject;
    public UILabel contextLabel;

    public UISlider loadingSlider;

    private GameObject enterButton;
    private JsonParser parser;


    #region Camera Pos

    private readonly Vector2 mainPos = Vector2.zero;
    private readonly Vector2 loadPos = new Vector2(0, 5);
    private readonly Vector2 tutoPos = new Vector2(5, 0);

    #endregion

    private void Awake()
    {
        int width = Screen.width;
        int height = Screen.width * 10 / 16;
        Application.targetFrameRate = 60;
        Screen.SetResolution(width, height, true);
    }
    // Use this for initialization
    void Start()
    {

        GDB = GameDataBase.getDBinstance;

        CameraTrs.position = mainPos;

        TempStaticMemory.gameCount = PlayerPrefs.GetInt("GameCount", 0);

        InitializeAnimation();
        parser = new JsonParser();
        for (int i = 0; i < ButtonTrs.childCount; i++)
        {
            UIEventListener.Get(ButtonTrs.GetChild(i).gameObject).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
            UIEventListener.Get(ButtonTrs.GetChild(i).gameObject).onClick += new UIEventListener.VoidDelegate(ButtonProcess);
        }
        selectContext.text = "마왕을 선택해주세요";
        inputObject.gameObject.SetActive(false);
        enterButton = ButtonTrs.FindChild("EnterButton").gameObject;//.SetActive(false);
        enterButton.SetActive(false);

    }
    void InitializeAnimation()
    {
        KingAnimation.Reset();
        QueenAnimation.Reset();
        KingAnimation.enabled = false;
        QueenAnimation.enabled = false;

    }

    string player_spriteName;

    void ButtonProcess(GameObject go)
    {

        string btn_name = go.name;
        InitializeAnimation();
        switch (btn_name)
        {
            case "King":
                //  PlayerPrefs.SetString("PlayerSprite", "King");

                player_spriteName = "King";
                KingAnimation.enabled = true;
                contextLabel.text = parser.getContext("King") + parser.getContext("Select");
                selectContext.text = "마왕의 이름을 정해주세요";
                inputObject.gameObject.SetActive(true);
                enterButton.SetActive(true);
                break;
            case "Queen":

                player_spriteName = "Queen";
                QueenAnimation.enabled = true;
                contextLabel.text = parser.getContext("Queen") + parser.getContext("Select");
                selectContext.text = "마왕의 이름을 정해주세요";
                inputObject.gameObject.SetActive(true);
                enterButton.SetActive(true);
                break;
		case "EnterButton":

			GDB.CreatePlayerData (inputObject.value, player_spriteName);
			CameraTrs.position = tutoPos;
			tutorialPanel.SetActive (true);

			GDB.SaveFile ();
        break;
            case "TouchBtn":
                // /* File Data PART
                if (GDB.isNoneData)
                {
                    go.SetActive(false);
                    LoginPanel.SetActive(false);
                }
                else
                {
                    MoveLoadingScene();
                }

                break;
		case "SkipButton":
			MoveLoadingScene ();
			break;

            default:
                break;
        }
    }
    void MoveLoadingScene()
    {
        CameraTrs.position = loadPos;
        LoadingPanel.SetActive(true);

        //Current File
        TempStaticMemory.gold = GDB.getUserDS().receiveIntCmd("currentGold");

        StartCoroutine("MoveGameScene");
    }
    IEnumerator MoveGameScene()
    {
        loadingSlider.value = 0;
        GDB.getUserDS().sendIntCmd("GameStart", 0);

        AsyncOperation async = SceneManager.LoadSceneAsync("Game");

        while (!async.isDone)
        {
            loadingSlider.value = async.progress;
            yield return new WaitForFixedUpdate();
            yield return true;
        }

        yield return null;
    }


}
