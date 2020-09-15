using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200056C RID: 1388
	[ProtoContract]
	public class ModifyWanMotaData
	{
		// Token: 0x0400255E RID: 9566
		[ProtoMember(1)]
		public string strParams;

		// Token: 0x0400255F RID: 9567
		[ProtoMember(2)]
		public string strSweepReward;
	}
}
