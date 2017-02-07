using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControllerNonVelocity : MovementController {


    /// <summary>
    /// Get the position of the JoyStick and estimate the object displacement/Velocity.
    /// This child class focuses on direct traslation of the object instead of adding a certain velocity to the rigidoby.
    /// </summary>
    /// <param name="JoyStickTranslation">The movement of the JoyStick</param>
    public override void Move(Vector3 JoyStickTranslation)
    {

        //base.Move(JoyStickTranslation);
        if (JoyStickTranslation.x != 0 && JoyStickTranslation.z != 0)
        {
            EstimateFuturePosition(JoyStickTranslation);
            //Debug.LogWarning("The Current POSITION: " + OPosition + " ___ The estimated Position: " + Position);

            EstimateTrayectory();
            //CurrentVelocity = DetermineVelocity();
            getTranslationMagnitude(JoyStickTranslation);

            //Debug.LogWarning("The Current Trayectory: " + Trayectory + " ___ The velocity: " + CurrentVelocity);
            TranslateDistance();
            OrientObject();
            
        }

    }


    public override void Update()
    {

        OPosition = this.transform.position;

    }


    /// <summary>
    /// The object will be translated a certain amount depending on the MovementSpeed and the trayectory.
    /// </summary>
    protected void TranslateDistance()
    {
        transform.Translate(Trayectory.normalized * MovementSpeed * Time.deltaTime* InpulseModificator);
    }



    /// <summary>
    /// This child class method will rotate slightly the object towards the target direction.
    /// </summary>
    protected override void OrientObject()
    {
        PhysicalPlayer.transform.LookAt(OPosition, transform.up);

        Trayectory.y = 0; // keep the direction strictly horizontal
        Quaternion rot = Quaternion.LookRotation(Trayectory);
        //Debug.LogWarning("Original Rot: " + rot + "Target Rot:" + PhysicalPlayer.transform.rotation);
        // slerp to the desired rotation over time
        PhysicalPlayer.transform.rotation = Quaternion.Slerp(transform.rotation, rot, RotationSpeed * Time.deltaTime);
        //PhysicalPlayer.transform.rotation = rot;
        //Quaternion n = PhysicalPlayer.transform.rotation;
        //float differenceRotation = Quaternion.Angle(rot, n);


    }

}
