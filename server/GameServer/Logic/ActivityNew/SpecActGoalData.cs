using System;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x020001CC RID: 460
	public class SpecActGoalData
	{
		// Token: 0x060005B5 RID: 1461 RVA: 0x00050158 File Offset: 0x0004E358
		public bool IsValid()
		{
			return this.NumOne > 0 || this.NumTwo > 0;
		}

		// Token: 0x04000A2D RID: 2605
		public int NumOne = 0;

		// Token: 0x04000A2E RID: 2606
		public int NumTwo = 0;
	}
}
