using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class NotifyMsgWithGoodsData
	{
		
		[ProtoMember(1)]
		public int index = 0;

		
		[ProtoMember(2)]
		public int type = 0;

		
		[ProtoMember(3)]
		public List<GoodsData> goodsDataList = null;

		
		[ProtoMember(4)]
		public string param1 = "";

		
		[ProtoMember(5)]
		public Dictionary<int, List<GoodsData>> goodsDic = null;
	}
}
