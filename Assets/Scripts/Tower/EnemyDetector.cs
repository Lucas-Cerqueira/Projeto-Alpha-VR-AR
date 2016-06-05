using UnityEngine;
using System.Collections;

public class EnemyDetector : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            print("Entrou");
            //other.GetComponent<Navigation_Enemy>().foundTower = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
           // other.GetComponent<Navigation_Enemy>().foundTower = true;
        }
    }
}
