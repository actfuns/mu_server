using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000754 RID: 1876
	public class CacheMergeItem
	{
		// Token: 0x17000347 RID: 839
		// (get) Token: 0x06002F33 RID: 12083 RVA: 0x002A4BE8 File Offset: 0x002A2DE8
		// (set) Token: 0x06002F34 RID: 12084 RVA: 0x002A4BFF File Offset: 0x002A2DFF
		public List<int> NewGoodsID { get; set; }

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06002F35 RID: 12085 RVA: 0x002A4C08 File Offset: 0x002A2E08
		// (set) Token: 0x06002F36 RID: 12086 RVA: 0x002A4C1F File Offset: 0x002A2E1F
		public List<int> OrigGoodsIDList { get; set; }

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x06002F37 RID: 12087 RVA: 0x002A4C28 File Offset: 0x002A2E28
		// (set) Token: 0x06002F38 RID: 12088 RVA: 0x002A4C3F File Offset: 0x002A2E3F
		public List<int> OrigGoodsNumList { get; set; }

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x06002F39 RID: 12089 RVA: 0x002A4C48 File Offset: 0x002A2E48
		// (set) Token: 0x06002F3A RID: 12090 RVA: 0x002A4C5F File Offset: 0x002A2E5F
		public int DianJuan { get; set; }

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x06002F3B RID: 12091 RVA: 0x002A4C68 File Offset: 0x002A2E68
		// (set) Token: 0x06002F3C RID: 12092 RVA: 0x002A4C7F File Offset: 0x002A2E7F
		public int Money { get; set; }

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x06002F3D RID: 12093 RVA: 0x002A4C88 File Offset: 0x002A2E88
		// (set) Token: 0x06002F3E RID: 12094 RVA: 0x002A4C9F File Offset: 0x002A2E9F
		public int ZhenQi { get; set; }

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06002F3F RID: 12095 RVA: 0x002A4CA8 File Offset: 0x002A2EA8
		// (set) Token: 0x06002F40 RID: 12096 RVA: 0x002A4CBF File Offset: 0x002A2EBF
		public int JiFen { get; set; }

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x06002F41 RID: 12097 RVA: 0x002A4CC8 File Offset: 0x002A2EC8
		// (set) Token: 0x06002F42 RID: 12098 RVA: 0x002A4CDF File Offset: 0x002A2EDF
		public int JingYuan { get; set; }

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x06002F43 RID: 12099 RVA: 0x002A4CE8 File Offset: 0x002A2EE8
		// (set) Token: 0x06002F44 RID: 12100 RVA: 0x002A4CFF File Offset: 0x002A2EFF
		public int SuccessRate { get; set; }

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06002F45 RID: 12101 RVA: 0x002A4D08 File Offset: 0x002A2F08
		// (set) Token: 0x06002F46 RID: 12102 RVA: 0x002A4D1F File Offset: 0x002A2F1F
		public Dictionary<string, int> DestroyGoodsIDs { get; set; }

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x06002F47 RID: 12103 RVA: 0x002A4D28 File Offset: 0x002A2F28
		// (set) Token: 0x06002F48 RID: 12104 RVA: 0x002A4D3F File Offset: 0x002A2F3F
		public string PubStartTime { get; set; }

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x06002F49 RID: 12105 RVA: 0x002A4D48 File Offset: 0x002A2F48
		// (set) Token: 0x06002F4A RID: 12106 RVA: 0x002A4D5F File Offset: 0x002A2F5F
		public string PubEndTime { get; set; }

		// Token: 0x04003CC9 RID: 15561
		public int MergeType;
	}
}
