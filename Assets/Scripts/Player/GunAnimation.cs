using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GunAnimation : NetworkBehaviour {

	[SerializeField] private bool holdToAim = false;
    [SerializeField] private float zoomInFOV = 40;

    private Animator myAnimator;
    private bool isAiming;
    private bool playedShooting;
    private float defaultFOV;
    private Shooting shootingScript;

	// Use this for initialization
	void Start ()
    {
        myAnimator = GetComponent<Animator>();
        isAiming = false;
        playedShooting = false;
        defaultFOV = Camera.main.fieldOfView;
        shootingScript = GameObject.Find("ShooterLocal").GetComponent<Shooting>();

        myAnimator.SetFloat("shootingSpeed", 1 / shootingScript.shootDelay);
	}
	
	// Update is called once per frame
    void Update()
    {
        if (!shootingScript)
            shootingScript = GameObject.Find("ShooterLocal").GetComponent<Shooting>();

        if (holdToAim)
        {
            if (Input.GetMouseButton(1))
            {
                myAnimator.SetBool("isAiming", true);
                Camera.main.fieldOfView = Mathf.Lerp(defaultFOV, zoomInFOV, .5f);
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

        //myAnimator.SetBool("isShooting", shootingScript.isShooting);
        if (shootingScript.isMachineGun)
        {
            if (Input.GetButton("Fire1"))
                myAnimator.SetBool("isShooting", true);
            else
                myAnimator.SetBool("isShooting", false);
        }
        else
        {
            if (shootingScript.isShooting && !playedShooting)
            {
                StartCoroutine(PlayOneShot("isShooting"));
                playedShooting = true;
            }

            if (!shootingScript.isShooting && playedShooting)
                playedShooting = false;
        }   
    }

    public IEnumerator PlayOneShot(string paramName)
    {
        myAnimator.SetBool(paramName, true);
        yield return null;
        myAnimator.SetBool(paramName, false);
        //if (string.Compare(paramName, "isShooting") == 0)
        //    isShooting = false;
    }
}
