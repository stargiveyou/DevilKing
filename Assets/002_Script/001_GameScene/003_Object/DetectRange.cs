using UnityEngine;
using System.Collections;

public class DetectRange : MonoBehaviour {


    private PlayerCharacter parentCtrl;

	// Use this for initialization
	void Start () {
        parentCtrl = transform.parent.GetComponent<PlayerCharacter>();
        parentCtrl.setArcher();

    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag.Equals("Enemy") || hit.gameObject.tag.Equals("SuperEnemy"))
        {
            parentCtrl.StartCoroutine("AttackProcess");
        }
    }
}
