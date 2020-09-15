using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000044 RID: 68
	[ProtoContract]
	public class DailyTaskData
	{
		// Token: 0x0400015C RID: 348
		[ProtoMember(1)]
		public int HuanID = 0;

		// Token: 0x0400015D RID: 349
		[ProtoMember(2)]
		public string RecTime = "";

		// Token: 0x0400015E RID: 350
		[ProtoMember(3)]
		public int RecNum = 0;

		// Token: 0x0400015F RID: 351
		[ProtoMember(4)]
		public int TaskClass = 0;

		// Token: 0x04000160 RID: 352
		[ProtoMember(5)]
		public int ExtDayID = 0;

		// Token: 0x04000161 RID: 353
		[ProtoMember(6)]
		public int ExtNum = 0;
	}
}
