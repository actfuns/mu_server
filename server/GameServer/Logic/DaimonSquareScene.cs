using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class DaimonSquareScene
	{
		
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

		
		public int m_nMapCode = 0;

		
		public long m_lPrepareTime = 0L;

		
		public long m_lBeginTime = 0L;

		
		public long m_lEndTime = 0L;

		
		public int m_nMonsterWave = 0;

		
		public int m_nMonsterTotalWave = 0;

		
		public int m_nCreateMonsterFlag = 0;

		
		public DaimonSquareStatus m_eStatus = DaimonSquareStatus.FIGHT_STATUS_NULL;

		
		public int m_nPlarerCount = 0;

		
		public int m_nCreateMonsterCount = 0;

		
		public int m_nKillMonsterNum = 0;

		
		public int m_nNeedKillMonsterNum = 0;

		
		public int m_nKillMonsterTotalNum = 0;

		
		public List<Monster> m_nDynamicMonsterList = new List<Monster>();

		
		public bool m_bIsFinishTask = false;

		
		public bool m_bEndFlag = false;

		
		public object m_CreateMonsterMutex = new object();

		
		public HashSet<long> m_KilledMonsterHashSet = new HashSet<long>();

		
		public CopyMap m_CopyMap;

		
		public bool ClearRole;

		
		public Dictionary<long, int> RoleIdSavedScoreDict = new Dictionary<long, int>();
	}
}
