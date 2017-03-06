using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class MovementController : NetworkBehaviour {

    [Header("Movement Parameters")]
    public float MovementSpeed = 2f;
    public float RotationSpeed = 100f;

    public float MaxSpeed = 100f;
    public float MaxRotationSpeed = 50f;
    public Vector3 velocity = new Vector3();
    public bool isControllable = true;


    [Header("Object Internal Attributes")]
    protected Vector3 Movement;
    protected Vector3 CurrentVelocity;
    protected Vector3 Trayectory;
    protected Vector3 Position;
    protected Vector3 OPosition;

   // [Header("Object Internal Attributes")]

    public Transform PhysicalPlayer;
    protected Rigidbody thePhysicalObject;
    protected float InpulseModificator;


    /// <summary>
    /// Find out the physical velocity which the Rigidbody is moving.
    /// </summary>
    /// <returns>The estimated object velocity</returns>
    protected virtual Vector3 DetermineVelocity()
    {

        Vector3 targetVelocity = CurrentVelocity + Trayectory * MovementSpeed * Time.deltaTime;

        if (targetVelocity.magnitude > MaxSpeed)
            targetVelocity = targetVelocity.normalized * MaxSpeed * InpulseModificator;


        return targetVelocity;
    }


    /// <summary>
    /// Get the position of the JoyStick and estimate the object displacement/Velocity.
    /// </summary>
    /// <param name="JoyStickTranslation">The movement of the JoyStick</param>
    public virtual void Move(Vector3 JoyStickTranslation)
    {

        if (JoyStickTranslation.x != 0 && JoyStickTranslation.z != 0)
        {
            EstimateFuturePosition(JoyStickTranslation);
            //Debug.LogWarning("The Current POSITION: " + OPosition + " ___ The estimated Position: " + Position);

            EstimateTrayectory();
            getTranslationMagnitude(JoyStickTranslation);
            CurrentVelocity = DetermineVelocity();
            //Debug.LogWarning("The Current Trayectory: " + Trayectory + " ___ The velocity: " + CurrentVelocity);

            OrientObject();
            thePhysicalObject.velocity = CurrentVelocity;
        }

    }






    /// <summary>
    /// Set the speed modificator depending on the modulus of the joystick.
    /// </summary>
    /// <param name="JoyStickTranslation">The joystick received input</param>
    protected void getTranslationMagnitude(Vector3 JoyStickTranslation)
    {
        JoyStickTranslation.y = 0; // Make sure Y remains as 0.
        InpulseModificator = JoyStickTranslation.magnitude;
    }


    /// <summary>
    /// Based on the Joystick position guess where the new position would be.
    /// Required for trayectory and collision estimations.
    /// </summary>
    /// <param name="JoyStickTranslation"> The Joystick Axis input</param>
    protected virtual void EstimateFuturePosition(Vector3 JoyStickTranslation)
    {
        Position = transform.position - JoyStickTranslation * 10;
    }



    /// <summary>
    /// Based on a future position get the vector estimating the trayectory.
    /// </summary>
    protected virtual void EstimateTrayectory()
    {
        Trayectory = OPosition - Position; // Same as velocity?
    }

    /// <summary>
    /// Based on a trayectory, change the Physical body of the object to face the new direction.
    /// </summary>
    protected virtual void OrientObject()
    {
        PhysicalPlayer.transform.LookAt(OPosition, transform.up);
      
        Trayectory.y = 0; // keep the direction strictly horizontal
        Quaternion rot = Quaternion.LookRotation(Trayectory);
        Debug.LogWarning("Original Rot: " + rot + "Target Rot:" + PhysicalPlayer.rotation);
        // slerp to the desired rotation over time
        PhysicalPlayer.rotation = Quaternion.Slerp(transform.rotation, rot, RotationSpeed * Time.deltaTime);
        //PhysicalPlayer.transform.rotation = rot;
        Quaternion n = PhysicalPlayer.rotation;
        float differenceRotation = Quaternion.Angle(rot ,n);
        

    }


    // Use this for initialization
    void Start () {


        Movement = new Vector3();
        CurrentVelocity = new Vector3();
        thePhysicalObject = this.GetComponent<Rigidbody>();
        try
        {
            PhysicalPlayer = this.transform.FindChild("MeshContainer").gameObject.transform;
        }
        catch(MissingReferenceException e)
        {
            Debug.LogWarning("No child \"MeshContainer\" found in PlayerObject. "+ e.Message);
        }
        
    }





    // Update is called once per frame
    public virtual void Update () {
        OPosition = this.transform.position;
        velocity = thePhysicalObject.velocity;
        Vector3 targetVelocity = new Vector3();





        Move(velocity * Time.deltaTime);

    }
}
