using System;
using System.Collections.Generic;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	public class CompBattleRuntimeData
	{
		
		public const string CompBattle_FileName = "Config/ForceCraft.xml";

		
		public const string CompStronghold_FileName = "Config/ForceStronghold.xml";

		
		public const string CompBattleBirth_FileName = "Config/ForceCraftBirth.xml";

		
		public const string CompBattleReward_FileName = "Config/ForceCraftReward.xml";

		
		public object Mutex = new object();

		
		public Dictionary<int, CompBattleConfig> CompBattleConfigDict = new Dictionary<int, CompBattleConfig>();

		
		public Dictionary<int, CompStrongholdConfig> CompStrongholdConfigDict = new Dictionary<int, CompStrongholdConfig>();

		
		public Dictionary<KeyValuePair<int, int>, CompBattleBirthConfig> CompBattleBirthConfigDict = new Dictionary<KeyValuePair<int, int>, CompBattleBirthConfig>();

		
		public List<CompBattleRewardConfig> CompBattleRewardConfigList = new List<CompBattleRewardConfig>();

		
		public int[] CompBattleSingleIntegral;

		
		public double[] CompBattleRewardRate;

		
		public int CompBattleFlagDamage = 10;

		
		public Dictionary<int, CompFuBenData> FuBenItemData = new Dictionary<int, CompFuBenData>();

		
		public Dictionary<KeyValuePair<int, int>, List<CompBattleWaitData>> CompBattleWaitQueueDict = new Dictionary<KeyValuePair<int, int>, List<CompBattleWaitData>>();

		
		public Dictionary<int, KeyValuePair<int, int>> CompBattleWaitAllDict = new Dictionary<int, KeyValuePair<int, int>>();

		
		public CompBattleBaseData compBattleBaseData = new CompBattleBaseData();
	}
}
