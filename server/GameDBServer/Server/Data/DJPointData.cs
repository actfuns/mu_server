using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000049 RID: 73
	[ProtoContract]
	public class DJPointData
	{
		// Token: 0x04000179 RID: 377
		[ProtoMember(1)]
		public int DbID = 0;

		// Token: 0x0400017A RID: 378
		[ProtoMember(2)]
		public int RoleID = 0;

		// Token: 0x0400017B RID: 379
		[ProtoMember(3)]
		public int DJPoint = 0;

		// Token: 0x0400017C RID: 380
		[ProtoMember(4)]
		public int Total = 0;

		// Token: 0x0400017D RID: 381
		[ProtoMember(5)]
		public int Wincnt = 0;

		// Token: 0x0400017E RID: 382
		[ProtoMember(6)]
		public int Yestoday = 0;

		// Token: 0x0400017F RID: 383
		[ProtoMember(7)]
		public int Lastweek = 0;

		// Token: 0x04000180 RID: 384
		[ProtoMember(8)]
		public int Lastmonth = 0;

		// Token: 0x04000181 RID: 385
		[ProtoMember(9)]
		public int Dayupdown = 0;

		// Token: 0x04000182 RID: 386
		[ProtoMember(10)]
		public int Weekupdown = 0;

		// Token: 0x04000183 RID: 387
		[ProtoMember(11)]
		public int Monthupdown = 0;

		// Token: 0x04000184 RID: 388
		[ProtoMember(12)]
		public DJRoleInfoData djRoleInfoData = null;
	}
}
