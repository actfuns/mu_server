using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200002F RID: 47
	[ProtoContract]
	public class BangHuiJunQiItemData
	{
		// Token: 0x040000E8 RID: 232
		[ProtoMember(1)]
		public int BHID = 0;

		// Token: 0x040000E9 RID: 233
		[ProtoMember(2)]
		public string QiName = "";

		// Token: 0x040000EA RID: 234
		[ProtoMember(3)]
		public int QiLevel = 0;
	}
}
