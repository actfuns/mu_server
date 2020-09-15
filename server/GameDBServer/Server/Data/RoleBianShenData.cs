using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000037 RID: 55
	[ProtoContract]
	public class RoleBianShenData
	{
		// Token: 0x04000120 RID: 288
		[ProtoMember(1)]
		public int BianShen;

		// Token: 0x04000121 RID: 289
		[ProtoMember(2)]
		public int Exp;
	}
}
