using System;
using ProtoBuf;

namespace GameDBServer.Logic.UnionAlly
{
	
	[ProtoContract]
	public class AllyData
	{
		
		[ProtoMember(1)]
		public int UnionID = 0;

		
		[ProtoMember(2)]
		public int UnionZoneID = 0;

		
		[ProtoMember(3)]
		public string UnionName = "";

		
		[ProtoMember(4)]
		public int UnionLevel = 0;

		
		[ProtoMember(5)]
		public int UnionNum = 0;

		
		[ProtoMember(6)]
		public int LeaderID = 0;

		
		[ProtoMember(7)]
		public int LeaderZoneID = 0;

		
		[ProtoMember(8)]
		public string LeaderName = "";

		
		[ProtoMember(9)]
		public DateTime LogTime = DateTime.MinValue;

		
		[ProtoMember(10)]
		public int LogState = 0;
	}
}
