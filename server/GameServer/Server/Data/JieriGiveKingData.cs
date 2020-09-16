using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JieriGiveKingData
	{
		
		[ProtoMember(1)]
		public List<JieriGiveKingItemData> RankingList;

		
		[ProtoMember(2)]
		public JieriGiveKingItemData MyData;
	}
}
