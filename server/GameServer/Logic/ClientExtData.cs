using System;

namespace GameServer.Logic
{
	// Token: 0x020004D0 RID: 1232
	public class ClientExtData
	{
		// Token: 0x04002096 RID: 8342
		public int MapCode;

		// Token: 0x04002097 RID: 8343
		public int FromX;

		// Token: 0x04002098 RID: 8344
		public int FromY;

		// Token: 0x04002099 RID: 8345
		public int ToX;

		// Token: 0x0400209A RID: 8346
		public int ToY;

		// Token: 0x0400209B RID: 8347
		public int MaxDistance2;

		// Token: 0x0400209C RID: 8348
		public long ClientCmdTicks;

		// Token: 0x0400209D RID: 8349
		public double MoveSpeed;

		// Token: 0x0400209E RID: 8350
		public bool RunStoryboard;

		// Token: 0x0400209F RID: 8351
		public long CanMoveTicks;

		// Token: 0x040020A0 RID: 8352
		public long StartMoveTicks;

		// Token: 0x040020A1 RID: 8353
		public long EndMoveTicks;

		// Token: 0x040020A2 RID: 8354
		public long ReservedTicks;

		// Token: 0x040020A3 RID: 8355
		public long StopMoveTicks;

		// Token: 0x040020A4 RID: 8356
		public long ServerClientTimeDiffTicks;

		// Token: 0x040020A5 RID: 8357
		public long CalcNum;

		// Token: 0x040020A6 RID: 8358
		public long MinTimeDiff = 2147483647L;

		// Token: 0x040020A7 RID: 8359
		public bool KeepAlive = true;

		// Token: 0x040020A8 RID: 8360
		public long ServerLoginTickCount;

		// Token: 0x040020A9 RID: 8361
		public long ClientLoginClientTicks;
	}
}
