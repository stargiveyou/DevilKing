using UnityEngine;

using System;

using System.Text;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;

using Random = UnityEngine.Random;


public class TrophyDataBase : FileDataInterface {

	[Serializable]
		struct tropy_data_struct
	{
		int tropyIndex;
		string spriteName;

		string condition;
		int amount;
		bool isCompleted;

		float progressValue;

		public tropy_data_struct(string condition, int amount)
		{
			tropyIndex =0;
			spriteName = string.Empty;
			this.condition = condition;
			this.amount = amount;
			isCompleted = false;
			progressValue = 0;
		}

		public tropy_data_struct(int index, string spriteName, string condition, int amount)
		{
			tropyIndex = index;
			this.spriteName = spriteName;

			this.condition = condition;
			this.amount = amount;

			isCompleted = false;
			progressValue = 0.0f;

		}


		public void SendProgress(int amount)
		{
			if (this.amount >= amount) {
				isCompleted = true;
				progressValue = 1;
			} else {
				progressValue = (float)amount / this.amount;
			}
		}

		public bool Completed
		{
			get {
				return isCompleted;
			}
		}
		public float Progress
		{
			get
			{
				return progressValue;
			}
		}
		public int TropyIndex
		{
			get {
				return tropyIndex;
			}
		}
		public string TropySpriteName
		{
			get
			{
				return spriteName;
			}
		}

		public bool Equals (string condition, int amount)
		{
			return this.condition.Equals (condition) && (this.amount == amount);
		}

	};

	private string thisBinData;
	private ClassEnumType thisClassEnum;

	MemoryStream Mem;
	BinaryFormatter bin;

	private List<tropy_data_struct> tropy_List;
	public delegate void callbackDelegate(int index, string spriteName);
	public event callbackDelegate callbackEvent;
	public TrophyDataBase()
	{
		thisClassEnum = ClassEnumType.TropyData;
	}

	public bool Initialize (string binData = null)
	{
		bin = new BinaryFormatter();

		if (string.IsNullOrEmpty(binData))
		{
			tropy_List = new List<tropy_data_struct> ();
			Debug.Log("Init Tropy Data");
			return false;
		}
		else
		{
			Debug.Log("Loaded Tropy Data");
			Mem = new MemoryStream (Convert.FromBase64String (binData));
			tropy_List = (List<tropy_data_struct>)bin.Deserialize (Mem);
		}
		return true;
	}

	public string getBinData ()
	{
		return thisBinData;

	}

	public void SaveData ()
	{
		try { 
			Mem = new MemoryStream();
			bin.Serialize(Mem, tropy_List);
			thisBinData = Convert.ToBase64String(Mem.GetBuffer());
		}
		catch(Exception e)
		{
			Debug.Log(e.StackTrace);
		}
		finally{
			#if UNITY_EDITOR
			Debug.Log("Save Data Binary File");
			#endif
		}
	}

	public void CreateTropyData(int index, string spriteName, string newCmd, int newAmount)
	{
		if (tropy_List == null)
			tropy_List = new List<tropy_data_struct> ();
		
		tropy_data_struct newData = new tropy_data_struct(index,spriteName,newCmd,newAmount);
		tropy_List.Add(newData);


	}

	public void sendTropyCommand(string command, int amount, callbackDelegate callback)
	{
		try
		{
		tropy_data_struct loadData = tropy_List [getListIndex (command, amount)];
			loadData.SendProgress(amount);
			if(loadData.Completed)
			{
				callback(loadData.TropyIndex,loadData.TropySpriteName);
			}
			tropy_List[getListIndex(command,amount)] = loadData;
		}
		catch (ArgumentNullException ane) {
			Debug.Log (ane.StackTrace);
		}
	}

	public bool checkTropyData(string cmd, int amount)
	{
		int listIndex;

		if (tropy_List.Count == 0) {
			return false;
		}

		if ((listIndex =getListIndex (cmd, amount)) == -1) {
			return false;
		}

		return tropy_List [listIndex].Completed;
	}
	public  bool checkTropyData(int listIndex)
	{
		if (tropy_List.Count == 0) {
			return false;
		}

		return tropy_List [listIndex].Completed;
	}

	public int getITropyContextIndex()
	{
		throw new Exception ();
	}
	public string getTrophySpriteName()
	{
		throw new Exception();
	}


	private int getListIndex(string cmd, int amount)
	{
		int returnValue = -1;

		for (int i = 0; i < tropy_List.Count; i++) {

			if(tropy_List[i].Equals(cmd,amount))
			{
				returnValue = i;
				break;
			}
		}
		return returnValue;

	}


    public int currentTropyListCount
    {
        get
        {
            return tropy_List.Count;
        }
    }


    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        int index = 0;
        foreach(tropy_data_struct data in this.tropy_List) {

			builder.AppendFormat("[ List Index ]: {0} ,[ Index ] : {1}  , SpriteName: {2} \n"
				, index++, data.TropyIndex, data.TropySpriteName);

        }

        return builder.ToString();

    }


}
