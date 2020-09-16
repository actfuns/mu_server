using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class HongBaoRankItemData
	{
		
		[ProtoMember(1)]
		public int rankID;

		
		[ProtoMember(2)]
		public string roleName;

		
		[ProtoMember(3)]
		public int daimondNum;

		
		[ProtoMember(4)]
		public int roleId;
	}
}
