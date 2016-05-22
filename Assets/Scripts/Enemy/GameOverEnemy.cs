using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameOverEnemy : GameOver{

    private Animator myAnimator;
	private GameObject money;

	// Use this for initialization
	void Start () {
		money = GameObject.Find ("Money");
        myAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void endGame ()
	{
		//print ("MORRI");
        myAnimator.SetBool("isAttacking", false);

        GetComponent<HealthBar>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
		if (money) money.GetComponent<MoneyHandler>().sumMoney(100);
        //NetworkServer.Destroy(this.gameObject);
        int i = Random.Range(1, 3);
        myAnimator.SetBool("isDead" + i.ToString(), true);
        //print("Current animation: " + myAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        GetComponent<NavMeshAgent>().Stop();

        Invoke("Destroy", myAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
	}

    void Destroy()
    {
        gameObject.SetActive(false);
        CancelInvoke();
    }
}
