using System;
using ProtoBuf;

namespace GameServer.Logic.BocaiSys
{
	
	[ProtoContract]
	public class BoCaiUpdate
	{
		
		[ProtoMember(1)]
		public int BocaiType;

		
		[ProtoMember(2)]
		public long DataPeriods;

		
		[ProtoMember(3)]
		public int Stage;

		
		[ProtoMember(4)]
		public long OpenTime;

		
		[ProtoMember(5)]
		public string Value1;
	}
}
