using UnityEngine;
using System.Collections;

public class GunAnimation : MonoBehaviour {

	[SerializeField] private bool holdToAim = false;
    [SerializeField] private float zoomInFOV = 40;

    private Animator myAnimator;
    private bool isAiming;
    private float defaultFOV;

	// Use this for initialization
	void Start ()
    {
        myAnimator = GetComponent<Animator>();
        isAiming = false;
        defaultFOV = Camera.main.fieldOfView;
	}
	
	// Update is called once per frame
    void Update()
    {
        if (holdToAim)
        {
            if (Input.GetMouseButton(1))
            {
                myAnimator.SetBool("isAiming", true);
                Camera.main.fieldOfView = Mathf.Lerp (defaultFOV, zoomInFOV, .5f);
            }
            else
            {
                myAnimator.SetBool("isAiming", false);
                Camera.main.fieldOfView = Mathf.Lerp(zoomInFOV, defaultFOV, .5f);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
                isAiming = !isAiming;

            if (isAiming)
                //Camera.main.fieldOfView = Mathf.Lerp(zoomInFOV, defaultFOV, 0.1f);
                Camera.main.fieldOfView = zoomInFOV;
            else
                //Camera.main.fieldOfView = Mathf.Lerp(defaultFOV, zoomInFOV, 0.1f);
                Camera.main.fieldOfView = defaultFOV;

            myAnimator.SetBool("isAiming", isAiming);
        }

        if (Input.GetButton("Fire1"))
            myAnimator.SetBool("isShooting", true);
        else
            myAnimator.SetBool("isShooting", false);
    }
}
