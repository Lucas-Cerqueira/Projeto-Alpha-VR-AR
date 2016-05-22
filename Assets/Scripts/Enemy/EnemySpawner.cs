using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] float pooledEnemiesAmount = 20;
    [SerializeField] float enemySpawnDelay = 1f;
    [SerializeField] float enemySpeed = 6;

    private float elapsedTime;
    private bool gameStarted = false;

    List<GameObject> enemyPool;

    public override void OnStartServer()
    {
        gameStarted = true;
    }

    void Awake()
    {
        
    }

    void Start ()
    {
        enemyPool = new List<GameObject>();
        for (int i = 0; i < pooledEnemiesAmount; i++)
            CmdPoolEnemy();

        elapsedTime = enemySpawnDelay;
    }

    [Command]
    void CmdPoolEnemy()
    {
        GameObject enemy = (GameObject) Instantiate(Resources.Load("Enemy1", typeof(GameObject)), transform.position, transform.rotation);
        enemy.SetActive(false);
		enemy.transform.SetParent (transform);
        enemy.GetComponent<Rigidbody>().velocity = transform.forward * enemySpeed;
        NetworkServer.Spawn(enemy);
        enemyPool.Add(enemy);
        //Destroy(enemy, 10);
    }

    [Command]
    void CmdActivateEnemy()
    {
        for (int i = 0; i < pooledEnemiesAmount; i++)
        {
            if (!enemyPool[i].activeInHierarchy)
            {
                enemyPool[i].transform.position = transform.position;
                enemyPool[i].transform.rotation = transform.rotation;
                enemyPool[i].SetActive(true);
                return;
            }
        }
    }

    void FixedUpdate ()
    {
        if (gameStarted)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= enemySpawnDelay)
            {
                CmdActivateEnemy();
                elapsedTime = 0;
            }
        }
    }
}
