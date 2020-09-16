using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SaleGoodsData : IComparer<SaleGoodsData>
	{
		
		public int Compare(SaleGoodsData x, SaleGoodsData y)
		{
			return x.GoodsDbID - y.GoodsDbID;
		}

		
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
