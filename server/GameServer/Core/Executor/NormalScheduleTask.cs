using System;

namespace GameServer.Core.Executor
{
	// Token: 0x020000C8 RID: 200
	public class NormalScheduleTask : ScheduleTask
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600037E RID: 894 RVA: 0x0003C6F8 File Offset: 0x0003A8F8
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0003C710 File Offset: 0x0003A910
		public NormalScheduleTask(string name, EventHandler timerCallProc)
		{
			this.TimerCallProc = timerCallProc;
			this.Name = name;
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0003C744 File Offset: 0x0003A944
		public void run()
		{
			try
			{
				if (null != this.TimerCallProc)
				{
					this.TimerCallProc(this, EventArgs.Empty);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000381 RID: 897 RVA: 0x0003C790 File Offset: 0x0003A990
		public override string ToString()
		{
			return this.Name;
		}

		// Token: 0x040004D2 RID: 1234
		private TaskInternalLock _InternalLock = new TaskInternalLock();

		// Token: 0x040004D3 RID: 1235
		private EventHandler TimerCallProc = null;

		// Token: 0x040004D4 RID: 1236
		private string Name = null;
	}
}
