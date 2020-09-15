using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200004B RID: 75
	[ProtoContract]
	public class DJRoleInfoData
	{
		// Token: 0x04000187 RID: 391
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000188 RID: 392
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x04000189 RID: 393
		[ProtoMember(3)]
		public int Level = 0;

		// Token: 0x0400018A RID: 394
		[ProtoMember(4)]
		public int Occupation = 0;

		// Token: 0x0400018B RID: 395
		[ProtoMember(5)]
		public int OnlineState = 0;
	}
}
