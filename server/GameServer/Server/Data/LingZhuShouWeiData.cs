using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200030F RID: 783
	[ProtoContract]
	public class LingZhuShouWeiData
	{
		// Token: 0x04001418 RID: 5144
		[ProtoMember(1)]
		public int Result;

		// Token: 0x04001419 RID: 5145
		[ProtoMember(2)]
		public List<LingDiShouWeiData> ShouWeiList;
	}
}
