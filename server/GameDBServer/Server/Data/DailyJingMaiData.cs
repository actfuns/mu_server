using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000043 RID: 67
	[ProtoContract]
	public class DailyJingMaiData
	{
		// Token: 0x0400015A RID: 346
		[ProtoMember(1)]
		public string JmTime = "";

		// Token: 0x0400015B RID: 347
		[ProtoMember(2)]
		public int JmNum = 0;
	}
}
