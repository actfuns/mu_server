using System;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class ZhengDuoData
	{
		
		[ProtoMember(1)]
		public int Step;

		
		[ProtoMember(2)]
		public int State;

		
		[ProtoMember(3)]
		public int SignUp;

		
		[ProtoMember(4)]
		public string OtherName;

		
		[ProtoMember(5)]
		public int OtherZoneId;

		
		[ProtoMember(6)]
		public int Lose;
	}
}
