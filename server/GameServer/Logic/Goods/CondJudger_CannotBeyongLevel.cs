using System;

namespace GameServer.Logic.Goods
{
	
	public class CondJudger_CannotBeyongLevel : ICondJudger
	{
		
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
