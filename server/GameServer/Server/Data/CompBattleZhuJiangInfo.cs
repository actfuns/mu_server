using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200027F RID: 639
	[ProtoContract]
	public class CompBattleZhuJiangInfo
	{
		// Token: 0x04000FEB RID: 4075
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000FEC RID: 4076
		[ProtoMember(2)]
		public string Name;

		// Token: 0x04000FED RID: 4077
		[ProtoMember(3)]
		public int Level = 0;

		// Token: 0x04000FEE RID: 4078
		[ProtoMember(4)]
		public int ZoneID = 0;

		// Token: 0x04000FEF RID: 4079
		[ProtoMember(5)]
		public int Occupation = 0;

		// Token: 0x04000FF0 RID: 4080
		[ProtoMember(6)]
		public int RoleSex = 0;

		// Token: 0x04000FF1 RID: 4081
		[ProtoMember(7)]
		public int CompZhiWu = 0;
	}
}
