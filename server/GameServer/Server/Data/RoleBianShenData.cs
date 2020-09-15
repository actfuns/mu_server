using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000025 RID: 37
	[ProtoContract]
	public class RoleBianShenData
	{
		// Token: 0x040000D5 RID: 213
		[ProtoMember(1)]
		public int BianShen;

		// Token: 0x040000D6 RID: 214
		[ProtoMember(2)]
		public int Exp;
	}
}
