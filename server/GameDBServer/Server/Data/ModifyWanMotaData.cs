using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200007C RID: 124
	[ProtoContract]
	public class ModifyWanMotaData
	{
		// Token: 0x040002A0 RID: 672
		[ProtoMember(1)]
		public string strParams;

		// Token: 0x040002A1 RID: 673
		[ProtoMember(2)]
		public string strSweepReward;
	}
}
