using System;

namespace GameServer.Logic.Today
{
	// Token: 0x0200044C RID: 1100
	public class TodayInfo
	{
		// Token: 0x0600142A RID: 5162 RVA: 0x0013DE80 File Offset: 0x0013C080
		public TodayInfo()
		{
		}

		// Token: 0x0600142B RID: 5163 RVA: 0x0013DEEC File Offset: 0x0013C0EC
		public TodayInfo(TodayInfo info)
		{
			this.Type = info.Type;
			this.ID = info.ID;
			this.Name = info.Name;
			this.FuBenID = info.FuBenID;
			this.HuoDongID = info.HuoDongID;
			this.LevelMin = info.LevelMin;
			this.LevelMax = info.LevelMax;
			this.TaskMin = info.TaskMin;
			this.NumMax = info.NumMax;
			this.NumEnd = info.NumEnd;
			this.AwardInfo = info.AwardInfo;
		}

		// Token: 0x04001DAD RID: 7597
		public int Type = 0;

		// Token: 0x04001DAE RID: 7598
		public int ID = 0;

		// Token: 0x04001DAF RID: 7599
		public string Name = "";

		// Token: 0x04001DB0 RID: 7600
		public int FuBenID = 0;

		// Token: 0x04001DB1 RID: 7601
		public int HuoDongID = 0;

		// Token: 0x04001DB2 RID: 7602
		public int LevelMin = 0;

		// Token: 0x04001DB3 RID: 7603
		public int LevelMax = 0;

		// Token: 0x04001DB4 RID: 7604
		public int TaskMin = 0;

		// Token: 0x04001DB5 RID: 7605
		public int NumMax = 0;

		// Token: 0x04001DB6 RID: 7606
		public int NumEnd = 0;

		// Token: 0x04001DB7 RID: 7607
		public TodayAwardInfo AwardInfo = new TodayAwardInfo();
	}
}
