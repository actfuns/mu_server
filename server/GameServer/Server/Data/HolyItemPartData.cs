using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000478 RID: 1144
	[ProtoContract]
	public class HolyItemPartData
	{
		// Token: 0x04001E1F RID: 7711
		[ProtoMember(1)]
		public sbyte m_sSuit = 0;

		// Token: 0x04001E20 RID: 7712
		[ProtoMember(2)]
		public int m_nSlice = 0;

		// Token: 0x04001E21 RID: 7713
		[ProtoMember(3)]
		public int m_nFailCount = 0;
	}
}
