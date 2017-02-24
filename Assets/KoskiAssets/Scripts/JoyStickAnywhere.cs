using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class TrackedTouch
{
    public Vector2 startPos;
    public Vector2 currentPos;
}

public class JoyStickAnywhere : MonoBehaviour, IPointerClickHandler
{

    public Canvas theCanvas;
    public GameObject theJoystickPrefab;
    public SmartARJoystick theJoyStickController;


    public void OnPointerClick(PointerEventData ED)
    {
        Vector3 localHit = transform.InverseTransformPoint(ED.pressPosition);
        Debug.LogError(localHit);

        /*
        int x = ((int)((localHit.x + 62.5f) / 25f));

        int y = 4 - (int)((localHit.y + 58.5f) / 23.4);

        if (x < 0 || x > 4 || y < 0 || y > 4)
        {
            //out of bound
            return;
        }
        
         */
    }


    /// <summary>
    /// Create the joystick prefab in the position
    /// </summary>
    /// <param name="touchPosition"></param>
    public void CreateJoystickAt(Vector3 touchPosition)
    {

        //Vector3 rot = new Vector3(0, 0, 0);

        //Instanciate Prefab
        //GameObject bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        GameObject theObject = (GameObject)Instantiate(theJoystickPrefab, touchPosition, new Quaternion());

        //Get Joystick Custom Controller
        try
        {
            theJoyStickController = theObject.GetComponent<SmartARJoystick>();
        }
         catch (MissingReferenceException e)
        {
            Debug.LogError("No SmartARJoystick attached to JoyStick");
            Application.Quit();
        }
    }

   

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
   public Vector3 GetTouchPosition()
    {
        return new Vector3();
    }




    /// <summary>
    /// Eliminate the previous instance of the JoyStick Controller.
    /// </summary>
    public void DestroyJoystick() {

        theJoyStickController.Disable();  // Unregister axis inputs
        Destroy(theJoyStickController.transform); // Destroy the object
    }

}