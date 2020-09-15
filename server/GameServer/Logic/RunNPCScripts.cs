using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020007C7 RID: 1991
	internal class RunNPCScripts
	{
		// Token: 0x060034AF RID: 13487 RVA: 0x002EB89C File Offset: 0x002E9A9C
		public static int ProcessNPCScript(GameClient client, int scriptID, int npcID)
		{
			int errorCode = 0;
			int result;
			if (Global.FilterNPCScriptByID(client, scriptID, out errorCode))
			{
				result = errorCode;
			}
			else
			{
				List<MagicActionItem> magicActionItemList = null;
				if (!GameManager.SystemMagicActionMgr.NPCScriptActionsDict.TryGetValue(scriptID, out magicActionItemList) || null == magicActionItemList)
				{
					result = -3;
				}
				else if (magicActionItemList.Count <= 0)
				{
					result = -1;
				}
				else
				{
					for (int i = 0; i < magicActionItemList.Count; i++)
					{
						MagicAction.ProcessAction(client, client, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, -1, -1, 0, 1, -1, npcID, 0, -1, 0, false, false, 1.0, 1, 0.0);
					}
					result = 0;
				}
			}
			return result;
		}
	}
}
