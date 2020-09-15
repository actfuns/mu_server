using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200007E RID: 126
	[ProtoContract]
	public class LingYuData
	{
		// Token: 0x06000119 RID: 281 RVA: 0x000069DE File Offset: 0x00004BDE
		public LingYuData()
		{
			this.Type = -1;
			this.Level = 0;
			this.Suit = 0;
		}

		// Token: 0x040002AB RID: 683
		[ProtoMember(1)]
		public int Type;

		// Token: 0x040002AC RID: 684
		[ProtoMember(2)]
		public int Level;

		// Token: 0x040002AD RID: 685
		[ProtoMember(3)]
		public int Suit;
	}
}
