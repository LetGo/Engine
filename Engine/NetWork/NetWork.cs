using System;
using System.Collections.Generic;
using System.Net;
using Utility;

namespace Engine.NetWork
{
    public class NetWork : Singleton<NetWork>
    {
        // 网络连接
        private NetLink m_netLink = null;

        public INetLink CreateNetLink(INetLinkSink linSink,INetLinkMonitor linkMonitor)
        {
            m_netLink = new NetLink(linSink,linkMonitor);
            return m_netLink;
        }

        public void Connect(string strIp,int nPort,System.Action<bool> connectCallback)
        {
            if (m_netLink != null)
            {
                m_netLink.NetLinkSink.ConnectCallback = connectCallback;
                m_netLink.Connect(strIp, nPort);
            }
        }

        public void Run()
        {
            if (m_netLink != null)
            {
                m_netLink.Run();
            }
        }

        public bool IsConnect()
        {
            if (m_netLink != null)
            {
                return m_netLink.IsConnect();
            }
            return false;
        }

        public void Close()
        {
            if (m_netLink != null)
            {
                m_netLink.Destroy();
            }
        }
    }
}
