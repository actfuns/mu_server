using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000064 RID: 100
	[ProtoContract]
	public class JingLingYuanSuJueXingData
	{
		// Token: 0x0400022B RID: 555
		[ProtoMember(1)]
		public int ActiveType;

		// Token: 0x0400022C RID: 556
		[ProtoMember(2)]
		public int[] ActiveIDs;
	}
}
