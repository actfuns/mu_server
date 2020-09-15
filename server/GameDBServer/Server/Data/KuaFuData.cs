using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000DE RID: 222
	[ProtoContract]
	[Serializable]
	public class KuaFuData<T> where T : new()
	{
		// Token: 0x060001D0 RID: 464 RVA: 0x00009768 File Offset: 0x00007968
		public KuaFuData()
		{
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x000097A8 File Offset: 0x000079A8
		public KuaFuData(long age, T v)
		{
			this.Age = age;
			this.V = v;
		}

		// Token: 0x04000616 RID: 1558
		[ProtoMember(1)]
		public long Age;

		// Token: 0x04000617 RID: 1559
		[ProtoMember(2)]
		public T V = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
	}
}
