using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000148 RID: 328
	[ProtoContract]
	public class InputKingPaiHangData_Old
	{
		// Token: 0x04000759 RID: 1881
		[ProtoMember(1)]
		public string UserID;

		// Token: 0x0400075A RID: 1882
		[ProtoMember(2)]
		public int PaiHang;

		// Token: 0x0400075B RID: 1883
		[ProtoMember(3)]
		public string PaiHangTime = "";

		// Token: 0x0400075C RID: 1884
		[ProtoMember(4)]
		public int PaiHangValue;

		// Token: 0x0400075D RID: 1885
		[ProtoMember(5)]
		public string MaxLevelRoleName = "";

		// Token: 0x0400075E RID: 1886
		[ProtoMember(6)]
		public int MaxLevelRoleZoneID = 1;
	}
}
