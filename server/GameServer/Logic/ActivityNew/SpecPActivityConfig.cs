using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.ActivityNew
{
	
	public class SpecPActivityConfig
	{
		
		public int TeQuanID;

		
		public int ActID;

		
		public int Param1;

		
		public int Param2;

		
		public int ZhiGouID;

		
		public int PurchaseNum = -1;

		
		public List<GoodsData> GoodsDataListOne = new List<GoodsData>();

		
		public List<GoodsData> GoodsDataListTwo = new List<GoodsData>();

		
		public AwardEffectTimeItem GoodsDataListThr = new AwardEffectTimeItem();

		
		public SpecPActivityType ActType;

		
		public double MultiNum;

		
		public int[] HongBaoRange;

		
		public HashSet<int> ChouJiangTypeSet = new HashSet<int>();
	}
}
