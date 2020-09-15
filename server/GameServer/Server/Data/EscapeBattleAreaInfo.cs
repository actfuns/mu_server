using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000093 RID: 147
	[ProtoContract]
	public class EscapeBattleAreaInfo
	{
		// Token: 0x04000385 RID: 901
		[ProtoMember(1)]
		public int AreaID;

		// Token: 0x04000386 RID: 902
		[ProtoMember(2)]
		public int PosX;

		// Token: 0x04000387 RID: 903
		[ProtoMember(3)]
		public int PosY;
	}
}
