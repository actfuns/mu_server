using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200011A RID: 282
	[ProtoContract]
	public class BangHuiMgrItemData
	{
		// Token: 0x04000603 RID: 1539
		[ProtoMember(1)]
		public int ZoneID = 0;

		// Token: 0x04000604 RID: 1540
		[ProtoMember(2)]
		public int RoleID = 0;

		// Token: 0x04000605 RID: 1541
		[ProtoMember(3)]
		public string RoleName = "";

		// Token: 0x04000606 RID: 1542
		[ProtoMember(4)]
		public int Occupation = 0;

		// Token: 0x04000607 RID: 1543
		[ProtoMember(5)]
		public int BHZhiwu = 0;

		// Token: 0x04000608 RID: 1544
		[ProtoMember(6)]
		public string ChengHao = "";

		// Token: 0x04000609 RID: 1545
		[ProtoMember(7)]
		public int BangGong = 0;

		// Token: 0x0400060A RID: 1546
		[ProtoMember(8)]
		public int Level = 0;
	}
}
