using UnityEngine;
using System.Collections;

public class AttackerHealGS : MonoBehaviour {

	// Use this for initialization
	void Start () {
        healAmount = 0.0f;

    }
    private float healAmount;

    public void SetHealAmount(float healAmount)
    {
        this.healAmount = healAmount;

    }

    public float GetHealAmount
    {
        get
        {
            return healAmount;
        }
    }
}
