using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x02000425 RID: 1061
	[ProtoContract]
	public class ZhengDuoRankList
	{
		// Token: 0x04001C84 RID: 7300
		[ProtoMember(1)]
		public List<ZhengDuoRankData> RankList;

		// Token: 0x04001C85 RID: 7301
		[ProtoMember(2)]
		public int UsedMillisecond;
	}
}
