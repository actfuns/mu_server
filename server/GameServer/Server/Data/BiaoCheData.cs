using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200011E RID: 286
	[ProtoContract]
	public class BiaoCheData
	{
		// Token: 0x0400062B RID: 1579
		[ProtoMember(1)]
		public int OwnerRoleID = 0;

		// Token: 0x0400062C RID: 1580
		[ProtoMember(2)]
		public int BiaoCheID = 0;

		// Token: 0x0400062D RID: 1581
		[ProtoMember(3)]
		public string BiaoCheName = "";

		// Token: 0x0400062E RID: 1582
		[ProtoMember(4)]
		public int YaBiaoID = 0;

		// Token: 0x0400062F RID: 1583
		[ProtoMember(5)]
		public int MapCode = 0;

		// Token: 0x04000630 RID: 1584
		[ProtoMember(6)]
		public int PosX = 0;

		// Token: 0x04000631 RID: 1585
		[ProtoMember(7)]
		public int PosY = 0;

		// Token: 0x04000632 RID: 1586
		[ProtoMember(8)]
		public int Direction = 0;

		// Token: 0x04000633 RID: 1587
		[ProtoMember(9)]
		public int LifeV = 0;

		// Token: 0x04000634 RID: 1588
		[ProtoMember(10)]
		public int CutLifeV = 0;

		// Token: 0x04000635 RID: 1589
		[ProtoMember(11)]
		public long StartTime = 0L;

		// Token: 0x04000636 RID: 1590
		[ProtoMember(12)]
		public int BodyCode = 0;

		// Token: 0x04000637 RID: 1591
		[ProtoMember(13)]
		public int PicCode = 0;

		// Token: 0x04000638 RID: 1592
		[ProtoMember(14)]
		public int CurrentLifeV = 0;

		// Token: 0x04000639 RID: 1593
		[ProtoMember(15)]
		public string OwnerRoleName = "";
	}
}
