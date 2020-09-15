using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000156 RID: 342
	[ProtoContract]
	internal class GroupMailData
	{
		// Token: 0x04000789 RID: 1929
		[ProtoMember(1)]
		public int GMailID = 0;

		// Token: 0x0400078A RID: 1930
		[ProtoMember(2)]
		public string Subject = "";

		// Token: 0x0400078B RID: 1931
		[ProtoMember(3)]
		public string Content = "";

		// Token: 0x0400078C RID: 1932
		[ProtoMember(4)]
		public string Conditions = "";

		// Token: 0x0400078D RID: 1933
		[ProtoMember(5)]
		public long InputTime = 0L;

		// Token: 0x0400078E RID: 1934
		[ProtoMember(6)]
		public long EndTime = 0L;

		// Token: 0x0400078F RID: 1935
		[ProtoMember(7)]
		public int Yinliang = 0;

		// Token: 0x04000790 RID: 1936
		[ProtoMember(8)]
		public int Tongqian = 0;

		// Token: 0x04000791 RID: 1937
		[ProtoMember(9)]
		public int YuanBao = 0;

		// Token: 0x04000792 RID: 1938
		[ProtoMember(10)]
		public List<GoodsData> GoodsList = null;
	}
}
