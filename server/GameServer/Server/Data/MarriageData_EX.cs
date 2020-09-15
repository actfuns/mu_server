using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000163 RID: 355
	[ProtoContract]
	public class MarriageData_EX
	{
		// Token: 0x040007D1 RID: 2001
		[ProtoMember(1)]
		public MarriageData myMarriageData = new MarriageData();

		// Token: 0x040007D2 RID: 2002
		[ProtoMember(2)]
		public string roleName = "";

		// Token: 0x040007D3 RID: 2003
		[ProtoMember(3)]
		public int Occupation = 0;
	}
}
