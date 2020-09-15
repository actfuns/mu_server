using System;

namespace HSGameEngine.Tools.AStar
{
	// Token: 0x020008E6 RID: 2278
	public interface IPriorityQueue<T>
	{
		// Token: 0x060041C7 RID: 16839
		int Push(T item);

		// Token: 0x060041C8 RID: 16840
		T Pop();

		// Token: 0x060041C9 RID: 16841
		T Peek();

		// Token: 0x060041CA RID: 16842
		void Update(int i);
	}
}
