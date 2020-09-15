using System;
using System.Collections.Generic;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000839 RID: 2105
	public class WanMoXiaGuScene
	{
		// Token: 0x06003B62 RID: 15202 RVA: 0x00327D8C File Offset: 0x00325F8C
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

		// Token: 0x06003B63 RID: 15203 RVA: 0x00327E08 File Offset: 0x00326008
		public void CleanAllInfo()
		{
		}

		// Token: 0x040045B2 RID: 17842
		public CopyMap CopyMapInfo;

		// Token: 0x040045B3 RID: 17843
		public int MapID;

		// Token: 0x040045B4 RID: 17844
		public int CopyMapID;

		// Token: 0x040045B5 RID: 17845
		public int FuBenSeqId;

		// Token: 0x040045B6 RID: 17846
		public long GameId;

		// Token: 0x040045B7 RID: 17847
		public int PlayerCount;

		// Token: 0x040045B8 RID: 17848
		public WanMoXiaGuScoreData ScoreData = new WanMoXiaGuScoreData();

		// Token: 0x040045B9 RID: 17849
		public GameSceneStatuses SceneStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x040045BA RID: 17850
		public long PrepareTime;

		// Token: 0x040045BB RID: 17851
		public long BeginTime;

		// Token: 0x040045BC RID: 17852
		public long EndTime;

		// Token: 0x040045BD RID: 17853
		public long LeaveTime;

		// Token: 0x040045BE RID: 17854
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x040045BF RID: 17855
		public double BossLifePercent;

		// Token: 0x040045C0 RID: 17856
		public bool MonsterCreated;

		// Token: 0x040045C1 RID: 17857
		public int MonsterCount;

		// Token: 0x040045C2 RID: 17858
		public Monster Boss;

		// Token: 0x040045C3 RID: 17859
		public long NextRelifeTicks;

		// Token: 0x040045C4 RID: 17860
		public int Success;

		// Token: 0x040045C5 RID: 17861
		public WanMoXiaGuMonsterConfigInfo ZuoQiInfo;

		// Token: 0x040045C6 RID: 17862
		public HashSet<long> KilledMonsterHashSet = new HashSet<long>();
	}
}
