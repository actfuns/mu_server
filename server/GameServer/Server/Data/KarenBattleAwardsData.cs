using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class KarenBattleAwardsData
	{
		
		[ProtoMember(1)]
		public int Success;

		
		[ProtoMember(2)]
		public int BindJinBi;

		
		[ProtoMember(3)]
		public long Exp;

		
		[ProtoMember(4)]
		public List<GoodsData> AwardGoodsDataList;
	}
}
