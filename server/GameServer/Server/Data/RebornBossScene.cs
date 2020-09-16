using System;
using System.Collections.Generic;
using GameServer.Logic;

namespace Server.Data
{
	
	public class RebornBossScene
	{
		
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

		
		public void SaveSceneDBInfo()
		{
			string rebornkey = string.Format("{0}{1}", "reborn_boss_", this.m_nMapCode);
			string rebornboss = string.Format("{0},{1}", this.scoreData.BossExtensionID, this.scoreData.BossRefreshTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'));
			GameManager.GameConfigMgr.SetGameConfigItem(rebornkey, rebornboss);
			Global.UpdateDBGameConfigg(rebornkey, rebornboss);
		}

		
		public int m_nMapCode = 0;

		
		public int m_nLineID;

		
		public List<RebornBossAttackLog> BossRankList = new List<RebornBossAttackLog>();

		
		public Dictionary<int, RebornBossAttackLog> BossRankDict = new Dictionary<int, RebornBossAttackLog>();

		
		public RebornBossScoreData scoreData = new RebornBossScoreData();

		
		public RebornBossState BossState = RebornBossState.RBS_None;

		
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();
	}
}
