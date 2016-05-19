using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_SyncPosition : NetworkBehaviour {

    [SyncVar]
    private Vector3 syncPos;
    [SyncVar]
    private Quaternion syncRot;


    [SerializeField]
    float lerpRate = 15;

	[Command]
    void CmdSendPositionToServer (Vector3 pos, Quaternion rot)
    {
        syncPos = pos;
        syncRot = rot;
    }

    // Só roda no cliente
    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer)
            CmdSendPositionToServer(this.transform.position, this.transform.rotation);
    }

    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, syncPos, Time.deltaTime * lerpRate);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, syncRot, Time.deltaTime * lerpRate);
        }

    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        TransmitPosition();
        LerpPosition();
	}
}
