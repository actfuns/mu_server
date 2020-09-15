using System;
using System.Collections.Generic;

namespace GameServer.Logic.Marriage.CoupleArena
{
	// Token: 0x0200035D RID: 861
	public class CoupleArenaBuffCfg
	{
		// Token: 0x040016BE RID: 5822
		public int Type;

		// Token: 0x040016BF RID: 5823
		public string Name;

		// Token: 0x040016C0 RID: 5824
		public List<CoupleArenaBuffCfg.RandPos> RandPosList;

		// Token: 0x040016C1 RID: 5825
		public List<int> FlushSecList;

		// Token: 0x040016C2 RID: 5826
		public Dictionary<ExtPropIndexes, double> ExtProps;

		// Token: 0x040016C3 RID: 5827
		public int MonsterId;

		// Token: 0x0200035E RID: 862
		public class RandPos
		{
			// Token: 0x040016C4 RID: 5828
			public int X;

			// Token: 0x040016C5 RID: 5829
			public int Y;

			// Token: 0x040016C6 RID: 5830
			public int R;
		}
	}
}
