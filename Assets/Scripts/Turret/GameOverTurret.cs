using UnityEngine;
using System.Collections;

public class GameOverTurret : GameOver {

	public override void EndGame ()
    {
        Destroy(this.gameObject);
    }
}
