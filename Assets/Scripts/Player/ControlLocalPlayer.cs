using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class ControlLocalPlayer : NetworkBehaviour {

    [HideInInspector] public string role;
    [HideInInspector] public bool useVuforia = false;
	[HideInInspector] public bool useCardBoard = false;

    private MyNetworkManager manager;
    private Transform spawnPoint;

    void Awake()
    {
        switch (gameObject.tag)
        {
            case "Shooter":
                {
                    role = "Shooter";
                    break;
                }
            case "General":
                {
                    role = "General";
                    break;
                }
        }
    }

    void Start()
    {
        manager = GameObject.Find("NetworkManager").GetComponent<MyNetworkManager>();

        if (isLocalPlayer)
        {
            gameObject.name = "ShooterLocal";

            int numberOfPlayers = 10;

            Camera.main.gameObject.SetActive(false);

            SetLayerOnAll(this.gameObject, 9);

            manager.matchMaker.ListMatches(0, 20, manager.matchName, manager.OnMatchList);

            if (manager.matches.Count != 0)
            {
                numberOfPlayers = manager.matches[0].currentSize;
                //foreach (UnityEngine.Networking.Match.MatchDesc match in matches)
                //{
                //    numberOfPlayers = match.currentSize;
                //}
            }
            else
                numberOfPlayers = 1;

            switch (gameObject.tag)
            {
                case "Shooter":
                    {
                        role = "Shooter";

                        Transform shooterSpawnPoints = GameObject.Find("ShooterSpawnPoints").transform;

                        // Subtracted 1 from numberOfPlayer because indexing of children starts at 0
                        spawnPoint = shooterSpawnPoints.GetChild(numberOfPlayers % shooterSpawnPoints.childCount);
						

                        // Quando desconectar da partida, lembra de desativar!!!
                        GameObject.Find("GeneralUI").transform.GetChild(0).gameObject.SetActive(false);
					    if (useCardBoard == false) 
                        {
						    GameObject.Find("ShooterUI").transform.GetChild(0).gameObject.SetActive (true);
						    transform.GetChild (0).tag = "MainCamera";
						    transform.GetChild (0).gameObject.SetActive (true);
						    transform.GetChild (1).gameObject.SetActive (false);

                            //GetComponent<RigidbodyFirstPersonController>().cam = transform.GetChild(0).GetComponent<Camera>();
					    } 
					    else 
                        {
						    GameObject.Find("ShooterUI").transform.GetChild(1).gameObject.SetActive (true);
						    //transform.GetChild(1).tag = "MainCamera";
						    transform.GetChild(0).gameObject.SetActive(false);
						    transform.GetChild(1).gameObject.SetActive(true);
                            //GetComponent<RigidbodyFirstPersonController>().cam = transform.GetChild(1).GetChild(0).GetComponentInChildren<Camera>();
					    }

   //                     GetComponent<RigidbodyFirstPersonController>().useCardboard = useCardBoard;
                        FPSPlayerMovement fpsMovement = GetComponent<FPSPlayerMovement>();
                        fpsMovement.m_Camera = Camera.main;
                        fpsMovement.m_MouseLook.Init(transform, Camera.main.transform);

                        break;
                    }
                case "General":
                    {
                        role = "General";

                        spawnPoint = GameObject.Find("GeneralSpawnPoint").transform;

                        // Quando desconectar da partida, lembra de desativar!!!
                        GameObject.Find("GeneralUI").transform.GetChild(0).gameObject.SetActive(true);
                        GameObject.Find("ShooterUI").transform.GetChild(0).gameObject.SetActive(false);

                        if (useVuforia == false)
                        {
                            transform.GetChild(0).tag = "MainCamera";
                            transform.GetChild(0).gameObject.SetActive(true);
                            transform.GetChild(1).gameObject.SetActive(false);
                        }
                        else
                        {
                            transform.GetChild(1).tag = "MainCamera";
                            transform.GetChild(0).gameObject.SetActive(false);
                            transform.GetChild(1).gameObject.SetActive(true);
                        }

                        break;
                    }
            }

			GameObject.Find ("MoneyHandlerUI").transform.GetChild (0).gameObject.SetActive (true);

            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;

			//Camera.main.enabled = false;
        }
        else
        {
            SetLayerOnAll(this.gameObject, 10);

            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && (NetworkClient.active || NetworkServer.active))
        {
            //if (isServer)
            //    manager.DisconnectHost();
            //else
			//GameObject.Find("EnemiesSpawners").GetComponent<PoolingObjectHandler> ().isInitialized = false;
            manager.DisconnectClient();
        }
    }

    static void SetLayerOnAll(GameObject obj, int layer)
    {
        foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true))
        {
            if (trans.gameObject.layer != 10)
                trans.gameObject.layer = layer;
        }
    }
}
