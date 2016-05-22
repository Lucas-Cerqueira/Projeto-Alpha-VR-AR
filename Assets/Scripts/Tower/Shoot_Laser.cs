﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Shoot_Laser : NetworkBehaviour {

    [SerializeField] int initialDamage = 10;
	[SerializeField] float shootDelay = 0.5f;
    [SerializeField] float hitChance = 90;
	private float actualTime = 0.0f;
	private int up = 10;
//	Combat[] combatComponentList;

	private GameObject enemySpawner;

    private List<Combat> enemiesInRange;
    private int correctHits = 0;

	void Start()
	{
        enemiesInRange = new List<Combat>();

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

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            print("Inimigo adicionado à lista de alvos!");
            enemiesInRange.Add(other.transform.GetComponentInParent<Combat>());
        }
    }

    void OnTriggerExit (Collider other)
    {
        print("Saiu da colisão");

        if (other.gameObject.tag == "Enemy")
            enemiesInRange.Remove(other.transform.GetComponentInParent<Combat>());
    }



    void ShootNextEnemy()
    {
        //If the tower hit an enemy...
        if (Random.Range(1, 101) <= hitChance)
        {
            CmdSendDamage(enemiesInRange[0].id, initialDamage);
            correctHits++;
        }
        if (correctHits == Mathf.Ceil(enemiesInRange[0].maxHealth / (float)initialDamage))
        {
            enemiesInRange.RemoveAt(0);
            correctHits = 0;
        }
    }

    //void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.tag == "Enemy" && Time.realtimeSinceStartup - actualTime > shootDelay)
    //    {
    //        actualTime = Time.realtimeSinceStartup;
    //        Vector3 destination = other.transform.position;
    //        Vector3 direction = (destination - transform.position).normalized;
    //        RaycastHit hit;
    //        if (Physics.Raycast(transform.position, direction, out hit))
    //        {
    //            if (hit.collider.gameObject.tag == "Enemy")
    //            {
    //                Debug.DrawLine(transform.position, hit.point, Color.cyan);
    //                int id = hit.transform.GetComponent<Combat>().id;
    //                //int id = other.gameObject.GetComponent<Combat>().id;
    //                CmdSendDamage(id, initialDamage);
    //            }
    //        }
    //    }
    //}

	public void LaserPromotion ()
	{
		if (up > 0)
			initialDamage += up;
	}

    void Update()
    {
        if (Time.realtimeSinceStartup - actualTime > shootDelay && enemiesInRange.Count!=0)
        {
            actualTime = Time.realtimeSinceStartup;
            ShootNextEnemy();
        }
    }
		
}
