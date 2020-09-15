using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000568 RID: 1384
	[ProtoContract]
	public class KaiFuOnlineAwardData
	{
		// Token: 0x04002550 RID: 9552
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04002551 RID: 9553
		[ProtoMember(2)]
		public int ZoneID = 0;

		// Token: 0x04002552 RID: 9554
		[ProtoMember(3)]
		public string RoleName = "";

		// Token: 0x04002553 RID: 9555
		[ProtoMember(4)]
		public int DayID = 0;

		// Token: 0x04002554 RID: 9556
		[ProtoMember(5)]
		public int TotalRoleNum = 0;
	}
}
