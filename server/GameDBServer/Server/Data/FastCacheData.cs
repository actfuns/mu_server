using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200004D RID: 77
	[ProtoContract]
	public class FastCacheData
	{
		// Token: 0x04000191 RID: 401
		[ProtoMember(1)]
		public int ID;

		// Token: 0x04000192 RID: 402
		[ProtoMember(2)]
		public bool Flag_BaseInfo;

		// Token: 0x04000193 RID: 403
		[ProtoMember(3)]
		public string Position;

		// Token: 0x04000194 RID: 404
		[ProtoMember(4)]
		public long ZhanLi;
	}
}
