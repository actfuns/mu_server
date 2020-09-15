using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200011F RID: 287
	[ProtoContract]
	public class BossData
	{
		// Token: 0x0400063A RID: 1594
		[ProtoMember(1)]
		public int MonsterID = 0;

		// Token: 0x0400063B RID: 1595
		[ProtoMember(2)]
		public int ExtensionID = 0;

		// Token: 0x0400063C RID: 1596
		[ProtoMember(3)]
		public string KillMonsterName = "";

		// Token: 0x0400063D RID: 1597
		[ProtoMember(4)]
		public int KillerOnline = 0;

		// Token: 0x0400063E RID: 1598
		[ProtoMember(5)]
		public string NextTime = "";
	}
}
