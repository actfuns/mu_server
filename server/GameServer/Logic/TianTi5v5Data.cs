using System;
using System.Collections.Generic;
using KF.Contract.Data;

namespace GameServer.Logic
{
	
	public class TianTi5v5Data
	{
		
		
		public int TotalSecs
		{
			get
			{
				return this.WaitingEnterSecs + this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs;
			}
		}

		
		
		
		public DateTime ModifyTime
		{
			get
			{
				return this.RankData.ModifyTime;
			}
			set
			{
				this.RankData.ModifyTime = value;
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

		
		public int MaxZhanDuiNum = 1000;

		
		public TimeSpan RefreshTime = new TimeSpan(3, 0, 0);

		
		public int MinZhuanSheng = 1;

		
		public int MinLevel = 1;

		
		public int MinRequestNum = 1;

		
		public int MaxSignUp = 65;

		
		public int TianTi5v5CD = 60;

		
		public int WaitingEnterSecs;

		
		public int PrepareSecs;

		
		public int FightingSecs;

		
		public int ClearRolesSecs;

		
		public Dictionary<int, TianTiDuanWei> TianTi5v5DuanWeiDict = new Dictionary<int, TianTiDuanWei>();

		
		public Dictionary<RangeKey, int> DuanWeiJiFenRangeDuanWeiIdDict = new Dictionary<RangeKey, int>(RangeKey.Comparer);

		
		public int[] TeamBattleMap;

		
		public int[] TeamBattleWatch;

		
		public int[] TeamBattleNameRange;

		
		public int[] TeamApply;

		
		public int ZhanDuiJinZuan = 300;

		
		public int MinConfirmFightTeamCnt = 1;

		
		public Tuple<int, int> ZhanDuiDengJiTp = new Tuple<int, int>(5, 1);

		
		public Tuple<int, int> LvLimit = new Tuple<int, int>(5, 1);

		
		public int MaxTeamCnt = 5;

		
		public int MaxTianTi5v5JiFen = 600000;

		
		public int TeamConfirmTime = 30;

		
		public int TeamAwardLimit = 10;

		
		public int TeamPipeiTime = 60;

		
		public int MaxPaiMingRank = 100;

		
		public int MaxDayPaiMingListCount = 10;

		
		public int MaxMonthPaiMingListCount = 10;

		
		public int MinDayPaiMingListCount = 3;

		
		public TianTi5v5RankData RankData = new TianTi5v5RankData();

		
		public Dictionary<int, TianTi5v5ZhanDuiData> ZhanDuiDataPaiHangDict = new Dictionary<int, TianTi5v5ZhanDuiData>();

		
		public List<TianTi5v5ZhanDuiData> ZhanDuiDataPaiHangList = new List<TianTi5v5ZhanDuiData>();

		
		public Dictionary<int, TianTi5v5ZhanDuiData> ZhanDuiDataMonthPaiHangDict = new Dictionary<int, TianTi5v5ZhanDuiData>();

		
		public List<TianTi5v5ZhanDuiData> ZhanDuiDataMonthPaiHangList = new List<TianTi5v5ZhanDuiData>();

		
		public AgeDataT<List<TianTi5v5ZhanDuiMiniData>> ZhanDuiSimpleList = new AgeDataT<List<TianTi5v5ZhanDuiMiniData>>(0L, new List<TianTi5v5ZhanDuiMiniData>());

		
		public Dictionary<int, KuaFu5v5FuBenData> FuBenDataDict = new Dictionary<int, KuaFu5v5FuBenData>();

		
		public Dictionary<int, TianTi5v5FuBenItem> TianTi5v5FuBenItemDict = new Dictionary<int, TianTi5v5FuBenItem>();

		
		public Dictionary<int, AgeDataT<TianTi5v5ZhanDuiData>> ZhanDuiDataAgeDict = new Dictionary<int, AgeDataT<TianTi5v5ZhanDuiData>>();

		
		public Dictionary<int, List<int>> ZhanDuiInviteListDict = new Dictionary<int, List<int>>();

		
		public Dictionary<int, List<TianTi5v5ZhanDuiRoleData>> ZhanDuiRequestListDict = new Dictionary<int, List<TianTi5v5ZhanDuiRoleData>>();

		
		public Dictionary<long, long> RoleRequestZhanDuiTicksDict = new Dictionary<long, long>();

		
		public Dictionary<int, TianTi5v5PiPeiState> ConfirmBattleDict = new Dictionary<int, TianTi5v5PiPeiState>();
	}
}
