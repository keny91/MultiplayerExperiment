using UnityEngine;
using System.Collections;


/// <summary>
/// Class made as a complement of Player.
/// Apart from the functionalities of CollisionController; this class contain the procedure to collions with other objects
/// </summary>
public class PlayerCollisionController : CollisionControler
{

    MultiplayerControlScript thePlayer;
    MovementController thePlayerMovement;
    //GameControllerScript theController;
    GameObject LastObjectHit = null;


    void Start()
    {
        thePlayer = (MultiplayerControlScript)GetComponent<MultiplayerControlScript>();
        thePlayerMovement = GetComponent<MovementController>();
    }

    /*
    public PlayerCollisionController()
    {
        theCollider = GetComponent<BoxCollider>();

    }

    public PlayerCollisionController(int Xdivs = 4, int Ydivs = 4, int Zdivs = 4)
    {
        theCollider = GetComponent<BoxCollider>();
        XRayCount = Xdivs;
        YRayCount = Ydivs;
        ZRayCount = Zdivs;
    }
 */

    /********************************************************/
    /*************      PLAYER COLLISIONS    ****************/
    /********************************************************/

     
        /*   
    /// <summary>
    /// Determine the nature of the object hit.
    /// <para> Execute only after checking the collision Object is not Null</para>
    /// </summary>
    void RayHitCheck(char orientation)
    {
        if(ObjectHit.collider != null)
        {

       
            GameObject TheHitObject = ObjectHit.collider.gameObject;
            int identifierTag = thePlayer.theTagReference.tagList[TheHitObject.tag];
            
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
                    thePlayer.TakeDamage(10);
                    break;
                //Encountered Death object
                case 2:
                    Debug.Log("Hit Death", TheHitObject);
                    //thePlayer.TakeDamage(100);
                    thePlayer.PlayerRespawn();

                    break;
                //Encountered Vulnerable object
                case 3:
                    Debug.Log("Hit Vulnerable", TheHitObject);
                    if(orientation == 'y')
                    {
                        Vector3 ProvisionalVector = new Vector3(0, 0, 0);
                        TheHitObject.transform.parent.gameObject.SetActive(false); // deactivate instead of destroy so you could later reactivate (respawn) him
                        ProvisionalVector = thePlayerMovement.velocity;
                        ProvisionalVector.y = thePlayerMovement.enemyDeathRepulsionVelY;
                        thePlayerMovement.velocity = ProvisionalVector;
                    }
                    //colliders.Add(collision.collider); // saves the enemy for later respawn

                    //myBody.AddForce(0, 130, 0);
                    break;
                //Encountered Goal object
                case 4:
                    Debug.Log("Hit Goal", TheHitObject);
                    break;
                //Encountered Collectible object
                case 5:
                    Debug.Log("Hit Collectible", TheHitObject);
                    CollectibleMultiplayer collect = (CollectibleMultiplayer)TheHitObject.GetComponent(typeof(CollectibleMultiplayer));
                    theController.collectibleCollected(collect);
                    TheHitObject.SetActive(false); // deactivate instead of destroy so you could later reactivate (respawn) him
                    break;
            }

            TheHitObject = null;
        }

        }

    */







    /// <summary>
    /// Check if the object should stop in the Y axis due to proximity with a world obstacle.
    /// <para>This child´s version will also launch the method "RayHitCheck" when ObjectHit is not Null</para>
    /// </summary>
    /// <seealso cref="RayHitCheck()"/>
    /// <param name="velocity">Reference to the moving velocity</param>
    public new void verticalCollisions(ref Vector3 velocity)
    {

        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        // Depending on which direction we are moving 
        Vector3 rayOrigin = (directionY == -1) ? raycastOrigins.botLeftBack : raycastOrigins.topLeftBack;
        rayOrigin += new Vector3(velocity.x, 0, velocity.z);    // CHECK THIS... MIGHT BE TROUBLE

        // Create a rayCast plane below the player
        for (int i = 0; i < XRayCount; i++)
        {
            for (int j = 0; j < ZRayCount; j++)
            {
                Vector3 Origin = rayOrigin + Vector3.right * (XRaySpacing * i) + Vector3.forward * (ZRaySpacing * j);
                RaycastHit hit;
                //Ray ray = new Ray(rayOrigin, Vector3.up *directionY);
                bool isHit = Physics.Raycast(Origin, Vector3.up * directionY, out hit, rayLength, collisionMaskGround);
                //Physics2D.racasç
                /*
                if (!hitObj)
                {
                    hitObj = Physics.Raycast(Origin, Vector3.up * directionY, out ObjectHit, objectCollisionLenght, collisionMaskObjects);
                    RayHitCheck('y');
                }
                    */


                //Debug Display
                //Debug.DrawRay(Origin+Vector3.up*velocity.y, Vector3.up * (rayLength*directionY), Color.black);  // (Lenght, direction, color)

                //Debug.DrawRay(playerCollider.raycastOrigins.botLeftBack + Vector3.right * playerCollider.XRaySpacing * i + Vector3.forward * playerCollider.ZRaySpacing * j + Vector3.up*(velocity.y), Vector3.up * (velocity.y), Color.black);  // (Lenght, direction, color)

                if (isHit)
                {
                    //Debug.Log("Y axis collision detected");
                    velocity.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;   // this line prevent from having multiple

                    above = directionY == 1;
                    below = directionY == -1;
                }
                Debug.DrawRay(Origin + Vector3.up * velocity.y, Vector3.up * (4 * directionY), Color.green);  // Fixed lemgth version

            }
        }
    }




    /// <summary>
    /// Check if the object should stop in the XZ axis due to proximity with a world obstacle.
    /// We will modify both axis at the same time.
    /// <para>This child´s version will also launch the method "RayHitCheck" when ObjectHit is not Null</para>
    /// </summary>
    /// <seealso cref="RayHitCheck()"/>
    /// <param name="velocity">Reference to the moving velocity</param>
    public new void horizontalCollisionsInAxisX(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;


        // Depending on which direction we are moving 
        Vector3 rayOrigin = (directionX == -1 || directionX == 0) ? raycastOrigins.botLeftBack : raycastOrigins.botLeftFront;
        rayOrigin += new Vector3(0, velocity.y, velocity.z);    // CHECK THIS... MIGHT BE TROUBLE


        // Create a rayCast plane below the player
        for (int i = 0; i < ZRayCount; i++)
        {
            for (int j = 0; j < YRayCount; j++)
            {
                Vector3 Origin = rayOrigin + Vector3.forward * (ZRaySpacing * i) + Vector3.up * (YRaySpacing * j);
                //Vector3 Origin = rayOrigin + Vector3.right * (playerCollider.XRaySpacing * i) + Vector3.forward * (playerCollider.ZRaySpacing * j);
                RaycastHit hit;
                //Ray ray = new Ray(rayOrigin, Vector3.up *directionY);
                
                bool isHit = Physics.Raycast(Origin, Vector3.right * directionX, out hit, rayLength, collisionMaskGround);
                /*
                if (!hitObj)
                {
                    hitObj = Physics.Raycast(Origin, Vector3.up * directionX, out ObjectHit, objectCollisionLenght, collisionMaskObjects);
                    RayHitCheck('x');
                }
                 */
                //Physics2D.racas



                if (isHit)
                {
                    //Debug.Log("X axis collision detected");
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;   // this line prevent from having multiple
                    right = directionX == 1;
                    left = directionX == -1;
                }

                //Debug Display
                //Debug.DrawRay(Origin+Vector3.up*velocity.y, Vector3.up * (rayLength*directionY), Color.black);  // (Lenght, direction, color)
                //Debug.DrawRay(Origin + Vector3.up * velocity.y, Vector3.up * (4 * directionX), Color.black);  // Fixed lemgth version
                Debug.DrawRay(Origin + Vector3.up * velocity.x, Vector3.right * (4) * directionX, Color.blue);
                //Debug.DrawRay(playerCollider.raycastOrigins.botLeftBack + Vector3.right * playerCollider.XRaySpacing * i + Vector3.forward * playerCollider.ZRaySpacing * j + Vector3.up*(velocity.y), Vector3.up * (velocity.y), Color.black);  // (Lenght, direction, color)

            }
        }
    }

    /// <summary>
    /// Check if the object should stop in the XZ axis due to proximity with a world obstacle.
    /// We will modify both axis at the same time.
    /// <para>This child´s version will also launch the method "RayHitCheck" when ObjectHit is not Null</para>
    /// </summary>
    /// <seealso cref="RayHitCheck()"/>
    /// <param name="velocity">Reference to the moving velocity</param>
    public new void horizontalCollisionsInAxisZ(ref Vector3 velocity)
    {
        float directionZ = Mathf.Sign(velocity.z);
        float rayLength = Mathf.Abs(velocity.z) + skinWidth;


        // Depending on which direction we are moving 
        Vector3 rayOrigin = (directionZ == -1 || directionZ == 0) ? raycastOrigins.botLeftBack : raycastOrigins.botRightBack;
        rayOrigin += new Vector3(velocity.x, velocity.y, 0);    // CHECK THIS... MIGHT BE TROUBLE

        // Create a rayCast plane below the player
        for (int i = 0; i < XRayCount; i++)
        {
            for (int j = 0; j < YRayCount; j++)
            {
                Vector3 Origin = rayOrigin + Vector3.right * (XRaySpacing * i) + Vector3.up * (YRaySpacing * j);
                //Vector3 Origin = rayOrigin + Vector3.right * (playerCollider.XRaySpacing * i) + Vector3.forward * (playerCollider.ZRaySpacing * j);
                RaycastHit hit;
                //Ray ray = new Ray(rayOrigin, Vector3.up *directionY);
                bool isHit = Physics.Raycast(Origin, Vector3.forward * directionZ, out hit, rayLength, collisionMaskGround);
                //Physics2D.racas
                /*
                if (!hitObj)
                {
                    hitObj = Physics.Raycast(Origin, Vector3.up * directionZ, out ObjectHit, objectCollisionLenght, collisionMaskObjects);
                    RayHitCheck('z');
                }
                  */  


                if (isHit)
                {
                    velocity.z = (hit.distance - skinWidth) * directionZ;
                    rayLength = hit.distance;   // this line prevent from having multiple
                    front = directionZ == 1;
                    back = directionZ == -1;
                }
                Debug.DrawRay(Origin + Vector3.up * velocity.z, Vector3.forward * (4) * directionZ, Color.red);
            }
        }
    }





}
