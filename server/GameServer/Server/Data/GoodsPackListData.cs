using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200055A RID: 1370
	[ProtoContract]
	public class GoodsPackListData
	{
		// Token: 0x040024DB RID: 9435
		[ProtoMember(1)]
		public int AutoID = -1;

		// Token: 0x040024DC RID: 9436
		[ProtoMember(2)]
		public List<GoodsData> GoodsDataList = null;

		// Token: 0x040024DD RID: 9437
		[ProtoMember(3)]
		public int OpenState = -1;

		// Token: 0x040024DE RID: 9438
		[ProtoMember(4)]
		public int RetError = 0;

		// Token: 0x040024DF RID: 9439
		[ProtoMember(5)]
		public long LeftTicks = 0L;

		// Token: 0x040024E0 RID: 9440
		[ProtoMember(6)]
		public long PackTicks = -1L;
	}
}
