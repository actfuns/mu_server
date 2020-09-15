using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020007A8 RID: 1960
	[ProtoContract]
	public class GVoicePriorityData
	{
		// Token: 0x04003F1E RID: 16158
		[ProtoMember(5)]
		public string RoleIdList;

		// Token: 0x04003F1F RID: 16159
		[ProtoMember(6)]
		public int Type;

		// Token: 0x04003F20 RID: 16160
		[ProtoMember(7)]
		public int ID;
	}
}
