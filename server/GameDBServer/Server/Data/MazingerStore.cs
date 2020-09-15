using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000066 RID: 102
	[ProtoContract]
	public class MazingerStore
	{
		// Token: 0x04000231 RID: 561
		[ProtoMember(1)]
		public int result = 0;

		// Token: 0x04000232 RID: 562
		[ProtoMember(2)]
		private MazingerStoreData data;
	}
}
