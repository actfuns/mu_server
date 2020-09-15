using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000479 RID: 1145
	[ProtoContract]
	public class HolyItemData
	{
		// Token: 0x04001E22 RID: 7714
		[ProtoMember(1)]
		public sbyte m_sType = 0;

		// Token: 0x04001E23 RID: 7715
		[ProtoMember(2)]
		public Dictionary<sbyte, HolyItemPartData> m_PartArray = new Dictionary<sbyte, HolyItemPartData>();
	}
}
