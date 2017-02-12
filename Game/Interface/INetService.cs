using System;
using System.Collections.Generic;

namespace Client
{
    public interface INetService
    {
        void Connect(string strIP, int nPort, Action<bool> callback);

        void Close();

        void Send(ProtoBuf.IExtensible cmd);
    }
}
