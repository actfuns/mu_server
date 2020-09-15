using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000164 RID: 356
	[ProtoContract]
	public class MarryPartyData
	{
		// Token: 0x040007D4 RID: 2004
		[ProtoMember(1)]
		public int RoleID = -1;

		// Token: 0x040007D5 RID: 2005
		[ProtoMember(2)]
		public int PartyType = -1;

		// Token: 0x040007D6 RID: 2006
		[ProtoMember(3)]
		public int JoinCount = 0;

		// Token: 0x040007D7 RID: 2007
		[ProtoMember(4)]
		public long StartTime = -1L;

		// Token: 0x040007D8 RID: 2008
		[ProtoMember(5)]
		public string HusbandName = "";

		// Token: 0x040007D9 RID: 2009
		[ProtoMember(6)]
		public string WifeName = "";

		// Token: 0x040007DA RID: 2010
		[ProtoMember(7)]
		public int HusbandRoleID = -1;

		// Token: 0x040007DB RID: 2011
		[ProtoMember(8)]
		public int WifeRoleID = -1;
	}
}
