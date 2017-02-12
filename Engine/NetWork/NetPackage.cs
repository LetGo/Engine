using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.NetWork
{
    public class NetPackageIn : MemoryStream
    {
        public static int HEADER_SIZE = 5;//protobuff中一个整数占用的最大字节数 为5

        public NetPackageIn(byte[] buffer)
            :base(buffer)
        {

        }
    }

    public class NetPackageOut : MemoryStream
    {
        private int m_nCode = 0;
        public NetPackageOut(MemoryStream buff,int nCode)
        {
            Write(buff.GetBuffer(), 0, (int)buff.Length);
            Flush();
            m_nCode = nCode;
        }

        public int GetCode()
        {
            return m_nCode;
        }
    }
}
