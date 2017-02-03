using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementController : MonoBehaviour {


    public float MovementSpeed = 2f;
    public float RotationSpeed = 100f;

    public float MaxSpeed = 100f;
    public float MaxRotationSpeed = 50f;

    protected Vector3 Movement;
    protected  Vector3 CurrentVelocity;
    protected Vector3 Trayectory;
    protected Vector3 Position;
    protected Vector3 OPosition;
    public GameObject PhysicalPlayer;
    protected Rigidbody thePhysics;

    public Vector3 DetermineVelocity()
    {

        Vector3 targetVelocity = CurrentVelocity + Trayectory * MovementSpeed * Time.deltaTime;

        if (targetVelocity.magnitude > MaxSpeed)
            targetVelocity = targetVelocity.normalized * MaxSpeed;


        return targetVelocity;
    }

    public virtual void Move(Vector3 JoyStickTranslation)
    {

        if (JoyStickTranslation.x != 0 && JoyStickTranslation.z != 0)
        {
            EstimateFuturePosition(JoyStickTranslation);
            Debug.LogWarning("The Current POSITION: " + OPosition + " ___ The estimated Position: " + Position);

            EstimateTrayectory();
            CurrentVelocity = DetermineVelocity();
            Debug.LogWarning("The Current Trayectory: " + Trayectory + " ___ The velocity: " + CurrentVelocity);

            OrientObject();
            thePhysics.velocity = CurrentVelocity;
        }

    }

    public void EstimateFuturePosition(Vector3 JoyStickTranslation)
    {
        Position = transform.position - JoyStickTranslation * 10;

    }


    public void EstimateTrayectory()
    {
        Trayectory = OPosition - Position; // Same as velocity?


    }

    public virtual void OrientObject()
    {
        PhysicalPlayer.transform.LookAt(OPosition, transform.up);
      
        Trayectory.y = 0; // keep the direction strictly horizontal
        Quaternion rot = Quaternion.LookRotation(Trayectory);
        Debug.LogWarning("Original Rot: " + rot + "Target Rot:" + PhysicalPlayer.transform.rotation);
        // slerp to the desired rotation over time
        PhysicalPlayer.transform.rotation = Quaternion.Slerp(transform.rotation, rot, RotationSpeed * Time.deltaTime);
        //PhysicalPlayer.transform.rotation = rot;
        Quaternion n = PhysicalPlayer.transform.rotation;
        float differenceRotation = Quaternion.Angle(rot ,n);
        

    }


    // Use this for initialization
    void Start () {
        Movement = new Vector3();
        CurrentVelocity = new Vector3();
        thePhysics = this.GetComponent<Rigidbody>();
        try
        {
            PhysicalPlayer = this.transform.FindChild("MeshContainer").gameObject;
        }
        catch(MissingReferenceException e)
        {
            Debug.LogWarning("No child \"MeshContainer\" found in PlayerObject. ");
        }
        
    }
	
	// Update is called once per frame
	public virtual void Update () {
        OPosition = this.transform.position;
        CurrentVelocity = thePhysics.velocity;
	}
}
