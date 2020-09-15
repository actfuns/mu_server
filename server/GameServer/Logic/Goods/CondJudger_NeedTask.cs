using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x0200046F RID: 1135
	public class CondJudger_NeedTask : ICondJudger
	{
		// Token: 0x060014C3 RID: 5315 RVA: 0x00145784 File Offset: 0x00143984
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
