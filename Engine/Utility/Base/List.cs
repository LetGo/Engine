using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Utility
{
    /// <summary>
    /// faster than List when you Add and Remove items
    /// doesn't release the buffer on Clear()
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QuickList<T>
    {
        public T[] buffer = null;

        public int size = 0;

        public int Capacity { get; set; }
        public QuickList()
        {
            
        }

        public QuickList(int capacity)
        {
            Capacity = capacity;
            buffer = new T[capacity];
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (buffer != null)
            {
                for (int i = 0; i < size; i++)
                {
                    yield return buffer[i];
                }
            }
        }

        public T this[int i]
        {
            get { return buffer[i]; }
            set { buffer[i] = value; }
        }

        public void Add(T item)
        {
            CheckAllocate();

            buffer[size++] = item;
        }

        private void CheckAllocate()
        {
            if (buffer == null || size == buffer.Length)
            {
                AllocateMore();
            }
        }

        public void Insert(int index,T item)
        {
            CheckAllocate();

            if (index > -1 && index < size)
            {
                for (int i = size - 1; i < index; i--)
                {
                    buffer[i + 1] = buffer[i];
                }
                buffer[index] = item;
            }
            else
            {
                Add(item);
            }
        }

        public bool Contains(T item)
        {
            if (buffer == null)
            {
                return false;
            }
            for (int i = 0; i < size; i++)
            {
                if (buffer[i].Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        public int IndexOf(T item)
        {
            if (buffer == null)
            {
                return -1;
            }

            for (int i = 0; i < size; i++)
            {
                if (buffer[i].Equals(item))
                {
                    return i;
                }
            }
            return -1;
        }

        public bool Remove(T item)
        {
            if (buffer != null)
            {
                EqualityComparer<T> comp = EqualityComparer<T>.Default;

                for (int i = 0; i < size; ++i)
                {
                    if (comp.Equals(buffer[i], item))
                    {
                        --size;
                        buffer[i] = default(T);
                        for (int b = i; b < size; ++b)
                        {
                            buffer[b] = buffer[b + 1];
                        }
                        buffer[size] = default(T);
                        return true;
                    }
                }
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            if (buffer != null && index > -1 && index < size)
            {
                --size;
                buffer[index] = default(T);
                for (int b = index; b < size; ++b)
                {
                    buffer[b] = buffer[b + 1];
                }
                buffer[size] = default(T);
            }
        }

        public T[] ToArray()
        { 
            Trim(); 
            return buffer; 
        }

        void AllocateMore()
        {
            if (buffer != null)
            {
                Capacity = Math.Max(buffer.Length << 1, 32);
            }
            else
            {
                Capacity = 32;
            }
            T[] newLiset = new T[Capacity];
            if (buffer != null && size > 0)
            {
                buffer.CopyTo(newLiset,0);
            }
            buffer = newLiset;
        }

        void Trim()
        {
            if (size > 0)
            {
                if (size < buffer.Length)
                {
                    T[] newList = new T[size];
                    for (int i = 0; i < size; i++)
                    {
                        newList[i] = buffer[i];
                    }
                    Capacity = size;
                    buffer = newList;
                }
            }
            else
            {
                buffer = null;
            }
        }

        /// <summary>
        /// size 为0 不清空内存
        /// </summary>
        public void Clear()
        {
            size = 0;
        }

        /// <summary>
        /// size 为0 清空内存
        /// </summary>
        public void Release()
        {
            Clear();
            buffer = null;
        }
    }
}
