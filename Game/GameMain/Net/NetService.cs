using System;
using System.Collections.Generic;
using System.Net;
using Client;
using Utility;
using Engine.NetWork;

public class NetServece : Singleton<NetServece>, INetService
{

    private INetLink m_netLink = null;
    private INetLinkSink m_netLinkSink = null;
    private INetLinkMonitor m_netLinkMonitor = null;

    public override void Initialize()
    {
        base.Initialize();

        m_netLinkSink = new NetLinkSink();
        m_netLinkMonitor = new NetLinkMonitor();

        m_netLink = NetWork.Instance.CreateNetLink(m_netLinkSink,m_netLinkMonitor);
    }

    public override void UnInitialize()
    {
        base.UnInitialize();
        Close();
    }

    public void Connect(string strIP, int nPort, Action<bool> callback)
    {
        if (m_netLink == null)
        {
            return;
        }
        m_netLinkSink.ConnectCallback = callback;
        m_netLink.Connect(strIP, nPort);
    }

    public void Close()
    {
        if (m_netLink != null)
        {
            m_netLink.Destroy();
        }
    }

    public void Send(ProtoBuf.IExtensible cmd)
    {

    }
}
