using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x02000503 RID: 1283
	[ProtoContract]
	public class HuanYingSiYuanLianshaOver
	{
		// Token: 0x040021EA RID: 8682
		[ProtoMember(1)]
		public int KillerZoneID;

		// Token: 0x040021EB RID: 8683
		[ProtoMember(2)]
		public string KillerName = "";

		// Token: 0x040021EC RID: 8684
		[ProtoMember(3)]
		public int KillerOccupation;

		// Token: 0x040021ED RID: 8685
		[ProtoMember(4)]
		public int KillerSide;

		// Token: 0x040021EE RID: 8686
		[ProtoMember(5)]
		public int KilledZoneID;

		// Token: 0x040021EF RID: 8687
		[ProtoMember(6)]
		public string KilledName = "";

		// Token: 0x040021F0 RID: 8688
		[ProtoMember(7)]
		public int KilledOccupation;

		// Token: 0x040021F1 RID: 8689
		[ProtoMember(8)]
		public int KilledSide;

		// Token: 0x040021F2 RID: 8690
		[ProtoMember(9)]
		public int ExtScore;
	}
}
