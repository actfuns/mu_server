using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000055 RID: 85
	[ProtoContract]
	public class FuBenData
	{
		// Token: 0x040001C9 RID: 457
		[ProtoMember(1)]
		public int FuBenID;

		// Token: 0x040001CA RID: 458
		[ProtoMember(2)]
		public int DayID;

		// Token: 0x040001CB RID: 459
		[ProtoMember(3)]
		public int EnterNum;

		// Token: 0x040001CC RID: 460
		[ProtoMember(4)]
		public int QuickPassTimer;

		// Token: 0x040001CD RID: 461
		[ProtoMember(5)]
		public int FinishNum;
	}
}
