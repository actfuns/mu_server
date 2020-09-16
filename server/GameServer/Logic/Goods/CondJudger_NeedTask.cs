using System;

namespace GameServer.Logic.Goods
{
	
	public class CondJudger_NeedTask : ICondJudger
	{
		
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			int needTaskId = Global.SafeConvertToInt32(arg);
			if (client != null)
			{
				if (client.ClientData.MainTaskID >= needTaskId)
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(685, new object[0]), GlobalNew.GetTaskName(needTaskId)));
			}
			return bOK;
		}
	}
}
