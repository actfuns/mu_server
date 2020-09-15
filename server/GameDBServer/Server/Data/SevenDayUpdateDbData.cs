using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200010C RID: 268
	[ProtoContract]
	public class SevenDayUpdateDbData
	{
		// Token: 0x04000741 RID: 1857
		[ProtoMember(1)]
		public int RoleId;

		// Token: 0x04000742 RID: 1858
		[ProtoMember(2)]
		public int ActivityType;

		// Token: 0x04000743 RID: 1859
		[ProtoMember(3)]
		public int Id;

		// Token: 0x04000744 RID: 1860
		[ProtoMember(4)]
		public SevenDayItemData Data;
	}
}
