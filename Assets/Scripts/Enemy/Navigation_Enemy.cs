using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Navigation_Enemy : NetworkBehaviour {

	public float damageDelay = 0.5f;
	private float actualTime = 0.0f;
	public int damage = 10;

	private NavMeshAgent agent;
    private Transform destination;

    private Animator myAnimator;

	// Use this for initialization
	void OnEnable () 
    {
		agent = gameObject.GetComponent<NavMeshAgent>();
        destination = GameObject.Find("Tower").transform;
		agent.SetDestination(destination.position);

        myAnimator = GetComponent<Animator>();
        myAnimator.SetBool("isWalking", true);
        //myAnimator
	}
	
	// Update is called once per frame
	void Update () 
    {
	}

	void OnCollisionStay(Collision collision) 
    {
		if (collision.gameObject.tag == "Tower" && Time.realtimeSinceStartup - actualTime > damageDelay) 
		{
            agent.Stop();
			actualTime = Time.realtimeSinceStartup;
            myAnimator.SetBool("isWalking", false);
            myAnimator.SetBool("isAttacking", true);
            //collision.gameObject.GetComponent <Life_Bar> ().doDamage (damage);
            if (isServer)
                collision.gameObject.GetComponent<Combat>().CmdTakeDamage(damage);
			
		}
        //else
        //    myAnimator.SetBool("isAttacking", false);
	}
}
