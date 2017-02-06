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
    public Vector3 Rescaling = new Vector3(3, 3, 3);


    [Header("Player Attributes")]
    public float movingSpeed = 3f;
    public float rotateSpeed = 150.0f;



    //MovementController theMoveController;
    MovementControllerNonVelocity theMoveController;

    bool isInvulnerable = false;

    HealthIndicator theHealth;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public TagDatabase theTagReference;

    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }


        /* float x = CrossPlatformInputManager.GetAxis("Horizontal") * Time.deltaTime * rotateSpeed;
        float z;
        if (CrossPlatformInputManager.GetButton("Forward"))
        {
            z = Time.deltaTime * movingSpeed;

        }
                else
            z = 0;
        */

        float x = CrossPlatformInputManager.GetAxis("Horizontal") * Time.deltaTime * movingSpeed;
        float z = CrossPlatformInputManager.GetAxis("Vertical") * Time.deltaTime * movingSpeed;

        Vector3 JoystickMovement = new Vector3(x,0,z);

        theMoveController.Move(JoystickMovement);


        /*if (CrossPlatformInputManager.GetButtonDown("Fire") || Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();

        }
        */

        // transform.Rotate(0, x, 0);
        //transform.Translate(JoystickMovement);


    }



    // CALLED ON THE CLIENT AND EXECUTED ON THE SERVER

        /*
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
    */

    void Start()
    {
        theHealth = (HealthIndicator)GetComponent<HealthIndicator>();
        //theMoveController = GetComponent<MovementController>();
        theMoveController = GetComponent<MovementControllerNonVelocity>();
    }


    public override void OnStartLocalPlayer()
    {
        //transform.FindChild("Body").GetComponent<Renderer>().material.color = playerColor;
        
        Renderer[] rend = GetComponentsInChildren<Renderer>();
        transform.parent = GameObject.Find("Scene").GetComponent<Transform>();
        //transform.localScale = Rescaling;

        foreach (Renderer R in rend)
        {
            R.material.color = playerColor;
        }
        this.transform.position = new Vector3(Random.Range(-5,5),0,Random.Range(-5, 5));



    }

    /*
    [ClientRpc]
    void RpcRespawn()
    */

    void OnTriggerEnter(Collider collision)
    {

        GameObject TheHitObject = collision.gameObject;
        int identifierTag;
        try
        {
            identifierTag = theTagReference.tagList[TheHitObject.tag];
        }
        catch
        {
            identifierTag = 0;
        }

        // Colliding with a damaging element will cause an inverse Forse push to in the X, Z axis.

        switch (identifierTag)
        {
            default:
                Debug.LogWarning("Hit NON-IDENTIFIED", TheHitObject);
                break;

            case 0:
                Debug.LogWarning("Hit NON-IDENTIFIED", TheHitObject);

                break;

            //Encountered Damaging object
            case 1:
                Debug.Log("Hit Damaging", TheHitObject);
                if (!isInvulnerable)
                {
                    theHealth.TakeDamage(10);
                    //theController.theSoundController.playClip(theController.theSoundController.SoundPlayerHit);
                    Vector3 colDir = collision.transform.position - gameObject.transform.FindChild("Origin").transform.position;

                    //AdditionalMath.EstimateHorizontalRepulsion(colDir, ref velocity, HorizontalPushByEnemyDmg);
                    //AdditionalMath.AddVerticalRepulsion(ref velocity, VerticalPushByEnemyDmg);
                }


                break;
            //Encountered Death object
            case 2:
                Debug.Log("Hit Death", TheHitObject);
                //PlayerRespawn();
                //takeDamage();
                break;
            //Encountered Vulnerable object
            case 3:
                Debug.Log("Hit Vulnerable", TheHitObject);
                Vector3 ProvisionalVector = new Vector3(0, 0, 0);
                TheHitObject.transform.parent.gameObject.SetActive(false); // deactivate instead of destroy so you could later reactivate (respawn) him

                //AdditionalMath.AddVerticalRepulsion(ref velocity, VerticalPushByEnemyKill);

                //colliders.Add(collision.collider); // saves the enemy for later respawn

                //myBody.AddForce(0, 130, 0);
                break;
            //Encountered Goal object
            case 4:
                Debug.Log("Hit Goal", TheHitObject);
                //theController.GoalReached();
                break;
            //Encountered Collectible object
            case 5:
                Debug.Log("Hit Collectible", TheHitObject);
                Collectible collect = (Collectible)TheHitObject.GetComponent(typeof(Collectible));
                if (collect.isPowerUp)
                {

                }

                TheHitObject.SetActive(false); // deactivate instead of destroy so you could later reactivate (respawn) him
                break;
        }

        TheHitObject = null;
    }



}
