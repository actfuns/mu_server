using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x020002BC RID: 700
	[ProtoContract]
	public class FundData
	{
		// Token: 0x0400120A RID: 4618
		[ProtoMember(1, IsRequired = true)]
		public bool IsOpen = false;

		// Token: 0x0400120B RID: 4619
		[ProtoMember(2, IsRequired = true)]
		public int State = 0;

		// Token: 0x0400120C RID: 4620
		[ProtoMember(3, IsRequired = true)]
		public int FundType = 0;

		// Token: 0x0400120D RID: 4621
		[ProtoMember(4, IsRequired = true)]
		public Dictionary<int, FundItem> FundDic = new Dictionary<int, FundItem>();
	}
}
