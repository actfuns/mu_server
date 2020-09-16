using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class WangChengMapInfoData
	{
		
		[ProtoMember(1)]
		public long FightingEndTime = 0L;

		
		[ProtoMember(2)]
		public int FightingState = 0;

		
		[ProtoMember(3)]
		public string NextBattleTime = "";

		
		[ProtoMember(4)]
		public string WangZuBHName = "";

		
		[ProtoMember(5)]
		public int WangZuBHid = -1;
	}
}
