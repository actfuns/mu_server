using System;
using GameServer.Logic;

namespace Server.Data
{
	
	public class RebornStageInfo
	{
		
		public int ID;

		
		public int[] NeedZhuanSheng;

		
		public int NeedRebornLevel;

		
		public int NeedZhanLi;

		
		public double[] NeedMaxWing;

		
		public int NeedChengJie;

		
		public int NeedShengWang;

		
		public int[] NeedMagicBook;

		
		public int MaxRebornLevel;

		
		public int RebornPoint;

		
		public double[][] extProps = new double[6][];

		
		public AwardsItemList AwardGoods = new AwardsItemList();
	}
}
