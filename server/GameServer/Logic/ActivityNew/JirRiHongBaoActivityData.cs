using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x0200002B RID: 43
	[ProtoContract]
	public class JirRiHongBaoActivityData
	{
		// Token: 0x040000EC RID: 236
		[ProtoMember(1)]
		public List<JirRiHongBaoData> InfoList;

		// Token: 0x040000ED RID: 237
		[ProtoMember(2)]
		public long DataAge;
	}
}
