using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Threading;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;

public class UDPReceiver : MonoBehaviour {

    string MyIP;
    UdpClient receiver;
    public int remotePort = 19784;
    KoskiNetworkManager netManager;

    /// <summary>
    /// 
    /// </summary>
    public void StartReceivingIP()
    {
        
        try
        {
            if (receiver == null)
            {
                receiver = new UdpClient(remotePort);
                receiver.BeginReceive(new AsyncCallback(ReceiveData), null);
                Debug.Log("Started Listening for LAN Messages over port:" + remotePort);
            }
        }
        catch (SocketException e)
        {
            Debug.Log(e.Message);
        }
    }

    void Start()
    {
        MyIP = Network.player.ipAddress;
        StartReceivingIP();
    }



    /// <summary>
    /// 
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
            Debug.Log("Received message:" + receivedString);
            netManager.changeNetworkAddress(receivedString);
        }
    }
}
