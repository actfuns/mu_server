using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x020001F3 RID: 499
	[ProtoContract]
	public class ArmorUpdateResultData
	{
		// Token: 0x04000B0D RID: 2829
		[ProtoMember(1)]
		public int Type;

		// Token: 0x04000B0E RID: 2830
		[ProtoMember(2)]
		public int Armor;

		// Token: 0x04000B0F RID: 2831
		[ProtoMember(3)]
		public int Exp;

		// Token: 0x04000B10 RID: 2832
		[ProtoMember(4)]
		public int Auto;

		// Token: 0x04000B11 RID: 2833
		[ProtoMember(5)]
		public int ZuanShi;

		// Token: 0x04000B12 RID: 2834
		[ProtoMember(6)]
		public int Result;
	}
}
