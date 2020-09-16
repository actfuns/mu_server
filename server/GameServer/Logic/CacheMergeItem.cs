using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000754 RID: 1876
	public class CacheMergeItem
	{
		// Token: 0x17000347 RID: 839
		
		
		public List<int> NewGoodsID { get; set; }

		// Token: 0x17000348 RID: 840
		
		
		public List<int> OrigGoodsIDList { get; set; }

		// Token: 0x17000349 RID: 841
		
		
		public List<int> OrigGoodsNumList { get; set; }

		// Token: 0x1700034A RID: 842
		
		
		public int DianJuan { get; set; }

		// Token: 0x1700034B RID: 843
		
		
		public int Money { get; set; }

		// Token: 0x1700034C RID: 844
		
		
		public int ZhenQi { get; set; }

		// Token: 0x1700034D RID: 845
		
		
		public int JiFen { get; set; }

		// Token: 0x1700034E RID: 846
		
		
		public int JingYuan { get; set; }

		// Token: 0x1700034F RID: 847
		
		
		public int SuccessRate { get; set; }

		// Token: 0x17000350 RID: 848
		
		
		public Dictionary<string, int> DestroyGoodsIDs { get; set; }

		// Token: 0x17000351 RID: 849
		
		
		public string PubStartTime { get; set; }

		// Token: 0x17000352 RID: 850
		
		
		public string PubEndTime { get; set; }

		// Token: 0x04003CC9 RID: 15561
		public int MergeType;
	}
}
