using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Tools;

namespace GameServer.Logic
{
	
	public class ZhanDuiZhengBaData
	{
		
		public object Mutex = new object();

		
		public int TotalSecs = 1860;

		
		public List<ZhanDuiZhengBaAwardsConfig> AwardsConfig = new List<ZhanDuiZhengBaAwardsConfig>();

		
		public ZhanDuiZhengBaConfig Config = new ZhanDuiZhengBaConfig();

		
		public int[] TeamBattleWatch = new int[2];

		
		public int[] TeamBattleName = new int[2];

		
		public RunByTime SyncDataByTime = new RunByTime(15000L);

		
		public ZhanDuiZhengBaSyncData SyncDataRequest = new ZhanDuiZhengBaSyncData();

		
		public ZhanDuiZhengBaSyncData SyncData = new ZhanDuiZhengBaSyncData();

		
		public TimeSpan StartTime;

		
		public Dictionary<long, ZhanDuiZhengBaFuBenData> KuaFuCopyDataDict = new Dictionary<long, ZhanDuiZhengBaFuBenData>();

		
		public Queue<Tuple<int, int, int>> PKResultQueue = new Queue<Tuple<int, int, int>>();
	}
}
