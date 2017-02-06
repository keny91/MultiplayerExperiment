using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUpControl : Collectible {

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


    [ClientRpc]
    public void RpcDespawn()
    {
        Destroy(gameObject);
        //this.transform.gameObject.
    }

    public void OnCollected(PlayerControllerScript theP)
    {
        Effect(theP);
        RpcDespawn();
    }


    public virtual void Effect(PlayerControllerScript theP)
    {

    }


}
