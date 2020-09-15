using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200003B RID: 59
	[ProtoContract]
	public class EachRoleChangeName
	{
		// Token: 0x04000137 RID: 311
		[ProtoMember(1)]
		public int RoleId = 0;

		// Token: 0x04000138 RID: 312
		[ProtoMember(2)]
		public int LeftFreeTimes = 0;

		// Token: 0x04000139 RID: 313
		[ProtoMember(3)]
		public int AlreadyZuanShiTimes = 0;
	}
}
