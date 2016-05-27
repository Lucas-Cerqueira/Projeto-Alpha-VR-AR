using UnityEngine;
using System.Collections; 
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpgradeLaser : NetworkBehaviour {

	private Component Tower;
	private MoneyHandler moneyHandler;
	[SerializeField] int upgradePrice = 200;

	// Use this for initialization
	void Start () {
		if (isLocalPlayer)
		{
			moneyHandler = GameObject.Find("Money").GetComponent<MoneyHandler>();
			Tower = GameObject.Find ("Laser").GetComponent <Shoot_Laser>();
			Button button = (Button)GameObject.Find ("Upgrade Laser").GetComponent<Button> ();
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener (() => RequestUpgrade ());
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	[Command]
	public void CmdUpgrade ()
	{
		if (moneyHandler.GetComponent<MoneyHandler>().GetMoney() >= upgradePrice) 
		{
			moneyHandler.GetComponent<MoneyHandler>().SpendMoney (upgradePrice);
			Tower.BroadcastMessage ("LaserPromotion");
		}
	}

	[Client]
	public void RequestUpgrade ()
	{
		CmdUpgrade ();
	}
}
