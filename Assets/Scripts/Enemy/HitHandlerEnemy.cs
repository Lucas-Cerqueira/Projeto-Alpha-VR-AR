using UnityEngine;
using System.Collections;

public class HitHandlerEnemy : HitHandler {

    public ParticleSystem blood;

	// Use this for initialization
	void Update () 
    {
        if (GetComponent<Combat>().isDead)
            blood.Stop();
	}
	
	public override void TakeHit()
    {
        blood.Play();
    }


}
