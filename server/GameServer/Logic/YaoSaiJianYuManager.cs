using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.JingJiChang;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	public class YaoSaiJianYuManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx
	{
		
		public static YaoSaiJianYuManager getInstance()
		{
			return YaoSaiJianYuManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1520, 1, 1, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1521, 1, 1, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1522, 2, 2, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1523, 3, 3, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1524, 2, 2, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1525, 3, 3, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1527, 1, 1, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1528, 1, 1, YaoSaiJianYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(53, YaoSaiJianYuManager.getInstance());
			GlobalEventSource.getInstance().registerListener(54, YaoSaiJianYuManager.getInstance());
			GlobalEventSource.getInstance().registerListener(14, YaoSaiJianYuManager.getInstance());
			GlobalEventSource.getInstance().registerListener(12, YaoSaiJianYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(29, 44, YaoSaiJianYuManager.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(53, YaoSaiJianYuManager.getInstance());
			GlobalEventSource.getInstance().removeListener(54, YaoSaiJianYuManager.getInstance());
			GlobalEventSource.getInstance().removeListener(14, YaoSaiJianYuManager.getInstance());
			GlobalEventSource.getInstance().removeListener(12, YaoSaiJianYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(29, 44, YaoSaiJianYuManager.getInstance());
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.Building, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1520:
					return this.ProcessGetYaoSaiMainDataCmd(client, nID, bytes, cmdParams);
				case 1521:
					return this.ProcessGetYaoSaiHuDongDataCmd(client, nID, bytes, cmdParams);
				case 1522:
					return this.ProcessYaoSaiRevoltCmd(client, nID, bytes, cmdParams);
				case 1523:
					return this.ProcessYaoSaiHuDongCmd(client, nID, bytes, cmdParams);
				case 1524:
					return this.ProcessYaoSaiFreeCmd(client, nID, bytes, cmdParams);
				case 1525:
					return this.ProcessYaoSaiOptCmd(client, nID, bytes, cmdParams);
				case 1527:
					return this.ProcessBuyZhengFuCountCmd(client, nID, bytes, cmdParams);
				case 1528:
					return this.ProcessGetYaoSaiDataCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 53)
			{
				JingJiChangWinEventObject evObj = eventObject as JingJiChangWinEventObject;
				if (evObj != null && evObj.getType() == 1)
				{
					this.processWin(evObj.getPlayer(), evObj.getRobot());
				}
			}
			else if (eventObject.getEventType() == 54)
			{
				JingJiChangFailedEventObject evObj2 = eventObject as JingJiChangFailedEventObject;
				if (evObj2 != null && evObj2.getType() == 1)
				{
					this.processFailed(evObj2.getPlayer(), evObj2.getRobot());
				}
			}
			else if (eventObject.getEventType() == 14)
			{
				GameClient client = (eventObject as PlayerInitGameEventObject).getPlayer();
				this.OnLogin(client);
			}
			else if (eventObject.getEventType() == 12)
			{
				GameClient client = (eventObject as PlayerLogoutEventObject).getPlayer();
				this.OnLogout(client);
			}
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
			if (eventObject.EventType == 29)
			{
				OnClientChangeMapEventObject evObj = eventObject as OnClientChangeMapEventObject;
				if (null != evObj)
				{
					bool isOpen = GlobalNew.IsGongNengOpened(evObj.Client, GongNengIDs.Building, false);
					if (isOpen)
					{
						GameClient client = evObj.Client;
						if (Global.CanRecordPos(client))
						{
							GameManager.DBCmdMgr.AddDBCmd(10001, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								client.ClientData.RoleID,
								client.ClientData.MapCode,
								client.ClientData.RoleDirection,
								client.ClientData.PosX,
								client.ClientData.PosY
							}), null, client.ServerId);
						}
						KFPrisonRoleData kfpData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(client.ClientData.RoleID);
						if (null == kfpData)
						{
							this.UpdateYaoSaiPrisonRoleData(evObj.Client);
							YaoSaiMissionManager.getInstance().RefReshMission(evObj.Client);
						}
						evObj.Result = true;
					}
					else
					{
						evObj.Result = false;
					}
					evObj.Handled = true;
				}
			}
		}

		
		public bool ProcessGetYaoSaiMainDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Convert.ToInt32(cmdParams[0]);
				PrisonMainData mainData = this.BuildYaoSaiMainDataForClient(client);
				client.sendCmd<PrisonMainData>(nID, mainData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessGetYaoSaiHuDongDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Convert.ToInt32(cmdParams[0]);
				List<PrisonLogData> logDataList = this.BuildYaoSaiLogDataForClient(client);
				client.sendCmd<List<PrisonLogData>>(nID, logDataList, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessYaoSaiRevoltCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Convert.ToInt32(cmdParams[0]);
				int useZuanShi = Convert.ToInt32(cmdParams[1]);
				if (client.ClientData.RoleID != roleID)
				{
					return true;
				}
				if (JingJiChangManager.getInstance().IsJingJiChangMap(client.ClientData.MapCode))
				{
					return true;
				}
				bool inCoolDown = TimeUtil.NOW() - this.GetRevoltCD(client) < (long)(60000 * this.FightCDTime);
				if (inCoolDown && useZuanShi == 0)
				{
					result = -2007;
					client.sendCmd(nID, string.Format("{0}", result), false);
					return true;
				}
				KFPrisonRoleData kfMineData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(client.ClientData.RoleID);
				if (kfMineData == null || kfMineData.OwnerID == 0)
				{
					result = -5;
					client.sendCmd(nID, string.Format("{0}", result), false);
					return true;
				}
				KFPrisonRoleData kfOwnerData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(kfMineData.OwnerID);
				if (kfOwnerData == null || kfOwnerData.RoleID == 0)
				{
					result = -5;
					client.sendCmd(nID, string.Format("{0}", result), false);
					return true;
				}
				if (inCoolDown && useZuanShi == 1)
				{
					if (client.ClientData.UserMoney < this.FightCDClearCost)
					{
						result = -10;
						client.sendCmd(nID, string.Format("{0}", result), false);
						return true;
					}
				}
				int mineLevelID = this.GetYaoSaiLevelID(Global.GetUnionLevel2(client.ClientData.ChangeLifeCount, client.ClientData.Level));
				int ownerLeveID = this.GetYaoSaiLevelID(kfOwnerData.UnionLevel);
				if (ownerLeveID > mineLevelID)
				{
					int ret = JunTuanClient.getInstance().YaoSaiPrisonOpt(kfMineData.RoleID, kfOwnerData.RoleID, -1, true);
					if (ret < 0)
					{
						result = -5;
						client.sendCmd(nID, string.Format("{0}", result), false);
						return true;
					}
					result = 1;
				}
				else
				{
					KFPrisonJingJiData jingjiData = JunTuanClient.getInstance().GetYaoSaiPrisonJingJiData(kfMineData.OwnerID);
					if (jingjiData == null || null == jingjiData.PlayerJingJiMirrorData)
					{
						result = -4;
						client.sendCmd(nID, string.Format("{0}", result), false);
						return true;
					}
					PlayerJingJiData jingJiData = DataHelper.BytesToObject<PlayerJingJiData>(jingjiData.PlayerJingJiMirrorData, 0, jingjiData.PlayerJingJiMirrorData.Length);
					JingJiChangManager.getInstance().enterJingJiChang(client, jingJiData, JingJiFuBenType.YAOSAI);
					client.ClientData.YaoSaiPrisonOptType = YaoSaiOptType.fankang;
					this.UpdateYaoSaiPrisonRoleData(client);
				}
				if (inCoolDown && useZuanShi == 1)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, this.FightCDClearCost, "要塞征服反抗CD", true, true, false, DaiBiSySType.None))
					{
						result = -10;
						client.sendCmd(nID, string.Format("{0}", result), false);
						return true;
					}
				}
				this.SetRevoltCD(client, TimeUtil.NOW());
				client.sendCmd(nID, string.Format("{0}", result), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessYaoSaiHuDongCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Convert.ToInt32(cmdParams[0]);
				int targetID = Convert.ToInt32(cmdParams[1]);
				int hudongType = Convert.ToInt32(cmdParams[2]);
				if (client.ClientData.RoleID != roleID)
				{
					return true;
				}
				Dictionary<int, YaoSaiJianYuManorCommandConfig> tempCommandAwardConfigDict = null;
				List<FallGoodsItem> tempFallGoodsItemConfigList = null;
				lock (this.ConfigMutex)
				{
					tempCommandAwardConfigDict = this.CommandAwardConfigDict;
					tempFallGoodsItemConfigList = this.FallGoodsItemConfigList;
				}
				List<int> countList = this.GetYaoSaiJianYuCount(client);
				if (countList[2] >= this.FuLuHuDongCount)
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						targetID,
						hudongType,
						this.FuLuHuDongCount
					}), false);
					return true;
				}
				YaoSaiJianYuManorCommandConfig hudongConfig = null;
				if (!tempCommandAwardConfigDict.TryGetValue(hudongType, out hudongConfig))
				{
					result = -3;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						targetID,
						hudongType,
						countList[2]
					}), false);
					return true;
				}
				List<KFPrisonRoleData> fuluList = JunTuanClient.getInstance().GetYaoSaiPrisonFuLuData(roleID);
				if (null == fuluList)
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						targetID,
						hudongType,
						countList[2]
					}), false);
					return true;
				}
				KFPrisonRoleData fuluRoleData = fuluList.Find((KFPrisonRoleData x) => x.RoleID == targetID);
				if (null == fuluRoleData)
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						targetID,
						hudongType,
						countList[2]
					}), false);
					return true;
				}
				List<PrisonHuDongData> hudongDataList = this.GetHuDongData(client);
				PrisonHuDongData hudongData = hudongDataList.Find((PrisonHuDongData x) => x.RoleID == targetID);
				if (null != hudongData)
				{
					long laodongTime = TimeUtil.NOW() - hudongData.HuDongStartTicks;
					if (laodongTime < (long)(this.FuLuHuDongUseTime * 60000))
					{
						result = -2007;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							result,
							roleID,
							targetID,
							hudongType,
							countList[2]
						}), false);
						return true;
					}
				}
				int mineUnionLev = Global.GetUnionLevel2(client.ClientData.ChangeLifeCount, client.ClientData.Level);
				int fuluUnionLev = fuluRoleData.UnionLevel;
				double powAwardNum = Math.Pow((double)(mineUnionLev + fuluUnionLev), 1.6);
				int mineAward = (int)(powAwardNum * hudongConfig.OwnerFactor);
				int fuluAward = (int)(powAwardNum * hudongConfig.FuLuFactor);
				mineAward -= ((mineAward > 100) ? (mineAward % 100) : 0);
				fuluAward -= ((fuluAward > 100) ? (fuluAward % 100) : 0);
				int ret = JunTuanClient.getInstance().YaoSaiPrisonHuDong(roleID, targetID, hudongType, (int)hudongConfig.AwardType, mineAward, fuluAward);
				if (ret < 0)
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						targetID,
						hudongType,
						countList[2]
					}), false);
					return true;
				}
				this.GiveHuDongAward(client, hudongConfig.AwardType, mineAward);
				List<FallGoodsItem> tempItemList2 = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(tempFallGoodsItemConfigList, tempFallGoodsItemConfigList.Count, 0, 1.0);
				if (tempItemList2 != null && tempItemList2.Count > 0)
				{
					List<GoodsData> goodsDataList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(tempItemList2);
					for (int i = 0; i < goodsDataList.Count; i++)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsDataList[i].Quality, goodsDataList[i].Props, goodsDataList[i].Forge_level, goodsDataList[i].Binding, 0, "", true, 1, "要塞监狱互动", goodsDataList[i].Endtime, goodsDataList[i].AddPropIndex, goodsDataList[i].BornIndex, goodsDataList[i].Lucky, goodsDataList[i].Strong, goodsDataList[i].ExcellenceInfo, goodsDataList[i].AppendPropLev, goodsDataList[i].ChangeLifeLevForEquip, null, null, 0, true);
					}
				}
				List<int> list;
				(list = countList)[2] = list[2] + 1;
				this.SaveYaoSaiJianYuCount(client, countList);
				this.HuDongStart(client, targetID);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					result,
					roleID,
					targetID,
					hudongType,
					countList[2]
				}), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessYaoSaiFreeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Convert.ToInt32(cmdParams[0]);
				int targetID = Convert.ToInt32(cmdParams[1]);
				if (client.ClientData.RoleID != roleID)
				{
					return true;
				}
				List<KFPrisonRoleData> fuluList = JunTuanClient.getInstance().GetYaoSaiPrisonFuLuData(roleID);
				if (null == fuluList)
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, roleID, targetID), false);
					return true;
				}
				KFPrisonRoleData fuluRoleData = fuluList.Find((KFPrisonRoleData x) => x.RoleID == targetID);
				if (null == fuluRoleData)
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, roleID, targetID), false);
					return true;
				}
				int ret = JunTuanClient.getInstance().YaoSaiPrisonOpt(roleID, targetID, -2, true);
				if (ret < 0)
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, roleID, targetID), false);
					return true;
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, roleID, targetID), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessYaoSaiOptCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Convert.ToInt32(cmdParams[0]);
				int targetID = Convert.ToInt32(cmdParams[1]);
				int optType = Convert.ToInt32(cmdParams[2]);
				if (client.ClientData.RoleID != roleID)
				{
					return true;
				}
				if (targetID == roleID)
				{
					return true;
				}
				if (optType != 0 && optType != 1 && optType != 2)
				{
					return true;
				}
				if (JingJiChangManager.getInstance().IsJingJiChangMap(client.ClientData.MapCode))
				{
					return true;
				}
				KFPrisonRoleData srcData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(roleID);
				KFPrisonJingJiData jingjiData = JunTuanClient.getInstance().GetYaoSaiPrisonJingJiData(targetID);
				KFPrisonRoleData targetData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(targetID);
				if (srcData == null || targetData == null || jingjiData == null || null == jingjiData.PlayerJingJiMirrorData)
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						roleID,
						targetID,
						optType
					}), false);
					return true;
				}
				client.ClientData.YaoSaiPrisonOptType = (YaoSaiOptType)optType;
				client.ClientData.YaoSaiPrisonTargetID = targetID;
				client.ClientData.YaoSaiPrisonTargetName = targetData.RoleName;
				if (optType == 0 || optType == 1)
				{
					if (srcData.OwnerID == targetID)
					{
						result = -12;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							roleID,
							targetID,
							optType
						}), false);
						return true;
					}
					int srcUnionLev = Global.GetUnionLevel2(client.ClientData.ChangeLifeCount, client.ClientData.Level);
					if (this.GetYaoSaiLevelID(srcUnionLev) != this.GetYaoSaiLevelID(targetData.UnionLevel))
					{
						result = -19;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							roleID,
							targetID,
							optType
						}), false);
						return true;
					}
					List<KFPrisonRoleData> fuluList = JunTuanClient.getInstance().GetYaoSaiPrisonFuLuData(roleID);
					bool flag;
					if (fuluList != null)
					{
						if (fuluList.Count < 4)
						{
							flag = !fuluList.Exists((KFPrisonRoleData x) => x.RoleID == targetID);
						}
						else
						{
							flag = false;
						}
					}
					else
					{
						flag = true;
					}
					if (!flag)
					{
						result = -36;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							roleID,
							targetID,
							optType
						}), false);
						return true;
					}
				}
				if (optType == 2 || optType == 1)
				{
					if (targetData.OwnerID == 0 || targetData.OwnerID == roleID)
					{
						result = -12;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							roleID,
							targetID,
							optType
						}), false);
						return true;
					}
					jingjiData = JunTuanClient.getInstance().GetYaoSaiPrisonJingJiData(targetData.OwnerID);
					targetData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(targetData.OwnerID);
					if (targetData == null || targetData.RoleID == 0 || jingjiData == null || null == jingjiData.PlayerJingJiMirrorData)
					{
						result = -12;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							roleID,
							targetID,
							optType
						}), false);
						return true;
					}
				}
				List<int> tempZhengFuCostList = null;
				List<int> tempHelpCostList = null;
				lock (this.ConfigMutex)
				{
					tempZhengFuCostList = this.ZhengFuCostList;
					tempHelpCostList = this.HelpCostList;
				}
				int costUserMoney = 0;
				List<int> countList = this.GetYaoSaiJianYuCount(client);
				if (optType == 0 || optType == 1)
				{
					List<int> list;
					if (countList[4] > 0)
					{
						(list = countList)[4] = list[4] - 1;
					}
					else
					{
						int costIndex = countList[3] - tempZhengFuCostList[0] + 1;
						if (costIndex > 0)
						{
							costUserMoney = ((costIndex >= tempZhengFuCostList.Count) ? tempZhengFuCostList[tempZhengFuCostList.Count - 1] : tempZhengFuCostList[costIndex]);
						}
					}
					(list = countList)[3] = list[3] + 1;
				}
				else
				{
					int costIndex = countList[1] - tempHelpCostList[0] + 1;
					if (costIndex > 0)
					{
						costUserMoney = ((costIndex >= tempHelpCostList.Count) ? tempHelpCostList[tempHelpCostList.Count - 1] : tempHelpCostList[costIndex]);
					}
					List<int> list;
					(list = countList)[1] = list[1] + 1;
				}
				if (costUserMoney > 0)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, costUserMoney, "要塞征服抢夺解救", true, true, false, DaiBiSySType.None))
					{
						result = -10;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							roleID,
							targetID,
							optType
						}), false);
						return true;
					}
				}
				PlayerJingJiData jingJiData = DataHelper.BytesToObject<PlayerJingJiData>(jingjiData.PlayerJingJiMirrorData, 0, jingjiData.PlayerJingJiMirrorData.Length);
				JingJiChangManager.getInstance().enterJingJiChang(client, jingJiData, JingJiFuBenType.YAOSAI);
				this.UpdateYaoSaiPrisonRoleData(client);
				this.SaveYaoSaiJianYuCount(client, countList);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					result,
					roleID,
					targetID,
					optType
				}), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessBuyZhengFuCountCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Convert.ToInt32(cmdParams[0]);
				if (client.ClientData.RoleID != roleID)
				{
					return true;
				}
				List<int> countList = this.GetYaoSaiJianYuCount(client);
				if (countList[4] > 0)
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, roleID, countList[4]), false);
					return true;
				}
				List<int> tempZhengFuCostList = null;
				lock (this.ConfigMutex)
				{
					tempZhengFuCostList = this.ZhengFuCostList;
				}
				int costUserMoney = 0;
				int costIndex = countList[3] - tempZhengFuCostList[0] + 1;
				if (costIndex > 0)
				{
					costUserMoney = ((costIndex >= tempZhengFuCostList.Count) ? tempZhengFuCostList[tempZhengFuCostList.Count - 1] : tempZhengFuCostList[costIndex]);
				}
				if (costUserMoney > 0)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, costUserMoney, "要塞征服抢夺解救", true, true, false, DaiBiSySType.None))
					{
						result = -10;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, roleID, countList[4]), false);
						return true;
					}
				}
				List<int> list;
				(list = countList)[4] = list[4] + 1;
				this.SaveYaoSaiJianYuCount(client, countList);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, roleID, countList[4]), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessGetYaoSaiDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int targetID = Convert.ToInt32(cmdParams[0]);
				if (targetID == 0)
				{
					YaoSaiWorldData worldData = new YaoSaiWorldData();
					if (this.ManorSearchCost > 0 && Global.GetTotalBindTongQianAndTongQianVal(client) < this.ManorSearchCost)
					{
						client.sendCmd<YaoSaiWorldData>(nID, worldData, false);
						return true;
					}
					HashSet<int> frindSet = new HashSet<int>();
					if (null != client.ClientData.FriendDataList)
					{
						foreach (FriendData item in client.ClientData.FriendDataList)
						{
							if (item.FriendType == 0)
							{
								frindSet.Add(item.OtherRoleID);
							}
						}
					}
					KFPrisonRoleData kfpRoleData = JunTuanClient.getInstance().SearchYaoSaiFuLu(client.ClientData.RoleID, Global.GetUnionLevel2(client), client.ClientData.Faction, frindSet);
					if (kfpRoleData != null && this.ManorSearchCost > 0)
					{
						if (!Global.SubBindTongQianAndTongQian(client, this.ManorSearchCost, "世界战役搜索"))
						{
							return true;
						}
					}
					this.BuildYaoSaiWolrdDataForClient(client, worldData, kfpRoleData);
					client.sendCmd<YaoSaiWorldData>(nID, worldData, false);
				}
				else
				{
					KFPrisonRoleData kfpRoleData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(targetID);
					YaoSaiWorldData worldData = new YaoSaiWorldData();
					this.BuildYaoSaiWolrdDataForClient(client, worldData, kfpRoleData);
					client.sendCmd<YaoSaiWorldData>(nID, worldData, false);
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private void GiveHuDongAward(GameClient client, PrisonAwardType awardType, int addNum)
		{
			if (addNum > 0)
			{
				if (awardType == PrisonAwardType.mojing)
				{
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, addNum, "要塞征服互动奖励", true, true, false);
				}
				else if (awardType == PrisonAwardType.xinghun)
				{
					GameManager.ClientMgr.ModifyStarSoulValue(client, addNum, "要塞征服互动奖励", true, true);
				}
				else if (awardType == PrisonAwardType.chengjiu)
				{
					GameManager.ClientMgr.ModifyChengJiuPointsValue(client, addNum, "GM要塞征服互动奖励", true, true);
				}
				else if (awardType == PrisonAwardType.shengwang)
				{
					GameManager.ClientMgr.ModifyShengWangValue(client, addNum, "要塞征服互动奖励", true, true);
				}
				else if (awardType == PrisonAwardType.zhangong)
				{
					GameManager.ClientMgr.AddBangGong(client, ref addNum, AddBangGongTypes.YaoSaiJianYu, 0);
				}
			}
		}

		
		private void processWin(GameClient player, Robot robot)
		{
			if (player.ClientData.YaoSaiPrisonOptType == YaoSaiOptType.jiejiu || player.ClientData.YaoSaiPrisonOptType == YaoSaiOptType.qiangduo)
			{
				JunTuanClient.getInstance().YaoSaiPrisonOpt(player.ClientData.RoleID, player.ClientData.YaoSaiPrisonTargetID, (int)player.ClientData.YaoSaiPrisonOptType, true);
			}
			else
			{
				JunTuanClient.getInstance().YaoSaiPrisonOpt(player.ClientData.RoleID, robot.PlayerId, (int)player.ClientData.YaoSaiPrisonOptType, true);
			}
			string roleName;
			if (player.ClientData.YaoSaiPrisonOptType == YaoSaiOptType.jiejiu)
			{
				roleName = player.ClientData.YaoSaiPrisonTargetName;
			}
			else
			{
				roleName = robot.getRoleDataMini().RoleName;
			}
			player.sendCmd(1526, string.Format("{0}:{1}:{2}", (int)player.ClientData.YaoSaiPrisonOptType, 1, roleName), false);
		}

		
		private void processFailed(GameClient player, Robot robot)
		{
			if (player.ClientData.YaoSaiPrisonOptType == YaoSaiOptType.jiejiu || player.ClientData.YaoSaiPrisonOptType == YaoSaiOptType.qiangduo)
			{
				JunTuanClient.getInstance().YaoSaiPrisonOpt(player.ClientData.RoleID, player.ClientData.YaoSaiPrisonTargetID, (int)player.ClientData.YaoSaiPrisonOptType, false);
			}
			else
			{
				JunTuanClient.getInstance().YaoSaiPrisonOpt(player.ClientData.RoleID, robot.PlayerId, (int)player.ClientData.YaoSaiPrisonOptType, false);
			}
			string roleName;
			if (player.ClientData.YaoSaiPrisonOptType == YaoSaiOptType.jiejiu)
			{
				roleName = player.ClientData.YaoSaiPrisonTargetName;
			}
			else
			{
				roleName = robot.getRoleDataMini().RoleName;
			}
			player.sendCmd(1526, string.Format("{0}:{1}:{2}", (int)player.ClientData.YaoSaiPrisonOptType, 0, roleName), false);
		}

		
		public int GetYaoSaiJianYuState(int roleID, int unionLev = 0)
		{
			int result;
			if (this.ManorFriendListOpenUnionLev != 0 && unionLev > 0 && unionLev < this.ManorFriendListOpenUnionLev)
			{
				result = 1;
			}
			else
			{
				KFPrisonRoleData roleData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(roleID);
				if (null == roleData)
				{
					result = 0;
				}
				else if (roleData.OwnerID != 0)
				{
					result = 2;
				}
				else
				{
					result = 1;
				}
			}
			return result;
		}

		
		public void UpdateYaoSaiPrisonRoleData(GameClient client)
		{
			KFUpdatePrisonRole data = new KFUpdatePrisonRole();
			data.RoleID = client.ClientData.RoleID;
			data.RoleName = client.ClientData.RoleName;
			data.UnionLevel = Global.GetUnionLevel2(client);
			data.Faction = client.ClientData.Faction;
			data.RoleSex = (byte)client.ClientData.RoleSex;
			data.Occupation = (byte)client.ClientData.Occupation;
			data.CombatForce = client.ClientData.CombatForce;
			data.ZoneID = client.ClientData.ZoneID;
			PlayerJingJiData jingjiData = JingJiChangManager.getInstance().createJingJiData(client);
			data.PlayerJingJiMirrorData = DataHelper.ObjectToBytes<PlayerJingJiData>(jingjiData);
			JunTuanClient.getInstance().UpdateYaoSaiPrisonRoleData(data);
		}

		
		private void OnLogin(GameClient client)
		{
			this.UpdateYaoSaiLogData(client.ClientData.RoleID);
		}

		
		private void OnLogout(GameClient client)
		{
			JunTuanClient.getInstance().ClearYaoSaiPrisonData(client.ClientData.RoleID);
		}

		
		public void UpdateYaoSaiLogData(int roleID)
		{
			lock (this.ConfigMutex)
			{
				GameClient client = GameManager.ClientMgr.FindClient(roleID);
				if (null != client)
				{
					List<KFPrisonLogData> logListData = JunTuanClient.getInstance().GetYaoSaiPrisonLogData(client.ClientData.RoleID);
					if (null != logListData)
					{
						foreach (KFPrisonLogData item in logListData)
						{
							if (item.State == 0)
							{
								int ret = JunTuanClient.getInstance().UpdateYaoSaiPrisonLogData(client.ClientData.RoleID, item.ID, 1);
								if (ret < 0)
								{
									break;
								}
								this.GiveHuDongAward(client, (PrisonAwardType)item.Param1, item.Param2);
							}
						}
					}
				}
			}
		}

		
		private long GetRevoltCD(GameClient client)
		{
			return Global.GetRoleParamsInt64FromDB(client, "10183");
		}

		
		private void SetRevoltCD(GameClient client, long time)
		{
			Global.SaveRoleParamsInt64ValueToDB(client, "10183", time, true);
		}

		
		public int GetYaoSaiLevelID(int unionlev)
		{
			Dictionary<int, YaoSaiJianYuManorLevelConfig> tempLevelConfigDict = null;
			lock (this.ConfigMutex)
			{
				tempLevelConfigDict = this.LevelConfigDict;
			}
			int levelID = 0;
			foreach (YaoSaiJianYuManorLevelConfig item in tempLevelConfigDict.Values)
			{
				int minUnionLev = item.MinZhuanSheng * 100 + item.MinLevel;
				int maxUnionLev = item.MaxZhuanSheng * 100 + item.MaxLevel;
				if (unionlev >= minUnionLev && unionlev <= maxUnionLev)
				{
					levelID = item.ID;
					break;
				}
			}
			return levelID;
		}

		
		private void FilterHuDongData(GameClient client, List<PrisonHuDongData> hudongDataList, List<KFPrisonRoleData> myFuLuListData)
		{
			List<int> removeHuDongList = new List<int>();
			using (List<PrisonHuDongData>.Enumerator enumerator = hudongDataList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PrisonHuDongData item = enumerator.Current;
					KFPrisonRoleData roleData = myFuLuListData.Find((KFPrisonRoleData x) => x.RoleID == item.RoleID);
					if (null == roleData)
					{
						removeHuDongList.Add(item.RoleID);
					}
				}
			}
			using (List<int>.Enumerator enumerator2 = removeHuDongList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					int item = enumerator2.Current;
					hudongDataList.RemoveAll((PrisonHuDongData x) => x.RoleID == item);
				}
			}
			if (removeHuDongList.Count != 0)
			{
				this.SaveHuDongData(client, hudongDataList);
			}
		}

		
		private List<PrisonHuDongData> GetHuDongData(GameClient client)
		{
			List<PrisonHuDongData> hudongDataList = new List<PrisonHuDongData>();
			List<ulong> saveList = Global.GetRoleParamsUlongListFromDB(client, "20021");
			for (int i = 0; i < saveList.Count - 1; i += 2)
			{
				PrisonHuDongData hudongData = new PrisonHuDongData
				{
					RoleID = (int)saveList[i],
					HuDongStartTicks = (long)saveList[i + 1]
				};
				hudongDataList.Add(hudongData);
			}
			return hudongDataList;
		}

		
		private void SaveHuDongData(GameClient client, List<PrisonHuDongData> hudongDataList)
		{
			List<ulong> saveList = new List<ulong>();
			foreach (PrisonHuDongData item in hudongDataList)
			{
				saveList.Add((ulong)((long)item.RoleID));
				saveList.Add((ulong)item.HuDongStartTicks);
			}
			Global.SaveRoleParamsUlongListToDB(client, saveList, "20021", true);
		}

		
		private void HuDongStart(GameClient client, int fuluID)
		{
			long nowTimeTicks = TimeUtil.NOW();
			List<PrisonHuDongData> hudongDataList = this.GetHuDongData(client);
			PrisonHuDongData hudongData = hudongDataList.Find((PrisonHuDongData x) => x.RoleID == fuluID);
			if (null != hudongData)
			{
				hudongData.HuDongStartTicks = nowTimeTicks;
			}
			else
			{
				hudongDataList.Add(new PrisonHuDongData
				{
					RoleID = fuluID,
					HuDongStartTicks = nowTimeTicks
				});
			}
			this.SaveHuDongData(client, hudongDataList);
		}

		
		private List<int> GetYaoSaiJianYuCount(GameClient client)
		{
			List<int> tempZhengFuCostList = null;
			lock (this.ConfigMutex)
			{
				tempZhengFuCostList = this.ZhengFuCostList;
			}
			int offsetDay = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
			List<int> countList = Global.GetRoleParamsIntListFromDB(client, "20020");
			if (countList.Count != 5)
			{
				countList.Clear();
				countList.Add(offsetDay);
				countList.Add(0);
				countList.Add(0);
				countList.Add(0);
				countList.Add(tempZhengFuCostList[0]);
			}
			if (countList[0] != offsetDay)
			{
				countList[0] = offsetDay;
				countList[1] = 0;
				countList[2] = 0;
				countList[3] = 0;
				countList[4] = tempZhengFuCostList[0];
			}
			return countList;
		}

		
		private void SaveYaoSaiJianYuCount(GameClient client, List<int> countList)
		{
			Global.SaveRoleParamsIntListToDB(client, countList, "20020", true);
		}

		
		private List<PrisonLogData> BuildYaoSaiLogDataForClient(GameClient client)
		{
			List<PrisonLogData> resultList = new List<PrisonLogData>();
			List<KFPrisonLogData> logDataList = JunTuanClient.getInstance().GetYaoSaiPrisonLogData(client.ClientData.RoleID);
			List<PrisonLogData> result;
			if (null == logDataList)
			{
				result = resultList;
			}
			else
			{
				foreach (KFPrisonLogData item in logDataList)
				{
					PrisonLogData log = new PrisonLogData
					{
						ID = item.IntroID,
						Name1 = item.Name1,
						Name2 = item.Name2,
						JiangLiType = item.Param1,
						JiangLiCount = item.Param2
					};
					resultList.Add(log);
				}
				result = resultList;
			}
			return result;
		}

		
		private YaoSaiWorldData BuildYaoSaiWolrdDataForClient(GameClient client, YaoSaiWorldData worldData, KFPrisonRoleData kfpRoleData)
		{
			List<int> countList = this.GetYaoSaiJianYuCount(client);
			worldData.jiejiuCount = countList[1];
			worldData.zhenfuCount = countList[3];
			worldData.zhenfuLeftCount = countList[4];
			YaoSaiWorldData result;
			if (null == kfpRoleData)
			{
				result = worldData;
			}
			else
			{
				worldData.state = ((kfpRoleData.OwnerID != 0) ? 1 : 0);
				this.TransKFPrisonRoleDataToPrisonRoleData(kfpRoleData, worldData.Mine);
				if (kfpRoleData.OwnerID != 0)
				{
					KFPrisonRoleData ownerRoleData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(kfpRoleData.OwnerID);
					this.TransKFPrisonRoleDataToPrisonRoleData(ownerRoleData, worldData.Master);
				}
				List<KFPrisonRoleData> fuluList = JunTuanClient.getInstance().GetYaoSaiPrisonFuLuData(kfpRoleData.RoleID);
				if (null != fuluList)
				{
					foreach (KFPrisonRoleData item in fuluList)
					{
						PrisonFuLuData fuluData = new PrisonFuLuData();
						fuluData.RoleID = item.RoleID;
						fuluData.Name = item.RoleName;
						fuluData.Level = (item.UnionLevel - 1) % 100 + 1;
						fuluData.ChangeLevel = (item.UnionLevel - 1) / 100;
						fuluData.ZoneID = item.ZoneID;
						fuluData.Occupation = (int)item.Occupation;
						fuluData.RoleSex = (int)item.RoleSex;
						worldData.FuLuList.Add(fuluData);
					}
				}
				result = worldData;
			}
			return result;
		}

		
		private void TransKFPrisonRoleDataToPrisonRoleData(KFPrisonRoleData kfpRoleData, PrisonRoleData prisonRoleData)
		{
			if (kfpRoleData != null && null != prisonRoleData)
			{
				prisonRoleData.RoleID = kfpRoleData.RoleID;
				prisonRoleData.Name = kfpRoleData.RoleName;
				prisonRoleData.Level = (kfpRoleData.UnionLevel - 1) % 100 + 1;
				prisonRoleData.ChangeLevel = (kfpRoleData.UnionLevel - 1) / 100;
				prisonRoleData.ZoneID = kfpRoleData.ZoneID;
				prisonRoleData.Occupation = (int)kfpRoleData.Occupation;
				prisonRoleData.RoleSex = (int)kfpRoleData.RoleSex;
				prisonRoleData.CombatForce = kfpRoleData.CombatForce;
			}
		}

		
		private PrisonMainData BuildYaoSaiMainDataForClient(GameClient client)
		{
			PrisonMainData mainData = new PrisonMainData();
			KFPrisonRoleData ownerRoleData = null;
			List<KFPrisonRoleData> myFuLuListData = null;
			KFPrisonRoleData myRoleData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(client.ClientData.RoleID);
			if (myRoleData != null && myRoleData.OwnerID > 0)
			{
				ownerRoleData = JunTuanClient.getInstance().GetYaoSaiPrisonRoleData(myRoleData.OwnerID);
			}
			if (myRoleData != null && myRoleData.RoleID == client.ClientData.RoleID)
			{
				myFuLuListData = JunTuanClient.getInstance().GetYaoSaiPrisonFuLuData(client.ClientData.RoleID);
			}
			List<int> countList = this.GetYaoSaiJianYuCount(client);
			mainData.JieJiuCount = countList[1];
			mainData.ZhengFuCount = countList[3];
			mainData.ZhengFuLeftCount = countList[4];
			mainData.LaoDongCount = countList[2];
			long RevoltCDPass = TimeUtil.NOW() - this.GetRevoltCD(client);
			long RevoltCD = (long)(60000 * this.FightCDTime);
			mainData.RevoltCD = ((RevoltCDPass >= RevoltCD) ? 0L : (RevoltCD - RevoltCDPass));
			if (myRoleData != null && myRoleData.RoleID == client.ClientData.RoleID)
			{
				if (myRoleData.OwnerID == 0 || ownerRoleData == null || 0 == ownerRoleData.RoleID)
				{
					this.TransKFPrisonRoleDataToPrisonRoleData(myRoleData, mainData.roleData);
					mainData.MineFuLuState = 0;
				}
				else
				{
					this.TransKFPrisonRoleDataToPrisonRoleData(ownerRoleData, mainData.roleData);
					mainData.MineFuLuState = 1;
				}
			}
			if (null != myFuLuListData)
			{
				long nowTimeTicks = TimeUtil.NOW();
				List<PrisonHuDongData> hudongDataList = this.GetHuDongData(client);
				this.FilterHuDongData(client, hudongDataList, myFuLuListData);
				myFuLuListData.Sort(delegate(KFPrisonRoleData left, KFPrisonRoleData right)
				{
					int result;
					if (left.FuLuTime < right.FuLuTime)
					{
						result = -1;
					}
					else if (left.FuLuTime > right.FuLuTime)
					{
						result = 1;
					}
					else
					{
						result = 0;
					}
					return result;
				});
				foreach (KFPrisonRoleData item in myFuLuListData)
				{
					PrisonFuLuData fuluData = new PrisonFuLuData();
					fuluData.RoleID = item.RoleID;
					fuluData.Name = item.RoleName;
					fuluData.Level = (item.UnionLevel - 1) % 100 + 1;
					fuluData.ChangeLevel = (item.UnionLevel - 1) / 100;
					fuluData.ZoneID = item.ZoneID;
					fuluData.Occupation = (int)item.Occupation;
					fuluData.RoleSex = (int)item.RoleSex;
					PrisonHuDongData hudongData = hudongDataList.Find((PrisonHuDongData x) => x.RoleID == fuluData.RoleID);
					if (null != hudongData)
					{
						long laodongTime = nowTimeTicks - hudongData.HuDongStartTicks;
						if (laodongTime < (long)(this.FuLuHuDongUseTime * 60000))
						{
							fuluData.LaoDongState = 1;
							fuluData.LaoDongTime = (long)(this.FuLuHuDongUseTime * 60000) - laodongTime;
						}
					}
					mainData.fuLuDataList.Add(fuluData);
				}
			}
			return mainData;
		}

		
		public int GetManorFriendListOpenUnionLev()
		{
			return this.ManorFriendListOpenUnionLev;
		}

		
		public bool InitConfig()
		{
			string StringUnionLev = GameManager.systemParamsList.GetParamValueByName("ManorFriendListOpen");
			string[] UnionLevFields = StringUnionLev.Split(new char[]
			{
				','
			});
			if (UnionLevFields.Length == 2)
			{
				this.ManorFriendListOpenUnionLev = Global.GetUnionLevel2(Global.SafeConvertToInt32(UnionLevFields[0]), Global.SafeConvertToInt32(UnionLevFields[1]));
			}
			List<int> tempZhengFuCostList = new List<int>();
			string StringManorCatch = GameManager.systemParamsList.GetParamValueByName("ManorCatch");
			string[] ManorCatchFields = StringManorCatch.Split(new char[]
			{
				','
			});
			foreach (string item in ManorCatchFields)
			{
				tempZhengFuCostList.Add(Global.SafeConvertToInt32(item));
			}
			lock (this.ConfigMutex)
			{
				this.ZhengFuCostList = tempZhengFuCostList;
			}
			List<int> tempHelpCostList = new List<int>();
			string StringManorHelp = GameManager.systemParamsList.GetParamValueByName("ManorHelp");
			string[] ManorHelpFields = StringManorHelp.Split(new char[]
			{
				','
			});
			foreach (string item in ManorHelpFields)
			{
				tempHelpCostList.Add(Global.SafeConvertToInt32(item));
			}
			lock (this.ConfigMutex)
			{
				this.HelpCostList = tempHelpCostList;
			}
			string StringManorCommandAward = GameManager.systemParamsList.GetParamValueByName("ManorCommandAward");
			string[] ManorCommandFields = StringManorCommandAward.Split(new char[]
			{
				','
			});
			if (ManorCommandFields.Length == 3)
			{
				this.FuLuHuDongCount = Global.SafeConvertToInt32(ManorCommandFields[0]);
				this.FuLuAwardCount = Global.SafeConvertToInt32(ManorCommandFields[1]);
				this.FuLuHuDongUseTime = Global.SafeConvertToInt32(ManorCommandFields[2]);
			}
			string StringManorCommandAgainst = GameManager.systemParamsList.GetParamValueByName("ManorCommandAgainst");
			if (null != StringManorCommandAgainst)
			{
				string[] AgainstFields = StringManorCommandAgainst.Split(new char[]
				{
					','
				});
				if (AgainstFields.Length == 2)
				{
					this.FightCDTime = Global.SafeConvertToInt32(AgainstFields[0]);
					this.FightCDClearCost = Global.SafeConvertToInt32(AgainstFields[1]);
				}
			}
			string StringManorSearchCost = GameManager.systemParamsList.GetParamValueByName("ManorSearchCost");
			this.ManorSearchCost = Global.SafeConvertToInt32(StringManorSearchCost);
			this.LoadFallGoodsItemList();
			return this.LoadYaoSaiJianYuManorLevelConfig() && this.LoadYaoSaiJianYuCommandConfigFile();
		}

		
		public void LoadFallGoodsItemList()
		{
			string StringFallGoodsItem = GameManager.systemParamsList.GetParamValueByName("ManorWorkAward");
			if (!string.IsNullOrEmpty(StringFallGoodsItem))
			{
				List<FallGoodsItem> tempFallGoodsItemConfigList = new List<FallGoodsItem>();
				int basePercent = 0;
				FallGoodsItem fallGoodsItem = null;
				string[] goodsFields = StringFallGoodsItem.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < goodsFields.Length; i++)
				{
					string item = goodsFields[i].Trim();
					if (!(item == ""))
					{
						string[] itemFields = item.Split(new char[]
						{
							','
						});
						if (itemFields.Length == 7)
						{
							fallGoodsItem = null;
							try
							{
								fallGoodsItem = new FallGoodsItem
								{
									GoodsID = Convert.ToInt32(itemFields[0]),
									BasePercent = basePercent,
									SelfPercent = (int)(Convert.ToDouble(itemFields[1]) * 100000.0),
									Binding = Convert.ToInt32(itemFields[2]),
									LuckyRate = (int)Convert.ToDouble(itemFields[3]),
									FallLevelID = Convert.ToInt32(itemFields[4]),
									ZhuiJiaID = Convert.ToInt32(itemFields[5]),
									ExcellencePropertyID = Convert.ToInt32(itemFields[6])
								};
								basePercent += fallGoodsItem.SelfPercent;
							}
							catch (Exception)
							{
								fallGoodsItem = null;
							}
							if (null == fallGoodsItem)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("解析要塞监狱掉落项时发生错误", new object[0]), null, true);
							}
							else
							{
								tempFallGoodsItemConfigList.Add(fallGoodsItem);
								if (basePercent > 100000)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("解析要塞监狱掉落项时发生概率溢出100000错误", new object[0]), null, true);
								}
							}
						}
					}
				}
				lock (this.ConfigMutex)
				{
					this.FallGoodsItemConfigList = tempFallGoodsItemConfigList;
				}
			}
		}

		
		public bool LoadYaoSaiJianYuManorLevelConfig()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/ManorLevel.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/ManorLevel.xml"));
				if (null == xml)
				{
					return false;
				}
				Dictionary<int, YaoSaiJianYuManorLevelConfig> tempLevelConfigDict = new Dictionary<int, YaoSaiJianYuManorLevelConfig>();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					YaoSaiJianYuManorLevelConfig myData = new YaoSaiJianYuManorLevelConfig();
					myData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					string tempValue = Global.GetSafeAttributeStr(xmlItem, "MinLevel");
					string[] ValueFields = tempValue.Split(new char[]
					{
						'|'
					});
					if (ValueFields.Length == 2)
					{
						myData.MinZhuanSheng = Global.SafeConvertToInt32(ValueFields[0]);
						myData.MinLevel = Global.SafeConvertToInt32(ValueFields[1]);
					}
					tempValue = Global.GetSafeAttributeStr(xmlItem, "MaxLevel");
					ValueFields = tempValue.Split(new char[]
					{
						'|'
					});
					if (ValueFields.Length == 2)
					{
						myData.MaxZhuanSheng = Global.SafeConvertToInt32(ValueFields[0]);
						myData.MaxLevel = Global.SafeConvertToInt32(ValueFields[1]);
					}
					tempLevelConfigDict[myData.ID] = myData;
				}
				lock (this.ConfigMutex)
				{
					this.LevelConfigDict = tempLevelConfigDict;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/ManorLevel.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public bool LoadYaoSaiJianYuCommandConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/ManorCommand.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/ManorCommand.xml"));
				if (null == xml)
				{
					return false;
				}
				Dictionary<int, YaoSaiJianYuManorCommandConfig> tempCommandAwardConfigDict = new Dictionary<int, YaoSaiJianYuManorCommandConfig>();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					YaoSaiJianYuManorCommandConfig myData = new YaoSaiJianYuManorCommandConfig();
					myData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					string tempValue = Global.GetSafeAttributeStr(xmlItem, "Award");
					string[] ValueFields = tempValue.Split(new char[]
					{
						'|'
					});
					if (ValueFields.Length == 3)
					{
						myData.AwardType = (PrisonAwardType)Global.SafeConvertToInt32(ValueFields[0]);
						myData.OwnerFactor = Global.SafeConvertToDouble(ValueFields[1]);
						myData.FuLuFactor = Global.SafeConvertToDouble(ValueFields[2]);
					}
					tempCommandAwardConfigDict[myData.ID] = myData;
				}
				lock (this.ConfigMutex)
				{
					this.CommandAwardConfigDict = tempCommandAwardConfigDict;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/ManorCommand.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		private const string YaoSaiJianYu_ManorCommandFileName = "Config/ManorCommand.xml";

		
		private const string YaoSaiJianYu_ManorLevelFileName = "Config/ManorLevel.xml";

		
		public const int YaoSaiPrisonFuLuMaxNum = 4;

		
		private object ConfigMutex = new object();

		
		private int ManorFriendListOpenUnionLev = 0;

		
		private List<int> ZhengFuCostList = new List<int>();

		
		private List<int> HelpCostList = new List<int>();

		
		private int FuLuHuDongCount = 20;

		
		private int FuLuAwardCount = 20;

		
		private int FuLuHuDongUseTime = 30;

		
		private int FightCDTime = 20;

		
		private int FightCDClearCost = 20;

		
		private int ManorSearchCost = 10000;

		
		private Dictionary<int, YaoSaiJianYuManorCommandConfig> CommandAwardConfigDict = new Dictionary<int, YaoSaiJianYuManorCommandConfig>();

		
		public Dictionary<int, YaoSaiJianYuManorLevelConfig> LevelConfigDict = new Dictionary<int, YaoSaiJianYuManorLevelConfig>();

		
		public List<FallGoodsItem> FallGoodsItemConfigList = new List<FallGoodsItem>();

		
		private static YaoSaiJianYuManager instance = new YaoSaiJianYuManager();
	}
}
