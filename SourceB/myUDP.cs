using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class myUDP : MonoBehaviour
{
    public List<ClientData> ref_clientList;
    public ClientData myData = null;

    public List<RecvData> recvList = new List<RecvData>();
    public UdpClient severSock;
    public int myPort;
    public string myName = "null";
    void Update()
    {
        if (recvList != null && recvList.Count > 0)
        {
            RecvData recvData = recvList[0];
            if (recvList[0].m_todo != null)
            {
                recvData.m_todo(recvData);
                recvList.RemoveAt(0);
            }
        }
    }

    public void InitRECV(UDP_Manager udpM, int port = 0)
    {
        ref_clientList = udpM.clientList;

        try
        {
            if (severSock == null)
            {
                SocketManager.setPort(myName, port);
                severSock = new UdpClient(port);
                myPort = ((IPEndPoint)severSock.Client.LocalEndPoint).Port;
                SocketManager.setListen(severSock, ("JOIN"));

                Debug.Log("Listen Start - " + myName + " at " + SocketManager.GetLocalIPAddress() + " / " + myPort);
                severSock.BeginReceive(new AsyncCallback(RECV_CALLBACK), null);
            }
        }
        catch (SocketException e)
        {
            Debug.Log(e.Message);
        }
    }

    public void InitRECV()
    {
        try
        {
            if (severSock == null)
            {
                SocketManager.setPort(myName, myPort);
                severSock = new UdpClient(0); 
                severSock.BeginReceive(new AsyncCallback(RECV_CALLBACK), null);
                SocketManager.setListen(severSock, ("SYNC"));

                myPort = ((IPEndPoint)severSock.Client.LocalEndPoint).Port;
 
                Debug.Log("Listen Start - " + myName + " at " + SocketManager.GetLocalIPAddress() + " / " + myPort);
            }
        }
        catch (SocketException e)
        {
            Debug.Log(e.Message);
        }
    }

    public void RECV_CALLBACK(IAsyncResult ar)
    {
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, myPort);
        byte[] m_ReceivedBytes;

        if (severSock == null)
        {
            return;
        }

        m_ReceivedBytes = severSock.EndReceive(ar, ref ipEndPoint);

        if (m_ReceivedBytes.Length > 1)
        {
            Debug.Log("RECV - " + myName + " / " + m_ReceivedBytes.Length);
            RecvData newData = new RecvData(m_ReceivedBytes, ipEndPoint);
            workFunc(newData);
            if (newData.m_todo != null)
                recvList.Add(newData);
            else
                Debug.Log("trash Data");
        }

        severSock.BeginReceive(new AsyncCallback(RECV_CALLBACK), null);
    }

    public void SEND_CALLBACK(IAsyncResult ar)
    {
        Debug.Log(myName + " called SEND_CALLBACK");
    }

    public void SEND_ALL_CLIENT(byte[] sendData)
    {
        for (int i = 0; i < ref_clientList.Count; i++)
        {
            ref_clientList[i].sendMSG(myName, sendData, new AsyncCallback(SEND_CALLBACK));
        }
    }

    public virtual void workFunc(RecvData recvData)
    {
        //Debug.Log("time to work UDP");
        //recvList.Clear();
    }
}



[Serializable]
public class RecvData
{
    public static int index = 1;
    public int m_OP;
    public UdpClient m_sender;
    public IPEndPoint m_adr;
    public byte[] m_buffer;
    public int port;
    public string ip;
    public int process = 0;

    public TODO m_todo;

    public RecvData(byte[] i_data, IPEndPoint i_adr)
    {
        m_adr = i_adr;
        port = m_adr.Port;
        ip = m_adr.Address + "";
        m_buffer = i_data;
        m_OP = RECV_INT();
    }

    public void sendMSG(byte[] msg, AsyncCallback func)
    {
        if (m_sender == null)
        {
            m_sender = new UdpClient(); SocketManager.setSocket(m_sender, ("JOIN"));
        }

        byte[] packets = msg;
        m_sender.BeginSend(packets, packets.Length, m_adr, new AsyncCallback(func), this);
    }

    public void sendMSG(byte[] msg)
    {
        if (m_sender == null)
        {
            m_sender = new UdpClient(); SocketManager.setSocket(m_sender, ("JOIN"));
        }

        Debug.Log(port);
        byte[] packets = msg;

        m_sender.Send(packets, packets.Length, m_adr);
    }

    public void sendMSG(byte[] msg, String str)
    {
        if (m_sender == null)
        {
            m_sender = new UdpClient(); SocketManager.setSocket(m_sender, str);
        }

        Debug.Log(port);
        byte[] packets = msg;

        m_sender.Send(packets, packets.Length, m_adr);
    }

    public int RECV_INT()
    {
        process += 4;
        return BitConverter.ToInt32(m_buffer, process-4);
    }
    public float RECV_FLOAT()
    {
        process += 4;
        return BitConverter.ToSingle(m_buffer, process - 4);
    }
    public long RECV_IPADDR()
    {
        process += 8;
        return BitConverter.ToInt64(m_buffer, process - 8);
    }
    public string RECV_STR()
    {
        int temp = process; process = m_buffer.Length;
        return Encoding.Default.GetString(m_buffer ,temp , m_buffer.Length - temp);
    }

    public delegate void TODO(RecvData recvData);
}

public static class SendData
{
    public static int SEND_INT(this byte[] m_buffer,int process, int input)
    {
        int size = 4;
        int space = 0;
        space = m_buffer.Length - process;

        //space 버린단 마인드, 필요 여백공간 출력
        space = size - space;
        if (space > 0)
        {
            return space;
        }

        Array.Copy(BitConverter.GetBytes(input), 0, m_buffer, process, size);

        return 0;
    }
    public static int SEND_FLOAT(this byte[] m_buffer, int process, float input)
    {
        int size = 4;
        int space = 0;
        space = m_buffer.Length - process;

        //space 버린단 마인드, 필요 여백공간 출력
        space = size - space;
        if (space > 0)
        {
            return space;
        }

        Array.Copy(BitConverter.GetBytes(input), 0, m_buffer, process, size);

        return 0;
    }
    public static int SEND_IPADDR(this byte[] m_buffer, int process, IPAddress input)
    {
        byte[] byteIP = input.GetAddressBytes();

        int size = byteIP.Length;
        int space = 0;
        space = m_buffer.Length - process;

        //space 버린단 마인드, 필요 여백공간 출력
        space = size - space;
        if (space > 0)
        {
            return space;
        }

        Array.Copy(byteIP, 0, m_buffer, process, size);

        return 0;
    }
    public static int SEND_STR(this byte[] m_buffer, int process, string input)
    {
        int size = input.Length;
        int space = 0;
        space = m_buffer.Length - process;

        //space 버린단 마인드, 필요 여백공간 출력
        space = size - space;
        if (space > 0)
        {
            return space;
        }

        Array.Copy(Encoding.ASCII.GetBytes(input), 0, m_buffer, process, size);

        return 0;
    }
}