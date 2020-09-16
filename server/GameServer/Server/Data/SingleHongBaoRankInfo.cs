using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SingleHongBaoRankInfo
	{
		
		[ProtoMember(1)]
		public string roleName;

		
		[ProtoMember(2)]
		public int diamondNum;

		
		[ProtoMember(3)]
		public int zuiJia;

		
		[ProtoMember(4)]
		public DateTime recvTime;
	}
}
