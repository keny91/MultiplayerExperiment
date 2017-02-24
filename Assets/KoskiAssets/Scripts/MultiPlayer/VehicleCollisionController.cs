using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCollisionController : CollisionControler
{


    MultiplayerControlScript thePlayer;
    MovementController thePlayerMovement;
    //GameControllerScript theController;
    GameObject LastObjectHit = null;


    void Start()
    {
        thePlayer = (MultiplayerControlScript)GetComponent<MultiplayerControlScript>();
        thePlayerMovement = GetComponent<MovementController>();

        collisionMaskGround.value = 1 << LayerMask.NameToLayer("Ground");
        collisionMaskObjects.value = 1 << LayerMask.NameToLayer("WorldObjects");


    }








    /// <summary>
    /// Check if the object should stop in the Y axis due to proximity with a world obstacle.
    /// <para>This child´s version will also launch the method "RayHitCheck" when ObjectHit is not Null</para>
    /// </summary>
    /// <seealso cref="RayHitCheck()"/>
    /// <param name="velocity">Reference to the moving velocity</param>
    public new void verticalCollisions(ref Vector3 velocity)
    {

        float directionY = Mathf.Sign(-1);
        float rayLength = Mathf.Abs(3) + skinWidth;

        // Depending on which direction we are moving 
        Vector3 rayOrigin = (directionY == -1) ? raycastOrigins.botLeftBack : raycastOrigins.topLeftBack;
        //rayOrigin += new Vector3(velocity.x, 0, velocity.z);    // CHECK THIS... MIGHT BE TROUBLE
        rayOrigin = new Vector3();
        // Create a rayCast plane below the player
        for (int i = 0; i < XRayCount; i++)
        {
            for (int j = 0; j < ZRayCount; j++)
            {
                Vector3 Origin = rayOrigin + Vector3.right * (XRaySpacing * i) + Vector3.forward * (ZRaySpacing * j);
                RaycastHit hit;
                //Ray ray = new Ray(rayOrigin, Vector3.up *directionY);
                bool isHit = Physics.Raycast(Origin, Vector3.up * directionY, out hit, rayLength, collisionMaskGround);

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


    




}
