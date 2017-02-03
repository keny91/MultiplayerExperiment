using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControllerNonVelocity : MovementController {



    public override void Move(Vector3 JoyStickTranslation)
    {

        //base.Move(JoyStickTranslation);
        if (JoyStickTranslation.x != 0 && JoyStickTranslation.z != 0)
        {
            EstimateFuturePosition(JoyStickTranslation);
            Debug.LogWarning("The Current POSITION: " + OPosition + " ___ The estimated Position: " + Position);

            EstimateTrayectory();
            //CurrentVelocity = DetermineVelocity();


            Debug.LogWarning("The Current Trayectory: " + Trayectory + " ___ The velocity: " + CurrentVelocity);
            TranslateDistance();
            OrientObject();
            
        }

    }


    public override void Update()
    {

        OPosition = this.transform.position;

    }

    private void TranslateDistance()
    {
        transform.Translate(Trayectory.normalized * MovementSpeed * Time.deltaTime);
    }


    public override void OrientObject()
    {
        PhysicalPlayer.transform.LookAt(OPosition, transform.up);

        Trayectory.y = 0; // keep the direction strictly horizontal
        Quaternion rot = Quaternion.LookRotation(Trayectory);
        Debug.LogWarning("Original Rot: " + rot + "Target Rot:" + PhysicalPlayer.transform.rotation);
        // slerp to the desired rotation over time
        PhysicalPlayer.transform.rotation = Quaternion.Slerp(transform.rotation, rot, RotationSpeed * Time.deltaTime);
        //PhysicalPlayer.transform.rotation = rot;
        //Quaternion n = PhysicalPlayer.transform.rotation;
        //float differenceRotation = Quaternion.Angle(rot, n);


    }

}
