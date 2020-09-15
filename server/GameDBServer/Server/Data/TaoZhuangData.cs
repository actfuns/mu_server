using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000146 RID: 326
	[ProtoContract]
	public class TaoZhuangData
	{
		// Token: 0x04000825 RID: 2085
		[ProtoMember(1)]
		public int ID;

		// Token: 0x04000826 RID: 2086
		[ProtoMember(2)]
		public List<int> ActiviteList;
	}
}
