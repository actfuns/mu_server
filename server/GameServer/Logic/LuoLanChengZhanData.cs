using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class LuoLanChengZhanData
	{
		
		public object Mutex = new object();

		
		public int MapCode;

		
		public int MapCode_LongTa;

		
		public Dictionary<int, List<MapBirthPoint>> MapBirthPointListDict = new Dictionary<int, List<MapBirthPoint>>();

		
		public Dictionary<int, QiZhiConfig> NPCID2QiZhiConfigDict = new Dictionary<int, QiZhiConfig>();

		
		public long ApplyZhangMengZiJin = 0L;

		
		public int MaxZhanMengNum = 4;

		
		public long BidZhangMengZiJin = 0L;

		
		public int MinZhuanSheng = 0;

		
		public int MinLevel = 0;

		
		public int MinRequestNum = 1;

		
		public int MaxEnterNum = 1000;

		
		public int InstallJunQiNeedMoney = 0;

		
		public long EnrollTime = 1800L;

		
		public int GongNengOpenDaysFromKaiFu = 5;

		
		public TimeSpan NoRequestTimeStart;

		
		public TimeSpan NoRequestTimeEnd;

		
		public int[] WeekPoints = new int[0];

		
		public DateTime TimePoints;

		
		public DateTime WangChengZhanFightingDateTime;

		
		public int WaitingEnterSecs;

		
		public int PrepareSecs;

		
		public int FightingSecs;

		
		public int ClearRolesSecs;

		
		public bool CanRequestState = false;

		
		public Dictionary<int, SiegeWarfareEveryDayAwardsItem> SiegeWarfareEveryDayAwardsDict = new Dictionary<int, SiegeWarfareEveryDayAwardsItem>();

		
		public long ExpAward;

		
		public int ZhanGongAward;

		
		public int ZiJin;

		
		public string WarRequestStr = null;

		
		public Dictionary<int, LuoLanChengZhanRequestInfo> WarRequstDict = new Dictionary<int, LuoLanChengZhanRequestInfo>();

		
		public Dictionary<int, int> BHID2SiteDict = new Dictionary<int, int>();

		
		public List<LuoLanChengZhanRoleCountData> LongTaBHRoleCountList = new List<LuoLanChengZhanRoleCountData>();

		
		public LuoLanChengZhanLongTaOwnerData LongTaOwnerData = new LuoLanChengZhanLongTaOwnerData();

		
		public List<LuoLanChengZhanQiZhiBuffOwnerData> QiZhiBuffOwnerDataList = new List<LuoLanChengZhanQiZhiBuffOwnerData>();

		
		public int SuperQiZhiNpcId = 80000;

		
		public int SuperQiZhiOwnerBirthPosX;

		
		public int SuperQiZhiOwnerBirthPosY;

		
		public int SuperQiZhiOwnerBhid = 0;

		
		public long LastClearMapTicks = 0L;

		
		public DateTime FightEndTime;

		
		public Dictionary<int, double[]> QiZhiBuffDisableParamsDict = new Dictionary<int, double[]>();

		
		public Dictionary<int, double[]> QiZhiBuffEnableParamsDict = new Dictionary<int, double[]>();

		
		public int LuoLanChengZhuBHID;

		
		public string LuoLanChengZhuBHName;

		
		public long LuoLanChengZhuLastLoginTicks;

		
		public GameClient LuoLanChengZhuClient;
	}
}
