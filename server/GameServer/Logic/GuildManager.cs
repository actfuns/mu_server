using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	internal class GuildManager
	{
		
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

		
		public static Dictionary<int, List<int>> m_GuildApplyDic = new Dictionary<int, List<int>>();
	}
}
