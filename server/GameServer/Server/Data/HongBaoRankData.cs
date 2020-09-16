using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class HongBaoRankData
	{
		
		[ProtoMember(1)]
		public int type;

		
		[ProtoMember(2)]
		public List<HongBaoRankItemData> items;

		
		[ProtoMember(3)]
		public long flag;
	}
}
