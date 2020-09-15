using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x02000426 RID: 1062
	[ProtoContract]
	public class ZhengDuoScoreInfo
	{
		// Token: 0x04001C86 RID: 7302
		[ProtoMember(1)]
		public List<ZhengDuoScoreData> ScoreRank;

		// Token: 0x04001C87 RID: 7303
		[ProtoMember(2)]
		public int Step;
	}
}
