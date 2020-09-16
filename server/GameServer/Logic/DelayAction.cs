using System;

namespace GameServer.Logic
{
	
	internal class DelayAction
	{
		
		public GameClient m_Client = null;

		
		public long m_StartTime = 0L;

		
		public long m_DelayTime = 0L;

		
		public int[] m_Params = new int[2];

		
		public DelayActionType m_DelayActionType = DelayActionType.DB_NULL;
	}
}
