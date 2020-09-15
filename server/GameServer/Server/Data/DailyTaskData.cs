using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200054A RID: 1354
	[ProtoContract]
	public class DailyTaskData
	{
		// Token: 0x04002458 RID: 9304
		[ProtoMember(1)]
		public int HuanID = 0;

		// Token: 0x04002459 RID: 9305
		[ProtoMember(2)]
		public string RecTime = "";

		// Token: 0x0400245A RID: 9306
		[ProtoMember(3)]
		public int RecNum = 0;

		// Token: 0x0400245B RID: 9307
		[ProtoMember(4)]
		public int TaskClass = 0;

		// Token: 0x0400245C RID: 9308
		[ProtoMember(5)]
		public int ExtDayID = 0;

		// Token: 0x0400245D RID: 9309
		[ProtoMember(6)]
		public int ExtNum = 0;
	}
}
