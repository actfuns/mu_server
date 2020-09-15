using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000591 RID: 1425
	[ProtoContract]
	public class SkillData
	{
		// Token: 0x0400281F RID: 10271
		[ProtoMember(1)]
		public int DbID;

		// Token: 0x04002820 RID: 10272
		[ProtoMember(2)]
		public int SkillID;

		// Token: 0x04002821 RID: 10273
		[ProtoMember(3)]
		public int SkillLevel;

		// Token: 0x04002822 RID: 10274
		[ProtoMember(4)]
		public int UsedNum;
	}
}
