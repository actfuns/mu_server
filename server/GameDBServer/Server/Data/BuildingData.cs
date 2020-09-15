using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000039 RID: 57
	[ProtoContract]
	public class BuildingData
	{
		// Token: 0x04000127 RID: 295
		[ProtoMember(1)]
		public int BuildId = 0;

		// Token: 0x04000128 RID: 296
		[ProtoMember(2)]
		public int BuildLev = 0;

		// Token: 0x04000129 RID: 297
		[ProtoMember(3)]
		public int BuildExp = 0;

		// Token: 0x0400012A RID: 298
		[ProtoMember(4)]
		public string BuildTime = null;

		// Token: 0x0400012B RID: 299
		[ProtoMember(5)]
		public int TaskID_1 = 0;

		// Token: 0x0400012C RID: 300
		[ProtoMember(6)]
		public int TaskID_2 = 0;

		// Token: 0x0400012D RID: 301
		[ProtoMember(7)]
		public int TaskID_3 = 0;

		// Token: 0x0400012E RID: 302
		[ProtoMember(8)]
		public int TaskID_4 = 0;
	}
}
