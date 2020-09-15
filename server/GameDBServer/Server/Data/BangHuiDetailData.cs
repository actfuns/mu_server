using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200002D RID: 45
	[ProtoContract]
	public class BangHuiDetailData
	{
		// Token: 0x040000C6 RID: 198
		[ProtoMember(1)]
		public int BHID = 0;

		// Token: 0x040000C7 RID: 199
		[ProtoMember(2)]
		public string BHName = "";

		// Token: 0x040000C8 RID: 200
		[ProtoMember(3)]
		public int ZoneID = 0;

		// Token: 0x040000C9 RID: 201
		[ProtoMember(4)]
		public int BZRoleID = 0;

		// Token: 0x040000CA RID: 202
		[ProtoMember(5)]
		public string BZRoleName = "";

		// Token: 0x040000CB RID: 203
		[ProtoMember(6)]
		public int BZOccupation = 0;

		// Token: 0x040000CC RID: 204
		[ProtoMember(7)]
		public int TotalNum = 0;

		// Token: 0x040000CD RID: 205
		[ProtoMember(8)]
		public int TotalLevel = 0;

		// Token: 0x040000CE RID: 206
		[ProtoMember(9)]
		public string BHBulletin = "";

		// Token: 0x040000CF RID: 207
		[ProtoMember(10)]
		public string BuildTime = "";

		// Token: 0x040000D0 RID: 208
		[ProtoMember(11)]
		public string QiName = "";

		// Token: 0x040000D1 RID: 209
		[ProtoMember(12)]
		public int QiLevel = 0;

		// Token: 0x040000D2 RID: 210
		[ProtoMember(13)]
		public List<BangHuiMgrItemData> MgrItemList = null;

		// Token: 0x040000D3 RID: 211
		[ProtoMember(14)]
		public int IsVerify = 0;

		// Token: 0x040000D4 RID: 212
		[ProtoMember(15)]
		public int TotalMoney = 0;

		// Token: 0x040000D5 RID: 213
		[ProtoMember(16)]
		public int TodayZhanGongForGold = 0;

		// Token: 0x040000D6 RID: 214
		[ProtoMember(17)]
		public int TodayZhanGongForDiamond = 0;

		// Token: 0x040000D7 RID: 215
		[ProtoMember(18)]
		public int JiTan = 0;

		// Token: 0x040000D8 RID: 216
		[ProtoMember(19)]
		public int JunXie = 0;

		// Token: 0x040000D9 RID: 217
		[ProtoMember(20)]
		public int GuangHuan = 0;

		// Token: 0x040000DA RID: 218
		[ProtoMember(21)]
		public int CanModNameTimes = 0;

		// Token: 0x040000DB RID: 219
		[ProtoMember(22)]
		public long TotalCombatForce;

		// Token: 0x040000DC RID: 220
		[ProtoMember(23)]
		public long ZhengDuoUsedTime;
	}
}
