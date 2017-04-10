using UnityEngine;
using System.Collections;

public class TestData : MonoBehaviour {

    GameDataBase GDB;
    void Awake()
    {
     
    }
	// Use this for initialization
	void Start () {
        GDB = GameDataBase.getDBinstance;
        
    }
	
}
