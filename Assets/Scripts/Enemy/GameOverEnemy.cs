using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameOverEnemy : GameOver{


	GameObject money;
	// Use this for initialization
	void Start () {
		money = GameObject.Find ("Money");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void endGame ()
	{
		print ("MORRI");
		if (money) money.GetComponent<MoneyHandler>().sumMoney(100);
        //NetworkServer.Destroy(this.gameObject);
		Destroy (this.gameObject);
	}
}
