using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class HongBaoRuntimeData
	{
		
		public object Mutex = new object();

		
		public int RedPacketsTime;

		
		public int RedPacketsRight;

		
		public int RedPacketsNumMax;

		
		public int RedPacketsMessage;

		
		public int[] RedPacketsLimit;

		
		public int[] RedPacketsInfomationLimit;

		
		public int RedPacketsRequest;

		
		public int LoadFromDBInterval1 = 60000;

		
		public int LoadFromDBInterval2 = 60000;

		
		public string RedPacketsQuanMinMessage;

		
		public string RedPacketsChongZhiMessage;

		
		public string RedPacketsTeQuanMessage;

		
		public int RedPacketsAutomaticRecordMax;

		
		public int RedPacketsRankLimit = 10;

		
		public long AddChargeValue = 0L;

		
		public Dictionary<int, int> BangHuiHongBaoIdDict = new Dictionary<int, int>();

		
		public Dictionary<int, HongBaoSendData> OldHongBaoDict = new Dictionary<int, HongBaoSendData>();

		
		public Dictionary<int, HongBaoSendData> HongBaoDict = new Dictionary<int, HongBaoSendData>();

		
		public Dictionary<int, ZhanMengHongBaoData> ZhanMengHongBaoDict = new Dictionary<int, ZhanMengHongBaoData>();

		
		public Dictionary<int, SystemHongBaoData> ChongZhiHongBaoDict = new Dictionary<int, SystemHongBaoData>();

		
		public Dictionary<int, List<HongBaoRecvData>> ChongZhiHongBaoRecvDict = new Dictionary<int, List<HongBaoRecvData>>();

		
		public Dictionary<int, HongBaoSendData> JieRiHongBaoDict = new Dictionary<int, HongBaoSendData>();

		
		public Dictionary<int, HongBaoSendData> SpecPHongBaoDict = new Dictionary<int, HongBaoSendData>();

		
		public KuaFuData<List<HongBaoRankItemData>> RecvRankList = new KuaFuData<List<HongBaoRankItemData>>();

		
		public Dictionary<int, HongBaoSendData> DelayUpdateDict = new Dictionary<int, HongBaoSendData>();

		
		public HashSet<int> DestoryBhIds = new HashSet<int>();

		
		public bool Initialized = false;

		
		public bool ZhanMengHongBaoInitialized = false;

		
		public bool JieRiHongBaoInitialized = false;

		
		public bool JieRiHongBaoBangInitialized = false;

		
		public bool SpecPHongBaoInitialized = false;

		
		public long NextCheckExpireTicks;

		
		public long NextTicks1;

		
		public long NextTicks3;
	}
}
