using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Engine.NetWork
{
    class ProtobufCommandSerializer : Singleton<ProtobufCommandSerializer>,IEnumerable<KeyValuePair<Type,NetMessageType>>
    {
        public class NetMessageTypeComparer : IEqualityComparer<NetMessageType>
        {
            public bool Equals(NetMessageType x, NetMessageType y)
            {
                return x == y;
            }
            public int GetHashCode(NetMessageType obj)
            {
                return obj.GetHashCode();
            }
        }


        private Dictionary<Type, NetMessageType> tableType2ID = new Dictionary<Type, NetMessageType>();
        private Dictionary<NetMessageType, Type> tableID2Type = new Dictionary<NetMessageType, Type>(new NetMessageTypeComparer());


        public NetMessageType this[Type type]
        {
            get
            {
                NetMessageType id;
                return tableType2ID.TryGetValue(type, out id) ? id : NetMessageType.Empty;
            }
        }

        public Type this[NetMessageType id]
        {
            get
            {
                Type ret;
                return tableID2Type.TryGetValue(id, out ret) ? ret : null;
            }
        }

        public override void Initialize()
        {
            
        }


        #region Register
        public void Register(Assembly methodAssembly)
        {
           // foreach (var msg in Parse(typeof(GameCmd.ClientCommand)))
          //      Register(msg.Key, msg.Value);
        }

        /// <summary>注册可被解析的消息类型</summary>
        /// <typeparam name="T">可被解析的消息类型ID</typeparam>
        /// <param name="messageTypeID"><typeparamref name="T"/>对应的<see cref="ProtoBuf.IExtensible"/>类型</param>
        public void Register<T>(NetMessageType messageTypeID) where T : ProtoBuf.IExtensible
        {
            // 反序列化预编译
            //ProtoBuf.Serializer.PrepareSerializer<T>();

            // 注册
            tableType2ID[typeof(T)] = messageTypeID;
            tableID2Type[messageTypeID] = typeof(T);
        }

        private System.Reflection.MethodInfo methodRegister;
        /// <summary>注册可被解析的消息类型</summary>
        /// <param name="messageTypeID">可被解析的消息类型ID</param>
        /// <param name="messageType"><paramref name="messageTypeID"/>对应的<see cref="ProtoBuf.IExtensible"/>类型</param>
        /// <remarks>对泛型重载Register&lt;T&gt;的非泛型包装</remarks>
        public void Register(NetMessageType messageTypeID, Type messageType)
        {
            if (methodRegister == null)
                methodRegister = this.GetType().GetMethod("Register", new Type[] { typeof(NetMessageType) });
            // Call Register<messageType>(messageTypeID) by refelect.
            this.methodRegister
                .MakeGenericMethod(messageType)
                .Invoke(this, new object[] { messageTypeID });
        }

        /// <summary>
		/// 解析消息号和消息类型对应表
		/// </summary>
		/// <returns></returns>
        private static IEnumerable<KeyValuePair<NetMessageType, Type>> Parse(Type categoryIdType)
        {
            Assembly assembly = null;
            foreach (var assemb in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assemb.GetName().ToString().Contains(NetWork.PROTOBUFFASSEMBLY))
                {
                    assembly = assemb;
                    break;
                }
            }

            var ret = new Dictionary<NetMessageType, Type>(new NetMessageTypeComparer());
            foreach (var cName in Enum.GetNames(categoryIdType))
            {
                var cValue = Convert.ToByte(Enum.Parse(categoryIdType, cName));
                var pre = categoryIdType.Name + "_";
                var name1 = categoryIdType.Namespace + "." + (cName.StartsWith(pre) ? cName.Substring(pre.Length) : cName) + "+Param";
                var typeIdType = assembly.GetType(name1);
                if (typeIdType == null)
                {
                    //UnityEngine.Debug.LogWarning("Can't find type by name: " + name1);
                    continue;
                }
                foreach (var tName in Enum.GetNames(typeIdType))
                {
                    var tValue = Convert.ToByte(Enum.Parse(typeIdType, tName));
                    var name2 = categoryIdType.Namespace + "." + tName;
                    var messageType = assembly.GetType(name2);
                    if (messageType == null)
                    {
                        //UnityEngine.Debug.LogWarning("Can't find type by name: " + name2);
                        continue;
                    }
                    if (ret.ContainsKey(new NetMessageType() { Cmd = cValue, Param = tValue }))
                    {
                        Type mess = ret[new NetMessageType() { Cmd = cValue, Param = tValue }];
                       // Engine.Utility.Log.Error("重复注册 cmd is  " + cValue + " param is " + tValue + " 已经注册过的名字 " + mess.ToString() + " 要注册的是 " + tName + " 检查proto号是否重复");
                    }
                    else
                    {
                        ret.Add(new NetMessageType() { Cmd = cValue, Param = tValue }, messageType);
                    }

                }
            }
            return ret;
        }


#endregion

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var pair in tableType2ID)
            {
                sb.AppendLine(pair.Value + " " + pair.Key);
            }
            return sb.ToString();
        }

        public IEnumerator<KeyValuePair<Type, NetMessageType>> GetEnumerator()
        {
            return tableType2ID.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
