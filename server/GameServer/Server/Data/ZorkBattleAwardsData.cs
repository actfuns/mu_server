using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ZorkBattleAwardsData
	{
		
		[ProtoMember(1)]
		public int Success;

		
		[ProtoMember(2)]
		public int RankNum;

		
		[ProtoMember(3)]
		public int AwardID;

		
		[ProtoMember(4)]
		public int SelfJiFen;

		
		[ProtoMember(5)]
		public int JiFen;

		
		[ProtoMember(6)]
		public int WinToDay;

		
		[ProtoMember(7)]
		public List<AwardsItemData> BossAwardGoodsDataList;
	}
}
