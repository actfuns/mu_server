using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000D5 RID: 213
	[ProtoContract]
	public class ZaJinDanHistory
	{
		// Token: 0x040005D0 RID: 1488
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040005D1 RID: 1489
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x040005D2 RID: 1490
		[ProtoMember(3)]
		public int TimesSelected = 0;

		// Token: 0x040005D3 RID: 1491
		[ProtoMember(4)]
		public int UsedYuanBao = 0;

		// Token: 0x040005D4 RID: 1492
		[ProtoMember(5)]
		public int UsedJinDan = 0;

		// Token: 0x040005D5 RID: 1493
		[ProtoMember(6)]
		public int GainGoodsId = 0;

		// Token: 0x040005D6 RID: 1494
		[ProtoMember(7)]
		public int GainGoodsNum = 0;

		// Token: 0x040005D7 RID: 1495
		[ProtoMember(8)]
		public int GainGold = 0;

		// Token: 0x040005D8 RID: 1496
		[ProtoMember(9)]
		public int GainYinLiang = 0;

		// Token: 0x040005D9 RID: 1497
		[ProtoMember(10)]
		public int GainExp = 0;

		// Token: 0x040005DA RID: 1498
		[ProtoMember(11)]
		public string GoodPorp = "";

		// Token: 0x040005DB RID: 1499
		[ProtoMember(12)]
		public string OperationTime = "";
	}
}
