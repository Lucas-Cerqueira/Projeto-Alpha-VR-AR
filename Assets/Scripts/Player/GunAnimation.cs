using UnityEngine;
using System.Collections;

public class GunAnimation : MonoBehaviour {

	[SerializeField] private bool holdToAim = false;

    private Animator myAnimator;
    private bool isAiming;

	// Use this for initialization
	void Start ()
    {
        myAnimator = GetComponent<Animator>();
        isAiming = false;
	}
	
	// Update is called once per frame
    void Update()
    {
        if (holdToAim)
        {
            if (Input.GetMouseButton(1))
                myAnimator.SetBool("isAiming", true);
            else
                myAnimator.SetBool("isAiming", false);
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
                isAiming = !isAiming;

            myAnimator.SetBool("isAiming", isAiming);
        }

        if (Input.GetButton("Fire1"))
            myAnimator.SetBool("isShooting", true);
        else
            myAnimator.SetBool("isShooting", false);
    }
}
