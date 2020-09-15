using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.Today
{
	// Token: 0x0200044D RID: 1101
	public class TodayAwardInfo
	{
		// Token: 0x0600142C RID: 5164 RVA: 0x0013DFDC File Offset: 0x0013C1DC
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

		// Token: 0x04001DB8 RID: 7608
		public double Exp = 0.0;

		// Token: 0x04001DB9 RID: 7609
		public double GoldBind = 0.0;

		// Token: 0x04001DBA RID: 7610
		public double MoJing = 0.0;

		// Token: 0x04001DBB RID: 7611
		public double ChengJiu = 0.0;

		// Token: 0x04001DBC RID: 7612
		public double ShengWang = 0.0;

		// Token: 0x04001DBD RID: 7613
		public double ZhanGong = 0.0;

		// Token: 0x04001DBE RID: 7614
		public double DiamondBind = 0.0;

		// Token: 0x04001DBF RID: 7615
		public double XingHun = 0.0;

		// Token: 0x04001DC0 RID: 7616
		public double YuanSuFenMo = 0.0;

		// Token: 0x04001DC1 RID: 7617
		public double ShouHuDianShu = 0.0;

		// Token: 0x04001DC2 RID: 7618
		public double ZaiZao = 0.0;

		// Token: 0x04001DC3 RID: 7619
		public double LingJing = 0.0;

		// Token: 0x04001DC4 RID: 7620
		public double RongYao = 0.0;

		// Token: 0x04001DC5 RID: 7621
		public double ExtDiamondBind = 0.0;

		// Token: 0x04001DC6 RID: 7622
		public List<GoodsData> GoodsList = new List<GoodsData>();
	}
}
