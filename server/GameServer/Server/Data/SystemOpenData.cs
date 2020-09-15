using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000021 RID: 33
	[ProtoContract]
	public class SystemOpenData : ICloneable
	{
		// Token: 0x06000034 RID: 52 RVA: 0x00006114 File Offset: 0x00004314
		public object Clone()
		{
			return base.MemberwiseClone();
		}

		// Token: 0x040000C6 RID: 198
		[ProtoMember(1)]
		public int paimaihangjinbi;

		// Token: 0x040000C7 RID: 199
		[ProtoMember(2)]
		public int paimaihangzuanshi;

		// Token: 0x040000C8 RID: 200
		[ProtoMember(3)]
		public int paimaihangmobi;

		// Token: 0x040000C9 RID: 201
		[ProtoMember(4)]
		public int bangzuan;

		// Token: 0x040000CA RID: 202
		[ProtoMember(5)]
		public int zuanshi;

		// Token: 0x040000CB RID: 203
		[ProtoMember(6)]
		public int mobi;

		// Token: 0x040000CC RID: 204
		[ProtoMember(7)]
		public int paimaijiemianmobi;

		// Token: 0x040000CD RID: 205
		[ProtoMember(8)]
		public int paimaihangyinpiao;
	}
}
