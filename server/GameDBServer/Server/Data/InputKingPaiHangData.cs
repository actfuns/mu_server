using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000CD RID: 205
	[ProtoContract]
	public class InputKingPaiHangData
	{
		// Token: 0x0400059C RID: 1436
		[ProtoMember(1)]
		public string UserID;

		// Token: 0x0400059D RID: 1437
		[ProtoMember(2)]
		public int PaiHang;

		// Token: 0x0400059E RID: 1438
		[ProtoMember(3)]
		public string PaiHangTime = "";

		// Token: 0x0400059F RID: 1439
		[ProtoMember(4)]
		public int PaiHangValue;

		// Token: 0x040005A0 RID: 1440
		[ProtoMember(5)]
		public string MaxLevelRoleName = "";

		// Token: 0x040005A1 RID: 1441
		[ProtoMember(6)]
		public int MaxLevelRoleZoneID = 1;
	}
}
