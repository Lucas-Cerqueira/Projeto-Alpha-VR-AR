using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;


public class UpgradeShooterDamage : NetworkBehaviour {


	private MoneyHandler moneyHandler;

	[SerializeField] int upgradePrice = 0;
	// Use this for initialization
	void Start () {
		if (isLocalPlayer)
		{
			moneyHandler = GameObject.Find("Money").GetComponent<MoneyHandler>();
			Button button = (Button)GameObject.Find ("Upgrade Shooter").GetComponent<Button> ();
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener (() => RequestUpgradeDamage ());
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[Command]
	public void CmdUpgradeDamage ()
	{
		Shooting.ShootingUpgradeDamage ();
	}
		
	public void RequestUpgradeDamage ()
	{
		if (moneyHandler.GetComponent<MoneyHandler>().GetMoney() >= upgradePrice) 
		{
			moneyHandler.GetComponent<MoneyHandler>().SpendMoney (upgradePrice);
			CmdUpgradeDamage ();
		}
	}

}
