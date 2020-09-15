using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002D2 RID: 722
	[ProtoContract]
	public class SoulStoneExtFuncItem
	{
		// Token: 0x040012A1 RID: 4769
		[ProtoMember(1)]
		public int FuncType;

		// Token: 0x040012A2 RID: 4770
		[ProtoMember(2)]
		public int CostType;
	}
}
