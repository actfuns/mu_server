using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000065 RID: 101
	[ProtoContract]
	public class RebornEquipData
	{
		// Token: 0x0400022D RID: 557
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x0400022E RID: 558
		[ProtoMember(2)]
		public int HoleID = 0;

		// Token: 0x0400022F RID: 559
		[ProtoMember(3)]
		public int Level = 0;

		// Token: 0x04000230 RID: 560
		[ProtoMember(4)]
		public int Able = 0;
	}
}
