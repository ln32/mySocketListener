using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using Unity.VisualScripting;

public class UDP_Manager : MonoBehaviour
{
    public List<ClientData> clientList = new List<ClientData>();
    public int myName;
    public Transform myTrans;
    JOIN_Listener m_join;
    SYNC_Listener m_sync;

    void Start()
    {
        if (m_join != null)
        {
            m_join = SocketManager.AddObj(transform, "JOIN_Listener").AddComponent<JOIN_Listener>();
            m_join.InitRECV(this, 52555);
        }

        if (m_sync == null)
        {
            m_sync = SocketManager.AddObj(transform, "SYNC_Listener").AddComponent<SYNC_Listener>();
            m_sync.InitRECV(this);
        }
    }

    public void setHost()
    {
        broadCast();
    }

    public void broadCast()
    {
        byte[] packets = new byte[40]; 
        Array.Copy(BitConverter.GetBytes(clientList[0].m_id), 0, packets, 0, 4);
        Array.Copy(BitConverter.GetBytes(myTrans.position.x),0, packets, 4,4);
        Array.Copy(BitConverter.GetBytes(myTrans.position.y), 0, packets, 8, 4);
        for (int i = 0; i < clientList.Count; i++)
        {
            clientList[i].sendMSG("SYNC", packets, new AsyncCallback(RECV_CALLBACK));
        }
        
    }

    void RECV_CALLBACK(IAsyncResult ar)
    {
        ClientData client = (ClientData)ar.AsyncState;
        client.m_sender.EndSend(ar);
    }
}

[Serializable]
public class ClientData
{
    public int m_id;
    public string m_name;
    public string IPADDR;
    public int PORT;
    public UdpClient m_sender;
    public IPEndPoint[] m_adr = new IPEndPoint[SocketManager.OP_MAX];
    public IPAddress m_ip;
    public Transform trans;
    public ClientData(IPEndPoint i_adr,int i_id, string i_name)
    {
        m_adr[SocketManager.getPortIndex("JOIN")] = i_adr;
        m_ip = i_adr.Address;
        
        IPADDR = m_ip.ToString();
        PORT = m_adr[0].Port;

        m_id = i_id;
        m_name = i_name;
    }

    public void sendMSG(string OP_CASE,byte[] msg, AsyncCallback func)
    {
        if (SocketManager.getPortIndex(OP_CASE) == -1)
        { Debug.Log("fuck"); return; }

        if (m_sender == null)
        {
            m_sender = new UdpClient();

            m_sender.Client.Bind(new IPEndPoint(IPAddress.Any, SocketManager.getPort(OP_CASE)));
        }
        Debug.Log(((IPEndPoint)m_sender.Client.LocalEndPoint).Port);

        byte[] packets = msg;
        Debug.Log(packets[0] + " / " + packets[1]);
        m_sender.BeginSend(packets, packets.Length, m_adr[SocketManager.getPortIndex(OP_CASE)], new AsyncCallback(func), this);
    }

    public void sendMSG(string OP_CASE, byte[] msg)
    {
        if (SocketManager.getPortIndex(OP_CASE) == -1)
        { Debug.Log("fuck"); return; }

        if (m_sender == null)
        {
            m_sender = new UdpClient();
            m_sender.Client.Bind(new IPEndPoint(IPAddress.Any, SocketManager.getPort(OP_CASE)));
        }

        byte[] packets = msg;
        m_sender.Send(packets, packets.Length, m_adr[SocketManager.getPortIndex(OP_CASE)]);
    }
}

