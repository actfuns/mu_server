using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000125 RID: 293
	[ProtoContract]
	public class EachRoleChangeName
	{
		// Token: 0x0400065B RID: 1627
		[ProtoMember(1)]
		public int RoleId = 0;

		// Token: 0x0400065C RID: 1628
		[ProtoMember(2)]
		public int LeftFreeTimes = 0;

		// Token: 0x0400065D RID: 1629
		[ProtoMember(3)]
		public int AlreadyZuanShiTimes = 0;
	}
}
