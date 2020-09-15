using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000032 RID: 50
	[ProtoContract]
	public class EverydayActInfoDB
	{
		// Token: 0x0400010A RID: 266
		[ProtoMember(1)]
		public int GroupID = 0;

		// Token: 0x0400010B RID: 267
		[ProtoMember(2)]
		public int ActID = 0;

		// Token: 0x0400010C RID: 268
		[ProtoMember(3)]
		public int PurNum = 0;

		// Token: 0x0400010D RID: 269
		[ProtoMember(4)]
		public int CountNum = 0;

		// Token: 0x0400010E RID: 270
		[ProtoMember(5)]
		public int ActiveDay = 0;
	}
}
