using System;
using System.Collections.Generic;
using Engine.NetWork;

class NetLinkMonitor : INetLinkMonitor
{

    public long GetTotalReceiveBytes()
    {
        return 0;
    }

    public long GetTotalSendBytes()
    {
        return 0;
    }

    public void OnReceive(NetPackageIn pak)
    {
    }

    public void OnSend(NetPackageOut pak)
    {
    }
}