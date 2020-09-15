using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000D4 RID: 212
	[ProtoContract]
	public class YueDuChouJiangData
	{
		// Token: 0x040005C8 RID: 1480
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040005C9 RID: 1481
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x040005CA RID: 1482
		[ProtoMember(3)]
		public int GainGoodsId = 0;

		// Token: 0x040005CB RID: 1483
		[ProtoMember(4)]
		public int GainGoodsNum = 0;

		// Token: 0x040005CC RID: 1484
		[ProtoMember(5)]
		public int GainGold = 0;

		// Token: 0x040005CD RID: 1485
		[ProtoMember(6)]
		public int GainYinLiang = 0;

		// Token: 0x040005CE RID: 1486
		[ProtoMember(7)]
		public int GainExp = 0;

		// Token: 0x040005CF RID: 1487
		[ProtoMember(8)]
		public string OperationTime = "";
	}
}
