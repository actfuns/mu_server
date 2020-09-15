using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000398 RID: 920
	[ProtoContract]
	public class OnePieceTreasureData
	{
		// Token: 0x04001840 RID: 6208
		[ProtoMember(1)]
		public int PosID = 0;

		// Token: 0x04001841 RID: 6209
		[ProtoMember(2)]
		public int EventID = 0;

		// Token: 0x04001842 RID: 6210
		[ProtoMember(3)]
		public int RollNumNormal = 0;

		// Token: 0x04001843 RID: 6211
		[ProtoMember(4)]
		public int RollNumMiracle = 0;

		// Token: 0x04001844 RID: 6212
		[ProtoMember(5)]
		public long ResetPosTicks = 0L;
	}
}
