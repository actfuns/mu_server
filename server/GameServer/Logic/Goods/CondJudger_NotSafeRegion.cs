using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x0200046E RID: 1134
	public class CondJudger_NotSafeRegion : ICondJudger
	{
		// Token: 0x060014C1 RID: 5313 RVA: 0x001456E0 File Offset: 0x001438E0
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null)
			{
				GameMap gameMap = null;
				if (GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap) && gameMap != null)
				{
					bOK = !gameMap.InSafeRegionList(client.CurrentGrid);
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(687, new object[0]), ""));
			}
			return bOK;
		}
	}
}
