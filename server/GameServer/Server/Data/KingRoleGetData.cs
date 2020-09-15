using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000153 RID: 339
	[ProtoContract]
	public class KingRoleGetData
	{
		// Token: 0x04000783 RID: 1923
		[ProtoMember(1)]
		public int KingType;
	}
}
