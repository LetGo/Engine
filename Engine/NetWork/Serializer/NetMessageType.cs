using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.NetWork
{
    public struct NetMessageType : IComparable<NetMessageType>,IEquatable<NetMessageType>
    {
        public static readonly NetMessageType Empty = new NetMessageType();

        public uint Cmd { get; set; }
        public uint Param { get; set; }

        public static bool operator ==(NetMessageType a, NetMessageType b)
        {
            if (System.Object.ReferenceEquals(a, b))
                return true;
            if (((object)a == null) || ((object)b == null))
                return false;
            return a.Cmd == b.Cmd && a.Param == b.Param;
        }

        public static bool operator !=(NetMessageType a, NetMessageType b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is NetMessageType ? this == (NetMessageType)obj : false;
        }

        public override int GetHashCode()
        {
            return ((int)this.Cmd << 16) | ((int)this.Param & 0x0000FFFF);
        }

        #region IComparable<NetMessageType> 成员
        public int CompareTo(NetMessageType other)
        {
            if (this.Cmd > other.Cmd)
                return 1;
            else if (this.Cmd < other.Cmd)
                return -1;

            if (this.Param > other.Param)
                return 1;
            else if (this.Param < other.Param)
                return -1;

            return 0;
        }
        #endregion

        #region IEquatable<NetMessageType> 成员
        public bool Equals(NetMessageType other)
        {
            return this == other;
        }
        #endregion

        public override string ToString()
        {
            return string.Format("0x{0:X8} ({1},{2})", this.GetHashCode(), Cmd, Param);
        }
    }
}
