using System;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200074A RID: 1866
	public class TimedActionItem
	{
		// Token: 0x06002EFB RID: 12027 RVA: 0x002A0FC0 File Offset: 0x0029F1C0
		public void Exec(long execTicks)
		{
			try
			{
				this.ActionProc(execTicks, this.ContextObject);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x04003C8E RID: 15502
		public long Id;

		// Token: 0x04003C8F RID: 15503
		public long NextTicks;

		// Token: 0x04003C90 RID: 15504
		public int State;

		// Token: 0x04003C91 RID: 15505
		public int ExecCount;

		// Token: 0x04003C92 RID: 15506
		public long StartTicks;

		// Token: 0x04003C93 RID: 15507
		public long PeriodTicks;

		// Token: 0x04003C94 RID: 15508
		public long EndTicks;

		// Token: 0x04003C95 RID: 15509
		public int Type;

		// Token: 0x04003C96 RID: 15510
		public Action<long, object> ActionProc;

		// Token: 0x04003C97 RID: 15511
		public object ContextObject;
	}
}
