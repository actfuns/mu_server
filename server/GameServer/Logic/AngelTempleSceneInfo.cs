using System;

namespace GameServer.Logic
{
	// Token: 0x020005B9 RID: 1465
	public class AngelTempleSceneInfo
	{
		// Token: 0x06001A9C RID: 6812 RVA: 0x00197DFC File Offset: 0x00195FFC
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

		// Token: 0x04002944 RID: 10564
		public int m_nMapCode = 0;

		// Token: 0x04002945 RID: 10565
		public int m_nAngelMonsterID = 0;

		// Token: 0x04002946 RID: 10566
		public long m_lPrepareTime = 0L;

		// Token: 0x04002947 RID: 10567
		public long m_lBeginTime = 0L;

		// Token: 0x04002948 RID: 10568
		public long m_lEndTime = 0L;

		// Token: 0x04002949 RID: 10569
		public int m_bEndFlag = 0;

		// Token: 0x0400294A RID: 10570
		public AngelTempleStatus m_eStatus = AngelTempleStatus.FIGHT_STATUS_NULL;

		// Token: 0x0400294B RID: 10571
		public long m_lStatusEndTime = 0L;

		// Token: 0x0400294C RID: 10572
		public int m_nPlarerCount = 0;

		// Token: 0x0400294D RID: 10573
		public int m_nKillBossRole = 0;

		// Token: 0x0400294E RID: 10574
		public long m_NotifyInfoTick = 0L;
	}
}
