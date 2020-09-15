using System;
using ProtoBuf;

namespace GameServer.Logic.BocaiSys
{
	// Token: 0x02000072 RID: 114
	[ProtoContract]
	public class BoCaiUpdate
	{
		// Token: 0x0400029B RID: 667
		[ProtoMember(1)]
		public int BocaiType;

		// Token: 0x0400029C RID: 668
		[ProtoMember(2)]
		public long DataPeriods;

		// Token: 0x0400029D RID: 669
		[ProtoMember(3)]
		public int Stage;

		// Token: 0x0400029E RID: 670
		[ProtoMember(4)]
		public long OpenTime;

		// Token: 0x0400029F RID: 671
		[ProtoMember(5)]
		public string Value1;
	}
}
