using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200033A RID: 826
	[ProtoContract]
	public class KingOfBattleStoreSaleData
	{
		// Token: 0x040015A6 RID: 5542
		[ProtoMember(1)]
		public int ID;

		// Token: 0x040015A7 RID: 5543
		[ProtoMember(2)]
		public int Purchase;
	}
}
