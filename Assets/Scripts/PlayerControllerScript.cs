using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerControllerScript : NetworkBehaviour
{
    [SyncVar]
    public string pname = "thePlayer";
    [SyncVar]
    public Color playerColor = Color.green;


    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    

    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }


        float x = CrossPlatformInputManager.GetAxis("Horizontal") * Time.deltaTime * 150.0f; ;
        float z;
        if (CrossPlatformInputManager.GetButton("Forward"))
        {
            z =  Time.deltaTime * 3.0f;

        }
            
        else
            z = 0;

        if (CrossPlatformInputManager.GetButtonDown("Fire") || Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();

        }


        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);


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
        //transform.FindChild("Body").GetComponent<Renderer>().material.color = playerColor;
        Renderer[] rend = GetComponentsInChildren<Renderer>();
        foreach (Renderer R in rend)
        {
            R.material.color = playerColor;
        }
        this.transform.position = new Vector3(Random.Range(-5,5),0,Random.Range(-5, 5));



    }



}
