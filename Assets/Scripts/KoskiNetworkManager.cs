using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class KoskiNetworkManager : NetworkManager {

    UDPMulticast BroadCaster;
    UDPReceiver UDPReceiver;

    void start()
    {
        BroadCaster = GetComponent<UDPMulticast>();
        UDPReceiver = GetComponent<UDPReceiver>();
        UDPReceiver.StartReceivingIP();
        networkAddress = null;
    }

    public void StartLocalServer()
    {
        BroadCaster = GetComponent<UDPMulticast>();
        //networkAddress = "192.168.1.132";
        Debug.LogError(Network.player.ipAddress);
        BroadCaster.StartBroadcast();        
     //   SceneManager.LoadScene("main");
        ServerChangeScene("main");
        StartHost();

    }

   
    public void StartClientGame()
    {
        if (getHostReady())
        {
            Debug.LogWarning("I want to connect to:" + networkAddress);
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
