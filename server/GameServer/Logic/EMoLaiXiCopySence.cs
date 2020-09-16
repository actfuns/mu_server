using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	internal class EMoLaiXiCopySence
	{
		
		public void InitInfo(int nMapCode, int CopyMapID, int nQueueID)
		{
			this.m_MapCodeID = nMapCode;
			this.m_CopyMapID = CopyMapID;
			this.m_CopyMapQueueID = nQueueID;
			this.CleanAllInfo();
		}

		
		public void CleanAllInfo()
		{
			this.m_CreateMonsterWave = 0;
			this.m_TimeNotifyFlag = 0;
			this.m_CreateMonsterTick2 = 0L;
			this.m_CreateMonsterCount = 0;
			this.m_CreateMonsterFirstWaveFlag = 0;
			this.m_CreateMonsterWaveNotify = 0;
			this.m_StartTimer = 0L;
			this.m_LoginEnterFlag = 0;
			this.m_LoginEnterTimer = 0L;
			this.m_EscapedMonsterNum = 0;
			this.m_bFinished = false;
			this.m_bAllMonsterCreated = false;
			this.m_TotalMonsterCount = 0;
			this.m_Delay2 = 0L;
		}

		
		public int m_MapCodeID = 0;

		
		public int m_CopyMapID = 0;

		
		public int m_CopyMapQueueID = 0;

		
		public int m_CreateMonsterWave = 0;

		
		public int m_TimeNotifyFlag = 0;

		
		public long m_Delay2 = 0L;

		
		public List<EMoLaiXiCopySenceMonster> m_CreateWaveMonsterList = new List<EMoLaiXiCopySenceMonster>();

		
		public long m_CreateMonsterTick2 = 0L;

		
		public int m_CreateMonsterCount = 0;

		
		public int m_TotalMonsterCount = 0;

		
		public int m_TotalMonsterCountAllWave = 0;

		
		public int m_CreateMonsterFirstWaveFlag = 0;

		
		public int m_CreateMonsterWaveNotify = 0;

		
		public long m_StartTimer = 0L;

		
		public int m_LoginEnterFlag = 0;

		
		public long m_LoginEnterTimer = 0L;

		
		public int m_EscapedMonsterNum = 0;

		
		public bool m_bAllMonsterCreated = false;

		
		public bool m_bFinished = false;
	}
}
