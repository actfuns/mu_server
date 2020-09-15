using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200006C RID: 108
	[ProtoContract]
	public class RoleHuiJiData
	{
		// Token: 0x0400024D RID: 589
		[ProtoMember(1)]
		public int huiji;

		// Token: 0x0400024E RID: 590
		[ProtoMember(2)]
		public int Exp;
	}
}
