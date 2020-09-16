using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Interface;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.MUWings;
using GameServer.Logic.Reborn;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	public class RebornManager : IManager, ICmdProcessorEx, ICmdProcessor, IManager2, IEventListener
	{
		
		public static RebornManager getInstance()
		{
			return RebornManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig(false);
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("RebornManager.TimerProc", new EventHandler(this.TimerProc)), 2000, 5000);
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1710, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1712, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1713, 2, 2, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1714, 2, 2, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2030, 4, 4, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2031, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2032, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2033, 3, 3, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2046, 2, 2, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2047, 2, 2, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2051, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2053, 2, 2, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2059, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2050, 4, 4, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2054, 3, 3, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2055, 2, 2, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2056, 2, 2, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2057, 6, 6, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2058, 3, 3, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2060, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2095, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2096, 1, 1, RebornManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(14, RebornManager.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(14, RebornManager.getInstance());
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
			bool isOpen = GlobalNew.IsGongNengOpened(client, GongNengIDs.Reborn, false);
			if (2046 == nID || 2060 == nID)
			{
				isOpen = true;
			}
			bool result2;
			if (!isOpen)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result2 = true;
			}
			else
			{
				switch (nID)
				{
				case 1710:
					return this.ProcessRebornUpgradeCmd(client, nID, bytes, cmdParams);
				case 1711:
					break;
				case 1712:
					return this.ProcessRebornAdmireDataCmd(client, nID, bytes, cmdParams);
				case 1713:
					return this.ProcessRebornAdmireCmd(client, nID, bytes, cmdParams);
				case 1714:
					return this.ProcessRebornRankDataCmd(client, nID, bytes, cmdParams);
				default:
					switch (nID)
					{
					case 2030:
						if (cmdParams == null || cmdParams.Length != 4)
						{
							return false;
						}
						try
						{
							int RoleID = Convert.ToInt32(cmdParams[0]);
							int StampID = Convert.ToInt32(cmdParams[1]);
							int StampType = Convert.ToInt32(cmdParams[2]);
							int UpNum = Convert.ToInt32(cmdParams[3]);
							int ZhuID;
							int MainYinJiID;
							int UsePoint;
							int result = Convert.ToInt32(RebornStamp.ProcessRebornYinJiLevelUp(client, RoleID, StampID, StampType, UpNum, out ZhuID, out MainYinJiID, out UsePoint));
							client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								result,
								ZhuID,
								MainYinJiID,
								UsePoint
							}), false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_YINJI_LEVELUP", false, false);
						}
						break;
					case 2031:
						if (cmdParams == null || cmdParams.Length != 1)
						{
							return false;
						}
						try
						{
							int RoleID = Convert.ToInt32(cmdParams[0]);
							RebornStampData dbInfo;
							int result = Convert.ToInt32(RebornStamp.ProcessRebornYinJiGetInfo(client, RoleID, out dbInfo));
							client.ClientData.RebornYinJi = dbInfo;
							client.sendCmd<RebornStampData>(nID, dbInfo, false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_YINJI_GETINFO", false, false);
						}
						break;
					case 2032:
						if (cmdParams == null || cmdParams.Length != 1)
						{
							return false;
						}
						try
						{
							int RoleID = Convert.ToInt32(cmdParams[0]);
							int result = Convert.ToInt32(RebornStamp.ProcessRebornYinJiReset(client, RoleID));
							client.sendCmd(nID, string.Format("{0}", result), false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_YINJI_RESET", false, false);
						}
						break;
					case 2033:
						if (cmdParams == null || cmdParams.Length != 3)
						{
							return false;
						}
						try
						{
							int RoleID = Convert.ToInt32(cmdParams[0]);
							int StampType2 = Convert.ToInt32(cmdParams[1]);
							int StampType3 = Convert.ToInt32(cmdParams[2]);
							int result = Convert.ToInt32(RebornStamp.ProcessRebornYinJiChoose(client, RoleID, StampType2, StampType3));
							client.sendCmd(nID, string.Format("{0}", result), false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_YINJI_CHOOSE", false, false);
						}
						break;
					case 2034:
					case 2035:
					case 2036:
					case 2037:
					case 2038:
					case 2039:
					case 2040:
					case 2041:
					case 2042:
					case 2043:
					case 2044:
					case 2045:
					case 2048:
					case 2049:
					case 2052:
						break;
					case 2046:
						if (cmdParams == null || cmdParams.Length != 2)
						{
							return false;
						}
						try
						{
							int RoleID = Convert.ToInt32(cmdParams[0]);
							string strGoodsID = cmdParams[1];
							int result = Convert.ToInt32(RebornEquip.SaleRebornEquipProcess(client, RoleID, strGoodsID));
							client.sendCmd(nID, string.Format("{0}", result), false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_SALEONE", false, false);
						}
						break;
					case 2047:
						if (cmdParams == null || cmdParams.Length != 2)
						{
							return false;
						}
						try
						{
							int RoleID = Convert.ToInt32(cmdParams[0]);
							string strGoodsID = cmdParams[1];
							int result = Convert.ToInt32(RebornEquip.SaleStoreRebornEquipProcess(client, RoleID, strGoodsID));
							client.sendCmd(nID, string.Format("{0}", result), false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_SALEMORE", false, false);
						}
						break;
					case 2050:
						if (cmdParams == null || cmdParams.Length != 4)
						{
							return false;
						}
						try
						{
							int DBID = Convert.ToInt32(cmdParams[0]);
							int Number = Convert.ToInt32(cmdParams[1]);
							int Bind = Convert.ToInt32(cmdParams[2]);
							int Reset = Convert.ToInt32(cmdParams[3]);
							string str;
							int bind;
							int result = Convert.ToInt32(RebornStone.ProessMakeRebornEquipHold(client, DBID, Bind, Reset, Number, out str, out bind));
							client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, str, bind), false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_DAKONG", false, false);
						}
						break;
					case 2051:
						if (cmdParams == null || cmdParams.Length != 1)
						{
							return false;
						}
						try
						{
							int RoleID = Convert.ToInt32(cmdParams[0]);
							int result = Convert.ToInt32(RebornEquip.RebornEquipShowProcess(client, RoleID));
							client.sendCmd(nID, string.Format("{0}:{1}", result, client.ClientData.RebornShowEquip), false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_SHOW", false, false);
						}
						break;
					case 2053:
						if (cmdParams == null || cmdParams.Length != 2)
						{
							return false;
						}
						try
						{
							int RoleID = Convert.ToInt32(cmdParams[0]);
							int DBID = Convert.ToInt32(cmdParams[1]);
							int result = Convert.ToInt32(RebornEquip.RebornEquipAdvanceProcess(client, RoleID, DBID));
							client.sendCmd(nID, string.Format("{0}", result), false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_UPDATA", false, false);
						}
						break;
					case 2054:
						if (cmdParams == null || cmdParams.Length != 3)
						{
							return false;
						}
						try
						{
							int EquipDBID = Convert.ToInt32(cmdParams[0]);
							int StoneDBID = Convert.ToInt32(cmdParams[1]);
							int Number = Convert.ToInt32(cmdParams[2]);
							string prop = "";
							int bind;
							int result = Convert.ToInt32(RebornStone.ProessRebornStoneInlayHold(client, EquipDBID, StoneDBID, Number, out prop, out bind));
							client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, prop, bind), false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_INLAY", false, false);
						}
						break;
					case 2055:
						if (cmdParams == null || cmdParams.Length != 2)
						{
							return false;
						}
						try
						{
							int EquipDBID = Convert.ToInt32(cmdParams[0]);
							int Site = Convert.ToInt32(cmdParams[1]);
							string prop = "";
							int result = Convert.ToInt32(RebornStone.ProessRebornStoneDisInlayHold(client, EquipDBID, Site, out prop));
							client.sendCmd(nID, string.Format("{0}:{1}", result, prop), false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_DEMOUNT", false, false);
						}
						break;
					case 2056:
						if (cmdParams == null || cmdParams.Length != 2)
						{
							return false;
						}
						try
						{
							int GoodID = Convert.ToInt32(cmdParams[0]);
							int Count = Convert.ToInt32(cmdParams[1]);
							int result = Convert.ToInt32(RebornStone.ProessRebornStoneComplex(client, GoodID, Count));
							client.sendCmd(nID, string.Format("{0}", result), false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_STONECOMPLEX", false, false);
						}
						break;
					case 2057:
						if (cmdParams == null || cmdParams.Length != 6)
						{
							return false;
						}
						try
						{
							int DBID2 = Convert.ToInt32(cmdParams[0]);
							int Num = Convert.ToInt32(cmdParams[1]);
							int DBID3 = Convert.ToInt32(cmdParams[2]);
							int Num2 = Convert.ToInt32(cmdParams[3]);
							int DBID4 = Convert.ToInt32(cmdParams[4]);
							int Num3 = Convert.ToInt32(cmdParams[5]);
							int result = Convert.ToInt32(RebornStone.RebornXuanCaiComplexStone(client, DBID2, Num, DBID3, Num2, DBID4, Num3));
							client.sendCmd(nID, string.Format("{0}", result), false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_XUANCAICOMPLEX", false, false);
						}
						break;
					case 2058:
						if (cmdParams == null || cmdParams.Length != 3)
						{
							return false;
						}
						try
						{
							int GoodsID = Convert.ToInt32(cmdParams[0]);
							int Count = Convert.ToInt32(cmdParams[1]);
							int Bind = Convert.ToInt32(cmdParams[2]);
							int result = Convert.ToInt32(RebornStone.RebornStoneResolve(client, GoodsID, Count, Bind));
							client.sendCmd(nID, string.Format("{0}", result), false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_STONERESOLVE", false, false);
						}
						break;
					case 2059:
						if (cmdParams == null || cmdParams.Length != 1)
						{
							return false;
						}
						try
						{
							int RoleID = Convert.ToInt32(cmdParams[0]);
							int result = Convert.ToInt32(RebornEquip.RebornEquipShowModelProcess(client, RoleID));
							client.sendCmd(nID, string.Format("{0}:{1}", result, client.ClientData.RebornShowModel), false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_SHOWMODEL", false, false);
						}
						break;
					case 2060:
						if (cmdParams == null || cmdParams.Length != 1)
						{
							return false;
						}
						try
						{
							string strGoodsID = cmdParams[0];
							int result = Convert.ToInt32(RebornStone.SaleRebornStoneProcess(client, strGoodsID));
							client.sendCmd(nID, string.Format("{0}", result), false);
						}
						catch (Exception ex)
						{
							client.sendCmd(nID, "-1", false);
							DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORN_BATCH_STONERESOLVE", false, false);
						}
						break;
					default:
						switch (nID)
						{
						case 2095:
							if (cmdParams == null || cmdParams.Length != 1)
							{
								return false;
							}
							try
							{
								int HoleID = Convert.ToInt32(cmdParams[0]);
								int ClientAble;
								int result = Convert.ToInt32(RebornEquip.RebornEquipHolePerfusionProcess(client, HoleID, out ClientAble));
								client.sendCmd(nID, string.Format("{0}:{1}", result, ClientAble), false);
							}
							catch (Exception ex)
							{
								client.sendCmd(nID, "-1", false);
								DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORNHOLE_PERFUSION", false, false);
							}
							break;
						case 2096:
							if (cmdParams == null || cmdParams.Length != 1)
							{
								return false;
							}
							try
							{
								int HoleID = Convert.ToInt32(cmdParams[0]);
								int ClientAble;
								int ClientLevel;
								int result = Convert.ToInt32(RebornEquip.RebornEquipHoleAbschreckenProcess(client, HoleID, out ClientLevel, out ClientAble));
								client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, ClientLevel, ClientAble), false);
							}
							catch (Exception ex)
							{
								client.sendCmd(nID, "-1", false);
								DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REBORNHOLE_ABSCHRECKEN", false, false);
							}
							break;
						}
						break;
					}
					break;
				}
				result2 = true;
			}
			return result2;
		}

		
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 14)
			{
				PlayerInitGameEventObject playerInitGameEventObject = eventObject as PlayerInitGameEventObject;
				if (null != playerInitGameEventObject)
				{
					this.OnInitGame(playerInitGameEventObject.getPlayer());
				}
			}
		}

		
		public bool ProcessRebornUpgradeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Convert.ToInt32(cmdParams[0]);
				Dictionary<int, RebornStageInfo> tempRebornStageInfoDict = this.RebornStageInfoDict;
				string strcmd;
				if (client.ClientData.RebornCount >= tempRebornStageInfoDict.Count)
				{
					strcmd = string.Format("{0}:{1}:{2}", -12, roleID, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				if (client.ClientData.HideGM > 0)
				{
					strcmd = string.Format("{0}:{1}:{2}", -12, roleID, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				int oldNum = client.ClientData.RebornCount;
				string resList = "";
				RebornStageInfo rebornInfo;
				if (!tempRebornStageInfoDict.TryGetValue(client.ClientData.RebornCount + 1, out rebornInfo))
				{
					strcmd = string.Format("{0}:{1}:{2}", -30, roleID, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				int ret = this.CheckRebornUpgradeLimit(client);
				if (ret != 0)
				{
					strcmd = string.Format("{0}:{1}:{2}", -1, roleID, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				KuaFuWorldRoleData kuaFuWorldRoleData = new KuaFuWorldRoleData
				{
					LocalRoleID = client.ClientData.LocalRoleID,
					UserID = client.strUserID,
					WorldRoleID = client.ClientData.WorldRoleID,
					Channel = client.ClientData.Channel,
					PTID = client.ClientData.ServerPTID,
					ServerID = client.ServerId,
					ZoneID = client.ClientData.ZoneID
				};
				int result = KuaFuWorldClient.getInstance().RegPTKuaFuRoleData(ref kuaFuWorldRoleData);
				if (result < 0)
				{
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, roleID, client.ClientData.RebornCount), false);
					return true;
				}
				result = KuaFuWorldClient.getInstance().Reborn_RoleReborn(client.ClientData.ServerPTID, client.ClientData.RoleID, client.ClientData.RoleName, 1);
				if (result < 0)
				{
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, roleID, client.ClientData.RebornCount), false);
					return true;
				}
				if (0 == client.ClientData.RebornCount)
				{
					client.ClientData.RebornLevel = 1;
				}
				client.ClientData.RebornCount++;
				lock (client.ClientData.PropPointMutex)
				{
					if (rebornInfo.RebornPoint > 0)
					{
						GameManager.ClientMgr.ModifyRebornYinJiPointValue(client, rebornInfo.RebornPoint, "重生", true, true, false);
					}
				}
				Global.SaveRoleParamsInt32ValueToDB(client, "10240", client.ClientData.RebornCount, true);
				Global.SaveRoleParamsInt32ValueToDB(client, "10241", client.ClientData.RebornLevel, true);
				long nExperienceNow = client.ClientData.RebornExperience;
				client.ClientData.RebornExperience = 0L;
				if (nExperienceNow <= 0L)
				{
					this.NotifySelfExperience(client, nExperienceNow);
					GameManager.ClientMgr.ModifyRebornExpMaxAddValue(client, 0L, "", MoneyTypes.RebornExpMonster, false, true, false);
					GameManager.ClientMgr.ModifyRebornExpMaxAddValue(client, 0L, "", MoneyTypes.RebornExpSale, false, true, false);
				}
				else
				{
					this.ProcessRoleExperience(client, nExperienceNow, MoneyTypes.RebornExp, false, true, false, "none");
				}
				if (1 == client.ClientData.RebornCount)
				{
					this.AutoGiveRebornInitGoods(client);
				}
				this.InitPlayerRebornPorperty(client);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				if (rebornInfo.AwardGoods != null && rebornInfo.AwardGoods.Items != null)
				{
					foreach (AwardsItemData item in rebornInfo.AwardGoods.Items)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, "重生", "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
					}
				}
				EventLogManager.AddRebornEvent(client, oldNum, client.ClientData.ChangeLifeCount, resList);
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, client.ClientData.RebornCount);
				client.sendCmd(nID, strcmd, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessRebornAdmireDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Convert.ToInt32(cmdParams[0]);
				Dictionary<int, List<KFRebornRankInfo>> RebornRankDict;
				lock (this.RebornSyncDataCache)
				{
					RebornRankDict = this.RebornSyncDataCache.RebornRankDict.V;
				}
				Dictionary<int, RebornRankAdmireData> AdmireDataDict = new Dictionary<int, RebornRankAdmireData>();
				for (int loop = 0; loop <= 3; loop++)
				{
					RebornRankAdmireData myData = new RebornRankAdmireData();
					List<KFRebornRankInfo> rankInfoList = null;
					if (RebornRankDict.TryGetValue(loop, out rankInfoList) && rankInfoList != null && rankInfoList.Count > 0)
					{
						KFRebornRankInfo Top = rankInfoList[0];
						KFRebornRoleData rebornRoleData = KuaFuWorldClient.getInstance().Reborn_GetRebornRoleData(Top.PtID, Top.Key);
						if (rebornRoleData != null && null != rebornRoleData.RoleData4Selector)
						{
							myData.RoleData4Selector = DataHelper.BytesToObject<RoleData4Selector>(rebornRoleData.RoleData4Selector, 0, rebornRoleData.RoleData4Selector.Length);
							myData.Value = Top.Value;
							myData.PtID = Top.PtID;
							myData.Param = Top.Param2;
						}
					}
					myData.AdmireCount = this.GetRebornAdmireCount(client, loop);
					AdmireDataDict[loop] = myData;
				}
				client.sendCmd<Dictionary<int, RebornRankAdmireData>>(nID, AdmireDataDict, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessRebornAdmireCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Convert.ToInt32(cmdParams[0]);
				int rankType = Convert.ToInt32(cmdParams[1]);
				string strcmd;
				if (this.EveryDayMaxRebornExp == null || rankType < 0 || rankType >= this.EveryDayMaxRebornExp.Length)
				{
					strcmd = string.Format("{0}:{1}", -2, rankType);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				if (this.GetRebornAdmireCount(client, rankType) >= 1)
				{
					strcmd = string.Format("{0}:{1}", -3, rankType);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				this.ProcessRoleExperience(client, (long)this.EveryDayMaxRebornExp[rankType], MoneyTypes.RebornExp, true, true, false, "膜拜");
				this.ProcessIncreaseRebornAdmireCount(client, rankType);
				strcmd = string.Format("{0}:{1}", 1, rankType);
				client.sendCmd(nID, strcmd, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessRebornRankDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Convert.ToInt32(cmdParams[0]);
				int rankType = Convert.ToInt32(cmdParams[1]);
				Dictionary<int, List<KFRebornRankInfo>> RebornRankDict;
				lock (this.RebornSyncDataCache)
				{
					RebornRankDict = this.RebornSyncDataCache.RebornRankDict.V;
				}
				RebornRankInfoToClient rankInfo = new RebornRankInfoToClient();
				List<KFRebornRankInfo> kfRankList = null;
				if (RebornRankDict.TryGetValue(rankType, out kfRankList) && null != kfRankList)
				{
					foreach (KFRebornRankInfo item in kfRankList)
					{
						rankInfo.rankList.Add(new RebornRankInfo
						{
							Key = item.Key,
							Value = item.Value,
							Param1 = item.Param1,
							Param2 = item.Param2,
							UserPtID = Data.GetUserPtIDByUserID(item.UserID)
						});
					}
				}
				rankInfo.RankType = rankType;
				client.sendCmd<RebornRankInfoToClient>(nID, rankInfo, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public void AutoGiveRebornInitGoods(GameClient client)
		{
			if (null == client)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("client不存在，服务器无法给与重生初始装备", new object[0]), null, true);
			}
			else
			{
				int nRoleID = client.ClientData.RoleID;
				try
				{
					List<List<int>> giveEquip = ConfigParser.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("RebornInitialEquip"), true, '|', ',');
					if (null == giveEquip)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("重生初始化装备默认数据报错.RoleID{0}", nRoleID), null, true);
					}
					else if (giveEquip.Count <= 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("重生初始化装备数量为空.RoleID{0}", nRoleID), null, true);
					}
					else
					{
						bool bRingFalg = false;
						for (int i = 0; i < giveEquip.Count; i++)
						{
							int nGoodID = giveEquip[i][0];
							int nNum = giveEquip[i][1];
							int nBind = giveEquip[i][2];
							int nIntensify = giveEquip[i][3];
							int nAppendPropLev = giveEquip[i][4];
							int nLuck = giveEquip[i][5];
							int nExcellence = giveEquip[i][6];
							SystemXmlItem sytemGoodsItem = null;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(nGoodID, out sytemGoodsItem))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("重生初始化装备数量ID不存在:RoleID{0},GoodsID={1}", nRoleID, nGoodID), null, true);
							}
							else if (!Global.IsRoleOccupationMatchGoods(client, nGoodID))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("重生初始化装备与职业不符RoleID{0}, 物品id{1}.", nRoleID, nGoodID), null, true);
							}
							else if (1 != nNum)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("重生初始化装备数量必须为1件RoleID{0}, 数量{1}.", nRoleID, nNum), null, true);
							}
							else
							{
								int nSeriralID = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, nGoodID, nNum, 0, "", nIntensify, nBind, 15000, "", false, 1, "自动给于重生装备", "1900-01-01 12:00:00", 0, 0, nLuck, 0, nExcellence, nAppendPropLev, 0, null, null, 0, true);
								if (nSeriralID <= 0)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("重生初始化装备数量[AddGoodsDBCommand]失败.RoleID{0}", nRoleID), null, true);
								}
								else
								{
									GoodsData newEquip = Global.GetRebornGoodsByDbID(client, nSeriralID);
									if (null == newEquip)
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("重生初始化装备数量[GetGoodsByID]失败.RoleID{0}", nRoleID), null, true);
									}
									else
									{
										int nBagIndex = 0;
										int nCatetoriy = Global.GetGoodsCatetoriy(newEquip.GoodsID);
										if (nCatetoriy == 36 && bRingFalg)
										{
											nBagIndex++;
										}
										string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
										{
											client.ClientData.RoleID,
											1,
											newEquip.Id,
											newEquip.GoodsID,
											1,
											newEquip.Site,
											newEquip.GCount,
											nBagIndex,
											""
										});
										TCPProcessCmdResults eErrorCode = Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
										if (TCPProcessCmdResults.RESULT_FAILED == eErrorCode)
										{
											LogManager.WriteLog(LogTypes.Error, string.Format("重生初始化装备数量[ModifyGoodsByCmdParams]失败.RoleID{0}", nRoleID), null, true);
										}
										else if (nCatetoriy == 36)
										{
											bRingFalg = true;
										}
									}
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
			}
		}

		
		public bool CheckRebornUpgradeIcon(GameClient client)
		{
			return this.CheckRebornUpgradeLimit(client) == 0;
		}

		
		public int CheckRebornUpgradeLimit(GameClient client)
		{
			int ret = 0;
			Dictionary<int, RebornStageInfo> tempRebornStageInfoDict = this.RebornStageInfoDict;
			RebornStageInfo rebornInfo;
			int result;
			if (!tempRebornStageInfoDict.TryGetValue(client.ClientData.RebornCount + 1, out rebornInfo))
			{
				ret = -3;
				result = ret;
			}
			else if (rebornInfo.NeedZhuanSheng.Length == 2 && Global.GetUnionLevel2(client) < Global.GetUnionLevel2(rebornInfo.NeedZhuanSheng[0], rebornInfo.NeedZhuanSheng[1]))
			{
				ret = -3;
				result = ret;
			}
			else if (client.ClientData.RebornLevel < rebornInfo.NeedRebornLevel)
			{
				ret = -12;
				result = ret;
			}
			else if (client.ClientData.CombatForce < rebornInfo.NeedZhanLi)
			{
				ret = -12;
				result = ret;
			}
			else
			{
				if (rebornInfo.NeedMaxWing.Length == 5)
				{
					if ((double)client.ClientData.MyWingData.WingID < rebornInfo.NeedMaxWing[0] || ((double)client.ClientData.MyWingData.WingID == rebornInfo.NeedMaxWing[0] && (double)client.ClientData.MyWingData.ForgeLevel < rebornInfo.NeedMaxWing[1]))
					{
						return -12;
					}
					if ((double)LingYuManager.GetTotalLevel(client) < rebornInfo.NeedMaxWing[2])
					{
						return -12;
					}
					if (ZhuLingZhuHunManager.GetZhuLingPct(client) < rebornInfo.NeedMaxWing[3] || ZhuLingZhuHunManager.GetZhuHunPct(client) < rebornInfo.NeedMaxWing[4])
					{
						return -12;
					}
				}
				if (rebornInfo.NeedChengJie > 0 && ChengJiuManager.GetChengJiuLevel(client) < rebornInfo.NeedChengJie)
				{
					ret = -12;
					result = ret;
				}
				else if (rebornInfo.NeedShengWang > 0 && GameManager.ClientMgr.GetShengWangLevelValue(client) < rebornInfo.NeedShengWang)
				{
					ret = -12;
					result = ret;
				}
				else
				{
					if (rebornInfo.NeedMagicBook.Length == 2)
					{
						if (client.ClientData.MerlinData._Level < rebornInfo.NeedMagicBook[0] || (client.ClientData.MerlinData._Level == rebornInfo.NeedMagicBook[0] && client.ClientData.MerlinData._StarNum < rebornInfo.NeedMagicBook[1]))
						{
							return -12;
						}
					}
					result = ret;
				}
			}
			return result;
		}

		
		public int GetRebornAdmireCount(GameClient client, int rankType)
		{
			List<int> countList = Global.GetRoleParamsIntListFromDB(client, "151");
			if (countList.Count != 5)
			{
				for (int i = countList.Count; i < 5; i++)
				{
					countList.Add(0);
				}
			}
			int nToday = TimeUtil.NowDateTime().DayOfYear;
			int result;
			if (countList[0] != nToday)
			{
				result = 0;
			}
			else
			{
				result = countList[rankType + 1];
			}
			return result;
		}

		
		public void ProcessIncreaseRebornAdmireCount(GameClient client, int rankType)
		{
			int nToday = TimeUtil.NowDateTime().DayOfYear;
			List<int> countList = Global.GetRoleParamsIntListFromDB(client, "151");
			if (countList.Count != 5)
			{
				for (int i = countList.Count; i < 5; i++)
				{
					countList.Add(0);
				}
			}
			if (countList[0] == nToday)
			{
				List<int> list;
				int index;
				(list = countList)[index = rankType + 1] = list[index] + 1;
			}
			else
			{
				countList[0] = nToday;
				for (int i = 1; i < countList.Count; i++)
				{
					countList[i] = 0;
				}
				countList[rankType + 1] = 1;
			}
			Global.SaveRoleParamsIntListToDB(client, countList, "151", true);
		}

		
		public bool CheckRebornCountLevelValid(GameClient client, int count, int level)
		{
			Dictionary<int, RebornStageInfo> tempRebornStageInfoDict = this.RebornStageInfoDict;
			Dictionary<int, RebornLevelInfo> tempRebornLevelInfoDict = this.RebornLevelInfoDict;
			RebornStageInfo rebornInfo;
			return tempRebornStageInfoDict.TryGetValue(count, out rebornInfo) && (rebornInfo.MaxRebornLevel <= 0 || level <= rebornInfo.MaxRebornLevel);
		}

		
		public void OnLogin(GameClient client, bool login = false)
		{
			if (login)
			{
				this.InitPlayerRebornPorperty(client);
			}
			else
			{
				GameManager.ClientMgr.ModifyRebornExpMaxAddValue(client, 0L, "", MoneyTypes.RebornExpMonster, false, true, false);
				GameManager.ClientMgr.ModifyRebornExpMaxAddValue(client, 0L, "", MoneyTypes.RebornExpSale, false, true, false);
			}
		}

		
		public void InitPlayerRebornPorperty(GameClient client)
		{
			if (client.ClientData.RebornCount > 0)
			{
				Dictionary<int, RebornStageInfo> tempRebornStageInfoDict = this.RebornStageInfoDict;
				for (int rebornLoop = 1; rebornLoop <= tempRebornStageInfoDict.Count; rebornLoop++)
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.Reborn,
						rebornLoop,
						PropsCacheManager.ConstExtProps
					});
				}
				for (int rebornLoop = 1; rebornLoop <= client.ClientData.RebornCount; rebornLoop++)
				{
					RebornStageInfo rebornInfo;
					if (tempRebornStageInfoDict.TryGetValue(rebornLoop, out rebornInfo))
					{
						if (0 <= client.ClientData.Occupation && client.ClientData.Occupation < rebornInfo.extProps.Length)
						{
							if (null != rebornInfo.extProps[client.ClientData.Occupation])
							{
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.Reborn,
									rebornLoop,
									rebornInfo.extProps[client.ClientData.Occupation]
								});
							}
						}
					}
				}
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					DelayExecProcIds.RecalcProps,
					DelayExecProcIds.NotifyRefreshProps
				});
			}
		}

		
		public int CalcRebornInjure(IObject attacker, IObject defender, double injurePercnet, double baseRate, ref int burst)
		{
			int injure = 0;
			for (int idx = 122; idx <= 150; idx += 7)
			{
				injure += (int)RoleAlgorithm.CalRebornAttackInjureValue(attacker, defender, (ExtPropIndexes)idx, ref burst);
			}
			if (attacker is GameClient && (defender is GameClient || defender is Robot))
			{
				injure /= 2;
			}
			injure = (int)((double)injure * injurePercnet * baseRate);
			return Math.Max(0, injure);
		}

		
		public int CalculateCombatForce(GameClient client)
		{
			CombatForceInfo CombatForce = this.RebornCombatForceData;
			double nMinAttack = RoleAlgorithm.GetMinAttackV(client);
			double nMaxAttack = RoleAlgorithm.GetMaxAttackV(client);
			double nMinDefense = RoleAlgorithm.GetMinADefenseV(client);
			double nMaxDefense = RoleAlgorithm.GetMaxADefenseV(client);
			double nMinMAttack = RoleAlgorithm.GetMinMagicAttackV(client);
			double nMaxMAttack = RoleAlgorithm.GetMaxMagicAttackV(client);
			double nMinMDefense = RoleAlgorithm.GetMinMDefenseV(client);
			double nMaxMDefense = RoleAlgorithm.GetMaxMDefenseV(client);
			double nHit = RoleAlgorithm.GetHitV(client);
			double nDodge = RoleAlgorithm.GetDodgeV(client);
			double addAttackInjure = RoleAlgorithm.GetAddAttackInjureValue(client);
			double decreaseInjure = RoleAlgorithm.GetDecreaseInjureValue(client);
			double nMaxHP = RoleAlgorithm.GetMaxLifeV(client);
			double nMaxMP = RoleAlgorithm.GetMaxMagicV(client);
			double nLifeSteal = RoleAlgorithm.GetLifeStealV(client);
			double dFireAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Fire);
			double dWaterAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Water);
			double dLightningAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Lightning);
			double dSoilAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Soil);
			double dIceAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Ice);
			double dWindAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Wind);
			double HolyAttack = RoleAlgorithm.GetExtProp(client, 122);
			double HolyDefense = RoleAlgorithm.GetExtProp(client, 123);
			double ShadowAttack = RoleAlgorithm.GetExtProp(client, 129);
			double ShadowDefense = RoleAlgorithm.GetExtProp(client, 130);
			double NatureAttack = RoleAlgorithm.GetExtProp(client, 136);
			double NatureDefense = RoleAlgorithm.GetExtProp(client, 137);
			double ChaosAttack = RoleAlgorithm.GetExtProp(client, 143);
			double ChaosDefense = RoleAlgorithm.GetExtProp(client, 144);
			double IncubusAttack = RoleAlgorithm.GetExtProp(client, 150);
			double IncubusDefense = RoleAlgorithm.GetExtProp(client, 151);
			if (null != CombatForce)
			{
				double nValue = (nMinAttack / CombatForce.MinPhysicsAttackModulus + nMaxAttack / CombatForce.MaxPhysicsAttackModulus) / 2.0 + (nMinDefense / CombatForce.MinPhysicsDefenseModulus + nMaxDefense / CombatForce.MaxPhysicsDefenseModulus) / 2.0 + (nMinMAttack / CombatForce.MinMagicAttackModulus + nMaxMAttack / CombatForce.MaxMagicAttackModulus) / 2.0 + (nMinMDefense / CombatForce.MinMagicDefenseModulus + nMaxMDefense / CombatForce.MaxMagicDefenseModulus) / 2.0 + addAttackInjure / CombatForce.AddAttackInjureModulus + decreaseInjure / CombatForce.DecreaseInjureModulus + nHit / CombatForce.HitValueModulus + nDodge / CombatForce.DodgeModulus + nMaxHP / CombatForce.MaxHPModulus + nMaxMP / CombatForce.MaxMPModulus + nLifeSteal / CombatForce.LifeStealModulus;
				nValue += dFireAttack / CombatForce.FireAttack + dWaterAttack / CombatForce.WaterAttack + dLightningAttack / CombatForce.LightningAttack + dSoilAttack / CombatForce.SoilAttack + dIceAttack / CombatForce.IceAttack + dWindAttack / CombatForce.WindAttack;
				nValue += HolyAttack / CombatForce.HolyAttack + HolyDefense / CombatForce.HolyDefense + ShadowAttack / CombatForce.ShadowAttack + ShadowDefense / CombatForce.ShadowDefense + NatureAttack / CombatForce.NatureAttack + NatureDefense / CombatForce.NatureDefense + ChaosAttack / CombatForce.ChaosAttack + ChaosDefense / CombatForce.ChaosDefense + IncubusAttack / CombatForce.IncubusAttack + IncubusDefense / CombatForce.IncubusDefense;
				client.ClientData.RebornCombatForce = (int)nValue;
			}
			return client.ClientData.RebornCombatForce;
		}

		
		public void EarnExperience(GameClient sprite, long experience)
		{
			Dictionary<int, RebornStageInfo> tempRebornStageInfoDict = this.RebornStageInfoDict;
			Dictionary<int, RebornLevelInfo> tempRebornLevelInfoDict = this.RebornLevelInfoDict;
			if (sprite.ClientData.RebornCount > 0 && sprite.ClientData.RebornCount <= tempRebornStageInfoDict.Count)
			{
				RebornStageInfo rebornInfo;
				if (tempRebornStageInfoDict.TryGetValue(sprite.ClientData.RebornCount, out rebornInfo))
				{
					RebornLevelInfo levelInfo = null;
					if (tempRebornLevelInfoDict.TryGetValue(sprite.ClientData.RebornLevel, out levelInfo))
					{
						long nNeedExp = (long)levelInfo.NeedRebornExp;
						bool reachTopLevel = false;
						if (rebornInfo.MaxRebornLevel > 0 && sprite.ClientData.RebornLevel >= rebornInfo.MaxRebornLevel)
						{
							reachTopLevel = true;
						}
						if (!reachTopLevel && sprite.ClientData.RebornLevel <= tempRebornLevelInfoDict.Count - 1 && sprite.ClientData.RebornExperience + experience >= nNeedExp)
						{
							int oldLevel = sprite.ClientData.RebornLevel;
							sprite.ClientData.RebornLevel++;
							experience = sprite.ClientData.RebornExperience + experience - nNeedExp;
							sprite.ClientData.RebornExperience = 0L;
							this.EarnExperience(sprite, experience);
						}
						else
						{
							sprite.ClientData.RebornExperience += experience;
							sprite.ClientData.RebornExperience = Global.GMax(0L, sprite.ClientData.RebornExperience);
						}
					}
				}
			}
		}

		
		public void ProcessRoleExperience(GameClient client, long experience, MoneyTypes types, bool enableFilter = true, bool writeToDB = true, bool checkDead = false, string strFrom = "none")
		{
			if (types == MoneyTypes.RebornExpMonster || types == MoneyTypes.RebornExpSale || types == MoneyTypes.RebornExp)
			{
				if (client.ClientData.HideGM <= 0)
				{
					if (!checkDead || client.ClientData.CurrentLifeV > 0)
					{
						if (experience > 0L)
						{
							if (types != MoneyTypes.RebornExp)
							{
								experience = Math.Min(experience, this.GetRebornExpMaxValueLeft(client, types));
							}
							if (experience > 0L)
							{
								long oldExp = client.ClientData.RebornExperience;
								int oldUnionLevel = Global.GetUnionLevel2(client.ClientData.RebornCount, client.ClientData.RebornLevel);
								EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.Awards, types, experience, -1L, strFrom);
								int oldLevel = client.ClientData.RebornLevel;
								this.EarnExperience(client, experience);
								long nowTicks = TimeUtil.NOW();
								if (writeToDB || oldLevel != client.ClientData.RebornLevel)
								{
									Dictionary<int, RebornLevelInfo> tempRebornLevelInfoDict = this.RebornLevelInfoDict;
									lock (client.ClientData.PropPointMutex)
									{
										for (int loop = oldLevel + 1; loop <= client.ClientData.RebornLevel; loop++)
										{
											RebornLevelInfo levelInfo;
											if (tempRebornLevelInfoDict.TryGetValue(loop, out levelInfo) && levelInfo.RebornPoint > 0)
											{
												GameManager.ClientMgr.ModifyRebornYinJiPointValue(client, levelInfo.RebornPoint, "升级", true, true, false);
											}
										}
									}
									Global.SaveRoleParamsInt32ValueToDB(client, "10241", client.ClientData.RebornLevel, true);
									Global.SaveRoleParamsInt64ValueToDB(client, "10242", client.ClientData.RebornExperience, true);
									Global.SetLastDBRoleParamCmdTicks(client, "10241", nowTicks);
									Global.SetLastDBRoleParamCmdTicks(client, "10242", nowTicks);
								}
								else
								{
									Global.SaveRoleParamsInt64ValueToDB(client, "10242", client.ClientData.RebornExperience, false);
									Global.SetLastDBRoleParamCmdTicks(client, "10242", nowTicks);
								}
								if (oldLevel != client.ClientData.RebornLevel)
								{
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
									EventLogManager.AddRoleRebornUpgradeEvent(client, experience, oldExp, oldUnionLevel, strFrom);
									KuaFuWorldClient.getInstance().Reborn_RebornOpt(client.ClientData.ServerPTID, client.ClientData.LocalRoleID, 0, client.ClientData.RebornLevel, 0, "");
									if (client._IconStateMgr.CheckReborn(client))
									{
										client._IconStateMgr.SendIconStateToClient(client);
									}
								}
								GameManager.ClientMgr.UpdateRoleDailyData_RebornExp(client, types, experience);
								this.NotifySelfExperience(client, experience);
								GameManager.ClientMgr.ModifyRebornExpMaxAddValue(client, 0L, strFrom, MoneyTypes.RebornExpMonster, false, true, false);
								GameManager.ClientMgr.ModifyRebornExpMaxAddValue(client, 0L, strFrom, MoneyTypes.RebornExpSale, false, true, false);
							}
						}
					}
				}
			}
		}

		
		public void NotifySelfExperience(GameClient client, long newExperience)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				client.ClientData.RebornExperience,
				client.ClientData.RebornLevel,
				newExperience,
				client.ClientData.RebornCount
			});
			client.sendCmd(1711, strcmd, false);
		}

		
		public long GetRebornExpMaxValue(GameClient client, MoneyTypes types)
		{
			long result;
			if (types != MoneyTypes.RebornExpMonster && types != MoneyTypes.RebornExpSale)
			{
				result = 0L;
			}
			else
			{
				long maxfixvalue = this.GetRebornExpMaxValueFix(client, types);
				long maxaddvalue = GameManager.ClientMgr.GetRebornExpMaxAddValue(client, types);
				long addvalue = GameManager.ClientMgr.GetRoleDailyData_RebornExp(client, types);
				result = Math.Max(0L, maxfixvalue + maxaddvalue);
			}
			return result;
		}

		
		public long GetRebornExpMaxValueLeft(GameClient client, MoneyTypes types)
		{
			long result;
			if (types != MoneyTypes.RebornExpMonster && types != MoneyTypes.RebornExpSale)
			{
				result = 0L;
			}
			else
			{
				long maxfixvalue = this.GetRebornExpMaxValueFix(client, types);
				long maxaddvalue = GameManager.ClientMgr.GetRebornExpMaxAddValue(client, types);
				long addvalue = GameManager.ClientMgr.GetRoleDailyData_RebornExp(client, types);
				result = Math.Max(0L, maxfixvalue + maxaddvalue - addvalue);
			}
			return result;
		}

		
		private long GetRebornExpMaxValueFix(GameClient client, MoneyTypes types)
		{
			Dictionary<int, RebornLevelInfo> tempRebornLevelInfoDict = this.RebornLevelInfoDict;
			RebornLevelInfo levInfo = null;
			long result;
			if (!tempRebornLevelInfoDict.TryGetValue(client.ClientData.RebornLevel, out levInfo))
			{
				result = 0L;
			}
			else
			{
				long maxValue = 0L;
				if (types == MoneyTypes.RebornExpMonster)
				{
					maxValue = (long)levInfo.MaxOfMonsters;
				}
				else if (types == MoneyTypes.RebornExpSale)
				{
					maxValue = (long)levInfo.MaxOfGoods;
				}
				result = maxValue;
			}
			return result;
		}

		
		public int GetTodayLianZhanMax(GameClient client)
		{
			List<int> countList = Global.GetRoleParamsIntListFromDB(client, "152");
			if (countList.Count != 2)
			{
				for (int i = countList.Count; i < 2; i++)
				{
					countList.Add(0);
				}
			}
			int nToday = TimeUtil.NowDateTime().DayOfYear;
			int result;
			if (countList[0] != nToday)
			{
				result = 0;
			}
			else
			{
				result = countList[1];
			}
			return result;
		}

		
		public void SetTodayLianZhanMax(GameClient client, int max)
		{
			int nToday = TimeUtil.NowDateTime().DayOfYear;
			List<int> countList = Global.GetRoleParamsIntListFromDB(client, "152");
			if (countList.Count != 2)
			{
				for (int i = countList.Count; i < 2; i++)
				{
					countList.Add(0);
				}
			}
			if (countList[0] == nToday)
			{
				countList[1] = max;
			}
			else
			{
				countList[0] = nToday;
				countList[1] = max;
			}
			Global.SaveRoleParamsIntListToDB(client, countList, "152", true);
		}

		
		public void ProcessLianZhan(GameClient client)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			if (SceneUIClasses.ChongShengMap == sceneType)
			{
				if (client.ClientData.TempLianZhan > this.GetTodayLianZhanMax(client))
				{
					this.SetTodayLianZhanMax(client, client.ClientData.TempLianZhan);
					KuaFuWorldClient.getInstance().Reborn_RebornOpt(client.ClientData.ServerPTID, client.ClientData.LocalRoleID, 3, client.ClientData.TempLianZhan, 0, "");
				}
			}
		}

		
		public void ProcessRebornMonsterFallGoods(GameClient client, Monster monster)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType(monster.CurrentMapCode);
			if (SceneUIClasses.ChongShengMap == sceneType)
			{
				if (monster.MonsterType == 301)
				{
					KuaFuWorldClient.getInstance().Reborn_RebornOpt(client.ClientData.ServerPTID, client.ClientData.LocalRoleID, 1, 1, 0, "");
				}
				else if (monster.MonsterType == 401)
				{
					KuaFuWorldClient.getInstance().Reborn_RebornOpt(client.ClientData.ServerPTID, client.ClientData.LocalRoleID, 2, 1, 0, "");
				}
			}
		}

		
		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				string valueString = Global.GetRoleParamsFromDBByRoleID(roleId, "10240", 0);
				int RebornCount = Global.SafeConvertToInt32(valueString);
				if (RebornCount > 0)
				{
					KuaFuWorldClient.getInstance().Reborn_ChangeName(GameManager.PTID, roleId, newName);
				}
			}
		}

		
		public void PlatFormChat(GameClient client, string text)
		{
			KFPlatFormChat chat = new KFPlatFormChat(client.ClientData.ZoneID, client.ClientData.RoleName, text, client.ClientData.UserPTID);
			lock (this.Mutex)
			{
				this.PlatFormChatList.Add(chat);
			}
			this.BroadcastPlatFormChatMsg(chat);
		}

		
		public void BroadcastPlatFormChatMsg(KFPlatFormChat kfChat)
		{
			foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
			{
				if (client != null && client.ClientData.UserPTID == kfChat.PtID)
				{
					client.sendCmd(157, kfChat.Text, false);
				}
			}
		}

		
		public void OnChatListData(byte[] data)
		{
			if (null != data)
			{
				List<KFPlatFormChat> chatList = DataHelper.BytesToObject<List<KFPlatFormChat>>(data, 0, data.Length);
				if (null != chatList)
				{
					foreach (KFPlatFormChat kfChat in chatList)
					{
						this.BroadcastPlatFormChatMsg(kfChat);
					}
				}
			}
		}

		
		public bool InitConfig(bool reload = false)
		{
			bool result;
			if (!RebornStamp.ParseYinJiConfig())
			{
				result = false;
			}
			else if (!RebornEquip.ParseRebornEquipConfig())
			{
				result = false;
			}
			else if (!RebornStone.ParseRebornStoneConfig())
			{
				result = false;
			}
			else if (!this.LoadRebornStageConfigFile())
			{
				result = false;
			}
			else if (!this.LoadRebornCombatForceConfigFile())
			{
				result = false;
			}
			else
			{
				if (!reload)
				{
					if (!this.LoadRebornLevelConfigFile())
					{
						return false;
					}
				}
				this.EveryDayMaxRebornExp = GameManager.systemParamsList.GetParamValueIntArrayByName("EveryDayMaxRebornExp", ',');
				this.RebornMapPKMode = GameManager.systemParamsList.GetParamValueIntArrayByName("RebornMapPK", ',');
				result = true;
			}
			return result;
		}

		
		public bool LoadRebornStageConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(RebornDataConst.RebornStage));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(RebornDataConst.RebornStage));
				if (null == xml)
				{
					return false;
				}
				Dictionary<int, RebornStageInfo> tempRebornStageInfoDict = new Dictionary<int, RebornStageInfo>();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					RebornStageInfo myData = new RebornStageInfo();
					myData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					myData.NeedZhuanSheng = Global.GetSafeAttributeIntArray(xmlItem, "NeedZhuanSheng", -1, '|');
					myData.NeedRebornLevel = (int)Global.GetSafeAttributeLong(xmlItem, "NeedRebornLevel");
					myData.NeedZhanLi = (int)Global.GetSafeAttributeLong(xmlItem, "NeedZhanLi");
					myData.NeedMaxWing = Global.GetSafeAttributeDoubleArray(xmlItem, "NeedMaxWing", -1, '|');
					myData.NeedChengJie = (int)Global.GetSafeAttributeLong(xmlItem, "NeedChengJie");
					myData.NeedShengWang = (int)Global.GetSafeAttributeLong(xmlItem, "NeedShengWang");
					myData.NeedMagicBook = Global.GetSafeAttributeIntArray(xmlItem, "NeedMagicBook", -1, '|');
					myData.MaxRebornLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MaxRebornLevel");
					myData.RebornPoint = (int)Global.GetSafeAttributeLong(xmlItem, "RebornDian");
					string strextprops = Global.GetSafeAttributeStr(xmlItem, "ExtProp");
					if (!string.IsNullOrEmpty(strextprops))
					{
						string[] extpropsocc = strextprops.Split(new char[]
						{
							'|'
						});
						for (int occidx = 0; occidx < 6; occidx++)
						{
							string[] KvpFileds = extpropsocc[occidx].Split(new char[]
							{
								','
							});
							if (KvpFileds.Length == 2)
							{
								ExtPropIndexes index = ConfigParser.GetPropIndexByPropName(KvpFileds[0]);
								myData.extProps[occidx] = new double[177];
								myData.extProps[occidx][(int)index] = Global.SafeConvertToDouble(KvpFileds[1]);
							}
						}
					}
					ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xmlItem, "AwardGoods"), ref myData.AwardGoods, '|', ',');
					tempRebornStageInfoDict[myData.ID] = myData;
				}
				this.RebornStageInfoDict = tempRebornStageInfoDict;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", RebornDataConst.RebornStage, ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public bool LoadRebornLevelConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(RebornDataConst.RebornLevel));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(RebornDataConst.RebornLevel));
				if (null == xml)
				{
					return false;
				}
				Dictionary<int, RebornLevelInfo> tempRebornLevelInfoDict = new Dictionary<int, RebornLevelInfo>();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					RebornLevelInfo myData = new RebornLevelInfo();
					myData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					myData.NeedRebornExp = (int)Global.GetSafeAttributeLong(xmlItem, "NeedRebornExp");
					myData.MaxOfMonsters = (int)Global.GetSafeAttributeLong(xmlItem, "MaxOfMonsters");
					myData.MaxOfGoods = (int)Global.GetSafeAttributeLong(xmlItem, "MaxOfGoods");
					myData.RebornPoint = (int)Global.GetSafeAttributeLong(xmlItem, "RebornDian");
					tempRebornLevelInfoDict[myData.ID] = myData;
				}
				this.RebornLevelInfoDict = tempRebornLevelInfoDict;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", RebornDataConst.RebornLevel, ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public bool LoadRebornCombatForceConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(RebornDataConst.RebornCombatForce));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(RebornDataConst.RebornCombatForce));
				if (null == xml)
				{
					return false;
				}
				CombatForceInfo tempRebornCombatForceData = new CombatForceInfo();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					tempRebornCombatForceData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					tempRebornCombatForceData.MaxHPModulus = Global.GetSafeAttributeDouble(xmlItem, "LifeV");
					tempRebornCombatForceData.MaxMPModulus = Global.GetSafeAttributeDouble(xmlItem, "MagicV");
					tempRebornCombatForceData.MinPhysicsDefenseModulus = Global.GetSafeAttributeDouble(xmlItem, "MinDefenseV");
					tempRebornCombatForceData.MaxPhysicsDefenseModulus = Global.GetSafeAttributeDouble(xmlItem, "MaxDefenseV");
					tempRebornCombatForceData.MinMagicDefenseModulus = Global.GetSafeAttributeDouble(xmlItem, "MinMDefenseV");
					tempRebornCombatForceData.MaxMagicDefenseModulus = Global.GetSafeAttributeDouble(xmlItem, "MaxMDefenseV");
					tempRebornCombatForceData.MinPhysicsAttackModulus = Global.GetSafeAttributeDouble(xmlItem, "MinAttackV");
					tempRebornCombatForceData.MaxPhysicsAttackModulus = Global.GetSafeAttributeDouble(xmlItem, "MaxAttackV");
					tempRebornCombatForceData.MinMagicAttackModulus = Global.GetSafeAttributeDouble(xmlItem, "MinMAttackV");
					tempRebornCombatForceData.MaxMagicAttackModulus = Global.GetSafeAttributeDouble(xmlItem, "MaxMAttackV");
					tempRebornCombatForceData.HitValueModulus = Global.GetSafeAttributeDouble(xmlItem, "HitV");
					tempRebornCombatForceData.DodgeModulus = Global.GetSafeAttributeDouble(xmlItem, "Dodge");
					tempRebornCombatForceData.AddAttackInjureModulus = Global.GetSafeAttributeDouble(xmlItem, "AddAttackInjure");
					tempRebornCombatForceData.DecreaseInjureModulus = Global.GetSafeAttributeDouble(xmlItem, "DecreaseInjureValue");
					tempRebornCombatForceData.LifeStealModulus = Global.GetSafeAttributeDouble(xmlItem, "LifeSteal");
					tempRebornCombatForceData.AddAttackModulus = Global.GetSafeAttributeDouble(xmlItem, "AddAttack");
					tempRebornCombatForceData.AddDefenseModulus = Global.GetSafeAttributeDouble(xmlItem, "AddDefense");
					tempRebornCombatForceData.FireAttack = Global.GetSafeAttributeDouble(xmlItem, "FireAttack");
					tempRebornCombatForceData.WaterAttack = Global.GetSafeAttributeDouble(xmlItem, "WaterAttack");
					tempRebornCombatForceData.LightningAttack = Global.GetSafeAttributeDouble(xmlItem, "LightningAttack");
					tempRebornCombatForceData.SoilAttack = Global.GetSafeAttributeDouble(xmlItem, "SoilAttack");
					tempRebornCombatForceData.IceAttack = Global.GetSafeAttributeDouble(xmlItem, "IceAttack");
					tempRebornCombatForceData.WindAttack = Global.GetSafeAttributeDouble(xmlItem, "WindAttack");
					tempRebornCombatForceData.ArmorMax = ConfigHelper.GetElementAttributeValueDouble(xmlItem, "ArmorMax", 1.0);
					tempRebornCombatForceData.HolyAttack = Global.GetSafeAttributeDouble(xmlItem, "HolyAttack");
					tempRebornCombatForceData.HolyDefense = Global.GetSafeAttributeDouble(xmlItem, "HolyDefense");
					tempRebornCombatForceData.ShadowAttack = Global.GetSafeAttributeDouble(xmlItem, "ShadowAttack");
					tempRebornCombatForceData.ShadowDefense = Global.GetSafeAttributeDouble(xmlItem, "ShadowDefense");
					tempRebornCombatForceData.NatureAttack = Global.GetSafeAttributeDouble(xmlItem, "NatureAttack");
					tempRebornCombatForceData.NatureDefense = Global.GetSafeAttributeDouble(xmlItem, "NatureDefense");
					tempRebornCombatForceData.ChaosAttack = Global.GetSafeAttributeDouble(xmlItem, "ChaosAttack");
					tempRebornCombatForceData.ChaosDefense = Global.GetSafeAttributeDouble(xmlItem, "ChaosDefense");
					tempRebornCombatForceData.IncubusAttack = Global.GetSafeAttributeDouble(xmlItem, "IncubusAttack");
					tempRebornCombatForceData.IncubusDefense = Global.GetSafeAttributeDouble(xmlItem, "IncubusDefense");
				}
				this.RebornCombatForceData = tempRebornCombatForceData;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", RebornDataConst.RebornCombatForce, ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public bool IfSupportPKModeNotNormal(int mapCode)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType(mapCode);
			bool result;
			if (SceneUIClasses.ChongShengMap != sceneType)
			{
				result = true;
			}
			else
			{
				List<KuaFuLineData> list = KuaFuWorldClient.getInstance().GetKuaFuLineDataList(mapCode) as List<KuaFuLineData>;
				if (null == list)
				{
					result = false;
				}
				else
				{
					KuaFuLineData currentLineData = list.Find((KuaFuLineData x) => x.ServerId == GameManager.KuaFuServerId);
					result = (null != currentLineData && this.RebornMapPKMode[currentLineData.Line - 1] != 0);
				}
			}
			return result;
		}

		
		public void OnInitGame(GameClient client)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			if (SceneUIClasses.ChongShengMap == sceneType)
			{
				if (0 != client.ClientData.PKMode)
				{
					if (this.IfSupportPKModeNotNormal(client.ClientData.MapCode))
					{
						client.ClientData.PKMode = 2;
					}
					else
					{
						client.ClientData.PKMode = 0;
					}
				}
			}
		}

		
		private void TimerProc(object sender, EventArgs e)
		{
			try
			{
				RebornSyncData SyncData = KuaFuWorldClient.getInstance().Reborn_SyncData(this.RebornSyncDataCache.RebornRankDict.Age, this.RebornSyncDataCache.BossRefreshDict.Age);
				if (null != SyncData)
				{
					lock (this.RebornSyncDataCache)
					{
						if (this.RebornSyncDataCache.RebornRankDict.Age != SyncData.RebornRankDict.Age && null != SyncData.BytesRebornRankDict)
						{
							this.RebornSyncDataCache.RebornRankDict = DataHelper2.BytesToObject<KuaFuData<Dictionary<int, List<KFRebornRankInfo>>>>(SyncData.BytesRebornRankDict, 0, SyncData.BytesRebornRankDict.Length);
							if (!GameManager.IsKuaFuServer)
							{
								int loop = 0;
								while (loop <= 3)
								{
									List<KFRebornRankInfo> rankInfoList = null;
									if (this.RebornSyncDataCache.RebornRankDict.V.TryGetValue(loop, out rankInfoList) && rankInfoList != null && rankInfoList.Count > 0)
									{
										KFRebornRankInfo Top = rankInfoList[0];
										if (Top.PtID == GameManager.PTID)
										{
											RoleDataEx dbRd = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, Top.Key), 0);
											if (dbRd != null && dbRd.RoleID > 0)
											{
												RoleData4Selector RoleData4Selector = Global.RoleDataEx2RoleData4Selector(dbRd);
												KuaFuWorldClient.getInstance().Reborn_SetRoleData4Selector(Top.PtID, Top.Key, DataHelper.ObjectToBytes<RoleData4Selector>(RoleData4Selector));
											}
										}
									}
									IL_17F:
									loop++;
									continue;
									goto IL_17F;
								}
							}
						}
						if (this.RebornSyncDataCache.BossRefreshDict.Age != SyncData.BossRefreshDict.Age && null != SyncData.BytesRebornBossRefreshDict)
						{
							this.RebornSyncDataCache.BossRefreshDict = DataHelper2.BytesToObject<KuaFuData<Dictionary<KeyValuePair<int, int>, KFRebornBossRefreshData>>>(SyncData.BytesRebornBossRefreshDict, 0, SyncData.BytesRebornBossRefreshDict.Length);
							if (null == this.RebornSyncDataCache.BossRefreshDict)
							{
								this.RebornSyncDataCache.BossRefreshDict = new KuaFuData<Dictionary<KeyValuePair<int, int>, KFRebornBossRefreshData>>();
							}
						}
					}
					List<KFPlatFormChat> chatList = null;
					lock (this.Mutex)
					{
						if (this.PlatFormChatList.Count > 0)
						{
							chatList = new List<KFPlatFormChat>(this.PlatFormChatList);
							this.PlatFormChatList.Clear();
						}
					}
					if (null != chatList)
					{
						KuaFuWorldClient.getInstance().Reborn_PlatFormChat(chatList);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
		}

		
		public object Mutex = new object();

		
		private Dictionary<int, RebornStageInfo> RebornStageInfoDict = new Dictionary<int, RebornStageInfo>();

		
		private Dictionary<int, RebornLevelInfo> RebornLevelInfoDict = new Dictionary<int, RebornLevelInfo>();

		
		private CombatForceInfo RebornCombatForceData = new CombatForceInfo();

		
		private int[] EveryDayMaxRebornExp;

		
		private int[] RebornMapPKMode;

		
		public RebornSyncData RebornSyncDataCache = new RebornSyncData();

		
		public List<KFPlatFormChat> PlatFormChatList = new List<KFPlatFormChat>();

		
		private static RebornManager instance = new RebornManager();
	}
}
