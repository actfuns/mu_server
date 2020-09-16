using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.Today
{
	
	public class TodayAwardInfo
	{
		
		public TodayAwardInfo AddAward(TodayAwardInfo info, int count = 1)
		{
			this.Exp += info.Exp * (double)count;
			this.GoldBind += info.GoldBind * (double)count;
			this.MoJing += info.MoJing * (double)count;
			this.ChengJiu += info.ChengJiu * (double)count;
			this.ShengWang += info.ShengWang * (double)count;
			this.ZhanGong += info.ZhanGong * (double)count;
			this.DiamondBind += info.DiamondBind * (double)count;
			this.XingHun += info.XingHun * (double)count;
			this.YuanSuFenMo += info.YuanSuFenMo * (double)count;
			this.ShouHuDianShu += info.ShouHuDianShu * (double)count;
			this.ZaiZao += info.ZaiZao * (double)count;
			this.LingJing += info.LingJing * (double)count;
			this.RongYao += info.RongYao * (double)count;
			this.ExtDiamondBind += info.ExtDiamondBind;
			return this;
		}

		
		public double Exp = 0.0;

		
		public double GoldBind = 0.0;

		
		public double MoJing = 0.0;

		
		public double ChengJiu = 0.0;

		
		public double ShengWang = 0.0;

		
		public double ZhanGong = 0.0;

		
		public double DiamondBind = 0.0;

		
		public double XingHun = 0.0;

		
		public double YuanSuFenMo = 0.0;

		
		public double ShouHuDianShu = 0.0;

		
		public double ZaiZao = 0.0;

		
		public double LingJing = 0.0;

		
		public double RongYao = 0.0;

		
		public double ExtDiamondBind = 0.0;

		
		public List<GoodsData> GoodsList = new List<GoodsData>();
	}
}
