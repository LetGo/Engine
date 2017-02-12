using System;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using Utility;

namespace Engine.NetWork
{
    /// <summary>
    /// 网络线程
    /// </summary>
    public abstract class NetThread
    {
        private Thread m_thread;
        private bool m_terminateFlag;
        private System.Object m_terminateFlagMutex;
        protected NetworkStream m_netStream;

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

        /// <summary>
        /// 等待此线程执行完毕
        /// </summary>
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
                ReadFromStream();
                if (_writeOffset >= NetPackageIn.HEADER_SIZE)
                {
                    ReadPackage();
                }
            }
        }

        private void ReadFromStream()
        {
            if (m_netStream.CanRead && m_netStream.DataAvailable)
            {
                _writeOffset += m_netStream.Read(_readBuffer, _writeOffset, _readBuffer.Length - _writeOffset);
            }
            else
            {
                Thread.Sleep(16);
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void ReadPackage()
        {
            int dataLeft = _writeOffset - _readOffset;

            do 
            {
                int len = -1;
                int nIndex = 0;
                if (_readOffset + NetPackageIn.HEADER_SIZE <= _writeOffset)
                {
                    while (nIndex < NetPackageIn.HEADER_SIZE)
                    {
                        nIndex++;
                        if (TryReadUInt32Variant(_readBuffer, _readOffset, nIndex, out len) > 0)
                        {
                            _readOffset += nIndex;
                            break;
                        }
                    }
                }

                if (len == 0 || len == -1)
                {
                    _readOffset = 0;
                    return;
                }
                dataLeft = _writeOffset - _readOffset;
                //包足够长;
                if (dataLeft >= len && len != 0)
                {
                    byte[] buf = new byte[len];
                    Array.Copy(_readBuffer, _readOffset, buf, 0, len);

//                     if (NetWork.DEENCODE)
//                     {
//                         /// decode
//                         //Engine.Encrypt.Decode(ref buf);
//                     }

                    NetPackageIn package = new NetPackageIn(buf);
                    NetManager.Instance.PushRecv(package);
                    _readOffset += len;
                    dataLeft = _writeOffset - _readOffset;
                    //AddPackage(buff);
                }
                else
                {
                    _readOffset -= nIndex;
                    dataLeft = _writeOffset - _readOffset;
                    break;
                }
            } while (dataLeft >= NetPackageIn.HEADER_SIZE);
        }

        /// <returns>The number of bytes consumed; 0 if no data available</returns>
        public int TryReadUInt32Variant(byte[] source, int offset, int count, out int value)
        {
            var max = Math.Min(source.Length, offset + count);
            value = 0;
            int b = offset >= max ? -1 : source[offset++];
            if (b < 0)
            {
                return 0;
            }
            value = (int)b;
            if ((value & 128u) == 0u)
            {
                return 1;
            }
            value &= 127;
            b = offset >= max ? -1 : source[offset++];
            if (b < 0)
            {
                return 0;
            }
            value |= (int)((uint)(b & 127) << 7);
            if ((b & 128) == 0)
            {
                return 2;
            }
            b = offset >= max ? -1 : source[offset++];
            if (b < 0)
            {
                return 0;
            }
            value |= (int)((uint)(b & 127) << 14);
            if ((b & 128) == 0)
            {
                return 3;
            }
            b = offset >= max ? -1 : source[offset++];
            if (b < 0)
            {
                return 0;
            }
            value |= (int)((uint)(b & 127) << 21);
            if ((b & 128) == 0)
            {
                return 4;
            }
            b = offset >= max ? -1 : source[offset++];
            if (b < 0)
            {
                return 0;
            }
            value |= (int)((uint)b << 28);
            if ((b & 240) == 0)
            {
                return 5;
            }
            return 0;
        }
    }

    class SenderThread : NetThread
    {
        private TcpClient m_tcpClient = null;
        private SwitchList<NetPackageOut> m_packetList = new SwitchList<NetPackageOut>();
        // 线程事件
        private AutoResetEvent m_SendEvent = new AutoResetEvent(false);
        // 发送异常缓存队列
        private Queue<NetPackageOut> packout = new Queue<NetPackageOut>();

        public SenderThread(TcpClient client,NetworkStream stream)
            :base(stream)
        {
            m_tcpClient = client;

        }

        public void Send(NetPackageOut package)
        {
            m_packetList.Push(package);
            m_SendEvent.Set();//设置线程为终止 WaitOne()方法后面的代码才会执行，然后会重置为非终止状态
        }

        public void EnqueErrorPack(NetPackageOut pkg)
        {
            packout.Enqueue(pkg);
        }

        public void PopNotSendPackage(out List<NetPackageOut> listPkg)
        {
            listPkg = new List<NetPackageOut>();

            if (packout != null)
            {
                while (packout.Count > 0)
                {
                    listPkg.Add(packout.Dequeue());
                }
            }
        }

        protected override void Main()
        {
            while (!IsTerminateFlagSet() && m_SendEvent != null)
            {
                m_SendEvent.WaitOne();
                m_packetList.Switch();

                while (m_packetList.OutCmdList.size > 0)
                {
                    NetPackageOut pkg = m_packetList.OutCmdList.buffer[0];

                    m_packetList.OutCmdList.RemoveAt(0);

                    if (m_tcpClient != null && m_tcpClient.Connected)
                    {
                        try
                        {
                            byte[] buff = pkg.GetBuffer();

                            m_netStream.Write(buff, 0, buff.Length);
                            m_netStream.Flush();
                        }
                        catch (System.Exception ex)
                        {
                            EnqueErrorPack(pkg);
                        }
                    }
                    else
                    {
                        EnqueErrorPack(pkg);
                        break;
                    }
                }
            }
        }
    }
}
