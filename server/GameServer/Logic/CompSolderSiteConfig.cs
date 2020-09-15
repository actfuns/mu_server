using System;

namespace GameServer.Logic
{
	// Token: 0x02000266 RID: 614
	public class CompSolderSiteConfig
	{
		// Token: 0x060008AA RID: 2218 RVA: 0x00086B10 File Offset: 0x00084D10
		public object Clone()
		{
			return new CompSolderSiteConfig
			{
				ID = this.ID,
				PosX = this.PosX,
				PosY = this.PosY,
				RefreshTimeBegin = this.RefreshTimeBegin,
				RefreshTimeEnd = this.RefreshTimeEnd
			};
		}

		// Token: 0x04000F2A RID: 3882
		public int ID;

		// Token: 0x04000F2B RID: 3883
		public int PosX;

		// Token: 0x04000F2C RID: 3884
		public int PosY;

		// Token: 0x04000F2D RID: 3885
		public TimeSpan RefreshTimeBegin;

		// Token: 0x04000F2E RID: 3886
		public TimeSpan RefreshTimeEnd;

		// Token: 0x04000F2F RID: 3887
		public int MonsterState;

		// Token: 0x04000F30 RID: 3888
		public CompSolderConfig SolderConfig;
	}
}
