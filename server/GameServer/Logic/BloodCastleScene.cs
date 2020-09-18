using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class BloodCastleScene
	{
		
		public void CleanAllInfo()
		{
			this.m_nMapCode = 0;
			this.m_lPrepareTime = 0L;
			this.m_lBeginTime = 0L;
			this.m_lEndTime = 0L;
			this.m_eStatus = BloodCastleStatus.FIGHT_STATUS_NULL;
			this.m_nPlarerCount = 0;
			this.m_nKillMonsterACount = 0;
			this.m_nKillMonsterBCount = 0;
			this.m_nDynamicMonsterList.Clear();
			this.m_bIsFinishTask = false;
			this.m_nRoleID = -1;
			this.m_bKillMonsterAStatus = 0;
			this.m_bKillMonsterBStatus = 0;
			this.m_bEndFlag = false;
			this.RoleIdSavedScoreDict.Clear();
			this.RoleIdTotalScoreDict.Clear();
		}

		
		public bool AddRole(GameClient client)
		{
			bool result = false;
			int roleId = client.ClientData.RoleID;
			lock (this.Mutex)
			{
				if (!this.RoleIdSavedScoreDict.ContainsKey((long)roleId))
				{
					this.RoleIdSavedScoreDict[(long)roleId] = 0;
					result = true;
				}
				if (!this.RoleIdTotalScoreDict.ContainsKey((long)roleId))
				{
					this.RoleIdTotalScoreDict[(long)roleId] = 0;
				}
			}
			return result;
		}

		
		public void RemoveRole(GameClient client)
		{
			int roleId = client.ClientData.RoleID;
			lock (this.Mutex)
			{
				this.RoleIdSavedScoreDict.Remove((long)roleId);
			}
		}

		
		public bool CantiansRole(GameClient client)
		{
			int roleId = client.ClientData.RoleID;
			bool result;
			lock (this.Mutex)
			{
				result = this.RoleIdTotalScoreDict.ContainsKey((long)roleId);
			}
			return result;
		}

		
		public void AddRoleScoreAll(int addScore)
		{
			lock (this.Mutex)
			{
				foreach (long key in this.RoleIdSavedScoreDict.Keys)
				{
					Dictionary<long, int> roleIdSavedScoreDict;
					long key2;
					(roleIdSavedScoreDict = this.RoleIdSavedScoreDict)[key2 = key] = roleIdSavedScoreDict[key2] + addScore;
				}
			}
		}

		
		public int AddRoleScore(GameClient client, int addScore)
		{
			int roleId = client.ClientData.RoleID;
			lock (this.Mutex)
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
			lock (this.Mutex)
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

		
		public BloodCastleStatus m_eStatus = BloodCastleStatus.FIGHT_STATUS_NULL;

		
		public int m_nPlarerCount = 0;

		
		public int m_nKillMonsterACount = 0;

		
		public int m_bKillMonsterAStatus = 0;

		
		public int m_nKillMonsterBCount = 0;

		
		public int m_bKillMonsterBStatus = 0;

		
		public List<Monster> m_nDynamicMonsterList = new List<Monster>();

		
		public bool m_bIsFinishTask = false;

		
		public int m_nRoleID = -1;

		
		public bool m_bEndFlag = false;

		
		public object Mutex = new object();

		
		public int m_Step = 0;

		
		public CopyMap m_CopyMap;

		
		public Dictionary<long, int> RoleIdSavedScoreDict = new Dictionary<long, int>();

		
		public Dictionary<long, int> RoleIdTotalScoreDict = new Dictionary<long, int>();
	}
}
