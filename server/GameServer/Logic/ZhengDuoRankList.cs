using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class ZhengDuoRankList
	{
		
		[ProtoMember(1)]
		public List<ZhengDuoRankData> RankList;

		
		[ProtoMember(2)]
		public int UsedMillisecond;
	}
}
