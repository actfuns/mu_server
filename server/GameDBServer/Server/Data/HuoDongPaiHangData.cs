using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000CC RID: 204
	[ProtoContract]
	public class HuoDongPaiHangData
	{
		// Token: 0x04000595 RID: 1429
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x04000596 RID: 1430
		[ProtoMember(2)]
		public string RoleName;

		// Token: 0x04000597 RID: 1431
		[ProtoMember(3)]
		public int ZoneID;

		// Token: 0x04000598 RID: 1432
		[ProtoMember(4)]
		public int Type;

		// Token: 0x04000599 RID: 1433
		[ProtoMember(5)]
		public int PaiHang;

		// Token: 0x0400059A RID: 1434
		[ProtoMember(6)]
		public string PaiHangTime;

		// Token: 0x0400059B RID: 1435
		[ProtoMember(7)]
		public int PaiHangValue;
	}
}
