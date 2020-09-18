using System;

namespace GameServer.Core.Executor
{
	
	public class NormalScheduleTask : ScheduleTask
	{
		
		
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		
		public NormalScheduleTask(string name, EventHandler timerCallProc)
		{
			this.TimerCallProc = timerCallProc;
			this.Name = name;
		}

		
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

		
		public override string ToString()
		{
			return this.Name;
		}

		
		private TaskInternalLock _InternalLock = new TaskInternalLock();

		
		private EventHandler TimerCallProc = null;

		
		private string Name = null;
	}
}
