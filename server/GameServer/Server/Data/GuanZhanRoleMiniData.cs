using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020007AA RID: 1962
	[ProtoContract]
	public class GuanZhanRoleMiniData
	{
		// Token: 0x04003F23 RID: 16163
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04003F24 RID: 16164
		[ProtoMember(2)]
		public string Name;

		// Token: 0x04003F25 RID: 16165
		[ProtoMember(3)]
		public int Level = 0;

		// Token: 0x04003F26 RID: 16166
		[ProtoMember(4)]
		public int ChangeLevel = 0;

		// Token: 0x04003F27 RID: 16167
		[ProtoMember(5)]
		public int Occupation = 0;

		// Token: 0x04003F28 RID: 16168
		[ProtoMember(6)]
		public int RoleSex = 0;

		// Token: 0x04003F29 RID: 16169
		[ProtoMember(7)]
		public int BHZhiWu = 0;

		// Token: 0x04003F2A RID: 16170
		[ProtoMember(8)]
		public int Param1 = 0;

		// Token: 0x04003F2B RID: 16171
		[ProtoMember(9)]
		public int Param2 = 0;
	}
}
