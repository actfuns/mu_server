using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000281 RID: 641
	[ProtoContract]
	public class CompSelectData
	{
		// Token: 0x04000FF7 RID: 4087
		[ProtoMember(1)]
		public List<int> RecommendCompList = new List<int>();

		// Token: 0x04000FF8 RID: 4088
		[ProtoMember(2)]
		public List<string> DaLingZhuNameList = new List<string>();
	}
}
