using System;
using System.Collections.Generic;

namespace HSGameEngine.Tools.AStar
{
	// Token: 0x020008E7 RID: 2279
	public class PriorityQueueB<T> : IPriorityQueue<T>
	{
		// Token: 0x060041CB RID: 16843 RVA: 0x003C2912 File Offset: 0x003C0B12
		public PriorityQueueB()
		{
			this.mComparer = Comparer<T>.Default;
		}

		// Token: 0x060041CC RID: 16844 RVA: 0x003C2933 File Offset: 0x003C0B33
		public PriorityQueueB(IComparer<T> comparer)
		{
			this.mComparer = comparer;
		}

		// Token: 0x060041CD RID: 16845 RVA: 0x003C2950 File Offset: 0x003C0B50
		public PriorityQueueB(IComparer<T> comparer, int capacity)
		{
			this.mComparer = comparer;
			this.InnerList.Capacity = capacity;
		}

		// Token: 0x060041CE RID: 16846 RVA: 0x003C297C File Offset: 0x003C0B7C
		protected void SwitchElements(int i, int j)
		{
			T h = this.InnerList[i];
			this.InnerList[i] = this.InnerList[j];
			this.InnerList[j] = h;
		}

		// Token: 0x060041CF RID: 16847 RVA: 0x003C29C0 File Offset: 0x003C0BC0
		protected virtual int OnCompare(int i, int j)
		{
			return this.mComparer.Compare(this.InnerList[i], this.InnerList[j]);
		}

		// Token: 0x060041D0 RID: 16848 RVA: 0x003C29F8 File Offset: 0x003C0BF8
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

		// Token: 0x060041D1 RID: 16849 RVA: 0x003C2A60 File Offset: 0x003C0C60
		public T Pop()
		{
			T result = this.InnerList[0];
			int p = 0;
			this.InnerList[0] = this.InnerList[this.InnerList.Count - 1];
			this.InnerList.RemoveAt(this.InnerList.Count - 1);
			for (;;)
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

		// Token: 0x060041D2 RID: 16850 RVA: 0x003C2B44 File Offset: 0x003C0D44
		public void Update(int i)
		{
			int p2;
			for (int p = i; p != 0; p = p2)
			{
				p2 = (p - 1) / 2;
				if (this.OnCompare(p, p2) >= 0)
				{
					IL_44:
					if (p >= i)
					{
						for (;;)
						{
							int pn = p;
							int p3 = 2 * p + 1;
							p2 = 2 * p + 2;
							if (this.InnerList.Count > p3 && this.OnCompare(p, p3) > 0)
							{
								p = p3;
							}
							if (this.InnerList.Count > p2 && this.OnCompare(p, p2) > 0)
							{
								p = p2;
							}
							if (p == pn)
							{
								break;
							}
							this.SwitchElements(p, pn);
						}
					}
					return;
				}
				this.SwitchElements(p, p2);
			}
			goto IL_44;
		}

		// Token: 0x060041D3 RID: 16851 RVA: 0x003C2C24 File Offset: 0x003C0E24
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

		// Token: 0x060041D4 RID: 16852 RVA: 0x003C2C63 File Offset: 0x003C0E63
		public void Clear()
		{
			this.InnerList.Clear();
		}

		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x060041D5 RID: 16853 RVA: 0x003C2C74 File Offset: 0x003C0E74
		public int Count
		{
			get
			{
				return this.InnerList.Count;
			}
		}

		// Token: 0x060041D6 RID: 16854 RVA: 0x003C2C94 File Offset: 0x003C0E94
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

		// Token: 0x1700065F RID: 1631
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

		// Token: 0x04004FF5 RID: 20469
		protected List<T> InnerList = new List<T>();

		// Token: 0x04004FF6 RID: 20470
		protected IComparer<T> mComparer;
	}
}
