using UnityEngine;
using UnityEngine.Networking;

public class Combat : NetworkBehaviour
{
    public int maxHealth;
	private static int uniqueId=0;
	[SyncVar] public int id;
    public bool destroyOnDeath;

    [SyncVar (hook="OnHealthChanged")]
    public int health;
    

    void OnHealthChanged (int newHealth)
    {
        health = newHealth;
    }

	void Start ()
	{
		id = uniqueId;
		uniqueId++;

        maxHealth = health;
	}

    [Command]
    public void CmdTakeDamage(int amount)
    {
        if (!isServer)
            return;

        health -= amount;


        if (isServer)
        {
            if (health <= 0)
            {
                if (destroyOnDeath)
                {
                    this.gameObject.GetComponent<GameOver>().BroadcastMessage("endGame");
                    //Destroy(gameObject);
                    //CmdDestroyObject(this.gameObject);
                }
                else
                {
                    health = maxHealth;

                    // called on the server, will be invoked on the clients
                    //RpcRespawn();
                }
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
            // move back to zero location
            transform.position = Vector3.zero;
        }
    }


    
}