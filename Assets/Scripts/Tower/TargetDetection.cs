using UnityEngine;
using System.Collections;

public class TargetDetection : MonoBehaviour {

    private Shoot_Laser shootLaser;

    void Start()
    {
        shootLaser = transform.parent.GetComponent<Shoot_Laser>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            shootLaser.enemiesInRange.Add(other.transform.GetComponentInParent<Combat>());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
            shootLaser.enemiesInRange.Remove(other.transform.GetComponentInParent<Combat>());
    }
}
