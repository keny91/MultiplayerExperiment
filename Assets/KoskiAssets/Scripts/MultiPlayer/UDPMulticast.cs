using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class UDPMulticast : MonoBehaviour {

    UdpClient sender;
    public int localPort = 7755;
    int remotePort = 19784;
    string customMessage;
   // NetworkManager NetworkM;

    void Start()
    {

       // NetworkM = GetComponent<NetworkManager>();       
        customMessage = Network.player.ipAddress;
    }


    /// <summary>
    /// Begin emmiting the broadcast.
    /// </summary>
    public void StartBroadcast()
    {
        //customMessage = Network.player.ipAddress;
        //sender = new UdpClient(localPort, AddressFamily.InterNetwork);
        sender = new UdpClient(localPort, AddressFamily.InterNetwork);
        sender.Close();
        try
        {
            customMessage = Network.player.ipAddress;
            sender = new UdpClient(localPort, AddressFamily.InterNetwork);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Broadcast, remotePort);
            sender.Connect(groupEP);

            //SendData ();
            //Invoke("StopBroadcast", 0.1f);
            InvokeRepeating("SendData", 0, 2f);

        }
        catch(SocketException e)
        {
            Debug.LogWarning("Someone is Hosting a game already!");
        }
    }


    /// <summary>
    /// Stop all UDP broadcast emitted by this object.
    /// </summary>
    public void StopBroadcast()
    {
        sender.Close();
        CancelInvoke();
        Debug.Log("The UDP BroadCast has been Stopped.");
    }

    /// <summary>
    /// Send String message over the local network
    /// </summary>
    void SendData()
    {
        //  ip
        //string customMessage = myName + " * " + myIP + " * " + myGameName;
        //string customMessage = NetworkM.singleton.networkAddress;
        if (customMessage != "")
        {
            Debug.Log("Sent host message: "+customMessage);
            sender.Send(Encoding.ASCII.GetBytes(customMessage), customMessage.Length);
        }
    }


    /// <summary>
    /// Cancel any actions reserving the selected port for package sending
    /// </summary>
    void CancelSendData()
    {

            string NewcustomMessage = "";
            Debug.Log("Sent host message: HOSTING CANCELLED");
            sender.Send(Encoding.ASCII.GetBytes(NewcustomMessage), NewcustomMessage.Length);
            sender.Close();

    }


}
