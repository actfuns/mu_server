using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000131 RID: 305
	[ProtoContract]
	public class FluorescentGemSaveDBData
	{
		// Token: 0x040006AD RID: 1709
		[ProtoMember(1)]
		public int _RoleID;

		// Token: 0x040006AE RID: 1710
		[ProtoMember(2)]
		public int _GoodsID;

		// Token: 0x040006AF RID: 1711
		[ProtoMember(3)]
		public int _Position;

		// Token: 0x040006B0 RID: 1712
		[ProtoMember(4)]
		public int _GemType;

		// Token: 0x040006B1 RID: 1713
		[ProtoMember(5)]
		public int _Bind;
	}
}
