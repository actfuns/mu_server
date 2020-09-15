using System;

namespace HSGameEngine.Tools.AStar
{
	// Token: 0x020008DD RID: 2269
	public enum HeuristicFormula
	{
		// Token: 0x04004FAA RID: 20394
		Manhattan = 1,
		// Token: 0x04004FAB RID: 20395
		MaxDXDY,
		// Token: 0x04004FAC RID: 20396
		DiagonalShortCut,
		// Token: 0x04004FAD RID: 20397
		Euclidean,
		// Token: 0x04004FAE RID: 20398
		EuclideanNoSQR,
		// Token: 0x04004FAF RID: 20399
		Custom1
	}
}
