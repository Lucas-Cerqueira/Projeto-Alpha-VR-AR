using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CreateTurret : NetworkBehaviour {

	public GameObject turret;
	private GameObject plotTurret;
	private bool isPloting= false;
	private Ray ray;
	private RaycastHit hit;
	int layermask = 1 << 8;
	public GameObject money;
	int muney = 0;
	// Use this for initialization

	void Start ()
	{
		if (isLocalPlayer) {
			Button button = (Button)GameObject.Find ("Create Turret").GetComponent<Button> ();
			button.onClick.AddListener (() => makeTurret ());
			money = GameObject.Find ("Money");
			muney = money.GetComponent<MoneyHandler> ().getMoney ();
		}
	}


	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) {
			muney = money.GetComponent<MoneyHandler> ().getMoney ();
			if (isPloting) {
				ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (ray, out hit, 500, layermask)) {
					if (hit.collider.gameObject.tag == "Scenario") {
						if (plotTurret) {
							Debug.DrawLine (ray.origin, hit.point, Color.cyan);
							plotTurret.GetComponent<Renderer> ().enabled = true;
							plotTurret.transform.position = new Vector3 (hit.point.x, hit.point.y + plotTurret.GetComponent<Renderer> ().bounds.extents.y, hit.point.z);
							plotTurret.transform.rotation = hit.collider.transform.rotation;
						} else {
							plotTurret = (GameObject)Instantiate (turret, hit.point, hit.collider.transform.rotation);
							plotTurret.transform.GetChild (0).gameObject.SetActive (false);
							plotTurret.GetComponent<NavMeshObstacle> ().enabled = false;
							plotTurret.transform.position = new Vector3 (plotTurret.transform.position.x, plotTurret.transform.position.y + plotTurret.GetComponent<Renderer> ().bounds.extents.y, plotTurret.transform.position.z);
							plotTurret.GetComponent<Collider> ().enabled = false;
						}
					}
					if (Input.GetMouseButtonDown (0) && plotTurret) {
						//plotTurret.GetComponent<Collider> ().enabled = true;
						//plotTurret = null;

						GenerateTurret (plotTurret.transform.position, plotTurret.transform.rotation, turret);
						Destroy (plotTurret);
						isPloting = false;
					}
				} else {
					if (plotTurret)
						plotTurret.GetComponent<Renderer> ().enabled = false;
				}
			}
		}
	}

	public void makeTurret ()
	{
//		if (muney>=100)
//		{
//			getmoney().GetComponent<MoneyHandler>().sumMoney (-100);
			isPloting = true;
//		}
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

	public GameObject getmoney()
	{
		return money;
	}
}
