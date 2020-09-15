using System;
using System.Collections.Generic;
using GameServer.Logic.Goods;

namespace GameServer.Logic
{
	// Token: 0x02000297 RID: 663
	public class JingLingQiYuanData
	{
		// Token: 0x0400105E RID: 4190
		public object Mutex = new object();

		// Token: 0x0400105F RID: 4191
		public List<PetGroupPropertyItem> PetGroupPropertyList = new List<PetGroupPropertyItem>();

		// Token: 0x04001060 RID: 4192
		public List<PetLevelAwardItem> PetLevelAwardList = new List<PetLevelAwardItem>();

		// Token: 0x04001061 RID: 4193
		public List<PetSkillLevelAwardItem> PetSkillLevelAwardList = new List<PetSkillLevelAwardItem>();

		// Token: 0x04001062 RID: 4194
		public List<PetTianFuAwardItem> PetTianFuAwardList = new List<PetTianFuAwardItem>();

		// Token: 0x04001063 RID: 4195
		public List<PetSkillGroupInfo> PetSkillAwardList = new List<PetSkillGroupInfo>();
	}
}
