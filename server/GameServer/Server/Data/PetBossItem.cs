using System;
using System.Collections.Generic;

namespace Server.Data
{
	// Token: 0x020007F1 RID: 2033
	public class PetBossItem
	{
		// Token: 0x0400436F RID: 17263
		public int ID;

		// Token: 0x04004370 RID: 17264
		public int MonsterID;

		// Token: 0x04004371 RID: 17265
		public int Star;

		// Token: 0x04004372 RID: 17266
		public int FreeStartValue;

		// Token: 0x04004373 RID: 17267
		public int FreeEndValue;

		// Token: 0x04004374 RID: 17268
		public int ZuanShiStartValue;

		// Token: 0x04004375 RID: 17269
		public int ZuanShiEndValue;

		// Token: 0x04004376 RID: 17270
		public int Time;

		// Token: 0x04004377 RID: 17271
		public string FightAward;

		// Token: 0x04004378 RID: 17272
		public string KillAward;

		// Token: 0x04004379 RID: 17273
		public string KillExtraAward;

		// Token: 0x0400437A RID: 17274
		public int PetLevelStep;

		// Token: 0x0400437B RID: 17275
		public int[] PetLevelStepNum;

		// Token: 0x0400437C RID: 17276
		public int ExcellentStep;

		// Token: 0x0400437D RID: 17277
		public int[] ExcellentStepNum;

		// Token: 0x0400437E RID: 17278
		public List<int> PetSuit;

		// Token: 0x0400437F RID: 17279
		public List<int> PetRate;

		// Token: 0x04004380 RID: 17280
		public int SuitRate;
	}
}
