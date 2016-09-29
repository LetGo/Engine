using System;

namespace Engine
{

    /// <summary>
    /// 网络连接接口
    /// </summary>
    public interface INetLink
    {
        void Connect(string strIp,int port);

        void DisConnect();

        bool IsConnect();

        void SendMsg();

        void Destroy();
    }

    /// <summary>
    /// 网络连接回调
    /// </summary>
    public interface INetLinkSink
    {
        void OnConnectError();

        void Update();

        void OnDisConnect();

        void ReConnected();

        void OnClose();
    }

    /// <summary>
    /// 网络监控接口
    /// </summary>
    public interface INetLinkMonitor
    {
        void OnReceive();

        void OnSend();

        long GetTotalReceiveBytes();

        long GetTotalSendBytes();
    }
}
