using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FuBenData
	{
		
		[ProtoMember(1)]
		public int FuBenID;

		
		[ProtoMember(2)]
		public int DayID;

		
		[ProtoMember(3)]
		public int EnterNum;

		
		[ProtoMember(4)]
		public int QuickPassTimer;

		
		[ProtoMember(5)]
		public int FinishNum;
	}
}
