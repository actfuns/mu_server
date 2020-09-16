using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class YongZheZhanChangData
	{
		
		public object Mutex = new object();

		
		public Dictionary<int, YongZheZhanChangBirthPoint> MapBirthPointDict = new Dictionary<int, YongZheZhanChangBirthPoint>();

		
		public Dictionary<RangeKey, YongZheZhanChangSceneInfo> LevelRangeSceneIdDict = new Dictionary<RangeKey, YongZheZhanChangSceneInfo>(RangeKey.Comparer);

		
		public Dictionary<int, YongZheZhanChangSceneInfo> SceneDataDict = new Dictionary<int, YongZheZhanChangSceneInfo>();

		
		public Dictionary<int, BattleCrystalMonsterItem> BattleCrystalMonsterDict = new Dictionary<int, BattleCrystalMonsterItem>();

		
		public Dictionary<int, KuaFuServerLoginData> RoleIdKuaFuLoginDataDict = new Dictionary<int, KuaFuServerLoginData>();

		
		public Dictionary<int, KuaFuServerLoginData> NotifyRoleEnterDict = new Dictionary<int, KuaFuServerLoginData>();

		
		public ConcurrentDictionary<int, int> RoleId2JoinGroup = new ConcurrentDictionary<int, int>();

		
		public Dictionary<int, List<BattleDynamicMonsterItem>> SceneDynMonsterDict = new Dictionary<int, List<BattleDynamicMonsterItem>>();

		
		public double WarriorBattleBossAttackPercent = 0.001;

		
		public int WarriorBattleBossAttackScore = 20;

		
		public int WarriorBattleBOssLastAttack = 20;

		
		public int WarriorBattlePk = 8;

		
		public int WarriorBattleLowestJiFen = 5;

		
		public int WarriorBattleDie = 5;

		
		public int WarriorBattleUltraKillParam1 = 27;

		
		public int WarriorBattleUltraKillParam2 = 3;

		
		public int WarriorBattleUltraKillParam3 = 30;

		
		public int WarriorBattleUltraKillParam4 = 75;

		
		public int WarriorBattleShutDownParam1 = -10;

		
		public int WarriorBattleShutDownParam2 = 5;

		
		public int WarriorBattleShutDownParam3 = 0;

		
		public int WarriorBattleShutDownParam4 = 100;

		
		public int SighUpStateMagicNum = 100000000;

		
		public string RoleParamsAwardsDefaultString = "";

		
		public bool PrepareGame;

		
		public Dictionary<int, YongZheZhanChangFuBenData> FuBenItemData = new Dictionary<int, YongZheZhanChangFuBenData>();
	}
}
