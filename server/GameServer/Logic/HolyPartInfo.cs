using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200047A RID: 1146
	internal class HolyPartInfo
	{
		// Token: 0x060014D9 RID: 5337 RVA: 0x001462C0 File Offset: 0x001444C0
		public static int GetBujianID(sbyte nType, sbyte nSlot, sbyte nSuit)
		{
			return (int)nType * 1000 + (int)((nSlot - 1) * 100) + (int)nSuit;
		}

		// Token: 0x04001E24 RID: 7716
		public int m_nCostBandJinBi = 0;

		// Token: 0x04001E25 RID: 7717
		public List<List<int>> NeedGoods;

		// Token: 0x04001E26 RID: 7718
		public List<List<int>> FaildNeedGoods;

		// Token: 0x04001E27 RID: 7719
		public int m_nNeedGoodsID = 0;

		// Token: 0x04001E28 RID: 7720
		public int m_nNeedGoodsCount = 0;

		// Token: 0x04001E29 RID: 7721
		public int m_nFailCostGoodsID = 0;

		// Token: 0x04001E2A RID: 7722
		public int m_nFailCostGoodsCount = 0;

		// Token: 0x04001E2B RID: 7723
		public sbyte m_sSuccessProbability = 0;

		// Token: 0x04001E2C RID: 7724
		public List<MagicActionItem> m_PropertyList = new List<MagicActionItem>();

		// Token: 0x04001E2D RID: 7725
		public int m_nMaxFailCount = 0;
	}
}
