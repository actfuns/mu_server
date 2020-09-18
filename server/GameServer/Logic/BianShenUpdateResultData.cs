using System;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class BianShenUpdateResultData
	{
		
		[ProtoMember(1)]
		public int Type;

		
		[ProtoMember(2)]
		public int BianShen;

		
		[ProtoMember(3)]
		public int Exp;

		
		[ProtoMember(4)]
		public int Auto;

		
		[ProtoMember(5)]
		public int ZuanShi;

		
		[ProtoMember(6)]
		public int Result;
	}
}
