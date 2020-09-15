using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200043D RID: 1085
	[ProtoContract]
	public class BHAttackLog
	{
		// Token: 0x04001D3F RID: 7487
		[ProtoMember(1)]
		public int BHID;

		// Token: 0x04001D40 RID: 7488
		[ProtoMember(2)]
		public string BHName;

		// Token: 0x04001D41 RID: 7489
		[ProtoMember(3)]
		public long BHInjure;

		// Token: 0x04001D42 RID: 7490
		public Dictionary<int, long> RoleInjure;
	}
}
