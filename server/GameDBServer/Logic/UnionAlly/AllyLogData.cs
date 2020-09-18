using System;
using ProtoBuf;

namespace GameDBServer.Logic.UnionAlly
{
	
	[ProtoContract]
	public class AllyLogData
	{
		
		[ProtoMember(1)]
		public int UnionID = 0;

		
		[ProtoMember(2)]
		public int UnionZoneID = 0;

		
		[ProtoMember(3)]
		public string UnionName = "";

		
		[ProtoMember(4)]
		public int MyUnionID = 0;

		
		[ProtoMember(5)]
		public DateTime LogTime = DateTime.MinValue;

		
		[ProtoMember(6)]
		public int LogState = 0;
	}
}
