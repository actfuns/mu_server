using System;
using System.Collections.Generic;
using System.Threading;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class MyTagLock : IDisposable
	{
		
		public MyTagLock(bool create)
		{
		}

		
		public MyTagLock Enter(int tag)
		{
			bool lockTaken = false;
			try
			{
				for (;;)
				{
					Monitor.TryEnter(this, 180000, ref lockTaken);
					if (lockTaken)
					{
						break;
					}
					LogManager.WriteLog(LogTypes.Fatal, "MyTagLock_Timeout#" + string.Join<int>(",", this.ExecutePath), null, true);
				}
				this.ExecutePath.Add(tag);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				throw;
			}
			return this;
		}

		
		private void Leave()
		{
			this.ExecutePath.Clear();
			Monitor.Exit(this);
		}

		
		public void SetTag(int tag)
		{
			this.ExecutePath.Add(tag);
		}

		
		void IDisposable.Dispose()
		{
			this.Leave();
		}

		
		private bool m_disposed = false;

		
		private List<int> ExecutePath = new List<int>();
	}
}
