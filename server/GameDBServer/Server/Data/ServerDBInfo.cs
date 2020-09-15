using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000047 RID: 71
	[ProtoContract]
	public class ServerDBInfo
	{
		// Token: 0x04000169 RID: 361
		[ProtoMember(1)]
		public string strIP;

		// Token: 0x0400016A RID: 362
		[ProtoMember(2)]
		public int Port;

		// Token: 0x0400016B RID: 363
		[ProtoMember(3)]
		public string dbName;

		// Token: 0x0400016C RID: 364
		[ProtoMember(4)]
		public string userName;

		// Token: 0x0400016D RID: 365
		[ProtoMember(5)]
		public string uPassword;

		// Token: 0x0400016E RID: 366
		[ProtoMember(6)]
		public int maxConns;

		// Token: 0x0400016F RID: 367
		[ProtoMember(7)]
		public string InternalIP;

		// Token: 0x04000170 RID: 368
		[ProtoMember(8)]
		public string ChargeKey;

		// Token: 0x04000171 RID: 369
		[ProtoMember(9)]
		public string ServerKey;

		// Token: 0x04000172 RID: 370
		[ProtoMember(64)]
		public string DbNames;

		// Token: 0x04000173 RID: 371
		[ProtoMember(65)]
		public int CodePage;
	}
}
