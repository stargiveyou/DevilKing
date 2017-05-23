using UnityEngine;
using System.Collections;

public class TrophyPanelControl : MonoBehaviour {

	public Transform TrophyGridTrs;
	public TrophyDisplayPanelManager TrophyDisplay;
	public GameObject TrophyItemObject;

	private TrophyDataBase TrophyDB;
	private UIGrid TrophyGrid;
	GameManager GM ;


	void Awake()
	{
		GM = GameManager.getInstance ();

		TrophyDB = GameDataBase.getDBinstance.getTropyDB;
		TrophyGrid = TrophyGridTrs.GetComponent<UIGrid> ();

		InitializeTrophyBoard ();

	}

	void OnEnable()
	{
		UpdateTrophyBoard ();
	}
	// Use this for initialization
	void Start () {
		
	}

	void InitializeTrophyBoard()
	{
		int totalTrophyCount = TrophyDB.currentTropyListCount;
		UISprite thumnailSprite;
		UILabel contextLabel;
		for (int i = 0; i < totalTrophyCount; i++) {
			bool isCompletedTropy = TrophyDB.checkTropyData (i);

			GameObject CreateTrophyItem = Instantiate (TrophyItemObject) as GameObject;
			CreateTrophyItem.name = i.ToString ("0");
			thumnailSprite = CreateTrophyItem.transform.FindChild ("ThumNail").GetComponent<UISprite> ();
			contextLabel = CreateTrophyItem.transform.FindChild ("Context").GetComponent<UILabel> ();

			UIEventListener.Get (CreateTrophyItem).onClick -= new UIEventListener.VoidDelegate (DisplayTrophyProcess);
			UIEventListener.Get (CreateTrophyItem).onClick += new UIEventListener.VoidDelegate (DisplayTrophyProcess);

			thumnailSprite.spriteName = TrophyDB.getTrophySpriteName (i);
			contextLabel.text = GM.getContext ("Trophy", isCompletedTropy, i);
			CreateTrophyItem.transform.FindChild ("lockSprite").gameObject.SetActive (!isCompletedTropy);



			CreateTrophyItem.transform.parent = TrophyGridTrs;
			ResetTransform (CreateTrophyItem.transform);
			CreateTrophyItem.transform.SetAsLastSibling ();
		}

		TrophyGrid.Reposition ();
	}

	void UpdateTrophyBoard()
	{
		UISprite thumnailSprite;
		UILabel contextLabel;
		bool isCompletedTropy;
		int length = TrophyGridTrs.childCount;
		for (int i = 0; i < length; i++) {
			Transform TrophyItem = TrophyGridTrs.GetChild (i);
			contextLabel = TrophyItem.FindChild ("Context").GetComponent<UILabel> ();
			isCompletedTropy = TrophyDB.checkTropyData (i);
			TrophyItem.transform.FindChild ("lockSprite").gameObject.SetActive (!isCompletedTropy);
			contextLabel.text = GM.getContext ("Trophy", isCompletedTropy, i);
		}
		TrophyGridTrs.GetChild (0).GetComponent<UICenterOnClick> ().CenterOn ();
	}

	void DisplayTrophyProcess(GameObject obj)
	{
		int index = int.Parse (obj.name);
		TrophyDB.sendTropyDisplay (index, DisplayTrophyCallback);
		obj.GetComponent<UICenterOnClick> ().CenterOn ();
	}

	void DisplayTrophyCallback(int index, string spriteName)
	{
		TrophyDisplay.SetTropy(index,spriteName,GameManager.getInstance ().getContext ("Trophy", true, index));
	}

	void ResetTransform(Transform trs)
	{
		trs.localPosition = Vector3.zero;
		trs.localScale = new Vector3 (1, 1, 1);
	}


}
