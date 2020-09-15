using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000189 RID: 393
	[ProtoContract]
	public class SpecialActivityData
	{
		// Token: 0x040008B6 RID: 2230
		[ProtoMember(1)]
		public int GroupID = 0;

		// Token: 0x040008B7 RID: 2231
		[ProtoMember(2)]
		public List<SpecActInfo> SpecActInfoList;
	}
}
