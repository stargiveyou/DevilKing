using UnityEngine;
using System.Collections;

public class TrophyPanelControl : MonoBehaviour {

	public Transform TrophyGridTrs;
	public TrophyDisplayPanelManager TrophyDisplay;
	public GameObject TrophyItemObject;

	private TrophyDataBase TrophyDB;
	private UIGrid TrophyGrid;


	// Use this for initialization
	void Start () {
		TrophyDB = GameDataBase.getDBinstance.getTropyDB;
		TrophyGrid = TrophyGridTrs.GetComponent<UIGrid> ();

		InitializeTrophyBoard ();

	}

	void InitializeTrophyBoard()
	{
		GameManager GM = GameManager.getInstance ();
		int totalTrophyCount = TrophyDB.currentTropyListCount;
		UISprite thumnailSprite;
		UILabel contextLabel;
		for (int i = 0; i < totalTrophyCount; i++) {
			bool isCompletedTropy = TrophyDB.checkTropyData (i);

			GameObject CreateTrophyItem = Instantiate (TrophyItemObject) as GameObject;
			CreateTrophyItem.name = i.ToString ("0");
			thumnailSprite = CreateTrophyItem.transform.FindChild ("ThumNail").GetComponent<UISprite> ();
			contextLabel = CreateTrophyItem.transform.FindChild ("Context").GetComponent<UILabel> ();

			//Button.oncli
			UIEventListener.Get (CreateTrophyItem).onClick += new UIEventListener.VoidDelegate (DisplayTrophyProcess);

			thumnailSprite.spriteName = TrophyDB.getTrophySpriteName (i);
			contextLabel.text = GM.getContext ("Trophy", isCompletedTropy, i);

					CreateTrophyItem.transform.parent = TrophyGridTrs;
			ResetTransform (CreateTrophyItem.transform);
			CreateTrophyItem.transform.SetAsLastSibling ();
		}

		TrophyGrid.Reposition ();
	}


	void DisplayTrophyProcess(GameObject obj)
	{
		int index = int.Parse (obj.name);
		TrophyDB.sendTropyDisplay (index, DisplayTrophyCallback);
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
