using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000D7 RID: 215
	[ProtoContract]
	public class ZhengBaPkLogData
	{
		// Token: 0x040005E5 RID: 1509
		[ProtoMember(1)]
		public int Day;

		// Token: 0x040005E6 RID: 1510
		[ProtoMember(2)]
		public int RoleID1;

		// Token: 0x040005E7 RID: 1511
		[ProtoMember(3)]
		public int ZoneID1;

		// Token: 0x040005E8 RID: 1512
		[ProtoMember(4)]
		public string RoleName1;

		// Token: 0x040005E9 RID: 1513
		[ProtoMember(5)]
		public int RoleID2;

		// Token: 0x040005EA RID: 1514
		[ProtoMember(6)]
		public int ZoneID2;

		// Token: 0x040005EB RID: 1515
		[ProtoMember(7)]
		public string RoleName2;

		// Token: 0x040005EC RID: 1516
		[ProtoMember(8)]
		public int PkResult;

		// Token: 0x040005ED RID: 1517
		[ProtoMember(9)]
		public bool UpGrade;

		// Token: 0x040005EE RID: 1518
		[ProtoMember(10)]
		public int Month;

		// Token: 0x040005EF RID: 1519
		[ProtoMember(11)]
		public bool IsMirror1;

		// Token: 0x040005F0 RID: 1520
		[ProtoMember(12)]
		public bool IsMirror2;

		// Token: 0x040005F1 RID: 1521
		[ProtoMember(13)]
		public DateTime StartTime;

		// Token: 0x040005F2 RID: 1522
		[ProtoMember(14)]
		public DateTime EndTime;
	}
}
