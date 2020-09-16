using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FluorescentGemData
	{
		
		[ProtoMember(1)]
		public Dictionary<int, Dictionary<int, GoodsData>> GemEquipDict = new Dictionary<int, Dictionary<int, GoodsData>>();

		
		[ProtoMember(2)]
		public Dictionary<int, GoodsData> GemBagDict = new Dictionary<int, GoodsData>();

		
		[ProtoMember(10)]
		public List<GoodsData> GemBagList = new List<GoodsData>();

		
		[ProtoMember(11)]
		public List<GoodsData> GemEquipList = new List<GoodsData>();
	}
}
