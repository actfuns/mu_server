using System;
using System.Collections.Generic;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class CopyWolfSceneInfo
	{
		
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

		
		public void CleanAllInfo()
		{
		}

		
		public CopyMap CopyMapInfo;

		
		public int MapID = 0;

		
		public int CopyID = 0;

		
		public int FuBenSeqId = 0;

		
		public long GameId;

		
		public int PlayerCount = 0;

		
		public CopyWolfScoreData ScoreData = new CopyWolfScoreData();

		
		public GameSceneStatuses SceneStatus = GameSceneStatuses.STATUS_NULL;

		
		public long PrepareTime = 0L;

		
		public long BeginTime = 0L;

		
		public long EndTime = 0L;

		
		public long LeaveTime = 0L;

		
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		
		public int IsFortFlag = 0;

		
		public Monster MonsterFort = null;

		
		public int IsMonsterFlag = 0;

		
		public long CreateMonsterTime = 0L;

		
		public int MonsterWaveOld = 0;

		
		public int MonsterWave = 1;

		
		public int MonsterWaveTotal = 15;

		
		public int MonsterCountCreate = 0;

		
		public Dictionary<int, int> RoleMonsterScore = new Dictionary<int, int>();

		
		public HashSet<long> KilledMonsterHashSet = new HashSet<long>();
	}
}
