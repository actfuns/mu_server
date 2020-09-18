using System;
using System.Collections.Generic;

namespace GameServer.Logic.AoYunDaTi
{
	
	public class QuestionData
	{
		
		public object Mutex = new object();

		
		public bool SendAnswer = true;

		
		public int CurrentQuestionTimeKey = 0;

		
		public List<int> CurrentQuestionBankKeyList = new List<int>();

		
		public int CurrentQuestionNum = -1;

		
		public DateTime CurrentQuestionBeginTime = DateTime.MaxValue;
	}
}
