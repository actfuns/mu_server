using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000515 RID: 1301
	internal class LingYuType
	{
		// Token: 0x04002298 RID: 8856
		public int Type;

		// Token: 0x04002299 RID: 8857
		public string Name;

		// Token: 0x0400229A RID: 8858
		public double LifeScale;

		// Token: 0x0400229B RID: 8859
		public double AttackScale;

		// Token: 0x0400229C RID: 8860
		public double DefenseScale;

		// Token: 0x0400229D RID: 8861
		public double MAttackScale;

		// Token: 0x0400229E RID: 8862
		public double MDefenseScale;

		// Token: 0x0400229F RID: 8863
		public double HitScale;

		// Token: 0x040022A0 RID: 8864
		public Dictionary<int, LingYuLevel> LevelDict = new Dictionary<int, LingYuLevel>();

		// Token: 0x040022A1 RID: 8865
		public Dictionary<int, LingYuSuit> SuitDict = new Dictionary<int, LingYuSuit>();
	}
}
