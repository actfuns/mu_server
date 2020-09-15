using System;
using ProtoBuf;

namespace GameDBServer.Logic.UserReturn
{
	// Token: 0x02000188 RID: 392
	[ProtoContract]
	public class ReturnData
	{
		// Token: 0x04000907 RID: 2311
		[ProtoMember(1)]
		public int DBID = 0;

		// Token: 0x04000908 RID: 2312
		[ProtoMember(2)]
		public int ActivityID = 0;

		// Token: 0x04000909 RID: 2313
		[ProtoMember(3)]
		public string ActivityDay = "";

		// Token: 0x0400090A RID: 2314
		[ProtoMember(4)]
		public int PZoneID = 0;

		// Token: 0x0400090B RID: 2315
		[ProtoMember(5)]
		public int PRoleID = 0;

		// Token: 0x0400090C RID: 2316
		[ProtoMember(6)]
		public int CZoneID = 0;

		// Token: 0x0400090D RID: 2317
		[ProtoMember(7)]
		public int CRoleID = 0;

		// Token: 0x0400090E RID: 2318
		[ProtoMember(8)]
		public int Vip = 0;

		// Token: 0x0400090F RID: 2319
		[ProtoMember(9)]
		public int Level = 0;

		// Token: 0x04000910 RID: 2320
		[ProtoMember(10)]
		public DateTime LogTime = DateTime.MinValue;

		// Token: 0x04000911 RID: 2321
		[ProtoMember(11)]
		public int StateCheck = 0;

		// Token: 0x04000912 RID: 2322
		[ProtoMember(12)]
		public int StateLog = 0;

		// Token: 0x04000913 RID: 2323
		[ProtoMember(13)]
		public int LeiJiChongZhi = 0;
	}
}
