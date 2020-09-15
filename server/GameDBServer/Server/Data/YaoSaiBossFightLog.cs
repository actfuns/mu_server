using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000192 RID: 402
	[ProtoContract]
	public class YaoSaiBossFightLog
	{
		// Token: 0x04000931 RID: 2353
		[ProtoMember(1)]
		public int OtherRid;

		// Token: 0x04000932 RID: 2354
		[ProtoMember(2)]
		public string OtherRname;

		// Token: 0x04000933 RID: 2355
		[ProtoMember(3)]
		public int InviteType;

		// Token: 0x04000934 RID: 2356
		[ProtoMember(4)]
		public int FightLife;
	}
}
