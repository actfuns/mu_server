using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000554 RID: 1364
	[ProtoContract]
	public class FuBenData
	{
		// Token: 0x040024A0 RID: 9376
		[ProtoMember(1)]
		public int FuBenID;

		// Token: 0x040024A1 RID: 9377
		[ProtoMember(2)]
		public int DayID;

		// Token: 0x040024A2 RID: 9378
		[ProtoMember(3)]
		public int EnterNum;

		// Token: 0x040024A3 RID: 9379
		[ProtoMember(4)]
		public int QuickPassTimer;

		// Token: 0x040024A4 RID: 9380
		[ProtoMember(5)]
		public int FinishNum;
	}
}
