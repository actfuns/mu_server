using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200030D RID: 781
	[ProtoContract]
	public class LingDiCaiJiMainData
	{
		// Token: 0x04001411 RID: 5137
		[ProtoMember(1)]
		public List<LingDiCaiJiData> LingDiCaiJiDataList;

		// Token: 0x04001412 RID: 5138
		[ProtoMember(2)]
		public int LingDiCaiJiLeftCount;
	}
}
