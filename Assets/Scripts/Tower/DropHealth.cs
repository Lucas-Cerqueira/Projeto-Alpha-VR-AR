using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class DropHealth : NetworkBehaviour {

	private int pHealth = 10;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter(Collision collision) 
	{
		if (collision.gameObject.tag == "Shooter") 
		{
			if (isServer)
				collision.gameObject.GetComponent<Combat>().CmdTakeDamage(-pHealth);
			GameObject.Destroy (this.gameObject);
		}
		//else
		//    myAnimator.SetBool("isAttacking", false);
	}
}