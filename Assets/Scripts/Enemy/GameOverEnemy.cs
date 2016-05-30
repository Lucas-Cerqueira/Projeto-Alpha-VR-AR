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
        transform.GetChild(1).GetComponent<BoxCollider>().enabled = true;
    }

	// Update is called once per frame
	void Update () {
	
	}

	public override void EndGame ()
	{
        RpcDeactivateStuff();
        GetComponent<HealthBar>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).GetComponent<BoxCollider>().enabled = false;
        GetComponent<NavMeshAgent>().Stop();

        myAnimator.SetBool("isAttacking", false);
		if (money) money.GetComponent<MoneyHandler>().AddMoney(100);
        //NetworkServer.Destroy(this.gameObject);
        int i = Random.Range(1, 3);
        myAnimator.SetBool("isDead" + i.ToString(), true);

        if (isServer)
            transform.parent.GetComponent<PoolingObjectHandler>().ServerReturnToPool(this.gameObject, myAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
	}

    [ClientRpc]
    void RpcDeactivateStuff()
    {
        GetComponent<HealthBar>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).GetComponent<BoxCollider>().enabled = false;
        GetComponent<NavMeshAgent>().Stop();
    }
}
