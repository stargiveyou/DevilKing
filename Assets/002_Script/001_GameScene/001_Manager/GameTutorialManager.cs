using UnityEngine;
using System.Collections;
using System.Text;
public class GameTutorialManager : MonoBehaviour {

    enum TutoType {None, Next, Alias, Trap,Castle};


    public GameObject Tuto_Button_Object, Tuto_tap_label;
    public ButtonController Button;
    public AttackerController Attack_Control;
    public StageManager stageManager;
    public Transform ArrowTrs;


    private readonly Vector3 main_display_collider_center = new Vector3(-92.1f, 4.5f, -4.23f);
    private readonly Vector3 main_display_collider_size = new Vector3(1279, 570, 0);


    private readonly Vector3 all_display_collider_center = new Vector3(-92, -99, -4.2f);
    private readonly Vector3 all_display_collider_size = new Vector3(1279, 777, 0);
    
    private GameManager GM;
    // Use this for initialization
    void Start () {
        GM = GameManager.getInstance();
        bubble_label = Tuto_Button_Object.transform.FindChild("bubble_label").GetComponent<UILabel>();
        Tuto_Button_Object.SetActive(true);

        tuto_box_col = Tuto_Button_Object.GetComponent<BoxCollider>();
        tuto_box_col.center = all_display_collider_center;
        tuto_box_col.size = all_display_collider_size;

        Button.TutorialBtnControl();
        PlayerPrefs.SetInt("trapLevel", 000);
        interface_tuto_process();


        UIEventListener.Get(Tuto_Button_Object).onClick -= new UIEventListener.VoidDelegate(interface_tuto_process);
        UIEventListener.Get(Tuto_Button_Object).onClick += new UIEventListener.VoidDelegate(interface_tuto_process);


    }
    #region Interface Tutorial
    private int tuto_count = 0;
    private UILabel bubble_label;
    private bool isTutoProcessing = true;
    private BoxCollider tuto_box_col;
    private void interface_tuto_process(GameObject obj = null)
    {
        TutoType _tuto_Enum = TutoType.None;
        
        if (isTutoProcessing)
        {
            isTutoProcessing = false;
            tuto_count++;
            string interface_msg = "";

            switch (tuto_count)
            {
                case 1:
                    
                    interface_msg = "용사란 놈들이 남의 집에 와서 마구 행패라니…_아무튼 이 돈은 마왕성 재건의 초석으로 사용해주마 후하하하!_ 먼저 몬스터의 소환이다";
                    break;
                case 2:
                    interface_msg = "몬스터의 소환은 소환하고 싶은 블록이나 그 위의 빈 공간을 터치하여 가능하지!";
                    _tuto_Enum = TutoType.Alias;
                    break;
                case 3:
                    interface_msg = "지금은 스켈레톤을 소환하는 것이 적기겠군. 자 와라 죽음의 군단이여";
                    _tuto_Enum = TutoType.Next;
                    break;
                case 4:
                    PlayerPrefs.SetInt("trapLevel", 100);
                    GM.SaveMonsterLevelData("Skeleton", 0);
                    interface_msg = "이번에는 함정이다!_있는걸 알면서도 시스템상 어쩔 수 없이 걸리게 되는 함정을 설치해주마!_자 설치할 곳을 골라볼까.";
                    _tuto_Enum = TutoType.Trap;
                    break;
                case 5:
                    GM.SaveMonsterLevelData("Skeleton", 1);
                    interface_msg = "스파이크는 고전적이지만 효과적이지 크큭… 용사녀석들!_알면서도 밟고 넘어올 수 밖에 없는 잔혹한 함정을 맛보아라!";
                    _tuto_Enum = TutoType.Next;
                    break;
                case 6:
                    Button.TutorialBtnControl(1);
                    interface_msg = "이제 남은 골드는 이 황량한 스테이지를 새로 단장하는데 사용해야겠어._마왕성 강화다!";
                    _tuto_Enum = TutoType.Castle;
                    break;
                case 7:
                    interface_msg = "좋아 이제 좀 지낼 만 하게 되었어!_후후훗 어서와라 용사 놈들… ";
                    break;
                case 8:
                    Button.TutorialBtnControl(2);
                    interface_msg = "남의 평온한 일상을 위협하는 녀석들은\n마왕성 부활 프로젝트의 제물로 써주마!";
                    break;
                case 9:
                    Debug.Log("????");
                    TempStaticMemory.gameCount++;
                    PlayerPrefs.SetInt("GameCount", TempStaticMemory.gameCount);
                    Tuto_Button_Object.SetActive(false);
                    GM.SendMessage("GameStartAllObject",SendMessageOptions.DontRequireReceiver);
                    
                    break;
                default:
                    break;
            }
            StopCoroutine("StartTypeingAnimation");
            if (!string.IsNullOrEmpty(interface_msg))
            { 
            StartCoroutine(StartTypeingAnimation(interface_msg, _tuto_Enum));
            }

        }
    }

    private IEnumerator TutorialWaiting()
    {
        yield return new WaitForFixedUpdate();

        if (!TempStaticMemory.isTutoProcessOn)
        {
            //Continue Coroutine
            StartCoroutine("TutorialWaiting");
        }
        else
        {
            isTutoProcessing = true;
            //Next

            tuto_box_col.center = all_display_collider_center;
            tuto_box_col.size = all_display_collider_size;

            ArrowTrs.gameObject.SetActive(false);
            
            TempStaticMemory.isTutoProcessOn = false;
            tuto_box_col.enabled = true;
            interface_tuto_process();


        }
    }

    private IEnumerator StartTypeingAnimation(string msg, TutoType typeEnum)
    {
        int str_length = 0;
        Tuto_tap_label.SetActive(false);
        bubble_label.text = "";
        string[] splited_string = msg.Split('_');
        string display_msg;
        StringBuilder str_bulder = new StringBuilder();
        int phase_length = splited_string.Length;

        for (int pl = 0; pl < phase_length; pl++)
        {
            display_msg = splited_string[pl];
            if (str_bulder.Length > 0)
            {
                str_bulder.Remove(0, str_bulder.Length);
            }

            char[] msgCharArray = display_msg.ToCharArray();
            str_length = display_msg.Length;
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < str_length; i++)
            {
                str_bulder.Append(msgCharArray[i]);
                yield return new WaitForFixedUpdate();
                bubble_label.text = str_bulder.ToString();
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(0.3f);
        }


        switch (typeEnum)
        {
            case TutoType.Castle:

                tuto_box_col.center = main_display_collider_center;
                tuto_box_col.size = main_display_collider_size;

                ArrowTrs.gameObject.SetActive(true);
                ArrowTrs.localPosition = new Vector2(0, -205);
                
                TempStaticMemory.isTutoProcessOn = false;
                StartCoroutine("TutorialWaiting");

                break;
            case TutoType.Alias:
                tuto_box_col.enabled = false;

                stageManager.SendMessage("tutorialArrowAppear");

                TempStaticMemory.isTutoProcessOn = false;
                StartCoroutine("TutorialWaiting");
                
                ArrowTrs.localPosition = new Vector2(-595, -194);
                break;
            case TutoType.Trap:
                tuto_box_col.enabled = false;

                stageManager.SendMessage("tutorialArrowAppear");

                TempStaticMemory.isTutoProcessOn = false;
                StartCoroutine("TutorialWaiting");

                
                ArrowTrs.localPosition = new Vector2(409, -194);
                break;

            case TutoType.Next:

                ArrowTrs.gameObject.SetActive(true);

                tuto_box_col.center = main_display_collider_center;
                tuto_box_col.size = main_display_collider_size;
                
                TempStaticMemory.isTutoProcessOn = false;
                StartCoroutine("TutorialWaiting");
                break;
            case TutoType.None:
                
                Tuto_tap_label.SetActive(true);
                isTutoProcessing = true;
                break;

        }
    }
    
    void readyNextTutorial()
    {
        Tuto_Button_Object.SetActive(true);

    }

    #endregion
}
