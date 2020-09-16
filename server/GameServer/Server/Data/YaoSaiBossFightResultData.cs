using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class YaoSaiBossFightResultData
	{
		
		[ProtoMember(1)]
		public int Result;

		
		[ProtoMember(2)]
		public int FightLife;

		
		[ProtoMember(3)]
		public YaoSaiBossData BossInfo;

		
		[ProtoMember(4)]
		public bool NeedNotifyAward;
	}
}
