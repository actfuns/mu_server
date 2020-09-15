using System;

namespace GameServer.Logic
{
	// Token: 0x0200031B RID: 795
	public class KarenCenterConfig
	{
		// Token: 0x06000CFD RID: 3325 RVA: 0x000CA4D4 File Offset: 0x000C86D4
		public object Clone()
		{
			return new KarenCenterConfig
			{
				ID = this.ID,
				NPCID = this.NPCID,
				PosX = this.PosX,
				PosY = this.PosY,
				Radius = this.Radius,
				ProduceTime = this.ProduceTime,
				ProduceNum = this.ProduceNum,
				OccupyTime = this.OccupyTime
			};
		}

		// Token: 0x04001485 RID: 5253
		public int ID;

		// Token: 0x04001486 RID: 5254
		public int NPCID;

		// Token: 0x04001487 RID: 5255
		public int PosX;

		// Token: 0x04001488 RID: 5256
		public int PosY;

		// Token: 0x04001489 RID: 5257
		public int Radius;

		// Token: 0x0400148A RID: 5258
		public int ProduceTime;

		// Token: 0x0400148B RID: 5259
		public int ProduceNum;

		// Token: 0x0400148C RID: 5260
		public int OccupyTime;

		// Token: 0x0400148D RID: 5261
		public int BattleWhichSide;

		// Token: 0x0400148E RID: 5262
		public long OwnTicks;

		// Token: 0x0400148F RID: 5263
		public long OwnTicksDelta;

		// Token: 0x04001490 RID: 5264
		public long OwnCalculateSide;

		// Token: 0x04001491 RID: 5265
		public long OwnCalculateTicks;
	}
}
