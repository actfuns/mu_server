using System;

namespace GameServer.Logic
{
	
	internal class ExperienceCopyScene
	{
		
		public void InitInfo(int nMapCode, int CopyMapID, int nQueueID)
		{
			this.m_MapCodeID = nMapCode;
			this.m_CopyMapID = CopyMapID;
			this.m_CopyMapQueueID = nQueueID;
			this.m_ExperienceCopyMapCreateMonsterWave = 0;
			this.m_ExperienceCopyMapCreateMonsterFlag = 0;
			this.m_ExperienceCopyMapCreateMonsterNum = 0;
			this.m_ExperienceCopyMapNeedKillMonsterNum = 0;
			this.m_ExperienceCopyMapKillMonsterNum = 0;
			this.m_ExperienceCopyMapRemainMonsterNum = 0;
			this.m_ExperienceCopyMapKillMonsterTotalNum = 0;
			this.m_StartTimer = 0L;
		}

		
		public void CleanAllInfo()
		{
			this.m_MapCodeID = 0;
			this.m_CopyMapQueueID = 0;
			this.m_ExperienceCopyMapCreateMonsterWave = 0;
			this.m_ExperienceCopyMapCreateMonsterFlag = 0;
			this.m_ExperienceCopyMapCreateMonsterNum = 0;
			this.m_ExperienceCopyMapNeedKillMonsterNum = 0;
			this.m_ExperienceCopyMapKillMonsterNum = 0;
			this.m_ExperienceCopyMapRemainMonsterNum = 0;
			this.m_ExperienceCopyMapKillMonsterTotalNum = 0;
			this.m_StartTimer = 0L;
		}

		
		public int m_MapCodeID = 0;

		
		public int m_CopyMapID = 0;

		
		public int m_CopyMapQueueID = 0;

		
		public int m_ExperienceCopyMapCreateMonsterWave = 0;

		
		public int m_ExperienceCopyMapCreateMonsterFlag = 0;

		
		public int m_ExperienceCopyMapCreateMonsterNum = 0;

		
		public int m_ExperienceCopyMapNeedKillMonsterNum = 0;

		
		public int m_ExperienceCopyMapKillMonsterNum = 0;

		
		public int m_ExperienceCopyMapRemainMonsterNum = 0;

		
		public int m_ExperienceCopyMapKillMonsterTotalNum = 0;

		
		public long m_StartTimer = 0L;
	}
}
