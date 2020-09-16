using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	public class ZuoQiManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		
		public static ZuoQiManager getInstance()
		{
			return ZuoQiManager.instance;
		}

		
		public bool initialize()
		{
			this.ReLoadConfig(true);
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1896, 1, 1, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1897, 3, 3, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1899, 1, 1, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1900, 1, 1, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1901, 2, 2, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1898, 2, 2, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1902, 1, 1, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1903, 1, 1, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2094, 2, 2, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		
		public bool showdown()
		{
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
			if (!this.IsGongNengOpen(client, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1896:
					result = this.ProcessZuoQiMainInfoCmd(client, nID, bytes, cmdParams);
					break;
				case 1897:
					result = this.ProcessZuoQiLieQuCmd(client, nID, bytes, cmdParams);
					break;
				case 1898:
					result = this.ProcessZuoQiUpGradeCmd(client, nID, bytes, cmdParams);
					break;
				case 1899:
					result = this.ProcessZuoQiRideCmd(client, nID, bytes, cmdParams);
					break;
				case 1900:
					result = this.ProcessZuoQiCheckCmd(client, nID, bytes, cmdParams);
					break;
				case 1901:
					result = this.ProcessZuoQiSkillModCmd(client, nID, bytes, cmdParams);
					break;
				case 1902:
					result = this.ProcessZuoQiUpLevelCmd(client, nID, bytes, cmdParams);
					break;
				case 1903:
					result = this.ProcessResetMountBagCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = (nID != 2094 || this.ProcessResetMountGradeCmd(client, nID, bytes, cmdParams));
					break;
				}
			}
			return result;
		}

		
		public bool ProcessZuoQiMainInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				ZuoQiMainData data = new ZuoQiMainData
				{
					MountList = client.ClientData.MountList,
					NextFreeTime = Global.GetRoleParamsDateTimeFromDB(client, "10205"),
					MountLevel = Global.GetRoleParamsInt32FromDB(client, "10207")
				};
				client.sendCmd<ZuoQiMainData>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZuoQi :: 获取坐骑主页面信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessZuoQiLieQuCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int type = Convert.ToInt32(cmdParams[1]);
				int cost = Convert.ToInt32(cmdParams[2]);
				int result = 0;
				DateTime nextFreeTime = Global.GetRoleParamsDateTimeFromDB(client, "10205");
				List<ZuoQiMini> chouQuList = new List<ZuoQiMini>();
				List<int> dbIDList = new List<int>();
				SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
				List<MountRandomItem> MountFreeRandomList;
				List<MountRandomItem> MountRandomList;
				List<MountRandomItem> MountPayRandomList;
				if (spAct != null && spAct.IsChouJiangOpen(SpecPActivityChouJiangType.TeQuanShouLie))
				{
					MountFreeRandomList = this.ZuoQiRunTimeData.MountFreeRandomListTeQuan;
					MountRandomList = this.ZuoQiRunTimeData.MountRandomListTeQuan;
					MountPayRandomList = this.ZuoQiRunTimeData.MountPayRandomListTeQuan;
				}
				else
				{
					MountFreeRandomList = this.ZuoQiRunTimeData.MountFreeRandomList;
					MountRandomList = this.ZuoQiRunTimeData.MountRandomList;
					MountPayRandomList = this.ZuoQiRunTimeData.MountPayRandomList;
				}
				if (type < 0 || type > 1)
				{
					result = 1;
				}
				else
				{
					int nTime = (type == 0) ? 1 : 10;
					if (!ZuoQiManager.CanAddGoodsNum(client, nTime))
					{
						result = 2;
					}
					else
					{
						DateTime now = TimeUtil.NowDateTime();
						if (nTime == 1)
						{
							if (nextFreeTime < now)
							{
								ZuoQiMini goodsTmp = this.GetRandomMount(MountFreeRandomList);
								if (null == goodsTmp)
								{
									result = 3;
									goto IL_4CD;
								}
								goodsTmp.Binding = 1;
								chouQuList.Add(goodsTmp);
								nextFreeTime = now.AddSeconds((double)this.ZuoQiFreeTime);
								Global.SaveRoleParamsDateTimeToDB(client, "10205", nextFreeTime, true);
							}
							else
							{
								ZuoQiMini goodsTmp = this.GetRandomMount(MountRandomList);
								if (null == goodsTmp)
								{
									result = 3;
									goto IL_4CD;
								}
								if (cost != this.ZuoQiChouQuCost)
								{
									result = 14;
									goto IL_4CD;
								}
								if (!GameManager.ClientMgr.ModifyLuckStarValue(client, -this.ZuoQiChouQuCost, "坐骑猎取_钻石(改幸运之星)", false, DaiBiSySType.ZuoQiBuHuo))
								{
									result = 4;
									goto IL_4CD;
								}
								GameManager.ClientMgr.ModifyMountPointValue(client, (int)(this.ConsumeHuntHorseJiFen * (double)this.ZuoQiChouQuCost), "坐骑猎取获得积分", true, true, false);
								chouQuList.Add(goodsTmp);
							}
						}
						else
						{
							if (cost != this.ZuoQiChouQuCost_10)
							{
								result = 14;
								goto IL_4CD;
							}
							ZuoQiMini goodsTmp;
							for (int i = 0; i < nTime - 1; i++)
							{
								goodsTmp = this.GetRandomMount(MountRandomList);
								if (null == goodsTmp)
								{
									break;
								}
								chouQuList.Add(goodsTmp);
							}
							goodsTmp = this.GetRandomMount(MountPayRandomList);
							if (null != goodsTmp)
							{
								chouQuList.Add(goodsTmp);
							}
							if (chouQuList.Count < nTime)
							{
								result = 3;
								goto IL_4CD;
							}
							ZuoQiMini lastVal = chouQuList[nTime - 1];
							int random = Global.GetRandomNumber(0, nTime);
							chouQuList[nTime - 1] = chouQuList[random];
							chouQuList[random] = lastVal;
							if (!GameManager.ClientMgr.ModifyLuckStarValue(client, -this.ZuoQiChouQuCost_10, "坐骑猎取10_钻石(改幸运之星)", false, DaiBiSySType.ZuoQiBuHuo))
							{
								result = -17;
								goto IL_4CD;
							}
							GameManager.ClientMgr.ModifyMountPointValue(client, (int)(this.ConsumeHuntHorseJiFen * (double)this.ZuoQiChouQuCost_10), "坐骑猎取获得积分", true, true, false);
						}
						foreach (ZuoQiMini goods in chouQuList)
						{
							int dbid = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goods.GoodsID, 1, 0, "", 0, goods.Binding, 12000, "", true, 1, "坐骑猎取抽取", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, goods.WashProps, null, 0, true);
							if (this.ZuoQiRunTimeData.HorseNotice.Contains(goods.GoodsID))
							{
								int num = 0;
								foreach (GoodsData item in client.ClientData.MountStoreList)
								{
									if (item.Id == dbid && null != item.WashProps)
									{
										num = item.WashProps.Count / 2;
										break;
									}
								}
								string horseNoticeMsg = string.Format(GLang.GetLang(5004, new object[0]), Global.FormatRoleName4(client), num, Global.GetGoodsNameByID(goods.GoodsID));
								Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, horseNoticeMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0, 0, 100, 100);
							}
							dbIDList.Add(dbid);
						}
						client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
						{
							DelayExecProcIds.RecalcProps,
							DelayExecProcIds.NotifyRefreshProps
						});
					}
				}
				IL_4CD:
				ZuoQiChouQuResult data = new ZuoQiChouQuResult
				{
					Result = result,
					GoodsList = string.Join<int>(",", dbIDList),
					FreeTime = nextFreeTime
				};
				client.sendCmd<ZuoQiChouQuResult>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZuoQi :: 坐骑猎取信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessZuoQiRideCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				if (ZuoQiManager.CanRide(client) != 1)
				{
					return true;
				}
				if (client.ClientData.DisMountTick + 2000U > TimeUtil.timeGetTime())
				{
					return true;
				}
				client.ClientData.IsRide = 1;
				GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.MountIsRide, 1);
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					PropsSystemTypes.ZuoQi,
					2,
					this.GetSpeedAdd(client)
				});
				client.ClientData.MoveSpeed = RoleAlgorithm.GetMoveSpeed(client);
				GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 14, 0L, 0, client.ClientData.MoveSpeed);
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 52, 1);
				client.sendOthersCmd(427, strcmd);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZuoQi :: 处理角色上马信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public void RoleRideMount(GameClient client)
		{
			if (ZuoQiManager.CanRide(client) == 1)
			{
				if (client.ClientData.DisMountTick + 2000U <= TimeUtil.timeGetTime())
				{
					client.ClientData.IsRide = 1;
					GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.MountIsRide, 1);
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						PropsSystemTypes.ZuoQi,
						2,
						this.GetSpeedAdd(client)
					});
					client.ClientData.MoveSpeed = RoleAlgorithm.GetMoveSpeed(client);
					GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 14, 0L, 0, client.ClientData.MoveSpeed);
					string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 52, 1);
					client.sendOthersCmd(427, strcmd);
				}
			}
		}

		
		public bool ProcessZuoQiCheckCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				if (null == client.ClientData.MountList)
				{
					return true;
				}
				foreach (MountData item in client.ClientData.MountList)
				{
					item.IsNew = false;
				}
				Global.sendToDB<int, string>(20321, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZuoQi :: 处理查看坐骑图鉴信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessZuoQiSkillModCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int oldSkillID = Global.GetRoleParamsInt32FromDB(client, "10206");
				int newSkillID = Convert.ToInt32(cmdParams[1]);
				int result = 12;
				if (null == client.ClientData.MountEquipList)
				{
					result = 12;
					newSkillID = oldSkillID;
				}
				else
				{
					foreach (GoodsData item in client.ClientData.MountEquipList)
					{
						AdvancedItem advanced = null;
						Dictionary<int, AdvancedItem> advanceDict;
						if (this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(item.GoodsID, out advanceDict))
						{
							if (advanceDict.TryGetValue(item.Forge_level, out advanced))
							{
								if (advanced.SkillID == newSkillID)
								{
									Global.SaveRoleParamsInt32ValueToDB(client, "10206", newSkillID, true);
									result = 0;
									break;
								}
							}
						}
					}
				}
				if (result != 0)
				{
					newSkillID = oldSkillID;
				}
				ExtData extData = ExtDataManager.GetClientExtData(client);
				long cdStartTicks = extData.ZuoQiSkillCDTicks - extData.ZuoQiSkillCdTime;
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, newSkillID, cdStartTicks), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZuoQi :: 处理更改角色坐骑技能信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessZuoQiUpGradeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int result = 0;
				int nextLevel = 0;
				int oldSkillID = 0;
				GoodsData goodsData = null;
				int goodsDbID = Convert.ToInt32(cmdParams[1]);
				goodsData = ZuoQiManager.GetMountEquipGoodsDataByDbID(client, goodsDbID);
				if (null == goodsData)
				{
					result = 9;
				}
				else
				{
					AdvancedItem advanced = null;
					Dictionary<int, AdvancedItem> advanceDict;
					if (!this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(goodsData.GoodsID, out advanceDict))
					{
						result = 11;
					}
					else if (!advanceDict.TryGetValue(goodsData.Forge_level + 1, out advanced))
					{
						result = 10;
					}
					else
					{
						foreach (List<int> gs in advanced.NeedGoods)
						{
							if (Global.GetTotalGoodsCountByID(client, gs[0]) < gs[1])
							{
								result = 7;
								break;
							}
						}
						if (result != 7)
						{
							foreach (List<int> gs in advanced.NeedGoods)
							{
								int needGoodsId = gs[0];
								int needNum = gs[1];
								while (needNum > 0)
								{
									GoodsData goods = Global.GetGoodsByID(client, needGoodsId);
									if (null == goods)
									{
										break;
									}
									int subNum = (needNum > goods.GCount) ? goods.GCount : needNum;
									if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goods, subNum, false, false))
									{
										result = 8;
										break;
									}
									needNum -= subNum;
									if (needNum <= 0 || result != 0)
									{
										break;
									}
								}
								if (needNum > 0)
								{
									if (result == 0)
									{
										result = 7;
									}
									GameManager.logDBCmdMgr.AddMessageLog(-1, "操作日志", "坐骑升阶", client.ClientData.RoleName, "材料不足升阶失败", "消耗", client.ClientData.RoleID, client.ClientData.ZoneID, client.strUserID, 0, GameManager.ServerId, "");
									break;
								}
							}
							if (result != 7)
							{
								int nBingProp = 1;
								string[] dbFields = null;
								string strDbCmd = Global.FormatUpdateDBGoodsStr(new object[]
								{
									client.ClientData.RoleID,
									goodsData.Id,
									"*",
									goodsData.Forge_level + 1,
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									nBingProp,
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*"
								});
								TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strDbCmd, out dbFields, client.ServerId);
								if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED || dbFields.Length <= 0 || Convert.ToInt32(dbFields[1]) < 0)
								{
									result = 8;
								}
								else
								{
									int oldUsing = goodsData.Using;
									if (goodsData.Using > 0)
									{
										goodsData.Using = 0;
										Global.RefreshEquipProp(client, goodsData);
									}
									oldSkillID = this.DelMountSkill(client, goodsData, false);
									goodsData.Forge_level++;
									goodsData.Binding = nBingProp;
									nextLevel = goodsData.Forge_level;
									if (oldUsing != goodsData.Using)
									{
										goodsData.Using = oldUsing;
										if (Global.RefreshEquipProp(client, goodsData))
										{
										}
									}
									client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
									{
										DelayExecProcIds.RecalcProps,
										DelayExecProcIds.NotifyRefreshProps
									});
									Global.ModRoleGoodsEvent(client, goodsData, 0, "强化", false);
									EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "强化");
								}
							}
						}
					}
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					result,
					cmdParams[1],
					nextLevel,
					goodsData.Binding
				}), false);
				this.AddMountSkill(client, goodsData, oldSkillID == Global.GetRoleParamsInt32FromDB(client, "10206"));
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZuoQi :: 处理更改坐骑升阶信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessZuoQiUpLevelCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int level = Global.GetRoleParamsInt32FromDB(client, "10207");
				int result = 0;
				int maxLevel = 0;
				int roleLevel = Global.GetUnionLevel(client, false);
				foreach (KeyValuePair<int, int> item in this.Level2UpLevel)
				{
					if (roleLevel <= item.Key)
					{
						break;
					}
					maxLevel = item.Value;
				}
				LevelUpItem nextLevel;
				if (maxLevel <= level + 1)
				{
					result = 5;
				}
				else if (!this.ZuoQiRunTimeData.LevelUpDict.TryGetValue(level + 1, out nextLevel))
				{
					result = 5;
				}
				else if ((long)nextLevel.Exp > client.ClientData.HunJing)
				{
					result = 6;
				}
				else if (!GameManager.ClientMgr.ModifyHunJingValue(client, -nextLevel.Exp, "坐骑栏升级消耗", true, true, false))
				{
					result = 6;
				}
				else
				{
					level++;
					Global.SaveRoleParamsInt32ValueToDB(client, "10207", level, true);
					client.ClientData.ZuoQiMainData.MountLevel = level;
					client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
					{
						DelayExecProcIds.RecalcProps,
						DelayExecProcIds.NotifyRefreshProps
					});
				}
				client.sendCmd(nID, string.Format("{0}:{1}", result, level), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZuoQi :: 处理更改坐骑栏升级信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessResetMountBagCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (null != client.ClientData.MountStoreList)
				{
					lock (client.ClientData.MountStoreList)
					{
						Dictionary<string, GoodsData> oldGoodsDict = new Dictionary<string, GoodsData>();
						List<GoodsData> toRemovedGoodsDataList = new List<GoodsData>();
						for (int i = 0; i < client.ClientData.MountStoreList.Count; i++)
						{
							client.ClientData.MountStoreList[i].BagIndex = 1;
							int gridNum = Global.GetGoodsGridNumByID(client.ClientData.MountStoreList[i].GoodsID);
							if (gridNum > 1)
							{
								GoodsData oldGoodsData = null;
								string key = string.Format("{0}_{1}_{2}", client.ClientData.MountStoreList[i].GoodsID, client.ClientData.MountStoreList[i].Binding, Global.DateTimeTicks(client.ClientData.MountStoreList[i].Endtime));
								if (oldGoodsDict.TryGetValue(key, out oldGoodsData))
								{
									int toAddNum = Global.GMin(gridNum - oldGoodsData.GCount, client.ClientData.MountStoreList[i].GCount);
									oldGoodsData.GCount += toAddNum;
									client.ClientData.MountStoreList[i].GCount -= toAddNum;
									client.ClientData.MountStoreList[i].BagIndex = 1;
									oldGoodsData.BagIndex = 1;
									if (!Global.ResetBagGoodsData(client, client.ClientData.MountStoreList[i]))
									{
										break;
									}
									if (oldGoodsData.GCount >= gridNum)
									{
										if (client.ClientData.MountStoreList[i].GCount > 0)
										{
											oldGoodsDict[key] = client.ClientData.MountStoreList[i];
										}
										else
										{
											oldGoodsDict.Remove(key);
											toRemovedGoodsDataList.Add(client.ClientData.MountStoreList[i]);
										}
									}
									else if (client.ClientData.MountStoreList[i].GCount <= 0)
									{
										toRemovedGoodsDataList.Add(client.ClientData.MountStoreList[i]);
									}
								}
								else
								{
									oldGoodsDict[key] = client.ClientData.MountStoreList[i];
								}
							}
						}
						for (int i = 0; i < toRemovedGoodsDataList.Count; i++)
						{
							client.ClientData.MountStoreList.Remove(toRemovedGoodsDataList[i]);
						}
						client.ClientData.MountStoreList.Sort((GoodsData x, GoodsData y) => y.GoodsID - x.GoodsID);
						int index = 0;
						for (int i = 0; i < client.ClientData.MountStoreList.Count; i++)
						{
							client.ClientData.MountStoreList[i].BagIndex = index++;
							if (!Global.ResetBagGoodsData(client, client.ClientData.MountStoreList[i]))
							{
								break;
							}
						}
					}
				}
				client.sendCmd<List<GoodsData>>(nID, client.ClientData.MountStoreList, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZuoQi :: 处理整理坐骑仓库信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessResetMountGradeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int DbID = Convert.ToInt32(cmdParams[1]);
				int tagLevel = 0;
				GoodsData goodsData = null;
				goodsData = Global.GetGoodsByDbID(client, DbID);
				if (null == goodsData)
				{
					result = 9;
				}
				else
				{
					Dictionary<int, int> dict = this.GetZuoQiNeedGoodsNumForCurrLevel(goodsData.GoodsID, goodsData.Forge_level);
					if (dict == null)
					{
						result = 3;
					}
					else
					{
						Dictionary<int, int> dictRate = new Dictionary<int, int>();
						double rate = this.GetZuoQiResetRate();
						foreach (KeyValuePair<int, int> it in dict)
						{
							if (dictRate.ContainsKey(it.Key))
							{
								Dictionary<int, int> dictionary;
								int key;
								(dictionary = dictRate)[key = it.Key] = dictionary[key] + (int)Math.Floor(rate * (double)it.Value);
							}
							else
							{
								dictRate.Add(it.Key, (int)Math.Floor(rate * (double)it.Value));
							}
						}
						if (!ZuoQiManager.CanAddGoodsNum(client, dictRate.Count))
						{
							result = 2;
						}
						else
						{
							bool flag = false;
							foreach (KeyValuePair<int, int> it in dictRate)
							{
								if (!Global.CanAddGoods2(client, it.Key, it.Value, 1, "1900-01-01 12:00:00", true))
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								result = 2;
							}
							else
							{
								int nBingProp = 1;
								string[] dbFields = null;
								string strDbCmd = Global.FormatUpdateDBGoodsStr(new object[]
								{
									client.ClientData.RoleID,
									goodsData.Id,
									"*",
									tagLevel,
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									nBingProp,
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*"
								});
								TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strDbCmd, out dbFields, client.ServerId);
								if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED || dbFields.Length <= 0 || Convert.ToInt32(dbFields[1]) < 0)
								{
									result = 8;
								}
								else
								{
									foreach (KeyValuePair<int, int> it in dictRate)
									{
										Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, it.Key, it.Value, 0, "", 0, nBingProp, 0, "", true, 1, string.Format("坐骑重置", new object[0]), false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
									}
									if (goodsData.Binding != 1)
									{
										goodsData.Binding = nBingProp;
									}
									goodsData.Forge_level = tagLevel;
									Global.ModRoleGoodsEvent(client, goodsData, 0, "坐骑重置", false);
								}
							}
						}
					}
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					result,
					cmdParams[1],
					goodsData.Forge_level,
					goodsData.Binding
				}), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZuoQi :: 重置坐骑信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public static GoodsData AddZuoQiGoodsData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string startTime, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife, int bagIndex = 0, List<int> washProps = null)
		{
			GoodsData gd = new GoodsData
			{
				Id = id,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = startTime,
				Endtime = endTime,
				Site = site,
				Quality = quality,
				Props = "",
				GCount = goodsNum,
				Binding = 1,
				Jewellist = jewelList,
				BagIndex = bagIndex,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong,
				ExcellenceInfo = ExcellenceProperty,
				AppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = nEquipChangeLife,
				WashProps = washProps
			};
			if (null == client.ClientData.MountStoreList)
			{
				client.ClientData.MountStoreList = new List<GoodsData>();
			}
			lock (client.ClientData.MountStoreList)
			{
				client.ClientData.MountStoreList.Add(gd);
			}
			if (null == client.ClientData.MountList)
			{
				client.ClientData.MountList = new List<MountData>();
			}
			foreach (MountData item in client.ClientData.MountList)
			{
				if (item.GoodsID == gd.GoodsID)
				{
					return gd;
				}
			}
			GoodsData result;
			if (!ZuoQiManager.CheckIsZuoQiByGoodsID(gd.GoodsID))
			{
				result = gd;
			}
			else
			{
				client.ClientData.MountList.Add(new MountData
				{
					GoodsID = gd.GoodsID,
					IsNew = true
				});
				Global.sendToDB<int, string>(20320, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, gd.GoodsID, 1), client.ServerId);
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					DelayExecProcIds.RecalcProps,
					DelayExecProcIds.NotifyRefreshProps
				});
				result = gd;
			}
			return result;
		}

		
		public static void ProcessZuoQiTuJian(GameClient client, int goodsId)
		{
			if (ZuoQiManager.CheckIsZuoQiByGoodsID(goodsId))
			{
				if (null == client.ClientData.MountList)
				{
					client.ClientData.MountList = new List<MountData>();
				}
				List<MountData> list = client.ClientData.MountList;
				if (!list.Any((MountData x) => x.GoodsID == goodsId))
				{
					list.Add(new MountData
					{
						GoodsID = goodsId,
						IsNew = true
					});
				}
			}
		}

		
		public static GoodsData GetMountFromEquipListByID(GameClient client, int goodsDbID)
		{
			GoodsData result;
			if (null == client.ClientData.MountEquipList)
			{
				result = null;
			}
			else
			{
				foreach (GoodsData goods in client.ClientData.MountEquipList)
				{
					if (goods.Id == goodsDbID)
					{
						return goods;
					}
				}
				result = null;
			}
			return result;
		}

		
		public static int GetIdleSlotOfZuoQiStoreGoods(GameClient client)
		{
			int idelPos = 0;
			int result;
			if (null == client.ClientData.MountStoreList)
			{
				result = idelPos;
			}
			else
			{
				List<int> usedBagIndex = new List<int>();
				for (int i = 0; i < client.ClientData.MountStoreList.Count; i++)
				{
					if (usedBagIndex.IndexOf(client.ClientData.MountStoreList[i].BagIndex) < 0)
					{
						usedBagIndex.Add(client.ClientData.MountStoreList[i].BagIndex);
					}
				}
				for (int j = 0; j < ZuoQiManager.GetMaxMountCount(); j++)
				{
					if (usedBagIndex.IndexOf(j) < 0)
					{
						idelPos = j;
						break;
					}
				}
				result = idelPos;
			}
			return result;
		}

		
		public static int GetIdleSlotOfZuoQiEquipGoods(GameClient client)
		{
			int idelPos = 0;
			int result;
			if (null == client.ClientData.MountEquipList)
			{
				result = idelPos;
			}
			else
			{
				List<int> usedBagIndex = new List<int>();
				for (int i = 0; i < client.ClientData.MountEquipList.Count; i++)
				{
					if (usedBagIndex.IndexOf(client.ClientData.MountEquipList[i].BagIndex) < 0)
					{
						usedBagIndex.Add(client.ClientData.MountEquipList[i].BagIndex);
					}
				}
				for (int j = 0; j < ZuoQiManager.GetMaxMountEquipCount(); j++)
				{
					if (usedBagIndex.IndexOf(j) < 0)
					{
						idelPos = j;
						break;
					}
				}
				result = idelPos;
			}
			return result;
		}

		
		public static bool RemoveStoreGoodsData(GameClient client, GoodsData gd)
		{
			bool result;
			if (null == gd)
			{
				result = false;
			}
			else if (client.ClientData.MountStoreList == null)
			{
				result = false;
			}
			else
			{
				bool ret = false;
				lock (client.ClientData.MountStoreList)
				{
					ret = client.ClientData.MountStoreList.Remove(gd);
				}
				result = ret;
			}
			return result;
		}

		
		public static bool RemoveEquipGoodsData(GameClient client, GoodsData gd)
		{
			bool result;
			if (null == gd)
			{
				result = false;
			}
			else if (client.ClientData.MountEquipList == null)
			{
				result = false;
			}
			else
			{
				bool ret = false;
				lock (client.ClientData.MountEquipList)
				{
					ret = client.ClientData.MountEquipList.Remove(gd);
				}
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					DelayExecProcIds.RecalcProps,
					DelayExecProcIds.NotifyRefreshProps
				});
				ZuoQiManager.getInstance().DelMountSkill(client, gd, true);
				result = ret;
			}
			return result;
		}

		
		public List<int> GetZuoQiSkillList(GameClient client)
		{
			List<int> ret = new List<int>();
			lock (this.ZuoQiRunTimeData.Mutex)
			{
				if (null == client.ClientData.MountEquipList)
				{
					return ret;
				}
				foreach (GoodsData item in client.ClientData.MountEquipList)
				{
					AdvancedItem advanced = null;
					Dictionary<int, AdvancedItem> advanceDict;
					if (this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(item.GoodsID, out advanceDict))
					{
						if (advanceDict.TryGetValue(item.Forge_level, out advanced))
						{
							ret.Add(advanced.SkillID);
						}
					}
				}
			}
			return ret;
		}

		
		public static void AddMountEquipGoodsData(GameClient client, GoodsData goodsData)
		{
			if (goodsData.Site == 0 || goodsData.Site == 12000)
			{
				if (null == client.ClientData.MountEquipList)
				{
					client.ClientData.MountEquipList = new List<GoodsData>();
				}
				lock (client.ClientData.MountEquipList)
				{
					client.ClientData.MountEquipList.Add(goodsData);
				}
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					DelayExecProcIds.RecalcProps,
					DelayExecProcIds.NotifyRefreshProps
				});
				ZuoQiManager.getInstance().AddMountSkill(client, goodsData, false);
			}
		}

		
		public void RefreshProps(GameClient client)
		{
			try
			{
				double[] _ExtProps = new double[177];
				if (null != client.ClientData.MountList)
				{
					foreach (MountData mount in client.ClientData.MountList)
					{
						PokedexItem pokedex;
						if (this.ZuoQiRunTimeData.PokedexDict.TryGetValue(mount.GoodsID, out pokedex))
						{
							for (int i = 0; i < 177; i++)
							{
								_ExtProps[i] += pokedex.PokedexAttribute[i];
							}
						}
					}
					int level = Global.GetRoleParamsInt32FromDB(client, "10207") + 1;
					int superior = 0;
					int order = 0;
					foreach (GoodsData item in client.ClientData.MountEquipList)
					{
						if (null != item.WashProps)
						{
							superior += item.WashProps.Count / 2;
						}
						order += item.Forge_level + 1;
					}
					List<ArrayAdditionItem> arrayAdditionList;
					if (this.ZuoQiRunTimeData.ArrayAdditiionDict.TryGetValue(1, out arrayAdditionList))
					{
						foreach (ArrayAdditionItem item2 in arrayAdditionList)
						{
							if (level >= item2.NeedLevel)
							{
								foreach (KeyValuePair<int, double> item3 in item2.AdditionProps)
								{
									_ExtProps[item3.Key] += item3.Value;
								}
								break;
							}
						}
					}
					if (this.ZuoQiRunTimeData.ArrayAdditiionDict.TryGetValue(2, out arrayAdditionList))
					{
						foreach (ArrayAdditionItem item2 in arrayAdditionList)
						{
							if (superior >= item2.NeedSuperiorNum)
							{
								foreach (KeyValuePair<int, double> item3 in item2.AdditionProps)
								{
									_ExtProps[item3.Key] += item3.Value;
								}
								break;
							}
						}
					}
					if (this.ZuoQiRunTimeData.ArrayAdditiionDict.TryGetValue(3, out arrayAdditionList))
					{
						foreach (ArrayAdditionItem item2 in arrayAdditionList)
						{
							if (order >= item2.NeedOrderNum)
							{
								foreach (KeyValuePair<int, double> item3 in item2.AdditionProps)
								{
									_ExtProps[item3.Key] += item3.Value;
								}
								break;
							}
						}
					}
					List<int> equipList = client.ClientData.MountEquipList.ConvertAll<int>((GoodsData _g) => _g.GoodsID);
					foreach (SuitItem suit in this.ZuoQiRunTimeData.SuitList)
					{
						bool canAdd = true;
						List<int> equipTmpList = new List<int>();
						equipTmpList.AddRange(equipList);
						foreach (int tmp in suit.HorseIDList)
						{
							if (!equipTmpList.Remove(tmp))
							{
								canAdd = false;
								break;
							}
						}
						if (canAdd)
						{
							foreach (KeyValuePair<int, double> extProp in suit.HorseSuitProps)
							{
								_ExtProps[extProp.Key] += extProp.Value;
							}
						}
					}
				}
				double[] extProps = new double[177];
				double[] extProps2 = new double[177];
				double[] extProps3 = new double[177];
				if (null != client.ClientData.GoodsDataList)
				{
					List<GoodsData> goodsDataList = new List<GoodsData>();
					goodsDataList.AddRange(client.ClientData.GoodsDataList);
					lock (this.ZuoQiRunTimeData.Mutex)
					{
						for (int i = 0; i < this.ZuoQiRunTimeData.HorseEquipAdditionItemList.Count; i++)
						{
							HorseEquipAdditionItem item4 = this.ZuoQiRunTimeData.HorseEquipAdditionItemList[i];
							int totalZuoQiEquipLevel = 0;
							int totalZuoQiEquipAppendLevel = 0;
							int totalZuoQiEquipSuitLevel = 0;
							foreach (GoodsData g in goodsDataList)
							{
								if (g.Using > 0 && GoodsUtil.IsZuoQiEquip(g.GoodsID))
								{
									totalZuoQiEquipLevel += g.Forge_level;
									totalZuoQiEquipAppendLevel += g.AppendPropLev;
									totalZuoQiEquipSuitLevel += Global.GetEquipGoodsSuitID(g.GoodsID);
								}
							}
							if (item4.Type == 1 && totalZuoQiEquipLevel >= item4.NeedStrengthenLevel)
							{
								extProps = item4.ExtProps;
							}
							else if (item4.Type == 2 && totalZuoQiEquipAppendLevel >= item4.NeedAdditionLevel)
							{
								extProps2 = item4.ExtProps;
							}
							else if (item4.Type == 3 && totalZuoQiEquipSuitLevel >= item4.NeedOrderNum)
							{
								extProps3 = item4.ExtProps;
							}
						}
					}
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.ZuoQiEquip,
					1,
					extProps
				});
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.ZuoQiEquip,
					2,
					extProps2
				});
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.ZuoQiEquip,
					3,
					extProps3
				});
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.ZuoQi,
					_ExtProps
				});
				if (client.ClientData.IsRide > 0)
				{
					this.RoleRideMount(client);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZuoQi :: 更新角色坐骑属性加成失败，rid={0}。", client.ClientData.RoleID), ex, true);
			}
		}

		
		public double GetExtpropsAddPercent(GameClient client, GoodsData goods)
		{
			double ret = 0.0;
			int level = Global.GetRoleParamsInt32FromDB(client, "10207");
			LevelUpItem levelItem;
			if (this.ZuoQiRunTimeData.LevelUpDict.TryGetValue(level, out levelItem))
			{
				ret += levelItem.AdvancedEffect;
			}
			Dictionary<int, AdvancedItem> advancedDict;
			if (this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(goods.GoodsID, out advancedDict))
			{
				AdvancedItem advanced;
				if (advancedDict.TryGetValue(goods.Forge_level, out advanced))
				{
					ret += advanced.AdvancedEffect;
				}
			}
			return ret;
		}

		
		public void AddMountSkill(GameClient client, GoodsData goodsData, bool isEquip = false)
		{
			if (null != goodsData)
			{
				AdvancedItem advanced = null;
				Dictionary<int, AdvancedItem> advanceDict;
				if (this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(goodsData.GoodsID, out advanceDict))
				{
					if (advanceDict.TryGetValue(goodsData.Forge_level, out advanced))
					{
						Global.AddSkillData(client, -1, advanced.SkillID, 0);
						if (isEquip)
						{
							Global.SaveRoleParamsInt32ValueToDB(client, "10206", advanced.SkillID, true);
							ExtData extData = ExtDataManager.GetClientExtData(client);
							long cdStartTicks = extData.ZuoQiSkillCDTicks - extData.ZuoQiSkillCdTime;
							client.sendCmd(1901, string.Format("{0}:{1}:{2}", 0, advanced.SkillID, cdStartTicks), false);
						}
					}
				}
			}
		}

		
		public int DelMountSkill(GameClient client, GoodsData goodsData, bool notifyClient = true)
		{
			int result;
			if (null == goodsData)
			{
				result = 0;
			}
			else
			{
				AdvancedItem advanced = null;
				Dictionary<int, AdvancedItem> advanceDict;
				if (!this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(goodsData.GoodsID, out advanceDict))
				{
					result = 0;
				}
				else if (!advanceDict.TryGetValue(goodsData.Forge_level, out advanced))
				{
					result = 0;
				}
				else
				{
					int skillID = advanced.SkillID;
					Global.DelSkillData(client, skillID);
					if (notifyClient && skillID == Global.GetRoleParamsInt32FromDB(client, "10206"))
					{
						Global.SaveRoleParamsInt32ValueToDB(client, "10206", 0, true);
						ExtData extData = ExtDataManager.GetClientExtData(client);
						long cdStartTicks = extData.ZuoQiSkillCDTicks - extData.ZuoQiSkillCdTime;
						client.sendCmd(1901, string.Format("{0}:{1}:{2}", 0, 0, cdStartTicks), false);
					}
					result = skillID;
				}
			}
			return result;
		}

		
		public double GetSpeedAdd(GameClient client)
		{
			double result;
			if (null == client.ClientData.MountEquipList)
			{
				result = 0.0;
			}
			else if (!Global.CanMapRideHorse(client.ClientData.MapCode))
			{
				result = 0.0;
			}
			else
			{
				foreach (GoodsData item in client.ClientData.MountEquipList)
				{
					if (item.Using == 1)
					{
						PokedexItem pokedex;
						if (!this.ZuoQiRunTimeData.PokedexDict.TryGetValue(item.GoodsID, out pokedex))
						{
							return 0.0;
						}
						return pokedex.HorseSpeed;
					}
				}
				result = 0.0;
			}
			return result;
		}

		
		public static int CanRide(GameClient client)
		{
			int result;
			if (null == client.ClientData.MountEquipList)
			{
				result = 0;
			}
			else if (client.buffManager.IsBuffEnabled(121))
			{
				result = 0;
			}
			else if (!Global.CanMapRideHorse(client.ClientData.MapCode))
			{
				result = 0;
			}
			else
			{
				foreach (GoodsData item in client.ClientData.MountEquipList)
				{
					if (item.Using == 1)
					{
						return 1;
					}
				}
				result = 0;
			}
			return result;
		}

		
		public static bool CanAddGoodsNum(GameClient client, int num)
		{
			return client != null && num > 0 && num + client.ClientData.MountStoreList.Count <= ZuoQiManager.GetMaxMountCount();
		}

		
		public static int GetMaxMountCount()
		{
			return 240;
		}

		
		public static int GetMaxMountEquipCount()
		{
			return 4;
		}

		
		public static GoodsData GetMountStoreGoodsDataByDbID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.MountStoreList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.MountStoreList)
				{
					for (int i = 0; i < client.ClientData.MountStoreList.Count; i++)
					{
						if (client.ClientData.MountStoreList[i].Id == id)
						{
							return client.ClientData.MountStoreList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		
		public static GoodsData GetMountEquipGoodsDataByDbID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.MountEquipList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.MountEquipList)
				{
					for (int i = 0; i < client.ClientData.MountEquipList.Count; i++)
					{
						if (client.ClientData.MountEquipList[i].Id == id)
						{
							return client.ClientData.MountEquipList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		
		public static bool CanAddGoodsToMountEquip(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true)
		{
			bool result;
			if (client.ClientData.MountEquipList == null)
			{
				result = true;
			}
			else
			{
				lock (client.ClientData.MountEquipList)
				{
					result = (client.ClientData.MountEquipList.Count < ZuoQiManager.GetMaxMountEquipCount());
				}
			}
			return result;
		}

		
		public TCPProcessCmdResults SaleMountProcess(GameClient client, int nRoleID, string strGoodsID)
		{
			TCPProcessCmdResults result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.ZuoQi, false))
			{
				result = TCPProcessCmdResults.RESULT_OK;
			}
			else
			{
				int totalJingYuan = 0;
				int totalZaiZao = 0;
				int totalHunJing = 0;
				string[] idsList = strGoodsID.Split(new char[]
				{
					','
				});
				int i = 0;
				while (i < idsList.Length)
				{
					int goodsDbID = Global.SafeConvertToInt32(idsList[i]);
					GoodsData goodsData = Global.GetGoodsByDbID(client, goodsDbID);
					if (goodsData != null && goodsData.Site == 0 && goodsData.Using <= 0 && goodsData.Forge_level == 0)
					{
						bool isZuoQi = ZuoQiManager.CheckIsZuoQiByGoodsID(goodsData.GoodsID);
						bool isZuoQiEquip = ZuoQiManager.CheckIsZuoQiEquipByGoodsID(goodsData.GoodsID);
						if (isZuoQi || isZuoQiEquip)
						{
							SystemXmlItem xmlItem = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out xmlItem) && null != xmlItem)
							{
								int hunJing = xmlItem.GetIntValue("ChangeHunJing", -1);
								int nCanExchangedJingYuan = 0;
								int nZaiZao = 0;
								if (isZuoQi)
								{
									AdvancedItem advanced = null;
									Dictionary<int, AdvancedItem> advanceDict;
									if (!this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(goodsData.GoodsID, out advanceDict))
									{
										goto IL_354;
									}
									if (!advanceDict.TryGetValue(goodsData.Forge_level, out advanced))
									{
										goto IL_354;
									}
									if (advanced.ChangeHunJing > 0)
									{
										hunJing += advanced.ChangeHunJing;
									}
								}
								if (isZuoQiEquip)
								{
									double nModulus = 1.0;
									if (goodsData.ExcellenceInfo != 0)
									{
										int nCount = Global.GetEquipExcellencePropNum(goodsData);
										if (nCount != 0)
										{
											double[] nValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuoYueHuiShouXiShu", ',');
											if (nValue != null && nValue.Length >= nCount)
											{
												nModulus = nValue[nCount - 1];
											}
										}
									}
									nCanExchangedJingYuan += (int)((double)xmlItem.GetIntValue("ChangeJinYuan", -1) * nModulus);
									nZaiZao += xmlItem.GetIntValue("ChangeZaiZao", -1);
									if (nZaiZao > 0)
									{
										if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.Artifact, false))
										{
											goto IL_354;
										}
										int excellentCount = Global.GetEquipExcellencePropNum(goodsData);
										if (excellentCount > 0)
										{
											int[] rates = GameManager.systemParamsList.GetParamValueIntArrayByName("ZhuoYueHuiShouZaiZaoXiShu", ',');
											nZaiZao *= rates[excellentCount - 1];
										}
									}
								}
								if (nCanExchangedJingYuan > 0 || nZaiZao > 0 || hunJing > 0)
								{
									string modGoodsCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										client.ClientData.RoleID,
										4,
										goodsData.Id,
										goodsData.GoodsID,
										0,
										goodsData.Site,
										goodsData.GCount,
										goodsData.BagIndex,
										""
									});
									int _gccount = goodsData.GCount;
									if (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, modGoodsCmd, "客户端修改", null))
									{
										totalJingYuan += nCanExchangedJingYuan;
										totalZaiZao += nZaiZao;
										totalHunJing += hunJing;
									}
								}
							}
						}
					}
					IL_354:
					i++;
					continue;
					goto IL_354;
				}
				if (totalJingYuan > 0)
				{
					SevenDayGoalEventObject evSaleBack = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.RecoverMoJing);
					evSaleBack.Arg1 = totalJingYuan;
					GlobalEventSource.getInstance().fireEvent(evSaleBack);
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, totalJingYuan, "一键出售或者回收", true, true, false);
				}
				if (totalZaiZao > 0)
				{
					GameManager.ClientMgr.ModifyZaiZaoValue(client, totalZaiZao, "一键出售或者回收", true, true, false);
				}
				if (totalHunJing > 0)
				{
					GameManager.ClientMgr.ModifyHunJingValue(client, totalHunJing, "一键出售或者回收", true, true, false);
				}
				result = TCPProcessCmdResults.RESULT_OK;
			}
			return result;
		}

		
		public TCPProcessCmdResults SaleStoreMountProcess(GameClient client, int nRoleID, string strGoodsID)
		{
			TCPProcessCmdResults result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.ZuoQi, false))
			{
				result = TCPProcessCmdResults.RESULT_OK;
			}
			else
			{
				int totalJingYuan = 0;
				int totalZaiZao = 0;
				int totalHunJing = 0;
				string[] idsList = strGoodsID.Split(new char[]
				{
					','
				});
				int i = 0;
				while (i < idsList.Length)
				{
					int goodsDbID = Global.SafeConvertToInt32(idsList[i]);
					GoodsData goodsData = ZuoQiManager.GetMountStoreGoodsDataByDbID(client, goodsDbID);
					if (goodsData != null && goodsData.Site == 12000 && goodsData.Using <= 0)
					{
						bool isZuoQi = ZuoQiManager.CheckIsZuoQiByGoodsID(goodsData.GoodsID);
						bool isZuoQiEquip = ZuoQiManager.CheckIsZuoQiEquipByGoodsID(goodsData.GoodsID);
						if (isZuoQi || isZuoQiEquip)
						{
							SystemXmlItem xmlItem = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out xmlItem) && null != xmlItem)
							{
								int hunJing = xmlItem.GetIntValue("ChangeHunJing", -1);
								int nCanExchangedJingYuan = 0;
								int nZaiZao = 0;
								if (isZuoQi)
								{
									AdvancedItem advanced = null;
									Dictionary<int, AdvancedItem> advanceDict;
									if (!this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(goodsData.GoodsID, out advanceDict))
									{
										goto IL_34C;
									}
									if (!advanceDict.TryGetValue(goodsData.Forge_level, out advanced))
									{
										goto IL_34C;
									}
									if (advanced.ChangeHunJing > 0)
									{
										hunJing += advanced.ChangeHunJing;
									}
								}
								if (isZuoQiEquip)
								{
									double nModulus = 1.0;
									if (goodsData.ExcellenceInfo != 0)
									{
										int nCount = Global.GetEquipExcellencePropNum(goodsData);
										if (nCount != 0)
										{
											double[] nValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuoYueHuiShouXiShu", ',');
											if (nValue != null && nValue.Length >= nCount)
											{
												nModulus = nValue[nCount - 1];
											}
										}
									}
									nCanExchangedJingYuan += (int)((double)xmlItem.GetIntValue("ChangeJinYuan", -1) * nModulus);
									if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.Artifact, false))
									{
										goto IL_34C;
									}
									nZaiZao += xmlItem.GetIntValue("ChangeZaiZao", -1);
									if (nZaiZao > 0)
									{
										int excellentCount = Global.GetEquipExcellencePropNum(goodsData);
										if (excellentCount > 0)
										{
											int[] rates = GameManager.systemParamsList.GetParamValueIntArrayByName("ZhuoYueHuiShouZaiZaoXiShu", ',');
											nZaiZao *= rates[excellentCount - 1];
										}
									}
								}
								if (nCanExchangedJingYuan > 0 || nZaiZao > 0 || hunJing > 0)
								{
									string modGoodsCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										client.ClientData.RoleID,
										4,
										goodsData.Id,
										goodsData.GoodsID,
										0,
										goodsData.Site,
										goodsData.GCount,
										goodsData.BagIndex,
										""
									});
									int _gccount = goodsData.GCount;
									if (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, modGoodsCmd, "客户端修改", null))
									{
										totalJingYuan += nCanExchangedJingYuan;
										totalZaiZao += nZaiZao;
										totalHunJing += hunJing;
									}
								}
							}
						}
					}
					IL_34C:
					i++;
					continue;
					goto IL_34C;
				}
				if (totalJingYuan > 0)
				{
					SevenDayGoalEventObject evSaleBack = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.RecoverMoJing);
					evSaleBack.Arg1 = totalJingYuan;
					GlobalEventSource.getInstance().fireEvent(evSaleBack);
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, totalJingYuan, "一键出售或者回收", true, true, false);
				}
				if (totalZaiZao > 0)
				{
					GameManager.ClientMgr.ModifyZaiZaoValue(client, totalZaiZao, "一键出售或者回收", true, true, false);
				}
				if (totalHunJing > 0)
				{
					GameManager.ClientMgr.ModifyHunJingValue(client, totalHunJing, "一键出售或者回收", true, true, false);
				}
				result = TCPProcessCmdResults.RESULT_OK;
			}
			return result;
		}

		
		public List<int> CalZhuoYueByID(int code)
		{
			List<int> ret = new List<int>();
			try
			{
				SuperiorDropItem superiorDrop;
				if (!this.ZuoQiRunTimeData.SuperiorDropDict.TryGetValue(code, out superiorDrop))
				{
					return ret;
				}
				double superiorRandom = Global.GetRandom();
				int commonSuperiorNum = 0;
				foreach (double[] commonSuperiorRate in superiorDrop.CommonSuperiorRate)
				{
					if (superiorRandom < commonSuperiorRate[0])
					{
						commonSuperiorNum = Convert.ToInt32(commonSuperiorRate[1]);
						break;
					}
				}
				superiorRandom = Global.GetRandom();
				int seniorSuperiorNum = 0;
				foreach (double[] seniorSuperiorRate in superiorDrop.SeniorSuperiorRate)
				{
					if (superiorRandom < seniorSuperiorRate[0])
					{
						seniorSuperiorNum = Convert.ToInt32(seniorSuperiorRate[1]);
						break;
					}
				}
				ret.AddRange(this.GetSuperior(superiorDrop.CommonSuperiorBank, commonSuperiorNum));
				ret.AddRange(this.GetSuperior(superiorDrop.SeniorSuperiorBank, seniorSuperiorNum));
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ZuoQi :: 根据卓越ID计算随机卓越属性，code={0}。", code), ex, true);
			}
			return ret;
		}

		
		public ZuoQiMini GetRandomMount(List<MountRandomItem> mountList)
		{
			ZuoQiMini goods = null;
			int random = Global.GetRandomNumber(1, 100001);
			lock (this.ZuoQiRunTimeData.Mutex)
			{
				foreach (MountRandomItem one in mountList)
				{
					if (random >= one.BeginNum && random <= one.EndNum)
					{
						goods = new ZuoQiMini
						{
							GoodsID = one.GoodsID,
							Binding = 1,
							WashProps = new List<int>()
						};
						goods.WashProps.AddRange(this.CalZhuoYueByID(one.SuperiorAttributeID));
						break;
					}
				}
			}
			return goods;
		}

		
		public List<int> GetSuperior(List<int> bankListSource, int num)
		{
			List<int> ret = new List<int>();
			List<int> bankList = new List<int>();
			bankList.AddRange(bankListSource);
			List<int> result;
			if (num <= 0 || num > bankList.Count)
			{
				result = ret;
			}
			else
			{
				lock (this.ZuoQiRunTimeData.Mutex)
				{
					for (int i = 0; i < num; i++)
					{
						if (bankList.Count < 1)
						{
							break;
						}
						int random = Global.GetRandomNumber(0, bankList.Count);
						SuperiorTypeItem typeItem;
						if (!this.ZuoQiRunTimeData.SuperiorTypeDict.TryGetValue(bankList[random], out typeItem))
						{
							break;
						}
						double random2 = Global.GetRandom();
						foreach (double[] paramterRate in typeItem.Parameter)
						{
							if (random2 <= paramterRate[0])
							{
								ret.Add(typeItem.Type);
								ret.Add(Convert.ToInt32(paramterRate[1]));
								break;
							}
						}
						bankList.RemoveAt(random);
					}
				}
				result = ret;
			}
			return result;
		}

		
		public static bool CheckIsZuoQiByGoodsID(int goodsID)
		{
			SystemXmlItem systemGoods = null;
			return GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods) && systemGoods.GetIntValue("Categoriy", -1) == 340;
		}

		
		public static bool CheckIsZuoQiEquipByGoodsID(int goodsID)
		{
			SystemXmlItem systemGoods = null;
			bool result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
			{
				result = false;
			}
			else
			{
				int categoriy = systemGoods.GetIntValue("Categoriy", -1);
				result = (categoriy >= 40 && categoriy <= 45);
			}
			return result;
		}

		
		public void RoleDisMount(GameClient client, bool needLog = true)
		{
			if (null != client)
			{
				if (client.ClientData.IsRide == 1)
				{
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						PropsSystemTypes.ZuoQi,
						2,
						0
					});
					client.ClientData.MoveSpeed = RoleAlgorithm.GetMoveSpeed(client);
					GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 14, 0L, 0, client.ClientData.MoveSpeed);
					client.ClientData.IsRide = 0;
					GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.MountIsRide, 0);
					string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 52, 0);
					client.sendOthersCmd(427, strcmd);
				}
				if (needLog)
				{
					client.ClientData.DisMountTick = TimeUtil.timeGetTime();
				}
			}
		}

		
		public bool IsGongNengOpen(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, GongNengIDs.ZuoQi, hint);
		}

		
		public double GetZuoQiResetRate()
		{
			return Math.Min(GameManager.systemParamsList.GetParamValueDoubleByName("HorseReturnNum", 0.0), 1.0);
		}

		
		public Dictionary<int, int> GetZuoQiNeedGoodsNumForCurrLevel(int goodid, int level)
		{
			AdvancedItem advanced = null;
			Dictionary<int, AdvancedItem> advanceDict;
			Dictionary<int, int> result;
			if (!this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(goodid, out advanceDict))
			{
				result = null;
			}
			else if (!advanceDict.TryGetValue(level, out advanced))
			{
				result = null;
			}
			else if (advanced.Level < 1)
			{
				result = null;
			}
			else
			{
				Dictionary<int, int> res = new Dictionary<int, int>();
				foreach (AdvancedItem iter in advanceDict.Values)
				{
					foreach (List<int> it in iter.NeedGoods)
					{
						if (it.Count == 2)
						{
							if (res.ContainsKey(it[0]))
							{
								Dictionary<int, int> dictionary;
								int key;
								(dictionary = res)[key = it[0]] = dictionary[key] + it[1];
							}
							else
							{
								res.Add(it[0], it[1]);
							}
						}
					}
					if (iter.Level == advanced.Level)
					{
						break;
					}
				}
				result = res;
			}
			return result;
		}

		
		public void InitRoleZuoQiData(GameClient client)
		{
			if (this.IsGongNengOpen(client, false))
			{
				if (null == client.ClientData.MountStoreList)
				{
					client.ClientData.MountStoreList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 12000), client.ServerId);
					if (null == client.ClientData.MountStoreList)
					{
						client.ClientData.MountStoreList = new List<GoodsData>();
					}
				}
				if (null == client.ClientData.MountEquipList)
				{
					client.ClientData.MountEquipList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 13000), client.ServerId);
					if (null == client.ClientData.MountEquipList)
					{
						client.ClientData.MountEquipList = new List<GoodsData>();
					}
					foreach (GoodsData item in client.ClientData.MountEquipList)
					{
						this.AddMountSkill(client, item, false);
					}
				}
				if (null == client.ClientData.MountList)
				{
					client.ClientData.MountList = Global.sendToDB<List<MountData>, string>(20319, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
					if (null == client.ClientData.MountList)
					{
						client.ClientData.MountList = new List<MountData>();
					}
				}
				client.ClientData.IsRide = ZuoQiManager.CanRide(client);
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					DelayExecProcIds.RecalcProps,
					DelayExecProcIds.NotifyRefreshProps
				});
				client.ClientData.ZuoQiMainData = new ZuoQiMainData
				{
					MountLevel = Global.GetRoleParamsInt32FromDB(client, "10207")
				};
			}
		}

		
		public void OnLogin(GameClient client)
		{
			if (this.IsGongNengOpen(client, false))
			{
				ExtData extData = ExtDataManager.GetClientExtData(client);
				long cdStartTicks = extData.ZuoQiSkillCDTicks - extData.ZuoQiSkillCdTime;
				client.sendCmd(1901, string.Format("{0}:{1}:{2}", 0, Global.GetRoleParamsInt32FromDB(client, "10206"), cdStartTicks), false);
				if (client.ClientData.IsRide == 1)
				{
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						PropsSystemTypes.ZuoQi,
						2,
						this.GetSpeedAdd(client)
					});
				}
			}
		}

		
		public int ReLoadConfig(bool init = false)
		{
			try
			{
				bool flag = true;
				List<MountRandomItem> mountFreeRandomList;
				if (!this.LoadHorseFreeRandom(out mountFreeRandomList))
				{
					flag = false;
				}
				List<MountRandomItem> mountRandomList;
				if (!this.LoadHorseRandom(out mountRandomList))
				{
					flag = false;
				}
				List<MountRandomItem> mountPayRandomList;
				if (!this.LoadHorsePayRandom(out mountPayRandomList))
				{
					flag = false;
				}
				List<MountRandomItem> MountFreeRandomListTeQuan;
				if (!this.LoadMountFreeRandomListTeQuan(out MountFreeRandomListTeQuan))
				{
					flag = false;
				}
				List<MountRandomItem> MountRandomListTeQuan;
				if (!this.LoadTeQuanHorseRandom(out MountRandomListTeQuan))
				{
					flag = false;
				}
				List<MountRandomItem> MountPayRandomListTeQuan;
				if (!this.LoadTeQuanHorsePayRandom(out MountPayRandomListTeQuan))
				{
					flag = false;
				}
				Dictionary<int, SuperiorDropItem> superiorDropDict;
				if (!this.LoadSuperiorDropXml(out superiorDropDict))
				{
					flag = false;
				}
				Dictionary<int, SuperiorTypeItem> superiorTypeDict;
				if (!this.LoadSuperiorTypeXml(out superiorTypeDict))
				{
					flag = false;
				}
				Dictionary<int, PokedexItem> pokedexDict;
				if (!this.LoadPokedexXml(out pokedexDict))
				{
					flag = false;
				}
				Dictionary<int, LevelUpItem> levelUpDict;
				if (!this.LoadLevelUpXml(out levelUpDict))
				{
					flag = false;
				}
				Dictionary<int, Dictionary<int, AdvancedItem>> advanceDict;
				if (!this.LoadAdvancedXml(out advanceDict))
				{
					flag = false;
				}
				Dictionary<int, List<ArrayAdditionItem>> arrayAdditionDict;
				if (!this.LoadArrayAdditionXml(out arrayAdditionDict))
				{
					flag = false;
				}
				List<SuitItem> suitList;
				if (!this.LoadSuitXml(out suitList))
				{
					flag = false;
				}
				List<HorseEquipAdditionItem> list;
				if (!this.LoadHorseEquipAdditionXml(out list))
				{
					flag = false;
				}
				Dictionary<string, object> DefaultDict;
				List<KeyValuePair<int, int>> Level2UpLevelXml;
				if (!this.LoadDefaultXml(out DefaultDict, out Level2UpLevelXml))
				{
					flag = false;
				}
				int[] horseNotice = GameManager.systemParamsList.GetParamValueIntArrayByName("HorseNotice", ',');
				if (!flag && !init)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]{0}", "重载坐骑失败"), null, true);
					return 0;
				}
				lock (this.ZuoQiRunTimeData.Mutex)
				{
					this.ZuoQiRunTimeData.MountFreeRandomList = mountFreeRandomList;
					this.ZuoQiRunTimeData.MountRandomList = mountRandomList;
					this.ZuoQiRunTimeData.MountPayRandomList = mountPayRandomList;
					this.ZuoQiRunTimeData.MountFreeRandomListTeQuan = MountFreeRandomListTeQuan;
					this.ZuoQiRunTimeData.MountRandomListTeQuan = MountRandomListTeQuan;
					this.ZuoQiRunTimeData.MountPayRandomListTeQuan = MountPayRandomListTeQuan;
					this.ZuoQiRunTimeData.HorseNotice = ((horseNotice == null) ? new HashSet<int>() : new HashSet<int>(horseNotice));
					this.ZuoQiRunTimeData.SuperiorDropDict = superiorDropDict;
					this.ZuoQiRunTimeData.SuperiorTypeDict = superiorTypeDict;
					this.ZuoQiRunTimeData.PokedexDict = pokedexDict;
					this.ZuoQiRunTimeData.LevelUpDict = levelUpDict;
					this.ZuoQiRunTimeData.AdvancedDict = advanceDict;
					this.ZuoQiRunTimeData.ArrayAdditiionDict = arrayAdditionDict;
					this.ZuoQiRunTimeData.SuitList = suitList;
					this.ZuoQiRunTimeData.HorseEquipAdditionItemList = list;
					this.Level2UpLevel = Level2UpLevelXml;
					this.ZuoQiFreeTime = (int)DefaultDict["ZuoQiFreeTime"];
					this.ZuoQiChouQuCost = (int)DefaultDict["ZuoQiChouQuCost"];
					this.ZuoQiChouQuCost_10 = (int)DefaultDict["ZuoQiChouQuCost_10"];
					this.ConsumeHuntHorseJiFen = (double)DefaultDict["ConsumeHuntHorseJiFen"];
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_ZuoQiConfig]{0}", ex.ToString()), null, true);
			}
			return 1;
		}

		
		private bool LoadHorseFreeRandom(out List<MountRandomItem> mountFreeRandomList)
		{
			mountFreeRandomList = new List<MountRandomItem>();
			try
			{
				XElement xml = CheckHelper.LoadXml(Global.GameResPath(ZuoQiConsts.HorseFreeRandom), true);
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("读取 {0} null == xml", ZuoQiConsts.HorseFreeRandom), null, true);
					return false;
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					if (xmlItem != null)
					{
						mountFreeRandomList.Add(new MountRandomItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0")),
							GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GoodsID", "0")),
							GoodsNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Num", "0")),
							SuperiorAttributeID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "SuperiorAttributeID", "0")),
							BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "BeginNum", "0")),
							EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "EndNum", "0"))
						});
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", ZuoQiConsts.HorseFreeRandom, ex.Message), null, true);
			}
			return false;
		}

		
		private bool LoadHorseRandom(out List<MountRandomItem> mountRandomList)
		{
			mountRandomList = new List<MountRandomItem>();
			try
			{
				XElement xml = CheckHelper.LoadXml(Global.GameResPath(ZuoQiConsts.HorseRandom), true);
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("读取 {0} null == xml", ZuoQiConsts.HorseRandom), null, true);
					return false;
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					if (xmlItem != null)
					{
						int id = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
						mountRandomList.Add(new MountRandomItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0")),
							GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GoodsID", "0")),
							GoodsNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Num", "0")),
							SuperiorAttributeID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "SuperiorAttributeID", "0")),
							BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "BeginNum", "0")),
							EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "EndNum", "0"))
						});
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", ZuoQiConsts.HorseRandom, ex.Message), null, true);
			}
			return false;
		}

		
		private bool LoadHorsePayRandom(out List<MountRandomItem> mountPayRandomList)
		{
			mountPayRandomList = new List<MountRandomItem>();
			try
			{
				XElement xml = CheckHelper.LoadXml(Global.GameResPath(ZuoQiConsts.HorsePayRandom), true);
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("读取 {0} null == xml", ZuoQiConsts.HorsePayRandom), null, true);
					return false;
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					if (xmlItem != null)
					{
						mountPayRandomList.Add(new MountRandomItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0")),
							GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GoodsID", "0")),
							GoodsNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Num", "0")),
							SuperiorAttributeID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "SuperiorAttributeID", "0")),
							BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "BeginNum", "0")),
							EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "EndNum", "0"))
						});
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", ZuoQiConsts.HorsePayRandom, ex.Message), null, true);
			}
			return false;
		}

		
		private bool LoadMountFreeRandomListTeQuan(out List<MountRandomItem> MountFreeRandomListTeQuan)
		{
			MountFreeRandomListTeQuan = new List<MountRandomItem>();
			try
			{
				XElement xml = CheckHelper.LoadXml(Global.GameResPath(ZuoQiConsts.TeQuanHorseFreeRandom), true);
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("读取 {0} null == xml", ZuoQiConsts.TeQuanHorseFreeRandom), null, true);
					return false;
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					if (xmlItem != null)
					{
						MountFreeRandomListTeQuan.Add(new MountRandomItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0")),
							GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GoodsID", "0")),
							GoodsNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Num", "0")),
							SuperiorAttributeID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "SuperiorAttributeID", "0")),
							BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "BeginNum", "0")),
							EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "EndNum", "0"))
						});
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", ZuoQiConsts.TeQuanHorseFreeRandom, ex.Message), null, true);
			}
			return false;
		}

		
		private bool LoadTeQuanHorseRandom(out List<MountRandomItem> MountRandomListTeQuan)
		{
			MountRandomListTeQuan = new List<MountRandomItem>();
			try
			{
				XElement xml = CheckHelper.LoadXml(Global.GameResPath(ZuoQiConsts.TeQuanHorseRandom), true);
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("读取 {0} null == xml", ZuoQiConsts.TeQuanHorseRandom), null, true);
					return false;
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					if (xmlItem != null)
					{
						int id = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
						MountRandomListTeQuan.Add(new MountRandomItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0")),
							GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GoodsID", "0")),
							GoodsNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Num", "0")),
							SuperiorAttributeID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "SuperiorAttributeID", "0")),
							BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "BeginNum", "0")),
							EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "EndNum", "0"))
						});
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", ZuoQiConsts.TeQuanHorseRandom, ex.Message), null, true);
			}
			return false;
		}

		
		private bool LoadTeQuanHorsePayRandom(out List<MountRandomItem> MountPayRandomListTeQuan)
		{
			MountPayRandomListTeQuan = new List<MountRandomItem>();
			try
			{
				XElement xml = CheckHelper.LoadXml(Global.GameResPath(ZuoQiConsts.TeQuanHorsePayRandom), true);
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("读取 {0} null == xml", ZuoQiConsts.TeQuanHorsePayRandom), null, true);
					return false;
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					if (xmlItem != null)
					{
						MountPayRandomListTeQuan.Add(new MountRandomItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0")),
							GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GoodsID", "0")),
							GoodsNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Num", "0")),
							SuperiorAttributeID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "SuperiorAttributeID", "0")),
							BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "BeginNum", "0")),
							EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "EndNum", "0"))
						});
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", ZuoQiConsts.TeQuanHorsePayRandom, ex.Message), null, true);
			}
			return false;
		}

		
		private bool LoadSuperiorDropXml(out Dictionary<int, SuperiorDropItem> superiorDropDict)
		{
			string fileName = "";
			superiorDropDict = new Dictionary<int, SuperiorDropItem>();
			try
			{
				fileName = Global.GameResPath(ZuoQiConsts.HorseSuperiorDrop);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					int id = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
					string commonSuperiorRateStr = Global.GetDefAttributeStr(xmlItem, "CommonSuperiorRate", "");
					List<double[]> commonSuperiorRate = new List<double[]>();
					double baseVal = 0.0;
					foreach (string one in commonSuperiorRateStr.Split(new char[]
					{
						'|'
					}))
					{
						string[] val = one.Split(new char[]
						{
							','
						});
						if (val.Length >= 2)
						{
							baseVal += Convert.ToDouble(val[1]);
							commonSuperiorRate.Add(new double[]
							{
								baseVal,
								Convert.ToDouble(val[0])
							});
						}
					}
					string seniorSuperiorRateStr = Global.GetDefAttributeStr(xmlItem, "SeniorSuperiorRate", "");
					List<double[]> seniorSuperiorRate = new List<double[]>();
					baseVal = 0.0;
					foreach (string one in seniorSuperiorRateStr.Split(new char[]
					{
						'|'
					}))
					{
						string[] val = one.Split(new char[]
						{
							','
						});
						if (val.Length >= 2)
						{
							baseVal += Convert.ToDouble(val[1]);
							seniorSuperiorRate.Add(new double[]
							{
								baseVal,
								Convert.ToDouble(val[0])
							});
						}
					}
					Dictionary<int, SuperiorDropItem> dictionary = superiorDropDict;
					int key = id;
					SuperiorDropItem superiorDropItem = new SuperiorDropItem();
					superiorDropItem.CommonSuperiorRate = commonSuperiorRate;
					superiorDropItem.CommonSuperiorBank = Array.ConvertAll<string, int>(Global.GetDefAttributeStr(xmlItem, "CommonSuperiorBank", "").Split(new char[]
					{
						','
					}), (string _x) => Global.SafeConvertToInt32(_x)).ToList<int>();
					superiorDropItem.SeniorSuperiorRate = seniorSuperiorRate;
					superiorDropItem.SeniorSuperiorBank = Array.ConvertAll<string, int>(Global.GetDefAttributeStr(xmlItem, "SeniorSuperiorBank", "").Split(new char[]
					{
						','
					}), (string _x) => Global.SafeConvertToInt32(_x)).ToList<int>();
					dictionary[key] = superiorDropItem;
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
			return false;
		}

		
		private bool LoadSuperiorTypeXml(out Dictionary<int, SuperiorTypeItem> superiorTypeDict)
		{
			string fileName = "";
			superiorTypeDict = new Dictionary<int, SuperiorTypeItem>();
			try
			{
				fileName = Global.GameResPath(ZuoQiConsts.HorseSuperiorType);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					int id = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
					string parameterStr = Global.GetDefAttributeStr(xmlItem, "Parameter", "");
					List<double[]> parameter = new List<double[]>();
					double baseVal = 0.0;
					foreach (string one in parameterStr.Split(new char[]
					{
						'|'
					}))
					{
						string[] val = one.Split(new char[]
						{
							','
						});
						if (val.Length >= 2)
						{
							baseVal += Convert.ToDouble(val[1]);
							parameter.Add(new double[]
							{
								baseVal,
								Convert.ToDouble(val[0])
							});
						}
					}
					superiorTypeDict[id] = new SuperiorTypeItem
					{
						Type = (int)ConfigParser.GetPropIndexByPropName(Global.GetDefAttributeStr(xmlItem, "Type", "")),
						Parameter = parameter
					};
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
			return false;
		}

		
		private bool LoadPokedexXml(out Dictionary<int, PokedexItem> pokedexDict)
		{
			string fileName = "";
			pokedexDict = new Dictionary<int, PokedexItem>();
			try
			{
				fileName = Global.GameResPath(ZuoQiConsts.HorsePokedex);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					int goodsID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "HorseGoods", "0"));
					string tempValueString = Global.GetDefAttributeStr(xmlItem, "PokedexAttribute", "");
					string[] valueFileds = tempValueString.Split(new char[]
					{
						'|'
					});
					double[] extProps = new double[177];
					if (valueFileds.Length > 0)
					{
						foreach (string value in valueFileds)
						{
							string[] KvpFileds = value.Split(new char[]
							{
								','
							});
							if (KvpFileds.Length == 2)
							{
								ExtPropIndexes index = ConfigParser.GetPropIndexByPropName(KvpFileds[0]);
								if (index < ExtPropIndexes.Max)
								{
									extProps[(int)index] += Global.SafeConvertToDouble(KvpFileds[1]);
								}
							}
						}
					}
					pokedexDict[goodsID] = new PokedexItem
					{
						HorseGoods = goodsID,
						PokedexAttribute = extProps,
						HorseSpeed = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "HorseSpeed", "0"))
					};
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
			return false;
		}

		
		private bool LoadHorseEquipAdditionXml(out List<HorseEquipAdditionItem> list)
		{
			string fileName = "";
			list = new List<HorseEquipAdditionItem>();
			try
			{
				fileName = Global.GameResPath(ZuoQiConsts.HorseEquipAddition);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					int id = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
					string tempValueString = Global.GetDefAttributeStr(xmlItem, "AdditionProps", "");
					string[] valueFileds = tempValueString.Split(new char[]
					{
						'|'
					});
					double[] extProps = new double[177];
					if (valueFileds.Length > 0)
					{
						foreach (string value in valueFileds)
						{
							string[] KvpFileds = value.Split(new char[]
							{
								','
							});
							if (KvpFileds.Length == 2)
							{
								ExtPropIndexes index = ConfigParser.GetPropIndexByPropName(KvpFileds[0]);
								if (index < ExtPropIndexes.Max)
								{
									extProps[(int)index] += Global.SafeConvertToDouble(KvpFileds[1]);
								}
							}
						}
					}
					HorseEquipAdditionItem item = new HorseEquipAdditionItem
					{
						Type = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Type", "0")),
						NeedStrengthenLevel = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedStrengthenLevel", "0")),
						NeedAdditionLevel = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedAdditionLevel", "0")),
						NeedOrderNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedOrderNum", "0")),
						AdditionProps = tempValueString,
						ExtProps = extProps
					};
					list.Add(item);
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
			return false;
		}

		
		private bool LoadAdvancedXml(out Dictionary<int, Dictionary<int, AdvancedItem>> advanceDict)
		{
			string fileName = "";
			advanceDict = new Dictionary<int, Dictionary<int, AdvancedItem>>();
			try
			{
				fileName = Global.GameResPath(ZuoQiConsts.HorseAdvanced);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					int horseID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "HorseID", "0"));
					int level = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Level", "0")) - 1;
					string needGoodsStr = Global.GetDefAttributeStr(xmlItem, "NeedGoods", "");
					List<List<int>> needGoodsList = ConfigHelper.ParserIntArrayList(needGoodsStr, true, '|', ',');
					Dictionary<int, AdvancedItem> advanceItemDict;
					if (!advanceDict.TryGetValue(horseID, out advanceItemDict))
					{
						advanceItemDict = new Dictionary<int, AdvancedItem>();
						advanceDict[horseID] = advanceItemDict;
					}
					advanceItemDict[level] = new AdvancedItem
					{
						Level = level,
						NeedGoods = needGoodsList,
						AdvancedEffect = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "AdvancedEffect", "0")),
						SkillID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "SkillID", "0")),
						ChangeHunJing = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ChangeHunJing", "0"))
					};
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
			return false;
		}

		
		private bool LoadLevelUpXml(out Dictionary<int, LevelUpItem> levelUpDict)
		{
			string fileName = "";
			levelUpDict = new Dictionary<int, LevelUpItem>();
			try
			{
				fileName = Global.GameResPath(ZuoQiConsts.HorseLevelUp);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					int level = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Level", "0")) - 1;
					levelUpDict[level] = new LevelUpItem
					{
						Level = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Level", "0")),
						Exp = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Exp", "0")),
						AdvancedEffect = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "AdvancedEffect", "0"))
					};
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
			return false;
		}

		
		private bool LoadArrayAdditionXml(out Dictionary<int, List<ArrayAdditionItem>> arrayAdditionDict)
		{
			string fileName = "";
			arrayAdditionDict = new Dictionary<int, List<ArrayAdditionItem>>();
			try
			{
				fileName = Global.GameResPath(ZuoQiConsts.HorseArrayAddition);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					int type = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Type", "0"));
					List<ArrayAdditionItem> arrayAdditionList = null;
					if (!arrayAdditionDict.TryGetValue(type, out arrayAdditionList))
					{
						arrayAdditionList = new List<ArrayAdditionItem>();
						arrayAdditionDict[type] = arrayAdditionList;
					}
					string tempValueString = Global.GetDefAttributeStr(xmlItem, "AdditionProps", "");
					string[] valueFileds = tempValueString.Split(new char[]
					{
						'|'
					});
					List<KeyValuePair<int, double>> extProps = new List<KeyValuePair<int, double>>();
					if (valueFileds.Length > 0)
					{
						foreach (string value in valueFileds)
						{
							string[] KvpFileds = value.Split(new char[]
							{
								','
							});
							if (KvpFileds.Length == 2)
							{
								ExtPropIndexes index = ConfigParser.GetPropIndexByPropName(KvpFileds[0]);
								if (index < ExtPropIndexes.Max)
								{
									extProps.Add(new KeyValuePair<int, double>((int)index, Global.SafeConvertToDouble(KvpFileds[1])));
								}
							}
						}
					}
					arrayAdditionList.Add(new ArrayAdditionItem
					{
						Type = type,
						NeedLevel = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedLevel", "0")),
						NeedSuperiorNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedSuperiorNum", "0")),
						NeedOrderNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedOrderNum", "0")),
						AdditionProps = extProps
					});
				}
				foreach (KeyValuePair<int, List<ArrayAdditionItem>> item in arrayAdditionDict)
				{
					item.Value.Sort(delegate(ArrayAdditionItem x, ArrayAdditionItem y)
					{
						int result;
						if (x.NeedLevel > 0 && y.NeedLevel > 0)
						{
							result = y.NeedLevel - x.NeedLevel;
						}
						else if (x.NeedOrderNum > 0 && y.NeedOrderNum > 0)
						{
							result = y.NeedOrderNum - x.NeedOrderNum;
						}
						else
						{
							result = y.NeedSuperiorNum - x.NeedSuperiorNum;
						}
						return result;
					});
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
			return false;
		}

		
		private bool LoadSuitXml(out List<SuitItem> suitList)
		{
			string fileName = "";
			suitList = new List<SuitItem>();
			try
			{
				fileName = Global.GameResPath(ZuoQiConsts.HorseSuit);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					string tempValueString = Global.GetDefAttributeStr(xmlItem, "HorseSuitProps", "");
					string[] valueFileds = tempValueString.Split(new char[]
					{
						'|'
					});
					List<KeyValuePair<int, double>> extProps = new List<KeyValuePair<int, double>>();
					if (valueFileds.Length > 0)
					{
						foreach (string value in valueFileds)
						{
							string[] KvpFileds = value.Split(new char[]
							{
								','
							});
							if (KvpFileds.Length == 2)
							{
								ExtPropIndexes index = ConfigParser.GetPropIndexByPropName(KvpFileds[0]);
								if (index < ExtPropIndexes.Max)
								{
									extProps.Add(new KeyValuePair<int, double>((int)index, Global.SafeConvertToDouble(KvpFileds[1])));
								}
							}
						}
					}
					List<SuitItem> list = suitList;
					SuitItem suitItem = new SuitItem();
					suitItem.HorseIDList = Array.ConvertAll<string, int>(Global.GetDefAttributeStr(xmlItem, "HorseID", "").Split(new char[]
					{
						','
					}), (string _x) => Global.SafeConvertToInt32(_x)).ToList<int>();
					suitItem.HorseSuitProps = extProps;
					list.Add(suitItem);
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
			return false;
		}

		
		private bool LoadDefaultXml(out Dictionary<string, object> DefaultDict, out List<KeyValuePair<int, int>> Level2UpLevelXml)
		{
			Level2UpLevelXml = new List<KeyValuePair<int, int>>();
			DefaultDict = new Dictionary<string, object>();
			DefaultDict.Add("ZuoQiFreeTime", 0);
			DefaultDict.Add("ZuoQiChouQuCost", 0);
			DefaultDict.Add("ZuoQiChouQuCost_10", 0);
			DefaultDict.Add("ConsumeHuntHorseJiFen", 0);
			try
			{
				DefaultDict["ZuoQiFreeTime"] = (int)GameManager.systemParamsList.GetParamValueIntByName("HorseFreeRandom", -1);
				string[] costArr = GameManager.systemParamsList.GetParamValueByName("HorsePay").Split(new char[]
				{
					','
				});
				if (costArr.Length < 2)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败--HorsePay。", "SystemParams.xml"), null, true);
				}
				string[] horseLevelMaxArr = GameManager.systemParamsList.GetParamValueByName("HorseLevelMax").Split(new char[]
				{
					'|'
				});
				foreach (string item in horseLevelMaxArr)
				{
					string[] itemList = item.Split(new char[]
					{
						','
					});
					if (itemList.Length >= 3)
					{
						int key = Global.GetUnionLevel(Convert.ToInt32(itemList[0]), Convert.ToInt32(itemList[1]), false);
						Level2UpLevelXml.Add(new KeyValuePair<int, int>(key, Convert.ToInt32(itemList[2])));
					}
				}
				DefaultDict["ZuoQiChouQuCost"] = Convert.ToInt32(costArr[0]);
				DefaultDict["ZuoQiChouQuCost_10"] = Convert.ToInt32(costArr[1]);
				DefaultDict["ConsumeHuntHorseJiFen"] = Convert.ToDouble(GameManager.systemParamsList.GetParamValueByName("ConsumeHuntHorseJiFen"));
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", "SystemParams.xml"), ex, true);
			}
			return false;
		}

		
		public ZuoQiRunData ZuoQiRunTimeData = new ZuoQiRunData();

		
		public int ZuoQiFreeTime = 0;

		
		public int ZuoQiChouQuCost = 0;

		
		public int ZuoQiChouQuCost_10 = 0;

		
		public double ConsumeHuntHorseJiFen = 0.0;

		
		public List<KeyValuePair<int, int>> Level2UpLevel = new List<KeyValuePair<int, int>>();

		
		private static ZuoQiManager instance = new ZuoQiManager();
	}
}
