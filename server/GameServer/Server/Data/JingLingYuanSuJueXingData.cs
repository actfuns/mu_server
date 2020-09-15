using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000024 RID: 36
	[ProtoContract]
	public class JingLingYuanSuJueXingData
	{
		// Token: 0x040000D3 RID: 211
		[ProtoMember(1)]
		public int ActiveType;

		// Token: 0x040000D4 RID: 212
		[ProtoMember(2)]
		public int[] ActiveIDs;
	}
}
