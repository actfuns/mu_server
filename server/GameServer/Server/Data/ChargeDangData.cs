using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200012B RID: 299
	[ProtoContract]
	public class ChargeDangData
	{
		// Token: 0x04000677 RID: 1655
		[ProtoMember(1)]
		public List<SingleChargeData> chargeData = new List<SingleChargeData>();
	}
}
