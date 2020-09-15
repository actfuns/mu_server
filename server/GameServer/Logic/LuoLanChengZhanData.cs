using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000258 RID: 600
	public class LuoLanChengZhanData
	{
		// Token: 0x04000E84 RID: 3716
		public object Mutex = new object();

		// Token: 0x04000E85 RID: 3717
		public int MapCode;

		// Token: 0x04000E86 RID: 3718
		public int MapCode_LongTa;

		// Token: 0x04000E87 RID: 3719
		public Dictionary<int, List<MapBirthPoint>> MapBirthPointListDict = new Dictionary<int, List<MapBirthPoint>>();

		// Token: 0x04000E88 RID: 3720
		public Dictionary<int, QiZhiConfig> NPCID2QiZhiConfigDict = new Dictionary<int, QiZhiConfig>();

		// Token: 0x04000E89 RID: 3721
		public long ApplyZhangMengZiJin = 0L;

		// Token: 0x04000E8A RID: 3722
		public int MaxZhanMengNum = 4;

		// Token: 0x04000E8B RID: 3723
		public long BidZhangMengZiJin = 0L;

		// Token: 0x04000E8C RID: 3724
		public int MinZhuanSheng = 0;

		// Token: 0x04000E8D RID: 3725
		public int MinLevel = 0;

		// Token: 0x04000E8E RID: 3726
		public int MinRequestNum = 1;

		// Token: 0x04000E8F RID: 3727
		public int MaxEnterNum = 1000;

		// Token: 0x04000E90 RID: 3728
		public int InstallJunQiNeedMoney = 0;

		// Token: 0x04000E91 RID: 3729
		public long EnrollTime = 1800L;

		// Token: 0x04000E92 RID: 3730
		public int GongNengOpenDaysFromKaiFu = 5;

		// Token: 0x04000E93 RID: 3731
		public TimeSpan NoRequestTimeStart;

		// Token: 0x04000E94 RID: 3732
		public TimeSpan NoRequestTimeEnd;

		// Token: 0x04000E95 RID: 3733
		public int[] WeekPoints = new int[0];

		// Token: 0x04000E96 RID: 3734
		public DateTime TimePoints;

		// Token: 0x04000E97 RID: 3735
		public DateTime WangChengZhanFightingDateTime;

		// Token: 0x04000E98 RID: 3736
		public int WaitingEnterSecs;

		// Token: 0x04000E99 RID: 3737
		public int PrepareSecs;

		// Token: 0x04000E9A RID: 3738
		public int FightingSecs;

		// Token: 0x04000E9B RID: 3739
		public int ClearRolesSecs;

		// Token: 0x04000E9C RID: 3740
		public bool CanRequestState = false;

		// Token: 0x04000E9D RID: 3741
		public Dictionary<int, SiegeWarfareEveryDayAwardsItem> SiegeWarfareEveryDayAwardsDict = new Dictionary<int, SiegeWarfareEveryDayAwardsItem>();

		// Token: 0x04000E9E RID: 3742
		public long ExpAward;

		// Token: 0x04000E9F RID: 3743
		public int ZhanGongAward;

		// Token: 0x04000EA0 RID: 3744
		public int ZiJin;

		// Token: 0x04000EA1 RID: 3745
		public string WarRequestStr = null;

		// Token: 0x04000EA2 RID: 3746
		public Dictionary<int, LuoLanChengZhanRequestInfo> WarRequstDict = new Dictionary<int, LuoLanChengZhanRequestInfo>();

		// Token: 0x04000EA3 RID: 3747
		public Dictionary<int, int> BHID2SiteDict = new Dictionary<int, int>();

		// Token: 0x04000EA4 RID: 3748
		public List<LuoLanChengZhanRoleCountData> LongTaBHRoleCountList = new List<LuoLanChengZhanRoleCountData>();

		// Token: 0x04000EA5 RID: 3749
		public LuoLanChengZhanLongTaOwnerData LongTaOwnerData = new LuoLanChengZhanLongTaOwnerData();

		// Token: 0x04000EA6 RID: 3750
		public List<LuoLanChengZhanQiZhiBuffOwnerData> QiZhiBuffOwnerDataList = new List<LuoLanChengZhanQiZhiBuffOwnerData>();

		// Token: 0x04000EA7 RID: 3751
		public int SuperQiZhiNpcId = 80000;

		// Token: 0x04000EA8 RID: 3752
		public int SuperQiZhiOwnerBirthPosX;

		// Token: 0x04000EA9 RID: 3753
		public int SuperQiZhiOwnerBirthPosY;

		// Token: 0x04000EAA RID: 3754
		public int SuperQiZhiOwnerBhid = 0;

		// Token: 0x04000EAB RID: 3755
		public long LastClearMapTicks = 0L;

		// Token: 0x04000EAC RID: 3756
		public DateTime FightEndTime;

		// Token: 0x04000EAD RID: 3757
		public Dictionary<int, double[]> QiZhiBuffDisableParamsDict = new Dictionary<int, double[]>();

		// Token: 0x04000EAE RID: 3758
		public Dictionary<int, double[]> QiZhiBuffEnableParamsDict = new Dictionary<int, double[]>();

		// Token: 0x04000EAF RID: 3759
		public int LuoLanChengZhuBHID;

		// Token: 0x04000EB0 RID: 3760
		public string LuoLanChengZhuBHName;

		// Token: 0x04000EB1 RID: 3761
		public long LuoLanChengZhuLastLoginTicks;

		// Token: 0x04000EB2 RID: 3762
		public GameClient LuoLanChengZhuClient;
	}
}
