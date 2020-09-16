using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class HongBaoItemData
	{
		
		[ProtoMember(1)]
		public int hongBaoStatus;

		
		[ProtoMember(2)]
		public int hongBaoID;

		
		[ProtoMember(3)]
		public string sender;

		
		[ProtoMember(4)]
		public DateTime beginTime;

		
		[ProtoMember(5)]
		public DateTime endTime;

		
		[ProtoMember(6)]
		public int diamondCount;

		
		[ProtoMember(7)]
		public int diamondSumCount;

		
		[ProtoMember(8)]
		public int type;
	}
}
