using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] float enemySpawnDelay = 1f;
    [SerializeField] float enemySpeed = 6;

    private float elapsedTime;
    private bool gameStarted = false;

    public override void OnStartServer()
    {
        gameStarted = true;
    }

    void Start ()
    {
        elapsedTime = enemySpawnDelay;
    }

    [Command]
    void CmdSpawnEnemy()
    {
        GameObject enemy = (GameObject) Instantiate(Resources.Load("Enemy1", typeof(GameObject)), transform.position, transform.rotation);
		enemy.transform.SetParent (transform);
        enemy.GetComponent<Rigidbody>().velocity = transform.forward * enemySpeed;
        NetworkServer.Spawn(enemy);
        //Destroy(enemy, 10);
    }

    void FixedUpdate ()
    {
        if (gameStarted)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= enemySpawnDelay)
            {
                CmdSpawnEnemy();
                elapsedTime = 0;
            }
        }
    }
}
