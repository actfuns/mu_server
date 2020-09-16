using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class KingOfBattleData
	{
		
		public object Mutex = new object();

		
		public Dictionary<int, YongZheZhanChangBirthPoint> MapBirthPointDict = new Dictionary<int, YongZheZhanChangBirthPoint>();

		
		public Dictionary<RangeKey, KingOfBattleSceneInfo> LevelRangeSceneIdDict = new Dictionary<RangeKey, KingOfBattleSceneInfo>(RangeKey.Comparer);

		
		public Dictionary<int, KingOfBattleSceneInfo> SceneDataDict = new Dictionary<int, KingOfBattleSceneInfo>();

		
		public Dictionary<int, KingOfBattleStoreConfig> KingOfBattleStoreDict = new Dictionary<int, KingOfBattleStoreConfig>();

		
		public List<KingOfBattleStoreConfig> KingOfBattleStoreList = new List<KingOfBattleStoreConfig>();

		
		public Dictionary<int, BattleCrystalMonsterItem> BattleCrystalMonsterDict = new Dictionary<int, BattleCrystalMonsterItem>();

		
		public Dictionary<int, KuaFuServerLoginData> RoleIdKuaFuLoginDataDict = new Dictionary<int, KuaFuServerLoginData>();

		
		public Dictionary<int, KuaFuServerLoginData> NotifyRoleEnterDict = new Dictionary<int, KuaFuServerLoginData>();

		
		public ConcurrentDictionary<int, int> RoleId2JoinGroup = new ConcurrentDictionary<int, int>();

		
		public Dictionary<int, List<KingOfBattleDynamicMonsterItem>> SceneDynMonsterDict = new Dictionary<int, List<KingOfBattleDynamicMonsterItem>>();

		
		public Dictionary<int, KingOfBattleDynamicMonsterItem> DynMonsterDict = new Dictionary<int, KingOfBattleDynamicMonsterItem>();

		
		public Dictionary<int, KingOfBattleQiZhiConfig> NPCID2QiZhiConfigDict = new Dictionary<int, KingOfBattleQiZhiConfig>();

		
		public int BattleQiZhiMonsterID1 = 8800003;

		
		public int BattleQiZhiMonsterID2 = 8800004;

		
		public int KingOfBattleDamageJunQi = 1;

		
		public int KingOfBattleDamageTower = 1;

		
		public int KingOfBattleDamageCenter = 1;

		
		public int KingOfBattleDie = 5;

		
		public int KingOfBattleLowestJiFen = 5;

		
		public double KingBattleBossAttackPercent = 0.001;

		
		public int KingOfBattleUltraKillParam1 = 27;

		
		public int KingOfBattleUltraKillParam2 = 3;

		
		public int KingOfBattleUltraKillParam3 = 30;

		
		public int KingOfBattleUltraKillParam4 = 75;

		
		public int KingOfBattleShutDownParam1 = -10;

		
		public int KingOfBattleShutDownParam2 = 5;

		
		public int KingOfBattleShutDownParam3 = 0;

		
		public int KingOfBattleShutDownParam4 = 100;

		
		public int KingOfBattleStoreRefreshTm = 24;

		
		public int KingOfBattleStoreRefreshNum = 6;

		
		public int KingOfBattleStoreRefreshCost = 100;

		
		public int SighUpStateMagicNum = 100000000;

		
		public string RoleParamsAwardsDefaultString = "";

		
		public bool PrepareGame;

		
		public Dictionary<int, YongZheZhanChangFuBenData> FuBenItemData = new Dictionary<int, YongZheZhanChangFuBenData>();
	}
}
