using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
//using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class Shooting : NetworkBehaviour {

	[SerializeField] public float shootDelay = 0.2f;
	[SerializeField] public float soundDelay = 4f;
	[SerializeField] int upgradeAmount = 10;
	[SerializeField] [SyncVar] int damage = 10;
	[SerializeField] private AudioClip m_ShootingSound;
	private AudioSource m_AudioSource;


	public static int damageStatic;
	private static int upgradeAmountStatic;


	private Ray ray; // the ray that will be shot
	private RaycastHit hit; // variable to hold the object that is hit

	public float elapsedTime;
	public float soundElapsedTime;

	private Animator myAnimator;

	void Start()
	{
		damageStatic = damage;
		upgradeAmountStatic = upgradeAmount;

		myAnimator = GetComponent<Animator>();
		soundElapsedTime = soundDelay;
		elapsedTime = shootDelay;
		m_AudioSource = GetComponent<AudioSource>();
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
				//CmdSendDamage(damage, hit.transform.gameObject);
			}
		}
	}

	void Update()
	{
		elapsedTime += Time.deltaTime;
		soundElapsedTime = elapsedTime;

		if (Input.GetButton ("Fire1") && isLocalPlayer) {
			myAnimator.SetBool ("isShooting", true);
			if (!m_AudioSource.isPlaying){
				print ("Dei play!");
				if (m_AudioSource.clip != m_ShootingSound)
					m_AudioSource.clip = m_ShootingSound;
				m_AudioSource.Play ();
			}
			if (elapsedTime >= shootDelay && isLocalPlayer) {
				Shoot ();
				elapsedTime = 0f;
				soundElapsedTime = 0f;
			}
		} else if (isLocalPlayer) {
			myAnimator.SetBool ("isShooting", false);
			print ("Parei de tocar!");
			m_AudioSource.Stop ();
		}
		damage = damageStatic;
	}

	public static void ShootingUpgradeDamage ()
	{
		damageStatic += upgradeAmountStatic;
	}

}