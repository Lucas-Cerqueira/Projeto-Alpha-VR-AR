using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class GameOverTower : GameOver {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    
    public override void endGame()
	{
		print ("GAMEOVER");
	}
}
