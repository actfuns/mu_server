using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000BA RID: 186
	[ProtoContract]
	public class TaskData
	{
		// Token: 0x040004EB RID: 1259
		[ProtoMember(1)]
		public int DbID;

		// Token: 0x040004EC RID: 1260
		[ProtoMember(2)]
		public int DoingTaskID;

		// Token: 0x040004ED RID: 1261
		[ProtoMember(3)]
		public int DoingTaskVal1;

		// Token: 0x040004EE RID: 1262
		[ProtoMember(4)]
		public int DoingTaskVal2;

		// Token: 0x040004EF RID: 1263
		[ProtoMember(5)]
		public int DoingTaskFocus;

		// Token: 0x040004F0 RID: 1264
		[ProtoMember(6)]
		public long AddDateTime;

		// Token: 0x040004F1 RID: 1265
		[ProtoMember(7)]
		public TaskAwardsData TaskAwards = null;

		// Token: 0x040004F2 RID: 1266
		[ProtoMember(8)]
		public int DoneCount = 0;

		// Token: 0x040004F3 RID: 1267
		[ProtoMember(9)]
		public int StarLevel = 0;
	}
}
