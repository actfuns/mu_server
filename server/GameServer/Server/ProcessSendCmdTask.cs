using System;
using GameServer.Core.Executor;

namespace GameServer.Server
{
	
	internal class ProcessSendCmdTask : ScheduleTask
	{
		
		
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		
		public ProcessSendCmdTask(TCPSession session)
		{
			this.session = session;
		}

		
		public void run()
		{
		}

		
		private TaskInternalLock _InternalLock = new TaskInternalLock();

		
		private TCPSession session = null;
	}
}
