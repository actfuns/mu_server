using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x020001E9 RID: 489
	[ProtoContract]
	public class HuiJiUpdateResultData
	{
		// Token: 0x04000AC8 RID: 2760
		[ProtoMember(1)]
		public int Type;

		// Token: 0x04000AC9 RID: 2761
		[ProtoMember(2)]
		public int HuiJi;

		// Token: 0x04000ACA RID: 2762
		[ProtoMember(3)]
		public int Exp;

		// Token: 0x04000ACB RID: 2763
		[ProtoMember(4)]
		public int Auto;

		// Token: 0x04000ACC RID: 2764
		[ProtoMember(5)]
		public int ZuanShi;

		// Token: 0x04000ACD RID: 2765
		[ProtoMember(6)]
		public int Result;
	}
}
