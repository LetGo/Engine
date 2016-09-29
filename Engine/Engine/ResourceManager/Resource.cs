using System;
using System.Collections.Generic;

namespace Engine
{
    interface IRefObj
    {
        void Retain();

        void Release();
    }

    public abstract class Resource: IRefObj
    {

        private int m_nRef = 1;
        //
        public void Retain()
        {
            m_nRef++;
        }
        public void Release()
        {
            m_nRef--;
            if (m_nRef == 0)
            {
               
            }
        }
    }
}
