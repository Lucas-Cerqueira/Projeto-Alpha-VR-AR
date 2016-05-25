using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
//using System.Collections.Generic;

public class Shooting : NetworkBehaviour {

    [SerializeField] int damage = 20;
    [SerializeField] float shootDelay = 0.2f;

    private Ray ray; // the ray that will be shot
    private RaycastHit hit; // variable to hold the object that is hit

    private float elapsedTime;
	private GameObject enemySpawner;

    private Animator myAnimator;

    void Start()
    {
        myAnimator = GetComponent<Animator>();
        elapsedTime = shootDelay;
		enemySpawner = GameObject.Find ("EnemySpawner");
    }


    void OnEnable ()
    {
        myAnimator.Rebind();
        myAnimator.SetBool("isDead", false);
    }

	[Command]
	void CmdSendDamage(int id, int damage)
	{
		Combat[] combatComponentList = enemySpawner.transform.GetComponentsInChildren<Combat> ();
		foreach (Combat component in combatComponentList) 
		{
			if (id == component.id) 
			{
				component.CmdTakeDamage(damage);
				break;
			}
		}
	}

    void Shoot()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                Debug.DrawLine(transform.position, hit.point, Color.green);
				int id = hit.transform.GetComponent<Combat> ().id;
                //hit.transform.GetComponent<Combat>().CmdTakeDamage(damage);
				CmdSendDamage(id, damage);
                //Debug.Log("TOMOU HIT, FPS PLAYER!");
            }
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (Input.GetMouseButton(0) && isLocalPlayer)
        {
            myAnimator.SetBool("isShooting", true);

            if (elapsedTime >= shootDelay && isLocalPlayer)
            {
                Shoot();
                elapsedTime = 0f;
            }
        }
        else if (isLocalPlayer)
            myAnimator.SetBool("isShooting", false);

    }
}
