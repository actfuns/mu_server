using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Tools;

namespace GameServer.Logic
{
	
	public class EscapeBattleData
	{
		
		public object Mutex = new object();

		
		public List<EscapeBattleAwardsConfig> AwardsConfig = new List<EscapeBattleAwardsConfig>();

		
		public List<EscapeBDuanAwardsConfig> DuanAwardsConfig = new List<EscapeBDuanAwardsConfig>();

		
		public Dictionary<int, EscapeBattleBirthPoint> MapBirthPointDict = new Dictionary<int, EscapeBattleBirthPoint>();

		
		public List<EscapeBattleCollection> CollectionConfigList = new List<EscapeBattleCollection>();

		
		public List<EscapeMapSafeArea> EscapeMapSafeAreaList = new List<EscapeMapSafeArea>();

		
		public EscapeBattleConfig Config = new EscapeBattleConfig();

		
		public int BuyDevilLossDiamonds = 5;

		
		public int KillReplyHp = 50000;

		
		public int[] BuffMaxLayerNum;

		
		public double[] BuffAttributeProportion;

		
		public int[] BuffAttributeType;

		
		public List<int[]> LifeSeedNum = new List<int[]>();

		
		public int MaxLifeNum = 5;

		
		public int DevilLossNum;

		
		public int ReadyMapCode;

		
		public DateTime EscapeStartTime;

		
		public double[] DebuffCalExtProps = new double[177];

		
		public int[] TeamBattleMap;

		
		public RunByTime SyncDataByTime = new RunByTime(15000L);

		
		public EscapeBattleSyncData SyncDataRequest = new EscapeBattleSyncData();

		
		public EscapeBattleSyncData SyncData = new EscapeBattleSyncData();

		
		public TimeSpan DiffTimeSpan = TimeSpan.Zero;

		
		public int LastStartSecs;

		
		public Dictionary<long, EscapeBattleFuBenData> KuaFuCopyDataDict = new Dictionary<long, EscapeBattleFuBenData>();

		
		public Dictionary<int, EscapeBattlePiPeiState> ConfirmBattleDict = new Dictionary<int, EscapeBattlePiPeiState>();

		
		public Queue<KeyValuePair<int, List<int>>> PKResultQueue = new Queue<KeyValuePair<int, List<int>>>();

		
		public Queue<KeyValuePair<int, int>> GameStateQueue = new Queue<KeyValuePair<int, int>>();
	}
}
