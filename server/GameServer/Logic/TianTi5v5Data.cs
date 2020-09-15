using System;
using System.Collections.Generic;
using KF.Contract.Data;

namespace GameServer.Logic
{
	// Token: 0x020000C1 RID: 193
	public class TianTi5v5Data
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600035B RID: 859 RVA: 0x0003B6C0 File Offset: 0x000398C0
		public int TotalSecs
		{
			get
			{
				return this.WaitingEnterSecs + this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600035C RID: 860 RVA: 0x0003B6F0 File Offset: 0x000398F0
		// (set) Token: 0x0600035D RID: 861 RVA: 0x0003B70D File Offset: 0x0003990D
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

		// Token: 0x0400048C RID: 1164
		public object Mutex = new object();

		// Token: 0x0400048D RID: 1165
		public Dictionary<RangeKey, int> Range2GroupIndexDict = new Dictionary<RangeKey, int>(RangeKey.Comparer);

		// Token: 0x0400048E RID: 1166
		public Dictionary<int, TianTiBirthPoint> MapBirthPointDict = new Dictionary<int, TianTiBirthPoint>();

		// Token: 0x0400048F RID: 1167
		public Dictionary<RangeKey, DuanWeiRankAward> DuanWeiRankAwardDict = new Dictionary<RangeKey, DuanWeiRankAward>(RangeKey.Comparer);

		// Token: 0x04000490 RID: 1168
		public Dictionary<RangeKey, RongYaoRankAward> RongYaoRankAwardDict = new Dictionary<RangeKey, RongYaoRankAward>(RangeKey.Comparer);

		// Token: 0x04000491 RID: 1169
		public Dictionary<int, int> GameId2FuBenSeq = new Dictionary<int, int>();

		// Token: 0x04000492 RID: 1170
		public Dictionary<int, int> MapCodeDict = new Dictionary<int, int>();

		// Token: 0x04000493 RID: 1171
		public List<int> MapCodeList = new List<int>();

		// Token: 0x04000494 RID: 1172
		public string TimePointsStr;

		// Token: 0x04000495 RID: 1173
		public List<TimeSpan> TimePoints = new List<TimeSpan>();

		// Token: 0x04000496 RID: 1174
		public int MaxZhanDuiNum = 1000;

		// Token: 0x04000497 RID: 1175
		public TimeSpan RefreshTime = new TimeSpan(3, 0, 0);

		// Token: 0x04000498 RID: 1176
		public int MinZhuanSheng = 1;

		// Token: 0x04000499 RID: 1177
		public int MinLevel = 1;

		// Token: 0x0400049A RID: 1178
		public int MinRequestNum = 1;

		// Token: 0x0400049B RID: 1179
		public int MaxSignUp = 65;

		// Token: 0x0400049C RID: 1180
		public int TianTi5v5CD = 60;

		// Token: 0x0400049D RID: 1181
		public int WaitingEnterSecs;

		// Token: 0x0400049E RID: 1182
		public int PrepareSecs;

		// Token: 0x0400049F RID: 1183
		public int FightingSecs;

		// Token: 0x040004A0 RID: 1184
		public int ClearRolesSecs;

		// Token: 0x040004A1 RID: 1185
		public Dictionary<int, TianTiDuanWei> TianTi5v5DuanWeiDict = new Dictionary<int, TianTiDuanWei>();

		// Token: 0x040004A2 RID: 1186
		public Dictionary<RangeKey, int> DuanWeiJiFenRangeDuanWeiIdDict = new Dictionary<RangeKey, int>(RangeKey.Comparer);

		// Token: 0x040004A3 RID: 1187
		public int[] TeamBattleMap;

		// Token: 0x040004A4 RID: 1188
		public int[] TeamBattleWatch;

		// Token: 0x040004A5 RID: 1189
		public int[] TeamBattleNameRange;

		// Token: 0x040004A6 RID: 1190
		public int[] TeamApply;

		// Token: 0x040004A7 RID: 1191
		public int ZhanDuiJinZuan = 300;

		// Token: 0x040004A8 RID: 1192
		public int MinConfirmFightTeamCnt = 1;

		// Token: 0x040004A9 RID: 1193
		public Tuple<int, int> ZhanDuiDengJiTp = new Tuple<int, int>(5, 1);

		// Token: 0x040004AA RID: 1194
		public Tuple<int, int> LvLimit = new Tuple<int, int>(5, 1);

		// Token: 0x040004AB RID: 1195
		public int MaxTeamCnt = 5;

		// Token: 0x040004AC RID: 1196
		public int MaxTianTi5v5JiFen = 600000;

		// Token: 0x040004AD RID: 1197
		public int TeamConfirmTime = 30;

		// Token: 0x040004AE RID: 1198
		public int TeamAwardLimit = 10;

		// Token: 0x040004AF RID: 1199
		public int TeamPipeiTime = 60;

		// Token: 0x040004B0 RID: 1200
		public int MaxPaiMingRank = 100;

		// Token: 0x040004B1 RID: 1201
		public int MaxDayPaiMingListCount = 10;

		// Token: 0x040004B2 RID: 1202
		public int MaxMonthPaiMingListCount = 10;

		// Token: 0x040004B3 RID: 1203
		public int MinDayPaiMingListCount = 3;

		// Token: 0x040004B4 RID: 1204
		public TianTi5v5RankData RankData = new TianTi5v5RankData();

		// Token: 0x040004B5 RID: 1205
		public Dictionary<int, TianTi5v5ZhanDuiData> ZhanDuiDataPaiHangDict = new Dictionary<int, TianTi5v5ZhanDuiData>();

		// Token: 0x040004B6 RID: 1206
		public List<TianTi5v5ZhanDuiData> ZhanDuiDataPaiHangList = new List<TianTi5v5ZhanDuiData>();

		// Token: 0x040004B7 RID: 1207
		public Dictionary<int, TianTi5v5ZhanDuiData> ZhanDuiDataMonthPaiHangDict = new Dictionary<int, TianTi5v5ZhanDuiData>();

		// Token: 0x040004B8 RID: 1208
		public List<TianTi5v5ZhanDuiData> ZhanDuiDataMonthPaiHangList = new List<TianTi5v5ZhanDuiData>();

		// Token: 0x040004B9 RID: 1209
		public AgeDataT<List<TianTi5v5ZhanDuiMiniData>> ZhanDuiSimpleList = new AgeDataT<List<TianTi5v5ZhanDuiMiniData>>(0L, new List<TianTi5v5ZhanDuiMiniData>());

		// Token: 0x040004BA RID: 1210
		public Dictionary<int, KuaFu5v5FuBenData> FuBenDataDict = new Dictionary<int, KuaFu5v5FuBenData>();

		// Token: 0x040004BB RID: 1211
		public Dictionary<int, TianTi5v5FuBenItem> TianTi5v5FuBenItemDict = new Dictionary<int, TianTi5v5FuBenItem>();

		// Token: 0x040004BC RID: 1212
		public Dictionary<int, AgeDataT<TianTi5v5ZhanDuiData>> ZhanDuiDataAgeDict = new Dictionary<int, AgeDataT<TianTi5v5ZhanDuiData>>();

		// Token: 0x040004BD RID: 1213
		public Dictionary<int, List<int>> ZhanDuiInviteListDict = new Dictionary<int, List<int>>();

		// Token: 0x040004BE RID: 1214
		public Dictionary<int, List<TianTi5v5ZhanDuiRoleData>> ZhanDuiRequestListDict = new Dictionary<int, List<TianTi5v5ZhanDuiRoleData>>();

		// Token: 0x040004BF RID: 1215
		public Dictionary<long, long> RoleRequestZhanDuiTicksDict = new Dictionary<long, long>();

		// Token: 0x040004C0 RID: 1216
		public Dictionary<int, TianTi5v5PiPeiState> ConfirmBattleDict = new Dictionary<int, TianTi5v5PiPeiState>();
	}
}
