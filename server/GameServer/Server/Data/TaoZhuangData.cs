using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002FD RID: 765
	[ProtoContract]
	public class TaoZhuangData
	{
		// Token: 0x040013C5 RID: 5061
		[ProtoMember(1)]
		public int ID;

		// Token: 0x040013C6 RID: 5062
		[ProtoMember(2)]
		public List<int> ActiviteList;
	}
}
