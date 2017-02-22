using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionControler : MonoBehaviour
{

    public float skinWidth = .015f;  // How thick our collider frame will be

    public BoxCollider theCollider;
    public RaycastOrigins raycastOrigins;

    public int XRayCount = 4;
    public int YRayCount = 4;
    public int ZRayCount = 4;

    public RaycastHit ObjectHit;

    protected float XRaySpacing;
    protected float YRaySpacing;
    protected float ZRaySpacing;

    protected float objectCollisionLenght = 1.2f;

    // Collision Status
    [HideInInspector]public bool above, below;
    [HideInInspector]
    public bool front, back;
    [HideInInspector]
    public bool left, right;
    [HideInInspector]
    public bool climbingSlope = false;


    // Layers
    public LayerMask collisionMaskGround;  // Terrain
    public LayerMask collisionMaskObjects;  // World Objects


    /// <summary>
    /// Set all collision indicator back to false.
    /// </summary>
        public void reset(){
            above = false; below = false;
            front = false; back = false;
            left = false; right = false;
            climbingSlope = false;
    }

    


    /// <summary>
    /// Constructor
    /// </summary>
     public void Init(int Xdivs = 4, int Ydivs = 4, int Zdivs = 4)
    {
        theCollider = GetComponent<BoxCollider>();
        XRayCount = Xdivs;
        YRayCount = Ydivs;
        ZRayCount = Zdivs;

    }

    /*
    public CollisionControler(int Xdivs=4 , int Ydivs = 4, int Zdivs = 4)
    {
        theCollider = GetComponent<BoxCollider>();
        XRayCount = Xdivs;
        YRayCount = Ydivs;
        ZRayCount = Zdivs;
    }
    */

    /// <summary>
    /// Struct to store the point positions required for the rayCasts.
    /// Each of the corners of a 3D box
    /// </summary>
    public struct RaycastOrigins
    {
        public Vector3 topLeftBack, topLeftFront;
        public Vector3 botLeftBack, botLeftFront;
        public Vector3 topRightBack, topRightFront;
        public Vector3 botRightBack, botRightFront;
    }



    /// <summary>
    /// <para>back-front  => X axis </para>
    /// <para>bot-top  => Y axis </para>
    /// <para>left-right  => Z axis </para>
    /// </summary>
    public void UpdateRaycastOrigins()
    {
        Bounds bounds = theCollider.bounds;
        bounds.Expand(skinWidth * -2);
        raycastOrigins.botLeftBack = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
        raycastOrigins.botRightBack = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);

        raycastOrigins.botLeftFront = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        raycastOrigins.botRightFront = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);

        raycastOrigins.topLeftBack = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        raycastOrigins.topRightBack = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);

        raycastOrigins.topLeftFront = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        raycastOrigins.topRightFront = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);
    }


    /// <summary>
    /// Self ajust the distance between the raycast in the figure accordint to the number 
    /// of rays casted in each dimension.
    /// </summary>
    public void CalculateRaySpacing()
    {
        Bounds bounds = theCollider.bounds;
        bounds.Expand(skinWidth * -2);

        XRayCount = Mathf.Clamp(XRayCount, 2, int.MaxValue);
        YRayCount = Mathf.Clamp(YRayCount, 2, int.MaxValue);
        ZRayCount = Mathf.Clamp(ZRayCount, 2, int.MaxValue);

        XRaySpacing = bounds.size.x / (XRayCount - 1);
        YRaySpacing = bounds.size.y / (YRayCount - 1);
        ZRaySpacing = bounds.size.z / (ZRayCount - 1);
    }





    /// <summary>
    /// Check if the object should stop in the Y axis due to proximity with a world obstacle.
    /// </summary>
    /// <param name="velocity">Reference to the moving velocity</param>
    public void verticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        // Depending on which direction we are moving 
        Vector3 rayOrigin = (directionY == -1) ? raycastOrigins.botLeftBack : raycastOrigins.topLeftBack;
        rayOrigin += new Vector3(velocity.x, 0, velocity.z);    // CHECK THIS... MIGHT BE TROUBLE
        bool hitObj = false;

        // Create a rayCast plane below the player
        for (int i = 0; i < XRayCount; i++)
        {
            for (int j = 0; j < ZRayCount; j++)
            {
                Vector3 Origin = rayOrigin + Vector3.right * (XRaySpacing * i) + Vector3.forward * (ZRaySpacing * j);
                RaycastHit hit;
                //Ray ray = new Ray(rayOrigin, Vector3.up *directionY);
                bool isHit = Physics.Raycast(Origin, Vector3.up * directionY, out hit, rayLength, collisionMaskGround);
                //Physics2D.racas
                if(!hitObj)
                    hitObj = Physics.Raycast(Origin, Vector3.up * directionY, out ObjectHit, 1, collisionMaskObjects);
                

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
    /// </summary>
    /// <param name="velocity">Reference to the moving velocity</param>
    public void horizontalCollisionsInAxisX(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;


        // Depending on which direction we are moving 
        Vector3 rayOrigin = (directionX == -1 || directionX == 0) ? raycastOrigins.botLeftBack : raycastOrigins.botLeftFront;
        rayOrigin += new Vector3(0, velocity.y, velocity.z);    // CHECK THIS... MIGHT BE TROUBLE
        bool hitObj = false;

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
                if (!hitObj)
                    hitObj = Physics.Raycast(Origin, Vector3.up * directionX, out ObjectHit, 1, collisionMaskObjects);
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
                Debug.DrawRay(Origin + Vector3.up * velocity.x, Vector3.right * (4)*directionX, Color.blue);
                //Debug.DrawRay(playerCollider.raycastOrigins.botLeftBack + Vector3.right * playerCollider.XRaySpacing * i + Vector3.forward * playerCollider.ZRaySpacing * j + Vector3.up*(velocity.y), Vector3.up * (velocity.y), Color.black);  // (Lenght, direction, color)

            }
        }
    }





    /// <summary>
    /// Check if the object should stop in the XZ axis due to proximity with a world obstacle.
    /// We will modify both axis at the same time.
    /// </summary>
    /// <param name="velocity">Reference to the moving velocity</param>
    public void horizontalCollisionsInAxisZ(ref Vector3 velocity)
    {
        float directionZ = Mathf.Sign(velocity.z);
        float rayLength = Mathf.Abs(velocity.z) + skinWidth;


        // Depending on which direction we are moving 
        Vector3 rayOrigin = (directionZ == -1 || directionZ == 0) ? raycastOrigins.botLeftBack : raycastOrigins.botRightBack;
        rayOrigin += new Vector3(velocity.x, velocity.y, 0);    // CHECK THIS... MIGHT BE TROUBLE
        bool hitObj = false;

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
                if (!hitObj)
                    hitObj = Physics.Raycast(Origin, Vector3.up * directionZ, out ObjectHit, 1, collisionMaskObjects);


                if (isHit)
                {
                    velocity.z = (hit.distance - skinWidth) * directionZ;
                    rayLength = hit.distance;   // this line prevent from having multiple
                    front = directionZ == 1;
                    back = directionZ == -1;
                }
                //Debug Display
                //Debug.DrawRay(Origin + Vector3.up * velocity.y, Vector3.up * (4 * directionX), Color.black);  // Fixed lemgth version
                Debug.DrawRay(Origin + Vector3.up * velocity.z, Vector3.forward * (4) *directionZ, Color.red);
                //Debug.DrawRay(playerCollider.raycastOrigins.botLeftBack + Vector3.right * playerCollider.XRaySpacing * i + Vector3.forward * playerCollider.ZRaySpacing * j + Vector3.up*(velocity.y), Vector3.up * (velocity.y), Color.black);  // (Lenght, direction, color)

            }
        }
    }





    /// <summary>
    /// If in contact with surfaces, reset its specific axis velocity to zero
    /// </summary>
    /// <param name="velocity">Reference to the moving velocity</param>
    public void SpeedLimitByCollision(ref Vector3 velocity)
    {
        if (velocity.y != 0 && (above || below))
            velocity.y = 0;
        // ADD X && Z axis?

        if (velocity.x != 0 && (right || left))
            velocity.x = 0;

        if (velocity.z != 0 && (front || back))
            velocity.z = 0;


    }

}




public struct VelocityLimit
{
    public float xVelocityLimit, zVelocityLimit, FallingVelocityLimit, RisingVelocityLimit;
    public VelocityLimit(float xSpeedMovementLimit, float zSpeedMovementLimit, float FallingSpeedLimit, float RisingSpeedLimit)
    {
        xVelocityLimit = xSpeedMovementLimit;
        zVelocityLimit = zSpeedMovementLimit;
        FallingVelocityLimit = FallingSpeedLimit;
        RisingVelocityLimit = RisingSpeedLimit;
    }



    /// <summary>
    /// If the object is moving to fast, the axial velocity will be set to a limit.
    /// <para>This process comes before any translation movement</para>
    /// </summary>
    /// <param name="velocity">Input reference to the speed vector under evaluation.</param>
    public void LimitVelocityCheck(ref Vector3 velocity)
    {

        if (velocity.x != 0)
        {
            if (Mathf.Abs(velocity.x) > xVelocityLimit)
                velocity.x = Mathf.Sign(velocity.x) * xVelocityLimit;
        }

        if (velocity.z != 0)
        {
            if (Mathf.Abs(velocity.z) > zVelocityLimit)
                velocity.z = Mathf.Sign(velocity.z) * zVelocityLimit;
        }

        // two different statements for falling and rising
        if (velocity.y != 0)
        {
            if (velocity.y > RisingVelocityLimit)
            {
                velocity.y = RisingVelocityLimit;
                
            }
                
            else if (velocity.y < FallingVelocityLimit)
            {
                velocity.y = FallingVelocityLimit;
            }
                
        }


    }

}

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