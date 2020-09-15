using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000032 RID: 50
	[ProtoContract]
	public class BangHuiListData
	{
		// Token: 0x040000FF RID: 255
		[ProtoMember(1)]
		public int TotalBangHuiItemNum = 0;

		// Token: 0x04000100 RID: 256
		[ProtoMember(2)]
		public List<BangHuiItemData> BangHuiItemDataList = null;
	}
}
