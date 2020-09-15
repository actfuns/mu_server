using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace Server.Data
{
	// Token: 0x0200084A RID: 2122
	[ProtoContract]
	public class ZorkBattleRankInfo
	{
		// Token: 0x0400464A RID: 17994
		[ProtoMember(1)]
		public Dictionary<int, List<KFZorkRankInfo>> rankInfo2Client = new Dictionary<int, List<KFZorkRankInfo>>();
	}
}
