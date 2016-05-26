using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ControlLocalPlayer : NetworkBehaviour {

    public string role;
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
            print(useVuforia);

            int numberOfPlayers = 10;


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

                        // Subtracted 1 from numberOfPlayer because indexing of children starts at 0
                        spawnPoint = GameObject.Find("ShooterSpawnPoints").transform.GetChild(numberOfPlayers%transform.childCount);
						

                        // Quando desconectar da partida, lembra de desativar!!!
                        GameObject.Find("GeneralUI").transform.GetChild(0).gameObject.SetActive(false);
					if (useCardBoard == false) {
						GameObject.Find("ShooterUI").transform.GetChild(0).gameObject.SetActive (true);
						transform.GetChild (0).tag = "MainCamera";
						transform.GetChild (0).gameObject.SetActive (true);
						transform.GetChild (1).gameObject.SetActive (false);
					} 
					else {
						GameObject.Find("ShooterUI").transform.GetChild(1).gameObject.SetActive (true);
						//transform.GetChild(1).tag = "MainCamera";
						transform.GetChild(0).gameObject.SetActive(false);
						transform.GetChild(1).gameObject.SetActive(true);
					}

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

            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;

			//Camera.main.enabled = false;
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
            if (gameObject.CompareTag("General"))
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
                manager.DisconnectClient();
        }
    }
}
