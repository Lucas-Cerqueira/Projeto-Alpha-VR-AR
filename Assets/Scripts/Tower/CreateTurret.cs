using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CreateTurret : NetworkBehaviour {

    [SerializeField] int turretPrice = 100;

	private GameObject plotTurret;
	private bool isDeploying= false;
	private Ray ray;
	private RaycastHit hit;

	int layermask = 1 << 8;

    private MoneyHandler moneyHandler;
	// Use this for initialization

	void Start ()
	{
		if (isLocalPlayer)
        {
            moneyHandler = GameObject.Find("Money").GetComponent<MoneyHandler>();
			Button button = (Button)GameObject.Find ("Create Turret").GetComponent<Button> ();
            button.onClick.RemoveAllListeners();
			button.onClick.AddListener (() => MakeTurret ());

            plotTurret = (GameObject)Instantiate(Resources.Load("Turret"), Vector3.zero, Quaternion.identity);
            plotTurret.tag = "Untagged";
            plotTurret.transform.GetChild(0).gameObject.SetActive(false);
            plotTurret.SetActive(false);
		}
	}


	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) 
        {
			if (isDeploying) 
            {
				ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (ray, out hit, 500, layermask))
                {
					if (hit.collider.gameObject.tag == "Scenario") 
                    {
						if (plotTurret) 
                        {
							Debug.DrawLine (ray.origin, hit.point, Color.cyan);
							//plotTurret.GetComponent<Renderer> ().enabled = true;
                            plotTurret.SetActive(true);
                            plotTurret.transform.position = hit.point;
							//plotTurret.transform.position = new Vector3 (hit.point.x, hit.point.y + plotTurret.GetComponent<Renderer> ().bounds.extents.y, hit.point.z);
							//plotTurret.transform.rotation = hit.collider.transform.rotation;
						}
                        //else 
                        //{
                        //    //plotTurret = (GameObject)Instantiate (Resources.Load("Turret"), hit.point, hit.collider.transform.rotation);
                        //    //plotTurret.transform.GetChild (0).gameObject.SetActive (false);
                        //    //plotTurret.GetComponent<NavMeshObstacle> ().enabled = false;
                        //    plotTurret.transform.position = new Vector3 (plotTurret.transform.position.x, plotTurret.transform.position.y + plotTurret.GetComponent<Renderer> ().bounds.extents.y, plotTurret.transform.position.z);
                        //    plotTurret.GetComponent<Collider> ().enabled = false;
                        //}
					}
					if (Input.GetMouseButtonDown (0) && plotTurret) 
                    {
						//plotTurret.GetComponent<Collider> ().enabled = true;
						//plotTurret = null;

                        GenerateTurret(plotTurret.transform.position, plotTurret.transform.rotation, (GameObject)Resources.Load("Turret"));
						//Destroy (plotTurret);
                        plotTurret.SetActive(false);
						isDeploying = false;
					}
				} 
                else 
                {
                    if (plotTurret)
                        //plotTurret.GetComponent<Renderer> ().enabled = false;
                        plotTurret.SetActive(false);
				}
			}
		}
	}

	public void MakeTurret ()
	{
        GetComponent<GeneralUIHab>().isUsingUI = false;
        if (moneyHandler.GetMoney() >= turretPrice)
        {
            moneyHandler.SpendMoney(turretPrice);
            isDeploying = true;
        }
	}

	[Client]
	void GenerateTurret (Vector3 pos, Quaternion rotation, GameObject prefabObject)
	{
		int prefabIndex = NetworkManager.singleton.spawnPrefabs.IndexOf (prefabObject);
		CmdCreateTurret (pos, rotation, prefabIndex);
	}

	[Command]
	void CmdCreateTurret (Vector3 pos, Quaternion rotation, int prefabIndex)
	{
		GameObject prefabToSpawn = NetworkManager.singleton.spawnPrefabs [prefabIndex];
		GameObject spawner = (GameObject)Instantiate (prefabToSpawn, pos, rotation);
		//NetworkServer.SpawnWithClientAuthority (spawner, connectionToClient);
		NetworkServer.Spawn (spawner);
	}
}
