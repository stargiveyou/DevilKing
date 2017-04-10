using UnityEngine;
using System.Collections;

public class CemetryControl : MonoBehaviour {


    private GameManager GM;
    private Transform Ghost;
	void Start () {
        GM = GameManager.getInstance();
	}
    

    void EndAnimFunction()
    {
        GM.PoolObject(this.gameObject);
    }


}
