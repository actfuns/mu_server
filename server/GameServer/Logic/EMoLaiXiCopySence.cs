using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020004D8 RID: 1240
	internal class EMoLaiXiCopySence
	{
		// Token: 0x0600170C RID: 5900 RVA: 0x00169DAE File Offset: 0x00167FAE
		public void InitInfo(int nMapCode, int CopyMapID, int nQueueID)
		{
			this.m_MapCodeID = nMapCode;
			this.m_CopyMapID = CopyMapID;
			this.m_CopyMapQueueID = nQueueID;
			this.CleanAllInfo();
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x00169DD0 File Offset: 0x00167FD0
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

		// Token: 0x040020E4 RID: 8420
		public int m_MapCodeID = 0;

		// Token: 0x040020E5 RID: 8421
		public int m_CopyMapID = 0;

		// Token: 0x040020E6 RID: 8422
		public int m_CopyMapQueueID = 0;

		// Token: 0x040020E7 RID: 8423
		public int m_CreateMonsterWave = 0;

		// Token: 0x040020E8 RID: 8424
		public int m_TimeNotifyFlag = 0;

		// Token: 0x040020E9 RID: 8425
		public long m_Delay2 = 0L;

		// Token: 0x040020EA RID: 8426
		public List<EMoLaiXiCopySenceMonster> m_CreateWaveMonsterList = new List<EMoLaiXiCopySenceMonster>();

		// Token: 0x040020EB RID: 8427
		public long m_CreateMonsterTick2 = 0L;

		// Token: 0x040020EC RID: 8428
		public int m_CreateMonsterCount = 0;

		// Token: 0x040020ED RID: 8429
		public int m_TotalMonsterCount = 0;

		// Token: 0x040020EE RID: 8430
		public int m_TotalMonsterCountAllWave = 0;

		// Token: 0x040020EF RID: 8431
		public int m_CreateMonsterFirstWaveFlag = 0;

		// Token: 0x040020F0 RID: 8432
		public int m_CreateMonsterWaveNotify = 0;

		// Token: 0x040020F1 RID: 8433
		public long m_StartTimer = 0L;

		// Token: 0x040020F2 RID: 8434
		public int m_LoginEnterFlag = 0;

		// Token: 0x040020F3 RID: 8435
		public long m_LoginEnterTimer = 0L;

		// Token: 0x040020F4 RID: 8436
		public int m_EscapedMonsterNum = 0;

		// Token: 0x040020F5 RID: 8437
		public bool m_bAllMonsterCreated = false;

		// Token: 0x040020F6 RID: 8438
		public bool m_bFinished = false;
	}
}
