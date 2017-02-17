﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using UnityStandardAssets.CrossPlatformInput;

public class MultiplayerControlScript : NetworkBehaviour
{
    [SyncVar]
    public string pname = "thePlayer";
    [SyncVar]
    public Color playerColor = Color.green;
    public Vector3 Rescaling = new Vector3(3, 3, 3);

    Transform[] SpawnPoints = null;

    //JoyStickCustomController joyStickController; 
    SmartARJoystick joyStickController;
    public float JoystickManualCalibrationAngle = 0;

    [Header("Player Attributes")]
    public float movingSpeed = 3f;
    public TagDatabase theTagReference;
    public float rotateSpeed = 150.0f;
    public bool Controllable = true;
    


    //MovementController theMoveController;
    MovementControllerNonVelocity theMoveController;

    bool isInvulnerable = false;

    HealthIndicator theHealth;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;



    public void OnGUI()
    {

        /*
        if (isLocalPlayer)
        {
            pname = GUI.TextField(new Rect(25, Screen.width - 40, 100, 30), pname);

            // CHANGE CONDITION
        }
        */
    }

   
    public void PlayerRespawn()
    {
        if (SpawnPoints == null)
            FindSpawnPoints();
        System.Random rnd = new System.Random();
        int randomNumber = rnd.Next(0, SpawnPoints.Length);

        transform.position = SpawnPoints[randomNumber].position;

    }


    private void FindSpawnPoints()
    {
        SpawnPoints = GameObject.Find("RespawnPoints").transform.GetComponentsInChildren<Transform>();

    }

    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }


        if (CrossPlatformInputManager.GetButtonDown("Refresh")){

            GetComponent<ARCheck>().OnRefreshPressed();
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

        Vector3 MVector = new Vector3();
        Vector3 targetVelocity = new Vector3();
        if (Controllable)
        {
            MVector = joyStickController.getAxisOutput();
            //Debug.LogWarning(MVector);
        }
        /*
        float x = CrossPlatformInputManager.GetAxis("Horizontal") * Time.deltaTime * movingSpeed;
        float z = CrossPlatformInputManager.GetAxis("Vertical") * Time.deltaTime * movingSpeed;
        */

        // Set horizontal velocity
        targetVelocity.x += movingSpeed * MVector.y* Time.deltaTime;
        targetVelocity.z += movingSpeed * MVector.x * Time.deltaTime;


        Vector3 JoystickMovement = targetVelocity;


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

		Physics.gravity = new Vector3(0,-50f,0);


        GameObject joyObject = GameObject.Find("JoyStick");
        joyStickController = (SmartARJoystick)joyObject.GetComponent<SmartARJoystick>();
        joyStickController.configureJoystick(false, JoystickManualCalibrationAngle);

        theTagReference = new TagDatabase();
        theTagReference.tagList = new Dictionary<string, int>();
        theTagReference.Initialize();

        FindSpawnPoints();


        Renderer[] rend = GetComponentsInChildren<Renderer>();
        foreach (Renderer R in rend)
        {
            R.material.color = playerColor;
        }

        transform.parent = GameObject.Find("Players").GetComponent<Transform>();
    }


    public override void OnStartLocalPlayer()
    {
        //transform.FindChild("Body").GetComponent<Renderer>().material.color = playerColor;


        //transform.parent = GameObject.Find("Players").GetComponent<Transform>();
        //transform.localScale = Rescaling;

        PlayerRespawn();
        //this.transform.position = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));



    }

    /*
    [ClientRpc]
    void RpcRespawn()
    */

    void OnTriggerEnter(Collider collision)
    {

        if (!isLocalPlayer)
            return;

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
                PlayerRespawn();
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
                CollectibleMultiplayer collect = (CollectibleMultiplayer)TheHitObject.GetComponent(typeof(CollectibleMultiplayer));
                if (collect.isPowerUp)
                {

                }

                TheHitObject.SetActive(false); // deactivate instead of destroy so you could later reactivate (respawn) him
                break;
        }

        TheHitObject = null;
    }



}




/// <summary>
/// Struct storing all samples of different colliding objects.
/// </summary>
public struct TagDatabase
{
    public Dictionary<string, int> tagList;

    /*
    public TagDatabase()
    {
        
        Initialize();
    }
    */

    public void Initialize()
    {
        //tagList = new Dictionary<string, int>();
        tagList.Add("Damaging", 1);
        tagList.Add("Death", 2);
        tagList.Add("Vulnerable", 3);
        tagList.Add("Goal", 4);
        tagList.Add("Collectable", 5);
        tagList.Add("Other", 0);
    }
}