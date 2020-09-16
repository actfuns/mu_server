using System;
using System.Collections.Generic;
using KF.Contract.Data;

namespace GameServer.Logic
{
	// Token: 0x02000210 RID: 528
	public class BangHuiMatchData
	{
		// Token: 0x1700001C RID: 28
		
		public object Mutex
		{
			get
			{
				return this.CommonConfigData.MutexConfig;
			}
		}

		// Token: 0x04000BCC RID: 3020
		public BangHuiMatchCommonData CommonConfigData = new BangHuiMatchCommonData();

		// Token: 0x04000BCD RID: 3021
		public Dictionary<int, BHMatchQiZhiConfig> NPCID2QiZhiConfigDict = new Dictionary<int, BHMatchQiZhiConfig>();

		// Token: 0x04000BCE RID: 3022
		public Dictionary<int, BHMatchBirthPoint> MapBirthPointDict = new Dictionary<int, BHMatchBirthPoint>();

		// Token: 0x04000BCF RID: 3023
		public List<BHMatchRankAwardConfig> RankAwardConfigList_Gold = new List<BHMatchRankAwardConfig>();

		// Token: 0x04000BD0 RID: 3024
		public List<BHMatchRankAwardConfig> RankAwardConfigList_Rookie = new List<BHMatchRankAwardConfig>();

		// Token: 0x04000BD1 RID: 3025
		public Dictionary<int, BHMatchGoldGuessConfig> BHMatchGoldGuessConfigDict = new Dictionary<int, BHMatchGoldGuessConfig>();

		// Token: 0x04000BD2 RID: 3026
		public int BHMatchGodDamagePeriod = 5;

		// Token: 0x04000BD3 RID: 3027
		public List<double> BHMatchGodDamagePctList = new List<double>();

		// Token: 0x04000BD4 RID: 3028
		public double BHMatchGodDebuffTemple = 0.2;

		// Token: 0x04000BD5 RID: 3029
		public double BHMatchGodDebuffQiZhi = 0.05;

		// Token: 0x04000BD6 RID: 3030
		public int GoldWingGoodsID = 0;

		// Token: 0x04000BD7 RID: 3031
		public Dictionary<int, int> MonsterIDVsDamage = new Dictionary<int, int>();

		// Token: 0x04000BD8 RID: 3032
		public int BattleQiZhiMonsterID1 = 0;

		// Token: 0x04000BD9 RID: 3033
		public int BattleQiZhiMonsterID2 = 0;

		// Token: 0x04000BDA RID: 3034
		public int TempleProduceTime = 0;

		// Token: 0x04000BDB RID: 3035
		public int TempleProduceNum = 0;

		// Token: 0x04000BDC RID: 3036
		public int MVPScoreFactorKill = 0;

		// Token: 0x04000BDD RID: 3037
		public int MVPScoreFactorQiZhi = 0;

		// Token: 0x04000BDE RID: 3038
		public int MVPScoreFactorTemple = 0;

		// Token: 0x04000BDF RID: 3039
		public bool PrepareGame;

		// Token: 0x04000BE0 RID: 3040
		public Dictionary<long, BHMatchFuBenData> FuBenItemData = new Dictionary<long, BHMatchFuBenData>();

		// Token: 0x04000BE1 RID: 3041
		public long ChengHaoBHid_Gold = 0L;

		// Token: 0x04000BE2 RID: 3042
		public long ChengHaoBHid_Rookie = 0L;

		// Token: 0x04000BE3 RID: 3043
		public int BHMatchPKInfoMinSeasonID = int.MaxValue;

		// Token: 0x04000BE4 RID: 3044
		public int BHMatchPKInfoMinRound = int.MaxValue;

		// Token: 0x04000BE5 RID: 3045
		public long UpdateLongTaOwnInfoTimes = 0L;
	}
}
