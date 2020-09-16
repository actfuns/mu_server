using System;

namespace GameServer.Logic
{
	
	internal class GoldCopyScene
	{
		
		public void InitInfo(int nMapCode, int CopyMapID, int nQueueID)
		{
			this.m_MapCodeID = nMapCode;
			this.m_CopyMapID = CopyMapID;
			this.m_CopyMapQueueID = nQueueID;
			this.m_CreateMonsterWave = 0;
			this.m_TimeNotifyFlag = 0;
			this.m_CreateMonsterTick1 = 0L;
			this.m_CreateMonsterTick2 = 0L;
			this.m_CreateMonsterCount = 0;
			this.m_CreateMonsterFirstWaveFlag = 0;
			this.m_CreateMonsterWaveNotify = 0;
			this.m_StartTimer = 0L;
			this.m_LoginEnterFlag = 0;
			this.m_LoginEnterTimer = 0L;
		}

		
		public void CleanAllInfo()
		{
			this.m_MapCodeID = 0;
			this.m_CopyMapQueueID = 0;
			this.m_CreateMonsterWave = 0;
			this.m_TimeNotifyFlag = 0;
			this.m_CreateMonsterTick1 = 0L;
			this.m_CreateMonsterTick2 = 0L;
			this.m_CreateMonsterCount = 0;
			this.m_CreateMonsterFirstWaveFlag = 0;
			this.m_CreateMonsterWaveNotify = 0;
			this.m_StartTimer = 0L;
			this.m_StartTimer = 0L;
			this.m_LoginEnterFlag = 0;
			this.m_LoginEnterTimer = 0L;
		}

		
		public int m_MapCodeID = 0;

		
		public int m_CopyMapID = 0;

		
		public int m_CopyMapQueueID = 0;

		
		public int m_CreateMonsterWave = 0;

		
		public int m_TimeNotifyFlag = 0;

		
		public long m_CreateMonsterTick1 = 0L;

		
		public long m_CreateMonsterTick2 = 0L;

		
		public int m_CreateMonsterCount = 0;

		
		public int m_CreateMonsterFirstWaveFlag = 0;

		
		public int m_CreateMonsterWaveNotify = 0;

		
		public long m_StartTimer = 0L;

		
		public int m_LoginEnterFlag = 0;

		
		public long m_LoginEnterTimer = 0L;
	}
}
