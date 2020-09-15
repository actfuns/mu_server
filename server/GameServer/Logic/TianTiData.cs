using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x0200048A RID: 1162
	public class TianTiData
	{
		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600150B RID: 5387 RVA: 0x00149418 File Offset: 0x00147618
		public int TotalSecs
		{
			get
			{
				return this.WaitingEnterSecs + this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs;
			}
		}

		// Token: 0x04001E99 RID: 7833
		public object Mutex = new object();

		// Token: 0x04001E9A RID: 7834
		public Dictionary<RangeKey, int> Range2GroupIndexDict = new Dictionary<RangeKey, int>(RangeKey.Comparer);

		// Token: 0x04001E9B RID: 7835
		public Dictionary<int, TianTiBirthPoint> MapBirthPointDict = new Dictionary<int, TianTiBirthPoint>();

		// Token: 0x04001E9C RID: 7836
		public Dictionary<RangeKey, DuanWeiRankAward> DuanWeiRankAwardDict = new Dictionary<RangeKey, DuanWeiRankAward>(RangeKey.Comparer);

		// Token: 0x04001E9D RID: 7837
		public Dictionary<RangeKey, RongYaoRankAward> RongYaoRankAwardDict = new Dictionary<RangeKey, RongYaoRankAward>(RangeKey.Comparer);

		// Token: 0x04001E9E RID: 7838
		public Dictionary<int, int> GameId2FuBenSeq = new Dictionary<int, int>();

		// Token: 0x04001E9F RID: 7839
		public Dictionary<int, int> MapCodeDict = new Dictionary<int, int>();

		// Token: 0x04001EA0 RID: 7840
		public List<int> MapCodeList = new List<int>();

		// Token: 0x04001EA1 RID: 7841
		public string TimePointsStr;

		// Token: 0x04001EA2 RID: 7842
		public List<TimeSpan> TimePoints = new List<TimeSpan>();

		// Token: 0x04001EA3 RID: 7843
		public int MaxZhanMengNum = 4;

		// Token: 0x04001EA4 RID: 7844
		public TimeSpan RefreshTime = new TimeSpan(3, 0, 0);

		// Token: 0x04001EA5 RID: 7845
		public int MinZhuanSheng = 1;

		// Token: 0x04001EA6 RID: 7846
		public int MinLevel = 1;

		// Token: 0x04001EA7 RID: 7847
		public int MinRequestNum = 1;

		// Token: 0x04001EA8 RID: 7848
		public int MaxEnterNum = 10;

		// Token: 0x04001EA9 RID: 7849
		public int FuBenId = 13000;

		// Token: 0x04001EAA RID: 7850
		public int HoldShengBeiSecs = 60;

		// Token: 0x04001EAB RID: 7851
		public int MinSubmitShengBeiSecs = 13;

		// Token: 0x04001EAC RID: 7852
		public int TianTiCD = 60;

		// Token: 0x04001EAD RID: 7853
		public int WaitingEnterSecs;

		// Token: 0x04001EAE RID: 7854
		public int PrepareSecs;

		// Token: 0x04001EAF RID: 7855
		public int FightingSecs;

		// Token: 0x04001EB0 RID: 7856
		public int ClearRolesSecs;

		// Token: 0x04001EB1 RID: 7857
		public Dictionary<int, TianTiDuanWei> TianTiDuanWeiDict = new Dictionary<int, TianTiDuanWei>();

		// Token: 0x04001EB2 RID: 7858
		public Dictionary<RangeKey, int> DuanWeiJiFenRangeDuanWeiIdDict = new Dictionary<RangeKey, int>(RangeKey.Comparer);

		// Token: 0x04001EB3 RID: 7859
		public int TempleMirageAwardChengJiu;

		// Token: 0x04001EB4 RID: 7860
		public int TempleMirageAwardShengWang;

		// Token: 0x04001EB5 RID: 7861
		public int WinDuanWeiJiFen = 1000;

		// Token: 0x04001EB6 RID: 7862
		public int LoseDuanWeiJiFen = 8;

		// Token: 0x04001EB7 RID: 7863
		public int DuanWeiJiFenNum = 5;

		// Token: 0x04001EB8 RID: 7864
		public int TempleMirageWinExtraRate = 10;

		// Token: 0x04001EB9 RID: 7865
		public int FuBenGoodsBinding = 1;

		// Token: 0x04001EBA RID: 7866
		public int MaxTianTiJiFen = 600000;

		// Token: 0x04001EBB RID: 7867
		public DateTime ModifyTime;

		// Token: 0x04001EBC RID: 7868
		public int MaxPaiMingRank = 100;

		// Token: 0x04001EBD RID: 7869
		public int MaxDayPaiMingListCount = 10;

		// Token: 0x04001EBE RID: 7870
		public int MaxMonthPaiMingListCount = 10;

		// Token: 0x04001EBF RID: 7871
		public Dictionary<int, TianTiPaiHangRoleData> TianTiPaiHangRoleDataDict = new Dictionary<int, TianTiPaiHangRoleData>();

		// Token: 0x04001EC0 RID: 7872
		public List<TianTiPaiHangRoleData> TianTiPaiHangRoleDataList = new List<TianTiPaiHangRoleData>();

		// Token: 0x04001EC1 RID: 7873
		public Dictionary<int, TianTiPaiHangRoleData> TianTiMonthPaiHangRoleDataDict = new Dictionary<int, TianTiPaiHangRoleData>();

		// Token: 0x04001EC2 RID: 7874
		public List<TianTiPaiHangRoleData> TianTiMonthPaiHangRoleDataList = new List<TianTiPaiHangRoleData>();

		// Token: 0x04001EC3 RID: 7875
		public ConcurrentDictionary<int, long> TianTiCDDict = new ConcurrentDictionary<int, long>();
	}
}
