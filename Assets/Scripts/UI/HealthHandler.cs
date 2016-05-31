using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HealthHandler : NetworkBehaviour {


	Text healthText;
	private Combat myShooter;


	// Use this for initialization
	void Start () {
		if (isLocalPlayer) 
		{
			healthText = GameObject.Find ("LifePercentage").GetComponent<Text> ();
			myShooter = GetComponent<Combat> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) healthText.text = myShooter.health + "%";
	}
}
