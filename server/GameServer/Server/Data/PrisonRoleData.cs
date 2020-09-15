using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000809 RID: 2057
	[ProtoContract]
	public class PrisonRoleData
	{
		// Token: 0x04004411 RID: 17425
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04004412 RID: 17426
		[ProtoMember(2)]
		public string Name;

		// Token: 0x04004413 RID: 17427
		[ProtoMember(3)]
		public int Level = 0;

		// Token: 0x04004414 RID: 17428
		[ProtoMember(4)]
		public int ChangeLevel = 0;

		// Token: 0x04004415 RID: 17429
		[ProtoMember(5)]
		public int ZoneID = 0;

		// Token: 0x04004416 RID: 17430
		[ProtoMember(6)]
		public int Occupation = 0;

		// Token: 0x04004417 RID: 17431
		[ProtoMember(7)]
		public int RoleSex = 0;

		// Token: 0x04004418 RID: 17432
		[ProtoMember(8)]
		public int CombatForce = 0;
	}
}
