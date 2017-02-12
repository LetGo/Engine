using System;

namespace Utility
{
    public class SwitchList<T>
    {
        private QuickList<T> m_lstInCmd = null;
        private QuickList<T> m_lstOutCmd = null;

        public QuickList<T> InCmdList { get { return m_lstInCmd; } }

        public QuickList<T> OutCmdList { get { return m_lstOutCmd; } }

        private object m_lock = new object();
        public SwitchList()
        {
            m_lstInCmd = new QuickList<T>();
            m_lstOutCmd = new QuickList<T>();
        }

        public void Push(T cmd)
        {
            if (cmd == null)
            {
                return;
            }

            lock (m_lock)
            {
                if (m_lstInCmd != null)
                {
                    m_lstInCmd.Add(cmd);
                }
            }
        }

        public void Switch()
        {
            lock (m_lock)
            {
                if (m_lstOutCmd != null)
                {
                    if (m_lstOutCmd.size > 0)
                    {
                        return;
                    }

                    QuickList<T> tempList = m_lstInCmd;
                    m_lstInCmd = m_lstOutCmd;
                    m_lstOutCmd = tempList;
                }
            }
        }
    }


}
