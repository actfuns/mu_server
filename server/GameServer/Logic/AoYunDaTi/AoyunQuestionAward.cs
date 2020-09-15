using System;
using ProtoBuf;

namespace GameServer.Logic.AoYunDaTi
{
	// Token: 0x02000204 RID: 516
	[ProtoContract]
	public class AoyunQuestionAward
	{
		// Token: 0x04000B72 RID: 2930
		[ProtoMember(1)]
		public int Result;

		// Token: 0x04000B73 RID: 2931
		[ProtoMember(2)]
		public int RightAnswer;

		// Token: 0x04000B74 RID: 2932
		[ProtoMember(3)]
		public int TianShiCount;

		// Token: 0x04000B75 RID: 2933
		[ProtoMember(4)]
		public int EMoCount;

		// Token: 0x04000B76 RID: 2934
		[ProtoMember(5)]
		public int RolePoint;
	}
}
