//*************************************************************************
//	创建日期:	2016-9-29 14:21
//	文件名称:	StateMachine.cs
//  创 建 人:   	Chengxue.Zhao
//	版权所有:	中青宝
//	说    明:	状态机
//*************************************************************************
using System;
using System.Collections.Generic;

namespace Engine.Utility
{
    public abstract class BaseState
    {
        protected Enum m_stateEnum;
        public virtual void Enter(object param);
        public virtual void Exit();
        public virtual void Update(float dt);
        public virtual void OnEvent(Enum evenid,object param);
        public virtual Enum GetStateEnum() { return m_stateEnum; }
    }

    class StateMachine
    {
    }
}
