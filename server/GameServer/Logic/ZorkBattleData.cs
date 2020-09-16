using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	public class ZorkBattleData
	{
		
		public object Mutex = new object();

		
		public Dictionary<int, ZorkBattleBirthPoint> MapBirthPointDict = new Dictionary<int, ZorkBattleBirthPoint>();

		
		public Dictionary<int, ZorkBattleSceneInfo> SceneDataDict = new Dictionary<int, ZorkBattleSceneInfo>();

		
		public List<ZorkBattleArmyConfig> ZorkBattleArmyList = new List<ZorkBattleArmyConfig>();

		
		public Dictionary<int, List<ZorkBattleMonsterConfig>> ZorkBattleMonsterDict = new Dictionary<int, List<ZorkBattleMonsterConfig>>();

		
		public Dictionary<int, ZorkAchievementConfig> ZorkAchievementDict = new Dictionary<int, ZorkAchievementConfig>();

		
		public List<ZorkBattleAwardConfig> ZorkLevelRangeList = new List<ZorkBattleAwardConfig>();

		
		public Dictionary<int, KuaFuServerLoginData> RoleIdKuaFuLoginDataDict = new Dictionary<int, KuaFuServerLoginData>();

		
		public Dictionary<int, KuaFuServerLoginData> NotifyRoleEnterDict = new Dictionary<int, KuaFuServerLoginData>();

		
		public ConcurrentDictionary<int, int> RoleId2JoinGroup = new ConcurrentDictionary<int, int>();

		
		public int BossHurtCleanTime;

		
		public HashSet<int> ZorkWarEnterMapSet = new HashSet<int>();

		
		public int ZorkBattleUltraKillParam1 = 27;

		
		public int ZorkBattleUltraKillParam2 = 3;

		
		public int ZorkBattleUltraKillParam3 = 30;

		
		public int ZorkBattleUltraKillParam4 = 75;

		
		public int ZorkBattleShutDownParam1 = -10;

		
		public int ZorkBattleShutDownParam2 = 5;

		
		public int ZorkBattleShutDownParam3 = 0;

		
		public int ZorkBattleShutDownParam4 = 100;

		
		public int ZorkEnterPlayNumMin = 4;

		
		public DateTime ZorkStartTime;

		
		public Dictionary<int, KuaFu5v5FuBenData> FuBenItemData = new Dictionary<int, KuaFu5v5FuBenData>();
	}
}
