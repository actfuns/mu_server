using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200055C RID: 1372
	[ProtoContract]
	public class HuangChengMapInfoData
	{
		// Token: 0x040024EB RID: 9451
		[ProtoMember(1)]
		public long FightingEndTime = 0L;

		// Token: 0x040024EC RID: 9452
		[ProtoMember(2)]
		public int HuangDiRoleID = 0;

		// Token: 0x040024ED RID: 9453
		[ProtoMember(3)]
		public string HuangDiRoleName = "";

		// Token: 0x040024EE RID: 9454
		[ProtoMember(4)]
		public string HuangDiBHName = "";

		// Token: 0x040024EF RID: 9455
		[ProtoMember(5)]
		public int FightingState = 0;

		// Token: 0x040024F0 RID: 9456
		[ProtoMember(6)]
		public string NextBattleTime = "";

		// Token: 0x040024F1 RID: 9457
		[ProtoMember(7)]
		public int WangZuBHid = -1;
	}
}
