using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005BC RID: 1468
	[ProtoContract]
	public class BangHuiJunQiItemData
	{
		// Token: 0x0400295D RID: 10589
		[ProtoMember(1)]
		public int BHID = 0;

		// Token: 0x0400295E RID: 10590
		[ProtoMember(2)]
		public string QiName = "";

		// Token: 0x0400295F RID: 10591
		[ProtoMember(3)]
		public int QiLevel = 0;
	}
}
