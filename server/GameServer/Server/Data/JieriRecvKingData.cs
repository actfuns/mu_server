using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JieriRecvKingData
	{
		
		[ProtoMember(1)]
		public List<JieriRecvKingItemData> RankingList;

		
		[ProtoMember(2)]
		public JieriRecvKingItemData MyData;
	}
}
