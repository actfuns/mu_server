using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000569 RID: 1385
	[ProtoContract]
	public class KaiFuOnlineInfoData
	{
		// Token: 0x04002555 RID: 9557
		[ProtoMember(1)]
		public int SelfDayBit = 0;

		// Token: 0x04002556 RID: 9558
		[ProtoMember(2)]
		public List<int> SelfDayOnlineSecsList;

		// Token: 0x04002557 RID: 9559
		[ProtoMember(3)]
		public List<KaiFuOnlineAwardData> KaiFuOnlineAwardDataList;
	}
}
