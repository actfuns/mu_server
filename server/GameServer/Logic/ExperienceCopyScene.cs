using System;

namespace GameServer.Logic
{
	// Token: 0x020006BA RID: 1722
	internal class ExperienceCopyScene
	{
		// Token: 0x06002061 RID: 8289 RVA: 0x001BE268 File Offset: 0x001BC468
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

		// Token: 0x06002062 RID: 8290 RVA: 0x001BE2C4 File Offset: 0x001BC4C4
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

		// Token: 0x0400365F RID: 13919
		public int m_MapCodeID = 0;

		// Token: 0x04003660 RID: 13920
		public int m_CopyMapID = 0;

		// Token: 0x04003661 RID: 13921
		public int m_CopyMapQueueID = 0;

		// Token: 0x04003662 RID: 13922
		public int m_ExperienceCopyMapCreateMonsterWave = 0;

		// Token: 0x04003663 RID: 13923
		public int m_ExperienceCopyMapCreateMonsterFlag = 0;

		// Token: 0x04003664 RID: 13924
		public int m_ExperienceCopyMapCreateMonsterNum = 0;

		// Token: 0x04003665 RID: 13925
		public int m_ExperienceCopyMapNeedKillMonsterNum = 0;

		// Token: 0x04003666 RID: 13926
		public int m_ExperienceCopyMapKillMonsterNum = 0;

		// Token: 0x04003667 RID: 13927
		public int m_ExperienceCopyMapRemainMonsterNum = 0;

		// Token: 0x04003668 RID: 13928
		public int m_ExperienceCopyMapKillMonsterTotalNum = 0;

		// Token: 0x04003669 RID: 13929
		public long m_StartTimer = 0L;
	}
}
