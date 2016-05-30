using UnityEngine;
using UnityEngine.Networking;

public class Combat : NetworkBehaviour
{
    [SerializeField] private bool destroyOnDeath;
    public int maxHealth = 100;
	
	[HideInInspector][SyncVar] public int id;
    private static int uniqueId = 0;

    [HideInInspector][SyncVar (hook="OnHealthChanged")]
    public int health;

    [HideInInspector][SyncVar]public bool isDead;

    private Transform shooterSpawnPoints;

    void OnHealthChanged (int newHealth)
    {
        health = newHealth;
    }

	void OnEnable ()
	{
        shooterSpawnPoints = GameObject.Find("ShooterSpawnPoints").transform;
        if (GetComponent<HealthBar>())
            GetComponent<HealthBar>().enabled = true;

		id = uniqueId;
		uniqueId++;

        health = maxHealth;

        isDead = false;
	}

    //void Update()
    //{
    //    if (isServer)
    //    {
    //        if (health <= 0)
    //        {
    //            isDead = true;

    //            if (destroyOnDeath)
    //            {
    //                this.gameObject.GetComponent<GameOver>().EndGame();
    //                //Destroy(gameObject);
    //                //CmdDestroyObject(this.gameObject);
    //            }
    //            else
    //            {
    //                health = maxHealth;
    //                RpcRespawn();
    //            }
    //        }
    //    }
    //}

    [Command]
    public void CmdTakeDamage(int amount)
    {
        if (!isServer)
            return;

        health -= amount;

        if (health <= 0)
        {
            isDead = true;

            if (destroyOnDeath)
            {
                this.gameObject.GetComponent<GameOver>().EndGame();
                //Destroy(gameObject);
                //CmdDestroyObject(this.gameObject);
            }
            else
            {
                health = maxHealth;
                RpcRespawn();
            }
        }      
    }

    [Command]
    void CmdDestroyObject (GameObject target)
    {
        NetworkServer.Destroy (target);
    }


    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            int point = Random.Range(0, shooterSpawnPoints.childCount);
            transform.position = shooterSpawnPoints.GetChild(point).position;
            transform.rotation = shooterSpawnPoints.GetChild(point).rotation;
            isDead = false;
        }
    }
}