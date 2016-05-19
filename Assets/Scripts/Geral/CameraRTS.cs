using UnityEngine;
using System.Collections;

public class CameraRTS : MonoBehaviour {

	public float speed = 5.0f;
	public float pixelEdge = 10f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		keyboardInput ();
	}

	private void keyboardInput ()
	{
		if (Input.GetKey (KeyCode.UpArrow) ||(Input.mousePosition.y > Screen.height - pixelEdge && Input.GetMouseButton(0)))
			this.transform.Translate (Vector3.forward*speed*Time.deltaTime, Space.Self);
		
		if (Input.GetKey (KeyCode.LeftArrow) || (Input.mousePosition.x < 0 + pixelEdge && Input.GetMouseButton(0)))
			this.transform.Translate (Vector3.left*speed*Time.deltaTime, Space.Self);
		
		if (Input.GetKey (KeyCode.RightArrow) || (Input.mousePosition.x > Screen.width - pixelEdge && Input.GetMouseButton(0)))
			this.transform.Translate (Vector3.right*speed*Time.deltaTime, Space.Self);
		
		if (Input.GetKey (KeyCode.DownArrow) || (Input.mousePosition.y < 0 + pixelEdge && Input.GetMouseButton(0)))
			this.transform.Translate (Vector3.back*speed*Time.deltaTime, Space.Self);
	}
}
