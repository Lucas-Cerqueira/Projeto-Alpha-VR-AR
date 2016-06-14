using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HealthHandler : NetworkBehaviour {

	[SerializeField] int numOfBars = 10;
	GameObject healthText;
	private Combat myShooter;
	private int prevHealth = 0;
	private int prevChanged = 10;

	// Use this for initialization
	void Start () 
    {
		if (isLocalPlayer) 
		{
			healthText = GameObject.Find ("LifePercentage");
			myShooter = GetComponent<Combat> ();
			prevHealth = myShooter.health;
		}
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (isLocalPlayer)
        {
			if (healthText) 
			{
				if (myShooter.health != prevHealth) 
				{
					prevHealth = myShooter.health;
					OnHealthChange ();
				}
			}
            else
                if (healthText = GameObject.Find("LifePercentage")) { }
        }
	}

	public void OnHealthChange ()
	{
		int activeHealth = prevHealth / numOfBars;
		for (int i = (activeHealth > prevChanged ? prevChanged : activeHealth); i< (activeHealth > prevChanged ?  activeHealth : prevChanged); i++)
			healthText.transform.GetChild (i).gameObject.SetActive (!healthText.transform.GetChild (i).gameObject.activeSelf);
		prevChanged = activeHealth;
	}
}
