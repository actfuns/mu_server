using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000414 RID: 1044
	[ProtoContract]
	public class ShenShiMainData
	{
		// Token: 0x04001BDD RID: 7133
		[ProtoMember(1)]
		public int FuWenTabId;

		// Token: 0x04001BDE RID: 7134
		[ProtoMember(2)]
		public DateTime NextFreeTime;
	}
}
