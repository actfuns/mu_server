using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200016E RID: 366
	[ProtoContract]
	public class NameRegisterData
	{
		// Token: 0x04000822 RID: 2082
		[ProtoMember(1)]
		public string Name;

		// Token: 0x04000823 RID: 2083
		[ProtoMember(2)]
		public string PingTaiID;

		// Token: 0x04000824 RID: 2084
		[ProtoMember(3)]
		public int ZoneID;

		// Token: 0x04000825 RID: 2085
		[ProtoMember(4)]
		public int NameType;

		// Token: 0x04000826 RID: 2086
		[ProtoMember(5)]
		public string UserID;

		// Token: 0x04000827 RID: 2087
		[ProtoMember(6)]
		public string RegTime;
	}
}
