using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(HealthIndicator))]


public class MultiplayerControlScript : NetworkBehaviour
{
    [SyncVar]
    public string pname = "thePlayer";
    [SyncVar]
    public Color playerColor = Color.green;
    

    Transform[] SpawnPoints = null;
    public float InvulnerableTime = 2f;

    //JoyStickCustomController joyStickController; 



    public TagDatabase theTagReference;


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

   /// <summary>
   /// Respawn the player given the preset set of points in the Scene
   /// </summary>
    public void PlayerRespawn()
    {
        if (isLocalPlayer)
        {
            if (SpawnPoints == null)
                FindSpawnPoints();
            System.Random rnd = new System.Random();
            int randomNumber = rnd.Next(0, SpawnPoints.Length);

            transform.position = SpawnPoints[randomNumber].position;
            GetComponent<Rigidbody>().velocity = new Vector3();
        }


    }


    /// <summary>
    /// Find and set as possible respawn the set of points in the respanw object.
    /// </summary>
    private void FindSpawnPoints()
    {
        SpawnPoints = GameObject.Find("RespawnPoints").transform.GetComponentsInChildren<Transform>();

    }

    // Update is called once per frame
   public void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }

        /*
        if (CrossPlatformInputManager.GetButtonDown("Refresh")){

            GetComponent<ARCheck>().OnRefreshPressed();
        }
        */




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
        isInvulnerable = true;
        Invoke("ResetInvulnerability", InvulnerableTime);


    }



    public void TakeDamage(int dmg)
    {
        if (isLocalPlayer && !isInvulnerable)
        {
            isInvulnerable = true;
            Invoke("ResetInvulnerability", InvulnerableTime);
            theHealth.TakeDamage(dmg);
        }
        
    }

    /// <summary>
    /// Set the player to vulnerable
    /// </summary>
    void ResetInvulnerability()
    {
        isInvulnerable = false;
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
                    TakeDamage(10);
                    //theController.theSoundController.playClip(theController.theSoundController.SoundPlayerHit);
                    Vector3 colDir = collision.transform.position - gameObject.transform.FindChild("Origin").transform.position;

                    //AdditionalMath.EstimateHorizontalRepulsion(colDir, ref velocity, HorizontalPushByEnemyDmg);
                    //AdditionalMath.AddVerticalRepulsion(ref velocity, VerticalPushByEnemyDmg);
                }


                break;
            //Encountered Death object
            case 2:
                Debug.Log("Hit Death", TheHitObject);
                //TakeDamage(theHealth.currentHealth);
                PlayerRespawn();
                
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


/*

/// <summary>
/// Struct storing all samples of different colliding objects.
/// </summary>
public struct TagDatabase
{
    public Dictionary<string, int> tagList;




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
*/