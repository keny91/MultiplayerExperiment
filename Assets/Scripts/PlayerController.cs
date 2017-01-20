using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : NetworkBehaviour {

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

	// Update is called once per frame
	void Update () {

        if (!isLocalPlayer)
        {
            return;
        }

        float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }

	}



    // CALLED ON THE CLIENT AND EXECUTED ON THE SERVER
    [Command]
    void CmdFire()
    {
        // Create bullet from prefab
        GameObject bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6.0f;
        NetworkServer.Spawn(bullet);
        //Destroy the bullet after 2 seconds
        Destroy(bullet, 2);

    }

    public override void OnStartLocalPlayer()
    {
        transform.FindChild("Body").GetComponent<Renderer>().material.color = Color.green;

    }



}
