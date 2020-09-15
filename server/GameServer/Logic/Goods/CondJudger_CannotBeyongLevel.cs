using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x0200046D RID: 1133
	public class CondJudger_CannotBeyongLevel : ICondJudger
	{
		// Token: 0x060014BF RID: 5311 RVA: 0x001455F8 File Offset: 0x001437F8
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				string[] fields = arg.Split(new char[]
				{
					'|'
				});
				if (fields.Length == 2)
				{
					int maxChangeLife = -1;
					int maxLvl = -1;
					if (int.TryParse(fields[0], out maxChangeLife) && int.TryParse(fields[1], out maxLvl))
					{
						if (client.ClientData.ChangeLifeCount < maxChangeLife || (client.ClientData.ChangeLifeCount == maxChangeLife && client.ClientData.Level <= maxLvl))
						{
							bOK = true;
						}
					}
				}
			}
			if (!bOK)
			{
				failedMsg = GLang.GetLang(139, new object[0]);
			}
			return bOK;
		}
	}
}
