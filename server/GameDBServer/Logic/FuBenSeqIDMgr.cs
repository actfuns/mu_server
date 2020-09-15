using System;

namespace GameDBServer.Logic
{
	// Token: 0x020001C9 RID: 457
	public class FuBenSeqIDMgr
	{
		// Token: 0x06000935 RID: 2357 RVA: 0x00058E84 File Offset: 0x00057084
		public static int GetFuBenSeqID()
		{
			int result;
			lock (FuBenSeqIDMgr.Mutex)
			{
				result = FuBenSeqIDMgr.BaseFuBenSeqID++;
			}
			return result;
		}

		// Token: 0x04000BB8 RID: 3000
		private static object Mutex = new object();

		// Token: 0x04000BB9 RID: 3001
		private static int BaseFuBenSeqID = 1;
	}
}
