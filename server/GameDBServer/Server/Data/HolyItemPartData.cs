using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000061 RID: 97
	[ProtoContract]
	public class HolyItemPartData
	{
		// Token: 0x0400021C RID: 540
		[ProtoMember(1)]
		public sbyte m_sSuit = 0;

		// Token: 0x0400021D RID: 541
		[ProtoMember(2)]
		public int m_nSlice = 0;

		// Token: 0x0400021E RID: 542
		[ProtoMember(3)]
		public int m_nFailCount = 0;
	}
}
