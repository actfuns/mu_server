using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020001C7 RID: 455
	[ProtoContract]
	public class SevenDayUpdateDbData
	{
		// Token: 0x04000A0E RID: 2574
		[ProtoMember(1)]
		public int RoleId;

		// Token: 0x04000A0F RID: 2575
		[ProtoMember(2)]
		public int ActivityType;

		// Token: 0x04000A10 RID: 2576
		[ProtoMember(3)]
		public int Id;

		// Token: 0x04000A11 RID: 2577
		[ProtoMember(4)]
		public SevenDayItemData Data;
	}
}
