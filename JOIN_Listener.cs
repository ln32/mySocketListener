using System.Net;
using UnityEngine;

public class JOIN_Listener : myUDP
{
    //걍 보기용 신경 ㄴ
    public string[] temp;
    public JOIN_Listener()
    {
        temp = SocketManager.nameAry;
        myName = "JOIN";
    }

    public override void workFunc(RecvData recvData)
    {
        Debug.Log("start read opCode - " + recvData.m_OP);
        switch (recvData.m_OP)
        {
            case 1:
                recvData.m_todo = OC_ACPT; break;
            case 2:
                recvData.m_todo = CA_ACPT; break;
            case 3:
                recvData.m_todo = AO_HELLO; break;
            case 9:
                recvData.m_todo = GODADD; break;
            default:
                recvData.m_todo = OC_JOIN; break;
        }
    }

    public void OC_JOIN(RecvData recvData)
    {
        // RECV - ??
        // TODO - check is ok, randID
        // SEND TO OTHER - OP ID Name(Str)

        recvData.sendMSG(OC_JOIN_PACKET(recvData.RECV_STR()), "JOIN");

        byte[] OC_JOIN_PACKET(string name)
        {
            byte[] temp = new byte[40];
            temp.SEND_INT(0,1);
            temp.SEND_INT(4, UnityEngine.Random.Range(1000,10000));
            temp.SEND_STR(8, name);
            return temp;
        }
    }

    public void OC_ACPT(RecvData recvData)
    {
        // RECV - OP(1) ID Name[Str]
        // TODO - none
        // SEND TO ALL - OP(2) ID Name[Str]
        myData = new ClientData(recvData.m_adr, recvData.RECV_INT(), recvData.RECV_STR());
        SEND_ALL_CLIENT(CA_ACPT_PACKET(myData));

        byte[] CA_ACPT_PACKET(ClientData newOne)
        {
            byte[] temp = new byte[40];
            temp.SEND_INT(0, 2);
            Debug.Log("saaad1");
            temp.SEND_IPADDR(4, myData.m_ip);
            Debug.Log("saaad2");
            temp.SEND_INT(12, myData.PORT);
            Debug.Log("saaad3");
            temp.SEND_INT(16, myData.m_id);
            temp.SEND_STR(20, myData.m_name);
            return temp;
        }
    }

    public void CA_ACPT(RecvData recvData)
    {
        // RECV - OP(2) ID Name[Str]
        // TODO - host call send, add new one and send my data to new one
        // SEND TO NEW - OP(3) ID Name[Str]

        ClientData otherData = new ClientData(new IPEndPoint(recvData.RECV_IPADDR(), recvData.RECV_INT()), recvData.RECV_INT(), recvData.RECV_STR());
        ref_clientList.Add(otherData);
        otherData.sendMSG("JOIN",AO_HELLO_PACKET());

        byte[] AO_HELLO_PACKET()
        {
            byte[] temp = new byte[40];
            temp.SEND_INT(0, 3);
            temp.SEND_INT(4, myData.m_id);
            temp.SEND_STR(8, myData.m_name);
            return temp;
        }
    }

    public void AO_HELLO(RecvData recvData)
    {
        ClientData otherData = new ClientData(recvData.m_adr, recvData.RECV_INT(), recvData.RECV_STR());
        ref_clientList.Add(otherData);
    }

    public void GODADD(RecvData recvData)
    {
        ClientData otherData = new ClientData(recvData.m_adr, recvData.RECV_INT(), recvData.RECV_STR());
        ref_clientList.Add(otherData);
    }

    public void SET_HOST(string name)
    {
        if (severSock == null)
        { Debug.Log("severSock is Null");  return; }

        IPEndPoint iPEndPoint = new IPEndPoint(SocketManager.GetLocalIPAddress(), myPort);
        ClientData otherData = new ClientData(iPEndPoint, 999, name);
        ref_clientList.Add(otherData);
    }
}
