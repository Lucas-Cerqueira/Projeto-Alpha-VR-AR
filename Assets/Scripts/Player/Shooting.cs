using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
//using System.Collections.Generic;

public class Shooting : NetworkBehaviour {

	[SerializeField] public float shootDelay = 0.2f;
	[SerializeField] int upgradeAmount = 10;
	[SerializeField] [SyncVar] int damage = 10;

    public static int damageStatic;
    private static int upgradeAmountStatic;
    

    private Ray ray; // the ray that will be shot
    private RaycastHit hit; // variable to hold the object that is hit

    public float elapsedTime;

    private Animator myAnimator;

    void Start()
    {
        damageStatic = damage;
        upgradeAmountStatic = upgradeAmount;

        myAnimator = GetComponent<Animator>();
        elapsedTime = shootDelay;
    }


    //void OnEnable ()
    //{
    //    myAnimator = GetComponent<Animator>();
    //    myAnimator.Rebind();
    //    myAnimator.SetBool("isDead", false);
    //}
     
    [Command]
    void CmdSendDamage(int damage, GameObject go)
    {
        Combat combat = go.GetComponent<Combat>();
        if (combat)
            combat.CmdTakeDamage(damage);

        //Combat[] combatComponentList = enemySpawner.transform.GetComponentsInChildren<Combat>();
        //foreach (Combat component in combatComponentList)
        //{
        //    if (id == component.id)
        //    {
        //        component.CmdTakeDamage(damage);
        //        break;
        //    }
        //}
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
				//int id = hit.transform.GetComponent<Combat> ().id;
				CmdSendDamage(damage, hit.transform.parent.gameObject); // parent because the collider is in a child
            }
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

		if (Input.GetButton("Fire1") && isLocalPlayer)
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

		damage = damageStatic;
    }
		
	public static void ShootingUpgradeDamage ()
	{
		damageStatic += upgradeAmountStatic;
	}
				
}
