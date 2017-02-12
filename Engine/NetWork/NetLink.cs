using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Engine.NetWork
{
    class NetLink : INetLink
    {
        private SCOKET_CONNECT_STATE socketstate = SCOKET_CONNECT_STATE.UNCONNECTED;

        private TcpClient m_tcpClient;
        private ReceiverThread m_receiver;
        private SenderThread m_sender;
        // 网络连接回调
        private INetLinkSink m_NetLinkSink = null;
        public INetLinkSink NetLinkSink { get { return m_NetLinkSink; } }
        // 网络监测回调
        private INetLinkMonitor m_NetLinkMonitor = null;
        public INetLinkMonitor NetLinkMonitor { get { return m_NetLinkMonitor; } }

        private string m_strIP = string.Empty;
        private int m_nPort = 0;

        // 掉线数据包缓存
        private List<NetPackageOut> DiaoxpackOut = null;

        double m_fLastCheckTime = 0;
        public bool Connected
        {
            get
            {
                if (m_tcpClient == null)
                {
                    return false;
                }

                if (m_tcpClient.Client.Poll(1000, SelectMode.SelectRead) && (m_tcpClient.Client.Available == 0))
                {
                    return false;
                }

                return socketstate == SCOKET_CONNECT_STATE.CONNECTED;
            }
        }

        public NetLink(INetLinkSink linkSink, INetLinkMonitor netLinkMonitor)
        {
            m_NetLinkSink = linkSink;
            m_NetLinkMonitor = netLinkMonitor;

            NetManager.Instance.Initialize(m_NetLinkSink,m_NetLinkMonitor);
        }

#region INetLink

        public void Connect(string strIp, int port)
        {
            DisConnect();

            m_tcpClient = new TcpClient();

            if (string.IsNullOrEmpty(strIp))
            {
                return;
            }
            m_strIP = strIp;
            m_nPort = port;
            //客户端异步模式下通过BeginConnect方法和EndConnect方法来实现与服务器的连接
            m_tcpClient.BeginConnect(m_strIP, m_nPort, ConnectAsyncCallback, m_tcpClient.Client);
        }

        public void DisConnect()
        {
            if (m_tcpClient != null && m_tcpClient.Connected)
            {
                m_tcpClient.GetStream().Close();
                m_tcpClient.Close();
                m_tcpClient = null;
                socketstate = SCOKET_CONNECT_STATE.UNCONNECTED;
                NetManager.Instance.PushClose();
            }
            SetTerminateFlag();
        }

        public bool IsConnect()
        {
            return Connected;
        }

        public void SendMsg(NetPackageOut pkg)
        {
            if (Connected)
            {
                if (m_sender != null)
                {
                    m_sender.Send(pkg);

                    if (m_NetLinkMonitor != null)
                    {
                        m_NetLinkMonitor.OnSend(pkg);
                    }
                }
            }
            else
            {
                if (m_sender != null)
                {
                    m_sender.EnqueErrorPack(pkg);
                }
            }
        }

        public void Destroy()
        {
            DisConnect();
            m_sender = null;
            m_receiver = null;
        }

#endregion
        public void Run()
        {
            if (m_tcpClient == null)
            {
                return;
            }


            TimeSpan endtimespan = new TimeSpan(System.DateTime.Now.Ticks);
            double currTime = endtimespan.TotalSeconds;
            if (currTime - m_fLastCheckTime > 2f)
            {
                CheckConnect();
                m_fLastCheckTime = currTime;
            }

            NetManager.Instance.OnEvent();
        }

        void ConnectAsyncCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.EndConnect(ar);//完成BeginConnect方法的异步连接远程主机的请求
            }
            catch (System.Exception ex)
            {
            	//TODO
                NetManager.Instance.PushConnectError(NetWorkError.NetWorkError_ConnectFailed);
            }
            finally
            {
                if (m_tcpClient.Connected)
                {
                    //  设置属性
                    m_tcpClient.NoDelay = true;
                    m_tcpClient.ReceiveBufferSize = 1024 * 1024;;
                    m_tcpClient.ReceiveTimeout = 10000;
                    m_tcpClient.SendBufferSize = 1024 * 1024;;
                    m_tcpClient.SendTimeout = 10000;

                    if (socketstate == SCOKET_CONNECT_STATE.CONNECTE_STOP)
                    {
                        if (m_sender != null)
                        {
                            m_sender.PopNotSendPackage(out DiaoxpackOut);
                            //Utility.Log.Trace("断线重连``````````````弹出包->count=" + DiaoxpackOut.Count);
                        }

                        // 重新连接回调
                        if (m_NetLinkSink != null)
                        {
                            m_NetLinkSink.ReConnected();
                        }
                    }

                    StartSendThread();
                    StartReceiveThread();
                    socketstate = SCOKET_CONNECT_STATE.CONNECTED;
                    NetManager.Instance.PushConnectSuccess();
                }
                else
                {
                    socketstate = SCOKET_CONNECT_STATE.UNCONNECTED;
                    NetManager.Instance.PushConnectError(NetWorkError.NetWorkError_UnConnect);
                }
            }
        }


        void CheckConnect()
        {
            if (socketstate == SCOKET_CONNECT_STATE.CONNECTED && m_tcpClient != null && (m_tcpClient.Client != null))
            {
                // 另外说明：tcpc.Connected同tcpc.Client.Connected；
                // tcpc.Client.Connected只能表示Socket上次操作(send,recieve,connect等)时是否能正确连接到资源,
                // 不能用来表示Socket的实时连接状态。
                //m_tcpClient.Connected

                //((m_tcpClient.Client.Poll(1000, SelectMode.SelectRead) && (m_tcpClient.Client.Available == 0)) || !m_tcpClient.Client.Connected)

                if ((!m_tcpClient.Client.Connected || (m_tcpClient.Client.Poll(1000, SelectMode.SelectRead) && m_tcpClient.Client.Available == 0)))
                {
                    DisConnect();
                    socketstate = SCOKET_CONNECT_STATE.CONNECTE_STOP;
                    NetManager.Instance.PushConnectError(NetWorkError.NetWorkError_DisConnect);
                }
            }
        }

        protected void StartReceiveThread()
        {
            if (m_receiver != null)
            {
                m_receiver = null;
            }
            m_receiver = new ReceiverThread(m_tcpClient.GetStream());
            m_receiver.StartThread();
        }

        protected void StartSendThread()
        {
            if (m_sender != null)
            {
                m_sender = null;
            }
            SenderThread sendThread = new SenderThread(m_tcpClient, m_tcpClient.GetStream());
            m_sender = sendThread;
            m_sender.StartThread();
        }

        protected void SetTerminateFlag()
        {
            if (m_sender != null)
            {
                m_sender.SetTerminateFlag();
            }

            if (m_receiver != null)
            {
                m_receiver.SetTerminateFlag();
            }
        }
    }
}
