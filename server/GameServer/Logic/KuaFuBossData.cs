using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class KuaFuBossData
	{
		
		public object Mutex = new object();

		
		public Dictionary<int, KuaFuBossBirthPoint> MapBirthPointDict = new Dictionary<int, KuaFuBossBirthPoint>();

		
		public Dictionary<RangeKey, KuaFuBossSceneInfo> LevelRangeSceneIdDict = new Dictionary<RangeKey, KuaFuBossSceneInfo>(RangeKey.Comparer);

		
		public Dictionary<int, KuaFuBossSceneInfo> SceneDataDict = new Dictionary<int, KuaFuBossSceneInfo>();

		
		public Dictionary<int, List<BattleDynamicMonsterItem>> SceneDynMonsterDict = new Dictionary<int, List<BattleDynamicMonsterItem>>();

		
		public Dictionary<int, KuaFuServerLoginData> RoleIdKuaFuLoginDataDict = new Dictionary<int, KuaFuServerLoginData>();

		
		public Dictionary<int, KuaFuServerLoginData> NotifyRoleEnterDict = new Dictionary<int, KuaFuServerLoginData>();

		
		public ConcurrentDictionary<int, int> RoleId2JoinGroup = new ConcurrentDictionary<int, int>();

		
		public bool PrepareGame;

		
		public Dictionary<int, YongZheZhanChangFuBenData> FuBenItemData = new Dictionary<int, YongZheZhanChangFuBenData>();
	}
}
