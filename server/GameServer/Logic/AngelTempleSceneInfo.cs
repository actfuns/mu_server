using System;

namespace GameServer.Logic
{
	
	public class AngelTempleSceneInfo
	{
		
		public void CleanAll()
		{
			this.m_NotifyInfoTick = 0L;
			this.m_bEndFlag = 0;
			this.m_nPlarerCount = 0;
			this.m_nKillBossRole = 0;
			this.m_nAngelMonsterID = 0;
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = AngelTempleStatus.FIGHT_STATUS_NULL;
		}

		
		public int m_nMapCode = 0;

		
		public int m_nAngelMonsterID = 0;

		
		public long m_lPrepareTime = 0L;

		
		public long m_lBeginTime = 0L;

		
		public long m_lEndTime = 0L;

		
		public int m_bEndFlag = 0;

		
		public AngelTempleStatus m_eStatus = AngelTempleStatus.FIGHT_STATUS_NULL;

		
		public long m_lStatusEndTime = 0L;

		
		public int m_nPlarerCount = 0;

		
		public int m_nKillBossRole = 0;

		
		public long m_NotifyInfoTick = 0L;
	}
}
