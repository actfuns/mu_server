using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000036 RID: 54
	[ProtoContract]
	public class BangQiInfoData
	{
		// Token: 0x0400011D RID: 285
		[ProtoMember(1)]
		public string BangQiName = "";

		// Token: 0x0400011E RID: 286
		[ProtoMember(2)]
		public int BangQiLevel = 0;

		// Token: 0x0400011F RID: 287
		[ProtoMember(3)]
		public Dictionary<int, BHLingDiOwnData> BHLingDiOwnDict = null;
	}
}
