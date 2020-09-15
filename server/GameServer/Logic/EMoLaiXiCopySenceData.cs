using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200060F RID: 1551
	public class EMoLaiXiCopySenceData
	{
		// Token: 0x04002BDF RID: 11231
		public List<EMoLaiXiCopySenceMonster> EMoLaiXiCopySenceMonsterData = new List<EMoLaiXiCopySenceMonster>();

		// Token: 0x04002BE0 RID: 11232
		public Dictionary<int, List<int[]>> m_MonsterPatorlPathLists = new Dictionary<int, List<int[]>>();

		// Token: 0x04002BE1 RID: 11233
		public int TotalWave;

		// Token: 0x04002BE2 RID: 11234
		public int FaildEscapeMonsterNum;
	}
}
