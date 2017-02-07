using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUpControl : CollectibleMultiplayer
{

    bool randomSpawn;
    bool Collected;
    new bool isPowerUp = true;
    bool despawnsOnCollected;
    /*
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    */



    /// <summary>
    /// Make the Object dissapear from the scene.
    /// </summary>
    [ClientRpc]
    public void RpcDespawn()
    {
        Destroy(gameObject);
        //this.transform.gameObject.
    }

    /// <summary>
    /// Trigger event when the Collectible has been collected
    /// </summary>
    /// <param name="theP"></param>
    public override void OnCollected(MultiplayerControlScript theP)
    {
        Effect(theP);
        RpcDespawn();
    }

    /// <summary>
    /// Trigger the effect.
    /// </summary>
    /// <param name="theP"></param>
    public virtual void Effect(MultiplayerControlScript theP)
    {

    }


}
