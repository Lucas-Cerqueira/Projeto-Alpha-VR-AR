using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GeneralUIHab : NetworkBehaviour {

	private bool isUsing= false;
	private Ray ray;
	private RaycastHit hit;
	GameObject generalUI;

    // Variables to Android builds
    private Touch touch;



    // Use this for initialization
	void Start () {
		generalUI = GameObject.Find ("GeneralUICommands");
	}
	
	// Update is called once per frame
	void Update () {
        if (isLocalPlayer)
        {
            if (Input.touches != null && Application.platform == RuntimePlatform.Android)
                touch = Input.GetTouch(0);
                
            if (Input.GetMouseButtonDown(0) || (touch.phase == TouchPhase.Began && Application.platform == RuntimePlatform.Android))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 500))
                {
                    if (hit.collider.gameObject.tag == "Tower")
                        isUsing = true;
                    else
                        isUsing = false;
                }
            }
            if (isUsing)
                generalUI.SetActive(true);
            else
                generalUI.SetActive(false);
        }
	}
}
