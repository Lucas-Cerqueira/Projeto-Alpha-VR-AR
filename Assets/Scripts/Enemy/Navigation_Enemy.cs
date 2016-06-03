using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Navigation_Enemy : NetworkBehaviour {

    [SerializeField] private bool lookForTarget;
    [SerializeField] private int attackDamage = 50;
    [SerializeField] private float timeBetweenAttacks = 0.5f;
    [SerializeField] private float maxDistanceToTargetPlayer = 15f;
    [SerializeField] private float meleeAttackRange = 0.5f;

    private Vector3 towerTargetPosition;
    private bool targetFound = false;
    private Transform target;
    private Combat combatEnemy;
    private Combat combatTarget;
    private NavMeshAgent agent;
    private Animator myAnimator;
	private float actualTime = 0.0f;
    private float elapsedTimeAttacking;
	[SyncVar] Vector3 realPosition;
	[SyncVar] Quaternion realRotation;
	private Vector3 prevPosition;
	private Quaternion prevRotation;

    void LookForTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxDistanceToTargetPlayer);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].CompareTag("Shooter") || hitColliders[i].CompareTag("Turret") || hitColliders[i].CompareTag("Tower"))
            {
                targetFound = true;
                target = hitColliders[i].transform;
                combatTarget = hitColliders[i].GetComponent<Combat>();
            }
        }
    }

    void AttackTarget()
    {
        if (target.CompareTag("Shooter"))
        {
            if (Vector3.Distance(transform.position, target.position) <= meleeAttackRange)
            {
                elapsedTimeAttacking += Time.deltaTime;
                myAnimator.SetBool("isAttacking", true);
                if (elapsedTimeAttacking >= timeBetweenAttacks)
                {
                    //actualTime = Time.realtimeSinceStartup;
                    elapsedTimeAttacking = 0f;
                    if (isServer)
                        combatTarget.CmdTakeDamage(attackDamage);
                }
            }
            else
            {
                elapsedTimeAttacking = 0f;
                myAnimator.SetBool("isAttacking", false);
            }
        }
        else if (target.CompareTag("Turret") || target.CompareTag("Tower"))
        {
            float distanceToTarget;
            if (target.CompareTag("Tower"))
                distanceToTarget = Vector3.Distance(transform.position, towerTargetPosition);
            else
                distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= meleeAttackRange)
            {
               // if (agent.velocity.magnitude <= 0.5f)
                //{
                    elapsedTimeAttacking += Time.deltaTime;
                    agent.Stop();
                    myAnimator.SetBool("isWalking", false);
                    myAnimator.SetBool("isAttacking", true);
                    if (elapsedTimeAttacking >= timeBetweenAttacks)
                    {
                        //actualTime = Time.realtimeSinceStartup;
                        elapsedTimeAttacking = 0f;
                        if (isServer)
                            combatTarget.CmdTakeDamage(attackDamage);
                    }
                //}
            }
            else
            {
                elapsedTimeAttacking = 0f;
                myAnimator.SetBool("isAttacking", false);
            }
        }
    }

	// Use this for initialization
	void OnEnable () 
    {
        elapsedTimeAttacking = 0f;

		agent = gameObject.GetComponent<NavMeshAgent>();
        target = GameObject.Find("Tower").transform;
        towerTargetPosition = target.position + new Vector3(0, 0, -20);
		agent.SetDestination(towerTargetPosition);

        combatEnemy = GetComponent<Combat>();
        targetFound = false;

        myAnimator = GetComponent<Animator>();
        myAnimator.SetBool("isWalking", true);
		prevRotation = transform.rotation;
		prevPosition = transform.position;
        //myAnimator
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (lookForTarget)
        {
            if (!combatEnemy.isDead && !targetFound)
                LookForTarget();

            if (targetFound)
            {
                if (combatTarget.isDead)
                {
                    agent.ResetPath();
                    agent.SetDestination(towerTargetPosition);
                    myAnimator.SetBool("isAttacking", false);
                    myAnimator.SetBool("isWalking", true);
                    targetFound = false;
                    elapsedTimeAttacking = 0f;
                }
                else
                {
                    agent.SetDestination(target.position);
                    AttackTarget();
                }
            }
        }
		if (isServer) {
			if (transform.position != prevPosition) {
				prevPosition = transform.position;
				realPosition = transform.position;
			}
			if (transform.rotation != prevRotation) {
				prevRotation = transform.rotation;
				realRotation = transform.rotation;
			}
		}
		if (isLocalPlayer) {
			transform.position = realPosition;
			transform.rotation = realRotation;
		}
	}

    //void OnCollisionStay(Collision collision) 
    //{
    //    if (collision.gameObject.tag == "Tower" && Time.realtimeSinceStartup - actualTime > timeBetweenAttacks) 
    //    {
    //        agent.Stop();
    //        actualTime = Time.realtimeSinceStartup;
    //        myAnimator.SetBool("isWalking", false);
    //        myAnimator.SetBool("isAttacking", true);
    //        //collision.gameObject.GetComponent <Life_Bar> ().doDamage (damage);
    //        if (isServer)
    //            collision.gameObject.GetComponent<Combat>().CmdTakeDamage(attackDamage);
			
    //    }
    //    //else
    //    //    myAnimator.SetBool("isAttacking", false);
    //}
}
