using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KoskiNetworkManager : NetworkManager {

    UDPMulticast BroadCaster;
    UDPReceiver UDPReceiver;
    public Text PROV;

    void start()
    {
        BroadCaster = GetComponent<UDPMulticast>();
        UDPReceiver = GetComponent<UDPReceiver>();
        UDPReceiver.StartReceivingIP();
        PROV = GameObject.Find("IPDISPLAY").GetComponent<Text>();
        networkAddress = null;
    }

    void update()
    {
        PROV.text = networkAddress;
        Debug.LogError(PROV.text);
    }


    public void StartLocalServer()
    {
        if (!NetworkClient.active && !NetworkServer.active && matchMaker == null)
        {
            BroadCaster = GetComponent<UDPMulticast>();
            //networkAddress = "192.168.1.132";
            Debug.LogError(Network.player.ipAddress);
            BroadCaster.StartBroadcast();
            //   SceneManager.LoadScene("main");
            //ServerChangeScene("main");
            
            StartHost();
        }


    }

   
    public void StartClientGame()
    {
        if (getHostReady())
        {
            
            Debug.LogWarning("I want to connect to:" + networkAddress);
            //SceneManager.LoadScene("main");
            StartClient();
        }
        else
        {
            Debug.LogWarning("No HOST AVAILABLE");
        }
    }



    public void changeNetworkAddress(string newValue)
    {
        networkAddress = newValue;
        
        //Debug.Log(PROV.name);
        if (PROV != null)
        {
            //PROV.text = networkAddress;
            Debug.Log("Value Changed to:" + networkAddress);
        }

        
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }


    public bool getHostReady()
    {
        if (networkAddress != null)
        {
            Debug.LogWarning("HOST READY");
            return true;
        }
            
        else
            return false;
    }



}
