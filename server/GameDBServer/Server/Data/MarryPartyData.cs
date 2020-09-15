using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000081 RID: 129
	[ProtoContract]
	public class MarryPartyData
	{
		// Token: 0x040002BC RID: 700
		[ProtoMember(1)]
		public int RoleID = -1;

		// Token: 0x040002BD RID: 701
		[ProtoMember(2)]
		public int PartyType = -1;

		// Token: 0x040002BE RID: 702
		[ProtoMember(3)]
		public int JoinCount = 0;

		// Token: 0x040002BF RID: 703
		[ProtoMember(4)]
		public long StartTime = -1L;

		// Token: 0x040002C0 RID: 704
		[ProtoMember(5)]
		public string HusbandName = "";

		// Token: 0x040002C1 RID: 705
		[ProtoMember(6)]
		public string WifeName = "";

		// Token: 0x040002C2 RID: 706
		[ProtoMember(7)]
		public int HusbandRoleID = -1;

		// Token: 0x040002C3 RID: 707
		[ProtoMember(8)]
		public int WifeRoleID = -1;
	}
}
