using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000549 RID: 1353
	[ProtoContract]
	public class DailyJingMaiData
	{
		// Token: 0x04002456 RID: 9302
		[ProtoMember(1)]
		public string JmTime = "";

		// Token: 0x04002457 RID: 9303
		[ProtoMember(2)]
		public int JmNum = 0;
	}
}
