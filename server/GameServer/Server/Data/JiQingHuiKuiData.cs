using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JiQingHuiKuiData
	{
		
		[ProtoMember(1)]
		public List<int> ChongJiQingQuShenZhuangQuota;

		
		[ProtoMember(2)]
		public int ShenZhuangHuiZengQuoto;

		
		[ProtoMember(3)]
		public int XingYunChouJiangCount;

		
		[ProtoMember(4)]
		public int TodayYuanBao;

		
		[ProtoMember(5)]
		public int TodayYuanBaoState;

		
		[ProtoMember(6)]
		public int ChongJiLingQuShenZhuangState;

		
		[ProtoMember(7)]
		public int XingYunChouJiangYuanBao;

		
		[ProtoMember(8)]
		public int ShenZhuangHuiZengState;
	}
}
