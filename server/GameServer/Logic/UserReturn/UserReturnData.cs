using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.UserReturn
{
	// Token: 0x020004BA RID: 1210
	[ProtoContract]
	public class UserReturnData
	{
		// Token: 0x04002017 RID: 8215
		[ProtoMember(1)]
		public bool ActivityIsOpen = false;

		// Token: 0x04002018 RID: 8216
		[ProtoMember(2)]
		public int ActivityID = 0;

		// Token: 0x04002019 RID: 8217
		[ProtoMember(3)]
		public string ActivityDay = "";

		// Token: 0x0400201A RID: 8218
		[ProtoMember(4)]
		public DateTime TimeBegin = DateTime.MinValue;

		// Token: 0x0400201B RID: 8219
		[ProtoMember(5)]
		public DateTime TimeEnd = DateTime.MinValue;

		// Token: 0x0400201C RID: 8220
		[ProtoMember(6)]
		public DateTime TimeAward = DateTime.MinValue;

		// Token: 0x0400201D RID: 8221
		[ProtoMember(7)]
		public string RecallCode = "0";

		// Token: 0x0400201E RID: 8222
		[ProtoMember(8)]
		public int RecallZoneID = 0;

		// Token: 0x0400201F RID: 8223
		[ProtoMember(9)]
		public int RecallRoleID = 0;

		// Token: 0x04002020 RID: 8224
		[ProtoMember(10)]
		public int Level = 0;

		// Token: 0x04002021 RID: 8225
		[ProtoMember(11)]
		public int Vip = 0;

		// Token: 0x04002022 RID: 8226
		[ProtoMember(12)]
		public DateTime TimeReturn = DateTime.MinValue;

		// Token: 0x04002023 RID: 8227
		[ProtoMember(13)]
		public int ReturnState = 0;

		// Token: 0x04002024 RID: 8228
		[ProtoMember(14)]
		public Dictionary<int, int[]> AwardDic = new Dictionary<int, int[]>();

		// Token: 0x04002025 RID: 8229
		[ProtoMember(15)]
		public DateTime TimeWait = DateTime.MinValue;

		// Token: 0x04002026 RID: 8230
		[ProtoMember(16)]
		public int ZhuanSheng = 0;

		// Token: 0x04002027 RID: 8231
		[ProtoMember(17)]
		public int DengJi = 0;

		// Token: 0x04002028 RID: 8232
		[ProtoMember(18)]
		public string MyCode = "";

		// Token: 0x04002029 RID: 8233
		[ProtoMember(19)]
		public int LeiJiChongZhi = 0;
	}
}
