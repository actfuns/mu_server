using System;
using System.Collections.Generic;
using System.Threading;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200024B RID: 587
	public class MyTagLock : IDisposable
	{
		// Token: 0x0600081C RID: 2076 RVA: 0x0007AFFD File Offset: 0x000791FD
		public MyTagLock(bool create)
		{
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x0007B01C File Offset: 0x0007921C
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

		// Token: 0x0600081E RID: 2078 RVA: 0x0007B0A8 File Offset: 0x000792A8
		private void Leave()
		{
			this.ExecutePath.Clear();
			Monitor.Exit(this);
		}

		// Token: 0x0600081F RID: 2079 RVA: 0x0007B0BE File Offset: 0x000792BE
		public void SetTag(int tag)
		{
			this.ExecutePath.Add(tag);
		}

		// Token: 0x06000820 RID: 2080 RVA: 0x0007B0CE File Offset: 0x000792CE
		void IDisposable.Dispose()
		{
			this.Leave();
		}

		// Token: 0x04000DF1 RID: 3569
		private bool m_disposed = false;

		// Token: 0x04000DF2 RID: 3570
		private List<int> ExecutePath = new List<int>();
	}
}
