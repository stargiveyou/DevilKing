using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Text;

public class TutorialProcessManager : MonoBehaviour
{

    private Animator thisAnimator;
    public UILabel convers_Label;
    public UISprite own_bubble_Sprite;
    public UISpriteAnimation player_animation;
    public UISprite kira_Player_Sprite;
    
	public GameObject nextButton;

    public UILabel tuto_goldLabel;
    private Transform own_bubble_Trs;

    int tuto_process = 0;


	void OnEnable()
	{

		//string player_sprite_type = PlayerPrefs.GetString("PlayerSprite", "King");
		string player_sprite_type = GameDataBase.getDBinstance.getUserDB.PlayerSpriteName;

		if (player_sprite_type.Equals("King"))
		{
			kira_Player_Sprite.transform.localPosition = Vector2.zero;
		}
		else if (player_sprite_type.Equals("Queen"))
		{
			kira_Player_Sprite.transform.localPosition = new Vector2(34, -5f);
		}

		player_animation.namePrefix = "attack_" + player_sprite_type + "_0";
		player_animation.Reset();
		player_animation.transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);
		player_animation.enabled = false;

		kira_Player_Sprite.spriteName = "emotion_" + player_sprite_type + "_kira";
		kira_Player_Sprite.MakePixelPerfect();
	}


    // Use this for initialization
    void Start()
    {
		tuto_goldLabel.text = "00";

        own_bubble_Trs = own_bubble_Sprite.transform;

        thisAnimator = GetComponent<Animator>();

        UIEventListener.Get(nextButton).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
        UIEventListener.Get(nextButton).onClick += new UIEventListener.VoidDelegate(ButtonProcess);

    }

    void ButtonProcess(GameObject go)
    {
        if (go.name.Equals("nextBtn"))
        {
            thisAnimator.SetTrigger("Next");
            TutoProcess();
        }
    }

    public void TutoProcess()
    {
		
        StopCoroutine("StartTypeingAnima");
        StopCoroutine("StartTutoAnimation");
        StartCoroutine(StartTutoAnimation(++tuto_process));

    }

    private IEnumerator StartTutoAnimation(int tuto)
    {
		nextButton.SetActive (false);
        string conversation = "";

        //conversation = "튜토리얼 테스트를 해보자/////" + tuto.ToString("#");

        switch (tuto)
        {
            case 1:
                conversation = "100년 전에 죽었던 마왕의 부활!";
                own_bubble_Sprite.spriteName = "emotion_anoy";
                break;
            case 2:
                conversation = "이놈의 용사들은 부활하기만 하면 쳐들어와서 남의 집에서 깽판을 놓는데, 어떡해야 하죠? ";
                break;
            case 3:
                conversation = "설상가상으로 마왕성은 거의 폐허 상태가 되고";
                break;
            case 4:
                conversation = "산더미 같았던 금은보화는 이미 흔적도 없이 털린 상태….";
                break;
            case 5:
                conversation = "용사들을 혼내주고 싶어도 과거의 수많던 군세들은 흔적조차 없다.";

                break;
            case 6:
                own_bubble_Sprite.spriteName = "emotion_depress";
                conversation = "아아 마왕님의 평온한 일상은 어디로 간 것 일까. ";

                break;
            case 7:

                conversation = "안 그래도 빡쳐있는 마왕님의 눈에 들어온 것은 이미 폐허가 된 마왕성에 놀러온 모험가 한 무리!";
                own_bubble_Sprite.spriteName = "emotion_exclam";

                break;
            case 8:
                own_bubble_Sprite.spriteName = "emotion_angry";
                conversation = "울컥한 마왕님의 공격에 모험가들은 재가 되어버리고… ";
                
                break;
            case 9:
                player_animation.Reset();
                player_animation.transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);
                conversation = "응? 하급 모험가들한테서 골드가 떨어졌다";
                break;
            case 10:
                conversation = "이거다!";
                own_bubble_Sprite.spriteName = "emotion_exclam";
                break;
            case 11:
                thisAnimator.Stop();
                conversation = "마왕성에 찾아오는 녀석들을 차례로 먼지로 만들면서 돈을 모아 마왕성을 예전의 찬란했던 영광의 시대로 바꿔보자!!";
                break;
            case 12:
                conversation = "마왕님의 본격 마왕성 재건 프로젝트 시작합니다!";
                break;
            case 13:
                GameObject.Find("ScriptManager").GetComponent<TutorialManager>().SendMessage("MoveLoadingScene", SendMessageOptions.DontRequireReceiver);
                //SceneManager.LoadScene("Game");
                break;
        }


        yield return StartCoroutine("StartTypeingAnima", conversation);

		nextButton.SetActive (true);


    }

    private IEnumerator StartTypeingAnima(string msg)
    {

        int str_length = msg.Length;
        convers_Label.text = "";


        StringBuilder str_bulder = new StringBuilder();
        char[] msgCharArray = msg.ToCharArray();

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < str_length; i++)
        {
            str_bulder.Append(msgCharArray[i]);
            yield return new WaitForFixedUpdate();
            convers_Label.text = str_bulder.ToString();
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    public void AddGoldValue()
    {
        tuto_goldLabel.text = "300";
    }



}

