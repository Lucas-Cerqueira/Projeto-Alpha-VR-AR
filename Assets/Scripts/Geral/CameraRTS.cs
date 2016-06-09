using UnityEngine;
using System.Collections;

public class CameraRTS : MonoBehaviour {

	public float speed = 5.0f;
	public float pixelEdge = 15f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
		    KeyboardInput ();
        else if (Application.platform == RuntimePlatform.Android)
            TouchInput();
	}

	private void KeyboardInput ()
	{
		if (Input.GetKey (KeyCode.UpArrow) ||(Input.mousePosition.y > Screen.height - pixelEdge && Input.GetMouseButton(0)))
			this.transform.Translate (Vector3.up*speed*Time.deltaTime, Space.Self);
		
		if (Input.GetKey (KeyCode.LeftArrow) || (Input.mousePosition.x < 0 + pixelEdge && Input.GetMouseButton(0)))
			this.transform.Translate (Vector3.left*speed*Time.deltaTime, Space.Self);
		
		if (Input.GetKey (KeyCode.RightArrow) || (Input.mousePosition.x > Screen.width - pixelEdge && Input.GetMouseButton(0)))
			this.transform.Translate (Vector3.right*speed*Time.deltaTime, Space.Self);
		
		if (Input.GetKey (KeyCode.DownArrow) || (Input.mousePosition.y < 0 + pixelEdge && Input.GetMouseButton(0)))
			this.transform.Translate (Vector3.down*speed*Time.deltaTime, Space.Self);
	}

    private void TouchInput ()
    {
        if (Input.touchCount != 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.position.y > Screen.height - pixelEdge && Input.GetMouseButton(0))
                this.transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);

            if (touch.position.x < 0 + pixelEdge && Input.GetMouseButton(0))
                this.transform.Translate(Vector3.left * speed * Time.deltaTime, Space.Self);

            if (touch.position.x > Screen.width - pixelEdge && Input.GetMouseButton(0))
                this.transform.Translate(Vector3.right * speed * Time.deltaTime, Space.Self);

            if (touch.position.y < 0 + pixelEdge && Input.GetMouseButton(0))
                this.transform.Translate(Vector3.down * speed * Time.deltaTime, Space.Self);

            if (touch.phase == TouchPhase.Moved)
                this.transform.Translate(new Vector3 (touch.deltaPosition.x, 0, touch.deltaPosition.y)*speed * Time.deltaTime, Space.Self);
        }
    }
}
