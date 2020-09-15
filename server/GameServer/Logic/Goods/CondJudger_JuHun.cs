using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x02000468 RID: 1128
	internal class CondJudger_JuHun : ICondJudger
	{
		// Token: 0x060014B5 RID: 5301 RVA: 0x00145334 File Offset: 0x00143534
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int iNeedType = int.MaxValue;
				if (int.TryParse(arg, out iNeedType) && client.ClientData._ReplaceExtArg.CurrEquipJuHun >= iNeedType)
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
