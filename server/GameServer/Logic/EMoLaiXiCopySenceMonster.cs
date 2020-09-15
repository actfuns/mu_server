using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020004D9 RID: 1241
	public class EMoLaiXiCopySenceMonster
	{
		// Token: 0x0600170F RID: 5903 RVA: 0x00169EE8 File Offset: 0x001680E8
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

		// Token: 0x040020F7 RID: 8439
		public int m_ID;

		// Token: 0x040020F8 RID: 8440
		public int m_Wave;

		// Token: 0x040020F9 RID: 8441
		public int[] PathIDArray;

		// Token: 0x040020FA RID: 8442
		public int m_Num;

		// Token: 0x040020FB RID: 8443
		public List<int> m_MonsterID;

		// Token: 0x040020FC RID: 8444
		public int m_Delay1;

		// Token: 0x040020FD RID: 8445
		public long m_CreateMonsterTick1 = 0L;

		// Token: 0x040020FE RID: 8446
		public int m_CreateMonsterCount = 0;

		// Token: 0x040020FF RID: 8447
		public int m_Delay2;

		// Token: 0x04002100 RID: 8448
		public List<int[]> PatrolPath;
	}
}
