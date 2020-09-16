using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class YaoSaiBossFightData
	{
		
		[ProtoMember(1)]
		public YaoSaiBossData BossMiniInfo;

		
		[ProtoMember(2)]
		public string JingLingZhenRong;

		
		[ProtoMember(3)]
		public int HaveFightTime;

		
		[ProtoMember(4)]
		public int ZuanShiFightCost;
	}
}
