using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionControl : MonoBehaviour {

    public TagDatabase theTagReference;
    PlayerControllerScript thePlayer;

    // Use this for initialization
    void Start () {

        theTagReference = new TagDatabase();
        theTagReference.tagList = new Dictionary<string, int>();
        theTagReference.Initialize();
    }
	
	// Update is called once per frame
	void Update () {
		
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