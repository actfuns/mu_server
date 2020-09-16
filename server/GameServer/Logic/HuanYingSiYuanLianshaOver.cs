using System;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class HuanYingSiYuanLianshaOver
	{
		
		[ProtoMember(1)]
		public int KillerZoneID;

		
		[ProtoMember(2)]
		public string KillerName = "";

		
		[ProtoMember(3)]
		public int KillerOccupation;

		
		[ProtoMember(4)]
		public int KillerSide;

		
		[ProtoMember(5)]
		public int KilledZoneID;

		
		[ProtoMember(6)]
		public string KilledName = "";

		
		[ProtoMember(7)]
		public int KilledOccupation;

		
		[ProtoMember(8)]
		public int KilledSide;

		
		[ProtoMember(9)]
		public int ExtScore;
	}
}
