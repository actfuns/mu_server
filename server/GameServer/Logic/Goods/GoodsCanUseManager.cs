using System;
using System.Collections.Generic;
using Server.Tools.Pattern;

namespace GameServer.Logic.Goods
{
	// Token: 0x0200047F RID: 1151
	public class GoodsCanUseManager : SingletonTemplate<GoodsCanUseManager>
	{
		// Token: 0x060014FD RID: 5373 RVA: 0x00149028 File Offset: 0x00147228
		private GoodsCanUseManager()
		{
		}

		// Token: 0x060014FE RID: 5374 RVA: 0x00149040 File Offset: 0x00147240
		public void Init()
		{
			this.canUseDict["WingSuit".ToLower()] = new CondJudger_WingSuit();
			this.canUseDict["ChengJiuLevel".ToLower()] = new CondJudger_ChengJiuLvl();
			this.canUseDict["JunXianLevel".ToLower()] = new CondJudger_JunXianLvl();
			this.canUseDict["ZhuanShengLevel".ToLower()] = new CondJudger_ChangeLife();
			this.canUseDict["Level".ToLower()] = new CondJudger_RoleLevel();
			this.canUseDict["VIP".ToLower()] = new CondJudger_VIPLvl();
			this.canUseDict["HuFuSuit".ToLower()] = new CondJudger_HuFuSuit();
			this.canUseDict["DaTianShiSuit".ToLower()] = new CondJudger_DaTianShiSuit();
			this.canUseDict["NeedMarry".ToLower()] = new CondJudger_NeedMarry();
			this.canUseDict["NeedTask".ToLower()] = new CondJudger_NeedTask();
			this.canUseDict["AddIntoBH".ToLower()] = new CondJudger_AddIntoBH();
			this.canUseDict["CanNotBeyondLevel".ToLower()] = new CondJudger_CannotBeyongLevel();
			this.canUseDict["FEIANQUANQU".ToLower()] = new CondJudger_NotSafeRegion();
			this.canUseDict["UseYuanBao".ToLower()] = new CondJudger_YuanBaoMoreThan();
			this.canUseDict["NeedOpen".ToLower()] = new CondJudger_NeedOpen();
			this.canUseDict["NoBuff".ToLower()] = new CondJudger_NoBuff();
			this.canUseDict["WingSuitLess".ToLower()] = new CondJudger_WingSuitLess();
			this.canUseDict["WingNotPerfect".ToLower()] = new CondJudger_WingNotPerfect();
			this.canUseDict["EquipUsingAll".ToLower()] = new CondJudger_EquipUsingAll();
			this.canUseDict["EquipUsing".ToLower()] = new CondJudger_EquipUsing();
			this.canUseDict["FruitSuitNotMax".ToLower()] = new CondJudger_FruitSuitNotMax();
			this.canUseDict["XingHunNotMax".ToLower()] = new CondJudger_XingHunNotMax();
			this.canUseDict["MerlinLess".ToLower()] = new CondJudger_MerlinLess();
		}

		// Token: 0x060014FF RID: 5375 RVA: 0x001492BC File Offset: 0x001474BC
		public bool CheckCanUse_ByToType(GameClient client, int goodsID)
		{
			string failedMsg = string.Empty;
			return this.CheckCanUse_ByToType(client, goodsID, out failedMsg);
		}

		// Token: 0x06001500 RID: 5376 RVA: 0x001492E0 File Offset: 0x001474E0
		public bool CheckCanUse_ByToType(GameClient client, int goodsID, out string failedMsg)
		{
			failedMsg = "";
			SystemXmlItem systemGoodsItem = null;
			bool result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoodsItem))
			{
				result = false;
			}
			else
			{
				string condIdx = systemGoodsItem.GetStringValue("ToType");
				string condArg = systemGoodsItem.GetStringValue("ToTypeProperty");
				if (string.IsNullOrEmpty(condIdx) || condIdx == "-1")
				{
					result = true;
				}
				else
				{
					condIdx = condIdx.ToLower();
					ICondJudger judger = null;
					result = (!this.canUseDict.TryGetValue(condIdx, out judger) || judger.Judge(client, condArg, out failedMsg));
				}
			}
			return result;
		}

		// Token: 0x04001E3F RID: 7743
		private Dictionary<string, ICondJudger> canUseDict = new Dictionary<string, ICondJudger>();
	}
}
