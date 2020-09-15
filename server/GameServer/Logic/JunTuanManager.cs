using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000318 RID: 792
	public class JunTuanManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		// Token: 0x06000CC1 RID: 3265 RVA: 0x000C6038 File Offset: 0x000C4238
		public static JunTuanManager getInstance()
		{
			return JunTuanManager.instance;
		}

		// Token: 0x06000CC2 RID: 3266 RVA: 0x000C6050 File Offset: 0x000C4250
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x06000CC3 RID: 3267 RVA: 0x000C6074 File Offset: 0x000C4274
		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("JunTuanManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 1800);
			return true;
		}

		// Token: 0x06000CC4 RID: 3268 RVA: 0x000C60B4 File Offset: 0x000C42B4
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1230, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1231, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1232, 2, 2, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1233, 2, 2, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1234, 3, 3, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1235, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1236, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1237, 2, 2, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1238, 2, 2, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1239, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1224, 2, 2, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1221, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1223, 2, 2, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1220, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1226, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1227, 2, 2, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1228, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(10, JunTuanManager.getInstance());
			GameManager.GameConfigMgr.UpdateGameConfigItem("juntuanbanghuimax", this.RuntimeData.LegionsNeed.ToString(), false);
			return true;
		}

		// Token: 0x06000CC5 RID: 3269 RVA: 0x000C6294 File Offset: 0x000C4494
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(10, JunTuanManager.getInstance());
			return true;
		}

		// Token: 0x06000CC6 RID: 3270 RVA: 0x000C62BC File Offset: 0x000C44BC
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000CC7 RID: 3271 RVA: 0x000C62D0 File Offset: 0x000C44D0
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x06000CC8 RID: 3272 RVA: 0x000C62E4 File Offset: 0x000C44E4
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 1220:
				return this.ProcessGetJunTuanRankListCmd(client, nID, bytes, cmdParams);
			case 1221:
				return this.ProcessGetJunTuanTaskListCmd(client, nID, bytes, cmdParams);
			case 1223:
				return this.ProcessGetJunTuanTaskAwardCmd(client, nID, bytes, cmdParams);
			case 1224:
				return this.ProcessJunTuanCreateCmd(client, nID, bytes, cmdParams);
			case 1226:
				return this.ProcessGetJunTuanRoleListCmd(client, nID, bytes, cmdParams);
			case 1227:
				return this.ProcessQuitJunTuanCmd(client, nID, bytes, cmdParams);
			case 1228:
				return this.ProcessDestroyJunTuanCmd(client, nID, bytes, cmdParams);
			case 1230:
				return this.ProcessGetJunTuanListCmd(client, nID, bytes, cmdParams);
			case 1231:
				return this.ProcessGetJunTuanDataCmd(client, nID, bytes, cmdParams);
			case 1232:
				return this.ProcessGetJunTuanBangHuiListCmd(client, nID, bytes, cmdParams);
			case 1233:
				return this.ProcessJunTuanJoinCmd(client, nID, bytes, cmdParams);
			case 1234:
				return this.ProcessJunTuanJoinResponseCmd(client, nID, bytes, cmdParams);
			case 1235:
				return this.ProcessGetJunTuanRequestListCmd(client, nID, bytes, cmdParams);
			case 1236:
				return this.ProcessGetJunTuanLogListCmd(client, nID, bytes, cmdParams);
			case 1237:
				return this.ProcessJunTuanBulltinCmd(client, nID, bytes, cmdParams);
			case 1238:
				return this.ProcessJunTuanBangHuiZhiWuCmd(client, nID, bytes, cmdParams);
			case 1239:
				return this.ProcessJunTuanRoleZhiWuCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		// Token: 0x06000CC9 RID: 3273 RVA: 0x000C6460 File Offset: 0x000C4660
		public void processEvent(EventObject eventObject)
		{
			int nID = eventObject.getEventType();
			int num = nID;
			if (num != 10)
			{
				if (num == 14)
				{
					PlayerInitGameEventObject playerInitGameEventObject = eventObject as PlayerInitGameEventObject;
					if (null != playerInitGameEventObject)
					{
						this.OnInitGame(playerInitGameEventObject.getPlayer());
					}
				}
			}
			else
			{
				PlayerDeadEventObject playerDeadEventObject = eventObject as PlayerDeadEventObject;
				if (playerDeadEventObject != null && null != playerDeadEventObject.getAttackerRole())
				{
					this.AddJunTuanTaskValue(playerDeadEventObject.getAttackerRole(), 4, 1);
				}
			}
		}

		// Token: 0x06000CCA RID: 3274 RVA: 0x000C64D8 File Offset: 0x000C46D8
		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num == 10001)
			{
				KuaFuNotifyEnterGameEvent e = eventObject as KuaFuNotifyEnterGameEvent;
				if (null != e)
				{
					KuaFuServerLoginData kuaFuServerLoginData = e.Arg as KuaFuServerLoginData;
					if (null != kuaFuServerLoginData)
					{
						lock (this.RuntimeData.Mutex)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("通知角色ID={0}拥有进入勇者战场资格,跨服GameID={1}", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
						}
					}
					eventObject.Handled = true;
				}
			}
		}

		// Token: 0x06000CCB RID: 3275 RVA: 0x000C65A0 File Offset: 0x000C47A0
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.LegionsNeed = (int)GameManager.systemParamsList.GetParamValueIntByName("LegionsNeed", -1);
					this.RuntimeData.LegionsCreateCD = (int)GameManager.systemParamsList.GetParamValueIntByName("LegionsCreateCD", -1);
					this.RuntimeData.LegionsCastZuanShi = (int)GameManager.systemParamsList.GetParamValueIntByName("LegionsCastZuanShi", -1);
					this.RuntimeData.LegionsJionCD = (int)GameManager.systemParamsList.GetParamValueIntByName("LegionsJionCD", -1);
					this.RuntimeData.LegionsEliteNum = (int)GameManager.systemParamsList.GetParamValueIntByName("LegionsEliteNum", -1);
					this.RuntimeData.LegionProsperityCost = GameManager.systemParamsList.GetParamValueIntArrayByName("LegionProsperityCost", ',');
					string timeStr = GameManager.systemParamsList.GetParamValueByName("LegionTasksTime");
					if (!ConfigHelper.ParserTimeRangeListWithDay2(this.RuntimeData.TaskStartEndTimeList, timeStr, true, '|', '-', ',') || this.RuntimeData.TaskStartEndTimeList.Count != 2)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("解析systemparams.xml的LegionTasksTime出错", fileName), null, true);
					}
					fileName = "Config/LegionsManager.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					this.RuntimeData.RolePermissionDict.Load(fullPathFileName, null);
					fileName = "Config/LegionTasks.xml";
					fullPathFileName = Global.GameResPath(fileName);
					this.RuntimeData.TaskList.Load(fullPathFileName, null);
					this.RuntimeData.TaskCount = 0;
					this.RuntimeData.Task2IdxDict.Clear();
					this.RuntimeData.KillMonsterIds.Clear();
					foreach (KeyValuePair<int, JunTuanTaskInfo> kv in this.RuntimeData.TaskList.Value)
					{
						this.RuntimeData.Task2IdxDict[kv.Key] = this.RuntimeData.TaskCount++;
						if (kv.Value.CompleteType == 1)
						{
							foreach (int id in kv.Value.TypeID)
							{
								this.RuntimeData.KillMonsterIds.Add(id);
							}
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

		// Token: 0x06000CCC RID: 3276 RVA: 0x000C68E4 File Offset: 0x000C4AE4
		public bool ProcessGetJunTuanListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<JunTuanMiniData> list = null;
				if (this.IsGongNengOpened(client, false))
				{
					int bhid = client.ClientData.Faction;
					list = JunTuanClient.getInstance().GetJunTuanList(bhid);
				}
				client.sendCmd<List<JunTuanMiniData>>(nID, list, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000CCD RID: 3277 RVA: 0x000C6964 File Offset: 0x000C4B64
		public bool ProcessGetJunTuanDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				JunTuanData data = null;
				if (this.IsGongNengOpened(client, false))
				{
					int bhid = client.ClientData.Faction;
					int junTuanId = client.ClientData.JunTuanId;
					if (bhid > 0 && junTuanId > 0)
					{
						data = JunTuanClient.getInstance().GetJunTuanData(bhid, junTuanId, true);
						bool icon = data != null && client.ClientData.JunTuanZhiWu == 1 && data.RequestCount > 0;
						if (client._IconStateMgr.AddFlushIconState(15005, icon))
						{
							client._IconStateMgr.SendIconStateToClient(client);
						}
					}
				}
				client.sendCmd<JunTuanData>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000CCE RID: 3278 RVA: 0x000C6A54 File Offset: 0x000C4C54
		public bool ProcessJunTuanCreateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string junTuanName = cmdParams[1];
				int result = NameServerNamager.CheckInvalidCharacters(junTuanName, false);
				if (result < 0)
				{
					result = -1027;
				}
				else
				{
					int bhid = client.ClientData.Faction;
					if (bhid <= 0 || client.ClientData.BHZhiWu != 1)
					{
						result = -1002;
					}
					else if (client.ClientData.JunTuanId != 0)
					{
						result = -1020;
					}
					else if (!this.IsGongNengOpened(client, false))
					{
						result = -12;
					}
					else if (client.ClientData.UserMoney < this.RuntimeData.LegionsCastZuanShi)
					{
						result = -10;
					}
					else
					{
						BangHuiDetailData bhData = Global.GetBangHuiDetailData(client.ClientData.RoleID, bhid, client.ServerId);
						if (null == bhData)
						{
							result = -15;
						}
						else if (bhData.QiLevel < this.RuntimeData.LegionsNeed)
						{
							result = -1008;
						}
						else
						{
							JunTuanRequestData data = new JunTuanRequestData();
							data.BhId = bhid;
							data.BhName = bhData.BHName;
							data.BhZoneId = bhData.ZoneID;
							data.LeaderRoleId = client.ClientData.RoleID;
							data.LeaderName = client.ClientData.RoleName;
							data.LeaderZoneId = client.ClientData.ZoneID;
							data.ZhanLi = bhData.TotalCombatForce;
							data.RoleNum = bhData.TotalNum;
							data.JunTuanName = junTuanName;
							data.Occupation = client.ClientData.Occupation;
							result = JunTuanClient.getInstance().CreateJunTuan(data);
							if (result >= 0)
							{
								int juntuanId = result;
								result = this.UpdateJunTuanRoleList(bhid, client.ServerId);
								if (result >= 0)
								{
									int oldUserMoney = client.ClientData.UserMoney;
									if (!GameManager.ClientMgr.SubUserMoney(client, this.RuntimeData.LegionsCastZuanShi, "创建军团", true, true, true, true, DaiBiSySType.None))
									{
										result = -10;
										JunTuanClient.getInstance().DestroyJunTuan(bhid, juntuanId);
									}
									else
									{
										string strCostList = EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
										{
											-this.RuntimeData.LegionsCastZuanShi,
											oldUserMoney,
											client.ClientData.UserMoney
										});
										EventLogManager.AddJunTuanCreateEvent(client, juntuanId, bhid, strCostList);
										JunTuanBangHuiMiniData changeData = new JunTuanBangHuiMiniData
										{
											BhId = bhid,
											JunTuanId = juntuanId,
											JunTuanName = junTuanName,
											JunTuanZhiWu = 1
										};
										this.UpdateBhJunTuan(changeData);
										result = juntuanId;
									}
								}
							}
						}
					}
				}
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000CCF RID: 3279 RVA: 0x000C6D78 File Offset: 0x000C4F78
		public bool ProcessJunTuanBulltinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string bulltin = cmdParams[1];
				int bhid = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				int result;
				if (bhid <= 0 || client.ClientData.BHZhiWu != 1 || junTuanId <= 0)
				{
					result = -1024;
				}
				else
				{
					result = NameServerNamager.CheckInvalidCharacters(bulltin, false);
					if (result >= 0)
					{
						if (!this.IsGongNengOpened(client, false))
						{
							result = -12;
						}
						else
						{
							long nowTicks = TimeUtil.NOW();
							JunTuanRolePermissionInfo permition = this.GetRolePermitionInfo(client.ClientData.JunTuanZhiWu);
							if (permition.BulletinCD <= 0 || client.ClientData.LastJunTuanBulletinTicks > nowTicks)
							{
								result = -2007;
							}
							else
							{
								client.ClientData.LastJunTuanBulletinTicks = nowTicks + (long)(permition.BulletinCD * 1000);
								result = JunTuanClient.getInstance().ChangeJunTuanBulltin(bhid, client.ClientData.JunTuanId, bulltin);
							}
						}
					}
				}
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000CD0 RID: 3280 RVA: 0x000C6ED8 File Offset: 0x000C50D8
		public bool ProcessQuitJunTuanCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int otherBhid = Global.SafeConvertToInt32(cmdParams[1]);
				int bhid = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				int result;
				if (bhid <= 0 || client.ClientData.BHZhiWu != 1 || junTuanId <= 0)
				{
					result = -1024;
				}
				else if (!this.IsGongNengOpened(client, false))
				{
					result = -12;
				}
				else if (KarenBattleManager.getInstance().InActivityTime())
				{
					result = -2002;
				}
				else
				{
					JunTuanRolePermissionInfo permitionInfo = this.GetRolePermitionInfo(client.ClientData.JunTuanZhiWu);
					if (bhid != otherBhid)
					{
						if (permitionInfo.Manager <= 0)
						{
							result = -12;
							goto IL_F1;
						}
					}
					else if (permitionInfo.Quit <= 0)
					{
						result = -12;
						goto IL_F1;
					}
					result = JunTuanClient.getInstance().QuitJunTuan(bhid, client.ClientData.JunTuanId, otherBhid);
				}
				IL_F1:
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000CD1 RID: 3281 RVA: 0x000C701C File Offset: 0x000C521C
		public bool ProcessDestroyJunTuanCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int bhid = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				int result;
				if (bhid <= 0 || client.ClientData.BHZhiWu != 1 || junTuanId <= 0)
				{
					result = -1024;
				}
				else if (!this.IsGongNengOpened(client, false))
				{
					result = -12;
				}
				else
				{
					JunTuanRolePermissionInfo permitionInfo = this.GetRolePermitionInfo(client.ClientData.JunTuanZhiWu);
					if (permitionInfo.Dissolution <= 0)
					{
						result = -12;
					}
					else if (KarenBattleManager.getInstance().InActivityTime())
					{
						result = -2002;
					}
					else
					{
						result = JunTuanClient.getInstance().DestroyJunTuan(bhid, client.ClientData.JunTuanId);
						if (0 == result)
						{
							EventLogManager.AddJunTuanDestroyEvent(client, junTuanId, bhid);
						}
					}
				}
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000CD2 RID: 3282 RVA: 0x000C7140 File Offset: 0x000C5340
		public bool ProcessJunTuanBangHuiZhiWuCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int otherBhId = Global.SafeConvertToInt32(cmdParams[1]);
				int bhid = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				if (otherBhId > 0 && otherBhId != bhid)
				{
					if (bhid <= 0 || client.ClientData.BHZhiWu != 1 || junTuanId <= 0)
					{
						result = -1024;
					}
					else if (!this.IsGongNengOpened(client, false))
					{
						result = -12;
					}
					else if (KarenBattleManager.getInstance().InActivityTime())
					{
						result = -2002;
					}
					else
					{
						result = JunTuanClient.getInstance().JunTuanChangeBangHuiZhiWu(bhid, client.ClientData.JunTuanId, otherBhId, 1);
						if (0 == result)
						{
							JunTuanData data = JunTuanClient.getInstance().GetJunTuanData(client.ClientData.Faction, client.ClientData.JunTuanId, true);
							if (null != data)
							{
								EventLogManager.AddJunTuanZhiWuEvent(client, 1, data.LeaderRoleId, 1);
							}
						}
					}
				}
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x000C72AC File Offset: 0x000C54AC
		public bool ProcessJunTuanRoleZhiWuCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				List<int> list = DataHelper.BytesToObject<List<int>>(bytes, 0, bytes.Length);
				if (list.Count < 2 || list[0] != client.ClientData.RoleID || list[1] != 3)
				{
					result = -18;
				}
				else if (list.Count > this.RuntimeData.LegionsEliteNum + 2)
				{
					result = -1034;
				}
				else
				{
					int bhid = client.ClientData.Faction;
					int junTuanId = client.ClientData.JunTuanId;
					if (bhid <= 0 || client.ClientData.BHZhiWu != 1 || junTuanId <= 0)
					{
						result = -1024;
					}
					else if (this.GetRolePermitionInfo(client.ClientData.JunTuanZhiWu).AppointElite <= 0)
					{
						result = -12;
					}
					else if (!this.IsGongNengOpened(client, false))
					{
						result = -12;
					}
					else
					{
						List<JunTuanRoleData> roleList = this.GetJunTuanRoleList(bhid, client.ServerId);
						HashSet<int> roles = new HashSet<int>();
						HashSet<int> jingYingRoles = new HashSet<int>();
						for (int i = 0; i < roleList.Count; i++)
						{
							roles.Add(roleList[i].RoleId);
						}
						for (int i = 2; i < list.Count; i++)
						{
							jingYingRoles.Add(list[i]);
							if (!roles.Contains(list[i]))
							{
								result = -1000;
							}
						}
						if (result >= 0)
						{
							foreach (JunTuanRoleData roleData in roleList)
							{
								if (roleData.RoleId == client.ClientData.RoleID)
								{
									roleData.JuTuanZhiWu = 2;
								}
								else
								{
									roleData.JuTuanZhiWu = (jingYingRoles.Contains(roleData.RoleId) ? 3 : 0);
								}
								EventLogManager.AddJunTuanZhiWuEvent(client, client.ClientData.JunTuanZhiWu, roleData.RoleId, roleData.JuTuanZhiWu);
							}
							Global.SendToDB<List<int>>(1240, list, client.ServerId);
							result = JunTuanClient.getInstance().UpdateRoleDataList(bhid, roleList);
							foreach (GameClient c in GameManager.ClientMgr.GetAllClients(true))
							{
								if (c.ClientData.JunTuanId == junTuanId && c.ClientData.BHZhiWu != 1)
								{
									int zhiwu = jingYingRoles.Contains(c.ClientData.RoleID) ? 3 : 0;
									if (zhiwu != c.ClientData.JunTuanZhiWu)
									{
										JunTuanBangHuiMiniData data = new JunTuanBangHuiMiniData(c.ClientData.Faction, client.ClientData.JunTuanId, c.ClientData.JunTuanName, zhiwu, c.ClientData.LingDi);
										this.JunTuanZhiWuChanged(c, data);
									}
								}
							}
						}
					}
				}
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000CD4 RID: 3284 RVA: 0x000C768C File Offset: 0x000C588C
		public bool ProcessGetJunTuanRequestListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<JunTuanRequestData> result = null;
				int bhid = client.ClientData.Faction;
				if (bhid > 0 && client.ClientData.BHZhiWu == 1)
				{
					if (client.ClientData.JunTuanId != 0)
					{
						if (this.IsGongNengOpened(client, false))
						{
							result = JunTuanClient.getInstance().GetJunTuanRequestList(bhid);
						}
					}
				}
				client.sendCmd<List<JunTuanRequestData>>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000CD5 RID: 3285 RVA: 0x000C7744 File Offset: 0x000C5944
		public bool ProcessGetJunTuanLogListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<JunTuanEventLog> result = null;
				int bhid = client.ClientData.Faction;
				if (bhid > 0)
				{
					if (client.ClientData.JunTuanId != 0)
					{
						if (this.IsGongNengOpened(client, false))
						{
							result = JunTuanClient.getInstance().GetJunTuanLogList(bhid);
						}
					}
				}
				client.sendCmd<List<JunTuanEventLog>>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000CD6 RID: 3286 RVA: 0x000C77E8 File Offset: 0x000C59E8
		public bool ProcessGetJunTuanRoleListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<JunTuanRoleData> result = new List<JunTuanRoleData>();
				int bhid = client.ClientData.Faction;
				if (bhid > 0)
				{
					int junTuanId = client.ClientData.JunTuanId;
					if (junTuanId != 0)
					{
						if (this.IsGongNengOpened(client, false))
						{
							Dictionary<int, JunTuanRoleData> roleDict = new Dictionary<int, JunTuanRoleData>();
							List<JunTuanRoleData> roleList = this.GetJunTuanRoleList(bhid, client.ServerId);
							List<JunTuanRoleData> list = JunTuanClient.getInstance().GetJunTuanRoleList(bhid, junTuanId);
							if (list != null && list.Count > 0)
							{
								foreach (JunTuanRoleData roleData in list)
								{
									if (roleData.BhId != bhid)
									{
										result.Add(roleData);
									}
									else
									{
										roleDict[roleData.RoleId] = roleData;
									}
								}
							}
							foreach (JunTuanRoleData roleData in roleList)
							{
								result.Add(roleData);
								JunTuanRoleData data;
								if (roleDict.TryGetValue(roleData.RoleId, out data) && (data.JuTuanZhiWu == 2 || data.JuTuanZhiWu == 1))
								{
									roleData.JuTuanZhiWu = data.JuTuanZhiWu;
								}
							}
						}
					}
				}
				client.sendCmd<List<JunTuanRoleData>>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000CD7 RID: 3287 RVA: 0x000C79F8 File Offset: 0x000C5BF8
		public bool ProcessGetJunTuanRankListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<JunTuanRankData> result = null;
				int bhid = client.ClientData.Faction;
				if (bhid > 0)
				{
					if (client.ClientData.JunTuanId != 0)
					{
						if (this.IsGongNengOpened(client, false))
						{
							result = JunTuanClient.getInstance().GetJunTuanRankingData();
						}
					}
				}
				client.sendCmd<List<JunTuanRankData>>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000CD8 RID: 3288 RVA: 0x000C7A9C File Offset: 0x000C5C9C
		public bool ProcessJunTuanJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int junTuanId = Global.SafeConvertToInt32(cmdParams[1]);
				int bhid = client.ClientData.Faction;
				if (bhid <= 0 || client.ClientData.BHZhiWu != 1)
				{
					result = -1002;
				}
				else if (client.ClientData.JunTuanId != 0)
				{
					result = -1020;
				}
				else if (this.IsGongNengOpened(client, false))
				{
					if (KarenBattleManager.getInstance().InActivityTime())
					{
						result = -2002;
					}
					else
					{
						BangHuiDetailData bhData = Global.GetBangHuiDetailData(client.ClientData.RoleID, bhid, client.ServerId);
						if (null == bhData)
						{
							result = -15;
						}
						else if (bhData.QiLevel < this.RuntimeData.LegionsNeed)
						{
							result = -1008;
						}
						else
						{
							JunTuanRequestData data = new JunTuanRequestData();
							data.BhId = bhid;
							data.BhName = bhData.BHName;
							data.BhZoneId = bhData.ZoneID;
							data.LeaderRoleId = client.ClientData.RoleID;
							data.LeaderName = client.ClientData.RoleName;
							data.LeaderZoneId = client.ClientData.ZoneID;
							data.ZhanLi = bhData.TotalCombatForce;
							data.RoleNum = bhData.TotalNum;
							data.JunTuanId = junTuanId;
							data.Occupation = client.ClientData.Occupation;
							result = this.UpdateJunTuanRoleList(bhid, client.ServerId);
							if (result >= 0)
							{
								result = JunTuanClient.getInstance().JoinJunTuan(data);
							}
						}
					}
				}
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000CD9 RID: 3289 RVA: 0x000C7CA8 File Offset: 0x000C5EA8
		public bool ProcessJunTuanJoinResponseCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int otherBhid = Global.SafeConvertToInt32(cmdParams[1]);
				int accept = Global.SafeConvertToInt32(cmdParams[2]);
				int bhid = client.ClientData.Faction;
				int result;
				if (bhid <= 0 || client.ClientData.BHZhiWu != 1)
				{
					result = -1024;
				}
				else if (client.ClientData.JunTuanId == 0 || this.GetRolePermitionInfo(client.ClientData.JunTuanZhiWu).Manager <= 0)
				{
					result = -1024;
				}
				else if (!this.IsGongNengOpened(client, false))
				{
					result = -12;
				}
				else if (KarenBattleManager.getInstance().InActivityTime() && accept > 0)
				{
					result = -2002;
				}
				else
				{
					result = JunTuanClient.getInstance().JoinJunTuanResponse(bhid, client.ClientData.JunTuanId, otherBhid, accept > 0);
				}
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000CDA RID: 3290 RVA: 0x000C7DD8 File Offset: 0x000C5FD8
		public bool ProcessGetJunTuanBangHuiListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<JunTuanBangHuiData> result = null;
				int id = Global.SafeConvertToInt32(cmdParams[1]);
				int bhid = client.ClientData.Faction;
				if (bhid <= 0)
				{
				}
				int junTuanId = client.ClientData.JunTuanId;
				if (junTuanId <= 0)
				{
				}
				if (this.IsGongNengOpened(client, false))
				{
					if (bhid > 0 && id == junTuanId)
					{
						result = JunTuanClient.getInstance().GetJunTuanBangHuiList(bhid, junTuanId);
					}
					else
					{
						result = JunTuanClient.getInstance().GetJunTuanBangHuiList(id);
					}
				}
				client.sendCmd<List<JunTuanBangHuiData>>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000CDB RID: 3291 RVA: 0x000C7ED8 File Offset: 0x000C60D8
		public bool ProcessGetJunTuanTaskAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int taskId = Global.SafeConvertToInt32(cmdParams[1]);
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot4))
				{
					return true;
				}
				if (!this.IsGongNengOpened(client, false))
				{
					result = -12;
				}
				else
				{
					bool complete = false;
					JunTuanTaskAllData taskAllData;
					result = JunTuanClient.getInstance().GetJunTuanTaskAllData(client.ClientData.Faction, client.ClientData.JunTuanId, out taskAllData);
					if (result >= 0)
					{
						if (taskAllData != null && taskAllData.TaskList != null)
						{
							JunTuanTaskData taskData = taskAllData.TaskList.Find((JunTuanTaskData x) => x.TaskId == taskId);
							if (taskData != null && taskData.TaskState == 1L)
							{
								complete = true;
							}
						}
						if (!complete)
						{
							result = -1028;
						}
						else
						{
							string awardsInfo = Global.GetRoleParamByName(client, "20018");
							JunTuanTaskInfo taskInfo;
							int idx;
							lock (this.RuntimeData.Mutex)
							{
								if (!this.RuntimeData.TaskList.Value.TryGetValue(taskId, out taskInfo))
								{
									result = -1028;
									goto IL_463;
								}
								if (!this.RuntimeData.Task2IdxDict.TryGetValue(taskId, out idx) || idx >= this.RuntimeData.TaskCount)
								{
									result = -1028;
									goto IL_463;
								}
							}
							int nowWeekDayId = TimeUtil.GetWeekStartDayIdNow();
							int[] array = new int[this.RuntimeData.TaskCount];
							if (!string.IsNullOrEmpty(awardsInfo))
							{
								string[] arr = awardsInfo.Split(new char[]
								{
									','
								});
								int weekDayId;
								if (arr.Length == 2 && int.TryParse(arr[0], out weekDayId))
								{
									if (weekDayId >= nowWeekDayId)
									{
										int i = 0;
										while (i < arr[1].Length && i < this.RuntimeData.TaskCount)
										{
											array[i] = ((arr[1][i] == '0') ? 0 : 1);
											i++;
										}
									}
								}
							}
							if (array[idx] == 1)
							{
								result = -200;
							}
							else
							{
								array[idx] = 1;
								List<GoodsData> goodsDataList = Global.ConvertToGoodsDataList(taskInfo.Item.Items, -1);
								if (!Global.CanAddGoodsDataList(client, goodsDataList))
								{
									result = -100;
								}
								else
								{
									awardsInfo = string.Format("{0},{1}", nowWeekDayId, string.Join<int>("", array));
									Global.SaveRoleParamsStringToDB(client, "20018", awardsInfo, true);
									if (taskInfo.Exp > 0)
									{
										long expAward = Global.GetExpMultiByZhuanShengExpXiShu(client, (long)taskInfo.Exp);
										GameManager.ClientMgr.ProcessRoleExperience(client, expAward, true, false, false, "none");
									}
									if (taskInfo.ZhanGong > 0)
									{
										int zhanGongAwards = taskInfo.ZhanGong;
										GameManager.ClientMgr.AddBangGong(client, ref zhanGongAwards, AddBangGongTypes.JunTuanTaskAwards, 0);
									}
									if (!Global.CanAddGoodsDataList(client, goodsDataList))
									{
										GameManager.ClientMgr.SendMailWhenPacketFull(client, goodsDataList, GLang.GetLang(2608, new object[0]), GLang.GetLang(2608, new object[0]));
									}
									else
									{
										for (int i = 0; i < goodsDataList.Count; i++)
										{
											Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsDataList[i].Quality, "", goodsDataList[i].Forge_level, goodsDataList[i].Binding, 0, "", true, 1, "阵营战排名奖励", "1900-01-01 12:00:00", 0, goodsDataList[i].BornIndex, goodsDataList[i].Lucky, 0, goodsDataList[i].ExcellenceInfo, goodsDataList[i].AppendPropLev, 0, null, null, 0, true);
										}
									}
									result = 0;
								}
							}
						}
					}
				}
				IL_463:
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		// Token: 0x06000CDC RID: 3292 RVA: 0x000C83BC File Offset: 0x000C65BC
		public bool ProcessGetJunTuanTaskListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<JunTuanTaskData> taskDataList = new List<JunTuanTaskData>();
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot4))
				{
					return true;
				}
				int nowWeekDayId = TimeUtil.GetWeekStartDayIdNow();
				string awardsInfo = Global.GetRoleParamByName(client, "20018");
				int[] array = new int[this.RuntimeData.TaskCount];
				if (!string.IsNullOrEmpty(awardsInfo))
				{
					string[] arr = awardsInfo.Split(new char[]
					{
						','
					});
					int weekDayId;
					if (arr.Length == 2 && int.TryParse(arr[0], out weekDayId))
					{
						if (weekDayId >= nowWeekDayId)
						{
							int i = 0;
							while (i < arr[1].Length && i < this.RuntimeData.TaskCount)
							{
								array[i] = ((arr[1][i] == '0') ? 0 : 1);
								i++;
							}
						}
					}
				}
				JunTuanTaskAllData taskAllData;
				if (JunTuanClient.getInstance().GetJunTuanTaskAllData(client.ClientData.Faction, client.ClientData.JunTuanId, out taskAllData) >= 0)
				{
					lock (this.RuntimeData.Mutex)
					{
						if (taskAllData != null && taskAllData.TaskList != null)
						{
							foreach (JunTuanTaskData t in taskAllData.TaskList)
							{
								JunTuanTaskData taskData = t.Clone();
								taskDataList.Add(taskData);
								int idx;
								if (this.RuntimeData.Task2IdxDict.TryGetValue(taskData.TaskId, out idx) && idx < array.Length && array[idx] == 1)
								{
									taskData.HasGet = 1;
								}
								if (taskData.TaskState == 1L && taskData.HasGet == 0)
								{
								}
							}
						}
					}
				}
				client.sendCmd<List<JunTuanTaskData>>(nID, taskDataList, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		// Token: 0x06000CDD RID: 3293 RVA: 0x000C8680 File Offset: 0x000C6880
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot4) && GlobalNew.IsGongNengOpened(client, GongNengIDs.JunTuan, hint);
		}

		// Token: 0x06000CDE RID: 3294 RVA: 0x000C86B0 File Offset: 0x000C68B0
		public JunTuanRolePermissionInfo GetRolePermitionInfo(int zhiwu)
		{
			if (zhiwu == 0)
			{
				zhiwu = 4;
			}
			JunTuanRolePermissionInfo info = null;
			JunTuanRolePermissionInfo result;
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.RolePermissionDict.Value.TryGetValue(zhiwu, out info))
				{
					result = info;
				}
				else
				{
					result = new JunTuanRolePermissionInfo();
				}
			}
			return result;
		}

		// Token: 0x06000CDF RID: 3295 RVA: 0x000C873C File Offset: 0x000C693C
		public void UpdateJunTuanData(KuaFuData<JunTuanData> data)
		{
		}

		// Token: 0x06000CE0 RID: 3296 RVA: 0x000C8740 File Offset: 0x000C6940
		public void UpdateBhJunTuan(JunTuanBangHuiMiniData data)
		{
			GameManager.ClientMgr.BroadcastServerCmd<JunTuanBangHuiMiniData>(1229, data, true);
			int bhZhiWu = data.JunTuanZhiWu;
			DateTime now = TimeUtil.NowDateTime();
			List<BangHuiMemberData> memberList = Global.sendToDB<List<BangHuiMemberData>, string>(299, string.Format("{0}:{1}", 0, data.BhId), 0);
			foreach (BangHuiMemberData member in memberList)
			{
				int rid = member.RoleID;
				GameClient client = GameManager.ClientMgr.FindClient(rid);
				if (null == client)
				{
					if (data.JunTuanChanged > 0)
					{
						GameManager.DBCmdMgr.AddDBCmd(10100, string.Format("{0}:{1}:{2}", rid, "10182", now.Ticks.ToString()), null, 0);
					}
				}
				else
				{
					if (data.JunTuanChanged > 0)
					{
						Global.SaveRoleParamsDateTimeToDB(client, "10182", now, true);
					}
					if (client.ClientData.Faction == data.BhId && (client.ClientData.JunTuanId != data.JunTuanId || client.ClientData.JunTuanName != data.JunTuanName || client.ClientData.JunTuanZhiWu != data.JunTuanZhiWu))
					{
						client.ClientData.JunTuanId = data.JunTuanId;
						client.ClientData.JunTuanName = data.JunTuanName;
						data.JunTuanZhiWu = this.GetJunTuanZhiWu(client, bhZhiWu);
						data.RoleId = client.ClientData.RoleID;
						GameManager.ClientMgr.BroadcastOthersCmdData<JunTuanBangHuiMiniData>(client, 1225, data, true);
						LingDiCaiJiManager.getInstance().UpdateChengHaoBuff(client);
						EraManager.getInstance().OnJunTuanZhiWuChanged(client);
					}
				}
			}
		}

		// Token: 0x06000CE1 RID: 3297 RVA: 0x000C8960 File Offset: 0x000C6B60
		public void UpdateJunTuanTaskData(JunTuanTaskData data)
		{
		}

		// Token: 0x06000CE2 RID: 3298 RVA: 0x000C8964 File Offset: 0x000C6B64
		public JunTuanBaseData GetJunTuanBaseDataByJunTuanID(int junTuanID)
		{
			JunTuanBaseData result;
			lock (this.RuntimeData.Mutex)
			{
				JunTuanBaseData baseData;
				this.RuntimeData.JunTuanBaseDict.TryGetValue(junTuanID, out baseData);
				result = baseData;
			}
			return result;
		}

		// Token: 0x06000CE3 RID: 3299 RVA: 0x000C89EC File Offset: 0x000C6BEC
		public void OnInitGame(RoleDataEx dbRd)
		{
			bool flag = false;
			try
			{
				object mutex;
				Monitor.Enter(mutex = this.RuntimeData.Mutex, ref flag);
				int bhid = dbRd.Faction;
				int junTuanId;
				if (this.RuntimeData.BangHuiJunTuanIdDict.TryGetValue(bhid, out junTuanId))
				{
					JunTuanBaseData data;
					if (this.RuntimeData.JunTuanBaseDict.TryGetValue(junTuanId, out data))
					{
						dbRd.JunTuanId = data.JunTuanId;
						dbRd.JunTuanName = data.JunTuanName;
						dbRd.JunTuanZhiWu = this.GetJunTuanZhiWu(dbRd, (data.BhList.FindIndex((int x) => x == bhid) == 0) ? 1 : 0);
					}
				}
			}
			finally
			{
				if (flag)
				{
					object mutex;
					Monitor.Exit(mutex);
				}
			}
		}

		// Token: 0x06000CE4 RID: 3300 RVA: 0x000C8B04 File Offset: 0x000C6D04
		public void OnInitGame(GameClient client)
		{
			if (!KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				bool flag = false;
				try
				{
					object mutex;
					Monitor.Enter(mutex = this.RuntimeData.Mutex, ref flag);
					int bhid = client.ClientData.Faction;
					int junTuanId;
					if (!this.RuntimeData.BangHuiJunTuanIdDict.TryGetValue(bhid, out junTuanId))
					{
						return;
					}
					JunTuanBaseData data;
					if (this.RuntimeData.JunTuanBaseDict.TryGetValue(junTuanId, out data))
					{
						client.ClientData.JunTuanId = data.JunTuanId;
						client.ClientData.JunTuanName = data.JunTuanName;
						client.ClientData.JunTuanZhiWu = this.GetJunTuanZhiWu(client, (data.BhList.FindIndex((int x) => x == bhid) == 0) ? 1 : 0);
					}
				}
				finally
				{
					if (flag)
					{
						object mutex;
						Monitor.Exit(mutex);
					}
				}
				if (client.ClientData.JunTuanId > 0)
				{
					JunTuanData data2 = JunTuanClient.getInstance().GetJunTuanData(client.ClientData.Faction, client.ClientData.JunTuanId, false);
					if (null != data2)
					{
						if (data2.Point < this.RuntimeData.LegionProsperityCost[2])
						{
							GameManager.ClientMgr.NotifyHintMsgDelay(client, GLang.GetLang(2609, new object[0]));
						}
					}
				}
			}
		}

		// Token: 0x06000CE5 RID: 3301 RVA: 0x000C8CA8 File Offset: 0x000C6EA8
		public void JunTuanChat(GameClient client, string text)
		{
			long nowTicks = TimeUtil.NOW();
			JunTuanRolePermissionInfo permitionInfo = this.GetRolePermitionInfo(client.ClientData.JunTuanZhiWu);
			if (null != permitionInfo)
			{
				if (Global.GetUnionLevel(client, false) < Global.GetUnionLevel(permitionInfo.TalkLevel[0], permitionInfo.TalkLevel[1], false))
				{
					GameManager.ClientMgr.NotifyHintMsg(client, GLang.GetLang(2610, new object[0]));
				}
				else
				{
					int talkCD = Math.Max(permitionInfo.TalkCD * 1000, 3000);
					if (nowTicks - client.ClientData.LastJunTuanChatTicks < (long)talkCD)
					{
						long secs = ((long)talkCD - nowTicks - client.ClientData.LastJunTuanChatTicks) / 1000L + 1L;
						GameManager.ClientMgr.NotifyHintMsg(client, string.Format(GLang.GetLang(2611, new object[0]), secs));
					}
					else
					{
						client.ClientData.LastJunTuanChatTicks = nowTicks;
						KFChat chat = null;
						lock (this.RuntimeData.Mutex)
						{
							int bhid = client.ClientData.Faction;
							int junTuanId;
							if (!this.RuntimeData.BangHuiJunTuanIdDict.TryGetValue(bhid, out junTuanId) || junTuanId <= 0)
							{
								return;
							}
							chat = new KFChat(client.ClientData.ZoneID, client.ClientData.RoleName, text, junTuanId);
							this.RuntimeData.JunTuanChatList.Add(chat);
						}
						this.BroadcastJunTuanChatMsg(chat);
					}
				}
			}
		}

		// Token: 0x06000CE6 RID: 3302 RVA: 0x000C8E70 File Offset: 0x000C7070
		private int GetJunTuanZhiWu(RoleDataEx dbRd, int bhZhiWu)
		{
			int bhid = dbRd.Faction;
			int result;
			if (dbRd.BHZhiWu == 1)
			{
				if (bhZhiWu > 0)
				{
					result = 1;
				}
				else
				{
					result = 2;
				}
			}
			else
			{
				result = dbRd.JunTuanZhiWu;
			}
			return result;
		}

		// Token: 0x06000CE7 RID: 3303 RVA: 0x000C8EB4 File Offset: 0x000C70B4
		private int GetJunTuanZhiWu(GameClient client, int bhZhiWu)
		{
			int bhid = client.ClientData.Faction;
			int result;
			if (client.ClientData.BHZhiWu == 1)
			{
				if (bhZhiWu > 0)
				{
					result = 1;
				}
				else
				{
					result = 2;
				}
			}
			else
			{
				result = client.ClientData.JunTuanZhiWu;
			}
			return result;
		}

		// Token: 0x06000CE8 RID: 3304 RVA: 0x000C8F08 File Offset: 0x000C7108
		private List<JunTuanRoleData> GetJunTuanRoleList(int bhid, int serverId)
		{
			List<JunTuanRoleData> roleList = new List<JunTuanRoleData>();
			BangHuiMiniData bhMiniData = Global.GetBangHuiMiniData(bhid, serverId);
			List<BangHuiMemberData> memberList = Global.sendToDB<List<BangHuiMemberData>, string>(299, string.Format("{0}:{1}", 0, bhid), serverId);
			foreach (BangHuiMemberData member in memberList)
			{
				int zhiwu = (member.BHZhiwu == 1) ? 2 : member.JunTuanZhiWu;
				JunTuanRoleData roleData = new JunTuanRoleData
				{
					BhId = bhid,
					RoleId = member.RoleID,
					RoleName = member.RoleName,
					ZoneId = member.ZoneID,
					BhName = bhMiniData.BHName,
					BhZoneId = member.ZoneID,
					ZhanLi = member.BangHuiMemberCombatForce,
					ChangeLifeCount = member.BangHuiMemberChangeLifeLev,
					Level = member.Level,
					JuTuanZhiWu = zhiwu,
					Occu = member.Occupation,
					OnlineState = member.OnlineState
				};
				roleList.Add(roleData);
			}
			return roleList;
		}

		// Token: 0x06000CE9 RID: 3305 RVA: 0x000C9054 File Offset: 0x000C7254
		public void DelayUpdateJunTuanRoleList(int bhid)
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.HasUpdateRoleDataHashSet.Add(bhid);
			}
		}

		// Token: 0x06000CEA RID: 3306 RVA: 0x000C90B0 File Offset: 0x000C72B0
		public int UpdateJunTuanRoleList(int bhid, int serverId)
		{
			List<JunTuanRoleData> roleList = this.GetJunTuanRoleList(bhid, serverId);
			return JunTuanClient.getInstance().UpdateRoleDataList(bhid, roleList);
		}

		// Token: 0x06000CEB RID: 3307 RVA: 0x000C90D8 File Offset: 0x000C72D8
		public void AddJunTuanTaskValue(int bhid, int junTuanId, int sceneType, int taskValue)
		{
			if (bhid > 0 && junTuanId > 0)
			{
				TimeSpan nowTimespan = TimeUtil.GetTimeOfWeekNow();
				if (!(nowTimespan < this.RuntimeData.TaskStartEndTimeList[0]) && !(nowTimespan > this.RuntimeData.TaskStartEndTimeList[1]))
				{
					lock (this.RuntimeData.Mutex)
					{
						foreach (JunTuanTaskInfo taskInfo in this.RuntimeData.TaskList.Value.Values)
						{
							if (taskInfo.CompleteType == 2)
							{
								if (taskInfo.TypeID.Contains(sceneType))
								{
									this.RuntimeData.JunTuanTaskQueue.Enqueue(new Tuple<int, int, int, int, long>(bhid, junTuanId, taskInfo.ID, taskValue, TimeUtil.NowDateTime().Ticks));
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000CEC RID: 3308 RVA: 0x000C922C File Offset: 0x000C742C
		public void AddJunTuanTaskValue(GameClient client, int taskType, int taskValue)
		{
			if (!KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				int bhid = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				if (bhid > 0 && junTuanId > 0)
				{
					TimeSpan nowTimespan = TimeUtil.GetTimeOfWeekNow();
					if (!(nowTimespan < this.RuntimeData.TaskStartEndTimeList[0]) && !(nowTimespan > this.RuntimeData.TaskStartEndTimeList[1]))
					{
						lock (this.RuntimeData.Mutex)
						{
							foreach (JunTuanTaskInfo taskInfo in this.RuntimeData.TaskList.Value.Values)
							{
								if (taskInfo.CompleteType == taskType)
								{
									switch (taskType)
									{
									case 1:
									case 2:
										this.RuntimeData.JunTuanTaskQueue.Enqueue(new Tuple<int, int, int, int, long>(bhid, junTuanId, taskInfo.ID, taskValue, TimeUtil.NowDateTime().Ticks));
										break;
									case 3:
										this.RuntimeData.JunTuanTaskQueue.Enqueue(new Tuple<int, int, int, int, long>(bhid, junTuanId, taskInfo.ID, taskValue, TimeUtil.NowDateTime().Ticks));
										break;
									case 4:
									{
										int mapType = (int)Global.GetMapSceneType(client.ClientData.MapCode);
										if (taskInfo.TypeID.Contains(mapType))
										{
											this.RuntimeData.JunTuanTaskQueue.Enqueue(new Tuple<int, int, int, int, long>(bhid, junTuanId, taskInfo.ID, taskValue, TimeUtil.NowDateTime().Ticks));
										}
										break;
									}
									case 5:
										if (taskInfo.TypeID.Contains(client.ClientData.FuBenID))
										{
											this.RuntimeData.JunTuanTaskQueue.Enqueue(new Tuple<int, int, int, int, long>(bhid, junTuanId, taskInfo.ID, taskValue, TimeUtil.NowDateTime().Ticks));
										}
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000CED RID: 3309 RVA: 0x000C94CC File Offset: 0x000C76CC
		public void AddJunTuanTaskValue(GameClient client, Monster monter, int taskType, int taskValue)
		{
			if (!KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				int bhid = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				if (bhid > 0 || junTuanId > 0)
				{
					TimeSpan nowTimespan = TimeUtil.GetTimeOfWeekNow();
					if (!(nowTimespan < this.RuntimeData.TaskStartEndTimeList[0]) && !(nowTimespan > this.RuntimeData.TaskStartEndTimeList[1]))
					{
						lock (this.RuntimeData.Mutex)
						{
							if (this.RuntimeData.KillMonsterIds.Contains(monter.MonsterInfo.ExtensionID))
							{
								foreach (JunTuanTaskInfo taskInfo in this.RuntimeData.TaskList.Value.Values)
								{
									if (taskInfo.CompleteType == taskType)
									{
										if (taskType == 1)
										{
											if (taskInfo.TypeID.Contains(monter.MonsterInfo.ExtensionID))
											{
												this.RuntimeData.JunTuanTaskQueue.Enqueue(new Tuple<int, int, int, int, long>(bhid, junTuanId, taskInfo.ID, taskValue, TimeUtil.NowDateTime().Ticks));
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000CEE RID: 3310 RVA: 0x000C969C File Offset: 0x000C789C
		private void TimerProc(object sender, EventArgs e)
		{
			bool all = false;
			long nowTicks = TimeUtil.NOW();
			Dictionary<int, List<GameClient>> hashset = new Dictionary<int, List<GameClient>>();
			lock (this.RuntimeData.Mutex)
			{
				if (nowTicks > this.RuntimeData.NextUpdateTicks)
				{
					this.RuntimeData.NextUpdateTicks = nowTicks + 2200L;
					all = true;
				}
			}
			if (all)
			{
				foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
				{
					int bhid = client.ClientData.Faction;
					if (bhid > 0)
					{
						List<GameClient> list;
						if (!hashset.TryGetValue(bhid, out list))
						{
							list = new List<GameClient>();
							hashset[bhid] = list;
						}
						list.Add(client);
						if (client.ClientData.JunTuanId > 0 && !client.ClientSocket.IsKuaFuLogin && this.RuntimeData.HasUpdateRoleDataHashSet.Contains(bhid))
						{
							this.RuntimeData.HasUpdateRoleDataHashSet.Remove(bhid);
							this.UpdateJunTuanRoleList(bhid, client.ServerId);
						}
					}
					else if (client.ClientData.JunTuanId != 0 || client.ClientData.JunTuanName != null)
					{
						JunTuanBangHuiMiniData changeData = new JunTuanBangHuiMiniData();
						this.JunTuanZhiWuChanged(client, changeData);
					}
				}
			}
			List<JunTuanBaseData> dataList = JunTuanClient.getInstance().GetJunTuanBaseDataList(true);
			if (null != dataList)
			{
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.BangHuiJunTuanIdDict.Clear();
					this.RuntimeData.JunTuanBaseDict.Clear();
					foreach (JunTuanBaseData data in dataList)
					{
						if (null != data.BhList)
						{
							int bhZhiWu = 1;
							foreach (int bhid in data.BhList)
							{
								this.RuntimeData.BangHuiJunTuanIdDict[bhid] = data.JunTuanId;
								List<GameClient> list;
								if (hashset.TryGetValue(bhid, out list) && list != null)
								{
									foreach (GameClient c in list)
									{
										int junTuanZhiWu = this.GetJunTuanZhiWu(c, bhZhiWu);
										if (c.ClientData.JunTuanId != data.JunTuanId || c.ClientData.JunTuanName != data.JunTuanName || c.ClientData.JunTuanZhiWu != junTuanZhiWu || c.ClientData.LingDi != data.LingDi)
										{
											JunTuanBangHuiMiniData changeData = new JunTuanBangHuiMiniData(bhid, data.JunTuanId, data.JunTuanName, junTuanZhiWu, data.LingDi);
											this.JunTuanZhiWuChanged(c, changeData);
										}
									}
								}
								bhZhiWu = 0;
							}
							this.RuntimeData.JunTuanBaseDict[data.JunTuanId] = data;
						}
					}
					foreach (KeyValuePair<int, List<GameClient>> kv in hashset)
					{
						int bhid = kv.Key;
						if (!this.RuntimeData.BangHuiJunTuanIdDict.ContainsKey(bhid))
						{
							List<GameClient> list = kv.Value;
							JunTuanBangHuiMiniData data2 = new JunTuanBangHuiMiniData(bhid, 0, null, 0, 0);
							foreach (GameClient c in list)
							{
								data2.JunTuanZhiWu = 0;
								if (c.ClientData.JunTuanId != data2.JunTuanId || c.ClientData.JunTuanName != data2.JunTuanName || (c.ClientData.JunTuanZhiWu != data2.JunTuanZhiWu | c.ClientData.LingDi != data2.LingDi))
								{
									this.JunTuanZhiWuChanged(c, data2);
								}
							}
						}
					}
				}
			}
			List<Tuple<int, int, int, int, long>> taskOpList = null;
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.JunTuanTaskQueue.Count > 0)
				{
					taskOpList = new List<Tuple<int, int, int, int, long>>();
					taskOpList.AddRange(this.RuntimeData.JunTuanTaskQueue);
					this.RuntimeData.JunTuanTaskQueue.Clear();
				}
			}
			if (null != taskOpList)
			{
				foreach (Tuple<int, int, int, int, long> taskOp in taskOpList)
				{
					if (!JunTuanClient.getInstance().IsTaskComplete(taskOp.Item1, taskOp.Item2, taskOp.Item3))
					{
						int result = JunTuanClient.getInstance().JunTuanChangeTaskValue(taskOp.Item1, taskOp.Item2, taskOp.Item3, taskOp.Item4, taskOp.Item5);
						if (result == -11000)
						{
							lock (this.RuntimeData.Mutex)
							{
								this.RuntimeData.JunTuanTaskQueue.Enqueue(taskOp);
							}
						}
						else if (result == 1)
						{
						}
					}
				}
			}
			List<KFChat> chatList = null;
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.JunTuanChatList.Count > 0)
				{
					chatList = new List<KFChat>(this.RuntimeData.JunTuanChatList);
					this.RuntimeData.JunTuanChatList.Clear();
				}
			}
			if (null != chatList)
			{
				JunTuanClient.getInstance().JunTuanChat(chatList);
			}
		}

		// Token: 0x06000CEF RID: 3311 RVA: 0x000C9F08 File Offset: 0x000C8108
		public void OnChatListData(byte[] data)
		{
			if (null != data)
			{
				List<KFChat> chatList = DataHelper.BytesToObject<List<KFChat>>(data, 0, data.Length);
				if (null != chatList)
				{
					foreach (KFChat kfChat in chatList)
					{
						this.BroadcastJunTuanChatMsg(kfChat);
					}
				}
			}
		}

		// Token: 0x06000CF0 RID: 3312 RVA: 0x000C9F80 File Offset: 0x000C8180
		public void BroadcastJunTuanChatMsg(KFChat kfChat)
		{
			foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
			{
				if (client.ClientData.JunTuanId == kfChat.JunTuanId)
				{
					client.sendCmd(157, kfChat.Text, false);
				}
			}
		}

		// Token: 0x06000CF1 RID: 3313 RVA: 0x000CA008 File Offset: 0x000C8208
		public void JunTuanZhiWuChanged(GameClient client, JunTuanBangHuiMiniData data)
		{
			client.ClientData.JunTuanId = data.JunTuanId;
			client.ClientData.JunTuanName = data.JunTuanName;
			client.ClientData.JunTuanZhiWu = data.JunTuanZhiWu;
			client.ClientData.LingDi = data.LingDi;
			data.RoleId = client.ClientData.RoleID;
			GameManager.ClientMgr.BroadcastOthersCmdData<JunTuanBangHuiMiniData>(client, 1225, data, true);
			LingDiCaiJiManager.getInstance().UpdateChengHaoBuff(client);
			EraManager.getInstance().OnJunTuanZhiWuChanged(client);
		}

		// Token: 0x06000CF2 RID: 3314 RVA: 0x000CA09C File Offset: 0x000C829C
		public bool OnBangHuiMemberChanged(GameClient client, int bhid)
		{
			int serverId = client.ServerId;
			BangHuiDetailData bhData = Global.GetBangHuiDetailData(client.ClientData.RoleID, bhid, serverId);
			bool result;
			if (null == bhData)
			{
				result = true;
			}
			else if (bhData.QiLevel < this.RuntimeData.LegionsNeed)
			{
				result = true;
			}
			else
			{
				this.UpdateJunTuanRoleList(bhid, serverId);
				result = true;
			}
			return result;
		}

		// Token: 0x06000CF3 RID: 3315 RVA: 0x000CA104 File Offset: 0x000C8304
		public bool PreRemoveBangHui(GameClient client)
		{
			int bhid = client.ClientData.Faction;
			int serverId = client.ServerId;
			BangHuiDetailData bhData = Global.GetBangHuiDetailData(client.ClientData.RoleID, bhid, serverId);
			return null == bhData || bhData.QiLevel < this.RuntimeData.LegionsNeed || JunTuanClient.getInstance().RemoveBangHui(bhid) >= 0;
		}

		// Token: 0x06000CF4 RID: 3316 RVA: 0x000CA180 File Offset: 0x000C8380
		public void OnRoleChangName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				SafeClientData clientData = Global.GetSafeClientDataFromLocalOrDB(roleId);
				if (clientData != null && clientData.Faction > 0)
				{
					BangHuiDetailData bhData = Global.GetBangHuiDetailData(-1, clientData.Faction, GameManager.ServerId);
					if (bhData != null && bhData.QiLevel >= this.RuntimeData.LegionsNeed)
					{
						this.UpdateJunTuanRoleList(clientData.Faction, GameManager.ServerId);
					}
				}
			}
		}

		// Token: 0x06000CF5 RID: 3317 RVA: 0x000CA210 File Offset: 0x000C8410
		public bool PreRemoveRole(int roleId)
		{
			SafeClientData clientData = Global.GetSafeClientDataFromLocalOrDB(roleId);
			bool result;
			if (clientData == null || clientData.Faction <= 0)
			{
				result = true;
			}
			else
			{
				BangHuiDetailData bhData = Global.GetBangHuiDetailData(-1, clientData.Faction, GameManager.ServerId);
				if (bhData == null || bhData.QiLevel < this.RuntimeData.LegionsNeed)
				{
					result = true;
				}
				else
				{
					List<JunTuanRoleData> roleList = this.GetJunTuanRoleList(clientData.Faction, GameManager.ServerId);
					foreach (JunTuanRoleData roleData in roleList)
					{
						if (roleData.RoleId == roleId)
						{
							if (roleData.JuTuanZhiWu == 1 || roleData.JuTuanZhiWu == 2)
							{
								return false;
							}
						}
					}
					result = (JunTuanClient.getInstance().UpdateRoleDataList(clientData.Faction, roleList) >= 0);
				}
			}
			return result;
		}

		// Token: 0x06000CF6 RID: 3318 RVA: 0x000CA330 File Offset: 0x000C8530
		public void OnBangHuiChangeName(int bhid, string newName)
		{
			BangHuiDetailData bhData = Global.GetBangHuiDetailData(-1, bhid, GameManager.ServerId);
			if (bhData != null && bhData.QiLevel >= this.RuntimeData.LegionsNeed)
			{
				JunTuanClient.getInstance().ChangeBangHuiName(bhid, newName);
			}
		}

		// Token: 0x06000CF7 RID: 3319 RVA: 0x000CA37C File Offset: 0x000C857C
		public void NotifyJunTuanRequest(int junTuanId, bool icon)
		{
			foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
			{
				if (client.ClientData.JunTuanId == junTuanId && client.ClientData.JunTuanZhiWu == 1)
				{
					if (client._IconStateMgr.AddFlushIconState(15005, icon))
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
			}
		}

		// Token: 0x04001471 RID: 5233
		private static JunTuanManager instance = new JunTuanManager();

		// Token: 0x04001472 RID: 5234
		public JunTuanRuntimeData RuntimeData = new JunTuanRuntimeData();
	}
}
