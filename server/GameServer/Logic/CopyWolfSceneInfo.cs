using System;
using System.Collections.Generic;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x0200028B RID: 651
	public class CopyWolfSceneInfo
	{
		// Token: 0x06000975 RID: 2421 RVA: 0x000963A8 File Offset: 0x000945A8
		public int AddMonsterScore(int roleID, int score)
		{
			if (this.RoleMonsterScore.ContainsKey(roleID))
			{
				Dictionary<int, int> roleMonsterScore;
				(roleMonsterScore = this.RoleMonsterScore)[roleID] = roleMonsterScore[roleID] + score;
			}
			else
			{
				this.RoleMonsterScore.Add(roleID, score);
			}
			return this.RoleMonsterScore[roleID];
		}

		// Token: 0x06000976 RID: 2422 RVA: 0x00096404 File Offset: 0x00094604
		public int GetMonsterScore(int roleID)
		{
			int result;
			if (this.RoleMonsterScore.ContainsKey(roleID))
			{
				result = this.RoleMonsterScore[roleID];
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x0009643C File Offset: 0x0009463C
		public bool AddKilledMonster(Monster monster)
		{
			bool firstKill = false;
			lock (this.KilledMonsterHashSet)
			{
				if (!this.KilledMonsterHashSet.Contains(monster.UniqueID))
				{
					this.KilledMonsterHashSet.Add(monster.UniqueID);
					firstKill = true;
				}
			}
			return firstKill;
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x000964B8 File Offset: 0x000946B8
		public void CleanAllInfo()
		{
		}

		// Token: 0x0400101F RID: 4127
		public CopyMap CopyMapInfo;

		// Token: 0x04001020 RID: 4128
		public int MapID = 0;

		// Token: 0x04001021 RID: 4129
		public int CopyID = 0;

		// Token: 0x04001022 RID: 4130
		public int FuBenSeqId = 0;

		// Token: 0x04001023 RID: 4131
		public long GameId;

		// Token: 0x04001024 RID: 4132
		public int PlayerCount = 0;

		// Token: 0x04001025 RID: 4133
		public CopyWolfScoreData ScoreData = new CopyWolfScoreData();

		// Token: 0x04001026 RID: 4134
		public GameSceneStatuses SceneStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x04001027 RID: 4135
		public long PrepareTime = 0L;

		// Token: 0x04001028 RID: 4136
		public long BeginTime = 0L;

		// Token: 0x04001029 RID: 4137
		public long EndTime = 0L;

		// Token: 0x0400102A RID: 4138
		public long LeaveTime = 0L;

		// Token: 0x0400102B RID: 4139
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x0400102C RID: 4140
		public int IsFortFlag = 0;

		// Token: 0x0400102D RID: 4141
		public Monster MonsterFort = null;

		// Token: 0x0400102E RID: 4142
		public int IsMonsterFlag = 0;

		// Token: 0x0400102F RID: 4143
		public long CreateMonsterTime = 0L;

		// Token: 0x04001030 RID: 4144
		public int MonsterWaveOld = 0;

		// Token: 0x04001031 RID: 4145
		public int MonsterWave = 1;

		// Token: 0x04001032 RID: 4146
		public int MonsterWaveTotal = 15;

		// Token: 0x04001033 RID: 4147
		public int MonsterCountCreate = 0;

		// Token: 0x04001034 RID: 4148
		public Dictionary<int, int> RoleMonsterScore = new Dictionary<int, int>();

		// Token: 0x04001035 RID: 4149
		public HashSet<long> KilledMonsterHashSet = new HashSet<long>();
	}
}
