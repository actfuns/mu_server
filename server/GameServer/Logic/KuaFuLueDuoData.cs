using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Server.Data;

namespace GameServer.Logic
{
	
	public class KuaFuLueDuoData
	{
		
		
		public object Mutex
		{
			get
			{
				return this.CommonConfigData.MutexConfig;
			}
		}

		
		public KuaFuLueDuoCommonData CommonConfigData = new KuaFuLueDuoCommonData();

		
		public Dictionary<int, KuaFuLueDuoMonsterItem> CollectMonsterDict = new Dictionary<int, KuaFuLueDuoMonsterItem>();

		
		public Dictionary<int, QiZhiConfig> NPCID2QiZhiConfigDict = new Dictionary<int, QiZhiConfig>();

		
		public Dictionary<int, MapBirthPoint> MapBirthPointDict = new Dictionary<int, MapBirthPoint>();

		
		public HashSet<int> HideRankList = new HashSet<int>();

		
		public Dictionary<int, KuaFuLueDuoStoreConfig> KingOfBattleStoreDict = new Dictionary<int, KuaFuLueDuoStoreConfig>();

		
		public List<KuaFuLueDuoStoreConfig> KingOfBattleStoreList = new List<KuaFuLueDuoStoreConfig>();

		
		public int BeginNum;

		
		public int EndNum;

		
		public double[] CrusadeOrePercent;

		
		public int[] CrusadeUltraKill;

		
		public int[] CrusadeShutDown;

		
		public double[] CrusadeAwardAttacker;

		
		public double[] CrusadeAwardDefender;

		
		public int CrusadeSeason;

		
		public int[] CrusadeOre;

		
		public int CrusadeMinApply;

		
		public int CrusadeApplyCD;

		
		public int[] CrusadeEnterTime;

		
		public int[] CrusadeEnterPrice;

		
		public double CrusadePerfect;

		
		public int CrusadeStoreCD;

		
		public int CrusadeStorePrice;

		
		public int CrusadeStoreRandomNum;

		
		public int[] ZhanMengZiJin;

		
		public int GoldWingGoodsID = 0;

		
		public bool PrepareGame;

		
		public Dictionary<long, KuaFuLueDuoFuBenData> FuBenItemData = new Dictionary<long, KuaFuLueDuoFuBenData>();

		
		public Dictionary<int, KuaFuLueDuoBangHuiJingJiaData> JingJiaDict = new Dictionary<int, KuaFuLueDuoBangHuiJingJiaData>();

		
		public bool UpdateZiYuanData;

		
		public Dictionary<int, FightInfo> ServerZiYuanDict = new Dictionary<int, FightInfo>();

		
		public Dictionary<int, FightInfo> BhZiYuanDict = new Dictionary<int, FightInfo>();

		
		public Dictionary<int, int> CacheRole2KillDict = new Dictionary<int, int>();

		
		public Dictionary<int, int> CacheBh2LueDuoDict = new Dictionary<int, int>();

		
		public long ChengHaoBHid = 0L;

		
		public long ChengHaoBHid_Week = 0L;
	}
}
