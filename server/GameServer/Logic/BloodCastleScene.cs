using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020005CC RID: 1484
	public class BloodCastleScene
	{
		// Token: 0x06001B9B RID: 7067 RVA: 0x001A1124 File Offset: 0x0019F324
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

		// Token: 0x06001B9C RID: 7068 RVA: 0x001A11B4 File Offset: 0x0019F3B4
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

		// Token: 0x06001B9D RID: 7069 RVA: 0x001A125C File Offset: 0x0019F45C
		public void RemoveRole(GameClient client)
		{
			int roleId = client.ClientData.RoleID;
			lock (this.Mutex)
			{
				this.RoleIdSavedScoreDict.Remove((long)roleId);
			}
		}

		// Token: 0x06001B9E RID: 7070 RVA: 0x001A12BC File Offset: 0x0019F4BC
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

		// Token: 0x06001B9F RID: 7071 RVA: 0x001A1320 File Offset: 0x0019F520
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

		// Token: 0x06001BA0 RID: 7072 RVA: 0x001A13D0 File Offset: 0x0019F5D0
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

		// Token: 0x06001BA1 RID: 7073 RVA: 0x001A146C File Offset: 0x0019F66C
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

		// Token: 0x040029EA RID: 10730
		public int m_nMapCode = 0;

		// Token: 0x040029EB RID: 10731
		public long m_lPrepareTime = 0L;

		// Token: 0x040029EC RID: 10732
		public long m_lBeginTime = 0L;

		// Token: 0x040029ED RID: 10733
		public long m_lEndTime = 0L;

		// Token: 0x040029EE RID: 10734
		public BloodCastleStatus m_eStatus = BloodCastleStatus.FIGHT_STATUS_NULL;

		// Token: 0x040029EF RID: 10735
		public int m_nPlarerCount = 0;

		// Token: 0x040029F0 RID: 10736
		public int m_nKillMonsterACount = 0;

		// Token: 0x040029F1 RID: 10737
		public int m_bKillMonsterAStatus = 0;

		// Token: 0x040029F2 RID: 10738
		public int m_nKillMonsterBCount = 0;

		// Token: 0x040029F3 RID: 10739
		public int m_bKillMonsterBStatus = 0;

		// Token: 0x040029F4 RID: 10740
		public List<Monster> m_nDynamicMonsterList = new List<Monster>();

		// Token: 0x040029F5 RID: 10741
		public bool m_bIsFinishTask = false;

		// Token: 0x040029F6 RID: 10742
		public int m_nRoleID = -1;

		// Token: 0x040029F7 RID: 10743
		public bool m_bEndFlag = false;

		// Token: 0x040029F8 RID: 10744
		public object Mutex = new object();

		// Token: 0x040029F9 RID: 10745
		public int m_Step = 0;

		// Token: 0x040029FA RID: 10746
		public CopyMap m_CopyMap;

		// Token: 0x040029FB RID: 10747
		public Dictionary<long, int> RoleIdSavedScoreDict = new Dictionary<long, int>();

		// Token: 0x040029FC RID: 10748
		public Dictionary<long, int> RoleIdTotalScoreDict = new Dictionary<long, int>();
	}
}
