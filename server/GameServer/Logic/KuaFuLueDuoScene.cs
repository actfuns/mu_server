using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class KuaFuLueDuoScene
	{
		
		public int FuBenSeqId = 0;

		
		public long StartTimeTicks = 0L;

		
		public long m_lPrepareTime = 0L;

		
		public long m_lBeginTime = 0L;

		
		public long m_lEndTime = 0L;

		
		public long m_lLeaveTime = 0L;

		
		public GameSceneStatuses m_eStatus = GameSceneStatuses.STATUS_NULL;

		
		public bool m_bEndFlag = false;

		
		public int GameId;

		
		public CopyMap CopyMap;

		
		public KuaFuLueDuoConfig SceneInfo;

		
		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		
		public Dictionary<int, KuaFuLueDuoClientContextData> ClientContextDataDict = new Dictionary<int, KuaFuLueDuoClientContextData>();

		
		public Dictionary<int, KuaFuLueDuoBangHuiContextData> BangHuiContextDataDict = new Dictionary<int, KuaFuLueDuoBangHuiContextData>();

		
		public KuaFuLueDuoClientContextData ClientContextMVP = new KuaFuLueDuoClientContextData();

		
		public Dictionary<int, KuaFuLueDuoMonsterItem> CollectMonsterXml = new Dictionary<int, KuaFuLueDuoMonsterItem>();

		
		public KuaFuLueDuoStatisticalData GameStatisticalData = new KuaFuLueDuoStatisticalData();

		
		public KuaFuLueDuoFuBenData ThisFuBenData;

		
		public QiZhiConfig QiZhiItem;

		
		public int LeftZiYuan;

		
		public int TotalZiYuan;

		
		public int SmallZiYuanCount;

		
		public int BigZiYuanCount;

		
		public int MapGridWidth = 100;

		
		public int MapGridHeight = 100;
	}
}
