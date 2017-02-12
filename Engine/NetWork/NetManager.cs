using System;
using System.Collections.Generic;
using Utility;

namespace Engine.NetWork
{
    enum NetCmdType
    {
        NetCmd_Null = 0,
        NetCmd_ConnectSuccess,
        NetCmd_ConnectError,
        NetCmd_Recv,
        NetCmd_Close,
        NetCmd_Error,
    }

    class NetCommand
    {
        public NetCmdType dwType;  // 命令类型
        public int dwLen;          // 数据长度
        public NetWorkError error; // 网络错误
        public NetPackageIn msg;      // 数据buff

        public NetCommand()
        {
            this.dwType = NetCmdType.NetCmd_Null;
            this.dwLen = 0;
            this.error = NetWorkError.NetWorkError_null;
            msg = null;
        }
        public NetCommand(NetCommand cmd)
        {
            this.dwType = cmd.dwType;
            this.dwLen = cmd.dwLen;
            if (this.dwLen > 0)
            {
                this.msg = cmd.msg;
            }
        }
    }

    class NetManager : Singleton<NetManager>
    {
        private SwitchList<NetCommand> m_NetCommandList = new SwitchList<NetCommand>();   // 网络命令队列
        private INetLinkSink m_NetLinkSink = null;
        private INetLinkMonitor m_NetLinkMonitor = null;

        public void Initialize(INetLinkSink netLinkSink,INetLinkMonitor netLinkMonitor)
        {
            m_NetLinkMonitor = netLinkMonitor;
            m_NetLinkSink = netLinkSink;
        }

        public void PushConnectSuccess()
        {
            NetCommand cmd = new NetCommand();
            cmd.dwType = NetCmdType.NetCmd_ConnectSuccess;
            cmd.error = NetWorkError.NetWorkError_ConnectSuccess;
            m_NetCommandList.Push(cmd);
        }
        public void PushConnectError(NetWorkError error)
        {
            NetCommand cmd = new NetCommand();
            cmd.dwType = NetCmdType.NetCmd_ConnectError;
            cmd.error = error;
            m_NetCommandList.Push(cmd);
        }

        public void PushClose()
        {
            NetCommand cmd = new NetCommand();
            cmd.dwType = NetCmdType.NetCmd_Close;
            m_NetCommandList.Push(cmd);
        }

        public void PushRecv(NetPackageIn msg)
        {
            NetCommand cmd = new NetCommand();
            cmd.dwType = NetCmdType.NetCmd_Recv;
            cmd.msg = msg;
            cmd.dwLen = (int)msg.Length;
            m_NetCommandList.Push(cmd);
        }

        public void OnConnectSuccess()
        {
            if (m_NetLinkSink != null)
            {
                m_NetLinkSink.OnConnectError(NetWorkError.NetWorkError_ConnectSuccess);
            }
        }

        public void OnConnectError(NetWorkError error)
        {
            if (m_NetLinkSink != null)
            {
                if (error == NetWorkError.NetWorkError_DisConnect)
                {
                    m_NetLinkSink.OnDisConnect();
                }
                else
                {
                    m_NetLinkSink.OnConnectError(error);
                }
            }
        }

        public void OnClose()
        {
            if (m_NetLinkSink != null)
            {
                m_NetLinkSink.OnClose();
            }
        }
        public void OnRecv(NetPackageIn msg)
        {
            if (m_NetLinkSink != null)
            {
                m_NetLinkSink.OnReceive(msg);
            }

            if (m_NetLinkMonitor != null)
            {
                m_NetLinkMonitor.OnReceive(msg);
            }
        }

        public void OnEvent()
        {
            m_NetCommandList.Switch();
            if (m_NetCommandList.OutCmdList != null)
            {
                while (m_NetCommandList.OutCmdList.size > 0)
                {
                    NetCommand cmd = m_NetCommandList.OutCmdList.buffer[0];
                    try
                    {
                        switch (cmd.dwType)
                        {
                            case NetCmdType.NetCmd_ConnectSuccess:
                                {
                                    OnConnectSuccess();
                                    break;
                                }
                            case NetCmdType.NetCmd_ConnectError:
                                {
                                    OnConnectError(cmd.error);
                                    break;
                                }
                            case NetCmdType.NetCmd_Close:
                                {
                                    OnClose();
                                    break;
                                }
                            case NetCmdType.NetCmd_Recv:
                                {
                                    OnRecv(cmd.msg);
                                    break;
                                }
                        }
                    }
                    catch (SystemException e)
                    {
                        if (cmd.dwType == NetCmdType.NetCmd_Recv)
                        {
                            //Utility.Log.Error("网络消息处理异常{0}", e.ToString());
                        }
                    }
                    finally
                    {
                        m_NetCommandList.OutCmdList.RemoveAt(0);
                    }
                }
            }
        }
    }
}
