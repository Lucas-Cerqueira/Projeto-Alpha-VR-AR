using UnityEngine;
using System.Collections;

public class ReticleFollowGun : MonoBehaviour 
{

    private Transform weapon;
    private Transform weaponInitialTransform;
    private Vector3 initialPos;

	// Use this for initialization
	void Start () 
    {
        weapon = transform.parent.Find("Weapon");
        weaponInitialTransform = weapon;
        initialPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
    {
        print((weapon.forward));
        transform.position = new Vector3(transform.position.x, 
                                initialPos.y + (weapon.position.y - weaponInitialTransform.position.y) + (weaponInitialTransform.forward - weapon.forward).y,
                                transform.position.z);
	}
}
