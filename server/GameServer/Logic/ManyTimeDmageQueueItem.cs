using System;

namespace GameServer.Logic
{
	
	public class ManyTimeDmageQueueItem
	{
		
		public long ToExecTicks = 0L;

		
		public int enemy = -1;

		
		public int enemyX = 0;

		
		public int enemyY = 0;

		
		public int realEnemyX = 0;

		
		public int realEnemyY = 0;

		
		public int magicCode = 0;

		
		public int manyRangeIndex = 0;

		
		public double manyRangeInjuredPercent = 1.0;
	}
}
