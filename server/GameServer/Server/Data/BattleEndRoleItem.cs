using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200011D RID: 285
	[ProtoContract]
	public class BattleEndRoleItem
	{
		// Token: 0x04000628 RID: 1576
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000629 RID: 1577
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x0400062A RID: 1578
		[ProtoMember(3)]
		public int KilledNum = 0;
	}
}
