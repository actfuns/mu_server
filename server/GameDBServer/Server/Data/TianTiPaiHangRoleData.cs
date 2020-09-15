using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000C3 RID: 195
	[ProtoContract]
	public class TianTiPaiHangRoleData
	{
		// Token: 0x04000541 RID: 1345
		[ProtoMember(1)]
		public int RoleId;

		// Token: 0x04000542 RID: 1346
		[ProtoMember(2)]
		public string RoleName;

		// Token: 0x04000543 RID: 1347
		[ProtoMember(3)]
		public int Occupation;

		// Token: 0x04000544 RID: 1348
		[ProtoMember(4)]
		public int ZoneId;

		// Token: 0x04000545 RID: 1349
		[ProtoMember(5)]
		public int ZhanLi;

		// Token: 0x04000546 RID: 1350
		[ProtoMember(6)]
		public int DuanWeiId;

		// Token: 0x04000547 RID: 1351
		[ProtoMember(7)]
		public int DuanWeiJiFen;

		// Token: 0x04000548 RID: 1352
		[ProtoMember(8)]
		public int DuanWeiRank;

		// Token: 0x04000549 RID: 1353
		[ProtoMember(9)]
		public RoleData4Selector RoleData4Selector;
	}
}
