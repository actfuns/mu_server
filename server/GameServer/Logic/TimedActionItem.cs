using System;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class TimedActionItem
	{
		
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

		
		public long Id;

		
		public long NextTicks;

		
		public int State;

		
		public int ExecCount;

		
		public long StartTicks;

		
		public long PeriodTicks;

		
		public long EndTicks;

		
		public int Type;

		
		public Action<long, object> ActionProc;

		
		public object ContextObject;
	}
}
