using UnityEngine;
using System.Collections;

public class UpgradeLaser : MonoBehaviour {

	private Component Tower;
	private GameObject money;

	// Use this for initialization
	void Start () {
		Tower = GameObject.Find ("Tower").transform.GetChild(0).GetComponent <Shoot_Laser>();
		money = GameObject.Find ("Money");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void upgrade ()
	{
		if (money.GetComponent<MoneyHandler>().getMoney() >= 200) 
		{
			money.GetComponent<MoneyHandler>().sumMoney (-200);
			Tower.BroadcastMessage ("LaserPromotion");
		}
	}
}
