using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CreateDrop : NetworkBehaviour {

	public GameObject drop;
	private GameObject plotDrop;
	private bool isPloting= false;
	private Ray ray;
	private RaycastHit hit;
	int layermask = 1 << 8;
	public GameObject money;
	public float distance = 100.0f;
	int muney = 0;
	// Use this for initialization

	void Start ()
	{
		if(isLocalPlayer){
			Button button = (Button)GameObject.Find ("Create Drop").GetComponent<Button> ();
			button.onClick.AddListener (() => makedrop ());
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
						if (plotDrop) {
							Debug.DrawLine (ray.origin, hit.point, Color.cyan);
							plotDrop.GetComponent<Renderer> ().enabled = true;
							plotDrop.transform.position = new Vector3 (hit.point.x, hit.point.y + distance, hit.point.z);
							plotDrop.transform.rotation = hit.collider.transform.rotation;
						} else {
							plotDrop = (GameObject)Instantiate (drop, hit.point, Quaternion.identity);
							plotDrop.transform.position = new Vector3 (plotDrop.transform.position.x, plotDrop.transform.position.y + distance, plotDrop.transform.position.z);
							plotDrop.GetComponent<Collider> ().enabled = false;
						}
					}
					if (Input.GetMouseButtonDown (0) && plotDrop) {
						Generatedrop (plotDrop.transform.position, plotDrop.transform.rotation, drop);
						Destroy (plotDrop);
						isPloting = false;
					}
				} else {
					if (plotDrop)
						plotDrop.GetComponent<Renderer> ().enabled = false;
				}
			}
		}
	}

	public void makedrop ()
	{
		//		if (muney>=100)
		//		{
		//			getmoney().GetComponent<MoneyHandler>().sumMoney (-100);
					isPloting = true;
		//		}
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

	public GameObject getmoney()
	{
		return money;
	}
}
