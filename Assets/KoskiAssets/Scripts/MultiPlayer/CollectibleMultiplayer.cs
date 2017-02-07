using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CollectibleMultiplayer : NetworkBehaviour
{

    public int value = 1;
    public bool isPowerUp = false;
    public bool requiredToPassLevel = true;
    public int powerUpType = 0;
    public bool autoRotate = true;


    public int rotationAmount = 90;
    // hover too?

    //GameControllerScript theController;

    // Use this for initialization
    void Start()
    {
        //theController = GameObject.Find("Player").GetComponent<GameControllerScript>();
        /* if (GameObject.Find("GameControl").GetComponent<GameControllerScript>())
         {
             theController = GameObject.Find("GameControl").GetComponent<GameControllerScript>();
             //Debug.LogError("COIN INITTTTTTTT");
         }

     */
    }

    /// <summary>
    /// Trigger event when the Collectible has been collected
    /// </summary>
    /// <param name="theP"></param>
    public virtual void OnCollected(MultiplayerControlScript theP)
    {

    }



    // Update is called once per frame
    void Update()
    {

        if (autoRotate)
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.y = rot.y + rotationAmount * Time.deltaTime;
            if (rot.y > 360)
                rot.y -= 360;
            else if (rot.y < 360)
                rot.y += 360;

            transform.eulerAngles = rot;
        }
    }
}
