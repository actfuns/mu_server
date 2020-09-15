using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x02000469 RID: 1129
	internal class CondJudger_EquipSuit : ICondJudger
	{
		// Token: 0x060014B7 RID: 5303 RVA: 0x001453B0 File Offset: 0x001435B0
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int iNeedSuit = int.MaxValue;
				if (int.TryParse(arg, out iNeedSuit) && client.ClientData._ReplaceExtArg.CurrEquipSuit >= iNeedSuit)
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format("当前装备的品阶不能低于{0}", arg);
			}
			return bOK;
		}
	}
}
