using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.TuJian;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000061 RID: 97
	public class AlchemyManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		// Token: 0x0600014C RID: 332 RVA: 0x00016350 File Offset: 0x00014550
		public static AlchemyManager getInstance()
		{
			return AlchemyManager.instance;
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00016368 File Offset: 0x00014568
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x0600014E RID: 334 RVA: 0x0001638C File Offset: 0x0001458C
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1085, 1, 1, AlchemyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1086, 3, 4, AlchemyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1087, 2, 2, AlchemyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		// Token: 0x0600014F RID: 335 RVA: 0x000163E8 File Offset: 0x000145E8
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x000163FC File Offset: 0x000145FC
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00016410 File Offset: 0x00014610
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00016424 File Offset: 0x00014624
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.Alchemy, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1085:
					result = this.ProcessAlchemyDataCmd(client, nID, bytes, cmdParams);
					break;
				case 1086:
					result = this.ProcessAlchemyAddElementCmd(client, nID, bytes, cmdParams);
					break;
				case 1087:
					result = this.ProcessAlchemyExcuteCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x000164CB File Offset: 0x000146CB
		public void OnLogin(GameClient client)
		{
			this.AlchemyRollBack(client, client.ClientData.AlchemyInfo.rollbackType);
			this.RefreshAlchemyProps(client);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x000164F0 File Offset: 0x000146F0
		public bool AlchemyRollBackOffline(int rid, string rollbackType)
		{
			return Global.sendToDB<bool, string>(13098, string.Format("{0}:{1}", rid, rollbackType), 0);
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00016520 File Offset: 0x00014720
		public bool AlchemyRollBackCheck(int costType, int useNum)
		{
			bool result;
			if (useNum <= 0 || costType < 1 || costType >= 15)
			{
				result = false;
			}
			else
			{
				Dictionary<int, AlchemyConfigData> tempAlchemyConfig = null;
				lock (this.ConfigMutex)
				{
					tempAlchemyConfig = this.AlchemyConfig;
				}
				AlchemyConfigData alchemyConfig = null;
				result = (tempAlchemyConfig.TryGetValue(costType, out alchemyConfig) && useNum >= alchemyConfig.Unit);
			}
			return result;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x000165BC File Offset: 0x000147BC
		public void AlchemyRollBack(GameClient client, string rollbackType)
		{
			if (GlobalNew.IsGongNengOpened(client, GongNengIDs.Alchemy, false))
			{
				if (!string.IsNullOrEmpty(rollbackType))
				{
					string[] valueFields = rollbackType.Split(new char[]
					{
						','
					});
					if (valueFields.Length == 2)
					{
						int costType = Global.SafeConvertToInt32(valueFields[0]);
						int useNum = Global.SafeConvertToInt32(valueFields[1]);
						Dictionary<int, AlchemyConfigData> tempAlchemyConfig = null;
						lock (this.ConfigMutex)
						{
							tempAlchemyConfig = this.AlchemyConfig;
						}
						AlchemyConfigData alchemyConfig = null;
						if (!tempAlchemyConfig.TryGetValue(costType, out alchemyConfig) || useNum < alchemyConfig.Unit)
						{
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为角色：【{0}】回滚炼金灌注【{1}】 失败！", client.ClientData.RoleID, rollbackType), null, true);
							client.ClientData.AlchemyInfo.rollbackType = "";
							this.AlchemyRollBackOffline(client.ClientData.RoleID, "");
						}
						else
						{
							useNum -= useNum % alchemyConfig.Unit;
							int element = useNum / alchemyConfig.Unit * alchemyConfig.Element;
							int histCost = 0;
							client.ClientData.AlchemyInfo.HistCost.TryGetValue(costType, out histCost);
							if (histCost < useNum || client.ClientData.AlchemyInfo.BaseData.Element < element)
							{
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为角色：【{0}】回滚炼金灌注【{1}】 失败！", client.ClientData.RoleID, rollbackType), null, true);
								client.ClientData.AlchemyInfo.rollbackType = "";
								this.AlchemyRollBackOffline(client.ClientData.RoleID, "");
							}
							else
							{
								GameManager.ClientMgr.ModifyAlchemyElementValue(client, -element, "GM命令-alchemy", false, false);
								this.ModifyAddElementCost(client, costType, useNum, true);
								client.ClientData.AlchemyInfo.HistCost[costType] = histCost - useNum;
								this.UpdateAlchemyDataDB(client);
								client.ClientData.AlchemyInfo.rollbackType = "";
								this.AlchemyRollBackOffline(client.ClientData.RoleID, "");
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为角色：【{0}】回滚炼金灌注【{1}】 成功！", client.ClientData.RoleID, rollbackType), null, true);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00016844 File Offset: 0x00014A44
		private bool CheckCostEnough(GameClient client, int costType, int useNum, bool bindOnly)
		{
			bool result;
			switch (costType)
			{
			case 1:
				result = (GameManager.ClientMgr.GetTianDiJingYuanValue(client) >= useNum);
				break;
			case 2:
				result = (GameManager.ClientMgr.GetChengJiuPointsValue(client) >= useNum);
				break;
			case 3:
				result = (GameManager.ClientMgr.GetShengWangValue(client) >= useNum);
				break;
			case 4:
				result = (client.ClientData.StarSoul >= useNum);
				break;
			case 5:
				result = (GameManager.ClientMgr.GetMUMoHeValue(client) >= useNum);
				break;
			case 6:
				result = (Global.GetRoleParamsInt32FromDB(client, "ElementPowder") >= useNum);
				break;
			case 7:
				result = (GameManager.ClientMgr.GetZaiZaoValue(client) >= useNum);
				break;
			case 8:
				result = (client != null && client.ClientData.MyGuardStatueDetail.IsActived && client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint >= useNum);
				break;
			case 9:
				result = (client.ClientData.TianTiData.RongYao >= useNum);
				break;
			case 10:
				result = (client.ClientData.FluorescentPoint >= useNum);
				break;
			case 11:
				result = (GameManager.ClientMgr.GetLangHunFenMoValue(client) >= useNum);
				break;
			case 12:
				result = (client.ClientData.ShenLiJingHuaPoints >= useNum);
				break;
			case 13:
				result = (client.ClientData.BangGong >= useNum);
				break;
			case 14:
				result = (GameManager.ClientMgr.GetOrnamentCharmPointValue(client) >= useNum);
				break;
			default:
				if (costType >= this.MinGoodsID && useNum > 0)
				{
					if (bindOnly)
					{
						result = (Global.GetTotalBindGoodsCountByID(client, costType) >= useNum);
					}
					else
					{
						result = (Global.GetTotalGoodsCountByID(client, costType) >= useNum);
					}
				}
				else
				{
					result = false;
				}
				break;
			}
			return result;
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00016A5C File Offset: 0x00014C5C
		private bool ModifyAddElementCost(GameClient client, int costType, int useNum, bool bindOnly)
		{
			bool result;
			if (0 == useNum)
			{
				result = false;
			}
			else
			{
				string strLogFrom = "炼金-灌注";
				if (useNum > 0)
				{
					strLogFrom = "炼金-灌注GM回滚";
				}
				bool success;
				switch (costType)
				{
				case 1:
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, useNum, strLogFrom, true, true, false);
					success = true;
					break;
				case 2:
					GameManager.ClientMgr.ModifyChengJiuPointsValue(client, useNum, strLogFrom, true, true);
					success = true;
					break;
				case 3:
					GameManager.ClientMgr.ModifyShengWangValue(client, useNum, strLogFrom, true, true);
					success = true;
					break;
				case 4:
					GameManager.ClientMgr.ModifyStarSoulValue(client, useNum, strLogFrom, true, true);
					success = true;
					break;
				case 5:
					GameManager.ClientMgr.ModifyMUMoHeValue(client, useNum, strLogFrom, true, true, false);
					success = true;
					break;
				case 6:
					GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, useNum, strLogFrom, true, false);
					success = true;
					break;
				case 7:
					GameManager.ClientMgr.ModifyZaiZaoValue(client, useNum, strLogFrom, true, true, false);
					success = true;
					break;
				case 8:
					SingletonTemplate<GuardStatueManager>.Instance().AddGuardPoint(client, useNum, strLogFrom);
					success = true;
					break;
				case 9:
					success = GameManager.ClientMgr.ModifyTianTiRongYaoValue(client, useNum, strLogFrom, true);
					break;
				case 10:
					if (useNum > 0)
					{
						success = GameManager.FluorescentGemMgr.AddFluorescentPoint(client, useNum, strLogFrom, true);
					}
					else
					{
						success = GameManager.FluorescentGemMgr.DecFluorescentPoint(client, -useNum, strLogFrom, false);
					}
					break;
				case 11:
					GameManager.ClientMgr.ModifyLangHunFenMoValue(client, useNum, strLogFrom, true, true);
					success = true;
					break;
				case 12:
					GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, useNum, strLogFrom, true, true);
					success = true;
					break;
				case 13:
					if (useNum > 0)
					{
						success = GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref useNum, AddBangGongTypes.Alchemy, 0);
						if (success)
						{
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战功", strLogFrom, "系统", client.ClientData.RoleName, "增加", useNum, client.ClientData.ZoneID, client.strUserID, client.ClientData.BangGong, client.ServerId, null);
						}
					}
					else
					{
						success = GameManager.ClientMgr.SubUserBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, -useNum);
						if (success)
						{
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战功", strLogFrom, "系统", client.ClientData.RoleName, "减少", -useNum, client.ClientData.ZoneID, client.strUserID, client.ClientData.BangGong, client.ServerId, null);
						}
					}
					break;
				case 14:
					GameManager.ClientMgr.ModifyOrnamentCharmPointValue(client, useNum, strLogFrom, true, true, false);
					success = true;
					break;
				default:
					if (costType >= this.MinGoodsID)
					{
						if (useNum > 0)
						{
							GoodsData goodsData = new GoodsData
							{
								GoodsID = costType,
								GCount = useNum,
								Binding = 1
							};
							if (!Global.CanAddGoodsNum(client, useNum))
							{
								Global.UseMailGivePlayerAward(client, goodsData, GLang.GetLang(5000, new object[0]), GLang.GetLang(5000, new object[0]), 1.0);
								GameManager.ClientMgr.NotifyHintMsg(client, GLang.GetLang(5001, new object[0]));
							}
							else
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, "", goodsData.Forge_level, goodsData.Binding, 0, "", true, 1, strLogFrom, goodsData.Endtime, 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
							}
						}
						else
						{
							useNum = -useNum;
							if (Global.GetTotalGoodsCountByID(client, costType) < useNum)
							{
								return false;
							}
							int bindGoodsCount = Global.GetTotalBindGoodsCountByID(client, costType);
							if (bindOnly && bindGoodsCount < useNum)
							{
								return false;
							}
							int useBindGoodsCount = Math.Min(bindGoodsCount, useNum);
							if (useBindGoodsCount > 0)
							{
								bool usedBinding;
								bool usedTimeLimited;
								if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, costType, useBindGoodsCount, false, out usedBinding, out usedTimeLimited, false))
								{
									LogManager.WriteLog(LogTypes.Error, "扣除物品时发现道具不足", null, true);
								}
							}
							int useNoBindGoodsCount = useNum - useBindGoodsCount;
							if (useNoBindGoodsCount <= 0)
							{
								return true;
							}
							bool bUsedBinding_just_placeholder;
							bool bUsedTimeLimited_just_placeholder;
							if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, costType, useNoBindGoodsCount, false, out bUsedBinding_just_placeholder, out bUsedTimeLimited_just_placeholder, false))
							{
								LogManager.WriteLog(LogTypes.Error, "扣除物品时发现道具不足", null, true);
							}
							return true;
						}
					}
					return false;
				}
				result = success;
			}
			return result;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00016F78 File Offset: 0x00015178
		private int GetTodayAddElementCost(GameClient client, int costType)
		{
			int todayCost = 0;
			int curDayID = Global.GetOffsetDay(TimeUtil.NowDateTime());
			if (curDayID == client.ClientData.AlchemyInfo.ElementDayID)
			{
				client.ClientData.AlchemyInfo.BaseData.ToDayCost.TryGetValue(costType, out todayCost);
			}
			return todayCost;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00016FD4 File Offset: 0x000151D4
		private void UpdateTodayAddElementCost(GameClient client, int costType, int useNum)
		{
			int curDayID = Global.GetOffsetDay(TimeUtil.NowDateTime());
			int todayCost = this.GetTodayAddElementCost(client, costType);
			if (curDayID > client.ClientData.AlchemyInfo.ElementDayID)
			{
				client.ClientData.AlchemyInfo.ElementDayID = curDayID;
				client.ClientData.AlchemyInfo.BaseData.ToDayCost.Clear();
			}
			client.ClientData.AlchemyInfo.BaseData.ToDayCost[costType] = todayCost + useNum;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00017060 File Offset: 0x00015260
		private void UpdateHistAddElementCost(GameClient client, int costType, int useNum)
		{
			int histCost = 0;
			client.ClientData.AlchemyInfo.HistCost.TryGetValue(costType, out histCost);
			client.ClientData.AlchemyInfo.HistCost[costType] = (int)Math.Min((long)histCost + (long)useNum, 2147483647L);
		}

		// Token: 0x0600015C RID: 348 RVA: 0x000170B4 File Offset: 0x000152B4
		public bool UpdateAlchemyDataDB(GameClient client)
		{
			return Global.sendToDB<bool, AlchemyDataDB>(13097, client.ClientData.AlchemyInfo, client.ServerId);
		}

		// Token: 0x0600015D RID: 349 RVA: 0x000170E4 File Offset: 0x000152E4
		private int RandomAlchemyProp(GameClient client)
		{
			int prop = 0;
			int minNum = 0;
			Dictionary<int, int> AlchemyValue = client.ClientData.AlchemyInfo.BaseData.AlchemyValue;
			foreach (KeyValuePair<int, int> item in AlchemyValue)
			{
				if (minNum == 0 || item.Value < minNum)
				{
					minNum = item.Value;
				}
			}
			List<int> ValidPropList = new List<int>();
			for (int i = 0; i < 9; i++)
			{
				int curNum = 0;
				AlchemyValue.TryGetValue(i, out curNum);
				if (curNum - minNum < this.RandomLimit)
				{
					ValidPropList.Add(i);
				}
			}
			if (ValidPropList.Count != 0)
			{
				prop = ValidPropList[Global.GetRandomNumber(0, ValidPropList.Count)];
			}
			return prop;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x000171F0 File Offset: 0x000153F0
		private void RefreshAlchemyProps(GameClient client)
		{
			List<double> tempAlchemyPropList = null;
			lock (this.ConfigMutex)
			{
				tempAlchemyPropList = this.AlchemyPropList;
			}
			double[] _ExtProps = new double[177];
			foreach (KeyValuePair<int, int> item in client.ClientData.AlchemyInfo.BaseData.AlchemyValue)
			{
				switch (item.Key)
				{
				case 0:
					_ExtProps[13] += (double)item.Value * tempAlchemyPropList[item.Key];
					break;
				case 1:
					_ExtProps[7] += (double)item.Value * tempAlchemyPropList[item.Key];
					_ExtProps[8] += (double)item.Value * tempAlchemyPropList[item.Key];
					_ExtProps[9] += (double)item.Value * tempAlchemyPropList[item.Key];
					_ExtProps[10] += (double)item.Value * tempAlchemyPropList[item.Key];
					break;
				case 2:
					_ExtProps[3] += (double)item.Value * tempAlchemyPropList[item.Key];
					_ExtProps[4] += (double)item.Value * tempAlchemyPropList[item.Key];
					_ExtProps[5] += (double)item.Value * tempAlchemyPropList[item.Key];
					_ExtProps[6] += (double)item.Value * tempAlchemyPropList[item.Key];
					break;
				case 3:
					_ExtProps[18] += (double)item.Value * tempAlchemyPropList[item.Key];
					break;
				case 4:
					_ExtProps[19] += (double)item.Value * tempAlchemyPropList[item.Key];
					break;
				case 5:
					_ExtProps[27] += (double)item.Value * tempAlchemyPropList[item.Key];
					break;
				case 6:
					_ExtProps[38] += (double)item.Value * tempAlchemyPropList[item.Key];
					break;
				case 7:
					_ExtProps[44] += (double)item.Value * tempAlchemyPropList[item.Key];
					break;
				case 8:
					_ExtProps[30] += (double)item.Value * tempAlchemyPropList[item.Key];
					break;
				}
			}
			client.ClientData.PropsCacheManager.SetExtProps(new object[]
			{
				PropsSystemTypes.Alchemy,
				_ExtProps
			});
			client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
			{
				DelayExecProcIds.RecalcProps,
				DelayExecProcIds.NotifyRefreshProps
			});
		}

		// Token: 0x0600015F RID: 351 RVA: 0x000175E0 File Offset: 0x000157E0
		public bool ProcessAlchemyDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Convert.ToInt32(cmdParams[0]);
				int curDayID = Global.GetOffsetDay(TimeUtil.NowDateTime());
				if (curDayID > client.ClientData.AlchemyInfo.ElementDayID)
				{
					client.ClientData.AlchemyInfo.BaseData.ToDayCost.Clear();
					client.ClientData.AlchemyInfo.ElementDayID = curDayID;
				}
				byte[] bytesData = DataHelper.ObjectToBytes<AlchemyData>(client.ClientData.AlchemyInfo.BaseData);
				GameManager.ClientMgr.SendToClient(client, bytesData, nID);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000160 RID: 352 RVA: 0x000176B0 File Offset: 0x000158B0
		public bool ProcessAlchemyAddElementCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Convert.ToInt32(cmdParams[0]);
				int costType = Convert.ToInt32(cmdParams[1]);
				int useNum = Convert.ToInt32(cmdParams[2]);
				bool bindOnly = true;
				if (cmdParams.Length >= 4)
				{
					bindOnly = (Convert.ToInt32(cmdParams[3]) > 0);
				}
				Dictionary<int, AlchemyConfigData> tempAlchemyConfig = null;
				lock (this.ConfigMutex)
				{
					tempAlchemyConfig = this.AlchemyConfig;
				}
				AlchemyConfigData alchemyConfig = null;
				if (!tempAlchemyConfig.TryGetValue(costType, out alchemyConfig))
				{
					result = -3;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						0,
						costType,
						0
					}), false);
					return true;
				}
				if (!this.CheckCostEnough(client, costType, useNum, bindOnly) || useNum < alchemyConfig.Unit)
				{
					if (costType < this.MinGoodsID)
					{
						result = -12;
					}
					else
					{
						result = -6;
					}
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						0,
						costType,
						0
					}), false);
					return true;
				}
				int todayCost = this.GetTodayAddElementCost(client, costType);
				if (alchemyConfig.Limit != -1 && todayCost + useNum > alchemyConfig.Limit)
				{
					result = -36;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						0,
						costType,
						todayCost
					}), false);
					return true;
				}
				useNum -= useNum % alchemyConfig.Unit;
				if (!this.ModifyAddElementCost(client, costType, -useNum, bindOnly))
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						result,
						roleID,
						0,
						costType,
						todayCost
					}), false);
					return true;
				}
				GameManager.ClientMgr.ModifyAlchemyElementValue(client, useNum / alchemyConfig.Unit * alchemyConfig.Element, "灌注", false, false);
				this.UpdateTodayAddElementCost(client, costType, useNum);
				this.UpdateHistAddElementCost(client, costType, useNum);
				this.UpdateAlchemyDataDB(client);
				todayCost = this.GetTodayAddElementCost(client, costType);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					result,
					roleID,
					client.ClientData.AlchemyInfo.BaseData.Element,
					costType,
					todayCost
				}), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00017A4C File Offset: 0x00015C4C
		public bool ProcessAlchemyExcuteCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Convert.ToInt32(cmdParams[0]);
				int alchemyType = Convert.ToInt32(cmdParams[1]);
				if (alchemyType != 0 && alchemyType != 1)
				{
					return true;
				}
				string alchemyProp = "";
				if (alchemyType == 0)
				{
					if (client.ClientData.AlchemyInfo.BaseData.Element < this.LevelUpElement)
					{
						result = -12;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							roleID,
							client.ClientData.AlchemyInfo.BaseData.Element,
							alchemyProp
						}), false);
						return true;
					}
					int prop = this.RandomAlchemyProp(client);
					alchemyProp = string.Format("{0}", prop);
					int propNum = 0;
					client.ClientData.AlchemyInfo.BaseData.AlchemyValue.TryGetValue(prop, out propNum);
					propNum = (client.ClientData.AlchemyInfo.BaseData.AlchemyValue[prop] = propNum + 1);
					GameManager.ClientMgr.ModifyAlchemyElementValue(client, -this.LevelUpElement, "炼金", false, false);
				}
				else
				{
					if (client.ClientData.AlchemyInfo.BaseData.Element < this.LevelUpElement * 10)
					{
						result = -12;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							roleID,
							client.ClientData.AlchemyInfo.BaseData.Element,
							alchemyProp
						}), false);
						return true;
					}
					for (int loop = 0; loop < 10; loop++)
					{
						int prop = this.RandomAlchemyProp(client);
						alchemyProp += string.Format("{0}|", prop);
						int propNum = 0;
						client.ClientData.AlchemyInfo.BaseData.AlchemyValue.TryGetValue(prop, out propNum);
						propNum = (client.ClientData.AlchemyInfo.BaseData.AlchemyValue[prop] = propNum + 1);
					}
					GameManager.ClientMgr.ModifyAlchemyElementValue(client, -this.LevelUpElement * 10, "炼金", false, false);
				}
				this.RefreshAlchemyProps(client);
				this.UpdateAlchemyDataDB(client);
				if (!string.IsNullOrEmpty(alchemyProp) && alchemyProp.Substring(alchemyProp.Length - 1) == "|")
				{
					alchemyProp = alchemyProp.Substring(0, alchemyProp.Length - 1);
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					result,
					roleID,
					client.ClientData.AlchemyInfo.BaseData.Element,
					alchemyProp
				}), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00017DB8 File Offset: 0x00015FB8
		public bool InitConfig()
		{
			string AlchemyLevelUp = GameManager.systemParamsList.GetParamValueByName("AlchemyLevelUp");
			if (!string.IsNullOrEmpty(AlchemyLevelUp))
			{
				this.LevelUpElement = Global.SafeConvertToInt32(AlchemyLevelUp);
			}
			List<double> tempAlchemyPropList = new List<double>();
			string AlchemyProperty = GameManager.systemParamsList.GetParamValueByName("AlchemyProperty");
			if (!string.IsNullOrEmpty(AlchemyProperty))
			{
				string[] kvpFields = AlchemyProperty.Split(new char[]
				{
					','
				});
				foreach (string item in kvpFields)
				{
					tempAlchemyPropList.Add(Global.SafeConvertToDouble(item));
				}
			}
			lock (this.ConfigMutex)
			{
				this.AlchemyPropList = tempAlchemyPropList;
			}
			string AlchemyRandomLimit = GameManager.systemParamsList.GetParamValueByName("AlchemyRandomLimit");
			if (!string.IsNullOrEmpty(AlchemyRandomLimit))
			{
				this.RandomLimit = Global.SafeConvertToInt32(AlchemyRandomLimit);
			}
			return this.LoadAlchemyConfigFile();
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00017EE4 File Offset: 0x000160E4
		public bool LoadAlchemyConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/CurrencyConversion.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/CurrencyConversion.xml"));
				if (null == xml)
				{
					return false;
				}
				Dictionary<int, AlchemyConfigData> tempAlchemyConfig = new Dictionary<int, AlchemyConfigData>();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					AlchemyConfigData data = new AlchemyConfigData();
					data.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					data.TypeID = (int)Global.GetSafeAttributeLong(xmlItem, "Type");
					data.Unit = (int)Global.GetSafeAttributeLong(xmlItem, "Unit");
					data.Element = (int)Global.GetSafeAttributeLong(xmlItem, "Element");
					data.Limit = (int)Global.GetSafeAttributeLong(xmlItem, "Limit");
					tempAlchemyConfig[data.TypeID] = data;
				}
				lock (this.ConfigMutex)
				{
					this.AlchemyConfig = tempAlchemyConfig;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/CurrencyConversion.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x04000221 RID: 545
		private const string Alchemy_FileName = "Config/CurrencyConversion.xml";

		// Token: 0x04000222 RID: 546
		private const string Alchemy_SystemParamKey_Level = "AlchemyLevelUp";

		// Token: 0x04000223 RID: 547
		private const string Alchemy_SystemParamKey_Prop = "AlchemyProperty";

		// Token: 0x04000224 RID: 548
		private const string Alchemy_SystemParamKey_Limit = "AlchemyRandomLimit";

		// Token: 0x04000225 RID: 549
		private object ConfigMutex = new object();

		// Token: 0x04000226 RID: 550
		private int LevelUpElement = 0;

		// Token: 0x04000227 RID: 551
		private List<double> AlchemyPropList = null;

		// Token: 0x04000228 RID: 552
		private int RandomLimit = 0;

		// Token: 0x04000229 RID: 553
		private int MinGoodsID = 100;

		// Token: 0x0400022A RID: 554
		private Dictionary<int, AlchemyConfigData> AlchemyConfig = new Dictionary<int, AlchemyConfigData>();

		// Token: 0x0400022B RID: 555
		private static AlchemyManager instance = new AlchemyManager();
	}
}
