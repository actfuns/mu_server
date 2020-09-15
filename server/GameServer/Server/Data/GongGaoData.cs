using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000144 RID: 324
	[ProtoContract]
	public class GongGaoData
	{
		// Token: 0x0400074A RID: 1866
		[ProtoMember(1)]
		public int nHaveGongGao = 0;

		// Token: 0x0400074B RID: 1867
		[ProtoMember(2)]
		public int nLianXuLoginReward = 0;

		// Token: 0x0400074C RID: 1868
		[ProtoMember(3)]
		public int nLeiJiLoginReward = 0;

		// Token: 0x0400074D RID: 1869
		[ProtoMember(4)]
		public string strGongGaoInfo = "";
	}
}
