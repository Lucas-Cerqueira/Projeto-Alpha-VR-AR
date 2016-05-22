using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Shoot_Laser : NetworkBehaviour {

    [SerializeField] int initialDamage = 10;
	[SerializeField] float shootDelay = 0.5f;
	private float actualTime = 0.0f;
	private int up = 10;
//	Combat[] combatComponentList;

	private GameObject enemySpawner;

	void Start()
	{
		enemySpawner = GameObject.Find ("EnemySpawner");
	}

	//[Command]
	void CmdSendDamage(int id, int damage)
	{
		Combat[] combatComponentList =  enemySpawner.transform.GetComponentsInChildren<Combat> ();
		foreach (Combat component in combatComponentList) 
		{
			if (id == component.id) 
			{
				component.CmdTakeDamage(damage);
				break;
			}
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.gameObject.tag == "Enemy" && Time.realtimeSinceStartup - actualTime > shootDelay)
		{
			actualTime = Time.realtimeSinceStartup;
			Vector3 destination = other.transform.position;
			Vector3 direction = (destination - transform.position).normalized;
			RaycastHit hit;
			if (Physics.Raycast (transform.position, direction, out hit))
			{
				if (hit.collider.gameObject.tag == "Enemy") 
				{
					Debug.DrawLine (transform.position, hit.point, Color.cyan);
                    int id = hit.transform.GetComponent<Combat>().id;
                    //int id = other.gameObject.GetComponent<Combat>().id;
					CmdSendDamage (id, initialDamage);
				}
			}
		}
	}

	public void LaserPromotion ()
	{
		if (up > 0)
			initialDamage += up;
	}

//	void Update ()
//	{
//		combatComponentList = enemySpawner.transform.GetComponentsInChildren<Combat> ();
//	}
		
}
