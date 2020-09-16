using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class ZhengDuoScoreInfo
	{
		
		[ProtoMember(1)]
		public List<ZhengDuoScoreData> ScoreRank;

		
		[ProtoMember(2)]
		public int Step;
	}
}
