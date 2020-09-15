using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020001EB RID: 491
	public class HuiJiManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		// Token: 0x06000624 RID: 1572 RVA: 0x00056298 File Offset: 0x00054498
		public static HuiJiManager getInstance()
		{
			return HuiJiManager.instance;
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x000562B0 File Offset: 0x000544B0
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x000562D4 File Offset: 0x000544D4
		public void processEvent(EventObject eventObject)
		{
			int nID = eventObject.getEventType();
			int num = nID;
			if (num == 10)
			{
				PlayerDeadEventObject playerDeadEventObject = eventObject as PlayerDeadEventObject;
				if (playerDeadEventObject != null && null != playerDeadEventObject.getPlayer())
				{
					this.OnRoleDead(playerDeadEventObject.getPlayer());
				}
			}
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x00056320 File Offset: 0x00054520
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.IsGongNengOpend = false;
					this.RuntimeData.EmblemFull = GameManager.systemParamsList.GetParamValueIntArrayByName("EmblemFull", ',');
					this.RuntimeData.EmblemShengXing = GameManager.systemParamsList.GetParamValueDoubleArrayByName("EmblemShengXing", ',');
					int platformId = (int)GameCoreInterface.getinstance().GetPlatformType();
					List<string> emblemOpenStrs = GameManager.systemParamsList.GetParamValueStringListByName("EmblemOpen", '|');
					foreach (string str in emblemOpenStrs)
					{
						List<int> args = Global.StringToIntList(str, ',');
						if (args != null && args[0] == platformId && args[1] > 0)
						{
							this.RuntimeData.IsGongNengOpend = true;
						}
					}
					this.RuntimeData.IsGongNengOpend &= !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot5);
					fileName = "Config/EmblemUp.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					this.RuntimeData.EmblemUpDict.Load(fullPathFileName, null);
					foreach (EmblemUpInfo info in this.RuntimeData.EmblemUpDict.Value.Values)
					{
						info.ExtPropTempValues[24] = info.SubAttackInjurePercent;
						info.ExtPropTempValues[102] = info.SPAttackInjurePercent;
						info.ExtPropTempValues[103] = info.AttackInjurePercent;
						info.ExtPropTempValues[104] = info.ElementAttackInjurePercent;
					}
					fileName = "Config/EmblemStar.xml";
					fullPathFileName = Global.GameResPath(fileName);
					this.RuntimeData.EmblemStarDict.Load(fullPathFileName, null);
					foreach (EmblemStarInfo starInfo in this.RuntimeData.EmblemStarDict.Value.Values)
					{
						EmblemUpInfo emblemUpInfo;
						if (this.RuntimeData.EmblemUpDict.Value.TryGetValue(starInfo.EmblemLevel, out emblemUpInfo))
						{
							starInfo.EmblemUpInfo = emblemUpInfo;
							emblemUpInfo.MaxStarLevel = Math.Max(emblemUpInfo.MaxStarLevel, starInfo.EmblemStar);
							starInfo.LifeV += emblemUpInfo.LifeV;
							starInfo.AddAttack += emblemUpInfo.AddAttack;
							starInfo.AddDefense += emblemUpInfo.AddDefense;
							starInfo.DecreaseInjureValue += emblemUpInfo.DecreaseInjureValue;
							starInfo.ExtPropValues[13] = (double)starInfo.LifeV;
							starInfo.ExtPropValues[45] = (double)starInfo.AddAttack;
							starInfo.ExtPropValues[46] = (double)starInfo.AddDefense;
							starInfo.ExtPropValues[38] = (double)starInfo.DecreaseInjureValue;
						}
					}
				}
				catch (Exception ex)
				{
					success = false;
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x00056718 File Offset: 0x00054918
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1445, 1, 1, HuiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1446, 1, 1, HuiJiManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			GlobalEventSource.getInstance().registerListener(10, HuiJiManager.getInstance());
			return true;
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x00056770 File Offset: 0x00054970
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(10, HuiJiManager.getInstance());
			return true;
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x00056798 File Offset: 0x00054998
		public bool destroy()
		{
			return true;
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x000567AC File Offset: 0x000549AC
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x000567C0 File Offset: 0x000549C0
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1445:
				result = this.ProcessExecuteHuiJiHuTiCmd(client, nID, bytes, cmdParams);
				break;
			case 1446:
				result = this.ProcessHuiJiStarUpCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x00056808 File Offset: 0x00054A08
		private bool IsGongNengOpened(GameClient client)
		{
			return this.RuntimeData.IsGongNengOpend && GlobalNew.IsGongNengOpened(client, GongNengIDs.HuiJiHuTi, false);
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x00056844 File Offset: 0x00054A44
		private bool ProcessHuiJiStarUpCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int result = 0;
			HuiJiUpdateResultData resultData = new HuiJiUpdateResultData();
			RoleHuiJiData huiJiData = client.ClientData.HuiJiData;
			int exp = 0;
			HuiJiUpdateResultData requestData = DataHelper.BytesToObject<HuiJiUpdateResultData>(bytes, 0, bytes.Length);
			int type = requestData.Type;
			int zuanshi = requestData.ZuanShi;
			int auto = requestData.Auto;
			long nowTicks = TimeUtil.NOW();
			if (!this.IsGongNengOpened(client))
			{
				result = -12;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					EmblemStarInfo starInfo;
					EmblemStarInfo starInfo2;
					if (huiJiData.huiji != requestData.HuiJi)
					{
						result = -3;
					}
					else if (!this.RuntimeData.EmblemStarDict.Value.TryGetValue(huiJiData.huiji, out starInfo))
					{
						result = -3;
					}
					else if (!this.RuntimeData.EmblemStarDict.Value.TryGetValue(huiJiData.huiji + 1, out starInfo2))
					{
						result = -4004;
					}
					else
					{
						bool useBind = false;
						bool useTimeLimit = false;
						string strCostList;
						if (type == 0)
						{
							if (starInfo.EmblemUpInfo.MaxStarLevel == starInfo.EmblemStar)
							{
								result = -4;
								goto IL_646;
							}
							if (Global.UseGoodsBindOrNot(client, starInfo.NeedGoods[0], starInfo.NeedGoods[1], true, out useBind, out useTimeLimit) < 0)
							{
								if (zuanshi <= 0 || zuanshi != starInfo.NeedDiamond)
								{
									result = -6;
									goto IL_646;
								}
								if (!GameManager.ClientMgr.SubUserMoney(client, zuanshi, "徽记升星", true, true, true, true, DaiBiSySType.HuiJiShengXing))
								{
									result = -10;
									goto IL_646;
								}
								exp = starInfo.ZuanShiExp;
								strCostList = EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
								{
									-zuanshi,
									client.ClientData.UserMoney + zuanshi,
									client.ClientData.UserMoney
								});
								if (Global.GetRandom() < this.RuntimeData.EmblemShengXing[1])
								{
									exp = (int)((double)exp * this.RuntimeData.EmblemShengXing[2]);
								}
							}
							else
							{
								exp = starInfo.GoodsExp;
								strCostList = EventLogManager.NewGoodsDataPropString(new GoodsData
								{
									GoodsID = starInfo.NeedGoods[0],
									GCount = starInfo.NeedGoods[1]
								});
								if (Global.GetRandom() < this.RuntimeData.EmblemShengXing[0])
								{
									exp = (int)((double)exp * this.RuntimeData.EmblemShengXing[2]);
								}
							}
							huiJiData.Exp += exp;
							if (huiJiData.Exp >= starInfo.StarExp)
							{
								huiJiData.huiji++;
								if (starInfo.EmblemStar < starInfo.EmblemUpInfo.MaxStarLevel - 1)
								{
									huiJiData.Exp -= starInfo.StarExp;
								}
								else
								{
									huiJiData.Exp = 0;
								}
							}
						}
						else
						{
							if (starInfo.EmblemUpInfo.MaxStarLevel != starInfo.EmblemStar)
							{
								result = -4;
								goto IL_646;
							}
							if (Global.UseGoodsBindOrNot(client, starInfo.EmblemUpInfo.NeedGoods[0], starInfo.EmblemUpInfo.NeedGoods[1], true, out useBind, out useTimeLimit) < 0)
							{
								if (zuanshi <= 0 || zuanshi != starInfo.EmblemUpInfo.NeedDiamond)
								{
									result = -6;
									goto IL_646;
								}
								if (!GameManager.ClientMgr.SubUserMoney(client, zuanshi, "徽记升阶", true, true, true, true, DaiBiSySType.HuiJiShengJie))
								{
									result = -10;
									goto IL_646;
								}
								strCostList = EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
								{
									-zuanshi,
									client.ClientData.UserMoney + zuanshi,
									client.ClientData.UserMoney
								});
							}
							else
							{
								strCostList = EventLogManager.NewGoodsDataPropString(new GoodsData
								{
									GoodsID = starInfo.EmblemUpInfo.NeedGoods[0],
									GCount = starInfo.EmblemUpInfo.NeedGoods[1]
								});
								exp = starInfo.GoodsExp;
							}
							huiJiData.Exp++;
							if (starInfo.EmblemUpInfo.LuckyOne + huiJiData.Exp >= 110000)
							{
								huiJiData.huiji++;
								huiJiData.Exp = 0;
							}
							else if (starInfo.EmblemUpInfo.LuckyOne + huiJiData.Exp > starInfo.EmblemUpInfo.LuckyTwo)
							{
								if (Global.GetRandom() < starInfo.EmblemUpInfo.LuckyTwoRate)
								{
									huiJiData.huiji++;
									huiJiData.Exp = 0;
								}
							}
						}
						Global.SendToDB<RoleDataCmdT<RoleHuiJiData>>(1446, new RoleDataCmdT<RoleHuiJiData>(client.ClientData.RoleID, huiJiData), client.ServerId);
						if (huiJiData.huiji > requestData.HuiJi)
						{
							client.ClientData.PropsCacheManager.SetExtProps(new object[]
							{
								PropsSystemTypes.HuiJiHuTi,
								starInfo2.ExtPropValues
							});
							client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
							{
								DelayExecProcIds.RecalcProps,
								DelayExecProcIds.NotifyRefreshProps
							});
							EventLogManager.AddHuiJiEvent(client, type, (zuanshi > 0) ? 1 : 0, exp, starInfo.EmblemLevel, starInfo.EmblemStar, starInfo2.EmblemLevel, starInfo2.EmblemStar, huiJiData.Exp, strCostList);
						}
						else
						{
							EventLogManager.AddHuiJiEvent(client, type, (zuanshi > 0) ? 1 : 0, exp, starInfo.EmblemLevel, starInfo.EmblemStar, starInfo.EmblemLevel, starInfo.EmblemStar, huiJiData.Exp, strCostList);
						}
					}
				}
			}
			IL_646:
			if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieRiHuiJi))
			{
				client._IconStateMgr.SendIconStateToClient(client);
			}
			resultData.Result = result;
			resultData.HuiJi = huiJiData.huiji;
			resultData.Exp = huiJiData.Exp;
			resultData.Auto = auto;
			resultData.Type = type;
			client.sendCmd<HuiJiUpdateResultData>(nID, resultData, false);
			return true;
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x00056F18 File Offset: 0x00055118
		private bool ProcessExecuteHuiJiHuTiCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int result = 0;
			long nowTicks = TimeUtil.NOW();
			ExtData extData = ExtDataManager.GetClientExtData(client);
			if (nowTicks < extData.HuiJiCDTicks)
			{
				result = -2007;
			}
			else if (!this.IsGongNengOpened(client))
			{
				result = -12;
			}
			else
			{
				long cdTime = 0L;
				int durationTime = 0;
				double[] props = null;
				lock (this.RuntimeData.Mutex)
				{
					EmblemStarInfo starInfo;
					if (!this.RuntimeData.EmblemStarDict.Value.TryGetValue(client.ClientData.HuiJiData.huiji, out starInfo))
					{
						result = -20;
						goto IL_150;
					}
					EmblemUpInfo upInfo = starInfo.EmblemUpInfo;
					if (null == upInfo)
					{
						result = -20;
						goto IL_150;
					}
					cdTime = (long)upInfo.CDTime;
					durationTime = upInfo.DurationTime;
					props = upInfo.ExtPropTempValues;
				}
				extData.HuiJiCDTicks = nowTicks + (long)durationTime + cdTime;
				extData.HuiJiCdTime = cdTime;
				client.buffManager.SetStatusBuff(116, nowTicks, (long)durationTime, 0L);
				this.OnHuiJiStateChange(client, true, client.ClientData.HuiJiData.huiji, durationTime, props);
				Global.RemoveBufferData(client, 119);
				ZuoQiManager.getInstance().RoleDisMount(client, true);
			}
			IL_150:
			client.sendCmd<int>(nID, result, false);
			return true;
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x00057098 File Offset: 0x00055298
		public void OnRoleDead(GameClient client)
		{
			ExtData extData = ExtDataManager.GetClientExtData(client);
			long maxCdTicks = TimeUtil.NOW() + extData.HuiJiCdTime;
			if (maxCdTicks < extData.HuiJiCDTicks)
			{
				extData.HuiJiCDTicks = maxCdTicks;
				client.buffManager.SetStatusBuff(116, 0L, 0L, 0L);
				double moveCost = RoleAlgorithm.GetMoveSpeed(client);
				client.ClientData.MoveSpeed = moveCost;
				GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 11, 0L, 0, moveCost);
			}
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x00057124 File Offset: 0x00055324
		public void OnInitGame(GameClient client)
		{
			if (client.ClientData.HuiJiData == null)
			{
				client.ClientData.HuiJiData = new RoleHuiJiData();
			}
			bool resetCD = false;
			int[] array = this.RuntimeData.EmblemFull;
			if (array != null && array[0] > 0)
			{
				for (int i = 1; i < array.Length; i++)
				{
					if (array[i] == client.ClientData.MapCode)
					{
						resetCD = true;
						break;
					}
				}
			}
			ExtData extData = ExtDataManager.GetClientExtData(client);
			this.InitDataByTask(client);
			lock (this.RuntimeData.Mutex)
			{
				EmblemStarInfo starInfo;
				if (this.RuntimeData.EmblemStarDict.Value.TryGetValue(client.ClientData.HuiJiData.huiji, out starInfo) && starInfo.EmblemUpInfo != null)
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.HuiJiHuTi,
						starInfo.ExtPropValues
					});
					if (resetCD)
					{
						extData.HuiJiCDTicks = 0L;
					}
					else
					{
						extData.HuiJiCDTicks = TimeUtil.NOW() + (long)starInfo.EmblemUpInfo.CDTime;
					}
					extData.HuiJiCdTime = (long)starInfo.EmblemUpInfo.CDTime;
				}
			}
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x000572BC File Offset: 0x000554BC
		public void InitDataByTask(GameClient client)
		{
			if (client.ClientData.HuiJiData.huiji <= 0)
			{
				if (this.IsGongNengOpened(client))
				{
					client.ClientData.HuiJiData.huiji = 1;
				}
			}
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x00057304 File Offset: 0x00055504
		public void OnHuiJiStateChange(GameClient client, bool active, int level = 0, int keepTicks = 0, double[] props = null)
		{
			if (active)
			{
				if (null != props)
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						31,
						props
					});
				}
				client.ClientData.DongJieStart = 0L;
				client.ClientData.DongJieSeconds = 0;
				client.RoleBuffer.SetTempExtProp(47, 0.0, 0L);
				client.RoleBuffer.SetTempExtProp(2, 0.0, 0L);
				client.RoleBuffer.SetTempExtProp(18, 0.0, 0L);
				double moveCost = RoleAlgorithm.GetMoveSpeed(client);
				client.ClientData.MoveSpeed = moveCost;
				double[] actionParams = new double[]
				{
					(double)level,
					(double)keepTicks
				};
				Global.UpdateBufferData(client, BufferItemTypes.HuiJiHuTi, actionParams, 1, true);
				GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 11, TimeUtil.NOW(), keepTicks, moveCost);
			}
			else
			{
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					31,
					PropsCacheManager.ConstExtProps
				});
				double moveCost = RoleAlgorithm.GetMoveSpeed(client);
				client.ClientData.MoveSpeed = moveCost;
				double[] array = new double[2];
				array[0] = (double)level;
				double[] actionParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.HuiJiHuTi, actionParams, 1, false);
				GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 11, TimeUtil.NOW(), keepTicks, moveCost);
			}
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x000574A8 File Offset: 0x000556A8
		public EmblemStarInfo GetHuiJiStartInfo(GameClient client)
		{
			EmblemStarInfo starInfo;
			lock (this.RuntimeData.Mutex)
			{
				RoleHuiJiData huiJiData = client.ClientData.HuiJiData;
				this.RuntimeData.EmblemStarDict.Value.TryGetValue(huiJiData.huiji, out starInfo);
			}
			return starInfo;
		}

		// Token: 0x04000AD4 RID: 2772
		private static HuiJiManager instance = new HuiJiManager();

		// Token: 0x04000AD5 RID: 2773
		private HuiJiManagerData RuntimeData = new HuiJiManagerData();
	}
}
