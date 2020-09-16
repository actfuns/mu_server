using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BuildingData
	{
		
		[ProtoMember(1)]
		public int BuildId = 0;

		
		[ProtoMember(2)]
		public int BuildLev = 0;

		
		[ProtoMember(3)]
		public int BuildExp = 0;

		
		[ProtoMember(4)]
		public string BuildTime = null;

		
		[ProtoMember(5)]
		public int TaskID_1 = 0;

		
		[ProtoMember(6)]
		public int TaskID_2 = 0;

		
		[ProtoMember(7)]
		public int TaskID_3 = 0;

		
		[ProtoMember(8)]
		public int TaskID_4 = 0;
	}
}
