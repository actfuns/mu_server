using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200010F RID: 271
	[ProtoContract]
	public class AchievementRuneBasicData
	{
		// Token: 0x040005AA RID: 1450
		[ProtoMember(1)]
		public int RuneID = 0;

		// Token: 0x040005AB RID: 1451
		[ProtoMember(2)]
		public string RuneName = "";

		// Token: 0x040005AC RID: 1452
		[ProtoMember(3)]
		public int LifeMax = 0;

		// Token: 0x040005AD RID: 1453
		[ProtoMember(4)]
		public int AttackMax = 0;

		// Token: 0x040005AE RID: 1454
		[ProtoMember(5)]
		public int DefenseMax = 0;

		// Token: 0x040005AF RID: 1455
		[ProtoMember(6)]
		public int DodgeMax = 0;

		// Token: 0x040005B0 RID: 1456
		[ProtoMember(7)]
		public int AchievementCost = 0;

		// Token: 0x040005B1 RID: 1457
		[ProtoMember(8)]
		public List<int> RateList;

		// Token: 0x040005B2 RID: 1458
		public List<int[]> AddNumList;
	}
}
