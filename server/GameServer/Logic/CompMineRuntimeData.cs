using System;
using System.Collections.Generic;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	public class CompMineRuntimeData
	{
		
		public const string CompMine_FileName = "Config/CompMineWar.xml";

		
		public const string CompMineTruck_FileName = "Config/CompMineTruck.xml";

		
		public const string CompMineLink_FileName = "Config/CompMineLink.xml";

		
		public const string CompMineBirth_FileName = "Config/CompMineBirthPoint.xml";

		
		public const string CompMineReward_FileName = "Config/CompMineAward.xml";

		
		public object Mutex = new object();

		
		public Dictionary<int, CompMineConfig> CompMineConfigDict = new Dictionary<int, CompMineConfig>();

		
		public List<CompMineTruckConfig> CompMineTruckConfigList = new List<CompMineTruckConfig>();

		
		public List<CompMineLinkConfig> CompMineLinkConfigList = new List<CompMineLinkConfig>();

		
		public Dictionary<int, CompMineBirthConfig> CompMineBirthConfigDict = new Dictionary<int, CompMineBirthConfig>();

		
		public List<CompMineRewardConfig> CompMineRewardConfigList = new List<CompMineRewardConfig>();

		
		public int[] CompMineAttackKill;

		
		public int[] CompMineAttackShutDown;

		
		public int CompMineDie;

		
		public double[] CompMineAwardNum;

		
		public Dictionary<int, CompFuBenData> FuBenItemData = new Dictionary<int, CompFuBenData>();
	}
}
