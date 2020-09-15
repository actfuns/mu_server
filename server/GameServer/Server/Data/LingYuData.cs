using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000155 RID: 341
	[ProtoContract]
	public class LingYuData
	{
		// Token: 0x0600046E RID: 1134 RVA: 0x0003FB0F File Offset: 0x0003DD0F
		public LingYuData()
		{
			this.Type = -1;
			this.Level = 0;
			this.Suit = 0;
		}

		// Token: 0x04000786 RID: 1926
		[ProtoMember(1)]
		public int Type;

		// Token: 0x04000787 RID: 1927
		[ProtoMember(2)]
		public int Level;

		// Token: 0x04000788 RID: 1928
		[ProtoMember(3)]
		public int Suit;
	}
}
