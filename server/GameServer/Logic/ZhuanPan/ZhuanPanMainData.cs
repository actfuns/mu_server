using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.ZhuanPan
{
	
	[ProtoContract]
	public class ZhuanPanMainData
	{
		
		[ProtoMember(1)]
		public List<ZhuanPanItem> ZhuanPanAwardItemList;

		
		[ProtoMember(2)]
		public DateTime FreeTime;

		
		[ProtoMember(3)]
		public int LeftFuLiCount;

		
		[ProtoMember(4)]
		public int[] ZhuanPanCostArray;

		
		[ProtoMember(5)]
		public ZhuanPanItem GoodsAward;

		
		[ProtoMember(6)]
		public List<ZhuanPanGongGaoData> GongGaoList;
	}
}
