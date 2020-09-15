using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020002B2 RID: 690
	public class ElementWarData
	{
		// Token: 0x06000AAD RID: 2733 RVA: 0x000A94C8 File Offset: 0x000A76C8
		public ElementWarMonsterConfigInfo GetOrderConfig(int order)
		{
			ElementWarMonsterConfigInfo result;
			if (this.MonsterOrderConfigList.ContainsKey(order))
			{
				result = this.MonsterOrderConfigList[order];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000AAE RID: 2734 RVA: 0x000A9500 File Offset: 0x000A7700
		public int TotalSecs
		{
			get
			{
				return this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs;
			}
		}

		// Token: 0x04001199 RID: 4505
		public object Mutex = new object();

		// Token: 0x0400119A RID: 4506
		public int MapID = 70100;

		// Token: 0x0400119B RID: 4507
		public int CopyID = 70100;

		// Token: 0x0400119C RID: 4508
		public int MinAwardWave = 0;

		// Token: 0x0400119D RID: 4509
		public int[] AwardLight;

		// Token: 0x0400119E RID: 4510
		public int[] YuanSuShiLianAward2;

		// Token: 0x0400119F RID: 4511
		public int GoodsBinding = 1;

		// Token: 0x040011A0 RID: 4512
		public Dictionary<int, ElementWarMonsterConfigInfo> MonsterOrderConfigList = new Dictionary<int, ElementWarMonsterConfigInfo>();

		// Token: 0x040011A1 RID: 4513
		public int PrepareSecs = 1;

		// Token: 0x040011A2 RID: 4514
		public int FightingSecs = 900;

		// Token: 0x040011A3 RID: 4515
		public int ClearRolesSecs = 15;
	}
}
