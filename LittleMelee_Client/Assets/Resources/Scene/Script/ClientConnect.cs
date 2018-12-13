using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;
using UnityEngine;

public class ClientConnect : MonoBehaviour
{

    public string serverAddress;
    public int port;

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private byte[] resultBuffer = new byte[1024];
    private string resultStr;
    private bool isConnected = false;

    // Use this for initialization
    void Start()
    {
        //InitClientConnect();
    }

    ///<summary>
    ///client connect to the server
    ///</summary>
    public void InitClientConnect()
    {
        CloseConnecting();
        client = new TcpClient();
        try
        {
            client.Connect(serverAddress, port);
            stream = client.GetStream();
            receiveThread = new Thread(SocketReceiver);
            receiveThread.IsBackground = true;
            receiveThread.Start();
            isConnected = true;
            Debug.Log("Connect successed");
        }
        catch (Exception ep)
        {
            Debug.Log("connect failed" + ep.Message);
        }
    }

    ///<summary>
    ///client receive message from server
    ///</summary>
	private void SocketReceiver()
    {
        if (client != null)
        {
            while (true)
            {
                if (!client.Client.Connected) break;

                client.Client.Receive(resultBuffer);
                resultStr = Encoding.UTF8.GetString(resultBuffer,0,resultBuffer.Length);
                Debug.Log(resultStr);
            }
        }
    }

    ///<summary>
    ///client send message to server
    ///</summary>
    public void SocketSender(string message)
    {
        if (client != null && isConnected)
        {
            try
            {
                byte[] messageByte = Encoding.UTF8.GetBytes(message);
                stream.Write(messageByte, 0, messageByte.Length);
                stream.Flush();
            }
            catch (Exception ep)
            {
                Debug.Log("send failed" + ep.Message);
            }
        }
        else
        {
            Debug.Log("unconnected");
        }
    }

    ///<summary>
    ///Close connection
    ///</summary>
    public void CloseConnecting()
    {
        if(isConnected)
        {
            SocketSender("CloseConnect");
            stream.Close();
            client.Close();
            isConnected = false;
            Debug.Log("Client close Connect");
        }
    }

    public string GetResultStr()
    {
        string returnString = resultStr;
        resultStr = "";
        return returnString;
    }

    //public void EmptyByte()
    //{
    //    resultStr = "";
    //    //receiveThread.Abort();
    //    receiveThread = new Thread(SocketReceiver);
    //    receiveThread.IsBackground = true;
    //    receiveThread.Start();
    //}
}
