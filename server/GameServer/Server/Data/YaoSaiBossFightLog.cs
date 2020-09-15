using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020007F6 RID: 2038
	[ProtoContract]
	public class YaoSaiBossFightLog
	{
		// Token: 0x04004393 RID: 17299
		[ProtoMember(1)]
		public int OtherRid;

		// Token: 0x04004394 RID: 17300
		[ProtoMember(2)]
		public string OtherRname;

		// Token: 0x04004395 RID: 17301
		[ProtoMember(3)]
		public int InviteType;

		// Token: 0x04004396 RID: 17302
		[ProtoMember(4)]
		public int FightLife;
	}
}
