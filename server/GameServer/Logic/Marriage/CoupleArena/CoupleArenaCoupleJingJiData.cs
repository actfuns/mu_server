using System;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic.Marriage.CoupleArena
{
	// Token: 0x02000362 RID: 866
	[ProtoContract]
	public class CoupleArenaCoupleJingJiData
	{
		// Token: 0x040016E8 RID: 5864
		[ProtoMember(1)]
		public int ManRoleId;

		// Token: 0x040016E9 RID: 5865
		[ProtoMember(2)]
		public int ManZoneId;

		// Token: 0x040016EA RID: 5866
		[ProtoMember(3)]
		public RoleData4Selector ManSelector;

		// Token: 0x040016EB RID: 5867
		[ProtoMember(4)]
		public int WifeRoleId;

		// Token: 0x040016EC RID: 5868
		[ProtoMember(5)]
		public int WifeZoneId;

		// Token: 0x040016ED RID: 5869
		[ProtoMember(6)]
		public RoleData4Selector WifeSelector;

		// Token: 0x040016EE RID: 5870
		[ProtoMember(7)]
		public int TotalFightTimes;

		// Token: 0x040016EF RID: 5871
		[ProtoMember(8)]
		public int WinFightTimes;

		// Token: 0x040016F0 RID: 5872
		[ProtoMember(9)]
		public int LianShengTimes;

		// Token: 0x040016F1 RID: 5873
		[ProtoMember(10)]
		public int DuanWeiType;

		// Token: 0x040016F2 RID: 5874
		[ProtoMember(11)]
		public int DuanWeiLevel;

		// Token: 0x040016F3 RID: 5875
		[ProtoMember(12)]
		public int JiFen;

		// Token: 0x040016F4 RID: 5876
		[ProtoMember(13)]
		public int Rank;

		// Token: 0x040016F5 RID: 5877
		public int IsDivorced;
	}
}
