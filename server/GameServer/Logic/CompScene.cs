using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x0200026C RID: 620
	public class CompScene
	{
		// Token: 0x060008B1 RID: 2225 RVA: 0x00086C68 File Offset: 0x00084E68
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

		// Token: 0x060008B2 RID: 2226 RVA: 0x00086D38 File Offset: 0x00084F38
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

		// Token: 0x04000F69 RID: 3945
		public int m_nMapCode = 0;

		// Token: 0x04000F6A RID: 3946
		public int ResourceNum = 0;

		// Token: 0x04000F6B RID: 3947
		public int ResourceGrowUpNum = 0;

		// Token: 0x04000F6C RID: 3948
		public long SolderRefreshTimeMS = 0L;

		// Token: 0x04000F6D RID: 3949
		public int SolderNum = 0;

		// Token: 0x04000F6E RID: 3950
		public double BossMaxLifeFactor = 1.0;

		// Token: 0x04000F6F RID: 3951
		public CompConfig CompSceneInfo;

		// Token: 0x04000F70 RID: 3952
		public SortedList<long, List<object>> CreateMonsterQueue = new SortedList<long, List<object>>();

		// Token: 0x04000F71 RID: 3953
		public List<CompSolderSiteConfig> CompSolderSiteConfigList = new List<CompSolderSiteConfig>();

		// Token: 0x04000F72 RID: 3954
		public CompBattleScoreData ScoreData = new CompBattleScoreData();

		// Token: 0x04000F73 RID: 3955
		public Dictionary<int, CompMapClientContextData> ClientContextDataDict = new Dictionary<int, CompMapClientContextData>();

		// Token: 0x04000F74 RID: 3956
		public Dictionary<int, CoolDownItem> CompNoticeCoolDownDict = new Dictionary<int, CoolDownItem>();
	}
}
