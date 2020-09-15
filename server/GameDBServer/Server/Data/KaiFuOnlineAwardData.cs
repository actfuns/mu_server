using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200008E RID: 142
	[ProtoContract]
	public class KaiFuOnlineAwardData
	{
		// Token: 0x0400032B RID: 811
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x0400032C RID: 812
		[ProtoMember(2)]
		public int ZoneID = 0;

		// Token: 0x0400032D RID: 813
		[ProtoMember(3)]
		public string RoleName = "";

		// Token: 0x0400032E RID: 814
		[ProtoMember(4)]
		public int DayID = 0;

		// Token: 0x0400032F RID: 815
		[ProtoMember(5)]
		public int TotalRoleNum = 0;
	}
}
