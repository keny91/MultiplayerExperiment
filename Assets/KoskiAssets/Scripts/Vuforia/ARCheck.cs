using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ARCheck : MonoBehaviour {

    public ImageTargetBehaviour theTracker;
    public GameObject Cam;

	


    void FindCamera()
    {
        Cam = GameObject.Find("ARCamera");
    }


    /// <summary>
    /// Identify and get the Vuforia trackable component in the scene.
    /// </summary>
    void FindTracker()
    {
        GameObject[] trackers;
        trackers = GameObject.FindGameObjectsWithTag("Tracker");


        if(trackers.Length == 0) {
            Debug.LogError("No Vuforia Tracker found in Scene");
            Application.Quit();
        }
        else if (trackers.Length > 1)
        {
            Debug.LogError("Too MANY Vuforia Trackers found in Scene; Only need 1");
            Application.Quit();
        }
        else if (trackers.Length == 1){
            Debug.Log("Found Tracker: "+ trackers[0].name);
            theTracker = trackers[0].GetComponent<ImageTargetBehaviour>();
        }

    }



    /// <summary>
    /// Reset AR views and targets to reajust to the new
    /// </summary>
    public void OnRefreshPressed()
    {
        //Stop Image Target
        theTracker.enabled = false;
        //theTracker.
        Cam.GetComponent<VuforiaBehaviour>().enabled  = false;
        //Stop Extended Tracking

        //Autofocus

        //Restart Image Target
        theTracker.enabled = true;       
        Cam.GetComponent<VuforiaBehaviour>().enabled = true;
        //Restart Extended Tracking


    }


    //

    // Use this for initialization
    void Start()
    {
        FindTracker();
        FindCamera();

    }

    // Update is called once per frame
    void Update () {
		
	}
}
