using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	public class CompScene
	{
		
		public void LoadCompSceneDBInfo()
		{
			string compkey = string.Format("{0}{1}", "comp_monster_", this.CompSceneInfo.ID);
			string compmonster = GameManager.GameConfigMgr.GetGameConfigItemStr(compkey, "");
			string[] strfields = compmonster.Split(new char[]
			{
				','
			});
			if (strfields.Length >= 4)
			{
				this.ResourceNum = Global.SafeConvertToInt32(strfields[0]);
				this.ResourceGrowUpNum = Global.SafeConvertToInt32(strfields[1]);
				this.SolderRefreshTimeMS = Global.SafeConvertToInt64(strfields[2]);
				this.SolderNum = Global.SafeConvertToInt32(strfields[3]);
				if (strfields.Length >= 5)
				{
					this.BossMaxLifeFactor = Global.SafeConvertToDouble(strfields[4]);
				}
				this.BossMaxLifeFactor = Math.Max(1.0, this.BossMaxLifeFactor);
			}
		}

		
		public void SaveCompSceneDBInfo()
		{
			string compkey = string.Format("{0}{1}", "comp_monster_", this.CompSceneInfo.ID);
			string compmonster = string.Format("{0},{1},{2},{3},{4:0.000}", new object[]
			{
				this.ResourceNum,
				this.ResourceGrowUpNum,
				this.SolderRefreshTimeMS,
				this.SolderNum,
				this.BossMaxLifeFactor
			});
			GameManager.GameConfigMgr.SetGameConfigItem(compkey, compmonster);
			Global.UpdateDBGameConfigg(compkey, compmonster);
		}

		
		public int m_nMapCode = 0;

		
		public int ResourceNum = 0;

		
		public int ResourceGrowUpNum = 0;

		
		public long SolderRefreshTimeMS = 0L;

		
		public int SolderNum = 0;

		
		public double BossMaxLifeFactor = 1.0;

		
		public CompConfig CompSceneInfo;

		
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();

		
		public List<CompSolderSiteConfig> CompSolderSiteConfigList = new List<CompSolderSiteConfig>();

		
		public CompBattleScoreData ScoreData = new CompBattleScoreData();

		
		public Dictionary<int, CompMapClientContextData> ClientContextDataDict = new Dictionary<int, CompMapClientContextData>();

		
		public Dictionary<int, CoolDownItem> CompNoticeCoolDownDict = new Dictionary<int, CoolDownItem>();
	}
}
