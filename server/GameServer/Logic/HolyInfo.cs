using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200047B RID: 1147
	internal class HolyInfo
	{
		// Token: 0x060014DB RID: 5339 RVA: 0x00146334 File Offset: 0x00144534
		public static int GetShengwuID(sbyte nSuit, sbyte nType)
		{
			return (int)(nType * 100 + nSuit);
		}

		// Token: 0x04001E2E RID: 7726
		public List<MagicActionItem> m_ExtraPropertyList = new List<MagicActionItem>();
	}
}
