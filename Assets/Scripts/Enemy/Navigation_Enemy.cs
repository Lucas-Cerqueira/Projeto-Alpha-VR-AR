using UnityEngine;
using System.Collections;

public class Navigation_Enemy : MonoBehaviour {

	public float damageDelay = 0.5f;
	private float actualTime = 0.0f;
	public int damage = 10;

	private NavMeshAgent agent;
    private Transform destination;

    private Animator myAnimator;

	// Use this for initialization
	void Start () 
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
			actualTime = Time.realtimeSinceStartup;

            //collision.gameObject.GetComponent <Life_Bar> ().doDamage (damage);
            collision.gameObject.GetComponent<Combat>().CmdTakeDamage(damage);
			agent.Stop ();
		}
	}
}
