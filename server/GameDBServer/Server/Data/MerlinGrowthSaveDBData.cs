using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000084 RID: 132
	[ProtoContract]
	public class MerlinGrowthSaveDBData
	{
		// Token: 0x040002C9 RID: 713
		[ProtoMember(1)]
		public int _RoleID = 0;

		// Token: 0x040002CA RID: 714
		[ProtoMember(2)]
		public int _Occupation = 0;

		// Token: 0x040002CB RID: 715
		[ProtoMember(3)]
		public int _Level = 0;

		// Token: 0x040002CC RID: 716
		[ProtoMember(4)]
		public int _StarNum = 0;

		// Token: 0x040002CD RID: 717
		[ProtoMember(5)]
		public int _StarExp = 0;

		// Token: 0x040002CE RID: 718
		[ProtoMember(6)]
		public int _LuckyPoint = 0;

		// Token: 0x040002CF RID: 719
		[ProtoMember(7)]
		public long _ToTicks = 0L;

		// Token: 0x040002D0 RID: 720
		[ProtoMember(8)]
		public long _AddTime = 0L;

		// Token: 0x040002D1 RID: 721
		[ProtoMember(9)]
		public Dictionary<int, double> _ActiveAttr = new Dictionary<int, double>();

		// Token: 0x040002D2 RID: 722
		[ProtoMember(10)]
		public Dictionary<int, double> _UnActiveAttr = new Dictionary<int, double>();

		// Token: 0x040002D3 RID: 723
		[ProtoMember(11)]
		public int _LevelUpFailNum = 0;
	}
}
