using System;
using System.Collections.Generic;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class ElementWarScene
	{
		
		public bool AddKilledMonster(Monster monster)
		{
			bool firstKill = false;
			lock (this.KilledMonsterHashSet)
			{
				if (!this.KilledMonsterHashSet.Contains(monster.UniqueID))
				{
					this.KilledMonsterHashSet.Add(monster.UniqueID);
					this.MonsterCountKill++;
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

		
		public ElementWarScoreData ScoreData = new ElementWarScoreData();

		
		public GameSceneStatuses SceneStatus = GameSceneStatuses.STATUS_NULL;

		
		public long PrepareTime = 0L;

		
		public long BeginTime = 0L;

		
		public long EndTime = 0L;

		
		public long LeaveTime = 0L;

		
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		
		public int IsMonsterFlag = 0;

		
		public long CreateMonsterTime = 0L;

		
		public int MonsterWaveOld = 0;

		
		public int MonsterWave = 1;

		
		public int MonsterWaveTotal = 30;

		
		public int MonsterCountCreate = 0;

		
		public int MonsterCountKill = 0;

		
		public HashSet<long> KilledMonsterHashSet = new HashSet<long>();
	}
}
