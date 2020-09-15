using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000013 RID: 19
	[ProtoContract]
	public class MazingerStore
	{
		// Token: 0x04000077 RID: 119
		[ProtoMember(1)]
		public int result = 0;

		// Token: 0x04000078 RID: 120
		[ProtoMember(2)]
		public MazingerStoreData data;

		// Token: 0x04000079 RID: 121
		[ProtoMember(3)]
		public int IsBoom = 0;
	}
}
