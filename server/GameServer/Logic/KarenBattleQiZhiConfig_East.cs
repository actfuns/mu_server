using System;

namespace GameServer.Logic
{
	// Token: 0x0200031D RID: 797
	public class KarenBattleQiZhiConfig_East
	{
		// Token: 0x06000D00 RID: 3328 RVA: 0x000CA560 File Offset: 0x000C8760
		public object Clone()
		{
			return new KarenBattleQiZhiConfig_East
			{
				ID = this.ID,
				MonsterID = this.MonsterID,
				PosX = this.PosX,
				PosY = this.PosY,
				BeginTime = this.BeginTime,
				RefreshCD = this.RefreshCD,
				CollectTime = this.CollectTime,
				HandInNum = this.HandInNum,
				BuffGoodsID = this.BuffGoodsID
			};
		}

		// Token: 0x04001496 RID: 5270
		public int ID;

		// Token: 0x04001497 RID: 5271
		public int MonsterID;

		// Token: 0x04001498 RID: 5272
		public int PosX;

		// Token: 0x04001499 RID: 5273
		public int PosY;

		// Token: 0x0400149A RID: 5274
		public int BeginTime;

		// Token: 0x0400149B RID: 5275
		public int RefreshCD;

		// Token: 0x0400149C RID: 5276
		public int CollectTime;

		// Token: 0x0400149D RID: 5277
		public int HandInNum;

		// Token: 0x0400149E RID: 5278
		public int HoldTme;

		// Token: 0x0400149F RID: 5279
		public int BuffGoodsID;

		// Token: 0x040014A0 RID: 5280
		public int BattleWhichSide;

		// Token: 0x040014A1 RID: 5281
		public bool Alive;

		// Token: 0x040014A2 RID: 5282
		public long DeadTicks;
	}
}
