using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x020007BC RID: 1980
	[ProtoContract]
	public class CopyWolfAwardsData
	{
		// Token: 0x04003F7C RID: 16252
		[ProtoMember(1)]
		public long Exp;

		// Token: 0x04003F7D RID: 16253
		[ProtoMember(2)]
		public int Money;

		// Token: 0x04003F7E RID: 16254
		[ProtoMember(3)]
		public int WolfMoney;

		// Token: 0x04003F7F RID: 16255
		[ProtoMember(4)]
		public int Wave;

		// Token: 0x04003F80 RID: 16256
		[ProtoMember(5)]
		public int RoleScore = 0;
	}
}
