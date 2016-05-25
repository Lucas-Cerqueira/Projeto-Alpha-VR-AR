using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ControlLocalPlayer : NetworkBehaviour {

    public string role;

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

                        GetComponent<MeshRenderer>().material.color = Color.red;
                        // Subtracted 1 from numberOfPlayer because indexing of children starts at 0
                        spawnPoint = GameObject.Find("ShooterSpawnPoints").transform.GetChild(numberOfPlayers%transform.childCount);
						

                        // Quando desconectar da partida, lembra de desativar!!!
                        GameObject.Find("ShooterUI").transform.GetChild(0).gameObject.SetActive (true);
                        GameObject.Find("GeneralUI").transform.GetChild(0).gameObject.SetActive(false);

                        break;
                    }
                case "General":
                    {
                        role = "General";

                        spawnPoint = GameObject.Find("GeneralSpawnPoint").transform;

                        // Quando desconectar da partida, lembra de desativar!!!
                        GameObject.Find("GeneralUI").transform.GetChild(0).gameObject.SetActive(true);
                        GameObject.Find("ShooterUI").transform.GetChild(0).gameObject.SetActive(false);

                        break;
                    }
            }

            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;

			//Camera.main.enabled = false;
        }
        else
        {
            if (gameObject.tag == "Shooter")
                GetComponent<MeshRenderer>().material.color = Color.red;
            transform.GetChild(0).gameObject.SetActive(false);
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
