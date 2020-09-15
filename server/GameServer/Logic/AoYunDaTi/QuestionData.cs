using System;
using System.Collections.Generic;

namespace GameServer.Logic.AoYunDaTi
{
	// Token: 0x02000206 RID: 518
	public class QuestionData
	{
		// Token: 0x04000B82 RID: 2946
		public object Mutex = new object();

		// Token: 0x04000B83 RID: 2947
		public bool SendAnswer = true;

		// Token: 0x04000B84 RID: 2948
		public int CurrentQuestionTimeKey = 0;

		// Token: 0x04000B85 RID: 2949
		public List<int> CurrentQuestionBankKeyList = new List<int>();

		// Token: 0x04000B86 RID: 2950
		public int CurrentQuestionNum = -1;

		// Token: 0x04000B87 RID: 2951
		public DateTime CurrentQuestionBeginTime = DateTime.MaxValue;
	}
}
