using System;
using System.Collections.Generic;

namespace GameServer.Logic.FluorescentGem
{
	// Token: 0x020002CE RID: 718
	internal class SoulStoneExpConfig
	{
		// Token: 0x04001289 RID: 4745
		public int Suit;

		// Token: 0x0400128A RID: 4746
		public int MinLevel;

		// Token: 0x0400128B RID: 4747
		public int MaxLevel;

		// Token: 0x0400128C RID: 4748
		public Dictionary<int, int> Lvl2Exp = new Dictionary<int, int>();
	}
}
