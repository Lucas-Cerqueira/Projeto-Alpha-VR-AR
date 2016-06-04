using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] float enemySpawnDelay = 1f;

    private float elapsedTime;
    private bool gameStarted = false;

    void Awake()
    {
        
    }

    void Start ()
    {
        elapsedTime = enemySpawnDelay;
    }

	public override void OnStartServer()
	{
		gameStarted = true;
	}

	void FixedUpdate()
	{
		if (gameStarted && isServer)
		{
			//RpcSyncClientServerPooledObjects();
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= enemySpawnDelay)
			{
				GetComponent<PoolingObjectHandler> ().ServerCreateFromPool(transform.position, transform.rotation);
				elapsedTime = 0;
			}
		}
	}
}
