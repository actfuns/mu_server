using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000026 RID: 38
	[ProtoContract]
	public class FastCacheData
	{
		// Token: 0x040000D7 RID: 215
		[ProtoMember(1)]
		public int ID;

		// Token: 0x040000D8 RID: 216
		[ProtoMember(2)]
		public bool Flag_BaseInfo;

		// Token: 0x040000D9 RID: 217
		[ProtoMember(3)]
		public string Position;

		// Token: 0x040000DA RID: 218
		[ProtoMember(4)]
		public long ZhanLi;
	}
}
