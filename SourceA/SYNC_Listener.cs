using UnityEngine;

public class SYNC_Listener : myUDP
{
    public ClientData ownClient;

    public SYNC_Listener()
    {
        myName = "SYNC";
        ref_clientList.Find();
    }

    public override void workFunc(RecvData recvData)
    {
        trans.position = new Vector2(recvData.RECV_FLOAT(), recvData.RECV_FLOAT());
        recvData.m_todo = gogogo;
        Debug.Log( " send V2 : " + new Vector2(recvData.RECV_FLOAT(), recvData.RECV_FLOAT()));
    }

    public Transform trans;
    public void gogogo(RecvData recvData)
    {
        ClientData semiClient = new ClientData(recvData.m_adr, recvData.m_OP, Encoding.Default.GetString(recvData.m_buffer, 4, recvData.m_buffer.Length - 4));
        ClientData sender = ref_clientList.Find(x => (x.m_id == recvData.m_OP));
        if (sender == null)
            return;

        Debug.Log(" move V2 : " + new Vector2(recvData.RECV_FLOAT(), recvData.RECV_FLOAT()));
        trans.position = new Vector2(recvData.RECV_FLOAT(), recvData.RECV_FLOAT());
    }
}
