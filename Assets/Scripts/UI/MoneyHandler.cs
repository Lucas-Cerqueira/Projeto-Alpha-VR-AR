using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class MoneyHandler : NetworkBehaviour {

	[SyncVar] private int money;
	private Text text;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();

		money = 0;
	}
	
	// Update is called once per frame
	void Update () {
		text.text = "Money: " + money;
	}
		
	public void sumMoney (int sum)
	{
		money += sum;
	}

	public int getMoney ()
	{
		return money;
	}
}
