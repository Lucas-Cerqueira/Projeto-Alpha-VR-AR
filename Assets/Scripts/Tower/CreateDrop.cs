using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CreateDrop : NetworkBehaviour {

    [SerializeField] int dropPrice = 100;
    [SerializeField] float heightDrop = 100f;

	private GameObject plotDrop;
	private bool isDeploying= false;
	private Ray ray;
	private RaycastHit hit;

	int layermask = 1 << 8;

    private MoneyHandler moneyHandler;
	// Use this for initialization

	void Start ()
	{
		if(isLocalPlayer)
        {
			Button button = (Button)GameObject.Find ("Create Drop").GetComponent<Button> ();
            button.onClick.RemoveAllListeners();
			button.onClick.AddListener (() => MakeDrop ());
            moneyHandler = GameObject.Find("Money").GetComponent<MoneyHandler>();
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
						if (plotDrop) 
                        {
							Debug.DrawLine (ray.origin, hit.point, Color.cyan);
							plotDrop.GetComponent<Renderer> ().enabled = true;
                            plotDrop.transform.position = new Vector3(hit.point.x, hit.point.y + heightDrop, hit.point.z);
							plotDrop.transform.rotation = hit.collider.transform.rotation;
						} 
                        else 
                        {
							plotDrop = (GameObject)Instantiate (Resources.Load ("Drop"), hit.point, Quaternion.identity);
                            plotDrop.transform.position = new Vector3(plotDrop.transform.position.x, plotDrop.transform.position.y + heightDrop, plotDrop.transform.position.z);
							plotDrop.GetComponent<Collider> ().enabled = false;
						}
					}
					if (Input.GetMouseButtonDown (0) && plotDrop) 
                    {
                        Generatedrop(plotDrop.transform.position, plotDrop.transform.rotation, (GameObject)Resources.Load("Drop"));
						Destroy (plotDrop);
						isDeploying = false;
					}
				} 
                else 
                {
					if (plotDrop)
						plotDrop.GetComponent<Renderer> ().enabled = false;
				}
			}
		}
	}

	public void MakeDrop ()
	{
        GetComponent<GeneralUIHab>().isUsingUI = false;
        if (moneyHandler.GetMoney() >= dropPrice)
        {
            moneyHandler.SpendMoney(dropPrice);
            isDeploying = true;
        }
	}

	[Client]
	void Generatedrop (Vector3 pos, Quaternion rotation, GameObject prefabObject)
	{
		int prefabIndex = NetworkManager.singleton.spawnPrefabs.IndexOf (prefabObject);
		CmdCreatedrop (pos, rotation, prefabIndex);
	}

	[Command]
	void CmdCreatedrop (Vector3 pos, Quaternion rotation, int prefabIndex)
	{
		GameObject prefabToSpawn = NetworkManager.singleton.spawnPrefabs [prefabIndex];
		GameObject spawner = (GameObject)Instantiate (prefabToSpawn, pos, rotation);
		//NetworkServer.SpawnWithClientAuthority (spawner, connectionToClient);
		NetworkServer.Spawn (spawner);
	}
}
