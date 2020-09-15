using System;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x02000039 RID: 57
	public class EveryActGoalData
	{
		// Token: 0x06000081 RID: 129 RVA: 0x00009088 File Offset: 0x00007288
		public bool IsValid()
		{
			return this.NumOne > 0 || this.NumTwo > 0;
		}

		// Token: 0x04000134 RID: 308
		public int NumOne = 0;

		// Token: 0x04000135 RID: 309
		public int NumTwo = 0;
	}
}
