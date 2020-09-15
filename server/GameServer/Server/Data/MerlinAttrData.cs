using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000167 RID: 359
	[ProtoContract]
	public class MerlinAttrData
	{
		// Token: 0x040007E1 RID: 2017
		[ProtoMember(1)]
		public int _MinAttackV = 0;

		// Token: 0x040007E2 RID: 2018
		[ProtoMember(2)]
		public int _MaxAttackV = 0;

		// Token: 0x040007E3 RID: 2019
		[ProtoMember(3)]
		public int _MinMAttackV = 0;

		// Token: 0x040007E4 RID: 2020
		[ProtoMember(4)]
		public int _MaxMAttackV = 0;

		// Token: 0x040007E5 RID: 2021
		[ProtoMember(5)]
		public int _MinDefenseV = 0;

		// Token: 0x040007E6 RID: 2022
		[ProtoMember(6)]
		public int _MaxDefenseV = 0;

		// Token: 0x040007E7 RID: 2023
		[ProtoMember(7)]
		public int _MinMDefenseV = 0;

		// Token: 0x040007E8 RID: 2024
		[ProtoMember(8)]
		public int _MaxMDefenseV = 0;

		// Token: 0x040007E9 RID: 2025
		[ProtoMember(9)]
		public int _HitV = 0;

		// Token: 0x040007EA RID: 2026
		[ProtoMember(10)]
		public int _DodgeV = 0;

		// Token: 0x040007EB RID: 2027
		[ProtoMember(11)]
		public int _MaxHpV = 0;

		// Token: 0x040007EC RID: 2028
		[ProtoMember(12)]
		public double _ReviveP = 0.0;

		// Token: 0x040007ED RID: 2029
		[ProtoMember(13)]
		public double _MpRecoverP = 0.0;
	}
}
