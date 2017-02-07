using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Threading;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using Prototype.NetworkLobby;

public class UDPReceiver : MonoBehaviour {

    public string theReceivedIP;
    private string MyIP;
    UdpClient receiver;
    public int remotePort = 19784;
    LobbyManager netManager;
   
    public bool LanServerFound = false;


    /// <summary>
    /// Start the listening process for the selected port for any incomming asyncronous messages.
    /// </summary>
    public void StartReceivingIP()
    {
        
        try
        {
            if (receiver == null)
            {
                netManager = GetComponent<LobbyManager>();
                receiver = new UdpClient(remotePort);
                receiver.BeginReceive(new AsyncCallback(ReceiveData), null);
                Debug.Log("Started Listening for LAN Messages over port:" + remotePort);
            }

        }
        catch (SocketException e)
        {
            Debug.Log(e.Message + "Port" + remotePort);
        }
    }

    void Start()
    {
        MyIP = Network.player.ipAddress;
        theReceivedIP = null;
        StartReceivingIP();

    }



    /// <summary>
    /// Get and analyze an incoming message and determine a possible host
    /// </summary>
    /// <param name="result"></param>
    private void ReceiveData(IAsyncResult result)
    {
        IPEndPoint receiveIPGroup;
        receiveIPGroup = new IPEndPoint(IPAddress.Any, remotePort);
        byte[] received;
        if (receiver != null)
        {
            received = receiver.EndReceive(result, ref receiveIPGroup);
        }
        else
        {
            Debug.LogWarning("No Host available");
            return;
        }
        receiver.BeginReceive(new AsyncCallback(ReceiveData), null);
        string receivedString = Encoding.ASCII.GetString(received);

        if(MyIP == receivedString)
            Debug.Log("Received MY OWN message");
        else 
        {
            Debug.Log("Received message:" + receivedString + "My ID is: "+ MyIP);
            LanServerFound = true;
            theReceivedIP = receivedString;
            //netManager.changeNetworkAddress(receivedString);
        }
    }


    /// <summary>
    /// Stop listening for any new incomming messages.
    /// </summary>
    public void StopReceiving()
    {
        receiver = new UdpClient(remotePort);
        receiver.Close();
        //  receiver.
    }
}
