using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Interface;
using GameServer.Logic.Building;
using GameServer.Logic.MUWings;
using GameServer.Logic.Name;
using GameServer.Logic.NewBufferExt;
using GameServer.Logic.Talent;
using GameServer.Logic.TuJian;
using GameServer.Logic.YueKa;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000741 RID: 1857
	internal class MagicAction
	{
		// Token: 0x06002EC9 RID: 11977 RVA: 0x0028C230 File Offset: 0x0028A430
		private static bool ProcessActionSeveralTimes(IObject self, IObject obj, MagicActionIDs id, double[] actionParams, int binding, int actionGoodsID, bool bIsVerify, int timesNum)
		{
			bool ret = true;
			switch (id)
			{
			case MagicActionIDs.NEW_ADD_CHENGJIU:
				if (self is GameClient)
				{
					int chengJiuValue = (int)actionParams[0] * timesNum;
					ChengJiuManager.AddChengJiuPoints(self as GameClient, "脚本增加成就", chengJiuValue, true, true);
				}
				break;
			case MagicActionIDs.ADD_SHENGWANG:
				if (self is GameClient)
				{
					GameClient client = self as GameClient;
					int nAddValue = (int)actionParams[0] * timesNum;
					if (nAddValue > 0)
					{
						GameManager.ClientMgr.ModifyShengWangValue(client, nAddValue, "脚本增加声望", true, true);
						string strinfo = string.Format(GLang.GetLang(429, new object[0]), client.ClientData.RoleName, nAddValue);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					}
				}
				break;
			case MagicActionIDs.ADD_XINGHUN:
				if (self is GameClient)
				{
					GameClient client = self as GameClient;
					int nAddValue = (int)actionParams[0] * timesNum;
					if (nAddValue > 0)
					{
						GameManager.ClientMgr.ModifyStarSoulValue(client, nAddValue, "脚本增加星魂", true, true);
						string strinfo = string.Format(GLang.GetLang(430, new object[0]), client.ClientData.RoleName, nAddValue);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					}
				}
				break;
			case MagicActionIDs.NEW_PACK_JINGYUAN:
				if (self is GameClient)
				{
					int addValue = (int)actionParams[0] * timesNum;
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(self as GameClient, addValue, "脚本增加魔晶", true, true, false);
				}
				break;
			case MagicActionIDs.ADDYSFM:
			{
				int num = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyYuanSuFenMoValue(self as GameClient, num, "道具ADDYSFM", true, false);
				break;
			}
			case MagicActionIDs.ADD_LINGJING:
			{
				int num = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyMUMoHeValue(self as GameClient, num, "道具ADD_LINGJING", true, true, false);
				break;
			}
			case MagicActionIDs.ADD_ZAIZAO:
			{
				int num = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyZaiZaoValue(self as GameClient, num, "道具ADD_ZAIZAO", true, true, false);
				break;
			}
			case MagicActionIDs.ADD_RONGYAO:
			{
				int num = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyTianTiRongYaoValue(self as GameClient, num, "ADD_RONGYAO", true);
				break;
			}
			case MagicActionIDs.ADD_GUARDPOINT:
			{
				int point = (int)actionParams[0] * timesNum;
				SingletonTemplate<GuardStatueManager>.Instance().AddGuardPoint(self as GameClient, point, "道具脚本");
				break;
			}
			case MagicActionIDs.NEW_ADD_YINGGUANG:
				if (self is GameClient)
				{
					GameClient client = self as GameClient;
					int addValueMin = (int)actionParams[0];
					int addValueMax = (int)actionParams[1];
					int nAddValue = 0;
					for (int i = 0; i < timesNum; i++)
					{
						nAddValue += Global.GetRandomNumber(addValueMin, addValueMax + 1);
					}
					GameManager.FluorescentGemMgr.AddFluorescentPoint(client, nAddValue, "使用物品获得", false);
					GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.FluorescentGem, client.ClientData.FluorescentPoint);
				}
				break;
			case MagicActionIDs.ADD_BANGGONG:
			{
				int num = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.AddBangGong(self as GameClient, ref num, AddBangGongTypes.ADD_BANGGONG, 0);
				break;
			}
			case MagicActionIDs.ADD_ZHANMENGGAIMING:
			{
				GameClient client = self as GameClient;
				if (client != null)
				{
					int num = (int)actionParams[0] * timesNum;
					int dbRet = Global.sendToDB<int, string>(14009, string.Format("{0}:{1}", self.GetObjectID(), num), client.ServerId);
					if (dbRet > 0)
					{
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战盟改名次数", "ADD_ZHANMENGGAIMING", "系统", client.ClientData.RoleName, "修改", num, client.ClientData.ZoneID, client.strUserID, dbRet, client.ServerId, null);
						BangHuiDetailData data = Global.GetBangHuiDetailData(client.ClientData.RoleID, client.ClientData.Faction, client.ServerId);
						data.CanModNameTimes = dbRet;
					}
					else
					{
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战盟改名次数", "ADD_ZHANMENGGAIMING", "系统", client.ClientData.RoleName, "修改", 0, client.ClientData.ZoneID, client.strUserID, dbRet, client.ServerId, null);
					}
				}
				break;
			}
			case MagicActionIDs.ADD_LANGHUN:
			{
				int val = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyLangHunFenMoValue(self as GameClient, val, "道具脚本", true, true);
				break;
			}
			case MagicActionIDs.ADD_ZHENGBADIANSHU:
			{
				int val = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyZhengBaPointValue(self as GameClient, val, "道具脚本", true, true);
				break;
			}
			case MagicActionIDs.ADD_WANGZHEDIANSHU:
			{
				int val = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyKingOfBattlePointValue(self as GameClient, val, "道具脚本", true, true);
				break;
			}
			case MagicActionIDs.ADD_MEILIDIANSHU:
			{
				int val = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyOrnamentCharmPointValue(self as GameClient, val, "道具脚本", true, true, false);
				break;
			}
			case MagicActionIDs.ADD_SHENLIJINGHUA:
			{
				int val = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(self as GameClient, val, "道具脚本", true, true);
				break;
			}
			case MagicActionIDs.NEW_ADD_MONEY:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (int)actionParams[0] * timesNum, "脚本添加绑金", true);
				}
				break;
			case MagicActionIDs.NEW_ADD_YINLIANG:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (int)actionParams[0] * timesNum, "脚本增加金币一", false);
				}
				break;
			case MagicActionIDs.ADD_DJ:
				if (self is GameClient)
				{
					GameClient client = self as GameClient;
					int userMoney = (int)actionParams[0] * timesNum;
					if (userMoney >= 0)
					{
						GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, Math.Abs(userMoney), "ADD_DJ公式", ActivityTypes.None, "");
					}
					else
					{
						GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, Math.Abs(userMoney), "ADD_DJ公式", true, true, false, DaiBiSySType.None);
					}
					GameManager.SystemServerEvents.AddEvent(string.Format("角色获取元宝, roleID={0}({1}), UserMoney={2}, newUserMoney={3}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.RoleName,
						client.ClientData.UserMoney,
						userMoney
					}), EventLevels.Record);
				}
				break;
			case MagicActionIDs.ADD_BINDYUANBAO:
				if (self is GameClient)
				{
					GameClient client = self as GameClient;
					int gold = (int)actionParams[0] * timesNum;
					if (gold >= 0)
					{
						GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, Math.Abs(gold), "ADD_BINDYUANBAO");
					}
					else
					{
						GameManager.ClientMgr.SubUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, Math.Abs(gold), "ADD_BINDYUANBAO", false);
					}
					GameManager.SystemServerEvents.AddEvent(string.Format("角色获取金币, roleID={0}({1}), Gold={2}, newGold={3}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.RoleName,
						client.ClientData.Gold,
						gold
					}), EventLevels.Record);
				}
				break;
			case MagicActionIDs.ADD_GOODWILL:
				if (self is GameClient)
				{
					GameClient client = self as GameClient;
					int nAddValue = (int)actionParams[0] * timesNum;
					if (nAddValue > 0)
					{
						if (bIsVerify)
						{
							ret = MarriageOtherLogic.getInstance().CanAddMarriageGoodWill(client);
							if (!ret)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(67, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
						}
						else
						{
							MarriageOtherLogic.getInstance().UpdateMarriageGoodWill(client, nAddValue);
						}
					}
				}
				break;
			case MagicActionIDs.FALL_BAOXIANG:
			case MagicActionIDs.FALL_BAOXIANG_2:
				GoodsBaoXiang.ProcessFallBaoXiang_StepTwo(self as GameClient, (int)actionParams[0], (int)actionParams[1], binding, actionGoodsID);
				break;
			case MagicActionIDs.MU_RANDOMSHIZHUANG:
				FashionManager.getInstance().FashionActiveByMagic(self as GameClient, actionParams);
				break;
			case MagicActionIDs.ADD_LINGDICAIJI_COUNT:
			{
				GameClient client = self as GameClient;
				if (client.ClientData.LingDiCaiJiNum > LingDiCaiJiManager.WeeklyNum)
				{
					client.ClientData.LingDiCaiJiNum = LingDiCaiJiManager.WeeklyNum;
				}
				client.ClientData.LingDiCaiJiNum -= Convert.ToInt32(actionParams[0] * (double)timesNum);
				Global.SaveRoleParamsInt32ValueToDB(client, "10158", client.ClientData.LingDiCaiJiNum, true);
				if (LingDiCaiJiManager.getInstance().GetLingDiType(client.ClientData.MapCode) != 2)
				{
					client.sendCmd(1828, (LingDiCaiJiManager.WeeklyNum - client.ClientData.LingDiCaiJiNum).ToString(), false);
				}
				break;
			}
			case MagicActionIDs.ADD_NENGLIANG:
			{
				int nengLiangType = (int)actionParams[0];
				int addValue = (int)actionParams[1] * timesNum;
				BuildingManager.getInstance().ModifyNengLiangPointsValue(self as GameClient, nengLiangType, addValue, "道具脚本", true, true);
				break;
			}
			case MagicActionIDs.ADD_JINGLINGSHENJI:
			{
				int addValue = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyShenJiJiFenValue(self as GameClient, addValue, "道具脚本", true, true);
				break;
			}
			case MagicActionIDs.ADD_FUWENZHICHEN:
			{
				int val = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyFuWenZhiChenPointsValue(self as GameClient, val, "道具脚本", true, true, false);
				break;
			}
			case MagicActionIDs.ADD_JUEXINGZHICHEN:
			{
				int val = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyJueXingZhiChenValue(self as GameClient, val, "道具脚本", true, true, false);
				break;
			}
			case MagicActionIDs.ADD_JUEXING:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyJueXingValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), false);
				}
				break;
			case MagicActionIDs.ADD_HUNJING:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyHunJingValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_MOBI:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyMoBiValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), false);
				}
				break;
			case MagicActionIDs.ADD_JINGLINGJUEXINGSHI:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyYuanSuJueXingShiValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_FUMOLINGSHI:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyFuMoLingShiValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_FENYINJINGSHI:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyRebornFengYinJinShiValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_CHONGSHENGJINGSHI:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyRebornChongShengJinShiValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_XUANCAIJINGSHI:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyRebornXuanCaiJinShiValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_REBORNEQUIP1:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyRebornCuiLianPointValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_REBORNEQUIP2:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyRebornDuanZaoPointValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_REBORNEQUIP3:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyRebornNiePanPointValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_ZHANDUIRONGYAO:
			{
				int val = (int)actionParams[0] * timesNum;
				if (val < 2147483647)
				{
					GameManager.ClientMgr.ModifyTeamRongYaoValue(self as GameClient, val, "道具脚本", false);
				}
				break;
			}
			case MagicActionIDs.ADD_TEAMPOINT:
			{
				int val = (int)actionParams[0] * timesNum;
				if (val < 2147483647)
				{
					GameManager.ClientMgr.ModifyTeamPointValue(self as GameClient, val, "道具脚本", false);
				}
				break;
			}
			case MagicActionIDs.ADD_LUCKSTAR_MOJING:
			{
				int addValue = Math.Abs((int)actionParams[0] * timesNum);
				GameManager.ClientMgr.ModifyLuckStarValue(self as GameClient, addValue, "道具脚本", false, DaiBiSySType.None);
				GameManager.ClientMgr.ModifyTianDiJingYuanValue(self as GameClient, addValue, "脚本增加魔晶", true, true, false);
				break;
			}
			case MagicActionIDs.ADD_LUCKSTAR:
			{
				int addValue = Math.Abs((int)actionParams[0] * timesNum);
				GameManager.ClientMgr.ModifyLuckStarValue(self as GameClient, addValue, "道具脚本", false, DaiBiSySType.None);
				break;
			}
			}
			return ret;
		}

		// Token: 0x06002ECA RID: 11978 RVA: 0x0028D238 File Offset: 0x0028B438
		public static bool ProcessAction(IObject self, IObject obj, MagicActionIDs id, double[] actionParams, int targetX = -1, int targetY = -1, int usedMaigcV = 0, int skillLevel = 1, int skillid = -1, int npcID = 0, int binding = 0, int direction = -1, int actionGoodsID = 0, bool bItemAddVal = false, bool bIsVerify = false, double manyRangeInjuredPercent = 1.0, int timesNum = 1, double shenShiInjurePercent = 0.0)
		{
			if (MagicAction.MaxHitNum == 0)
			{
				MagicAction.MaxHitNum = 8;
			}
			skillLevel = Global.GMin(Global.MaxSkillLevel, skillLevel);
			skillLevel = Global.GMax(0, skillLevel - 1);
			if (self is GameClient)
			{
				GameClient client = self as GameClient;
				skillLevel += TalentManager.GetSkillLevel(client, skillid);
			}
			bool result;
			if (id > MagicActionIDs.ActionSeveralTimesBegin && id < MagicActionIDs.ActionSeveralTimesEnd)
			{
				result = MagicAction.ProcessActionSeveralTimes(self, obj, id, actionParams, binding, actionGoodsID, bIsVerify, timesNum);
			}
			else
			{
				bool ret = true;
				switch (id)
				{
				case MagicActionIDs.FOREVER_ADDHIT:
					if (obj is GameClient)
					{
						double addValue = actionParams[skillLevel];
						(obj as GameClient).RoleBuffer.AddForeverExtProp(18, addValue);
					}
					break;
				case MagicActionIDs.RANDOM_ADDATTACK1:
				{
					double addPercent = actionParams[skillLevel * 2];
					double addValue = actionParams[skillLevel * 2 + 1];
					int percent = (int)(100.0 * addPercent);
					if (Global.GetRandomNumber(0, 101) < percent)
					{
						int extPropIndex = 8;
						int nOcc = Global.CalcOriginalOccupationID(self as GameClient);
						if (1 == nOcc)
						{
							extPropIndex = 10;
						}
						else if (2 == nOcc)
						{
						}
						(obj as GameClient).RoleOnceBuffer.AddTempExtProp(extPropIndex, addValue, 0L);
					}
					else
					{
						ret = false;
					}
					break;
				}
				case MagicActionIDs.RANDOM_ADDATTACK2:
					if (self is GameClient)
					{
						int nOcc = Global.CalcOriginalOccupationID(self as GameClient);
						int attackType = nOcc;
						double minAttackPercent = actionParams[skillLevel * 2];
						double maxAttackPercent = actionParams[skillLevel * 2 + 1];
						double attackPercent = (double)Global.GetRandomNumber((int)(minAttackPercent * 10.0), (int)(maxAttackPercent * 10.0)) / 10.0;
						attackPercent = 1.0 + attackPercent;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, attackPercent, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, attackPercent, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					else
					{
						int attackType = (self as Monster).MonsterInfo.AttackType;
						double minAttackPercent = actionParams[2];
						double maxAttackPercent = actionParams[3];
						double attackPercent = (double)Global.GetRandomNumber((int)(minAttackPercent * 10.0), (int)(maxAttackPercent * 10.0)) / 10.0;
						attackPercent = 1.0 + attackPercent;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, attackPercent, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, attackPercent, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (!(obj is BiaoCheItem))
						{
							if (obj is JunQiItem)
							{
							}
						}
					}
					break;
				case MagicActionIDs.ATTACK_STRAIGHT:
				{
					direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
					Point selfGrid = self.CurrentGrid;
					Point objGrid = obj.CurrentGrid;
					Point nextGrid = Global.GetGridPointByDirection(direction, (int)selfGrid.X, (int)selfGrid.Y);
					double attackPercent = actionParams[skillLevel];
					bool ignoreDefenseAndDodge = nextGrid.X != objGrid.X || nextGrid.Y != objGrid.Y;
					if (!ignoreDefenseAndDodge)
					{
						attackPercent = 1.0;
					}
					if (self is GameClient)
					{
						int nOcc = Global.CalcOriginalOccupationID(self as GameClient);
						int attackType = nOcc;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, attackPercent, 0, 0, skillLevel, 0.0, 0.0, ignoreDefenseAndDodge, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, attackPercent, 0, 0, skillLevel, 0.0, 0.0, ignoreDefenseAndDodge, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					else
					{
						int attackType = (self as Monster).MonsterInfo.AttackType;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, attackPercent, 0, 0, skillLevel, 0.0, 0.0, ignoreDefenseAndDodge, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, attackPercent, 0, 0, skillLevel, 0.0, 0.0, ignoreDefenseAndDodge, 1.0, 0, 0, 0, 0.0);
						}
						else if (!(obj is BiaoCheItem))
						{
							if (!(obj is JunQiItem))
							{
								if (obj is FakeRoleItem)
								{
								}
							}
						}
					}
					break;
				}
				case MagicActionIDs.ATTACK_FRONT:
				{
					direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
					Point selfPoint = self.CurrentPos;
					Point objPoint = obj.CurrentPos;
					int objDirection = (int)Global.GetDirectionByTan(objPoint.X, objPoint.Y, selfPoint.X, selfPoint.Y);
					double attackPercent = actionParams[skillLevel];
					attackPercent = 0.5 * attackPercent;
					bool ignoreDefenseAndDodge = objDirection != direction;
					if (!ignoreDefenseAndDodge)
					{
						attackPercent = 1.0;
					}
					if (self is GameClient)
					{
						int nOcc = Global.CalcOriginalOccupationID(self as GameClient);
						int attackType = nOcc;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, attackPercent, 0, 0, skillLevel, 0.0, 0.0, ignoreDefenseAndDodge, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, attackPercent, 0, 0, skillLevel, 0.0, 0.0, ignoreDefenseAndDodge, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					else
					{
						int attackType = (self as Monster).MonsterInfo.AttackType;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, attackPercent, 0, 0, skillLevel, 0.0, 0.0, ignoreDefenseAndDodge, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, attackPercent, 0, 0, skillLevel, 0.0, 0.0, ignoreDefenseAndDodge, 1.0, 0, 0, 0, 0.0);
						}
						else if (!(obj is BiaoCheItem))
						{
							if (obj is JunQiItem)
							{
								JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, attackType, false, 0, attackPercent, 0, 0, 1.0, 0, 0);
							}
						}
					}
					break;
				}
				case MagicActionIDs.PUSH_STRAIGHT:
					if (self is GameClient)
					{
						direction = ((direction < 0) ? (self as GameClient).ClientData.RoleDirection : direction);
						int nOcc = Global.CalcOriginalOccupationID(self as GameClient);
						int attackType = nOcc;
						int moveNum = 2 + skillLevel;
						int maxMoveNum = moveNum;
						Point clientGrid = (self as GameClient).CurrentGrid;
						List<Point> selfPoints = Global.GetGridPointByDirection(direction, (int)clientGrid.X, (int)clientGrid.Y, moveNum);
						double addInjured = actionParams[skillLevel];
						byte holdBitSet = 0;
						holdBitSet |= 1;
						holdBitSet |= 2;
						for (int i = 0; i < selfPoints.Count; i++)
						{
							if (Global.InObsByGridXY((self as GameClient).ObjectType, (self as GameClient).ClientData.MapCode, (int)selfPoints[i].X, (int)selfPoints[i].Y, 0, holdBitSet))
							{
								break;
							}
							moveNum--;
						}
						if (moveNum < maxMoveNum)
						{
							clientGrid = selfPoints[maxMoveNum - moveNum - 1];
						}
						Point canMovePoint = clientGrid;
						if (!Global.CanQueueMoveObject(self as GameClient, direction, (int)clientGrid.X, (int)clientGrid.Y, 20, moveNum, holdBitSet, out canMovePoint, false))
						{
							GameMap gameMap = GameManager.MapMgr.DictMaps[(self as GameClient).ClientData.MapCode];
							Point clientMoveTo = new Point(canMovePoint.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), canMovePoint.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
							GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)clientMoveTo.X, (int)clientMoveTo.Y, (self as GameClient).ClientData.RoleDirection, 159, 3);
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, self as GameClient, 0, (int)addInjured, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else
						{
							GameMap gameMap = GameManager.MapMgr.DictMaps[(self as GameClient).ClientData.MapCode];
							Point clientMoveTo = new Point(selfPoints[selfPoints.Count - 1].X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), selfPoints[selfPoints.Count - 1].Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
							GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)clientMoveTo.X, (int)clientMoveTo.Y, (self as GameClient).ClientData.RoleDirection, 159, 3);
							if (moveNum > 0)
							{
								Global.QueueMoveObject(self as GameClient, (self as GameClient).ClientData.RoleDirection, (int)clientGrid.X, (int)clientGrid.Y, 20, moveNum, (int)addInjured, holdBitSet, false);
							}
						}
					}
					break;
				case MagicActionIDs.PUSH_CIRCLE:
					if (self is GameClient)
					{
						Point clientGrid = (self as GameClient).CurrentGrid;
						int nOcc = Global.CalcOriginalOccupationID(self as GameClient);
						double addInjured = actionParams[skillLevel];
						GameMap gameMap = GameManager.MapMgr.DictMaps[(self as GameClient).ClientData.MapCode];
						byte holdBitSet = 0;
						holdBitSet |= 1;
						holdBitSet |= 2;
						obj = null;
						for (int nDir = 0; nDir < 8; nDir++)
						{
							Global.QueueMoveObject(self as GameClient, nDir, (int)clientGrid.X, (int)clientGrid.Y, 20, 1, (int)addInjured, holdBitSet, false);
						}
					}
					break;
				case MagicActionIDs.MAGIC_ATTACK:
				case MagicActionIDs.DS_ATTACK:
				case MagicActionIDs.PHY_ATTACK:
				{
					direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
					double addMinInjure = actionParams[skillLevel * 2];
					double addMaxInjure = actionParams[skillLevel * 2 + 1];
					double addInjure = (double)Global.GetRandomNumber((int)addMinInjure, (int)addMaxInjure + 1);
					if (self is GameClient)
					{
						int nOcc = Global.CalcOriginalOccupationID(self as GameClient);
						int attackType = nOcc;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, (int)addInjure, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, (int)addInjure, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					else
					{
						int attackType = (self as Monster).MonsterInfo.AttackType;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, (int)addInjure, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, (int)addInjure, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (!(obj is BiaoCheItem))
						{
							if (obj is JunQiItem)
							{
							}
						}
					}
					break;
				}
				case MagicActionIDs.RANDOM_MOVE:
					if (self is GameClient)
					{
						double noMovePercent = actionParams[skillLevel];
						int percent = (int)(noMovePercent * 100.0);
						if (Global.GetRandomNumber(0, 101) >= percent)
						{
							if (Global.GetRandomNumber(0, 101) >= 10)
							{
								Point p = Global.GetRandomPoint(ObjectTypes.OT_CLIENT, (self as GameClient).ClientData.MapCode);
								if (!Global.InObs(ObjectTypes.OT_CLIENT, (self as GameClient).ClientData.MapCode, (int)p.X, (int)p.Y, 0, 0))
								{
									List<object> objsList = Global.GetAll9Clients(self as GameClient);
									GameManager.ClientMgr.NotifyOthersLeave(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, objsList);
									GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)p.X, (int)p.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
								}
							}
							else
							{
								int toMapCode = GameManager.MainMapCode;
								int toPosX = -1;
								int toPosY = -1;
								GameMap gameMap = null;
								if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, toMapCode, toPosX, toPosY, -1, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.FIRE_WALL:
					if (self is GameClient)
					{
						double attackPercent = actionParams[2 + skillLevel];
						double[] newParams = new double[]
						{
							actionParams[0],
							actionParams[0] / actionParams[1],
							attackPercent,
							(double)(self as GameClient).ClientData.RoleID
						};
						GameMap gameMap = GameManager.MapMgr.DictMaps[(self as GameClient).ClientData.MapCode];
						int gridX = targetX / gameMap.MapGridWidth;
						int gridY = targetY / gameMap.MapGridHeight;
						if (gridX > 0 && gridY > 0)
						{
							GameManager.GridMagicHelperMgr.AddMagicHelper(MagicActionIDs.FIRE_WALL, newParams, (self as GameClient).ClientData.MapCode, new Point((double)gridX, (double)gridY), 1, 1, self.CurrentCopyMapID);
						}
					}
					break;
				case MagicActionIDs.FIRE_CIRCLE:
					if (self is GameClient)
					{
						direction = ((direction < 0) ? (self as GameClient).ClientData.RoleDirection : direction);
						int nOcc = Global.CalcOriginalOccupationID(self as GameClient);
						int attackType = nOcc;
						double attackPercent = actionParams[skillLevel];
						if (!(obj is GameClient))
						{
							if (obj is Monster)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, attackPercent, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
							}
							else if (obj is BiaoCheItem)
							{
								BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
							else if (obj is JunQiItem)
							{
								JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
							else if (obj is FakeRoleItem)
							{
								FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
						}
					}
					break;
				case MagicActionIDs.NEW_MAGIC_SUBINJURE:
					if (self is GameClient)
					{
						direction = ((direction < 0) ? (self as GameClient).ClientData.RoleDirection : direction);
						int nOcc = Global.CalcOriginalOccupationID(self as GameClient);
						double secs = actionParams[skillLevel * 2];
						double subPercent = actionParams[skillLevel * 2 + 1];
						int injure = 0;
						int burst = 0;
						if (obj is GameClient)
						{
							RoleAlgorithm.MAttackEnemy(self as GameClient, obj as GameClient, false, 1.0, 0, 1.0, 1, 0, out burst, out injure, true, 0.0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							RoleAlgorithm.MAttackEnemy(self as GameClient, obj as Monster, false, 1.0, 0, 1.0, 1, 0, out burst, out injure, true, 0.0, 0, 0, 0.0);
						}
						secs += (double)injure;
						double[] newActionParams = new double[]
						{
							subPercent,
							secs
						};
						(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_MAGIC_SUBINJURE, newActionParams, -1);
						(self as GameClient).ClientData.FSHuDunStart = TimeUtil.NOW();
						(self as GameClient).ClientData.FSHuDunSeconds = (int)secs;
						GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, 0, (self as GameClient).ClientData.FSHuDunStart, (self as GameClient).ClientData.FSHuDunSeconds, 0.0);
						double[] newParams = new double[]
						{
							secs
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.FSAddHuDunNoShow, newParams, 1, true);
					}
					break;
				case MagicActionIDs.DS_ADDLIFE:
					if (self is GameClient)
					{
						double totalSecs = actionParams[0];
						double timeSlotSecs = actionParams[1];
						double addLiefV = actionParams[2 + skillLevel];
						if (obj is GameClient)
						{
							double[] newParams = new double[]
							{
								totalSecs,
								timeSlotSecs,
								addLiefV
							};
							Global.UpdateBufferData(obj as GameClient, BufferItemTypes.DSTimeAddLifeNoShow, newParams, 1, true);
						}
						else if (obj is Monster)
						{
							double[] newParams = new double[]
							{
								totalSecs,
								timeSlotSecs,
								addLiefV
							};
							Global.UpdateMonsterBufferData(obj as Monster, BufferItemTypes.DSTimeAddLifeNoShow, newParams);
						}
					}
					break;
				case MagicActionIDs.DS_CALL_GUARD:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						int monsterID = (int)actionParams[0];
						int seconds = (int)actionParams[1];
						Monster monster = Global.GetPetMonsterByMonsterByType(client, MonsterTypes.DSPetMonster);
						if (monster != null && monster.Alive && monster.MonsterInfo.ExtensionID == monsterID)
						{
							Global.RecalcDSMonsterProps(client, monster, skillLevel, seconds);
							Point clientPos = client.CurrentPos;
							GameManager.MonsterMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, (int)clientPos.X, (int)clientPos.Y, (int)monster.Direction, 159, 0);
						}
						else
						{
							Global.SystemKillSummonMonster(client, MonsterTypes.DSPetMonster);
							GameManager.LuaMgr.CallMonstersForGameClient(client, monsterID, skillLevel, seconds, 1001, 1);
						}
					}
					else if (self is Monster)
					{
						Monster owner = self as Monster;
						int monsterID = (int)actionParams[0];
						int seconds = (int)actionParams[1];
						Monster monster = owner.CallMonster;
						if (monster != null && monster.Alive && monster.MonsterInfo.ExtensionID == monsterID)
						{
							Global.RecalcDSMonsterProps(owner, monster, skillLevel, seconds);
							Point clientPos = owner.CurrentPos;
							GameManager.MonsterMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, (int)clientPos.X, (int)clientPos.Y, (int)monster.Direction, 159, 0);
						}
						else
						{
							Global.SystemKillMonster(monster);
							GameManager.LuaMgr.CallMonstersForMonster(owner, monsterID, skillLevel, seconds, 1001, 1);
						}
					}
					break;
				case MagicActionIDs.TIME_DS_ADD_DEFENSE:
					if (self is GameClient)
					{
					}
					break;
				case MagicActionIDs.TIME_DS_ADD_MDEFENSE:
					if (self is GameClient)
					{
					}
					break;
				case MagicActionIDs.TIME_DS_SUB_DEFENSE:
					if (self is GameClient)
					{
						direction = ((direction < 0) ? (self as GameClient).ClientData.RoleDirection : direction);
						int nOcc = Global.CalcOriginalOccupationID(self as GameClient);
						long ticks = TimeUtil.NOW() * 10000L + (long)actionParams[skillLevel * 2] * 1000L * 10000L;
						double defenseValue = actionParams[skillLevel * 2 + 1];
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(3, -defenseValue, ticks);
							(obj as GameClient).RoleBuffer.AddTempExtProp(4, -defenseValue, ticks);
							(obj as GameClient).RoleBuffer.AddTempExtProp(5, -defenseValue, ticks);
							(obj as GameClient).RoleBuffer.AddTempExtProp(6, -defenseValue, ticks);
						}
						else if (obj is Monster)
						{
						}
					}
					break;
				case MagicActionIDs.INSTANT_ATTACK:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, actionParams[0] / 100.0, 0, false, 0, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, actionParams[0] / 100.0, 0, false, 0, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, actionParams[0] / 100.0, 0, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, actionParams[0] / 100.0, 0, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, actionParams[0] / 100.0, 0, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.INSTANT_MAGIC:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, actionParams[0] / 100.0, 1, false, 0, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, actionParams[0] / 100.0, 1, false, 0, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, actionParams[0] / 100.0, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, actionParams[0] / 100.0, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, actionParams[0] / 100.0, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.INSTANT_ATTACK1:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.INSTANT_MAGIC1:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.INSTANT_ATTACK2LIFE:
					if (self is GameClient)
					{
						int injured = 0;
						if (obj is GameClient)
						{
							injured = GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							injured = GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						if (injured > 0 && (self as GameClient).ClientData.CurrentLifeV > 0)
						{
							double addLife = (double)injured * (actionParams[1] / 100.0);
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, addLife, "击中恢复， 脚本" + id.ToString());
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.INSTANT_MAGIC2LIFE:
					if (self is GameClient)
					{
						int injured = 0;
						if (obj is GameClient)
						{
							injured = GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							injured = GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						if (injured > 0 && (self as GameClient).ClientData.CurrentLifeV > 0)
						{
							double addLife = (double)injured * (actionParams[1] / 100.0);
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, addLife, "击中恢复， 脚本" + id.ToString());
						}
					}
					break;
				case MagicActionIDs.TIME_ATTACK:
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_ATTACK, actionParams, obj.GetObjectID());
					break;
				case MagicActionIDs.TIME_MAGIC:
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_MAGIC, actionParams, obj.GetObjectID());
					break;
				case MagicActionIDs.FOREVER_ADDATTACK:
					(obj as GameClient).RoleBuffer.AddForeverExtProp(7, actionParams[0]);
					(obj as GameClient).RoleBuffer.AddForeverExtProp(8, actionParams[0]);
					break;
				case MagicActionIDs.FOREVER_ADDMAGICATTACK:
					(obj as GameClient).RoleBuffer.AddForeverExtProp(9, actionParams[0]);
					(obj as GameClient).RoleBuffer.AddForeverExtProp(10, actionParams[0]);
					break;
				case MagicActionIDs.TIME_ADDATTACK:
				{
					long ticks = TimeUtil.NOW() * 10000L + (long)actionParams[1] * 1000L * 10000L;
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(7, actionParams[0] / 100.0, ticks);
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(8, actionParams[0] / 100.0, ticks);
					break;
				}
				case MagicActionIDs.TIME_SUBATTACK:
				{
					long ticks = TimeUtil.NOW() * 10000L + (long)actionParams[1] * 1000L * 10000L;
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(7, -(actionParams[0] / 100.0), ticks);
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(8, -(actionParams[0] / 100.0), ticks);
					break;
				}
				case MagicActionIDs.TIME_ADDMAGIC:
				{
					long ticks = TimeUtil.NOW() * 10000L + (long)actionParams[1] * 1000L * 10000L;
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(9, actionParams[0] / 100.0, ticks);
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(10, actionParams[0] / 100.0, ticks);
					break;
				}
				case MagicActionIDs.TIME_SUBMAGIC:
				{
					long ticks = TimeUtil.NOW() * 10000L + (long)actionParams[1] * 1000L * 10000L;
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(9, -(actionParams[0] / 100.0), ticks);
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(10, -(actionParams[0] / 100.0), ticks);
					break;
				}
				case MagicActionIDs.INSTANT_ADDLIFE1:
					if (obj is GameClient)
					{
						double value = actionParams[0] * (1.0 + RoleAlgorithm.GetPotionPercentV(obj as GameClient));
						GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, value, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
					}
					break;
				case MagicActionIDs.INSTANT_ADDMAGIC1:
					if (obj is GameClient)
					{
						if (obj is GameClient)
						{
							double value = actionParams[0] * (1.0 + RoleAlgorithm.GetPotionPercentV(obj as GameClient));
							GameManager.ClientMgr.AddSpriteMagicV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, value, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
						}
					}
					break;
				case MagicActionIDs.INSTANT_ADDLIFE2:
					if (obj is GameClient)
					{
						double val = actionParams[0] / 100.0 * (double)(obj as GameClient).ClientData.LifeV;
						GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, val, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
					}
					break;
				case MagicActionIDs.INSTANT_ADDMAGIC2:
					if (obj is GameClient)
					{
						double val = actionParams[0] / 100.0 * (double)(obj as GameClient).ClientData.MagicV;
						GameManager.ClientMgr.AddSpriteMagicV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, val, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
					}
					break;
				case MagicActionIDs.INSTANT_ADDLIFE3:
					if (obj is GameClient)
					{
						double val = (double)usedMaigcV + actionParams[0];
						GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, val, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
					}
					break;
				case MagicActionIDs.INSTANT_ADDLIFE4:
					if (obj is GameClient)
					{
						double val = (double)usedMaigcV * (actionParams[0] / 100.0);
						GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, val, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
					}
					break;
				case MagicActionIDs.INSTANT_COOLDOWN:
					if (self is GameClient)
					{
						GameManager.ClientMgr.RemoveCoolDown(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, 0, (int)actionParams[0]);
					}
					break;
				case MagicActionIDs.TIME_SUBLIFE:
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_SUBLIFE, actionParams, obj.GetObjectID());
					break;
				case MagicActionIDs.TIME_ADDLIFE:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_ADDLIFE, actionParams, -1);
					break;
				case MagicActionIDs.TIME_SLOW:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_SLOW, actionParams, -1);
					if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOthersMyAction(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (obj as GameClient).ClientData.RoleID, (obj as GameClient).ClientData.MapCode, (obj as GameClient).ClientData.RoleDirection, 0, (obj as GameClient).ClientData.PosX, (obj as GameClient).ClientData.PosY, -1, -1, -1, 0, 0, 114);
					}
					break;
				case MagicActionIDs.TIME_ADDDODGE:
				{
					long ticks = TimeUtil.NOW() * 10000L + (long)actionParams[1] * 1000L * 10000L;
					(obj as GameClient).RoleBuffer.AddTempExtProp(19, actionParams[0] / 100.0, ticks);
					break;
				}
				case MagicActionIDs.TIME_FREEZE:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_FREEZE, actionParams, -1);
					if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOthersMyAction(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (obj as GameClient).ClientData.RoleID, (obj as GameClient).ClientData.MapCode, (obj as GameClient).ClientData.RoleDirection, 0, (obj as GameClient).ClientData.PosX, (obj as GameClient).ClientData.PosY, -1, -1, -1, 0, 0, 114);
					}
					break;
				case MagicActionIDs.TIME_INJUE2LIFE:
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_INJUE2LIFE, actionParams, -1);
					break;
				case MagicActionIDs.INSTANT_BURSTATTACK:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							double percent2 = (double)(obj as GameClient).ClientData.CurrentLifeV / (double)(obj as GameClient).ClientData.LifeV;
							double percent3 = actionParams[1] / 100.0;
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, actionParams[0] / 100.0, 0, percent2 <= percent3, 0, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							double percent2 = (obj as Monster).VLife / (obj as Monster).MonsterInfo.VLifeMax;
							double percent3 = actionParams[1] / 100.0;
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, actionParams[0] / 100.0, 0, percent2 <= percent3, 0, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							double percent2 = (double)(obj as BiaoCheItem).CutLifeV / (double)(obj as BiaoCheItem).LifeV;
							double percent3 = actionParams[1] / 100.0;
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, actionParams[0] / 100.0, 0, percent2 <= percent3, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							double percent2 = (double)(obj as JunQiItem).CutLifeV / (double)(obj as JunQiItem).LifeV;
							double percent3 = actionParams[1] / 100.0;
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, actionParams[0] / 100.0, 0, percent2 <= percent3, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							double percent2 = (double)(obj as FakeRoleItem).CurrentLifeV / (double)(obj as FakeRoleItem).MyRoleDataMini.MaxLifeV;
							double percent3 = actionParams[1] / 100.0;
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, actionParams[0] / 100.0, 0, percent2 <= percent3, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.FOREVER_ADDDRUGEFFECT:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.FOREVER_ADDDRUGEFFECT, actionParams, -1);
					break;
				case MagicActionIDs.INSTANT_REMOVESLOW:
					(obj as GameClient).RoleMagicHelper.RemoveMagicHelper(MagicActionIDs.TIME_SLOW);
					break;
				case MagicActionIDs.TIME_SUBINJUE:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_SUBINJUE, actionParams, -1);
					break;
				case MagicActionIDs.TIME_ADDINJUE:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_ADDINJUE, actionParams, -1);
					break;
				case MagicActionIDs.TIME_SUBINJUE1:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_SUBINJUE1, actionParams, -1);
					break;
				case MagicActionIDs.TIME_ADDINJUE1:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_ADDINJUE1, actionParams, -1);
					break;
				case MagicActionIDs.TIME_DELAYATTACK:
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_DELAYATTACK, actionParams, obj.GetObjectID());
					break;
				case MagicActionIDs.TIME_DELAYMAGIC:
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_DELAYMAGIC, actionParams, obj.GetObjectID());
					break;
				case MagicActionIDs.FOREVER_ADDDODGE:
					(obj as GameClient).RoleBuffer.AddForeverExtProp(19, actionParams[0] / 100.0);
					break;
				case MagicActionIDs.TIME_INJUE2MAGIC:
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_INJUE2MAGIC, actionParams, -1);
					break;
				case MagicActionIDs.FOREVER_ADDMAGICV:
					(obj as GameClient).RoleBuffer.AddForeverExtProp(15, actionParams[0]);
					break;
				case MagicActionIDs.FOREVER_ADDLIFE:
					(obj as GameClient).RoleBuffer.AddForeverExtProp(13, actionParams[0]);
					break;
				case MagicActionIDs.INSTANT_MOVE:
					if (self is GameClient)
					{
						if (self != obj)
						{
							if (obj is GameClient)
							{
								Point selfPos = new Point((double)(self as GameClient).ClientData.PosX, (double)(self as GameClient).ClientData.PosY);
								Point targetPos = new Point((double)(obj as GameClient).ClientData.PosX, (double)(obj as GameClient).ClientData.PosY);
								if (selfPos.X != targetPos.X || selfPos.Y != targetPos.Y)
								{
									targetPos = Global.GetExtensionPointByObs(self as GameClient, targetPos, selfPos, Data.MinAttackDistance);
									GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)targetPos.X, (int)targetPos.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
								}
							}
							else if (obj is Monster)
							{
								Point selfPos = new Point((double)(self as GameClient).ClientData.PosX, (double)(self as GameClient).ClientData.PosY);
								Point targetPos = new Point((obj as Monster).SafeCoordinate.X, (obj as Monster).SafeCoordinate.Y);
								if (selfPos.X != targetPos.X || selfPos.Y != targetPos.Y)
								{
									targetPos = Global.GetExtensionPointByObs(self as GameClient, targetPos, selfPos, Data.MinAttackDistance);
									GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)targetPos.X, (int)targetPos.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
								}
							}
							else if (obj is BiaoCheItem)
							{
								Point selfPos = new Point((double)(self as GameClient).ClientData.PosX, (double)(self as GameClient).ClientData.PosY);
								Point targetPos = new Point((double)(obj as BiaoCheItem).PosX, (double)(obj as BiaoCheItem).PosY);
								if (selfPos.X != targetPos.X || selfPos.Y != targetPos.Y)
								{
									targetPos = Global.GetExtensionPointByObs(self as GameClient, targetPos, selfPos, Data.MinAttackDistance);
									GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)targetPos.X, (int)targetPos.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
								}
							}
							else if (obj is JunQiItem)
							{
								Point selfPos = new Point((double)(self as GameClient).ClientData.PosX, (double)(self as GameClient).ClientData.PosY);
								Point targetPos = new Point((double)(obj as JunQiItem).PosX, (double)(obj as JunQiItem).PosY);
								if (selfPos.X != targetPos.X || selfPos.Y != targetPos.Y)
								{
									targetPos = Global.GetExtensionPointByObs(self as GameClient, targetPos, selfPos, Data.MinAttackDistance);
									GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)targetPos.X, (int)targetPos.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
								}
							}
							else if (obj is FakeRoleItem)
							{
								Point selfPos = new Point((double)(self as GameClient).ClientData.PosX, (double)(self as GameClient).ClientData.PosY);
								Point targetPos = new Point((double)(obj as FakeRoleItem).MyRoleDataMini.PosX, (double)(obj as FakeRoleItem).MyRoleDataMini.PosY);
								if (selfPos.X != targetPos.X || selfPos.Y != targetPos.Y)
								{
									targetPos = Global.GetExtensionPointByObs(self as GameClient, targetPos, selfPos, Data.MinAttackDistance);
									GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)targetPos.X, (int)targetPos.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
								}
							}
						}
						else if (targetX != -1 && targetY != -1)
						{
							Point selfPos = new Point((double)(self as GameClient).ClientData.PosX, (double)(self as GameClient).ClientData.PosY);
							Point targetPos = new Point((double)targetX, (double)targetY);
							if (selfPos.X != targetPos.X || selfPos.Y != targetPos.Y)
							{
								targetPos = Global.GetExtensionPointByObs(self as GameClient, targetPos, selfPos, Data.MinAttackDistance);
								GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)targetPos.X, (int)targetPos.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
							}
						}
					}
					break;
				case MagicActionIDs.TIME_ADDMAGIC1:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_ADDMAGIC1, actionParams, -1);
					break;
				case MagicActionIDs.GOTO_MAP:
					if (self is GameClient)
					{
						int toMapCode = (int)actionParams[0];
						GameManager.LuaMgr.GotoMap(self as GameClient, toMapCode, -1, -1, -1);
					}
					break;
				case MagicActionIDs.INSTANT_MAP_POS:
				{
					Point p = Global.GetRandomPoint(ObjectTypes.OT_CLIENT, (self as GameClient).ClientData.MapCode);
					if (!Global.InObs(ObjectTypes.OT_CLIENT, (self as GameClient).ClientData.MapCode, (int)p.X, (int)p.Y, 0, 0))
					{
						List<object> objsList = Global.GetAll9Clients(self as GameClient);
						GameManager.ClientMgr.NotifyOthersLeave(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, objsList);
						GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)p.X, (int)p.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
					}
					break;
				}
				case MagicActionIDs.GOTO_LAST_MAP:
				{
					SceneUIClasses sceneType = Global.GetMapSceneType((self as GameClient).ClientData.MapCode);
					PreGotoLastMapEventObject eventObjectEx = new PreGotoLastMapEventObject(self as GameClient, (int)sceneType);
					GlobalEventSource4Scene.getInstance().fireEvent(eventObjectEx, eventObjectEx.SceneType);
					if (!eventObjectEx.Handled || eventObjectEx.Result)
					{
						if (Global.GotoLastMap(self as GameClient, 1))
						{
							if (self is GameClient)
							{
							}
						}
					}
					break;
				}
				case MagicActionIDs.ADD_HORSE:
				{
					int horseID = (int)actionParams[0];
					Global.AddHorseDBCommand(Global._TCPManager.TcpOutPacketPool, self as GameClient, horseID, 1);
					break;
				}
				case MagicActionIDs.ADD_PET:
				{
					int petID = (int)actionParams[0];
					SystemXmlItem systemPet = null;
					if (GameManager.systemPets.SystemXmlItemDict.TryGetValue(petID, out systemPet))
					{
						string petName = systemPet.GetStringValue("Name");
						int petType = 0;
						Global.AddPetDBCommand(Global._TCPManager.TcpOutPacketPool, self as GameClient, petID, petName, petType, "");
					}
					break;
				}
				case MagicActionIDs.ADD_PET_GRID:
				{
					int extGridNum = (int)actionParams[0];
					Global.ExtGridPortableBagDBCommand(Global._TCPManager.TcpOutPacketPool, self as GameClient, extGridNum);
					break;
				}
				case MagicActionIDs.ADD_SKILL:
				{
					int skillID = (int)actionParams[0];
					skillLevel = (int)actionParams[1];
					skillLevel = Global.GMax(1, skillLevel);
					skillLevel = Global.GMin(3, skillLevel);
					if (null == Global.GetSkillDataByID(self as GameClient, skillID))
					{
						SystemXmlItem systemMagic = null;
						if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(skillID, out systemMagic))
						{
							if (Global.MU_GetUpSkillLearnCondition(self as GameClient, skillID, systemMagic))
							{
								int nOcc = Global.CalcOriginalOccupationID(self as GameClient);
								if (nOcc == systemMagic.GetIntValue("ToOcuupation", -1))
								{
									Global.AddSkillDBCommand(Global._TCPManager.TcpOutPacketPool, self as GameClient, skillID, skillLevel);
									string skillName = Global.GetSkillNameByID(skillID);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(362, new object[0]), new object[]
									{
										skillName
									}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox, 0);
								}
							}
						}
					}
					break;
				}
				case MagicActionIDs.NEW_INSTANT_ATTACK:
					if (self is GameClient)
					{
						double attackVPercent = (double)((int)actionParams[0]);
						double attackVPerLevel = (double)((int)actionParams[1]);
						double addAttackV = (double)skillLevel * attackVPerLevel;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, attackVPercent, 0, false, (int)addAttackV, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, attackVPercent, 0, false, (int)addAttackV, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, attackVPercent, 0, false, (int)addAttackV, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, attackVPercent, 0, false, (int)addAttackV, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, attackVPercent, 0, false, (int)addAttackV, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.NEW_INSTANT_MAGIC:
					if (self is GameClient)
					{
						double attackVPercent = (double)((int)actionParams[0]);
						double attackVPerLevel = (double)((int)actionParams[1]);
						double addAttackV = (double)skillLevel * attackVPerLevel;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, attackVPercent, 1, false, (int)addAttackV, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, attackVPercent, 1, false, (int)addAttackV, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, attackVPercent, 1, false, (int)addAttackV, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, attackVPercent, 1, false, (int)addAttackV, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, attackVPercent, 1, false, (int)addAttackV, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.NEW_FOREVER_ADDATTACK:
				{
					double valuePerLevel = (double)((int)actionParams[0]);
					double addValue = (double)skillLevel * valuePerLevel;
					(obj as GameClient).RoleBuffer.AddForeverExtProp(7, addValue);
					(obj as GameClient).RoleBuffer.AddForeverExtProp(8, addValue);
					break;
				}
				case MagicActionIDs.NEW_FOREVER_ADDMAGICATTACK:
				{
					double valuePerLevel = (double)((int)actionParams[0]);
					double addValue = (double)skillLevel * valuePerLevel;
					(obj as GameClient).RoleBuffer.AddForeverExtProp(9, addValue);
					(obj as GameClient).RoleBuffer.AddForeverExtProp(10, addValue);
					break;
				}
				case MagicActionIDs.NEW_FOREVER_ADDHIT:
				{
					double valuePerLevel = (double)((int)actionParams[0]);
					double addValue = (double)skillLevel * valuePerLevel;
					(obj as GameClient).RoleBuffer.AddForeverExtProp(18, addValue);
					break;
				}
				case MagicActionIDs.NEW_FOREVER_ADDDODGE:
				{
					double valuePerLevel = (double)((int)actionParams[0]);
					double addValue = (double)skillLevel * valuePerLevel;
					(obj as GameClient).RoleBuffer.AddForeverExtProp(19, addValue);
					break;
				}
				case MagicActionIDs.NEW_FOREVER_ADDMAGICV:
				{
					double valuePerLevel = (double)((int)actionParams[0]);
					double addValue = (double)skillLevel * valuePerLevel;
					(obj as GameClient).RoleBuffer.AddForeverExtProp(15, addValue);
					break;
				}
				case MagicActionIDs.NEW_FOREVER_ADDLIFE:
				{
					double valuePerLevel = (double)((int)actionParams[0]);
					double addValue = (double)skillLevel * valuePerLevel;
					(obj as GameClient).RoleBuffer.AddForeverExtProp(13, addValue);
					break;
				}
				case MagicActionIDs.NEW_TIME_INJUE2MAGIC:
				{
					double valuePerLevel = (double)((int)actionParams[0]);
					double addValue = (double)skillLevel * valuePerLevel;
					double[] newActionParams = new double[]
					{
						addValue,
						actionParams[1]
					};
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_TIME_INJUE2MAGIC, newActionParams, -1);
					break;
				}
				case MagicActionIDs.NEW_TIME_ATTACK:
				{
					double valuePerLevel = (double)((int)actionParams[0]);
					double addValue = (double)skillLevel * valuePerLevel;
					double[] newActionParams = new double[]
					{
						addValue,
						actionParams[1],
						actionParams[2]
					};
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_TIME_ATTACK, newActionParams, obj.GetObjectID());
					break;
				}
				case MagicActionIDs.NEW_TIME_MAGIC:
				{
					double valuePerLevel = (double)((int)actionParams[0]);
					double addValue = (double)skillLevel * valuePerLevel;
					double[] newActionParams = new double[]
					{
						addValue,
						actionParams[1],
						actionParams[2]
					};
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_TIME_MAGIC, newActionParams, obj.GetObjectID());
					break;
				}
				case MagicActionIDs.NEW_INSTANT_ADDLIFE:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							int injure = 0;
							int burst = 0;
							RoleAlgorithm.MAttackEnemy(self as GameClient, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out injure, false, 0.0, 0, 0, 0.0);
							double valuePerLevel = (double)((int)actionParams[0]);
							double addValue = (double)skillLevel * valuePerLevel + (double)injure;
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, addValue, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
						}
					}
					break;
				case MagicActionIDs.DB_ADD_DBL_EXP:
				{
					Global.RemoveBufferData(self as GameClient, 18);
					Global.RemoveBufferData(self as GameClient, 36);
					Global.RemoveBufferData(self as GameClient, 46);
					double[] newParams = new double[]
					{
						actionParams[0],
						(double)actionGoodsID
					};
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.DblExperience, newParams, 0, true);
					break;
				}
				case MagicActionIDs.DB_ADD_DBL_MONEY:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.DblMoney, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_DBL_LINGLI:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.DblLingLi, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_LIFERESERVE:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.LifeVReserve, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_MAGICRESERVE:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.MagicVReserve, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_LINGLIRESERVE:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.LingLiVReserve, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_TEMPATTACK:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.AddTempAttack, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_TEMPDEFENSE:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.AddTempDefense, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_UPLIEFLIMIT:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.UpLifeLimit, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_UPMAGICLIMIT:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.UpMagicLimit, actionParams, 0, true);
					break;
				case MagicActionIDs.NEW_ADD_LINGLI:
					if (obj is GameClient)
					{
						GameManager.ClientMgr.AddInterPower(obj as GameClient, (int)actionParams[0], false, true);
					}
					break;
				case MagicActionIDs.NEW_ADD_EXP:
					if (obj is GameClient)
					{
						GameManager.ClientMgr.ProcessRoleExperience(obj as GameClient, (long)((int)actionParams[0]), false, true, false, "none");
					}
					break;
				case MagicActionIDs.NEW_ADD_DAILYCXNUM:
					if (obj is GameClient)
					{
						int subNum = -(int)actionParams[0];
						Global.UpdateDailyJingMaiData(obj as GameClient, subNum);
					}
					break;
				case MagicActionIDs.GOTO_NEXTMAP:
					if (obj is GameClient)
					{
						Global.ProcessGoToNextFuBenMap(obj as GameClient);
					}
					break;
				case MagicActionIDs.GET_AWARD:
					if (obj is GameClient)
					{
						Global.ProcessFuBenMapGetAward(obj as GameClient, false);
					}
					break;
				case MagicActionIDs.NEW_INSTANT_ADDLIFE2:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							double magicAttackV = (RoleAlgorithm.GetMinMagicAttackV(self as GameClient) + RoleAlgorithm.GetMaxMagicAttackV(self as GameClient)) / 2.0;
							double valuePerLevel = (double)((int)actionParams[0]);
							double addValue = (double)skillLevel * valuePerLevel + magicAttackV * (actionParams[0] / 100.0);
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, addValue, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
						}
					}
					break;
				case MagicActionIDs.NEW_INSTANT_ATTACK3:
					if (self is GameClient)
					{
						double attackVPercent = actionParams[0];
						double attackVPerLevel = actionParams[1];
						double addAttackVPercent = (double)skillLevel * attackVPerLevel;
						attackVPercent += addAttackVPercent;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 0, false, 0, attackVPercent, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 0, false, 0, attackVPercent, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 0, false, 0, attackVPercent, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 0, false, 0, attackVPercent, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 0, false, 0, attackVPercent, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.NEW_INSTANT_MAGIC3:
					if (self is GameClient)
					{
						double attackVPercent = actionParams[0];
						double attackVPerLevel = actionParams[1];
						double addAttackVPercent = (double)skillLevel * attackVPerLevel;
						attackVPercent += addAttackVPercent;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 1, false, 0, attackVPercent, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 1, false, 0, attackVPercent, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, attackVPercent, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, attackVPercent, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, attackVPercent, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.NEW_TIME_ATTACK3:
					if (self is GameClient)
					{
						double valueBase = actionParams[0];
						double valuePerLevel = actionParams[1];
						double addValueVPercent = (double)skillLevel * valuePerLevel;
						valueBase += addValueVPercent;
						int nDamageType = 0;
						int minAttackV = (int)RoleAlgorithm.GetMinAttackV(self as GameClient);
						int maxAttackV = (int)RoleAlgorithm.GetMaxAttackV(self as GameClient);
						int lucky = (int)RoleAlgorithm.GetLuckV(self as GameClient);
						int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(self as GameClient);
						if (obj is GameClient)
						{
							lucky -= (int)RoleAlgorithm.GetDeLuckyAttack(obj as GameClient);
							nFatalValue -= (int)RoleAlgorithm.GetDeFatalAttack(obj as GameClient);
						}
						int attackV = (int)RoleAlgorithm.CalcAttackValue(self as GameClient, minAttackV, maxAttackV, lucky, nFatalValue, out nDamageType, 0.0);
						attackV = (int)((double)attackV * valueBase);
						double[] newActionParams = new double[]
						{
							(double)attackV,
							actionParams[1],
							actionParams[2]
						};
						(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_TIME_ATTACK3, newActionParams, obj.GetObjectID());
					}
					break;
				case MagicActionIDs.NEW_TIME_MAGIC3:
					if (self is GameClient)
					{
						double valueBase = actionParams[0];
						double valuePerLevel = actionParams[1];
						double addValueVPercent = (double)skillLevel * valuePerLevel;
						valueBase += addValueVPercent;
						int nDamageType = 0;
						int minMAttackV = (int)RoleAlgorithm.GetMinMagicAttackV(self as GameClient);
						int maxMAttackV = (int)RoleAlgorithm.GetMaxMagicAttackV(self as GameClient);
						int lucky = (int)RoleAlgorithm.GetLuckV(self as GameClient);
						int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(self as GameClient);
						if (obj is GameClient)
						{
							lucky -= (int)RoleAlgorithm.GetDeLuckyAttack(obj as GameClient);
							nFatalValue -= (int)RoleAlgorithm.GetDeFatalAttack(obj as GameClient);
						}
						int magicAttackV2 = (int)RoleAlgorithm.CalcAttackValue(self as GameClient, minMAttackV, maxMAttackV, lucky, nFatalValue, out nDamageType, 0.0);
						magicAttackV2 = (int)((double)magicAttackV2 * valueBase);
						double[] newActionParams = new double[]
						{
							(double)magicAttackV2,
							actionParams[1],
							actionParams[2]
						};
						(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_TIME_MAGIC3, newActionParams, obj.GetObjectID());
					}
					break;
				case MagicActionIDs.NEW_INSTANT_ADDLIFE3:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							double attackVPercent = actionParams[0];
							double attackVPerLevel = actionParams[1];
							double addAttackVPercent = (double)skillLevel * attackVPerLevel;
							attackVPercent += addAttackVPercent;
							int nDamageType = 0;
							int minMAttackV = (int)RoleAlgorithm.GetMinMagicAttackV(self as GameClient);
							int maxMAttackV = (int)RoleAlgorithm.GetMaxMagicAttackV(self as GameClient);
							int lucky = (int)RoleAlgorithm.GetLuckV(self as GameClient);
							int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(self as GameClient);
							if (obj is GameClient)
							{
								lucky -= (int)RoleAlgorithm.GetDeLuckyAttack(obj as GameClient);
								nFatalValue -= (int)RoleAlgorithm.GetDeFatalAttack(obj as GameClient);
							}
							int magicAttackV2 = (int)RoleAlgorithm.CalcAttackValue(self as GameClient, minMAttackV, maxMAttackV, lucky, nFatalValue, out nDamageType, 0.0);
							magicAttackV2 = (int)((double)magicAttackV2 * attackVPercent);
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (double)magicAttackV2, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
						}
					}
					break;
				case MagicActionIDs.NEW_TIME_INJUE2MAGIC3:
					if (self is GameClient)
					{
						double attackVPercent = actionParams[0];
						double attackVPerLevel = actionParams[1];
						double addAttackVPercent = (double)skillLevel * attackVPerLevel;
						attackVPercent += addAttackVPercent;
						double[] newActionParams = new double[]
						{
							attackVPercent,
							actionParams[2]
						};
						(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_TIME_INJUE2MAGIC3, newActionParams, -1);
						(self as GameClient).ClientData.FSHuDunStart = TimeUtil.NOW();
					}
					break;
				case MagicActionIDs.GOTO_WUXING_MAP:
				{
					int needGoodsID = WuXingMapMgr.GetNeedGoodsIDByNPCID((self as GameClient).ClientData.MapCode, npcID - 2130706432);
					if (-1 != needGoodsID)
					{
						if (Global.GetTotalGoodsCountByID(self as GameClient, needGoodsID) > 0)
						{
							bool usedBinding = false;
							bool usedTimeLimited = false;
							if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, self as GameClient, needGoodsID, 1, false, out usedBinding, out usedTimeLimited, false))
							{
								int gotToMapCode = WuXingMapMgr.GetNextMapCodeByNPCID((self as GameClient).ClientData.MapCode, npcID - 2130706432);
								if (-1 != gotToMapCode)
								{
									GameMap gameMap = null;
									if (GameManager.MapMgr.DictMaps.TryGetValue(gotToMapCode, out gameMap))
									{
										GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, gotToMapCode, -1, -1, -1, 0);
									}
								}
							}
							else
							{
								string goodsName = Global.GetGoodsNameByID(needGoodsID);
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(432, new object[0]), new object[]
								{
									goodsName
								}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
						}
						else
						{
							string goodsName = Global.GetGoodsNameByID(needGoodsID);
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(433, new object[0]), new object[]
							{
								goodsName
							}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
					}
					break;
				}
				case MagicActionIDs.GET_WUXING_AWARD:
					if (obj is GameClient)
					{
						WuXingMapMgr.ProcessWuXingAward(self as GameClient);
					}
					break;
				case MagicActionIDs.LEAVE_LAOFANG:
					if (self is GameClient)
					{
						Global.BroadcastLeaveLaoFangHint(self as GameClient, (self as GameClient).ClientData.MapCode);
						int toMapCode = GameManager.MainMapCode;
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
						{
							GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, toMapCode, -1, -1, -1, 0);
						}
					}
					break;
				case MagicActionIDs.GOTO_CAISHENMIAO:
					if (self is GameClient)
					{
						int fuBenID = (int)actionParams[0];
						int needGoodsID = (int)actionParams[1];
						if (-1 != needGoodsID)
						{
							if (Global.GetTotalGoodsCountByID(self as GameClient, needGoodsID) > 0)
							{
								bool usedBinding = false;
								bool usedTimeLimited = false;
								if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, self as GameClient, needGoodsID, 1, false, out usedBinding, out usedTimeLimited, false))
								{
									Global.EnterCaiShenMiao(self as GameClient, fuBenID, usedBinding ? 1 : 0);
								}
								else
								{
									string goodsName = Global.GetGoodsNameByID(needGoodsID);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(434, new object[0]), new object[]
									{
										goodsName
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
							else
							{
								string goodsName = Global.GetGoodsNameByID(needGoodsID);
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(435, new object[0]), new object[]
								{
									goodsName
								}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 4);
							}
						}
					}
					break;
				case MagicActionIDs.RELOAD_COPYMONSTERS:
					if (self is GameClient)
					{
						if ((self as GameClient).ClientData.CopyMapID > 0)
						{
							int aliveMonsterCount = GameManager.MonsterMgr.GetCopyMapIDMonstersCount((self as GameClient).ClientData.CopyMapID, 0);
							if (aliveMonsterCount <= 0 && !GameManager.MonsterMgr.IsAnyMonsterAliveByCopyMapID((self as GameClient).ClientData.CopyMapID))
							{
								int needGoodsID = (int)actionParams[0];
								if (-1 != needGoodsID)
								{
									if (Global.GetTotalGoodsCountByID(self as GameClient, needGoodsID) > 0)
									{
										bool usedBinding = false;
										bool usedTimeLimited = false;
										if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, self as GameClient, needGoodsID, 1, false, out usedBinding, out usedTimeLimited, false))
										{
											if ((self as GameClient).ClientData.FuBenSeqID > 0)
											{
												FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID((self as GameClient).ClientData.FuBenSeqID);
												if (null != fuBenInfoItem)
												{
													fuBenInfoItem.GoodsBinding = (usedBinding ? 1 : 0);
												}
											}
											CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap((self as GameClient).ClientData.CopyMapID);
											if (null != copyMap)
											{
												copyMap.ClearKilledNormalDict();
												copyMap.ClearKilledBossDict();
											}
											GameManager.MonsterZoneMgr.ReloadCopyMapMonsters((self as GameClient).ClientData.MapCode, (self as GameClient).ClientData.CopyMapID);
										}
										else
										{
											string goodsName = Global.GetGoodsNameByID(needGoodsID);
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(436, new object[0]), new object[]
											{
												goodsName
											}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										}
									}
									else
									{
										string goodsName = Global.GetGoodsNameByID(needGoodsID);
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(437, new object[0]), new object[]
										{
											goodsName
										}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 4);
									}
								}
							}
							else
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(438, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
						}
					}
					break;
				case MagicActionIDs.DB_ADD_MONTHVIP:
				{
					bool isVipBefore = Global.IsVip(self as GameClient);
					actionParams = new double[]
					{
						43200.0,
						1.0
					};
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.MonthVIP, actionParams, 0, true);
					Global.BroadcastVIPMonthHint(self as GameClient, actionGoodsID);
					Global.TryGiveGuMuTimeLimitAwardOnBecomeVip(self as GameClient, isVipBefore);
					break;
				}
				case MagicActionIDs.INSTALL_JUNQI:
					Global.InstallJunQi(self as GameClient, npcID, SceneUIClasses.Normal);
					break;
				case MagicActionIDs.TAKE_SHELIZHIYUAN:
					Global.TakeSheLiZhiYuan(self as GameClient, npcID);
					break;
				case MagicActionIDs.DB_ADD_DBLSKILLUP:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.DblSkillUp, actionParams, 0, true);
					break;
				case MagicActionIDs.NEW_JIUHUA_ADDLIFE:
					if (obj is GameClient)
					{
						GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (double)((obj as GameClient).ClientData.LifeV - (obj as GameClient).ClientData.CurrentLifeV), string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
					}
					break;
				case MagicActionIDs.NEW_LIANZHAN_DELAY:
					if (obj is GameClient)
					{
						BufferData bufferData = Global.GetBufferDataByID(obj as GameClient, 11);
						if (null != bufferData)
						{
							if (!Global.IsBufferDataOver(bufferData, 0L))
							{
								if (1800 == bufferData.BufferSecs)
								{
									bufferData.BufferSecs += 3600;
									Global.UpdateDBBufferData(obj as GameClient, bufferData);
									GameManager.ClientMgr.NotifyBufferData(obj as GameClient, bufferData);
								}
							}
						}
					}
					break;
				case MagicActionIDs.DB_ADD_THREE_EXP:
				{
					Global.RemoveBufferData(self as GameClient, 1);
					Global.RemoveBufferData(self as GameClient, 36);
					Global.RemoveBufferData(self as GameClient, 46);
					double[] newParams = new double[]
					{
						actionParams[0],
						(double)actionGoodsID
					};
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.ThreeExperience, newParams, 0, true);
					break;
				}
				case MagicActionIDs.DB_ADD_THREE_MONEY:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.ThreeMoney, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_AF_PROTECT:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.AutoFightingProtect, actionParams, 0, true);
					break;
				case MagicActionIDs.NEW_INSTANT_ATTACK4:
					if (self is GameClient)
					{
						double attackVPercent = actionParams[0];
						double attackVPerLevel = actionParams[1];
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, skillLevel, attackVPercent, attackVPerLevel, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, skillLevel, attackVPercent, attackVPerLevel, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.NEW_INSTANT_MAGIC4:
					if (self is GameClient)
					{
						double attackVPercent = actionParams[0];
						double attackVPerLevel = actionParams[1];
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, skillLevel, attackVPercent, attackVPerLevel, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, skillLevel, attackVPercent, attackVPerLevel, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.NEW_TIME_MAGIC4:
					if (self is GameClient)
					{
						double valueBase = actionParams[0];
						double valuePerLevel = actionParams[1];
						int injure = 0;
						int burst = 0;
						if (obj is GameClient)
						{
							RoleAlgorithm.MAttackEnemy(self as GameClient, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out injure, false, 0.0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							RoleAlgorithm.MAttackEnemy(self as GameClient, obj as Monster, false, 1.0, 0, 1.0, 0, 0, out burst, out injure, false, 0.0, 0, 0, 0.0);
						}
						if (injure > 0)
						{
							double[] newActionParams = new double[]
							{
								(double)injure,
								actionParams[2],
								actionParams[3]
							};
							(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_TIME_MAGIC4, newActionParams, obj.GetObjectID());
						}
					}
					break;
				case MagicActionIDs.NEW_YINLIANG_RNDBAO:
					if (self is GameClient)
					{
						int minVal = (int)actionParams[0];
						int maxVal = (int)actionParams[1];
						int rndNum = Global.GetRandomNumber(minVal, maxVal);
						GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, self as GameClient, rndNum, "脚本增加金币二", false);
					}
					break;
				case MagicActionIDs.GOTO_LEAVELAOFANG:
					if (self is GameClient)
					{
						if ((self as GameClient).ClientData.PKPoint < Global.MinLeaveJailPKPoints)
						{
							Global.BroadcastLeaveLaoFangHint(self as GameClient, (self as GameClient).ClientData.MapCode);
							Global.ForceTakeOutLaoFangMap(self as GameClient, (self as GameClient).ClientData.PKPoint);
						}
						else
						{
							int needGoodsID = (int)actionParams[0];
							if (-1 != needGoodsID)
							{
								int needGoodsNum = (int)Math.Round(Math.Pow((double)Math.Max((self as GameClient).ClientData.PKValue, 1), 1.5));
								if (Global.GetTotalGoodsCountByID(self as GameClient, needGoodsID) >= needGoodsNum)
								{
									bool usedBinding = false;
									bool usedTimeLimited = false;
									if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, self as GameClient, needGoodsID, needGoodsNum, false, out usedBinding, out usedTimeLimited, false))
									{
										Global.BroadcastLeaveLaoFangHint2(self as GameClient, (self as GameClient).ClientData.MapCode);
										Global.ForceTakeOutLaoFangMap(self as GameClient, (self as GameClient).ClientData.PKPoint);
									}
									else
									{
										string goodsName = Global.GetGoodsNameByID(needGoodsID);
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(439, new object[0]), new object[]
										{
											goodsName
										}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
									}
								}
								else
								{
									string goodsName = Global.GetGoodsNameByID(needGoodsID);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(440, new object[0]), new object[]
									{
										needGoodsNum,
										goodsName
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.GOTO_MAPBYGOODS:
					if (self is GameClient)
					{
						if (JunQiManager.GetLingDiIDBy2MapCode((self as GameClient).ClientData.MapCode) == 2)
						{
							HuangChengManager.HandleLeaveMapHuangDiRoleChanging(self as GameClient);
						}
						int toMapCode = (int)actionParams[0];
						int needGoodsID = (int)actionParams[1];
						int needGoodsNum = (int)actionParams[2];
						if (toMapCode > 0)
						{
							if (-1 != needGoodsID)
							{
								if (Global.GetTotalGoodsCountByID(self as GameClient, needGoodsID) >= needGoodsNum)
								{
									bool usedBinding = false;
									bool usedTimeLimited = false;
									if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, self as GameClient, needGoodsID, needGoodsNum, false, out usedBinding, out usedTimeLimited, false))
									{
										GameMap gameMap = null;
										if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
										{
											GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, toMapCode, -1, -1, -1, 0);
										}
									}
									else
									{
										string goodsName = Global.GetGoodsNameByID(needGoodsID);
										string mapName = Global.GetMapName(toMapCode);
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(441, new object[0]), new object[]
										{
											mapName,
											goodsName
										}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
									}
								}
								else
								{
									string goodsName = Global.GetGoodsNameByID(needGoodsID);
									string mapName = Global.GetMapName(toMapCode);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(442, new object[0]), new object[]
									{
										mapName,
										needGoodsNum,
										goodsName
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.SUB_ZUIEZHI:
					if (self is GameClient)
					{
						int subPKValue = (int)actionParams[0];
						subPKValue = Global.GMax(0, subPKValue);
						subPKValue = Global.GMax((self as GameClient).ClientData.PKValue - subPKValue, 0);
						GameManager.ClientMgr.SetRolePKValuePoint(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, subPKValue, (self as GameClient).ClientData.PKPoint, true);
					}
					break;
				case MagicActionIDs.UN_PACK:
					if (self is GameClient)
					{
						int goodsID = (int)actionParams[0];
						int goodsNum = (int)actionParams[1];
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, self as GameClient, goodsID, goodsNum, 0, "", 0, binding, 0, "", true, 1, "解开简单物品获取", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
					}
					break;
				case MagicActionIDs.GOTO_MAPBYVIP:
					if (self is GameClient)
					{
						int vipLevel = 1;
						if (actionParams.Length > 1)
						{
							vipLevel = (int)actionParams[1];
						}
						if (Global.GetVipType(self as GameClient) < vipLevel)
						{
							GameManager.LuaMgr.Error(self as GameClient, GLang.GetLang(443, new object[0]), 0);
						}
						else
						{
							if (JunQiManager.GetLingDiIDBy2MapCode((self as GameClient).ClientData.MapCode) == 2)
							{
								HuangChengManager.HandleLeaveMapHuangDiRoleChanging(self as GameClient);
							}
							int toMapCode = (int)actionParams[0];
							if (toMapCode > 0)
							{
								if (DBRoleBufferManager.ProcessMonthVIP(self as GameClient) > 0.0)
								{
									GameMap gameMap = null;
									if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
									{
										GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, toMapCode, -1, -1, -1, 0);
									}
								}
								else
								{
									string mapName = Global.GetMapName(toMapCode);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(444, new object[0]), new object[]
									{
										mapName
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.GOTO_BATTLEMAP:
					if (self is GameClient)
					{
						Global.ClientEnterBattle(self as GameClient);
					}
					break;
				case MagicActionIDs.FALL_BAOXIANG2:
				{
					int nOcc = Global.CalcOriginalOccupationID(self as GameClient);
					int fallID = (int)actionParams[nOcc];
					GoodsBaoXiang.ProcessFallBaoXiang(self as GameClient, fallID, (int)actionParams[3], binding, actionGoodsID);
					break;
				}
				case MagicActionIDs.GOTO_SHILIANTA:
					if (self is GameClient)
					{
						int minLevel = -1;
						SystemXmlItem systemXmlItem = Global.FindShiLianTaFuBenIDByLevel(self as GameClient, out minLevel);
						if (null != systemXmlItem)
						{
							GameClient client = self as GameClient;
							int fuBenID = systemXmlItem.GetIntValue("ID", -1);
							int goodsNumber = systemXmlItem.GetIntValue("GoodsNumber", -1);
							goodsNumber = Global.GMax(1, goodsNumber);
							int myTongTianLing = GameManager.ClientMgr.GetShiLianLingValue(client);
							if (myTongTianLing >= goodsNumber)
							{
								GameManager.ClientMgr.ModifyShiLianLingValue(client, -goodsNumber, true, true);
								Global.EnterShiLianTaFuBen(client, fuBenID, systemXmlItem, 1);
							}
							else
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(445, new object[0]), new object[]
								{
									myTongTianLing,
									goodsNumber
								}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 24);
							}
						}
						else if ((self as GameClient).ClientData.Level < minLevel)
						{
							if (minLevel > 0)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(448, new object[0]), new object[]
								{
									minLevel
								}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
							else
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(449, new object[0]), new object[]
								{
									minLevel
								}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
						}
						else
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(450, new object[0]), new object[]
							{
								minLevel
							}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
					}
					break;
				case MagicActionIDs.NEW_ADD_GOLD:
					if (obj is GameClient)
					{
						GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (int)actionParams[0], "NEW_ADD_GOLD");
					}
					break;
				case MagicActionIDs.GOTO_SHENGXIAOGUESSMAP:
					if (self is GameClient)
					{
						Global.ClientEnterShengXiaoGuessMap(self as GameClient);
					}
					break;
				case MagicActionIDs.GOTO_ARENABATTLEMAP:
					if (self is GameClient)
					{
						GameManager.ArenaBattleMgr.ClientEnterArenaBattle(self as GameClient);
					}
					break;
				case MagicActionIDs.USE_GOODSFORDLG:
					if (self is GameClient)
					{
						int windType = (int)actionParams[0];
						int fildID = (int)actionParams[1];
						GameManager.ClientMgr.NotifyClientOpenWindow(self as GameClient, windType, fildID.ToString());
					}
					break;
				case MagicActionIDs.SUB_PKZHI:
					if (self is GameClient)
					{
						int subPkPoint = (int)actionParams[0];
						int pkValue = (self as GameClient).ClientData.PKValue;
						int pkPoint = (self as GameClient).ClientData.PKPoint;
						pkPoint = Global.GMax(0, pkPoint - subPkPoint);
						GameManager.ClientMgr.SetRolePKValuePoint(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, pkValue, pkPoint, true);
					}
					break;
				case MagicActionIDs.CALL_MONSTER:
					if (self is GameClient)
					{
						int monsterID = (int)actionParams[0];
						int addNum = (int)actionParams[1];
						Point grid = (self as GameClient).CurrentGrid;
						GameManager.LuaMgr.AddDynamicMonsters(self as GameClient, monsterID, addNum, (int)grid.X, (int)grid.Y, 3);
					}
					break;
				case MagicActionIDs.NEW_ADD_JIFEN:
					if (self is GameClient)
					{
						int addValue2 = (int)actionParams[0];
						GameManager.ClientMgr.ModifyZhuangBeiJiFenValue(self as GameClient, addValue2, true, true);
					}
					break;
				case MagicActionIDs.NEW_ADD_LIESHA:
					if (self is GameClient)
					{
						int addValue2 = (int)actionParams[0];
						GameManager.ClientMgr.ModifyLieShaValue(self as GameClient, addValue2, true, true);
					}
					break;
				case MagicActionIDs.NEW_ADD_WUXING:
					if (self is GameClient)
					{
						int addValue2 = (int)actionParams[0];
						GameManager.ClientMgr.ModifyWuXingValue(self as GameClient, addValue2, true, true, true);
					}
					break;
				case MagicActionIDs.NEW_ADD_ZHENQI:
					if (self is GameClient)
					{
						int addValue2 = (int)actionParams[0];
						GameManager.ClientMgr.ModifyZhenQiValue(self as GameClient, addValue2, true, true);
					}
					break;
				case MagicActionIDs.DB_ADD_TIANSHENG:
					if (self is GameClient)
					{
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.FallTianSheng, actionParams, 0, true);
					}
					break;
				case MagicActionIDs.ADD_XINGYUN:
					if (self is GameClient)
					{
						Global.AddWeaponLucky(self as GameClient, (int)actionParams[0]);
					}
					break;
				case MagicActionIDs.FALL_XINGYUN:
					if (self is GameClient)
					{
						Global.ProcessWeaponTongLing(self as GameClient);
					}
					break;
				case MagicActionIDs.NEW_PACK_SHILIAN:
					if (self is GameClient)
					{
						int addValue2 = (int)actionParams[0];
						GameManager.ClientMgr.ModifyShiLianLingValue(self as GameClient, addValue2, true, true);
					}
					break;
				case MagicActionIDs.DB_NEW_ADD_ZHUFUTIME:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[0] / 60.0,
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ZhuFu, newParams, 0, true);
						Global.NotifySelfAddKaoHuoTime(self as GameClient, (int)newParams[0]);
					}
					break;
				case MagicActionIDs.NEW_ADD_MAPTIME:
					if (self is GameClient)
					{
						Global.AddExtLimitSecsByMapCode(self as GameClient, (int)actionParams[0], (int)actionParams[1]);
					}
					break;
				case MagicActionIDs.DB_ADD_WAWA_EXP:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.WaWaExp, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_TIME_LIFE_MAGIC:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[2],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddLifeMagic, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_INSTANT_LIFE_MAGIC:
					if (self is GameClient)
					{
						double addLiefV = actionParams[0];
						double addMagicV = actionParams[1];
						GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, addLiefV, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
						GameManager.ClientMgr.AddSpriteMagicV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, addMagicV, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
					}
					break;
				case MagicActionIDs.DB_ADD_MAXATTACKV:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.RemoveBufferData(self as GameClient, 39);
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddAttack, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_MAXMATTACKV:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.RemoveBufferData(self as GameClient, 39);
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddMAttack, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_MAXDSATTACKV:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.RemoveBufferData(self as GameClient, 39);
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddDSAttack, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_MAXDEFENSEV:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddDefense, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_MAXMDEFENSEV:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddMDefense, newParams, 0, true);
					}
					break;
				case MagicActionIDs.OPEN_QIAN_KUN_DAI:
					if (self is GameClient)
					{
					}
					break;
				case MagicActionIDs.RUN_LUA_SCRIPT:
					if (self is GameClient)
					{
						int fileID = (int)actionParams[0];
						string scriptFile = Global.GetRunLuaScriptFile(fileID);
						Global.ExcuteLuaFunction(self as GameClient, scriptFile, "run", null, null);
					}
					break;
				case MagicActionIDs.DB_ADD_EXP:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeExp, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_SEASONVIP:
				{
					bool isVipBefore = Global.IsVip(self as GameClient);
					actionParams = new double[]
					{
						129600.0,
						3.0
					};
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.MonthVIP, actionParams, 0, true);
					Global.BroadcastVIPMonthHint(self as GameClient, actionGoodsID);
					Global.TryGiveGuMuTimeLimitAwardOnBecomeVip(self as GameClient, isVipBefore);
					break;
				}
				case MagicActionIDs.DB_ADD_HALFYEARVIP:
				{
					bool isVipBefore = Global.IsVip(self as GameClient);
					actionParams = new double[]
					{
						259200.0,
						6.0
					};
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.MonthVIP, actionParams, 0, true);
					Global.BroadcastVIPMonthHint(self as GameClient, actionGoodsID);
					Global.TryGiveGuMuTimeLimitAwardOnBecomeVip(self as GameClient, isVipBefore);
					break;
				}
				case MagicActionIDs.GOTO_MINGJIEMAP:
					if (self is GameClient && actionParams.Length >= 2)
					{
						Global.GotoMingJieTimeLimitMap(self as GameClient, (int)actionParams[0], (int)actionParams[1]);
					}
					break;
				case MagicActionIDs.ADD_GUMUMAPTIME:
					if (self is GameClient && actionParams.Length >= 1)
					{
						Global.AddGuMuMapTime(self as GameClient, 0, (int)actionParams[0]);
					}
					break;
				case MagicActionIDs.ADD_BOSSCOPYENTERNUM:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						int addNum = (int)actionParams[0];
						if (addNum >= 0)
						{
							Global.UpdateBossFuBenExtraEnterNum(client, addNum);
							GameManager.LuaMgr.Hot(self as GameClient, string.Format(GLang.GetLang(451, new object[0]), addNum), 0);
							Global.ExecNpcTalkText(client, 2, 2130706641, 209, 1);
						}
					}
					break;
				case MagicActionIDs.GOTO_BOSSCOPYMAP:
					if (self is GameClient)
					{
						GameManager.LuaMgr.EnterBossFuBen(self as GameClient);
					}
					break;
				case MagicActionIDs.DB_ADD_FIVE_EXP:
				{
					Global.RemoveBufferData(self as GameClient, 1);
					Global.RemoveBufferData(self as GameClient, 18);
					Global.RemoveBufferData(self as GameClient, 46);
					double[] newParams = new double[]
					{
						actionParams[0],
						(double)actionGoodsID
					};
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.FiveExperience, newParams, 0, true);
					break;
				}
				case MagicActionIDs.DB_ADD_RANDOM_EXP:
					if (self is GameClient)
					{
						int minExp = Global.GMax(0, (int)actionParams[0]);
						int maxExp = Global.GMax(minExp, (int)actionParams[1]);
						GameManager.ClientMgr.ProcessRoleExperience(self as GameClient, (long)Global.GetRandomNumber(minExp, maxExp), false, false, false, "none");
					}
					break;
				case MagicActionIDs.GOTO_MAPBYYUANBAO:
					if (self is GameClient)
					{
						int needYuanBao = (int)actionParams[0];
						int toMapCode = (int)actionParams[1];
						if (needYuanBao > 0)
						{
							bool subOk = GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, self as GameClient, needYuanBao, "GOTO_MAPBYYUANBAO公式", true, true, false, DaiBiSySType.None);
							if (subOk)
							{
								GameManager.LuaMgr.GotoMap(self as GameClient, toMapCode, -1, -1, -1);
							}
							else
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(431, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 30);
							}
						}
					}
					break;
				case MagicActionIDs.ADD_DAILY_NUM:
					if (self is GameClient)
					{
						int taskClass = Global.GMax(3, (int)actionParams[0]);
						taskClass = Global.GMin(9, (int)actionParams[0]);
						int addNum = Global.GMax(1, (int)actionParams[1]);
						Global.AddExtNumByGoods(self as GameClient, taskClass, addNum);
					}
					break;
				case MagicActionIDs.DB_TIME_LIFE_NOSHOW:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddLifeNoShow, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_TIME_MAGIC_NOSHOW:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddMagicNoShow, newParams, 0, true);
					}
					break;
				case MagicActionIDs.GOTO_GUMUMAP:
					if (self is GameClient)
					{
						Global.GotoGuMuMap(self as GameClient);
					}
					break;
				case MagicActionIDs.DB_ADD_MULTIEXP:
					if (self is GameClient)
					{
						Global.RemoveBufferData(self as GameClient, 1);
						Global.RemoveBufferData(self as GameClient, 18);
						Global.RemoveBufferData(self as GameClient, 36);
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)((long)actionGoodsID << 32 | (long)actionParams[0])
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.MutilExperience, newParams, 0, true);
					}
					break;
				case MagicActionIDs.RANDOM_SHENQIZHIHUN:
					if (self is GameClient)
					{
						int minNum = (int)actionParams[0];
						int maxNum = (int)actionParams[1];
						int giveShenQiZhiHun = Global.GetRandomNumber(minNum, maxNum + 1);
						GameManager.ClientMgr.ModifyZhuangBeiJiFenValue(self as GameClient, giveShenQiZhiHun, true, true);
					}
					break;
				case MagicActionIDs.ADD_JIERI_BUFF:
					if (self is GameClient)
					{
						int maxHours = (int)actionParams[0];
						double[] newActionParams = new double[]
						{
							(double)actionGoodsID,
							(double)maxHours
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.JieRiChengHao, newActionParams, 0, true);
						Global.InitJieriChengHao(self as GameClient, true);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, true, false, 7);
					}
					break;
				case MagicActionIDs.DB_ADD_ERGUOTOU:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)((long)actionGoodsID << 32 | (long)actionParams[0])
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ErGuoTou, newParams, 0, true);
					}
					break;
				case MagicActionIDs.NEW_ADD_ZHANHUN:
					if (self is GameClient)
					{
						GameManager.ClientMgr.ModifyZhanHunValue(self as GameClient, (int)actionParams[0], true, true);
					}
					break;
				case MagicActionIDs.NEW_ADD_RONGYU:
					if (self is GameClient)
					{
						GameManager.ClientMgr.ModifyRongYuValue(self as GameClient, (int)actionParams[0], true, true);
					}
					break;
				case MagicActionIDs.DB_ADD_TEMPSTRENGTH:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPStrength, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_TEMPINTELLIGENCE:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPIntelligsence, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_TEMPDEXTERITY:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPDexterity, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_TEMPCONSTITUTION:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPConstitution, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_TEMPATTACKSPEED:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPATTACKSPEED, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_LUCKYATTACK:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPLUCKYATTACK, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_FATALATTACK:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPFATALATTACK, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_DOUBLEATTACK:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPDOUBLEATTACK, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_LUCKYATTACKPERCENTTIMER:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDLUCKYATTACKPERCENTTIMER, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_FATALATTACKPERCENTTIMER:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDFATALATTACKPERCENTTIMER, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_DOUBLETACKPERCENTTIMER:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDDOUBLEATTACKPERCENTTIMER, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_MAXHPVALUE:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDMAXHPVALUE, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_MAXMPVALUE:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDMAXMPVALUE, newParams, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_LIFERECOVERPERCENT:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDLIFERECOVERPERCENT, newParams, 0, true);
					}
					break;
				case MagicActionIDs.MU_ADD_PHYSICAL_ATTACK:
				{
					double nMin = actionParams[0];
					double nMax = actionParams[1];
					double nStep = actionParams[2];
					nMin += nStep * (double)skillLevel;
					nMax += nStep * (double)skillLevel;
					direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
					int attackType = 0;
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, (int)nMin, (int)nMax, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, (int)nMin, (int)nMax, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, (int)nMin, (int)nMax, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, (int)nMin, (int)nMax, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, (int)nMin, (int)nMax, 1.0, 0, 0);
						}
					}
					else if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, (int)nMin, (int)nMax, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
					}
					else if (obj is Monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, (int)nMin, (int)nMax, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
					}
					else if (!(obj is BiaoCheItem))
					{
						if (obj is JunQiItem)
						{
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_MAGIC_ATTACK:
				{
					double nMin = actionParams[0];
					double nMax = actionParams[1];
					double nStep = actionParams[2];
					nMin += nStep * (double)skillLevel;
					nMax += nStep * (double)skillLevel;
					direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
					int attackType = 1;
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, (int)nMin, (int)nMax, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, (int)nMin, (int)nMax, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, (int)nMin, (int)nMax, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, (int)nMin, (int)nMax, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, (int)nMin, (int)nMax, 1.0, 0, 0);
						}
					}
					else if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, (int)nMin, (int)nMax, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
					}
					else if (obj is Monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, (int)nMin, (int)nMax, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
					}
					else if (!(obj is BiaoCheItem))
					{
						if (obj is JunQiItem)
						{
						}
					}
					break;
				}
				case MagicActionIDs.MU_SUB_DAMAGE_PERCENT_TIMER:
					if (self is GameClient)
					{
						double dSecs = actionParams[0];
						double dPercent = actionParams[1];
						double nStep = actionParams[2];
						dPercent += nStep * (double)skillLevel;
						double[] newActionParams = new double[]
						{
							dSecs,
							dPercent
						};
						if (obj is GameClient)
						{
							(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.MU_SUB_DAMAGE_PERCENT_TIMER, actionParams, -1);
							Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_SUBDAMAGEPERCENTTIMER, newActionParams, 1, true);
						}
					}
					break;
				case MagicActionIDs.MU_ADD_HP_PERCENT_TIMER:
					if (self is GameClient)
					{
						double dSecs = actionParams[0];
						double dPercent = actionParams[1];
						double nStep = actionParams[2];
						dPercent += nStep * (double)skillLevel;
						long NowTicks = TimeUtil.NOW() * 10000L;
						long ToTick = (long)((double)NowTicks + dSecs * 1000.0 * 10000.0);
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(14, dPercent, ToTick);
							double[] newActionParams = new double[]
							{
								dSecs,
								dPercent
							};
							Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_MAXLIFEPERCENT, newActionParams, 1, true);
						}
						else
						{
							(self as GameClient).RoleBuffer.AddTempExtProp(14, dPercent, ToTick);
							double[] newActionParams = new double[]
							{
								dSecs,
								dPercent
							};
							Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_MAXLIFEPERCENT, newActionParams, 1, true);
						}
					}
					break;
				case MagicActionIDs.MU_ADD_DEFENSE_TIMER:
					if (self is GameClient)
					{
						double dSecs = actionParams[0];
						double dValue = actionParams[1];
						double dStep = actionParams[2];
						dValue += dStep * (double)skillLevel;
						long NowTicks = TimeUtil.NOW() * 10000L;
						long ToTick = (long)((double)NowTicks + dSecs * 1000.0 * 10000.0);
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(3, dValue, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(4, dValue, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(5, dValue, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(6, dValue, ToTick);
							double[] newActionParams = new double[]
							{
								dSecs,
								dValue
							};
							Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_ADDDEFENSETIMER, newActionParams, 1, true);
						}
						else
						{
							(self as GameClient).RoleBuffer.AddTempExtProp(3, dValue, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(4, dValue, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(5, dValue, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(5, dValue, ToTick);
							double[] newActionParams = new double[]
							{
								dSecs,
								dValue
							};
							Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDDEFENSETIMER, newActionParams, 1, true);
						}
					}
					break;
				case MagicActionIDs.MU_ADD_ATTACK_TIMER:
					if (self is GameClient)
					{
						double dSecs = actionParams[0];
						double dValue = actionParams[1];
						double dStep = actionParams[2];
						dValue += dStep * (double)skillLevel;
						long NowTicks = TimeUtil.NOW() * 10000L;
						long ToTick = (long)((double)NowTicks + dSecs * 1000.0 * 10000.0);
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(7, dValue, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(8, dValue, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(9, dValue, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(10, dValue, ToTick);
							double[] newActionParams = new double[]
							{
								dSecs,
								dValue
							};
							Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_ADDATTACKTIMER, newActionParams, 1, true);
						}
						else
						{
							(self as GameClient).RoleBuffer.AddTempExtProp(7, dValue, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(8, dValue, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(9, dValue, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(10, dValue, ToTick);
							double[] newActionParams = new double[]
							{
								dSecs,
								dValue
							};
							Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDATTACKTIMER, newActionParams, 1, true);
						}
					}
					break;
				case MagicActionIDs.MU_ADD_HP:
					if (self is GameClient)
					{
						double dResume = actionParams[0];
						double dStep = actionParams[1];
						dResume += dStep * (double)skillLevel;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (double)((int)dResume), string.Format("无情一击, 脚本{0}", id));
						}
						else
						{
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (double)((int)dResume), string.Format("无情一击, 脚本{0}", id));
						}
					}
					break;
				case MagicActionIDs.MU_BLINK_MOVE:
					if (self is GameClient)
					{
						double dTime = actionParams[0];
						double dDistance = actionParams[1];
						long ticks = TimeUtil.NOW();
						DelayAction temInfor = new DelayAction();
						temInfor.m_DelayTime = (long)dTime;
						temInfor.m_StartTime = ticks;
						temInfor.m_Params[0] = (int)dDistance;
						temInfor.m_Client = (self as GameClient);
						List<object> objsList = Global.GetAll9Clients(self as GameClient);
						string strcmd = string.Format("{0}", (self as GameClient).ClientData.RoleID);
						GameManager.ClientMgr.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, objsList, strcmd, 510);
						DelayActionManager.AddDelayAction(temInfor);
					}
					break;
				case MagicActionIDs.MU_SUB_DAMAGE_PERCENT_TIMER1:
					if (self is GameClient)
					{
						double dSecs = actionParams[0];
						double dPercent = actionParams[1];
						double nStep = actionParams[2];
						dPercent += nStep * (double)skillLevel;
						double[] newActionParams = new double[]
						{
							dSecs,
							dPercent
						};
						if (obj is GameClient)
						{
							(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.MU_SUB_DAMAGE_PERCENT_TIMER, actionParams, -1);
							Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_SUBDAMAGEPERCENTTIMER1, newActionParams, 1, true);
						}
					}
					break;
				case MagicActionIDs.MU_RANDOM_SHUXING:
					if (self is GameClient)
					{
						DataHelper.WriteStackTraceLog("随机增加基础属性之一的功能尚未实现");
					}
					break;
				case MagicActionIDs.MU_RANDOM_STRENGTH:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						if (client != null)
						{
							int nRate = (int)(actionParams[0] * 100.0);
							int nMin2 = (int)actionParams[1];
							int nMax2 = (int)actionParams[2];
							int nOld = 0;
							int randNum = Global.GetRandomNumber(0, 101);
							if (randNum <= nRate)
							{
								int nValue = Global.GetRandomNumber(nMin2, nMax2 + 1);
								string strPorpName = GLang.GetLang(452, new object[0]);
								nOld = Global.GetRoleParamsInt32FromDB(client, "PropStrengthChangeless");
								if (bItemAddVal)
								{
									int nPropLimit = UseFruitVerify.GetFruitAddPropLimit(client, "Strength");
									nValue = UseFruitVerify.AddValueVerify(client, nOld, nPropLimit, nValue);
									if (nValue <= 0)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(457, new object[0]), new object[]
										{
											strPorpName
										}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										ret = false;
										break;
									}
								}
								if (!bIsVerify)
								{
									lock (client.ClientData.PropPointMutex)
									{
										nOld = Global.GetRoleParamsInt32FromDB(client, "PropStrengthChangeless");
										Global.SaveRoleParamsInt32ValueToDB(client, "PropStrengthChangeless", nOld + nValue, true);
										client.ClientData.PropStrength += nValue;
										Global.SaveRoleParamsInt32ValueToDB(client, "PropStrength", client.ClientData.PropStrength, true);
										nOld = Global.GetRoleParamsInt32FromDB(client, "TotalPropPoint");
										client.ClientData.TotalPropPoint = nOld + nValue;
									}
									Global.SaveRoleParamsInt32ValueToDB(client, "TotalPropPoint", nOld + nValue, true);
									Global.RefreshEquipProp(client);
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(456, new object[0]), new object[]
									{
										strPorpName,
										nValue
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.MU_RANDOM_INTELLIGENCE:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						if (client != null)
						{
							int nRate = (int)(actionParams[0] * 100.0);
							int nMin2 = (int)actionParams[1];
							int nMax2 = (int)actionParams[2];
							int nOld = 0;
							int randNum = Global.GetRandomNumber(0, 101);
							if (randNum <= nRate)
							{
								int nValue = Global.GetRandomNumber(nMin2, nMax2 + 1);
								string strPorpName = GLang.GetLang(453, new object[0]);
								nOld = Global.GetRoleParamsInt32FromDB(client, "PropIntelligenceChangeless");
								if (bItemAddVal)
								{
									int nPropLimit = UseFruitVerify.GetFruitAddPropLimit(client, "Intelligence");
									nValue = UseFruitVerify.AddValueVerify(client, nOld, nPropLimit, nValue);
									if (nValue <= 0)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(457, new object[0]), new object[]
										{
											strPorpName
										}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										ret = false;
										break;
									}
								}
								if (!bIsVerify)
								{
									lock (client.ClientData.PropPointMutex)
									{
										nOld = Global.GetRoleParamsInt32FromDB(client, "PropIntelligenceChangeless");
										Global.SaveRoleParamsInt32ValueToDB(client, "PropIntelligenceChangeless", nOld + nValue, true);
										client.ClientData.PropIntelligence += nValue;
										Global.SaveRoleParamsInt32ValueToDB(client, "PropIntelligence", client.ClientData.PropIntelligence, true);
										nOld = Global.GetRoleParamsInt32FromDB(client, "TotalPropPoint");
										client.ClientData.TotalPropPoint = nOld + nValue;
										Global.SaveRoleParamsInt32ValueToDB(client, "TotalPropPoint", nOld + nValue, true);
									}
									Global.RefreshEquipProp(client);
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(456, new object[0]), new object[]
									{
										strPorpName,
										nValue
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.MU_RANDOM_DEXTERITY:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						if (client != null)
						{
							int nRate = (int)(actionParams[0] * 100.0);
							int nMin2 = (int)actionParams[1];
							int nMax2 = (int)actionParams[2];
							int nOld = 0;
							int randNum = Global.GetRandomNumber(0, 101);
							if (randNum <= nRate)
							{
								int nValue = Global.GetRandomNumber(nMin2, nMax2 + 1);
								string strPorpName = GLang.GetLang(454, new object[0]);
								nOld = Global.GetRoleParamsInt32FromDB(client, "PropDexterityChangeless");
								if (bItemAddVal)
								{
									int nPropLimit = UseFruitVerify.GetFruitAddPropLimit(client, "Dexterity");
									nValue = UseFruitVerify.AddValueVerify(client, nOld, nPropLimit, nValue);
									if (nValue <= 0)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(457, new object[0]), new object[]
										{
											strPorpName
										}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										ret = false;
										break;
									}
								}
								if (!bIsVerify)
								{
									lock (client.ClientData.PropPointMutex)
									{
										nOld = Global.GetRoleParamsInt32FromDB(client, "PropDexterityChangeless");
										Global.SaveRoleParamsInt32ValueToDB(client, "PropDexterityChangeless", nOld + nValue, true);
										client.ClientData.PropDexterity += nValue;
										Global.SaveRoleParamsInt32ValueToDB(client, "PropDexterity", client.ClientData.PropDexterity, true);
										nOld = Global.GetRoleParamsInt32FromDB(client, "TotalPropPoint");
										client.ClientData.TotalPropPoint = nOld + nValue;
										Global.SaveRoleParamsInt32ValueToDB(client, "TotalPropPoint", nOld + nValue, true);
									}
									Global.RefreshEquipProp(client);
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(456, new object[0]), new object[]
									{
										strPorpName,
										nValue
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.MU_RANDOM_CONSTITUTION:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						if (client != null)
						{
							int nRate = (int)(actionParams[0] * 100.0);
							int nMin2 = (int)actionParams[1];
							int nMax2 = (int)actionParams[2];
							int nOld = 0;
							int randNum = Global.GetRandomNumber(0, 101);
							if (randNum <= nRate)
							{
								int nValue = Global.GetRandomNumber(nMin2, nMax2 + 1);
								string strPorpName = GLang.GetLang(455, new object[0]);
								nOld = Global.GetRoleParamsInt32FromDB(client, "PropConstitutionChangeless");
								if (bItemAddVal)
								{
									int nPropLimit = UseFruitVerify.GetFruitAddPropLimit(client, "Constitution");
									nValue = UseFruitVerify.AddValueVerify(client, nOld, nPropLimit, nValue);
									if (nValue <= 0)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(457, new object[0]), new object[]
										{
											strPorpName
										}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										ret = false;
										break;
									}
								}
								if (!bIsVerify)
								{
									lock (client.ClientData.PropPointMutex)
									{
										nOld = Global.GetRoleParamsInt32FromDB(client, "PropConstitutionChangeless");
										Global.SaveRoleParamsInt32ValueToDB(client, "PropConstitutionChangeless", nOld + nValue, true);
										client.ClientData.PropConstitution += nValue;
										Global.SaveRoleParamsInt32ValueToDB(client, "PropConstitution", client.ClientData.PropConstitution, true);
										nOld = Global.GetRoleParamsInt32FromDB(client, "TotalPropPoint");
										client.ClientData.TotalPropPoint = nOld + nValue;
										Global.SaveRoleParamsInt32ValueToDB(client, "TotalPropPoint", nOld + nValue, true);
									}
									Global.RefreshEquipProp(client);
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(456, new object[0]), new object[]
									{
										strPorpName,
										nValue
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.MU_ADD_PHYSICAL_ATTACK1:
				{
					double nBaseRateValue = actionParams[0];
					double nAddValue = actionParams[1];
					int nSkillLevel = skillLevel + 1;
					nBaseRateValue += nBaseRateValue / 200.0 * (double)nSkillLevel;
					nAddValue += nAddValue * (double)nSkillLevel;
					direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
					int attackType = 0;
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					else if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
					}
					else if (obj is Monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
					}
					else if (!(obj is BiaoCheItem))
					{
						if (obj is JunQiItem)
						{
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_PHYSICAL_ATTACK2:
				{
					double nBaseRateValue = actionParams[0];
					double nAddValue = actionParams[1];
					double addPercent = actionParams[2];
					int nSkillLevel = skillLevel + 1;
					nBaseRateValue += nBaseRateValue * addPercent * (double)skillLevel;
					nAddValue += nAddValue * (double)skillLevel;
					direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
					int attackType = 0;
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					else if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
					}
					else if (obj is Monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
					}
					else if (!(obj is BiaoCheItem))
					{
						if (obj is JunQiItem)
						{
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_ATTACK_DOWN:
					if (self is GameClient)
					{
						double dRate = actionParams[0];
						double dPercent = actionParams[1];
						double dTime = actionParams[2];
						int nSkillLevel = skillLevel + 1;
						dRate = StateRate.GetNegativeRate(self, obj, dRate, ExtPropIndexes.StateJiTui, id);
						if ((double)Global.GetRandomNumber(0, 101) > dRate * 100.0)
						{
							return false;
						}
						ZuoQiManager.getInstance().RoleDisMount(obj as GameClient, true);
						long ToTick = TimeUtil.NOW() * 10000L + (long)dTime * 1000L * 10000L;
						double addValue = actionParams[1];
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(11, -addValue, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(12, -addValue, ToTick);
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 6, TimeUtil.NOW(), (int)dTime, 0.0);
						}
						else if (obj is Monster)
						{
							Monster monster = obj as Monster;
							if (101 == monster.MonsterType || 301 == monster.MonsterType || 1801 == monster.MonsterType)
							{
								monster.TempPropsBuffer.AddTempExtProp(11, -addValue, ToTick);
								monster.TempPropsBuffer.AddTempExtProp(12, -addValue, ToTick);
								GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 6, TimeUtil.NOW(), (int)dTime, 0.0);
							}
						}
					}
					break;
				case MagicActionIDs.MU_ADD_HUNMI:
				{
					double dRate = actionParams[0];
					double dTime = actionParams[1];
					int nSkillLevel = skillLevel + 1;
					dRate = StateRate.GetNegativeRate(self, obj, dRate, ExtPropIndexes.StateHunMi, id);
					if ((double)Global.GetRandomNumber(0, 101) > dRate * 100.0)
					{
						return false;
					}
					ZuoQiManager.getInstance().RoleDisMount(obj as GameClient, true);
					if (obj is GameClient)
					{
						if (obj is GameClient)
						{
							(obj as GameClient).ClientData.DongJieStart = TimeUtil.NOW();
							(obj as GameClient).ClientData.DongJieSeconds = (int)dTime;
							long ToTick = TimeUtil.NOW() * 10000L + (long)dTime * 1000L * 10000L;
							(obj as GameClient).RoleBuffer.AddTempExtProp(50, 1.0, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(2, -10.0, ToTick);
							double moveCost = RoleAlgorithm.GetMoveSpeed(obj as GameClient);
							if ((obj as GameClient).ClientData.HorseDbID > 0)
							{
								double horseSpeed = Global.GetHorseSpeed(obj as GameClient);
								moveCost += horseSpeed;
							}
							(obj as GameClient).ClientData.MoveSpeed = moveCost;
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 3, TimeUtil.NOW(), (int)dTime, moveCost);
						}
					}
					else if (obj is Monster)
					{
						Monster monster = obj as Monster;
						if (101 == monster.MonsterType || 301 == monster.MonsterType || 1501 == monster.MonsterType || 1801 == monster.MonsterType)
						{
							monster.DongJieStart = TimeUtil.NOW();
							monster.DongJieSeconds = (int)dTime;
							monster.XuanYunStart = TimeUtil.NOW();
							monster.XuanYunSeconds = (int)dTime;
							GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 3, TimeUtil.NOW(), (int)dTime, 0.0);
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_MOVESPEED_DOWN:
				{
					double dRate = actionParams[0];
					double dPercent = actionParams[1];
					double dTime = actionParams[2];
					int nSkillLevel = skillLevel + 1;
					dRate = StateRate.GetNegativeRate(self, obj, dRate, ExtPropIndexes.StateMoveSpeed, id);
					if ((double)Global.GetRandomNumber(0, 101) > dRate * 100.0)
					{
						return false;
					}
					ZuoQiManager.getInstance().RoleDisMount(obj as GameClient, true);
					long ToTick = TimeUtil.NOW() * 10000L + (long)dTime * 1000L * 10000L;
					double addValue = actionParams[1];
					if (obj is GameClient)
					{
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(2, -addValue, ToTick);
							(obj as GameClient).buffManager.SetStatusBuff(118, TimeUtil.NOW(), (long)dTime * 1000L, 0L);
							double moveCost = RoleAlgorithm.GetMoveSpeed(obj as GameClient);
							if ((obj as GameClient).ClientData.HorseDbID > 0)
							{
								double horseSpeed = Global.GetHorseSpeed(obj as GameClient);
								moveCost += horseSpeed;
							}
							(obj as GameClient).ClientData.MoveSpeed = moveCost;
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 4, TimeUtil.NOW(), (int)dTime, moveCost);
						}
					}
					else if (obj is Monster)
					{
						Monster monster = obj as Monster;
						if (null != monster)
						{
							monster.TempPropsBuffer.AddTempExtProp(2, -addValue, ToTick);
							monster.SpeedDownStart = TimeUtil.NOW();
							monster.SpeedDownSeconds = (int)dTime;
							GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 4, TimeUtil.NOW(), (int)dTime, monster.MoveSpeed);
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_LIFE:
				{
					double dBaseRate = actionParams[0];
					double dAddValue = actionParams[1];
					double dTime = actionParams[2];
					int nSkillLevel = skillLevel + 1;
					double dRealyRate = dBaseRate + dBaseRate / 200.0 * (double)nSkillLevel;
					double dRealyValue = dAddValue + dAddValue * (double)nSkillLevel;
					long ToTick = TimeUtil.NOW() * 10000L + (long)dTime * 1000L * 10000L;
					double[] newParams = new double[]
					{
						dTime,
						(double)skillid,
						(double)nSkillLevel
					};
					if (obj is GameClient)
					{
						(obj as GameClient).RoleBuffer.AddTempExtProp(14, dRealyRate, ToTick);
						(obj as GameClient).RoleBuffer.AddTempExtProp(13, dRealyValue, ToTick);
					}
					else
					{
						(self as GameClient).RoleBuffer.AddTempExtProp(14, dRealyRate, ToTick);
						(self as GameClient).RoleBuffer.AddTempExtProp(13, dRealyValue, ToTick);
					}
					Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_ADDMAXLIFEPERCENTANDVALUE, newParams, 1, true);
					break;
				}
				case MagicActionIDs.MU_ADD_MAGIC_ATTACK1:
					if (0.0 != manyRangeInjuredPercent)
					{
						double nBaseRateValue = actionParams[0];
						double nAddValue = actionParams[1];
						int nSkillLevel = skillLevel + 1;
						nBaseRateValue += nBaseRateValue / 200.0 * (double)nSkillLevel;
						nAddValue += nAddValue * (double)nSkillLevel;
						direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
						int attackType = 1;
						if (self is GameClient)
						{
							if (obj is GameClient)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
							}
							else if (obj is Monster)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
							}
							else if (obj is BiaoCheItem)
							{
								BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
							else if (obj is JunQiItem)
							{
								JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
							else if (obj is FakeRoleItem)
							{
								FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
						}
						else if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
						}
						else if (!(obj is BiaoCheItem))
						{
							if (obj is JunQiItem)
							{
								JunQiManager.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
						}
					}
					break;
				case MagicActionIDs.MU_ADD_MAGIC_ATTACK2:
					if (0.0 != manyRangeInjuredPercent)
					{
						double nBaseRateValue = actionParams[0];
						double nAddValue = actionParams[1];
						double addPercent = actionParams[2];
						int nSkillLevel = skillLevel + 1;
						nBaseRateValue += nBaseRateValue * addPercent * (double)skillLevel;
						nAddValue += nAddValue * (double)skillLevel;
						direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
						int attackType = 1;
						if (self is GameClient)
						{
							if (obj is GameClient)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
							}
							else if (obj is Monster)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
							}
							else if (obj is BiaoCheItem)
							{
								BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
							else if (obj is JunQiItem)
							{
								JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
							else if (obj is FakeRoleItem)
							{
								FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
						}
						else if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, nBaseRateValue, (int)nAddValue, 0, skillid, shenShiInjurePercent);
						}
						else if (!(obj is BiaoCheItem))
						{
							if (obj is JunQiItem)
							{
								JunQiManager.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
						}
					}
					break;
				case MagicActionIDs.MU_ADD_HIT_DOWN:
				{
					double dRate = actionParams[0];
					double dPercent = actionParams[1];
					double dTime = actionParams[2];
					int nSkillLevel = skillLevel + 1;
					dRate = StateRate.GetNegativeRate(self, obj, dRate, ExtPropIndexes.StateJiTui, id);
					if ((double)Global.GetRandomNumber(0, 101) > dRate * 100.0)
					{
						return false;
					}
					ZuoQiManager.getInstance().RoleDisMount(obj as GameClient, true);
					long ToTick = TimeUtil.NOW() * 10000L + (long)dTime * 1000L * 10000L;
					double addValue = actionParams[1];
					if (obj is GameClient)
					{
						(obj as GameClient).RoleBuffer.AddTempExtProp(18, -addValue, ToTick);
						GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 5, TimeUtil.NOW(), (int)dTime, 0.0);
					}
					else if (obj is Monster)
					{
						Monster monster = obj as Monster;
						if (101 == monster.MonsterType || 301 == monster.MonsterType || 1801 == monster.MonsterType)
						{
							monster.TempPropsBuffer.AddTempExtProp(18, -addValue, ToTick);
							GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 5, TimeUtil.NOW(), (int)dTime, 0.0);
						}
					}
					break;
				}
				case MagicActionIDs.MU_SUB_DAMAGE_PERCENT:
					if (self is GameClient)
					{
						double dBaseRate = actionParams[0];
						double dAddValue = actionParams[1];
						double dTime = actionParams[2];
						int nSkillLevel = skillLevel + 1;
						double dRealyRate = dBaseRate + dBaseRate / 200.0 * (double)nSkillLevel;
						double dRealyValue = dAddValue + dAddValue * (double)nSkillLevel;
						double[] newActionParams2 = new double[]
						{
							dTime,
							dRealyRate
						};
						double[] newActionParams3 = new double[]
						{
							dTime,
							dRealyValue
						};
						double[] newParams = new double[]
						{
							dTime,
							(double)skillid,
							(double)nSkillLevel
						};
						if (obj is GameClient)
						{
							(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.MU_SUB_DAMAGE_PERCENT_TIMER, newActionParams2, -1);
							(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.MU_SUB_DAMAGE_VALUE, newActionParams3, -1);
						}
						else
						{
							(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.MU_SUB_DAMAGE_PERCENT_TIMER, newActionParams2, -1);
							(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.MU_SUB_DAMAGE_VALUE, newActionParams3, -1);
						}
						Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_SUBDAMAGEPERCENTVALUETIMER, newParams, 1, true);
					}
					break;
				case MagicActionIDs.MU_ADD_DEFENSE_DOWN:
					if (self is GameClient)
					{
						double dRate = actionParams[0];
						double dPercent = actionParams[1];
						double dTime = actionParams[2];
						int nSkillLevel = skillLevel + 1;
						double dRealRate = dRate + dRate / 200.0 * (double)nSkillLevel;
						if ((double)Global.GetRandomNumber(0, 101) > dRealRate * 100.0)
						{
							return false;
						}
						long ToTick = TimeUtil.NOW() * 10000L + (long)dTime * 1000L * 10000L;
						double addValue = actionParams[1];
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(42, -addValue, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(43, -addValue, ToTick);
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 7, TimeUtil.NOW(), (int)dTime, 0.0);
						}
						else if (obj is Monster)
						{
							Monster monster = obj as Monster;
							if (101 == monster.MonsterType || 301 == monster.MonsterType || 1801 == monster.MonsterType)
							{
								monster.TempPropsBuffer.AddTempExtProp(42, -addValue, ToTick);
								monster.TempPropsBuffer.AddTempExtProp(43, -addValue, ToTick);
								GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 7, TimeUtil.NOW(), (int)dTime, 0.0);
							}
						}
						else
						{
							(self as GameClient).RoleBuffer.AddTempExtProp(42, -addValue, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(43, -addValue, ToTick);
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, 7, TimeUtil.NOW(), (int)dTime, 0.0);
						}
					}
					break;
				case MagicActionIDs.MU_ADD_DEFENSE_ATTACK:
					if (self is GameClient)
					{
						double dBaseRate = actionParams[0];
						double dAddValue = actionParams[1];
						double dTime = actionParams[2];
						long ToTick = TimeUtil.NOW() * 10000L + (long)dTime * 1000L * 10000L;
						int nSkillLevel = skillLevel + 1;
						double dRealyRate = dBaseRate + dBaseRate / 200.0 * (double)nSkillLevel;
						double dRealyValue = dAddValue + dAddValue * (double)nSkillLevel;
						double[] newParams = new double[]
						{
							dTime,
							(double)skillid,
							(double)nSkillLevel
						};
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(11, dRealyRate, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(12, dRealyRate, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(7, dRealyValue, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(8, dRealyValue, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(9, dRealyValue, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(10, dRealyValue, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(42, dRealyRate, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(43, dRealyRate, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(3, dRealyValue, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(4, dRealyValue, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(5, dRealyValue, ToTick);
							(obj as GameClient).RoleBuffer.AddTempExtProp(6, dRealyValue, ToTick);
						}
						else
						{
							(self as GameClient).RoleBuffer.AddTempExtProp(11, dRealyRate, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(12, dRealyRate, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(7, dRealyValue, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(8, dRealyValue, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(9, dRealyValue, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(10, dRealyValue, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(42, dRealyRate, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(43, dRealyRate, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(3, dRealyValue, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(4, dRealyValue, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(5, dRealyValue, ToTick);
							(self as GameClient).RoleBuffer.AddTempExtProp(6, dRealyValue, ToTick);
						}
						Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_ADDATTACKANDDEFENSEEPERCENTVALUETIMER, newParams, 1, true);
					}
					break;
				case MagicActionIDs.MU_ADD_JITUI:
				{
					double dRate = actionParams[0];
					double dDistance = actionParams[1];
					double dType = actionParams[2];
					int nSkillLevel = skillLevel + 1;
					dRate = StateRate.GetNegativeRate(self, obj, dRate, ExtPropIndexes.StateJiTui, id);
					if ((double)Global.GetRandomNumber(0, 101) > dRate * 100.0)
					{
						return false;
					}
					ZuoQiManager.getInstance().RoleDisMount(obj as GameClient, true);
					int nDistance = (int)dDistance;
					int attackType = (int)dType;
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, false, 0.0, 0, nDistance, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, 0.0, 0, nDistance, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, (double)nDistance, 0, 0);
						}
					}
					else if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, 0.0, 0, nDistance, 0, 0.0);
					}
					else if (obj is Monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 0.0, 0, 0, 0, 0.0);
					}
					else if (!(obj is BiaoCheItem))
					{
						if (obj is JunQiItem)
						{
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_DINGSHENG:
				{
					double dRate = actionParams[0];
					double dTime = actionParams[1];
					int nSkillLevel = skillLevel + 1;
					dRate = StateRate.GetNegativeRate(self, obj, dRate, ExtPropIndexes.StateDingShen, id);
					if ((double)Global.GetRandomNumber(0, 101) > dRate * 100.0)
					{
						return false;
					}
					ZuoQiManager.getInstance().RoleDisMount(obj as GameClient, true);
					long ToTick = TimeUtil.NOW() * 10000L + (long)dTime * 1000L * 10000L;
					if (obj is GameClient)
					{
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(47, 1.0, ToTick);
							double moveCost = RoleAlgorithm.GetMoveSpeed(obj as GameClient);
							(obj as GameClient).ClientData.MoveSpeed = moveCost;
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 8, TimeUtil.NOW(), (int)dTime, moveCost);
						}
					}
					else if (obj is Monster)
					{
						Monster monster = obj as Monster;
						if (null != monster)
						{
							if (101 == monster.MonsterType || 301 == monster.MonsterType || 1501 == monster.MonsterType || 1801 == monster.MonsterType)
							{
								monster.TempPropsBuffer.AddTempExtProp(2, -monster.MoveSpeed, ToTick);
								monster.DingShenStart = TimeUtil.NOW();
								monster.DingShenSeconds = (int)dTime;
								GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 8, TimeUtil.NOW(), (int)dTime, monster.MoveSpeed);
							}
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_HIT_DODGE:
				{
					double dBaseRate = actionParams[0];
					double dAddValue = actionParams[1];
					double dTime = actionParams[2];
					int nSkillLevel = skillLevel + 1;
					double dRealyRate = dBaseRate + dBaseRate / 200.0 * (double)nSkillLevel;
					double dRealyValue = dAddValue + dAddValue * (double)nSkillLevel;
					long ToTick = TimeUtil.NOW() * 10000L + (long)dTime * 1000L * 10000L;
					double[] newParams = new double[]
					{
						dTime,
						(double)skillid,
						(double)nSkillLevel
					};
					if (obj is GameClient)
					{
						(obj as GameClient).RoleBuffer.AddTempExtProp(54, dRealyRate, ToTick);
						(obj as GameClient).RoleBuffer.AddTempExtProp(18, dRealyValue, ToTick);
						(obj as GameClient).RoleBuffer.AddTempExtProp(55, dRealyRate, ToTick);
						(obj as GameClient).RoleBuffer.AddTempExtProp(19, dRealyValue, ToTick);
					}
					else
					{
						(self as GameClient).RoleBuffer.AddTempExtProp(54, dRealyRate, ToTick);
						(self as GameClient).RoleBuffer.AddTempExtProp(18, dRealyValue, ToTick);
						(self as GameClient).RoleBuffer.AddTempExtProp(55, dRealyRate, ToTick);
						(self as GameClient).RoleBuffer.AddTempExtProp(19, dRealyValue, ToTick);
					}
					Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_ADD_HIT_DODGE_PERCENT, newParams, 1, true);
					break;
				}
				case MagicActionIDs.MU_ADD_CHENMO:
				{
					double[] newActionParams = new double[4];
					BufferItemTypes BufferItemType = BufferItemTypes.None;
					if (MagicActionIDs.MU_ADD_CHENMO == id)
					{
						BufferItemType = BufferItemTypes.TimeRANSHAONoShow;
					}
					if (!(obj is GameClient) || !(obj as GameClient).buffManager.IsBuffEnabled(116))
					{
						int nSkillLevel = skillLevel + 1;
						long period = 1000L;
						int execCount = (int)actionParams[1];
						double secs = actionParams[1];
						double percent4 = actionParams[0];
						int extInjured = 0;
						int injure = 0;
						int burst = 0;
						int attackType;
						if (self is GameClient)
						{
							attackType = Global.GetAttackType(self as GameClient);
							if (0 == attackType)
							{
								if (obj is GameClient)
								{
									RoleAlgorithm.AttackEnemy(self as GameClient, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out injure, false, percent4, extInjured, skillid, shenShiInjurePercent);
								}
								else if (obj is Monster)
								{
									RoleAlgorithm.AttackEnemy(self as GameClient, obj as Monster, false, 1.0, 0, 1.0, 0, 0, out burst, out injure, false, percent4, extInjured, skillid, shenShiInjurePercent);
								}
							}
							else if (1 == attackType || 2 == attackType)
							{
								if (obj is GameClient)
								{
									RoleAlgorithm.MAttackEnemy(self as GameClient, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out injure, false, percent4, extInjured, skillid, shenShiInjurePercent);
								}
								else if (obj is Monster)
								{
									RoleAlgorithm.MAttackEnemy(self as GameClient, obj as Monster, false, 1.0, 0, 1.0, 0, 0, out burst, out injure, false, percent4, extInjured, skillid, shenShiInjurePercent);
								}
							}
							int objectID = (self as GameClient).ClientData.RoleID;
							newActionParams[0] = secs;
							newActionParams[1] = 1.0;
							newActionParams[2] = (double)injure;
							newActionParams[3] = (double)objectID;
						}
						attackType = -1;
						if (self is GameClient)
						{
							if (obj is GameClient)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, burst, injure, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, true, false, percent4, 0, 0, 0, 0.0);
							}
							else if (obj is Monster)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, burst, injure, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, true, percent4, 0, 0, 0, 0.0);
							}
							else if (obj is BiaoCheItem)
							{
								BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, burst, injure, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, percent4, 0, 0);
							}
							else if (obj is JunQiItem)
							{
								JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, burst, injure, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, percent4, 0, 0);
							}
							else if (obj is FakeRoleItem)
							{
								FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, burst, injure, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, percent4, 0, 0);
							}
						}
						else if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, burst, injure, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, true, percent4, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, burst, injure, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, true, percent4, 0, 0, 0, 0.0);
						}
						else if (!(obj is BiaoCheItem))
						{
							if (obj is JunQiItem)
							{
								JunQiManager.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as JunQiItem, burst, injure, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, percent4, 0, 0);
							}
						}
						if (injure > 0)
						{
							if (obj is GameClient)
							{
								Global.UpdateBufferData(obj as GameClient, BufferItemType, newActionParams, 1, true);
								MagicAction.ChenMoContextData contextData = new MagicAction.ChenMoContextData
								{
									Attacker = self.GetObjectID(),
									Self = obj,
									Injure = (int)newActionParams[2],
									Occupation = (obj as GameClient).ClientData.Occupation,
									Stop = false,
									percent = percent4
								};
								(obj as GameClient).TimedActionMgr.AddItem(TimeUtil.NOW() + period, period, execCount, 0, new Action<long, object>(MagicAction.ChenMoActionProc), contextData);
								(obj as GameClient).ClientData.ZhongDuStart = TimeUtil.NOW();
								(obj as GameClient).ClientData.ZhongDuSeconds = (int)secs;
								GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 1, TimeUtil.NOW(), (int)secs, 0.0);
							}
							else if (obj is Monster)
							{
								Global.UpdateMonsterBufferData(obj as Monster, BufferItemType, newActionParams);
								MagicAction.ChenMoContextData contextData = new MagicAction.ChenMoContextData
								{
									Attacker = self.GetObjectID(),
									Self = obj,
									Injure = (int)newActionParams[2],
									Stop = false,
									percent = percent4
								};
								(obj as Monster).TimedActionMgr.AddItem(TimeUtil.NOW() + period, period, execCount, 0, new Action<long, object>(MagicAction.ChenMoActionProc), contextData);
								GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as Monster, 1, TimeUtil.NOW(), (int)secs, 0.0);
							}
						}
					}
					break;
				}
				case MagicActionIDs.HUIFU:
					if (actionParams.Length >= 2)
					{
						if (self is GameClient)
						{
							GameClient client = self as GameClient;
							int addVal = (int)((double)client.ClientData.LifeV * actionParams[0] + actionParams[1]);
							client.ClientData.CurrentLifeV += addVal;
							if (client.ClientData.CurrentLifeV > client.ClientData.LifeV)
							{
								client.ClientData.CurrentLifeV = client.ClientData.LifeV;
							}
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						}
					}
					break;
				case MagicActionIDs.MU_ADD_YISHANG:
					if (actionParams.Length >= 3)
					{
						if (Global.GetRandom() <= actionParams[0])
						{
							double[] newActionParams = new double[2];
							BufferItemTypes BufferItemType = BufferItemTypes.ZuZhou;
							newActionParams[0] = actionParams[2];
							newActionParams[1] = actionParams[1] * 1000.0;
							if (obj is GameClient)
							{
								Global.UpdateBufferData(obj as GameClient, BufferItemType, newActionParams, 1, true);
							}
							else if (obj is Monster)
							{
								Global.UpdateMonsterBufferData(obj as Monster, BufferItemType, newActionParams);
							}
						}
					}
					break;
				case MagicActionIDs.NODIE:
				{
					double[] newActionParams = new double[]
					{
						actionParams[1],
						actionParams[0]
					};
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						client.buffManager.SetStatusBuff(120, TimeUtil.NOW(), (long)(1000 * (int)newActionParams[0]), (long)((int)newActionParams[1]));
						client.buffManager.UpdateImmediately(self as GameClient, 120, TimeUtil.NOW());
					}
					break;
				}
				case MagicActionIDs.GOTO_ANGELTEMPLE:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						if (client.ClientData.ChangeLifeCount < GameManager.AngelTempleMgr.m_AngelTempleData.MinChangeLifeNum)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(459, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
						}
						else if (client.ClientData.ChangeLifeCount == GameManager.AngelTempleMgr.m_AngelTempleData.MinChangeLifeNum)
						{
							if (client.ClientData.Level < GameManager.AngelTempleMgr.m_AngelTempleData.MinLevel)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(460, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
							}
						}
						if (!GameManager.AngelTempleMgr.CanEnterAngelTempleOnTime())
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(461, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						else if (GameManager.AngelTempleMgr.m_AngelTempleScene.m_eStatus == AngelTempleStatus.FIGHT_STATUS_END)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(462, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						else if (Interlocked.Increment(ref GameManager.AngelTempleMgr.m_AngelTempleScene.m_nPlarerCount) > GameManager.AngelTempleMgr.m_AngelTempleData.MaxPlayerNum)
						{
							Interlocked.Decrement(ref GameManager.AngelTempleMgr.m_AngelTempleScene.m_nPlarerCount);
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(463, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						else
						{
							client.ClientData.bIsInAngelTempleMap = true;
							GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GameManager.AngelTempleMgr.m_AngelTempleData.MapCode, -1, -1, -1, 0);
							int nDate = TimeUtil.NowDateTime().DayOfYear;
							int nType = 5;
							int nCount = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, nDate, nType);
							nCount++;
							Global.UpdateDayActivityEnterCountToDB(client, client.ClientData.RoleID, nDate, nType, nCount);
							LogManager.WriteLog(LogTypes.Info, string.Format("{0} enter AngelTemple count={1} time={2}", client.ClientData.RoleID, nCount, TimeUtil.NowDateTime().ToLongDateString()), null, true);
						}
					}
					break;
				case MagicActionIDs.OPEN_TREASURE_BOX:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						double dGoodsID = actionParams[0];
						double dFallGoodsPackID = actionParams[1];
						double dNum = actionParams[2];
						GoodsData goods = Global.GetGoodsByID(client, (int)dGoodsID);
						if (goods != null)
						{
							int nCategories = Global.GetGoodsCatetoriy(goods.GoodsID);
							if (nCategories == 701)
							{
								int i = 0;
								while ((double)i < dNum)
								{
									GoodsBaoXiang.CreateGoodsBaseFallID(client, (int)dFallGoodsPackID, (int)dNum);
									i++;
								}
							}
						}
					}
					break;
				case MagicActionIDs.GOTO_BOOSZHIJIA:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						if (client != null)
						{
							if (client.ClientData.VipLevel < Data.BosshomeData.VIPLevLimit)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(464, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
							}
							else if (client.ClientData.ChangeLifeCount < Data.BosshomeData.MinChangeLifeLimit)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(465, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
							}
							else if (client.ClientData.ChangeLifeCount > Data.BosshomeData.MaxChangeLifeLimit)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(466, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
							}
							else
							{
								if (client.ClientData.ChangeLifeCount == GameManager.AngelTempleMgr.m_AngelTempleData.MinChangeLifeNum)
								{
									if (client.ClientData.Level < Data.BosshomeData.MinLevel)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(467, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
										break;
									}
									if (client.ClientData.Level > Data.BosshomeData.MaxLevel)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(468, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
										break;
									}
								}
								if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, Data.BosshomeData.EnterNeedDiamond, "进入BOSS之家", true, true, false, DaiBiSySType.None))
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(469, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 30);
								}
								else
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, Data.BosshomeData.MapID, -1, -1, -1, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.GOTO_HUANGJINSHENGDIAN:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						if (client != null)
						{
							if (client.ClientData.VipLevel < Data.GoldtempleData.VIPLevLimit)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(470, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
							}
							else if (client.ClientData.ChangeLifeCount < Data.GoldtempleData.MinChangeLifeLimit)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(471, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
							}
							else
							{
								if (client.ClientData.ChangeLifeCount == Data.GoldtempleData.MinChangeLifeLimit)
								{
									if (client.ClientData.Level < Data.GoldtempleData.MinLevel)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(472, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
										break;
									}
									if (client.ClientData.Level > Data.GoldtempleData.MaxLevel)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(473, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
										break;
									}
								}
								if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, Data.GoldtempleData.EnterNeedDiamond, "进入火龙突袭", true, true, false, DaiBiSySType.None))
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(474, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 30);
								}
								else
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, Data.GoldtempleData.MapID, -1, -1, -1, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.ADD_VIPEXP:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						int nAddValue2 = (int)actionParams[0];
						if (nAddValue2 > 0)
						{
							nAddValue2 += Global.GetRoleParamsInt32FromDB(client, "VIPExp");
							Global.SaveRoleParamsInt32ValueToDB(client, "VIPExp", nAddValue2, true);
							Global.ProcessVipLevelUp(client);
						}
					}
					break;
				case MagicActionIDs.GET_AWARD_BLOODCASTLECOPYSCENE:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						BloodCastleDataInfo bcDataTmp = null;
						if (client.ClientData.FuBenSeqID >= 0 && client.ClientData.CopyMapID >= 0 && Data.BloodCastleDataInfoList.TryGetValue(client.ClientData.FuBenID, out bcDataTmp) && bcDataTmp != null)
						{
							CopyMap cmInfo = GameManager.BloodCastleCopySceneMgr.GetBloodCastleCopySceneInfo(client.ClientData.FuBenSeqID);
							if (cmInfo != null)
							{
								int nSceneID = Global.GetRoleParamsInt32FromDB(client, "BloodCastleSceneid");
								BloodCastleScene bcTmp = GameManager.BloodCastleCopySceneMgr.GetBloodCastleCopySceneDataInfo(cmInfo, client.ClientData.FuBenSeqID, nSceneID);
								if (bcTmp != null)
								{
									if (bcTmp.m_eStatus == BloodCastleStatus.FIGHT_STATUS_END)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(458, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
									}
									else
									{
										bcTmp.m_nRoleID = client.ClientData.RoleID;
										GoodsData goodsData = Global.GetGoodsByID(client, 10000);
										if (goodsData == null)
										{
											goodsData = Global.GetGoodsByID(client, 10001);
										}
										if (goodsData == null)
										{
											goodsData = Global.GetGoodsByID(client, 10002);
										}
										if (goodsData != null)
										{
											GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsData, 1, false, false);
											string[] sItem = bcDataTmp.AwardItem1;
											if (sItem != null && sItem.Length > 0)
											{
												for (int i = 0; i < sItem.Length; i++)
												{
													if (!string.IsNullOrEmpty(sItem[i].Trim()))
													{
														string[] sFields = sItem[i].Split(new char[]
														{
															','
														});
														if (!string.IsNullOrEmpty(sFields[i].Trim()))
														{
															int nGoodsID = Convert.ToInt32(sFields[0].Trim());
															int nGoodsNum = Convert.ToInt32(sFields[1].Trim());
															int nBinding = Convert.ToInt32(sFields[2].Trim());
															GoodsData goods = new GoodsData
															{
																Id = -1,
																GoodsID = nGoodsID,
																Using = 0,
																Forge_level = 0,
																Starttime = "1900-01-01 12:00:00",
																Endtime = "1900-01-01 12:00:00",
																Site = 0,
																Quality = 0,
																Props = "",
																GCount = nGoodsNum,
																Binding = nBinding,
																Jewellist = "",
																BagIndex = 0,
																AddPropIndex = 0,
																BornIndex = 0,
																Lucky = 0,
																Strong = 0,
																ExcellenceInfo = 0,
																AppendPropLev = 0,
																ChangeLifeLevForEquip = 0
															};
															string sMsg = GLang.GetLang(23, new object[0]);
															if (!Global.CanAddGoodsNum(client, nGoodsNum))
															{
																Global.UseMailGivePlayerAward(client, goods, GLang.GetLang(1, new object[0]), sMsg, 1.0);
															}
															else
															{
																Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, nGoodsID, nGoodsNum, 0, "", 0, 0, 0, "", true, 1, sMsg, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
															}
														}
													}
												}
											}
											bcTmp.m_eStatus = BloodCastleStatus.FIGHT_STATUS_END;
											bcTmp.m_lEndTime = TimeUtil.NOW();
											bcTmp.m_bIsFinishTask = true;
											GameManager.BloodCastleCopySceneMgr.CompleteBloodCastScene(client, bcTmp, bcDataTmp);
										}
									}
								}
							}
						}
					}
					break;
				case MagicActionIDs.ADDMONSTERSKILL:
					if (self is Monster)
					{
						int monsterID = (int)actionParams[0];
						int skillID = (int)actionParams[1];
						int skillPriority = (int)actionParams[2];
						List<object> objsList = GameManager.MonsterMgr.FindMonsterByExtensionID((self as Monster).CopyMapID, monsterID);
						for (int i = 0; i < objsList.Count; i++)
						{
							(objsList[i] as Monster).AddDynSkillID(skillID, skillPriority);
						}
						Debug.WriteLine(string.Format("Boss AI, Add monster skill, MonsterID={0}, SkillID={1}, Priority={2}", monsterID, skillid, skillPriority));
					}
					break;
				case MagicActionIDs.REMOVEMONSTERSKILL:
					if (self is Monster)
					{
						int monsterID = (int)actionParams[0];
						int skillID = (int)actionParams[1];
						List<object> objsList = GameManager.MonsterMgr.FindMonsterByExtensionID((self as Monster).CopyMapID, monsterID);
						for (int i = 0; i < objsList.Count; i++)
						{
							(objsList[i] as Monster).RemoveDynSkill(skillID);
						}
						Debug.WriteLine(string.Format("Boss AI, Remove monster skill, MonsterID={0}, SkillID={1}", monsterID, skillid));
					}
					break;
				case MagicActionIDs.BOSS_CALLMONSTERONE:
					if (self is Monster)
					{
						int monsterID = (int)actionParams[0];
						int addNum = (int)actionParams[1];
						int radius = (int)actionParams[2];
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue((self as Monster).CurrentMapCode, out gameMap))
						{
							Point grid = (self as Monster).CurrentGrid;
							radius = (radius - 1) / gameMap.MapGridWidth + 1;
							GameManager.MonsterZoneMgr.AddDynamicMonsters((self as Monster).CurrentMapCode, monsterID, (self as Monster).CopyMapID, addNum, (int)grid.X, (int)grid.Y, radius, 0, SceneUIClasses.Normal, null, null);
							Debug.WriteLine(string.Format("Boss AI, Call monster one, MonsterID={0}, AddNum={1}, Radius={2}, Grid={3}", new object[]
							{
								monsterID,
								addNum,
								radius,
								grid
							}));
						}
					}
					break;
				case MagicActionIDs.BOSS_CALLMONSTERTWO:
					if (self is Monster)
					{
						int monsterID = (int)actionParams[0];
						int addNum = (int)actionParams[1];
						int posX = (int)actionParams[2];
						int posY = (int)actionParams[3];
						int radius = (int)actionParams[4];
						if (actionParams.Length >= 6)
						{
							int pursuitRadius = (int)actionParams[5];
						}
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue((self as Monster).CurrentMapCode, out gameMap))
						{
							Point grid = new Point((double)gameMap.CorrectPointToGrid(posX), (double)gameMap.CorrectPointToGrid(posY));
							radius = (radius - 1) / gameMap.MapGridWidth + 1;
							GameManager.MonsterZoneMgr.AddDynamicMonsters((self as Monster).CurrentMapCode, monsterID, (self as Monster).CopyMapID, addNum, (int)grid.X, (int)grid.Y, radius, 0, SceneUIClasses.Normal, null, null);
							Debug.WriteLine(string.Format("Boss AI, Call monster two, MonsterID={0}, AddNum={1}, Radius={2}, Grid={3}", new object[]
							{
								monsterID,
								addNum,
								radius,
								grid
							}));
						}
					}
					break;
				case MagicActionIDs.CLEAR_MONSTER_BUFFERID:
					if (self is Monster)
					{
						int monsterID = (int)actionParams[0];
						int bufferID = (int)actionParams[1];
						List<object> monstersList = GameManager.MonsterMgr.FindMonsterByExtensionID((self as Monster).CurrentCopyMapID, monsterID);
						if (monstersList != null && monstersList.Count > 0)
						{
							for (int i = 0; i < monstersList.Count; i++)
							{
								Global.RemoveMonsterBufferData(monstersList[i] as Monster, bufferID);
							}
						}
					}
					break;
				case MagicActionIDs.UP_LEVEL:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						int nLev = 0;
						int nAddValue2 = (int)actionParams[0];
						bool bCanUp = true;
						if (nAddValue2 > 0)
						{
							if (client.ClientData.ChangeLifeCount > GameManager.ChangeLifeMgr.m_MaxChangeLifeCount)
							{
								bCanUp = false;
							}
							else if (client.ClientData.ChangeLifeCount == GameManager.ChangeLifeMgr.m_MaxChangeLifeCount)
							{
								ChangeLifeDataInfo infoTmp = GameManager.ChangeLifeMgr.GetChangeLifeDataInfo(client, 0);
								if (infoTmp == null)
								{
									bCanUp = false;
								}
								else
								{
									nLev = infoTmp.NeedLevel;
									if (client.ClientData.Level >= nLev)
									{
										bCanUp = false;
									}
								}
							}
							else
							{
								ChangeLifeDataInfo infoTmp = GameManager.ChangeLifeMgr.GetChangeLifeDataInfo(client, client.ClientData.ChangeLifeCount + 1);
								if (infoTmp == null)
								{
									bCanUp = false;
								}
								else
								{
									nLev = infoTmp.NeedLevel;
									if (client.ClientData.Level >= nLev)
									{
										bCanUp = false;
									}
								}
							}
							if (!bCanUp)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(64, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
								return false;
							}
							if (client.ClientData.Level + nAddValue2 > nLev)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(65, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
								return false;
							}
							for (int i = 0; i < nAddValue2; i++)
							{
								GameManager.ClientMgr.ProcessRoleExperience(client, Global.GetCurrentLevelUpNeedExp(client), true, true, false, "none");
							}
						}
					}
					break;
				case MagicActionIDs.ADD_GUANGMUI:
					if (self is Monster)
					{
						int guangMuID = (int)actionParams[0];
						int mapCode = (int)actionParams[1];
						Monster monster = self as Monster;
						if (null != monster)
						{
							if (Global.GetMapSceneType(monster.CurrentMapCode) == SceneUIClasses.LuoLanChengZhan)
							{
								LuoLanChengZhanManager.getInstance().AddGuangMuEvent(guangMuID, 1);
								GameManager.ClientMgr.BroadSpecialMapAIEvent(mapCode, self.CurrentCopyMapID, guangMuID, 1);
							}
							else
							{
								CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(self.CurrentCopyMapID);
								if (null != copyMap)
								{
									copyMap.AddGuangMuEvent(guangMuID, 1);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(mapCode, self.CurrentCopyMapID, guangMuID, 1);
								}
							}
						}
					}
					break;
				case MagicActionIDs.CLEAR_GUANGMUI:
					if (self is Monster)
					{
						int guangMuID = (int)actionParams[0];
						int mapCode = (int)actionParams[1];
						Monster monster = self as Monster;
						if (null != monster)
						{
							if (Global.GetMapSceneType(monster.CurrentMapCode) == SceneUIClasses.LuoLanChengZhan)
							{
								LuoLanChengZhanManager.getInstance().AddGuangMuEvent(guangMuID, 0);
								GameManager.ClientMgr.BroadSpecialMapAIEvent(mapCode, self.CurrentCopyMapID, guangMuID, 0);
							}
							else
							{
								CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(self.CurrentCopyMapID);
								if (null != copyMap)
								{
									copyMap.AddGuangMuEvent(guangMuID, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(mapCode, self.CurrentCopyMapID, guangMuID, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.FEIXUE:
				case MagicActionIDs.ZHONGDU:
				case MagicActionIDs.LINGHUN:
				case MagicActionIDs.RANSHAO:
				{
					double[] newActionParams = new double[4];
					BufferItemTypes BufferItemType = BufferItemTypes.None;
					if (MagicActionIDs.FEIXUE == id)
					{
						BufferItemType = BufferItemTypes.TimeFEIXUENoShow;
					}
					else if (MagicActionIDs.ZHONGDU == id)
					{
						BufferItemType = BufferItemTypes.TimeZHONGDUNoShow;
					}
					else if (MagicActionIDs.LINGHUN == id)
					{
						BufferItemType = BufferItemTypes.TimeLINGHUNoShow;
					}
					else if (MagicActionIDs.RANSHAO == id)
					{
						BufferItemType = BufferItemTypes.TimeRANSHAONoShow;
					}
					int extInjured = 0;
					int nSkillLevel = skillLevel + 1;
					long period = (long)(actionParams[0] * 1000.0);
					int execCount = (int)actionParams[1];
					double secs = actionParams[0] * actionParams[1];
					double percent4 = actionParams[2] + actionParams[2] / 200.0 * (double)nSkillLevel;
					if (id != MagicActionIDs.RANSHAO)
					{
						extInjured = (int)actionParams[3] + (int)actionParams[3] * nSkillLevel;
					}
					int injure = 0;
					int burst = 0;
					int attackType;
					if (self is GameClient)
					{
						attackType = Global.GetAttackType(self as GameClient);
						if (id != MagicActionIDs.RANSHAO)
						{
							if (0 == attackType)
							{
								if (obj is GameClient)
								{
									RoleAlgorithm.AttackEnemy(self as GameClient, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out injure, false, percent4, extInjured, skillid, shenShiInjurePercent);
								}
								else if (obj is Monster)
								{
									RoleAlgorithm.AttackEnemy(self as GameClient, obj as Monster, false, 1.0, 0, 1.0, 0, 0, out burst, out injure, false, percent4, extInjured, skillid, shenShiInjurePercent);
								}
							}
							else if (1 == attackType || 2 == attackType)
							{
								if (obj is GameClient)
								{
									RoleAlgorithm.MAttackEnemy(self as GameClient, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out injure, false, percent4, extInjured, skillid, shenShiInjurePercent);
								}
								else if (obj is Monster)
								{
									RoleAlgorithm.MAttackEnemy(self as GameClient, obj as Monster, false, 1.0, 0, 1.0, 0, 0, out burst, out injure, false, percent4, extInjured, skillid, shenShiInjurePercent);
								}
							}
						}
						else if (obj is GameClient)
						{
							injure = (int)((double)(obj as GameClient).ClientData.LifeV * percent4);
						}
						else if (obj is Monster)
						{
							injure = (int)((obj as Monster).MonsterInfo.VLifeMax * percent4);
						}
						int objectID = (self as GameClient).ClientData.RoleID;
						newActionParams[0] = secs;
						newActionParams[1] = actionParams[0];
						newActionParams[2] = (double)injure;
						newActionParams[3] = (double)objectID;
					}
					else if (self is Monster)
					{
						attackType = (self as Monster).MonsterInfo.ToOccupation;
						if (id != MagicActionIDs.RANSHAO)
						{
							if (attackType == 0 || 2 == attackType)
							{
								if (obj is GameClient)
								{
									RoleAlgorithm.AttackEnemy(self as Monster, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out injure, false, percent4, extInjured, skillid, shenShiInjurePercent);
								}
								else if (obj is Monster)
								{
									RoleAlgorithm.AttackEnemy(self as Monster, obj as Monster, false, 1.0, 0, 1.0, 0, 0, out burst, out injure, false, percent4, extInjured, skillid, shenShiInjurePercent);
								}
							}
							else if (1 == attackType || (5 == attackType && skillid == 11006))
							{
								if (obj is GameClient)
								{
									RoleAlgorithm.MAttackEnemy(self as Monster, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out injure, false, percent4, extInjured, skillid, shenShiInjurePercent);
								}
								else if (obj is Monster)
								{
									RoleAlgorithm.MAttackEnemy(self as Monster, obj as Monster, false, 1.0, 0, 1.0, 0, 0, out burst, out injure, false, percent4, extInjured, skillid, shenShiInjurePercent);
								}
							}
						}
						else if (obj is GameClient)
						{
							injure = (int)((double)(obj as GameClient).ClientData.LifeV * percent4);
						}
						else if (obj is Monster)
						{
							injure = (int)((obj as Monster).MonsterInfo.VLifeMax * percent4);
						}
						int objectID = (self as Monster).RoleID;
						newActionParams[0] = secs;
						newActionParams[1] = actionParams[0];
						newActionParams[2] = (double)injure;
						newActionParams[3] = (double)objectID;
					}
					attackType = -1;
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, burst, injure, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, true, false, percent4, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, burst, injure, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, true, percent4, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, burst, injure, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, percent4, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, burst, injure, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, percent4, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, burst, injure, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, percent4, 0, 0);
						}
					}
					else if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, burst, injure, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, true, percent4, 0, 0, 0, 0.0);
					}
					else if (obj is Monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, burst, injure, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, true, percent4, 0, 0, 0, 0.0);
					}
					else if (!(obj is BiaoCheItem))
					{
						if (obj is JunQiItem)
						{
							JunQiManager.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as JunQiItem, burst, injure, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					if (injure > 0)
					{
						if (obj is GameClient)
						{
							Global.UpdateBufferData(obj as GameClient, BufferItemType, newActionParams, 1, true);
							MagicAction.ZhongDuContextData contextData2 = new MagicAction.ZhongDuContextData
							{
								Attacker = self.GetObjectID(),
								Self = obj,
								Injure = (int)newActionParams[2],
								Occupation = (obj as GameClient).ClientData.Occupation,
								percent = percent4
							};
							(obj as GameClient).TimedActionMgr.AddItem(TimeUtil.NOW() + period, period, execCount, 0, new Action<long, object>(MagicAction.ZhongDuActionProc), contextData2);
							(obj as GameClient).ClientData.ZhongDuStart = TimeUtil.NOW();
							(obj as GameClient).ClientData.ZhongDuSeconds = (int)secs;
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 1, TimeUtil.NOW(), (int)secs, 0.0);
						}
						else if (obj is Monster)
						{
							Global.UpdateMonsterBufferData(obj as Monster, BufferItemType, newActionParams);
							MagicAction.ZhongDuContextData contextData2 = new MagicAction.ZhongDuContextData
							{
								Attacker = self.GetObjectID(),
								Self = obj,
								Injure = (int)newActionParams[2],
								percent = percent4
							};
							(obj as Monster).TimedActionMgr.AddItem(TimeUtil.NOW() + period, period, execCount, 0, new Action<long, object>(MagicAction.ZhongDuActionProc), contextData2);
							GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as Monster, 1, TimeUtil.NOW(), (int)secs, 0.0);
						}
					}
					break;
				}
				case MagicActionIDs.HUZHAO:
				{
					BufferItemTypes BufferItemType = BufferItemTypes.TimeHUZHAONoShow;
					double[] newActionParams = new double[]
					{
						actionParams[1]
					};
					if (obj is GameClient)
					{
						Global.UpdateBufferData(obj as GameClient, BufferItemType, newActionParams, 1, true);
						(obj as GameClient).MyBufferExtManager.AddBufferItem((int)BufferItemType, new HuZhaoBufferItem
						{
							InjuredV = (int)actionParams[0],
							MaxLifeV = (int)actionParams[1],
							RecoverLifePercent = actionParams[2]
						});
					}
					else if (obj is Monster)
					{
						Global.UpdateMonsterBufferData(obj as Monster, BufferItemType, newActionParams);
						(obj as Monster).MyBufferExtManager.AddBufferItem((int)BufferItemType, new HuZhaoBufferItem
						{
							InjuredV = (int)actionParams[0],
							MaxLifeV = (int)actionParams[1],
							RecoverLifePercent = actionParams[2]
						});
					}
					break;
				}
				case MagicActionIDs.WUDIHUZHAO:
				{
					BufferItemTypes BufferItemType = BufferItemTypes.TimeWUDIHUZHAONoShow;
					double[] newActionParams = new double[]
					{
						actionParams[0]
					};
					if (obj is GameClient)
					{
						Global.UpdateBufferData(obj as GameClient, BufferItemType, newActionParams, 1, true);
					}
					else if (obj is Monster)
					{
						Global.UpdateMonsterBufferData(obj as Monster, BufferItemType, newActionParams);
					}
					break;
				}
				case MagicActionIDs.MU_FIRE_WALL1:
				case MagicActionIDs.MU_FIRE_WALL9:
				case MagicActionIDs.MU_FIRE_WALL25:
				{
					double[] newActionParams = new double[5];
					int attackerID = 0;
					if (self is GameClient)
					{
						attackerID = (self as GameClient).ClientData.RoleID;
					}
					else if (self is Monster)
					{
						Monster monster = self as Monster;
						attackerID = monster.RoleID;
					}
					newActionParams[0] = actionParams[0] * actionParams[1];
					newActionParams[1] = actionParams[1];
					newActionParams[2] = actionParams[3];
					newActionParams[3] = (double)attackerID;
					newActionParams[4] = actionParams[2];
					int gridNum = 0;
					if (id != MagicActionIDs.MU_FIRE_WALL1)
					{
						if (id == MagicActionIDs.MU_FIRE_WALL9)
						{
							gridNum = 1;
						}
						else if (id == MagicActionIDs.MU_FIRE_WALL25)
						{
							gridNum = 2;
						}
					}
					GameMap gameMap = GameManager.MapMgr.DictMaps[self.CurrentMapCode];
					int gridX = targetX / gameMap.MapGridWidth;
					int gridY = targetY / gameMap.MapGridHeight;
					if (gridX > 0 && gridY > 0)
					{
						GameManager.GridMagicHelperMgr.AddMagicHelper(id, newActionParams, self.CurrentMapCode, new Point((double)gridX, (double)gridY), gridNum, gridNum, self.CurrentCopyMapID);
					}
					break;
				}
				case MagicActionIDs.MU_FIRE_WALL_X:
				case MagicActionIDs.MU_FIRE_SECTOR:
				case MagicActionIDs.MU_FIRE_STRAIGHT:
				{
					double[] newActionParams = new double[16];
					int attackerID = 0;
					if (self is GameClient)
					{
						attackerID = (self as GameClient).ClientData.RoleID;
					}
					else if (self is Monster)
					{
						Monster monster = self as Monster;
						attackerID = monster.RoleID;
					}
					int nSkillLevel = skillLevel + 1;
					newActionParams[0] = actionParams[0] * actionParams[1];
					newActionParams[1] = actionParams[1];
					newActionParams[2] = actionParams[3] * (double)(1 + nSkillLevel);
					newActionParams[3] = (double)attackerID;
					newActionParams[4] = actionParams[2] * (1.0 + (double)nSkillLevel / 200.0);
					if (id == MagicActionIDs.MU_FIRE_WALL_X)
					{
						newActionParams[5] = actionParams[4];
					}
					else if (id == MagicActionIDs.MU_FIRE_SECTOR)
					{
						newActionParams[5] = actionParams[4];
						newActionParams[6] = actionParams[5];
						newActionParams[7] = (double)self.CurrentDir;
					}
					else if (id == MagicActionIDs.MU_FIRE_STRAIGHT)
					{
						newActionParams[5] = actionParams[4];
						newActionParams[6] = actionParams[5];
						newActionParams[7] = (double)targetX;
						newActionParams[8] = (double)targetY;
					}
					GameMap gameMap = GameManager.MapMgr.DictMaps[self.CurrentMapCode];
					int gridX = targetX / gameMap.MapGridWidth;
					int gridY = targetY / gameMap.MapGridHeight;
					newActionParams[15] = (double)gameMap.MapGridWidth;
					if (gridX > 0 && gridY > 0)
					{
						GameManager.GridMagicHelperMgrEx.AddMagicHelperEx(id, newActionParams, self.CurrentMapCode, gridX, gridY, self.CurrentCopyMapID);
					}
					break;
				}
				case MagicActionIDs.MU_FIRE_WALL_Y:
				{
					double[] newActionParams = new double[17];
					int attackerID = 0;
					if (self is GameClient)
					{
						attackerID = (self as GameClient).ClientData.RoleID;
					}
					else if (self is Monster)
					{
						Monster monster = self as Monster;
						attackerID = monster.RoleID;
					}
					int nSkillLevel = skillLevel + 1;
					newActionParams[0] = actionParams[0];
					newActionParams[1] = actionParams[1];
					newActionParams[2] = actionParams[3] * (double)(1 + nSkillLevel);
					newActionParams[3] = (double)attackerID;
					newActionParams[4] = actionParams[2] * (1.0 + (double)nSkillLevel / 200.0);
					Array.Copy(actionParams, 4, newActionParams, 5, actionParams.Length - 4);
					int DelayDecoration = 0;
					SystemXmlItem systemMagic = null;
					if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(skillid, out systemMagic))
					{
						DelayDecoration = systemMagic.GetIntValue("DelayDecoration", -1);
					}
					GameMap gameMap = GameManager.MapMgr.DictMaps[self.CurrentMapCode];
					int gridX = targetX / gameMap.MapGridWidth;
					int gridY = targetY / gameMap.MapGridHeight;
					newActionParams[15] = (double)gameMap.MapGridWidth;
					newActionParams[16] = (double)skillid;
					if (gridX > 0 && gridY > 0)
					{
						double DecorationTime = newActionParams[0] * newActionParams[1];
						GameManager.GridMagicHelperMgrEx.AddGridMagic(id, newActionParams, self.CurrentMapCode, gridX, gridY, DelayDecoration, (int)DecorationTime, self.CurrentCopyMapID, MagicAction.MaxHitNum);
					}
					break;
				}
				case MagicActionIDs.MU_FIRE_WALL_ACTION:
				{
					double[] newActionParams = new double[actionParams.Length + 1];
					int attackerID = 0;
					if (self is GameClient)
					{
						attackerID = (self as GameClient).ClientData.RoleID;
					}
					else if (self is Monster)
					{
						Monster monster = self as Monster;
						attackerID = monster.RoleID;
					}
					newActionParams[0] = actionParams[0] * actionParams[1];
					newActionParams[1] = actionParams[1];
					int gridNumX = (int)actionParams[2];
					int gridNumY = (int)actionParams[3];
					newActionParams[2] = (double)attackerID;
					Array.Copy(actionParams, 4, newActionParams, 3, actionParams.Length - 4);
					GameMap gameMap = GameManager.MapMgr.DictMaps[self.CurrentMapCode];
					int gridX = targetX / gameMap.MapGridWidth;
					int gridY = targetY / gameMap.MapGridHeight;
					if (gridX > 0 && gridY > 0)
					{
						GameManager.GridMagicHelperMgrEx.AddMagicHelperExAction(id, newActionParams, self.CurrentMapCode, new Point((double)gridX, (double)gridY), gridNumX, gridNumY, self.CurrentCopyMapID);
					}
					break;
				}
				case MagicActionIDs.BOSS_ADDANIMATION:
				{
					int mapCode = self.CurrentMapCode;
					int copyMapID = self.CurrentCopyMapID;
					int bossID = (int)actionParams[0];
					int animationID = (int)actionParams[2];
					int toX = (int)actionParams[3];
					int toY = (int)actionParams[4];
					int effectX = (int)actionParams[5];
					int effectY = (int)actionParams[6];
					long ticks = TimeUtil.NOW() / 10000L;
					int checkCode = Global.GetBossAnimationCheckCode(bossID, mapCode, toX, toY, effectX, effectY, ticks);
					string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
					{
						-1,
						bossID,
						mapCode,
						toX,
						toY,
						effectX,
						effectY,
						ticks,
						checkCode
					});
					GameManager.ClientMgr.BroadSpecialMapMessage(639, strcmd, mapCode, copyMapID);
					break;
				}
				case MagicActionIDs.MU_ADD_MOVE_SPEED_DOWN:
				{
					double dMoveSpeedValue = actionParams[0];
					double dTime = actionParams[1];
					long ToTick = TimeUtil.NOW() * 10000L + (long)dTime * 1000L * 10000L;
					if (obj is GameClient)
					{
						GameClient targetClient = obj as GameClient;
						if (targetClient != null)
						{
							if (!targetClient.buffManager.IsBuffEnabled(116) && !targetClient.buffManager.IsBuffEnabled(113))
							{
								if (!CaiJiLogic.IsCaiJiState(targetClient))
								{
									ZuoQiManager.getInstance().RoleDisMount(targetClient, true);
									targetClient.RoleBuffer.AddTempExtProp(2, -dMoveSpeedValue, ToTick);
									targetClient.buffManager.SetStatusBuff(118, TimeUtil.NOW(), (long)dTime * 1000L, 0L);
									double moveCost = RoleAlgorithm.GetMoveSpeed(targetClient);
									targetClient.ClientData.MoveSpeed = moveCost;
									GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, targetClient, 4, TimeUtil.NOW(), (int)dTime, moveCost);
								}
							}
						}
					}
					else if (obj is Monster)
					{
						Monster monster = obj as Monster;
						if (null != monster)
						{
							monster.TempPropsBuffer.AddTempExtProp(2, -dMoveSpeedValue, ToTick);
							monster.SpeedDownStart = TimeUtil.NOW();
							monster.SpeedDownSeconds = (int)dTime;
							GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 4, TimeUtil.NOW(), (int)dTime, monster.MoveSpeed);
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_PALSY:
				{
					double dMoveSpeedValue = actionParams[0];
					double dTime = actionParams[1];
					if (obj is GameClient)
					{
						GameClient targetClient = obj as GameClient;
						if (targetClient != null)
						{
							if (!targetClient.buffManager.IsBuffEnabled(116) && !targetClient.buffManager.IsBuffEnabled(113))
							{
								if (!CaiJiLogic.IsCaiJiState(targetClient))
								{
									ZuoQiManager.getInstance().RoleDisMount(targetClient, true);
									targetClient.ClientData.DongJieStart = TimeUtil.NOW();
									targetClient.ClientData.DongJieSeconds = (int)dTime;
									long ToTick = TimeUtil.NOW() * 10000L + (long)dTime * 1000L * 10000L;
									targetClient.RoleBuffer.AddTempExtProp(2, -dMoveSpeedValue, ToTick);
									targetClient.buffManager.SetStatusBuff(117, TimeUtil.NOW(), (long)dTime * 1000L, 0L);
									double moveCost = RoleAlgorithm.GetMoveSpeed(targetClient);
									targetClient.ClientData.MoveSpeed = moveCost;
									GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, targetClient, 3, targetClient.ClientData.DongJieStart, targetClient.ClientData.DongJieSeconds, moveCost);
								}
							}
						}
					}
					else if (obj is Monster)
					{
						Monster monster = obj as Monster;
						if (null != monster)
						{
							if (101 == monster.MonsterType || 301 == monster.MonsterType || 1501 == monster.MonsterType || 1801 == monster.MonsterType)
							{
								monster.DongJieStart = TimeUtil.NOW();
								monster.DongJieSeconds = (int)dTime;
								monster.XuanYunStart = TimeUtil.NOW();
								monster.XuanYunSeconds = (int)dTime;
								GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 3, monster.DongJieStart, monster.DongJieSeconds, 0.0);
							}
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_FROZEN:
				{
					double dMoveSpeedValue = actionParams[0];
					double dTime = actionParams[1];
					long ToTick = TimeUtil.NOW() * 10000L + (long)dTime * 1000L * 10000L;
					long lStartTick = TimeUtil.NOW();
					if (obj is GameClient)
					{
						GameClient targetClient = obj as GameClient;
						if (targetClient != null)
						{
							if (!targetClient.buffManager.IsBuffEnabled(116) && !targetClient.buffManager.IsBuffEnabled(113))
							{
								if (!CaiJiLogic.IsCaiJiState(targetClient))
								{
									ZuoQiManager.getInstance().RoleDisMount(targetClient, true);
									targetClient.RoleBuffer.AddTempExtProp(47, 1.0, ToTick);
									double moveCost = RoleAlgorithm.GetMoveSpeed(targetClient);
									targetClient.ClientData.MoveSpeed = moveCost;
									GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, targetClient, 8, lStartTick, (int)dTime, moveCost);
								}
							}
						}
					}
					else if (obj is Monster)
					{
						Monster monster = obj as Monster;
						if (null != monster)
						{
							if (101 == monster.MonsterType || 301 == monster.MonsterType || 1501 == monster.MonsterType || 1801 == monster.MonsterType)
							{
								monster.TempPropsBuffer.AddTempExtProp(2, -monster.MoveSpeed, ToTick);
								monster.DingShenStart = TimeUtil.NOW();
								monster.DingShenSeconds = (int)dTime;
								GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 8, lStartTick, (int)dTime, monster.MoveSpeed);
							}
						}
					}
					break;
				}
				case MagicActionIDs.MU_GETSHIZHUANG:
				{
					int nFashionID = (int)actionParams[0];
					FashionManager.getInstance().GetFashionByMagic(self as GameClient, nFashionID, true);
					break;
				}
				case MagicActionIDs.MU_ADD_BATI:
				{
					int nSkillLevel = skillLevel + 1;
					double dTimeMillisecond = actionParams[0];
					if (self is GameClient && nSkillLevel >= 30)
					{
						GameClient selfClient = self as GameClient;
						if (!selfClient.buffManager.IsBuffEnabled(113))
						{
							long ToTick = (long)(dTimeMillisecond * 1.0);
							selfClient.buffManager.SetStatusBuff(113, TimeUtil.NOW(), ToTick, (long)nSkillLevel);
						}
					}
					break;
				}
				case MagicActionIDs.ADD_SHENGWU:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						sbyte sTypeIdx = (sbyte)Global.GetRandomNumber(1, (int)(HolyItemManager.MAX_HOLY_NUM + 1));
						sbyte sSlotIdx = (sbyte)Global.GetRandomNumber(1, (int)(HolyItemManager.MAX_HOLY_PART_NUM + 1));
						string nGoodsName = HolyItemManager.SliceNameSet[(int)sTypeIdx, (int)sSlotIdx];
						int nCount = Global.GetRandomNumber((int)actionParams[0], (int)actionParams[1] + 1);
						HolyItemManager.getInstance().GetHolyItemPart(client, sTypeIdx, sSlotIdx, nCount);
					}
					break;
				case MagicActionIDs.ADD_SHENGBEI:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						sbyte sSlotIdx = (sbyte)Global.GetRandomNumber(1, (int)(HolyItemManager.MAX_HOLY_PART_NUM + 1));
						string nGoodsName = HolyItemManager.SliceNameSet[1, (int)sSlotIdx];
						int nCount = Global.GetRandomNumber((int)actionParams[0], (int)actionParams[1] + 1);
						HolyItemManager.getInstance().GetHolyItemPart(client, 1, sSlotIdx, nCount);
					}
					break;
				case MagicActionIDs.ADD_SHENGJIAN:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						sbyte sSlotIdx = (sbyte)Global.GetRandomNumber(1, (int)(HolyItemManager.MAX_HOLY_PART_NUM + 1));
						string nGoodsName = HolyItemManager.SliceNameSet[2, (int)sSlotIdx];
						int nCount = Global.GetRandomNumber((int)actionParams[0], (int)actionParams[1] + 1);
						HolyItemManager.getInstance().GetHolyItemPart(client, 2, sSlotIdx, nCount);
					}
					break;
				case MagicActionIDs.ADD_SHENGGUAN:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						sbyte sSlotIdx = (sbyte)Global.GetRandomNumber(1, (int)(HolyItemManager.MAX_HOLY_PART_NUM + 1));
						string nGoodsName = HolyItemManager.SliceNameSet[3, (int)sSlotIdx];
						int nCount = Global.GetRandomNumber((int)actionParams[0], (int)actionParams[1] + 1);
						HolyItemManager.getInstance().GetHolyItemPart(client, 3, sSlotIdx, nCount);
					}
					break;
				case MagicActionIDs.ADD_SHENGDIAN:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						sbyte sSlotIdx = (sbyte)Global.GetRandomNumber(1, (int)(HolyItemManager.MAX_HOLY_PART_NUM + 1));
						string nGoodsName = HolyItemManager.SliceNameSet[4, (int)sSlotIdx];
						int nCount = Global.GetRandomNumber((int)actionParams[0], (int)actionParams[1] + 1);
						HolyItemManager.getInstance().GetHolyItemPart(client, 4, sSlotIdx, nCount);
					}
					break;
				case MagicActionIDs.WOLF_SEARCH_ROAD:
					if (self is Monster)
					{
						Monster monster = self as Monster;
						Point start = monster.FirstCoordinate;
						int x2 = (int)actionParams[0];
						int y = (int)actionParams[1];
						int max = Math.Min(3, monster.AttackRange / 100);
						if (start.X + 1000.0 < (double)x2)
						{
							x2 -= monster.AttackRange - 100;
							int r = Global.GetRandomNumber(0, max);
							y -= r * 100 - 100;
						}
						else if (start.X - 1000.0 > (double)x2)
						{
							x2 += monster.AttackRange - 100;
							int r = Global.GetRandomNumber(0, max);
							y -= r * 100 - 100;
						}
						else
						{
							y -= monster.AttackRange - 100;
							int[] xs = new int[]
							{
								-1,
								0,
								1
							};
							int r = Global.GetRandomNumber(0, 3);
							x2 += xs[r] * 100;
						}
						Point end = new Point((double)x2, (double)y);
						List<int[]> path = GlobalNew.FindPath(end, start, monster.CurrentMapCode);
						monster.PatrolPath = path;
						monster.Direction = (double)Global.GetRandomNumber(0, 8);
						monster.IsAutoSearchRoad = true;
					}
					break;
				case MagicActionIDs.WOLF_ATTACK_ROLE:
					if (self is Monster)
					{
						Monster monster = self as Monster;
						bool isAtackRole = (int)actionParams[0] > 0;
						monster.IsAttackRole = isAtackRole;
					}
					break;
				case MagicActionIDs.SELF_BURST:
					if (self is Monster)
					{
						Global.SystemKillMonster(self as Monster);
					}
					break;
				case MagicActionIDs.MU_ADD_PROPERTY:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						long nowTicks = TimeUtil.NOW();
						client.bufferPropsManager.UpdateTimedPropsData(nowTicks, nowTicks, (int)actionParams[2] * 1000, 1, (int)actionParams[0], actionParams[1] * (double)(skillLevel + 1), (int)actionParams[3], 0);
						if (RoleAlgorithm.NeedNotifyClient((ExtPropIndexes)actionParams[0]))
						{
							client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
							{
								DelayExecProcIds.NotifyRefreshProps
							});
						}
					}
					break;
				case MagicActionIDs.ADD_GAIMING:
				{
					int count = (int)actionParams[0];
					SingletonTemplate<NameManager>.Instance().GM_SetFreeModName((self as GameClient).ClientData.RoleID, count);
					break;
				}
				case MagicActionIDs.MU_ADD_DAMAGETHORN:
				{
					double dBaseRate = actionParams[0];
					double dAddValue = actionParams[1];
					double dTime = actionParams[2];
					int nSkillLevel = skillLevel + 1;
					double dRealyRate = dBaseRate + dBaseRate / 200.0 * (double)nSkillLevel;
					double dRealyValue = dAddValue + dAddValue * (double)nSkillLevel;
					long ToTick = TimeUtil.NOW() * 10000L + (long)dTime * 1000L * 10000L;
					double[] newParams = new double[]
					{
						dTime,
						(double)skillid,
						(double)nSkillLevel
					};
					if (obj is GameClient)
					{
						(obj as GameClient).RoleBuffer.AddTempExtProp(29, dRealyRate, ToTick);
						(obj as GameClient).RoleBuffer.AddTempExtProp(30, dRealyValue, ToTick);
					}
					else
					{
						(self as GameClient).RoleBuffer.AddTempExtProp(29, dRealyRate, ToTick);
						(self as GameClient).RoleBuffer.AddTempExtProp(30, dRealyValue, ToTick);
					}
					Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_ADD_DAMAGE_THORN_PERCENT, newParams, 1, true);
					break;
				}
				case MagicActionIDs.MU_ADD_VAMPIRE:
				{
					double dBaseRate = actionParams[0];
					double dAddValue = actionParams[1];
					double dTransRate = actionParams[2];
					int injured = 0;
					int nSkillLevel = skillLevel + 1;
					dBaseRate += dBaseRate / 200.0 * (double)nSkillLevel;
					dAddValue += dAddValue * (double)nSkillLevel;
					int attackType = 1;
					if (self is GameClient)
					{
						attackType = Global.GetAttackType(self as GameClient);
						if (obj is GameClient)
						{
							injured = GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, false, dBaseRate, (int)dAddValue, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is Monster)
						{
							injured = GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, dBaseRate, (int)dAddValue, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is BiaoCheItem)
						{
							injured = BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							injured = JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						if (injured > 0 && (self as GameClient).ClientData.CurrentLifeV > 0)
						{
							double addLife = (double)injured * dTransRate;
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, addLife, "吸血攻击， 脚本" + id.ToString());
						}
						else if (obj is FakeRoleItem)
						{
							injured = FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					else if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, nSkillLevel, 0.0, 0.0, false, dBaseRate, (int)dAddValue, 0, skillid, shenShiInjurePercent);
					}
					else if (obj is Monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, attackType, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, dBaseRate, (int)dAddValue, 0, skillid, shenShiInjurePercent);
					}
					break;
				}
				case MagicActionIDs.BUY_YUEKA:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						YueKaManager.HandleUserBuyYueKa(client.strUserID, client.ClientData.RoleID);
					}
					break;
				case MagicActionIDs.SET_WING:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						int suit = (int)actionParams[0];
						int start2 = (int)actionParams[1];
						string txt = string.Format("-setwingsuitstar {0} {1}", suit, start2);
						GameManager.systemGMCommands.GMSetWingSuitStar(client, txt.Split(new char[]
						{
							' '
						}));
					}
					break;
				case MagicActionIDs.SET_MAX_WING:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						string txt = string.Format("-setwingsuitstar {0} {1}", MUWingsManager.MaxWingID, MUWingsManager.MaxWingEnchanceLevel);
						GameManager.systemGMCommands.GMSetWingSuitStar(client, txt.Split(new char[]
						{
							' '
						}));
						LingYuManager.SetLingYuMax_GM(client);
						ZhuLingZhuHunManager.SetZhuLingMax_GM(client);
						ZhuLingZhuHunManager.SetZhuHunMax_GM(client);
					}
					break;
				case MagicActionIDs.SET_ALLGOODS_LV:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						int forgelev = (int)actionParams[0];
						List<int> categoriyList = new List<int>();
						for (int i = 0; i <= 6; i++)
						{
							categoriyList.Add(i);
						}
						for (int i = 11; i <= 21; i++)
						{
							categoriyList.Add(i);
						}
						List<GoodsData> usingEquipList = client.UsingEquipMgr.GetGoodsByCategoriyList(categoriyList);
						if (null != usingEquipList)
						{
							foreach (GoodsData goods in usingEquipList)
							{
								GameManager.systemGMCommands.GMSetGoodsForgeLevel(client, goods, forgelev, 1, false);
							}
							Global.RefreshEquipProp(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						}
					}
					break;
				case MagicActionIDs.SET_GOODS_LV:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						int forgelev = (int)actionParams[0];
						int itemCat = (int)actionParams[1];
						List<int> categoriyList = new List<int>();
						if (1 <= itemCat && itemCat <= 5)
						{
							categoriyList.Add(itemCat - 1);
						}
						else if (6 == itemCat || 7 == itemCat)
						{
							for (int i = 11; i <= 21; i++)
							{
								categoriyList.Add(i);
							}
						}
						else if (8 == itemCat || 9 == itemCat)
						{
							categoriyList.Add(6);
						}
						else if (10 == itemCat)
						{
							categoriyList.Add(5);
						}
						List<GoodsData> usingEquipList = client.UsingEquipMgr.GetGoodsByCategoriyList(categoriyList);
						if (usingEquipList != null && usingEquipList.Count > 0)
						{
							if ((1 <= itemCat && itemCat <= 5) || 10 == itemCat)
							{
								GameManager.systemGMCommands.GMSetGoodsForgeLevel(client, usingEquipList[0], forgelev, 1, true);
							}
							else if (6 == itemCat || 7 == itemCat)
							{
								GoodsData goods = usingEquipList.Find(delegate(GoodsData x)
								{
									int nHandType = -1;
									int ActionType = -1;
									SystemXmlItem systemGoods = null;
									if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(x.GoodsID, out systemGoods))
									{
										nHandType = systemGoods.GetIntValue("HandType", -1);
										ActionType = systemGoods.GetIntValue("ActionType", -1);
									}
									return (6 == itemCat && nHandType == 1) || (6 == itemCat && nHandType == 2 && x.BagIndex == 0) || (7 == itemCat && nHandType == 0) || (7 == itemCat && nHandType == 2 && x.BagIndex == 1) || (ActionType != 1 && ActionType != 4);
								});
								if (null != goods)
								{
									GameManager.systemGMCommands.GMSetGoodsForgeLevel(client, goods, forgelev, 1, true);
								}
							}
							else if (8 == itemCat || 9 == itemCat)
							{
								GoodsData goods;
								if (8 == itemCat)
								{
									goods = usingEquipList.Find((GoodsData x) => x.BagIndex == 0);
								}
								else
								{
									goods = usingEquipList.Find((GoodsData x) => x.BagIndex == 1);
								}
								if (null != goods)
								{
									GameManager.systemGMCommands.GMSetGoodsForgeLevel(client, goods, forgelev, 1, true);
								}
							}
						}
					}
					break;
				case MagicActionIDs.SET_MAX_GUOSHI:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						int StrengthMax = UseFruitVerify.GetFruitAddPropLimit(client, "Strength");
						int IntelligenceMax = UseFruitVerify.GetFruitAddPropLimit(client, "Intelligence");
						int DexterityMax = UseFruitVerify.GetFruitAddPropLimit(client, "Dexterity");
						int ConstitutionMax = UseFruitVerify.GetFruitAddPropLimit(client, "Constitution");
						int Strength = Global.GetRoleParamsInt32FromDB(client, "PropStrengthChangeless");
						int Intelligence = Global.GetRoleParamsInt32FromDB(client, "PropIntelligenceChangeless");
						int Dexterity = Global.GetRoleParamsInt32FromDB(client, "PropDexterityChangeless");
						int Constitution = Global.GetRoleParamsInt32FromDB(client, "PropConstitutionChangeless");
						int ModValue = StrengthMax - Strength;
						double[] OptParams = new double[]
						{
							1.0,
							(double)ModValue,
							(double)ModValue
						};
						MagicAction.ProcessAction(self, null, MagicActionIDs.MU_RANDOM_STRENGTH, OptParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, true, false, 1.0, 1, 0.0);
						ModValue = IntelligenceMax - Intelligence;
						OptParams = new double[]
						{
							1.0,
							(double)ModValue,
							(double)ModValue
						};
						MagicAction.ProcessAction(self, null, MagicActionIDs.MU_RANDOM_INTELLIGENCE, OptParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, true, false, 1.0, 1, 0.0);
						ModValue = DexterityMax - Dexterity;
						OptParams = new double[]
						{
							1.0,
							(double)ModValue,
							(double)ModValue
						};
						MagicAction.ProcessAction(self, null, MagicActionIDs.MU_RANDOM_DEXTERITY, OptParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, true, false, 1.0, 1, 0.0);
						ModValue = ConstitutionMax - Constitution;
						OptParams = new double[]
						{
							1.0,
							(double)ModValue,
							(double)ModValue
						};
						MagicAction.ProcessAction(self, null, MagicActionIDs.MU_RANDOM_CONSTITUTION, OptParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, true, false, 1.0, 1, 0.0);
					}
					break;
				case MagicActionIDs.SET_MAX_XINGHUN:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						GameManager.StarConstellationMgr.ActivationStarConstellationAll(client);
					}
					break;
				case MagicActionIDs.SET_MEILIN:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						int Level = (int)actionParams[0];
						int Star = (int)actionParams[1];
						GameManager.MerlinMagicBookMgr.GMMerlinLevelUpToN(client, Level);
						GameManager.MerlinMagicBookMgr.GMMerlinStarUpToN(client, Star);
					}
					break;
				case MagicActionIDs.SET_MAINTASK:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						int taskID = (int)actionParams[0];
						ProcessTask.GMSetMainTaskID(client, taskID);
					}
					break;
				case MagicActionIDs.SET_LEVEL:
					if (self is GameClient)
					{
						GameClient client = self as GameClient;
						int changeLifeCount = (int)actionParams[0];
						int level = (int)actionParams[1];
						string txt = string.Format("-setlevel {0} {1} {2}", Global.FormatRoleName4(client), changeLifeCount, level);
						GameManager.systemGMCommands.GMSetLevel(client, txt.Split(new char[]
						{
							' '
						}));
					}
					break;
				case MagicActionIDs.DB_ADD_REBORNEXP:
					if (self is GameClient)
					{
						RebornManager.getInstance().ProcessRoleExperience(self as GameClient, (long)actionParams[0], MoneyTypes.RebornExp, true, true, false, "none");
					}
					break;
				case MagicActionIDs.DB_ADD_REBORNEXP_MONSTERS_MAX:
					if (self is GameClient)
					{
						GameManager.ClientMgr.ModifyRebornExpMaxAddValue(self as GameClient, (long)((int)actionParams[0]), "道具脚本", MoneyTypes.RebornExpMonster, false, true, false);
					}
					break;
				case MagicActionIDs.DB_ADD_REBORNEXP_GOODS_MAX:
					if (self is GameClient)
					{
						GameManager.ClientMgr.ModifyRebornExpMaxAddValue(self as GameClient, (long)((int)actionParams[0]), "道具脚本", MoneyTypes.RebornExpSale, false, true, false);
					}
					break;
				case MagicActionIDs.DB_ADD_MULTI_REBORNEXP:
					if (self is GameClient)
					{
						double[] newParams = new double[]
						{
							actionParams[1],
							(double)((long)actionGoodsID << 32 | (long)actionParams[0])
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.RebornMutilExp, newParams, 0, true);
					}
					break;
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x06002ECB RID: 11979 RVA: 0x0029EBF0 File Offset: 0x0029CDF0
		public static void ZhongDuActionProc(long execTicks, object state)
		{
			MagicAction.ZhongDuContextData context = state as MagicAction.ZhongDuContextData;
			if (null != context)
			{
				GameClient selfClient = context.Self as GameClient;
				GameClient attackerClient = GameManager.ClientMgr.FindClient(context.Attacker);
				Monster selfMonster = context.Self as Monster;
				Monster attackerMonster = null;
				IObject attackerObj;
				if (null == attackerClient)
				{
					if (selfClient != null)
					{
						attackerMonster = GameManager.MonsterMgr.FindMonster(selfClient.CurrentMapCode, context.Attacker);
					}
					else if (selfMonster != null)
					{
						attackerMonster = GameManager.MonsterMgr.FindMonster(selfMonster.CurrentMapCode, context.Attacker);
					}
					attackerObj = attackerMonster;
				}
				else
				{
					attackerObj = attackerClient;
				}
				if (null != attackerObj)
				{
					if (context.Self.CurrentMapCode == attackerObj.CurrentMapCode && context.Self.CurrentCopyMapID == attackerObj.CurrentCopyMapID)
					{
						if (selfClient != null)
						{
							if (attackerClient != null)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attackerClient, selfClient, 0, context.Injure, 1.0, context.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, true, context.percent, 0, 0, 0, 0.0);
							}
							else if (attackerMonster != null)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attackerMonster, selfClient, 0, context.Injure, 1.0, context.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, context.percent, 0, 0, 0, 0.0);
							}
						}
						else if (selfMonster != null)
						{
							if (attackerClient != null)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attackerClient, selfMonster, 0, context.Injure, 1.0, context.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, context.percent, 0, 0, 0, 0.0);
							}
							else if (attackerMonster != null)
							{
								GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attackerMonster, selfMonster, 0, context.Injure, 1.0, context.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, context.percent, 0, 0, 0, 0.0);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002ECC RID: 11980 RVA: 0x0029EEF8 File Offset: 0x0029D0F8
		public static void ChenMoActionProc(long execTicks, object state)
		{
			MagicAction.ChenMoContextData context = state as MagicAction.ChenMoContextData;
			if (null != context)
			{
				if (!context.Stop)
				{
					GameClient selfClient = context.Self as GameClient;
					GameClient attackerClient = GameManager.ClientMgr.FindClient(context.Attacker);
					Monster selfMonster = context.Self as Monster;
					Monster attackerMonster = null;
					IObject attackerObj;
					if (null == attackerClient)
					{
						if (selfClient != null)
						{
							attackerMonster = GameManager.MonsterMgr.FindMonster(selfClient.CurrentMapCode, context.Attacker);
						}
						else if (selfMonster != null)
						{
							attackerMonster = GameManager.MonsterMgr.FindMonster(selfMonster.CurrentMapCode, context.Attacker);
						}
						attackerObj = attackerMonster;
					}
					else
					{
						attackerObj = attackerClient;
					}
					if (null != attackerObj)
					{
						if (context.Self.CurrentMapCode == attackerObj.CurrentMapCode && context.Self.CurrentCopyMapID == attackerObj.CurrentCopyMapID)
						{
							if (selfClient != null)
							{
								if (selfClient.buffManager.IsBuffEnabled(116))
								{
									context.Stop = true;
								}
								else if (attackerClient != null)
								{
									GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attackerClient, selfClient, 0, context.Injure, 1.0, context.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, true, context.percent, 0, 0, 0, 0.0);
								}
								else if (attackerMonster != null)
								{
									GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attackerMonster, selfClient, 0, context.Injure, 1.0, context.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, context.percent, 0, 0, 0, 0.0);
								}
							}
							else if (selfMonster != null)
							{
								if (attackerClient != null)
								{
									GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attackerClient, selfMonster, 0, context.Injure, 1.0, context.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, context.percent, 0, 0, 0, 0.0);
								}
								else if (attackerMonster != null)
								{
									GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attackerMonster, selfMonster, 0, context.Injure, 1.0, context.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, context.percent, 0, 0, 0, 0.0);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x04003C69 RID: 15465
		[ThreadStatic]
		public static int MaxHitNum;

		// Token: 0x02000742 RID: 1858
		public class ZhongDuContextData
		{
			// Token: 0x04003C6C RID: 15468
			public IObject Self;

			// Token: 0x04003C6D RID: 15469
			public int Attacker;

			// Token: 0x04003C6E RID: 15470
			public int Injure;

			// Token: 0x04003C6F RID: 15471
			public int Occupation;

			// Token: 0x04003C70 RID: 15472
			public double percent;
		}

		// Token: 0x02000743 RID: 1859
		public class ChenMoContextData
		{
			// Token: 0x04003C71 RID: 15473
			public IObject Self;

			// Token: 0x04003C72 RID: 15474
			public int Attacker;

			// Token: 0x04003C73 RID: 15475
			public int Injure;

			// Token: 0x04003C74 RID: 15476
			public int Occupation;

			// Token: 0x04003C75 RID: 15477
			public bool Stop;

			// Token: 0x04003C76 RID: 15478
			public double percent;
		}
	}
}
