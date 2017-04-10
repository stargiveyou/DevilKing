using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

    private float atk;

    private bool isArcher =false;

    private float speed;
    private readonly Vector2 initShootPos = new Vector2(-60,130); 

    void Start()
    {
        string mosnterName = transform.parent.name;
        SpeedSet(mosnterName);
    }

    void SpeedSet(string monsterId)
    {
     
       switch (monsterId)
        {
            case "SkeletonMage":
                speed = 0.6f;
                break; 
            case "Imp":
                speed = 0.6f;
                break;
            case "RaccerImp":
            case "Demon":
                speed = 1.0f;
                break;
            case "HighDemon":
                speed = 1.2f;
                break;
        }
    }

    void FarAttacking(float range)
    {

    }

    void ShootSpell()
    {
        if (isArcher)
        {
            transform.localPosition = initShootPos;
            StartCoroutine("Shoot");
        }
    }

    IEnumerator Shoot()
    {

        transform.Translate(Vector3.left * Time.deltaTime * speed);
        yield return new WaitForFixedUpdate();
        if(this.gameObject.activeSelf)
        { 
            StartCoroutine("Shoot");
        }
        else
        {
            StopCoroutine("Shoot");
        }
    }
    
    public float damage
    {
        get
        {
            return atk;
        }
        set
        {
            atk = value;
        }
    }

    public bool Archery
    {
        set
        {
            isArcher = value;
        }
        get
        {
            return isArcher;
        }
    }
}
