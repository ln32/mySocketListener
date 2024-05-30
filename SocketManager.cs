using System;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

public class SocketManager : MonoBehaviour
{
    public static int OP_MAX = 10;
    public static int index = 0;
    public static string[] nameAry = new string[10];
    public static int[] portAry = new int[10];

    public static void setPort(string name,int port)
    {
        int temp = getPort(name);
        if (temp != -1)
            return;

        if (index > 9)
            return;

        nameAry[index] = name;
        portAry[index] = port;
        index++;
    }

    public static int getPort(string name)
    {
        for (int i = 0; i < nameAry.Length; i++)
        {
            if (nameAry[i] == name)
                return portAry[i];
        }

        return -1;
    }
    public static int getPortIndex(string name)
    {
        for (int i = 0; i < nameAry.Length; i++)
        {
            if (nameAry[i] == name)
                return i;
        }

        return -1;
    }
    public static Transform AddObj(Transform transform, string name)
    {
        GameObject temp = new GameObject(name);
        temp.transform.transform.parent = transform;
        return temp.transform;
    }
    public static void setSocket(UdpClient udpClient, string name)
    {
        udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, 1);
        udpClient.Client.Bind(new IPEndPoint(GetLocalIPAddress(), getPort(name)));
    }
    public static void setListen(UdpClient udpClient, string name)
    {
        udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, 1);
    }
    public static IPAddress GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip;
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}

