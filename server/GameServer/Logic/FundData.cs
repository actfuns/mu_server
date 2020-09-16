using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class FundData
	{
		
		[ProtoMember(1, IsRequired = true)]
		public bool IsOpen = false;

		
		[ProtoMember(2, IsRequired = true)]
		public int State = 0;

		
		[ProtoMember(3, IsRequired = true)]
		public int FundType = 0;

		
		[ProtoMember(4, IsRequired = true)]
		public Dictionary<int, FundItem> FundDic = new Dictionary<int, FundItem>();
	}
}
