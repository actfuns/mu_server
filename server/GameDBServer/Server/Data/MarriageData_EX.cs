using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000080 RID: 128
	[ProtoContract]
	public class MarriageData_EX
	{
		// Token: 0x040002B8 RID: 696
		[ProtoMember(1)]
		public MarriageData myMarriageData = new MarriageData();

		// Token: 0x040002B9 RID: 697
		[ProtoMember(2)]
		public string roleName = "";

		// Token: 0x040002BA RID: 698
		[ProtoMember(3)]
		public int Occupation = 0;

		// Token: 0x040002BB RID: 699
		[ProtoMember(4)]
		public int nRoleID = 0;
	}
}
