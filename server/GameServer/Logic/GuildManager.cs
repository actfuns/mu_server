using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020006E9 RID: 1769
	internal class GuildManager
	{
		// Token: 0x06002ABB RID: 10939 RVA: 0x00263244 File Offset: 0x00261444
		public static void AddGuildApply(int nID, int nRole)
		{
			lock (GuildManager.m_GuildApplyDic)
			{
				List<int> lListData = GuildManager.m_GuildApplyDic[nID];
				if (lListData == null)
				{
					lListData = new List<int>();
				}
				lListData.Add(nRole);
				GuildManager.m_GuildApplyDic[nID] = lListData;
			}
		}

		// Token: 0x06002ABC RID: 10940 RVA: 0x002632BC File Offset: 0x002614BC
		public static void RemoveGuildApply(int nID, int nRole)
		{
			lock (GuildManager.m_GuildApplyDic)
			{
				List<int> lListData = GuildManager.m_GuildApplyDic[nID];
				if (lListData != null)
				{
					lListData.Remove(nRole);
					GuildManager.m_GuildApplyDic[nID] = lListData;
				}
			}
		}

		// Token: 0x040039D3 RID: 14803
		public static Dictionary<int, List<int>> m_GuildApplyDic = new Dictionary<int, List<int>>();
	}
}
