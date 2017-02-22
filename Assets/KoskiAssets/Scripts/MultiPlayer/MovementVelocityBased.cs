using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(VehicleCollisionController))]
[RequireComponent(typeof(BoxCollider))]

public class MovementVelocityBased : MovementController
{

    private const float xVelocityLimit = 20f;
    private const float zVelocityLimit = 20f;
    private const float FallingVelocityLimit = -50f;
    private const float RisingVelocityLimit = 20f;



    [Header("Player Attributes")]
    //public float MovementSpeed = 2f;
    //public float RotationSpeed = 100f;
    public float FallingVelocity = 10f;
    

    VehicleCollisionController playerCollider;
    VelocityLimit VelocityLimits;
    SmartARJoystick joyStickController;
    public float JoystickManualCalibrationAngle = 0;

    /// <summary>
    /// Do not allow the object to rotate more than a certain amount of degrees.
    /// </summary>
    /// <param name="rot"></param>
    /// <returns></returns>
    public Quaternion EnforceTransformRotationConstrains(Quaternion rot)
    {

        return new Quaternion();
    }



    /// <summary>
    /// Specify movements (start/target) and specify the axis in which are, so they can be smoothed
    /// </summary>
    /// <param name="velocity">Current velocity</param>
    /// <param name="targetVelocity">Velocity we aim to achieve</param>
    public void VelocityTransition(ref Vector3 velocity, Vector3 targetVelocity, float SmoothMod)
    {

        SmoothMod = 0.7f;
        float velocitysmoothing = 0.0F;
        velocity.z = Mathf.SmoothDamp(velocity.z, targetVelocity.z, ref velocitysmoothing, SmoothMod);
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity.x, ref velocitysmoothing, SmoothMod);
        Debug.LogWarning("Final Velocity: "+ velocity);

    }


    /// <summary>
    /// Based on the Joystick position guess where the new position would be.
    /// Required for trayectory and collision estimations.
    /// </summary>
    /// <param name="JoyStickTranslation"> The Joystick Axis input</param>
    protected override void EstimateFuturePosition(Vector3 JoyStickTranslation)
    {
    //    Debug.LogWarning("Joystick: " + JoyStickTranslation);
        Position = OPosition - JoyStickTranslation * 10; 
//Debug.LogWarning("POSITION: "+Position);
    }

    /// <summary>
    /// Find out the physical velocity which the Rigidbody is moving.
    /// </summary>
    /// <returns>The estimated object velocity</returns>
    protected override Vector3 DetermineVelocity()
    {

        Vector3 targetVelocity = velocity + Trayectory * MovementSpeed * Time.deltaTime;

        if (targetVelocity.magnitude > MaxSpeed)
            targetVelocity = targetVelocity.normalized * MaxSpeed * InpulseModificator;

        return targetVelocity;
    }


    // Use this for initialization
    void Start () {

        playerCollider = (VehicleCollisionController)GetComponent<VehicleCollisionController>();
        playerCollider.Init(4, 6, 4);  // 6 vertical divisions
        playerCollider.collisionMaskGround.value = 1 << LayerMask.NameToLayer("Ground");
        playerCollider.collisionMaskObjects.value = 1 << LayerMask.NameToLayer("WorldObjects");

        Physics.gravity = new Vector3(0, -100f, 0);  // Set gravity to 0. We will ajustfalling speed manually         
        thePhysicalObject = this.GetComponent<Rigidbody>();

        try{
            PhysicalPlayer = this.transform.FindChild("MeshContainer").gameObject.transform;
        }
        catch (MissingReferenceException e)
        {
            Debug.LogWarning("No child \"MeshContainer\" found in PlayerObject. " + e.Message);
        }



        GameObject joyObject = GameObject.Find("JoyStick");
        joyStickController = (SmartARJoystick)joyObject.GetComponent<SmartARJoystick>();
        joyStickController.configureJoystick(false, JoystickManualCalibrationAngle);

        // Set Up velocity limit for the player
        VelocityLimits = new VelocityLimit(xVelocityLimit, zVelocityLimit, FallingVelocityLimit, RisingVelocityLimit);

        //Determine the box of the player and the ray colliders
        playerCollider.CalculateRaySpacing();

        Movement = new Vector3();
        CurrentVelocity = new Vector3();
        

        try
        {
            PhysicalPlayer = this.transform.FindChild("MeshContainer").gameObject.transform;
        }
        catch (MissingReferenceException e)
        {
            Debug.LogWarning("No child \"MeshContainer\" found in PlayerObject. " + e.Message);
        }

    }

    bool CheckControllable()
    {
        if (CheckGrounded())
        {
            isControllable = true;
            return true;
        }
            
        else
        {
            isControllable = false;
            Debug.LogError("UNCONTROLLABLE FALSE");
            return false;
        }
             

    }


    bool CheckGrounded()
    {
        playerCollider.UpdateRaycastOrigins();
        playerCollider.reset();
        playerCollider.verticalCollisions(ref velocity);

        return playerCollider.below;
    }




	// Update is called once per frame
	public override void Update () {

        if (isLocalPlayer) {

            Vector3 MVector, prov = new Vector3();
            velocity = thePhysicalObject.velocity;
            OPosition = transform.position;

            //velocity = 
            //velocity.y -= FallingVelocity * Time.deltaTime;
            

            if (CheckControllable())
            {
                MVector = joyStickController.getAxisOutput();
                prov.x = -MVector.x;
                prov.z = MVector.y;
                //playerCollider.below;
                //Debug.Log("after collision, Vertical speed is : " + velocity.y);
            }

            EstimateFuturePosition(prov);
            EstimateTrayectory(); // Add velocity to the equ

            // Set horizontal velocity
            //targetVelocity.x = MovementSpeed * MVector.x * Time.deltaTime + velocity.x;
            //targetVelocity.z = MovementSpeed * MVector.z * Time.deltaTime + velocity.z;

            //Position = transform.position - MVector * 10;


            //Vector3 JoystickMovement = targetVelocity;
            Move(prov);

        }
    }


    /// <summary>
    /// Based on a future position get the vector estimating the trayectory.
    /// </summary>
    protected override void EstimateTrayectory()
    {
        Trayectory = Position - OPosition; // Same as velocity?
    }


    /// <summary>
    /// This child class method will rotate slightly the object towards the target direction.
    /// </summary>
    /// 

    protected override void OrientObject()
    {
        //PhysicalPlayer.LookAt(velocity, transform.up);
        transform.LookAt(Position, transform.up);

        //velocity.y = 0; // keep the direction strictly horizontal
        //Quaternion rot = Quaternion.LookRotation(Trayectory);
        //Debug.LogWarning("Original Rot: " + rot + "Target Rot:" + PhysicalPlayer.transform.rotation);
        // slerp to the desired rotation over time
       // transform.rotation.eulerAngles = rot;
        //transform.rotation = Quaternion.Slerp(transform.rotation, rot, RotationSpeed*10 );
        //PhysicalPlayer.rotation = Quaternion.Slerp(transform.rotation, rot, RotationSpeed );
        //PhysicalPlayer.transform.rotation = rot;
        //transform.rotation = rot;
        //Quaternion n = PhysicalPlayer.transform.rotation;
        //float differenceRotation = Quaternion.Angle(rot, n);
    }



    public override void Move(Vector3 JoyStickTranslation)
    {
        Vector3 targetVelocity = new Vector3();
        Vector3 prov = velocity;
        prov.x = JoyStickTranslation.x;
        prov.z = JoyStickTranslation.z;
        float SmoothMod = 0.25f;

        thePhysicalObject.velocity = prov;
        velocity = prov;
        //transform.Translate(velocity);

        targetVelocity = DetermineVelocity();
        //VelocityTransition(ref velocity, targetVelocity, SmoothMod);
        //VelocityLimit();

        //VelocityLimits.LimitVelocityCheck(ref velocity);
        OrientObject();
        thePhysicalObject.velocity = targetVelocity;
    }



}
