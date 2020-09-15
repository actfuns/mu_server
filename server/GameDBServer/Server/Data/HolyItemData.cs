using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000062 RID: 98
	[ProtoContract]
	public class HolyItemData
	{
		// Token: 0x0400021F RID: 543
		[ProtoMember(1)]
		public sbyte m_sType = 0;

		// Token: 0x04000220 RID: 544
		[ProtoMember(2)]
		public Dictionary<sbyte, HolyItemPartData> m_PartArray = new Dictionary<sbyte, HolyItemPartData>();
	}
}
