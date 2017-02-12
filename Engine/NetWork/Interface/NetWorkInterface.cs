using System;

namespace Engine.NetWork
{
    /// <summary>
    /// 网络连接接口
    /// </summary>
    public interface INetLink
    {
        void Connect(string strIp,int port);

        void DisConnect();

        bool IsConnect();

        void SendMsg(NetPackageOut pak);

        void Destroy();
    }

    /// <summary>
    /// 网络连接回调
    /// </summary>
    public interface INetLinkSink
    {
        void OnConnectError(NetWorkError e);

        void Update();

        void OnDisConnect();

        void ReConnected();
        void OnReceive(NetPackageIn msg);
        void OnClose();

        Action<bool> ConnectCallback
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 网络监控接口
    /// </summary>
    public interface INetLinkMonitor
    {
        void OnReceive(NetPackageIn pak);

        void OnSend(NetPackageOut pak);

        long GetTotalReceiveBytes();

        long GetTotalSendBytes();
    }
}
