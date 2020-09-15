using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x020001F8 RID: 504
	[ProtoContract]
	public class BianShenUpdateResultData
	{
		// Token: 0x04000B2B RID: 2859
		[ProtoMember(1)]
		public int Type;

		// Token: 0x04000B2C RID: 2860
		[ProtoMember(2)]
		public int BianShen;

		// Token: 0x04000B2D RID: 2861
		[ProtoMember(3)]
		public int Exp;

		// Token: 0x04000B2E RID: 2862
		[ProtoMember(4)]
		public int Auto;

		// Token: 0x04000B2F RID: 2863
		[ProtoMember(5)]
		public int ZuanShi;

		// Token: 0x04000B30 RID: 2864
		[ProtoMember(6)]
		public int Result;
	}
}
