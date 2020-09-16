using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace Server.Data
{
	
	[ProtoContract]
	public class ZorkBattleRankInfo
	{
		
		[ProtoMember(1)]
		public Dictionary<int, List<KFZorkRankInfo>> rankInfo2Client = new Dictionary<int, List<KFZorkRankInfo>>();
	}
}
