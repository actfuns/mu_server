using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000168 RID: 360
	[ProtoContract]
	public class MerlinGrowthSaveDBData
	{
		// Token: 0x040007EE RID: 2030
		[ProtoMember(1)]
		public int _RoleID = 0;

		// Token: 0x040007EF RID: 2031
		[ProtoMember(2)]
		public int _Occupation = 0;

		// Token: 0x040007F0 RID: 2032
		[ProtoMember(3)]
		public int _Level = 0;

		// Token: 0x040007F1 RID: 2033
		[ProtoMember(4)]
		public int _StarNum = 0;

		// Token: 0x040007F2 RID: 2034
		[ProtoMember(5)]
		public int _StarExp = 0;

		// Token: 0x040007F3 RID: 2035
		[ProtoMember(6)]
		public int _LuckyPoint = 0;

		// Token: 0x040007F4 RID: 2036
		[ProtoMember(7)]
		public long _ToTicks = 0L;

		// Token: 0x040007F5 RID: 2037
		[ProtoMember(8)]
		public long _AddTime = 0L;

		// Token: 0x040007F6 RID: 2038
		[ProtoMember(9)]
		public Dictionary<int, double> _ActiveAttr = new Dictionary<int, double>();

		// Token: 0x040007F7 RID: 2039
		[ProtoMember(10)]
		public Dictionary<int, double> _UnActiveAttr = new Dictionary<int, double>();

		// Token: 0x040007F8 RID: 2040
		[ProtoMember(11)]
		public int _LevelUpFailNum = 0;
	}
}
