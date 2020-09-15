using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200057E RID: 1406
	[ProtoContract]
	public class PlayerJingJiSkillData
	{
		// Token: 0x04002600 RID: 9728
		[ProtoMember(1)]
		public int skillID;

		// Token: 0x04002601 RID: 9729
		[ProtoMember(2)]
		public int skillLevel;
	}
}
