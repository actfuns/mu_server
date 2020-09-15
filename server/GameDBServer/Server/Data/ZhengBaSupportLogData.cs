using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000D8 RID: 216
	[ProtoContract]
	public class ZhengBaSupportLogData
	{
		// Token: 0x040005F3 RID: 1523
		[ProtoMember(1)]
		public int FromRoleId;

		// Token: 0x040005F4 RID: 1524
		[ProtoMember(2)]
		public int FromZoneId;

		// Token: 0x040005F5 RID: 1525
		[ProtoMember(3)]
		public string FromRolename;

		// Token: 0x040005F6 RID: 1526
		[ProtoMember(4)]
		public int SupportType;

		// Token: 0x040005F7 RID: 1527
		[ProtoMember(5)]
		public int ToUnionGroup;

		// Token: 0x040005F8 RID: 1528
		[ProtoMember(6)]
		public int ToGroup;

		// Token: 0x040005F9 RID: 1529
		[ProtoMember(7)]
		public DateTime Time;

		// Token: 0x040005FA RID: 1530
		[ProtoMember(8)]
		public int Month;

		// Token: 0x040005FB RID: 1531
		[ProtoMember(9)]
		public int RankOfDay;

		// Token: 0x040005FC RID: 1532
		[ProtoMember(10)]
		public int FromServerId;
	}
}
