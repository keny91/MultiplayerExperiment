using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prototype.NetworkLobby;


    public class LiveUpdateUI : MonoBehaviour {

        public UDPReceiver BroadCastListener;
        public Text theInput2change;
        

    // Use this for initialization
    void Start() {
            BroadCastListener = GetComponent<UDPReceiver>();
            //transform.FindChild("InputField").GetComponent<InputField>();
        }
   
        // Update is called once per frame
        void Update() {

            //Debug.Log("Refresed ID");
            if (BroadCastListener)
            {
                if (BroadCastListener.LanServerFound)
                {
                    GetComponent<LobbyManager>().networkAddress = BroadCastListener.theReceivedIP;
                    //networkAddress = BroadCastListener.theReceivedIP;
                    theInput2change.text = BroadCastListener.theReceivedIP; 
                    


                }
            }

            else
            {
                try
                {
                    BroadCastListener = GetComponent<UDPReceiver>();
                }
                catch (MissingReferenceException e)
                {

                }
            }
        }

    


    }
