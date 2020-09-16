using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JieriHongBaoKingData
	{
		
		[ProtoMember(1)]
		public List<JieriHongBaoKingItemData> RankList;

		
		[ProtoMember(2)]
		public long DataAge;
	}
}
