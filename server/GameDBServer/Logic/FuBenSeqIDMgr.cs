using System;

namespace GameDBServer.Logic
{
	
	public class FuBenSeqIDMgr
	{
		
		public static int GetFuBenSeqID()
		{
			int result;
			lock (FuBenSeqIDMgr.Mutex)
			{
				result = FuBenSeqIDMgr.BaseFuBenSeqID++;
			}
			return result;
		}

		
		private static object Mutex = new object();

		
		private static int BaseFuBenSeqID = 1;
	}
}
