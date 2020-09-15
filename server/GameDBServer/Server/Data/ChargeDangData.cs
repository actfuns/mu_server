using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000041 RID: 65
	[ProtoContract]
	public class ChargeDangData
	{
		// Token: 0x04000152 RID: 338
		[ProtoMember(1)]
		public List<SingleChargeData> chargeData = new List<SingleChargeData>();
	}
}
