using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ShiLianTaAwardsInfoData
	{
		
		[ProtoMember(1)]
		public int CurrentFloorTotalMonsterNum = 0;

		
		[ProtoMember(2)]
		public int CurrentFloorExperienceAward = 0;

		
		[ProtoMember(3)]
		public int NextFloorNeedGoodsID = 0;

		
		[ProtoMember(4)]
		public int NextFloorNeedGoodsNum = 0;

		
		[ProtoMember(5)]
		public int NextFloorExperienceAward = 0;
	}
}
