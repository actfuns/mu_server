using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.AoYunDaTi
{
	
	[ProtoContract]
	public class AoyunDatiMainData
	{
		
		[ProtoMember(1)]
		public List<AoyunPaiHangRoleData> AoyunPaiHangRoleDataArray;

		
		[ProtoMember(2)]
		public DateTime StartTime;

		
		[ProtoMember(3)]
		public DateTime EndTime;

		
		[ProtoMember(4)]
		public int SelfRank;

		
		[ProtoMember(5)]
		public int TianShiNum;

		
		[ProtoMember(6)]
		public int EMoNum;

		
		[ProtoMember(7)]
		public DateTime NextStartTime;

		
		[ProtoMember(8)]
		public bool IsHaveAward;
	}
}
