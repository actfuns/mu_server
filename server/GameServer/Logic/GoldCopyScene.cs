using System;

namespace GameServer.Logic
{
	// Token: 0x020006D8 RID: 1752
	internal class GoldCopyScene
	{
		// Token: 0x060029CA RID: 10698 RVA: 0x00258D68 File Offset: 0x00256F68
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

		// Token: 0x060029CB RID: 10699 RVA: 0x00258DD8 File Offset: 0x00256FD8
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

		// Token: 0x04003965 RID: 14693
		public int m_MapCodeID = 0;

		// Token: 0x04003966 RID: 14694
		public int m_CopyMapID = 0;

		// Token: 0x04003967 RID: 14695
		public int m_CopyMapQueueID = 0;

		// Token: 0x04003968 RID: 14696
		public int m_CreateMonsterWave = 0;

		// Token: 0x04003969 RID: 14697
		public int m_TimeNotifyFlag = 0;

		// Token: 0x0400396A RID: 14698
		public long m_CreateMonsterTick1 = 0L;

		// Token: 0x0400396B RID: 14699
		public long m_CreateMonsterTick2 = 0L;

		// Token: 0x0400396C RID: 14700
		public int m_CreateMonsterCount = 0;

		// Token: 0x0400396D RID: 14701
		public int m_CreateMonsterFirstWaveFlag = 0;

		// Token: 0x0400396E RID: 14702
		public int m_CreateMonsterWaveNotify = 0;

		// Token: 0x0400396F RID: 14703
		public long m_StartTimer = 0L;

		// Token: 0x04003970 RID: 14704
		public int m_LoginEnterFlag = 0;

		// Token: 0x04003971 RID: 14705
		public long m_LoginEnterTimer = 0L;
	}
}
