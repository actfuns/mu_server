using System;
using System.Collections.Generic;
using GameServer.Logic;

namespace Server.Data
{
	// Token: 0x020003E7 RID: 999
	public class RebornBossScene
	{
		// Token: 0x06001149 RID: 4425 RVA: 0x0010F2C8 File Offset: 0x0010D4C8
		public void LoadSceneDBInfo()
		{
			string rebornkey = string.Format("{0}{1}", "reborn_boss_", this.m_nMapCode);
			string rebornboss = GameManager.GameConfigMgr.GetGameConfigItemStr(rebornkey, "");
			string[] strfields = rebornboss.Split(new char[]
			{
				','
			});
			if (strfields.Length >= 2)
			{
				this.scoreData.BossExtensionID = Global.SafeConvertToInt32(strfields[0]);
				DateTime.TryParse(strfields[1].Replace('$', ':'), out this.scoreData.BossRefreshTime);
			}
			else
			{
				this.BossState = RebornBossState.RBS_Dead;
			}
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x0010F360 File Offset: 0x0010D560
		public void SaveSceneDBInfo()
		{
			string rebornkey = string.Format("{0}{1}", "reborn_boss_", this.m_nMapCode);
			string rebornboss = string.Format("{0},{1}", this.scoreData.BossExtensionID, this.scoreData.BossRefreshTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'));
			GameManager.GameConfigMgr.SetGameConfigItem(rebornkey, rebornboss);
			Global.UpdateDBGameConfigg(rebornkey, rebornboss);
		}

		// Token: 0x04001A8E RID: 6798
		public int m_nMapCode = 0;

		// Token: 0x04001A8F RID: 6799
		public int m_nLineID;

		// Token: 0x04001A90 RID: 6800
		public List<RebornBossAttackLog> BossRankList = new List<RebornBossAttackLog>();

		// Token: 0x04001A91 RID: 6801
		public Dictionary<int, RebornBossAttackLog> BossRankDict = new Dictionary<int, RebornBossAttackLog>();

		// Token: 0x04001A92 RID: 6802
		public RebornBossScoreData scoreData = new RebornBossScoreData();

		// Token: 0x04001A93 RID: 6803
		public RebornBossState BossState = RebornBossState.RBS_None;

		// Token: 0x04001A94 RID: 6804
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();
	}
}
