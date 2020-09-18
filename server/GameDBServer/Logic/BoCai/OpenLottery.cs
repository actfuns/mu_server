using System;
using ProtoBuf;

namespace GameDBServer.Logic.BoCai
{
	
	[ProtoContract]
	public class OpenLottery
	{
		
		[ProtoMember(1)]
		public long DataPeriods;

		
		[ProtoMember(2)]
		public string strWinNum;

		
		[ProtoMember(3)]
		public int BocaiType;

		
		[ProtoMember(4)]
		public long SurplusBalance;

		
		[ProtoMember(5)]
		public long AllBalance;

		
		[ProtoMember(6)]
		public int XiaoHaoDaiBi;

		
		[ProtoMember(7)]
		public string WinInfo;

		
		[ProtoMember(8)]
		public bool IsAward;
	}
}
