using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class YueDuChouJiangData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public string RoleName = "";

		
		[ProtoMember(3)]
		public int GainGoodsId = 0;

		
		[ProtoMember(4)]
		public int GainGoodsNum = 0;

		
		[ProtoMember(5)]
		public int GainGold = 0;

		
		[ProtoMember(6)]
		public int GainYinLiang = 0;

		
		[ProtoMember(7)]
		public int GainExp = 0;

		
		[ProtoMember(8)]
		public string OperationTime = "";
	}
}
