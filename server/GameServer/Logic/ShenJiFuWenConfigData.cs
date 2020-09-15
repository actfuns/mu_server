using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000400 RID: 1024
	public class ShenJiFuWenConfigData
	{
		// Token: 0x06001212 RID: 4626 RVA: 0x0011F3F8 File Offset: 0x0011D5F8
		public ShenJiFuWenEffectData GetEffect(int lev)
		{
			ShenJiFuWenEffectData result;
			if (lev <= 0 || lev > this.ShenJiEffectList.Count)
			{
				result = null;
			}
			else
			{
				result = this.ShenJiEffectList[lev - 1];
			}
			return result;
		}

		// Token: 0x04001B48 RID: 6984
		public int ShenJiID;

		// Token: 0x04001B49 RID: 6985
		public int PreShenJiID;

		// Token: 0x04001B4A RID: 6986
		public int PreShenJiLev;

		// Token: 0x04001B4B RID: 6987
		public int MaxLevel;

		// Token: 0x04001B4C RID: 6988
		public int UpNeed;

		// Token: 0x04001B4D RID: 6989
		public List<ShenJiFuWenEffectData> ShenJiEffectList = new List<ShenJiFuWenEffectData>();
	}
}
