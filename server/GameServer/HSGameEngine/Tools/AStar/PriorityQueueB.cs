using System;
using System.Collections.Generic;

namespace HSGameEngine.Tools.AStar
{
    
    public class PriorityQueueB<T> : IPriorityQueue<T>
    {
        
        public PriorityQueueB()
        {
            this.mComparer = Comparer<T>.Default;
        }

        
        public PriorityQueueB(IComparer<T> comparer)
        {
            this.mComparer = comparer;
        }

        
        public PriorityQueueB(IComparer<T> comparer, int capacity)
        {
            this.mComparer = comparer;
            this.InnerList.Capacity = capacity;
        }

        
        protected void SwitchElements(int i, int j)
        {
            T h = this.InnerList[i];
            this.InnerList[i] = this.InnerList[j];
            this.InnerList[j] = h;
        }

        
        protected virtual int OnCompare(int i, int j)
        {
            return this.mComparer.Compare(this.InnerList[i], this.InnerList[j]);
        }

        
        public int Push(T item)
        {
            int p = this.InnerList.Count;
            this.InnerList.Add(item);
            while (p != 0)
            {
                int p2 = (p - 1) / 2;
                if (this.OnCompare(p, p2) >= 0)
                {
                    return p;
                }
                this.SwitchElements(p, p2);
                p = p2;
            }
            return p;
        }

        
        public T Pop()
        {
            T result = this.InnerList[0];
            int p = 0;
            this.InnerList[0] = this.InnerList[this.InnerList.Count - 1];
            this.InnerList.RemoveAt(this.InnerList.Count - 1);
            for (; ; )
            {
                int pn = p;
                int p2 = 2 * p + 1;
                int p3 = 2 * p + 2;
                if (this.InnerList.Count > p2 && this.OnCompare(p, p2) > 0)
                {
                    p = p2;
                }
                if (this.InnerList.Count > p3 && this.OnCompare(p, p3) > 0)
                {
                    p = p3;
                }
                if (p == pn)
                {
                    break;
                }
                this.SwitchElements(p, pn);
            }
            return result;
        }

        
        public void Update(int i)
        {
            int num = i;
            while (true)
            {
                int num4;
                bool flag;
                if (num != 0)
                {
                    num4 = (num - 1) / 2;
                    if (this.OnCompare(num, num4) < 0)
                    {
                        this.SwitchElements(num, num4);
                        num = num4;
                        flag = true;
                        continue;
                    }
                }
                if (num >= i)
                {
                    while (true)
                    {
                        int j = num;
                        int num3 = (2 * num) + 1;
                        num4 = (2 * num) + 2;
                        if ((this.InnerList.Count > num3) && (this.OnCompare(num, num3) > 0))
                        {
                            num = num3;
                        }
                        if ((this.InnerList.Count > num4) && (this.OnCompare(num, num4) > 0))
                        {
                            num = num4;
                        }
                        if (num == j)
                        {
                            break;
                        }
                        this.SwitchElements(num, j);
                        flag = true;
                    }
                }
                return;
            }
        }

        
        public T Peek()
        {
            T result;
            if (this.InnerList.Count > 0)
            {
                result = this.InnerList[0];
            }
            else
            {
                result = default(T);
            }
            return result;
        }

        
        public void Clear()
        {
            this.InnerList.Clear();
        }

        
        
        public int Count
        {
            get
            {
                return this.InnerList.Count;
            }
        }

        
        public void RemoveLocation(T item)
        {
            int index = -1;
            for (int i = 0; i < this.InnerList.Count; i++)
            {
                if (this.mComparer.Compare(this.InnerList[i], item) == 0)
                {
                    index = i;
                }
            }
            if (index != -1)
            {
                this.InnerList.RemoveAt(index);
            }
        }

        
        public T this[int index]
        {
            get
            {
                return this.InnerList[index];
            }
            set
            {
                this.InnerList[index] = value;
                this.Update(index);
            }
        }

        
        protected List<T> InnerList = new List<T>();

        
        protected IComparer<T> mComparer;
    }
}
