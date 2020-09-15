using System;
using System.Collections.Generic;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020002B6 RID: 694
	public class ElementWarScene
	{
		// Token: 0x06000AD1 RID: 2769 RVA: 0x000AB4C4 File Offset: 0x000A96C4
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

		// Token: 0x06000AD2 RID: 2770 RVA: 0x000AB550 File Offset: 0x000A9750
		public void CleanAllInfo()
		{
		}

		// Token: 0x040011B3 RID: 4531
		public CopyMap CopyMapInfo;

		// Token: 0x040011B4 RID: 4532
		public int MapID = 0;

		// Token: 0x040011B5 RID: 4533
		public int CopyID = 0;

		// Token: 0x040011B6 RID: 4534
		public int FuBenSeqId = 0;

		// Token: 0x040011B7 RID: 4535
		public long GameId;

		// Token: 0x040011B8 RID: 4536
		public int PlayerCount = 0;

		// Token: 0x040011B9 RID: 4537
		public ElementWarScoreData ScoreData = new ElementWarScoreData();

		// Token: 0x040011BA RID: 4538
		public GameSceneStatuses SceneStatus = GameSceneStatuses.STATUS_NULL;

		// Token: 0x040011BB RID: 4539
		public long PrepareTime = 0L;

		// Token: 0x040011BC RID: 4540
		public long BeginTime = 0L;

		// Token: 0x040011BD RID: 4541
		public long EndTime = 0L;

		// Token: 0x040011BE RID: 4542
		public long LeaveTime = 0L;

		// Token: 0x040011BF RID: 4543
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		// Token: 0x040011C0 RID: 4544
		public int IsMonsterFlag = 0;

		// Token: 0x040011C1 RID: 4545
		public long CreateMonsterTime = 0L;

		// Token: 0x040011C2 RID: 4546
		public int MonsterWaveOld = 0;

		// Token: 0x040011C3 RID: 4547
		public int MonsterWave = 1;

		// Token: 0x040011C4 RID: 4548
		public int MonsterWaveTotal = 30;

		// Token: 0x040011C5 RID: 4549
		public int MonsterCountCreate = 0;

		// Token: 0x040011C6 RID: 4550
		public int MonsterCountKill = 0;

		// Token: 0x040011C7 RID: 4551
		public HashSet<long> KilledMonsterHashSet = new HashSet<long>();
	}
}
