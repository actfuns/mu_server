using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	public class ZhengDuoRuntimeData
	{
		
		public object Mutex = new object();

		
		public Dictionary<int, ZhengDuoSignUpData> FightDataDict = new Dictionary<int, ZhengDuoSignUpData>();

		
		public ZhengDuoRankData[] ZhengDuoRankDatas = new ZhengDuoRankData[16];

		
		public Dictionary<int, ZhengDuoRankData> bhid2ZhengDuoRankDataDict = new Dictionary<int, ZhengDuoRankData>();

		
		public int Rank1Bhid;

		
		public long Age;

		
		public int ZhengDuoStep = 0;

		
		public int State = 0;

		
		public int WeekDay = 0;

		
		public ConcurrentDictionary<int, ZhengDuoScene> SceneDict = new ConcurrentDictionary<int, ZhengDuoScene>();

		
		public Dictionary<long, ZhengDuoFuBenData> FuBenItemData = new Dictionary<long, ZhengDuoFuBenData>();

		
		public Dictionary<int, ZhengDuoFuBenData> FuBenItemDataByBhid = new Dictionary<int, ZhengDuoFuBenData>();

		
		public long NextHeartBeatTicks = 0L;
	}
}
