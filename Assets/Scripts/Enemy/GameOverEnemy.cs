using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameOverEnemy : GameOver{

    private Animator myAnimator;
	private GameObject money;

    public bool isDead = false;

	// Use this for initialization
		
    void Start()
    {
        money = GameObject.Find("Money");
        myAnimator = GetComponent<Animator>();
    }
	
    void OnEnable()
    {
        transform.GetChild(1).GetComponent<CapsuleCollider>().enabled = true;
		GetComponent<Navigation_Enemy> ().enabled = true;
    }

	// Update is called once per frame
	void Update () {
	
	}

	public override void EndGame ()
	{
        RpcDeactivateStuff();
        GetComponent<HealthBar>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<NavMeshAgent>().Stop();

        
        //NetworkServer.Destroy(this.gameObject);
        int i = Random.Range(1, 3);
        myAnimator.Rebind();
        myAnimator.SetBool("isDead" + i.ToString(), true);
		if (isServer) 
		{
			if (money && GetComponent<Combat>().health >= 0) money.GetComponent<MoneyHandler>().AddMoney(100);
			transform.parent.GetComponent<PoolingObjectHandler> ().ServerReturnToPool (this.gameObject, myAnimator.GetCurrentAnimatorClipInfo (0) [0].clip.length);
		}
	}

    [ClientRpc]
    void RpcDeactivateStuff()
    {
        GetComponent<HealthBar>().enabled = false;
		GetComponent<Navigation_Enemy> ().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<NavMeshAgent>().Stop();
    }
}
