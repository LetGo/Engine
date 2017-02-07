using System;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;


namespace NetWork
{
    /// <summary>
    /// 网络线程
    /// </summary>
    public abstract class NetThread
    {
        private Thread m_thread;
        private bool m_terminateFlag;
        private System.Object m_terminateFlagMutex;
        private NetworkStream m_netStream;

        public NetThread(NetworkStream stream)
        {
            m_netStream = stream;
            m_thread = new Thread(ThreadProc);
            m_terminateFlag = false;
            m_terminateFlagMutex = new System.Object();
        }

        protected static void ThreadProc(object obj)
        {
            NetThread me = (NetThread)obj;
            me.Main();
        }

        public void StartThread()
        {
            m_thread.Start(this);
        }

        protected abstract void Main();

        public void WaitTermination()
        {
            m_thread.Join();
        }

        public void SetTerminateFlag()
        {
            lock (m_terminateFlagMutex)
            {
                m_terminateFlag = true;
            }
        }

        public bool IsTerminateFlagSet()
        {
            lock (m_terminateFlagMutex)
            {
                return m_terminateFlag;
            }
        }
    }

    class ReceiverThread : NetThread
    {
        const uint MaxPackageSize = 1024 * 512;
        //缓冲区;
        private byte[] _readBuffer;
        //缓冲区  读取 下标;
        private int _readOffset;
        //缓冲区  写入 下标;
        private int _writeOffset;

        public ReceiverThread(NetworkStream stream):base(stream)
        {
            _readBuffer = new byte[2 * MaxPackageSize];
            _readOffset = 0;
            _writeOffset = 0;
        }

        protected override void Main()
        {
            while (!IsTerminateFlagSet())
            {
                
            }
        }
    }
}
