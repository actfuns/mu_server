using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000DD RID: 221
	[ProtoContract]
	[Serializable]
	public class AgeDataT<T>
	{
		// Token: 0x060001CE RID: 462 RVA: 0x00009744 File Offset: 0x00007944
		public AgeDataT()
		{
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000974F File Offset: 0x0000794F
		public AgeDataT(long age, T v)
		{
			this.Age = age;
			this.V = v;
		}

		// Token: 0x04000614 RID: 1556
		[ProtoMember(1)]
		public long Age;

		// Token: 0x04000615 RID: 1557
		[ProtoMember(2)]
		public T V;
	}
}
