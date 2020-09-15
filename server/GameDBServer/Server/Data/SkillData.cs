using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000AF RID: 175
	[ProtoContract]
	public class SkillData
	{
		// Token: 0x04000494 RID: 1172
		[ProtoMember(1)]
		public int DbID;

		// Token: 0x04000495 RID: 1173
		[ProtoMember(2)]
		public int SkillID;

		// Token: 0x04000496 RID: 1174
		[ProtoMember(3)]
		public int SkillLevel;

		// Token: 0x04000497 RID: 1175
		[ProtoMember(4)]
		public int UsedNum;
	}
}
