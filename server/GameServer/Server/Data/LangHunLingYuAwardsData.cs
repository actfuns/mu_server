using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003BF RID: 959
	[ProtoContract]
	public class LangHunLingYuAwardsData
	{
		// Token: 0x04001918 RID: 6424
		[ProtoMember(1)]
		public int Success;

		// Token: 0x04001919 RID: 6425
		[ProtoMember(2)]
		public List<AwardsItemData> AwardsItemDataList;

		// Token: 0x0400191A RID: 6426
		[ProtoMember(3)]
		public string successBhName;
	}
}
