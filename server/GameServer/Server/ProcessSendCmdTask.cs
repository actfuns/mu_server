using System;
using GameServer.Core.Executor;

namespace GameServer.Server
{
	// Token: 0x020008B6 RID: 2230
	internal class ProcessSendCmdTask : ScheduleTask
	{
		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x06003DC8 RID: 15816 RVA: 0x0034C39C File Offset: 0x0034A59C
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		// Token: 0x06003DC9 RID: 15817 RVA: 0x0034C3B4 File Offset: 0x0034A5B4
		public ProcessSendCmdTask(TCPSession session)
		{
			this.session = session;
		}

		// Token: 0x06003DCA RID: 15818 RVA: 0x0034C3D8 File Offset: 0x0034A5D8
		public void run()
		{
		}

		// Token: 0x040047D8 RID: 18392
		private TaskInternalLock _InternalLock = new TaskInternalLock();

		// Token: 0x040047D9 RID: 18393
		private TCPSession session = null;
	}
}
