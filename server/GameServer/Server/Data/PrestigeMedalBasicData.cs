using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000177 RID: 375
	[ProtoContract]
	public class PrestigeMedalBasicData
	{
		// Token: 0x04000858 RID: 2136
		[ProtoMember(1)]
		public int MedalID = 0;

		// Token: 0x04000859 RID: 2137
		[ProtoMember(2)]
		public string MedalName = "";

		// Token: 0x0400085A RID: 2138
		[ProtoMember(3)]
		public int LifeMax = 0;

		// Token: 0x0400085B RID: 2139
		[ProtoMember(4)]
		public int AttackMax = 0;

		// Token: 0x0400085C RID: 2140
		[ProtoMember(5)]
		public int DefenseMax = 0;

		// Token: 0x0400085D RID: 2141
		[ProtoMember(6)]
		public int HitMax = 0;

		// Token: 0x0400085E RID: 2142
		[ProtoMember(7)]
		public int PrestigeCost = 0;

		// Token: 0x0400085F RID: 2143
		[ProtoMember(8)]
		public List<int> RateList;

		// Token: 0x04000860 RID: 2144
		public List<int[]> AddNumList;
	}
}
