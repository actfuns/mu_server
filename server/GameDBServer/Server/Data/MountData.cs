using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200019A RID: 410
	[ProtoContract]
	public class MountData
	{
		// Token: 0x04000950 RID: 2384
		[ProtoMember(1)]
		public int GoodsID;

		// Token: 0x04000951 RID: 2385
		[ProtoMember(2)]
		public bool IsNew;
	}
}
