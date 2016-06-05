using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class MoneyHandler : NetworkBehaviour {

	[SyncVar (hook = "OnMoneyChanged")] private int money;
	private Text text;

	// Use this for initialization
	void Start () 
    {
		text = GameObject.Find("Money").GetComponent<Text> ();
		money = 0;

	}
	
	// Update is called once per frame
	void Update () 
    {
		text.text = "Money: " + money;
	}
		
	public void AddMoney (int amount)
	{
		money += amount;
	}

    public void SpendMoney(int amount)
    {
        money -= amount;
    }

	public int GetMoney ()
	{
		return money;
	}

	public void OnMoneyChanged (int newMoney)
	{
		money = newMoney;
	}
}
