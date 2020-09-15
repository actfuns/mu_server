using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;

namespace GameServer.Logic
{
	// Token: 0x02000289 RID: 649
	public class CopyWolfInfo
	{
		// Token: 0x06000955 RID: 2389 RVA: 0x00094304 File Offset: 0x00092504
		public CopyWolfWaveInfo GetWaveConfig(int wave)
		{
			CopyWolfWaveInfo result;
			if (this.CopyWolfWaveDic.ContainsKey(wave))
			{
				result = this.CopyWolfWaveDic[wave];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x0009433C File Offset: 0x0009253C
		public int GetMonsterHurt(int monsterID)
		{
			int result;
			if (this.MonsterHurtDic.ContainsKey(monsterID))
			{
				result = this.MonsterHurtDic[monsterID];
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000957 RID: 2391 RVA: 0x00094374 File Offset: 0x00092574
		public int TotalSecs
		{
			get
			{
				return this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs;
			}
		}

		// Token: 0x0400100D RID: 4109
		public int MapID = 70200;

		// Token: 0x0400100E RID: 4110
		public int CopyID = 70200;

		// Token: 0x0400100F RID: 4111
		public int CampID = 1;

		// Token: 0x04001010 RID: 4112
		public int ScoreRateTime = 0;

		// Token: 0x04001011 RID: 4113
		public int ScoreRateLife = 0;

		// Token: 0x04001012 RID: 4114
		public int FortMonsterID = 0;

		// Token: 0x04001013 RID: 4115
		public Point FortSite = default(Point);

		// Token: 0x04001014 RID: 4116
		public Dictionary<int, CopyWolfWaveInfo> CopyWolfWaveDic = new Dictionary<int, CopyWolfWaveInfo>();

		// Token: 0x04001015 RID: 4117
		public Dictionary<int, int> MonsterHurtDic = new Dictionary<int, int>();

		// Token: 0x04001016 RID: 4118
		public ConcurrentDictionary<int, CopyWolfSceneInfo> SceneDict = new ConcurrentDictionary<int, CopyWolfSceneInfo>();

		// Token: 0x04001017 RID: 4119
		public int PrepareSecs = 1;

		// Token: 0x04001018 RID: 4120
		public int FightingSecs = 900;

		// Token: 0x04001019 RID: 4121
		public int ClearRolesSecs = 15;
	}
}
