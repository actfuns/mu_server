using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class HuanYingSiYuanData
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

		
		public Dictionary<int, HuanYingSiYuanBirthPoint> MapBirthPointDict = new Dictionary<int, HuanYingSiYuanBirthPoint>();

		
		public Dictionary<int, ContinuityKillAward> ContinuityKillAwardDict = new Dictionary<int, ContinuityKillAward>();

		
		public int MapCode;

		
		public List<TimeSpan> TimePoints = new List<TimeSpan>();

		
		public int MinZhuanSheng = 1;

		
		public int MinLevel = 1;

		
		public int MinRequestNum = 1;

		
		public int MaxEnterNum = 10;

		
		public int FuBenId = 13000;

		
		public int HoldShengBeiSecs = 60;

		
		public int MinSubmitShengBeiSecs = 13;

		
		public int TempleMirageMinJiFen = 0;

		
		public int WaitingEnterSecs;

		
		public int PrepareSecs;

		
		public int FightingSecs;

		
		public int ClearRolesSecs;

		
		public Dictionary<int, ShengBeiData> ShengBeiDataDict = new Dictionary<int, ShengBeiData>();

		
		public int MapGridWidth = 100;

		
		public int MapGridHeight = 100;

		
		public Dictionary<int, HuanYingSiYuanShengBeiContextData> ShengBeiContextDict = new Dictionary<int, HuanYingSiYuanShengBeiContextData>();

		
		public long TempleMirageEXPAward;

		
		public int TempleMirageAwardChengJiu;

		
		public int TempleMirageAwardShengWang;

		
		public int TempleMirageWin = 1000;

		
		public int TempleMiragePK = 8;

		
		public int TempleMirageWinExtraNum = 3;

		
		public int TempleMirageWinExtraRate = 10;

		
		public int FuBenGoodsBinding = 1;

		
		public string AwardGoods;

		
		public Dictionary<int, int> GameId2FuBenSeq = new Dictionary<int, int>();
	}
}
