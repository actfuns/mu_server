using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SaleGoodsData
	{
		
		[ProtoMember(1)]
		public int GoodsDbID = 0;

		
		[ProtoMember(2)]
		public GoodsData SalingGoodsData = null;

		
		[ProtoMember(3)]
		public int RoleID = 0;

		
		[ProtoMember(4)]
		public string RoleName = "";

		
		[ProtoMember(5)]
		public int RoleLevel = 0;
	}
}
