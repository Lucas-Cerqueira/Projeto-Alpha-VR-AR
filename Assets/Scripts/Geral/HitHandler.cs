using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HitHandler : NetworkBehaviour {

	[ClientRpc]
	public virtual void RpcTakeHit()
    {

    }
}
