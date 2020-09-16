using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class TianTiData
	{
		
		
		public int TotalSecs
		{
			get
			{
				return this.WaitingEnterSecs + this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs;
			}
		}

		
		public object Mutex = new object();

		
		public Dictionary<RangeKey, int> Range2GroupIndexDict = new Dictionary<RangeKey, int>(RangeKey.Comparer);

		
		public Dictionary<int, TianTiBirthPoint> MapBirthPointDict = new Dictionary<int, TianTiBirthPoint>();

		
		public Dictionary<RangeKey, DuanWeiRankAward> DuanWeiRankAwardDict = new Dictionary<RangeKey, DuanWeiRankAward>(RangeKey.Comparer);

		
		public Dictionary<RangeKey, RongYaoRankAward> RongYaoRankAwardDict = new Dictionary<RangeKey, RongYaoRankAward>(RangeKey.Comparer);

		
		public Dictionary<int, int> GameId2FuBenSeq = new Dictionary<int, int>();

		
		public Dictionary<int, int> MapCodeDict = new Dictionary<int, int>();

		
		public List<int> MapCodeList = new List<int>();

		
		public string TimePointsStr;

		
		public List<TimeSpan> TimePoints = new List<TimeSpan>();

		
		public int MaxZhanMengNum = 4;

		
		public TimeSpan RefreshTime = new TimeSpan(3, 0, 0);

		
		public int MinZhuanSheng = 1;

		
		public int MinLevel = 1;

		
		public int MinRequestNum = 1;

		
		public int MaxEnterNum = 10;

		
		public int FuBenId = 13000;

		
		public int HoldShengBeiSecs = 60;

		
		public int MinSubmitShengBeiSecs = 13;

		
		public int TianTiCD = 60;

		
		public int WaitingEnterSecs;

		
		public int PrepareSecs;

		
		public int FightingSecs;

		
		public int ClearRolesSecs;

		
		public Dictionary<int, TianTiDuanWei> TianTiDuanWeiDict = new Dictionary<int, TianTiDuanWei>();

		
		public Dictionary<RangeKey, int> DuanWeiJiFenRangeDuanWeiIdDict = new Dictionary<RangeKey, int>(RangeKey.Comparer);

		
		public int TempleMirageAwardChengJiu;

		
		public int TempleMirageAwardShengWang;

		
		public int WinDuanWeiJiFen = 1000;

		
		public int LoseDuanWeiJiFen = 8;

		
		public int DuanWeiJiFenNum = 5;

		
		public int TempleMirageWinExtraRate = 10;

		
		public int FuBenGoodsBinding = 1;

		
		public int MaxTianTiJiFen = 600000;

		
		public DateTime ModifyTime;

		
		public int MaxPaiMingRank = 100;

		
		public int MaxDayPaiMingListCount = 10;

		
		public int MaxMonthPaiMingListCount = 10;

		
		public Dictionary<int, TianTiPaiHangRoleData> TianTiPaiHangRoleDataDict = new Dictionary<int, TianTiPaiHangRoleData>();

		
		public List<TianTiPaiHangRoleData> TianTiPaiHangRoleDataList = new List<TianTiPaiHangRoleData>();

		
		public Dictionary<int, TianTiPaiHangRoleData> TianTiMonthPaiHangRoleDataDict = new Dictionary<int, TianTiPaiHangRoleData>();

		
		public List<TianTiPaiHangRoleData> TianTiMonthPaiHangRoleDataList = new List<TianTiPaiHangRoleData>();

		
		public ConcurrentDictionary<int, long> TianTiCDDict = new ConcurrentDictionary<int, long>();
	}
}
