using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000012 RID: 18
	[ProtoContract]
	public class RebornEquipData
	{
		// Token: 0x04000073 RID: 115
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000074 RID: 116
		[ProtoMember(2)]
		public int HoleID = 0;

		// Token: 0x04000075 RID: 117
		[ProtoMember(3)]
		public int Level = 0;

		// Token: 0x04000076 RID: 118
		[ProtoMember(4)]
		public int Able = 0;
	}
}
