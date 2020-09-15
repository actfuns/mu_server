using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x02000466 RID: 1126
	internal class CondJudger_EquipAppendLvl : ICondJudger
	{
		// Token: 0x060014B1 RID: 5297 RVA: 0x0014523C File Offset: 0x0014343C
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int iNeedLvl = int.MaxValue;
				if (int.TryParse(arg, out iNeedLvl) && client.ClientData._ReplaceExtArg.CurrEquipZhuiJiaLevel >= iNeedLvl)
				{
					bOK = true;
				}
			}
			if (!bOK)
			{
				failedMsg = string.Format("当前操作的装备的追加等级不能低于{0}", arg);
			}
			return bOK;
		}
	}
}
