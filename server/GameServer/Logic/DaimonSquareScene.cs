using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000615 RID: 1557
	public class DaimonSquareScene
	{
		// Token: 0x06001F8B RID: 8075 RVA: 0x001B5734 File Offset: 0x001B3934
		public void CleanAllInfo()
		{
			this.m_nMapCode = 0;
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_nPlarerCount = 0;
			this.m_nMonsterWave = 0;
			this.m_nCreateMonsterFlag = 0;
			this.m_eStatus = DaimonSquareStatus.FIGHT_STATUS_NULL;
			this.m_nCreateMonsterCount = 0;
			this.m_nNeedKillMonsterNum = 0;
			this.m_nDynamicMonsterList.Clear();
			this.m_bIsFinishTask = false;
			this.m_bEndFlag = false;
			this.m_nKillMonsterNum = 0;
			this.m_nKillMonsterTotalNum = 0;
			this.m_nMonsterTotalWave = 0;
			this.m_KilledMonsterHashSet.Clear();
		}

		// Token: 0x06001F8C RID: 8076 RVA: 0x001B57C8 File Offset: 0x001B39C8
		public bool AddKilledMonster(Monster monster)
		{
			bool firstKill = false;
			lock (this.m_KilledMonsterHashSet)
			{
				if (!this.m_KilledMonsterHashSet.Contains(monster.UniqueID))
				{
					this.m_KilledMonsterHashSet.Add(monster.UniqueID);
					this.m_nKillMonsterTotalNum++;
					firstKill = true;
				}
			}
			return firstKill;
		}

		// Token: 0x06001F8D RID: 8077 RVA: 0x001B5854 File Offset: 0x001B3A54
		public bool AddRole(GameClient client)
		{
			bool result = false;
			int roleId = client.ClientData.RoleID;
			lock (this.RoleIdSavedScoreDict)
			{
				if (!this.RoleIdSavedScoreDict.ContainsKey((long)roleId))
				{
					this.RoleIdSavedScoreDict[(long)roleId] = 0;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06001F8E RID: 8078 RVA: 0x001B58D8 File Offset: 0x001B3AD8
		public bool CantiansRole(GameClient client)
		{
			int roleId = client.ClientData.RoleID;
			bool result;
			lock (this.RoleIdSavedScoreDict)
			{
				result = this.RoleIdSavedScoreDict.ContainsKey((long)roleId);
			}
			return result;
		}

		// Token: 0x06001F8F RID: 8079 RVA: 0x001B593C File Offset: 0x001B3B3C
		public void AddRoleScoreAll(int addScore)
		{
			lock (this.RoleIdSavedScoreDict)
			{
				foreach (long key in this.RoleIdSavedScoreDict.Keys)
				{
					Dictionary<long, int> roleIdSavedScoreDict2;
					long key2;
					(roleIdSavedScoreDict2 = this.RoleIdSavedScoreDict)[key2 = key] = roleIdSavedScoreDict2[key2] + addScore;
				}
			}
		}

		// Token: 0x06001F90 RID: 8080 RVA: 0x001B59EC File Offset: 0x001B3BEC
		public int AddRoleScore(GameClient client, int addScore)
		{
			int roleId = client.ClientData.RoleID;
			lock (this.RoleIdSavedScoreDict)
			{
				int score;
				if (this.RoleIdSavedScoreDict.TryGetValue((long)roleId, out score))
				{
					score += addScore;
					this.RoleIdSavedScoreDict[(long)roleId] = 0;
				}
				else
				{
					score = addScore;
				}
				this.RoleIdSavedScoreDict[(long)roleId] = score;
			}
			return 0;
		}

		// Token: 0x06001F91 RID: 8081 RVA: 0x001B5A88 File Offset: 0x001B3C88
		public int GetRoleScore(GameClient client)
		{
			int roleId = client.ClientData.RoleID;
			lock (this.RoleIdSavedScoreDict)
			{
				int score;
				if (this.RoleIdSavedScoreDict.TryGetValue((long)roleId, out score))
				{
					return score;
				}
			}
			return 0;
		}

		// Token: 0x04002C15 RID: 11285
		public int m_nMapCode = 0;

		// Token: 0x04002C16 RID: 11286
		public long m_lPrepareTime = 0L;

		// Token: 0x04002C17 RID: 11287
		public long m_lBeginTime = 0L;

		// Token: 0x04002C18 RID: 11288
		public long m_lEndTime = 0L;

		// Token: 0x04002C19 RID: 11289
		public int m_nMonsterWave = 0;

		// Token: 0x04002C1A RID: 11290
		public int m_nMonsterTotalWave = 0;

		// Token: 0x04002C1B RID: 11291
		public int m_nCreateMonsterFlag = 0;

		// Token: 0x04002C1C RID: 11292
		public DaimonSquareStatus m_eStatus = DaimonSquareStatus.FIGHT_STATUS_NULL;

		// Token: 0x04002C1D RID: 11293
		public int m_nPlarerCount = 0;

		// Token: 0x04002C1E RID: 11294
		public int m_nCreateMonsterCount = 0;

		// Token: 0x04002C1F RID: 11295
		public int m_nKillMonsterNum = 0;

		// Token: 0x04002C20 RID: 11296
		public int m_nNeedKillMonsterNum = 0;

		// Token: 0x04002C21 RID: 11297
		public int m_nKillMonsterTotalNum = 0;

		// Token: 0x04002C22 RID: 11298
		public List<Monster> m_nDynamicMonsterList = new List<Monster>();

		// Token: 0x04002C23 RID: 11299
		public bool m_bIsFinishTask = false;

		// Token: 0x04002C24 RID: 11300
		public bool m_bEndFlag = false;

		// Token: 0x04002C25 RID: 11301
		public object m_CreateMonsterMutex = new object();

		// Token: 0x04002C26 RID: 11302
		public HashSet<long> m_KilledMonsterHashSet = new HashSet<long>();

		// Token: 0x04002C27 RID: 11303
		public CopyMap m_CopyMap;

		// Token: 0x04002C28 RID: 11304
		public bool ClearRole;

		// Token: 0x04002C29 RID: 11305
		public Dictionary<long, int> RoleIdSavedScoreDict = new Dictionary<long, int>();
	}
}
