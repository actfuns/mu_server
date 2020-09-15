using System;

namespace GameServer.Logic
{
	// Token: 0x0200031E RID: 798
	public class KarenBattleQiZhiConfig_West
	{
		// Token: 0x06000D02 RID: 3330 RVA: 0x000CA5F0 File Offset: 0x000C87F0
		public object Clone()
		{
			return new KarenBattleQiZhiConfig_West
			{
				ID = this.ID,
				QiZhiID = this.QiZhiID,
				QiZuoID = this.QiZuoID,
				PosX = this.PosX,
				PosY = this.PosY,
				BirthX = this.BirthX,
				BirthY = this.BirthY,
				BirthRadius = this.BirthRadius,
				ProduceTime = this.ProduceTime,
				ProduceNum = this.ProduceNum
			};
		}

		// Token: 0x040014A3 RID: 5283
		public int ID;

		// Token: 0x040014A4 RID: 5284
		public int QiZhiID;

		// Token: 0x040014A5 RID: 5285
		public int QiZuoID;

		// Token: 0x040014A6 RID: 5286
		public int PosX;

		// Token: 0x040014A7 RID: 5287
		public int PosY;

		// Token: 0x040014A8 RID: 5288
		public int BirthX;

		// Token: 0x040014A9 RID: 5289
		public int BirthY;

		// Token: 0x040014AA RID: 5290
		public int BirthRadius;

		// Token: 0x040014AB RID: 5291
		public int ProduceTime;

		// Token: 0x040014AC RID: 5292
		public int ProduceNum;

		// Token: 0x040014AD RID: 5293
		public int BattleWhichSide;

		// Token: 0x040014AE RID: 5294
		public bool Alive;

		// Token: 0x040014AF RID: 5295
		public long DeadTicks;

		// Token: 0x040014B0 RID: 5296
		public long OwnTicks;

		// Token: 0x040014B1 RID: 5297
		public long OwnTicksDelta;
	}
}
