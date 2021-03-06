﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TrophyDisplayPanelManager : MonoBehaviour {

    public UISprite trophysprite;
    public UILabel trophyContext;
    public GameObject Cancle;
	public GameObject PopUP;
	public UIAtlas[] tropysAtlas;

	struct tropyDisplayStruct
	{
		public string spriteName;
		public string context;
		public int index;

		public tropyDisplayStruct(int index, string spritename, string context)
		{
			this.index = index;
			this.spriteName = spritename;
			this.context = context;

		}
	};

	private Queue<tropyDisplayStruct> displayTropyQueue = new Queue<tropyDisplayStruct>();

    void Start()
    {
        UIEventListener.Get(Cancle).onClick -= new UIEventListener.VoidDelegate(ButtonProcess);
        UIEventListener.Get(Cancle).onClick += new UIEventListener.VoidDelegate(ButtonProcess);
    }
	/*
    public void SetTrophy(string spritename, string context)
    {
        trophysprite.spriteName = spritename;
        trophyContext.text = context;
		PopUP.SetActive (true);
		Time.timeScale = 0.0f;
    }
    */

	public void SetTropy(int index, string spritename, string context)
	{
		displayTropyQueue.Enqueue (new tropyDisplayStruct (index, spritename, context));
		if (displayTropyQueue.Count == 1) {
			UIAtlas selectedAtlas = tropysAtlas [(index - 1) / 16];

			PopUP.SetActive (true);
			trophysprite.atlas = selectedAtlas;
			//Trophy_Sprite_01
			//trophysprite.spriteName = spritename;
			trophysprite.spriteName = "Trophy_Sprite_" + index.ToString ("0#");
			trophyContext.text = context;

			Time.timeScale = 0.0f;
		}
	}


    void ButtonProcess(GameObject go)
    {
		//Time.timeScale = 1.0f;
		displayTropyQueue.Dequeue();
		if (displayTropyQueue.Count == 0) {
			PopUP.SetActive (false);
		} else {
			tropyDisplayStruct displayStruct =  displayTropyQueue.Dequeue();
			SetTropy (displayStruct.index, displayStruct.spriteName, displayStruct.context);
		}
    }
}
