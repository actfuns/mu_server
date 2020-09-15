using System;
using System.Collections.Generic;
using ProtoBuf;

namespace KF.Contract.Data
{
	// Token: 0x020000C0 RID: 192
	[ProtoContract]
	[Serializable]
	public class TianTi5v5ZhanDuiData
	{
		// Token: 0x060001A8 RID: 424 RVA: 0x00008F88 File Offset: 0x00007188
		public TianTi5v5ZhanDuiData Clone()
		{
			TianTi5v5ZhanDuiData data = base.MemberwiseClone() as TianTi5v5ZhanDuiData;
			if (null != this.teamerList)
			{
				data.teamerList = new List<TianTi5v5ZhanDuiRoleData>();
				foreach (TianTi5v5ZhanDuiRoleData role in this.teamerList)
				{
					data.teamerList.Add(role.Clone());
				}
			}
			return data;
		}

		// Token: 0x04000518 RID: 1304
		[ProtoMember(1)]
		public int ZhanDuiID;

		// Token: 0x04000519 RID: 1305
		[ProtoMember(2)]
		public string XuanYan;

		// Token: 0x0400051A RID: 1306
		[ProtoMember(3)]
		public string ZhanDuiName;

		// Token: 0x0400051B RID: 1307
		[ProtoMember(4)]
		public int LeaderRoleID;

		// Token: 0x0400051C RID: 1308
		[ProtoMember(5)]
		public int DuanWeiId;

		// Token: 0x0400051D RID: 1309
		[ProtoMember(6)]
		public int DuanWeiJiFen;

		// Token: 0x0400051E RID: 1310
		[ProtoMember(7)]
		public int DuanWeiRank;

		// Token: 0x0400051F RID: 1311
		[ProtoMember(8)]
		public long ZhanDouLi;

		// Token: 0x04000520 RID: 1312
		[ProtoMember(9)]
		public int LianSheng;

		// Token: 0x04000521 RID: 1313
		[ProtoMember(10)]
		public int SuccessCount;

		// Token: 0x04000522 RID: 1314
		[ProtoMember(11)]
		public int FightCount;

		// Token: 0x04000523 RID: 1315
		[ProtoMember(12)]
		public int MonthDuanWeiRank;

		// Token: 0x04000524 RID: 1316
		[ProtoMember(13)]
		public List<TianTi5v5ZhanDuiRoleData> teamerList = new List<TianTi5v5ZhanDuiRoleData>();

		// Token: 0x04000525 RID: 1317
		[ProtoMember(14)]
		public string TeamerRidList;

		// Token: 0x04000526 RID: 1318
		[ProtoMember(15)]
		public DateTime LastFightTime;

		// Token: 0x04000527 RID: 1319
		[ProtoMember(16)]
		public string LeaderRoleName;

		// Token: 0x04000528 RID: 1320
		[ProtoMember(17)]
		public int ZoneID;

		// Token: 0x04000529 RID: 1321
		[ProtoMember(18)]
		public int ZorkWin;

		// Token: 0x0400052A RID: 1322
		[ProtoMember(19)]
		public int ZorkWinStreak;

		// Token: 0x0400052B RID: 1323
		[ProtoMember(20)]
		public int ZorkBossInjure;

		// Token: 0x0400052C RID: 1324
		[ProtoMember(21)]
		public int ZorkJiFen;

		// Token: 0x0400052D RID: 1325
		[ProtoMember(22)]
		public DateTime ZorkLastFightTime;

		// Token: 0x0400052E RID: 1326
		[ProtoMember(23)]
		public int EscapeJiFen;

		// Token: 0x0400052F RID: 1327
		[ProtoMember(24)]
		public DateTime EscapeLastFightTime;

		// Token: 0x04000530 RID: 1328
		[ProtoMember(25)]
		public int ZhanDuiDataModeType;
	}
}
