using System;
using GameServer.Core.Executor;

namespace GameServer.Logic.SecondPassword
{
	
	public class SecPwdState
	{
		
		public string SecPwd = "";

		
		public DateTime AuthDeadTime = TimeUtil.NowDateTime();

		
		public bool NeedVerify = false;
	}
}
