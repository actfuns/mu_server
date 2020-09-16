using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class EMoLaiXiCopySenceMonster
	{
		
		public EMoLaiXiCopySenceMonster CloneMini()
		{
			return new EMoLaiXiCopySenceMonster
			{
				m_ID = this.m_ID,
				m_Wave = this.m_Wave,
				PathIDArray = this.PathIDArray,
				m_Num = this.m_Num,
				m_MonsterID = this.m_MonsterID,
				m_Delay1 = this.m_Delay1,
				m_CreateMonsterTick1 = this.m_CreateMonsterTick1,
				m_CreateMonsterCount = this.m_CreateMonsterCount,
				m_Delay2 = this.m_Delay2
			};
		}

		
		public int m_ID;

		
		public int m_Wave;

		
		public int[] PathIDArray;

		
		public int m_Num;

		
		public List<int> m_MonsterID;

		
		public int m_Delay1;

		
		public long m_CreateMonsterTick1 = 0L;

		
		public int m_CreateMonsterCount = 0;

		
		public int m_Delay2;

		
		public List<int[]> PatrolPath;
	}
}
