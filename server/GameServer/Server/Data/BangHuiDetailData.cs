using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200011B RID: 283
	[ProtoContract]
	public class BangHuiDetailData
	{
		// Token: 0x0400060B RID: 1547
		[ProtoMember(1)]
		public int BHID = 0;

		// Token: 0x0400060C RID: 1548
		[ProtoMember(2)]
		public string BHName = "";

		// Token: 0x0400060D RID: 1549
		[ProtoMember(3)]
		public int ZoneID = 0;

		// Token: 0x0400060E RID: 1550
		[ProtoMember(4)]
		public int BZRoleID = 0;

		// Token: 0x0400060F RID: 1551
		[ProtoMember(5)]
		public string BZRoleName = "";

		// Token: 0x04000610 RID: 1552
		[ProtoMember(6)]
		public int BZOccupation = 0;

		// Token: 0x04000611 RID: 1553
		[ProtoMember(7)]
		public int TotalNum = 0;

		// Token: 0x04000612 RID: 1554
		[ProtoMember(8)]
		public int TotalLevel = 0;

		// Token: 0x04000613 RID: 1555
		[ProtoMember(9)]
		public string BHBulletin = "";

		// Token: 0x04000614 RID: 1556
		[ProtoMember(10)]
		public string BuildTime = "";

		// Token: 0x04000615 RID: 1557
		[ProtoMember(11)]
		public string QiName = "";

		// Token: 0x04000616 RID: 1558
		[ProtoMember(12)]
		public int QiLevel = 0;

		// Token: 0x04000617 RID: 1559
		[ProtoMember(13)]
		public List<BangHuiMgrItemData> MgrItemList = null;

		// Token: 0x04000618 RID: 1560
		[ProtoMember(14)]
		public int IsVerify = 0;

		// Token: 0x04000619 RID: 1561
		[ProtoMember(15)]
		public int TotalMoney = 0;

		// Token: 0x0400061A RID: 1562
		[ProtoMember(16)]
		public int TodayZhanGongForGold = 0;

		// Token: 0x0400061B RID: 1563
		[ProtoMember(17)]
		public int TodayZhanGongForDiamond = 0;

		// Token: 0x0400061C RID: 1564
		[ProtoMember(18)]
		public int JiTan = 0;

		// Token: 0x0400061D RID: 1565
		[ProtoMember(19)]
		public int JunXie = 0;

		// Token: 0x0400061E RID: 1566
		[ProtoMember(20)]
		public int GuangHuan = 0;

		// Token: 0x0400061F RID: 1567
		[ProtoMember(21)]
		public int CanModNameTimes = 0;

		// Token: 0x04000620 RID: 1568
		[ProtoMember(22)]
		public long TotalCombatForce;
	}
}
