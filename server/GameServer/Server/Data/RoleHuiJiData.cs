using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000018 RID: 24
	[ProtoContract]
	public class RoleHuiJiData
	{
		// Token: 0x04000094 RID: 148
		[ProtoMember(1)]
		public int huiji;

		// Token: 0x04000095 RID: 149
		[ProtoMember(2)]
		public int Exp;
	}
}
