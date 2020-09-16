using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class YaoSaiBossMainData
	{
		
		[ProtoMember(1)]
		public YaoSaiBossData BossInfo;

		
		[ProtoMember(2)]
		public int TaoFaCount;

		
		[ProtoMember(3)]
		public int HaveZhaoHuanCount;

		
		[ProtoMember(4)]
		public int ZhaoHuanBossID;

		
		[ProtoMember(5)]
		public int OtherID;

		
		[ProtoMember(6)]
		public int FightCount;
	}
}
