using System;
using ProtoBuf;

namespace GameServer.Logic.Olympics
{
	
	[ProtoContract]
	public class OlympicsGuessDataDB
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public int DayID = 0;

		
		[ProtoMember(3)]
		public int A1 = -1;

		
		[ProtoMember(4)]
		public int A2 = -1;

		
		[ProtoMember(5)]
		public int A3 = -1;

		
		[ProtoMember(6)]
		public int Award1 = 0;

		
		[ProtoMember(7)]
		public int Award2 = 0;

		
		[ProtoMember(8)]
		public int Award3 = 0;
	}
}
