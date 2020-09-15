using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000078 RID: 120
	[ProtoContract]
	public class KingRoleGetData
	{
		// Token: 0x04000297 RID: 663
		[ProtoMember(1)]
		public int KingType;
	}
}
