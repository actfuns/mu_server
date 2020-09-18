using System;
using System.Collections.Generic;
using KF.Contract.Data;

namespace GameServer.Logic
{
	
	public class BangHuiMatchData
	{
		
		
		public object Mutex
		{
			get
			{
				return this.CommonConfigData.MutexConfig;
			}
		}

		
		public BangHuiMatchCommonData CommonConfigData = new BangHuiMatchCommonData();

		
		public Dictionary<int, BHMatchQiZhiConfig> NPCID2QiZhiConfigDict = new Dictionary<int, BHMatchQiZhiConfig>();

		
		public Dictionary<int, BHMatchBirthPoint> MapBirthPointDict = new Dictionary<int, BHMatchBirthPoint>();

		
		public List<BHMatchRankAwardConfig> RankAwardConfigList_Gold = new List<BHMatchRankAwardConfig>();

		
		public List<BHMatchRankAwardConfig> RankAwardConfigList_Rookie = new List<BHMatchRankAwardConfig>();

		
		public Dictionary<int, BHMatchGoldGuessConfig> BHMatchGoldGuessConfigDict = new Dictionary<int, BHMatchGoldGuessConfig>();

		
		public int BHMatchGodDamagePeriod = 5;

		
		public List<double> BHMatchGodDamagePctList = new List<double>();

		
		public double BHMatchGodDebuffTemple = 0.2;

		
		public double BHMatchGodDebuffQiZhi = 0.05;

		
		public int GoldWingGoodsID = 0;

		
		public Dictionary<int, int> MonsterIDVsDamage = new Dictionary<int, int>();

		
		public int BattleQiZhiMonsterID1 = 0;

		
		public int BattleQiZhiMonsterID2 = 0;

		
		public int TempleProduceTime = 0;

		
		public int TempleProduceNum = 0;

		
		public int MVPScoreFactorKill = 0;

		
		public int MVPScoreFactorQiZhi = 0;

		
		public int MVPScoreFactorTemple = 0;

		
		public bool PrepareGame;

		
		public Dictionary<long, BHMatchFuBenData> FuBenItemData = new Dictionary<long, BHMatchFuBenData>();

		
		public long ChengHaoBHid_Gold = 0L;

		
		public long ChengHaoBHid_Rookie = 0L;

		
		public int BHMatchPKInfoMinSeasonID = int.MaxValue;

		
		public int BHMatchPKInfoMinRound = int.MaxValue;

		
		public long UpdateLongTaOwnInfoTimes = 0L;
	}
}
