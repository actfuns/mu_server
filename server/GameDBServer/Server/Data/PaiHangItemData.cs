using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000094 RID: 148
	[ProtoContract]
	public class PaiHangItemData
	{
		// Token: 0x0400035A RID: 858
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x0400035B RID: 859
		[ProtoMember(2)]
		public string RoleName;

		// Token: 0x0400035C RID: 860
		[ProtoMember(3)]
		public int Val1;

		// Token: 0x0400035D RID: 861
		[ProtoMember(4)]
		public int Val2;

		// Token: 0x0400035E RID: 862
		[ProtoMember(5)]
		public int Val3;

		// Token: 0x0400035F RID: 863
		[ProtoMember(6)]
		public string uid;
	}
}
