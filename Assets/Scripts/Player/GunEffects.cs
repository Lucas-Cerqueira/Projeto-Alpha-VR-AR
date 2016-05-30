using UnityEngine;
using UnityEngine.Networking;

public class GunEffects : MonoBehaviour {

    public ParticleSystem muzzleFlash;

    private Shooting shootingScript;
    private float elapsedTime;

    Transform FindShooterParent (Transform transform)
    {
        if (transform.CompareTag("Shooter"))
            return transform;
        return FindShooterParent(transform.parent);
    }

	// Use this for initialization
	void Start () 
    {
        shootingScript = FindShooterParent(transform).GetComponent<Shooting>();
        if (!shootingScript)
            Debug.LogError("'Shooting' script not found!");

        elapsedTime = shootingScript.shootDelay;
	}
	
	// Update is called once per frame
	void Update ()
    {
        elapsedTime += Time.deltaTime;

        if (Input.GetButton("Fire1"))
        {
            if (elapsedTime >= shootingScript.shootDelay)
            { 
                muzzleFlash.Play();
                elapsedTime = 0f;
            }
        }
	}
}
