using System;

namespace GameServer.Logic
{
	// Token: 0x0200020C RID: 524
	public class BHMatchQiZhiConfig
	{
		// Token: 0x060006C3 RID: 1731 RVA: 0x000606DC File Offset: 0x0005E8DC
		public object Clone()
		{
			return new BHMatchQiZhiConfig
			{
				NPCID = this.NPCID,
				PosX = this.PosX,
				PosY = this.PosY,
				RebirthSiteX = this.RebirthSiteX,
				RebirthSiteY = this.RebirthSiteY,
				RebirthRadius = this.RebirthRadius,
				ProduceTime = this.ProduceTime,
				ProduceNum = this.ProduceNum
			};
		}

		// Token: 0x04000BB0 RID: 2992
		public int NPCID;

		// Token: 0x04000BB1 RID: 2993
		public int PosX;

		// Token: 0x04000BB2 RID: 2994
		public int PosY;

		// Token: 0x04000BB3 RID: 2995
		public int RebirthSiteX;

		// Token: 0x04000BB4 RID: 2996
		public int RebirthSiteY;

		// Token: 0x04000BB5 RID: 2997
		public int RebirthRadius;

		// Token: 0x04000BB6 RID: 2998
		public int ProduceTime;

		// Token: 0x04000BB7 RID: 2999
		public int ProduceNum;

		// Token: 0x04000BB8 RID: 3000
		public int BattleWhichSide;

		// Token: 0x04000BB9 RID: 3001
		public bool Alive;

		// Token: 0x04000BBA RID: 3002
		public long DeadTicks;

		// Token: 0x04000BBB RID: 3003
		public long OwnTicks;

		// Token: 0x04000BBC RID: 3004
		public long OwnTicksDelta;
	}
}
