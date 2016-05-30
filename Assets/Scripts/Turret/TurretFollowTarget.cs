using UnityEngine;
using System.Collections;

public class TurretFollowTarget : MonoBehaviour {

    private Shoot_Laser shootLaserScript;

    void Start()
    {
        shootLaserScript = GetComponent<Shoot_Laser>();
    }

	// Update is called once per frame
	void Update ()
    {
        if (shootLaserScript.enemiesInRange.Count > 0)
        {
            Vector3 oldRotation = transform.eulerAngles;
            transform.LookAt(shootLaserScript.enemiesInRange[0].transform.position);
            transform.rotation = Quaternion.Euler(oldRotation.x, transform.eulerAngles.y, oldRotation.z);
        }
	}
}
