using System;
using ProtoBuf;

namespace KF.Contract.Data
{
	// Token: 0x020000BB RID: 187
	[ProtoContract]
	[Serializable]
	public class TianTi5v5ZhanDuiRoleData
	{
		// Token: 0x060001A1 RID: 417 RVA: 0x00008EE0 File Offset: 0x000070E0
		public TianTi5v5ZhanDuiRoleData Clone()
		{
			return base.MemberwiseClone() as TianTi5v5ZhanDuiRoleData;
		}

		// Token: 0x040004F4 RID: 1268
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040004F5 RID: 1269
		[ProtoMember(2)]
		public int RoleOcc;

		// Token: 0x040004F6 RID: 1270
		[ProtoMember(5)]
		public string RoleName = "";

		// Token: 0x040004F7 RID: 1271
		[ProtoMember(6)]
		public long ZhanLi = 0L;

		// Token: 0x040004F8 RID: 1272
		[ProtoMember(7)]
		public int ZhuanSheng = 0;

		// Token: 0x040004F9 RID: 1273
		[ProtoMember(8)]
		public int Level = 0;

		// Token: 0x040004FA RID: 1274
		[ProtoMember(9)]
		public byte[] ModelData;

		// Token: 0x040004FB RID: 1275
		[ProtoMember(10)]
		public int OnlineState;

		// Token: 0x040004FC RID: 1276
		[ProtoMember(11)]
		public byte[] PlayerJingJiMirrorData;

		// Token: 0x040004FD RID: 1277
		[ProtoMember(12)]
		public int ZoneID;

		// Token: 0x040004FE RID: 1278
		[ProtoMember(13)]
		public int TodayFightCount;

		// Token: 0x040004FF RID: 1279
		[ProtoMember(14)]
		public int LastFightDayId;

		// Token: 0x04000500 RID: 1280
		[ProtoMember(15)]
		public int MonthFigntCount;

		// Token: 0x04000501 RID: 1281
		[ProtoMember(16)]
		public int MonthAwardsFlags;

		// Token: 0x04000502 RID: 1282
		[ProtoMember(17)]
		public DateTime FetchMonthDuanWeiRankAwardsTime;

		// Token: 0x04000503 RID: 1283
		[ProtoMember(18)]
		public int[] MonthFightCounts;

		// Token: 0x04000504 RID: 1284
		[ProtoMember(19)]
		public int RebornLevel = 0;
	}
}
