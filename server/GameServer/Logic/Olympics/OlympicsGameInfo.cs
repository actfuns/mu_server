using System;
using System.Collections.Generic;

namespace GameServer.Logic.Olympics
{
	// Token: 0x02000395 RID: 917
	public class OlympicsGameInfo
	{
		// Token: 0x04001820 RID: 6176
		public int GameID = 0;

		// Token: 0x04001821 RID: 6177
		public int CountFree = 0;

		// Token: 0x04001822 RID: 6178
		public int CountDiamond = 0;

		// Token: 0x04001823 RID: 6179
		public List<int> DiamondList = null;

		// Token: 0x04001824 RID: 6180
		public int CountGame = 0;

		// Token: 0x04001825 RID: 6181
		public int CountWin = 0;

		// Token: 0x04001826 RID: 6182
		public int GradeWin = 0;

		// Token: 0x04001827 RID: 6183
		public int GradeLost = 0;
	}
}
