using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LangHunLingYuAwardsData
	{
		
		[ProtoMember(1)]
		public int Success;

		
		[ProtoMember(2)]
		public List<AwardsItemData> AwardsItemDataList;

		
		[ProtoMember(3)]
		public string successBhName;
	}
}
