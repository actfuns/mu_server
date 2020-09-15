using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.AoYunDaTi
{
	// Token: 0x02000203 RID: 515
	[ProtoContract]
	public class AoyunQuestionItemData
	{
		// Token: 0x04000B69 RID: 2921
		[ProtoMember(1)]
		public int QuestionId = 0;

		// Token: 0x04000B6A RID: 2922
		[ProtoMember(2)]
		public string Question = null;

		// Token: 0x04000B6B RID: 2923
		[ProtoMember(3)]
		public string[] AnswerContentArray;

		// Token: 0x04000B6C RID: 2924
		[ProtoMember(4)]
		public bool UseTianShi;

		// Token: 0x04000B6D RID: 2925
		[ProtoMember(5)]
		public bool UseEMo;

		// Token: 0x04000B6E RID: 2926
		[ProtoMember(6)]
		public int RoleAnswer;

		// Token: 0x04000B6F RID: 2927
		[ProtoMember(7)]
		public DateTime EndTime;

		// Token: 0x04000B70 RID: 2928
		[ProtoMember(8)]
		public List<bool> QuestionState;

		// Token: 0x04000B71 RID: 2929
		[ProtoMember(9)]
		public int RolePoint;
	}
}
