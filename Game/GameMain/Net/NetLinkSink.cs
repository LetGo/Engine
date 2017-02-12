using System;
using System.Collections.Generic;
using Engine.NetWork;

public class NetLinkSink : INetLinkSink
{

    public Action<bool> ConnectCallback
    {
        get;
        set;
    }

    public void OnClose()
    {
    }

    public void OnConnectError(NetWorkError e)
    {
    }

    public void OnDisConnect()
    {

    }

    public void OnReceive(NetPackageIn msg)
    {

    }

    public void ReConnected()
    {

    }

    public void Update()
    {
    }
}
