using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEditor.Sprites;
using UnityEngine;

public class Temp_Linsener : SYNC_Listener
{
    public string des_IP;
    public int des_Port;
    public byte[] bytes = new byte[40]; 
    public UdpClient m_sender;

    UdpClient testSender = new UdpClient();
    public void SEND_OC_JOIN(string name)
    {

        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(des_IP), des_Port);
        IPAddress temp;
        if (IPAddress.TryParse(des_IP, out temp) == false)
        {
            Debug.Log("string is sad");
            return;
        }

        if (m_sender == null)
        {
            m_sender = new UdpClient();
        }

        m_sender.Send(OC_JOIN_PACKET(name),name.Length,iPEndPoint);

        byte[] OC_JOIN_PACKET(string name)
        {
            byte[] temp = new byte[40];
            temp.SEND_INT(0, 0);
            temp.SEND_STR(4, name);
            return temp;
        }
    }

    public override void workFunc(RecvData recvData)
    {
        Debug.Log("[Test] start read opCode - " + recvData.m_OP);
        bytes = recvData.m_buffer;
    }
}
