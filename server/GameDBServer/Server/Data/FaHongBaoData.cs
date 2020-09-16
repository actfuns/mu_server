using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FaHongBaoData
	{
		
		[ProtoMember(1)]
		public int roleID;

		
		[ProtoMember(2)]
		public int type;

		
		[ProtoMember(3)]
		public int count;

		
		[ProtoMember(4)]
		public int diamondNum;

		
		[ProtoMember(5)]
		public string message;
	}
}
