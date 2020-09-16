using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class HuangChengMapInfoData
	{
		
		[ProtoMember(1)]
		public long FightingEndTime = 0L;

		
		[ProtoMember(2)]
		public int HuangDiRoleID = 0;

		
		[ProtoMember(3)]
		public string HuangDiRoleName = "";

		
		[ProtoMember(4)]
		public string HuangDiBHName = "";

		
		[ProtoMember(5)]
		public int FightingState = 0;

		
		[ProtoMember(6)]
		public string NextBattleTime = "";

		
		[ProtoMember(7)]
		public int WangZuBHid = -1;
	}
}
