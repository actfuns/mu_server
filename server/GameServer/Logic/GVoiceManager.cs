using System;
using System.Collections.Generic;
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
	// Token: 0x020007AF RID: 1967
	public class GVoiceManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		// Token: 0x0600338F RID: 13199 RVA: 0x002DBA0C File Offset: 0x002D9C0C
		public static GVoiceManager getInstance()
		{
			return GVoiceManager.instance;
		}

		// Token: 0x06003390 RID: 13200 RVA: 0x002DBA24 File Offset: 0x002D9C24
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x06003391 RID: 13201 RVA: 0x002DBA48 File Offset: 0x002D9C48
		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("GVoiceManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 1000);
			return true;
		}

		// Token: 0x06003392 RID: 13202 RVA: 0x002DBA88 File Offset: 0x002D9C88
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1110, 1, 1, GVoiceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1112, 2, 2, GVoiceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1111, 3, 3, GVoiceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(28, GVoiceManager.getInstance());
			return true;
		}

		// Token: 0x06003393 RID: 13203 RVA: 0x002DBAF8 File Offset: 0x002D9CF8
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(28, GVoiceManager.getInstance());
			return true;
		}

		// Token: 0x06003394 RID: 13204 RVA: 0x002DBB20 File Offset: 0x002D9D20
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06003395 RID: 13205 RVA: 0x002DBB34 File Offset: 0x002D9D34
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x06003396 RID: 13206 RVA: 0x002DBB48 File Offset: 0x002D9D48
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1110:
				result = this.ProcessGetGVoiceSceneDataCmd(client, nID, bytes, cmdParams);
				break;
			case 1111:
				result = this.ProcessGVoiceSetRoleListCmd(client, nID, bytes, cmdParams);
				break;
			case 1112:
				result = this.ProcessGVoiceGetRoleListCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x06003397 RID: 13207 RVA: 0x002DBBA4 File Offset: 0x002D9DA4
		public void processEvent(EventObject eventObject)
		{
			int nID = eventObject.getEventType();
			int num = nID;
			if (num == 28)
			{
				OnStartPlayGameEventObject obj = eventObject as OnStartPlayGameEventObject;
				if (null != obj)
				{
					this.OnStartPlayGame(obj.Client);
				}
			}
		}

		// Token: 0x06003398 RID: 13208 RVA: 0x002DBBE2 File Offset: 0x002D9DE2
		public void processEvent(EventObjectEx eventObject)
		{
		}

		// Token: 0x06003399 RID: 13209 RVA: 0x002DBBE8 File Offset: 0x002D9DE8
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					int group = 0;
					this.RuntimeData.SDKGameID = StringEncrypt.Decrypt(GameManager.PlatConfigMgr.GetGameConfigItemStr("gvoice_app_id", ""), "eabcix675u49,/", "3&3i4x4^+-0");
					this.RuntimeData.SDKKey = StringEncrypt.Decrypt(GameManager.PlatConfigMgr.GetGameConfigItemStr("gvoice_app_key", ""), "eabcix675u49,/", "3&3i4x4^+-0");
					this.RuntimeData.VoiceMessage = GameManager.systemParamsList.GetParamValueIntArrayByName("VoiceMessage", ',');
					this.RuntimeData.VoicePowerNum = GameManager.systemParamsList.GetParamValueIntArrayByName("VoicePowerNum", ',');
					this.RuntimeData.MapCode2GVoiceTypeDict.Clear();
					this.RuntimeData.MapCode2GVoiceGroupDict.Clear();
					string str = GameManager.systemParamsList.GetParamValueByName("ZhanMengVoice");
					if (!string.IsNullOrEmpty(str))
					{
						List<List<int>> ls = ConfigHelper.ParserIntArrayList(str, false, '|', ',');
						foreach (List<int> list in ls)
						{
							group++;
							foreach (int mapCode in list)
							{
								this.RuntimeData.MapCode2GVoiceTypeDict[mapCode] = 1;
								this.RuntimeData.MapCode2GVoiceGroupDict[mapCode] = group;
							}
						}
					}
					str = GameManager.systemParamsList.GetParamValueByName("JunTuanVoice");
					if (!string.IsNullOrEmpty(str))
					{
						List<List<int>> ls = ConfigHelper.ParserIntArrayList(str, false, '|', ',');
						foreach (List<int> list in ls)
						{
							group++;
							foreach (int mapCode in list)
							{
								this.RuntimeData.MapCode2GVoiceTypeDict[mapCode] = 2;
								this.RuntimeData.MapCode2GVoiceGroupDict[mapCode] = group;
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

		// Token: 0x0600339A RID: 13210 RVA: 0x002DBF4C File Offset: 0x002DA14C
		public void SendGVoiceInitData(GameClient client)
		{
			client.sendCmd<GVoiceSceneData>(1110, new GVoiceSceneData
			{
				SDKGameID = this.RuntimeData.SDKGameID,
				SDKKey = this.RuntimeData.SDKKey
			}, false);
		}

		// Token: 0x0600339B RID: 13211 RVA: 0x002DBF90 File Offset: 0x002DA190
		public bool ProcessGetGVoiceSceneDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				GVoiceSceneData result = new GVoiceSceneData();
				GVoicePriorityData priorityData = null;
				int bhid = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				int mapCode = client.ClientData.MapCode;
				MapTypes mapType = Global.GetMapType(mapCode);
				lock (this.RuntimeData.Mutex)
				{
					int type;
					if (this.RuntimeData.MapCode2GVoiceTypeDict.TryGetValue(mapCode, out type))
					{
						string priority = null;
						if (type == 1)
						{
							if (!this.RuntimeData.ZhanMengGVoiceDict.TryGetValue(bhid, out priority))
							{
								string[] ret = Global.SendToDB<int>(1112, bhid, client.ServerId);
								if (ret != null && ret.Length >= 1)
								{
									priority = ret[0];
									this.RuntimeData.ZhanMengGVoiceDict[bhid] = priority;
								}
							}
							client.ClientData.GVoicePrioritys = priority;
							priorityData = new GVoicePriorityData
							{
								ID = bhid,
								Type = type,
								RoleIdList = priority
							};
						}
						else if (type == 2)
						{
							if (!this.RuntimeData.JunTuanGVoiceDict.TryGetValue(junTuanId, out priority))
							{
								priority = JunTuanClient.getInstance().GetJunTuanGVoicePrioritys(bhid);
								this.RuntimeData.JunTuanGVoiceDict[junTuanId] = priority;
							}
							client.ClientData.GVoicePrioritys = priority;
							priorityData = new GVoicePriorityData
							{
								ID = junTuanId,
								Type = type,
								RoleIdList = priority
							};
						}
						int group;
						this.RuntimeData.MapCode2GVoiceGroupDict.TryGetValue(mapCode, out group);
						string id;
						if (mapType == MapTypes.Normal)
						{
							id = string.Format("{0}_{1}_{2}", GameManager.ServerId, group, priorityData.ID);
						}
						else
						{
							id = string.Format("{0}_{1}_{2}_{3}", new object[]
							{
								GameManager.ServerId,
								group,
								client.ClientData.FuBenSeqID,
								priorityData.ID
							});
						}
						GVoiceSceneData data;
						if (!this.RuntimeData.FuBenSeqID2RoomName.TryGetValue(id, out data))
						{
							data = new GVoiceSceneData();
							data.RoomName = Guid.NewGuid().ToString("N");
							this.RuntimeData.FuBenSeqID2RoomName[id] = data;
						}
						result.RoomName = data.RoomName;
						result.SDKGameID = this.RuntimeData.SDKGameID;
						result.SDKKey = this.RuntimeData.SDKKey;
					}
				}
				if (priorityData != null)
				{
					client.sendCmd<GVoicePriorityData>(1112, priorityData, false);
				}
				client.sendCmd<GVoiceSceneData>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x0600339C RID: 13212 RVA: 0x002DC304 File Offset: 0x002DA504
		public bool ProcessGVoiceSetRoleListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int type = Global.SafeConvertToInt32(cmdParams[1]);
				string prioritys = cmdParams[2];
				int bhid = client.ClientData.Faction;
				int rid = client.ClientData.RoleID;
				string leader = rid.ToString();
				HashSet<string> rids = new HashSet<string>();
				if (!string.IsNullOrEmpty(prioritys))
				{
					foreach (string str in prioritys.Split(new char[]
					{
						','
					}))
					{
						if (str != leader)
						{
							rids.Add(str);
						}
					}
				}
				prioritys = string.Join(",", rids);
				if (type == 1)
				{
					if (bhid <= 0 || client.ClientData.BHZhiWu != 1)
					{
						result = -1002;
					}
					else if (rids.Count >= this.RuntimeData.VoicePowerNum[0])
					{
						result = -1035;
					}
					else
					{
						Global.sendToDB<string, string>(1111, string.Format("{0}:{1}:{2}", rid, bhid, prioritys), client.ServerId);
						GMCmdData gmCmdData = new GMCmdData
						{
							Fields = new string[]
							{
								"-gvoicepriority",
								1.ToString(),
								bhid.ToString(),
								prioritys
							}
						};
						HuanYingSiYuanClient.getInstance().BroadcastGMCmdData(gmCmdData, 1);
						this.UpdateGVoicePriority(gmCmdData, true);
					}
				}
				else if (type == 2)
				{
					if (client.ClientData.BHZhiWu != 1 || (client.ClientData.JunTuanId <= 0 && client.ClientData.JunTuanZhiWu == 1))
					{
						result = -1024;
					}
					else if (rids.Count >= this.RuntimeData.VoicePowerNum[1])
					{
						result = -1035;
					}
					else
					{
						result = JunTuanClient.getInstance().ChangeJunTuanGVoicePrioritys(bhid, prioritys);
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

		// Token: 0x0600339D RID: 13213 RVA: 0x002DC58C File Offset: 0x002DA78C
		public bool ProcessGVoiceGetRoleListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int type = Global.SafeConvertToInt32(cmdParams[1]);
				int bhid = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				int rid = client.ClientData.RoleID;
				GVoicePriorityData resultData = new GVoicePriorityData();
				resultData.Type = type;
				string priority = "";
				lock (this.RuntimeData.Mutex)
				{
					if (type == 1)
					{
						resultData.ID = bhid;
						if (bhid > 0)
						{
							if (!this.RuntimeData.ZhanMengGVoiceDict.TryGetValue(bhid, out priority))
							{
								priority = Global.sendToDB<string, int>(1112, bhid, client.ServerId);
								this.RuntimeData.ZhanMengGVoiceDict[bhid] = priority;
							}
							client.ClientData.GVoicePrioritys = priority;
						}
					}
					else if (type == 2)
					{
						resultData.ID = client.ClientData.JunTuanId;
						if (client.ClientData.JunTuanId > 0)
						{
							if (!this.RuntimeData.JunTuanGVoiceDict.TryGetValue(junTuanId, out priority))
							{
								priority = JunTuanClient.getInstance().GetJunTuanGVoicePrioritys(bhid);
								this.RuntimeData.JunTuanGVoiceDict[junTuanId] = priority;
							}
							client.ClientData.GVoicePrioritys = priority;
						}
					}
				}
				resultData.RoleIdList = priority;
				client.sendCmd<GVoicePriorityData>(nID, resultData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x0600339E RID: 13214 RVA: 0x002DC790 File Offset: 0x002DA990
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot6) || Global.GetUnionLevel(client, false) >= Global.GetUnionLevel(this.RuntimeData.VoiceMessage[0], this.RuntimeData.VoiceMessage[1], false) || client.ClientData.VipLevel >= this.RuntimeData.VoiceMessage[2];
		}

		// Token: 0x0600339F RID: 13215 RVA: 0x002DC80C File Offset: 0x002DAA0C
		public void OnStartPlayGame(GameClient client)
		{
			int mapCode = client.ClientData.MapCode;
			lock (this.RuntimeData.Mutex)
			{
				int type;
				if (!this.RuntimeData.MapCode2GVoiceTypeDict.TryGetValue(mapCode, out type))
				{
					client.ClientData.GVoiceType = GVoiceTypes.None;
				}
				else
				{
					client.ClientData.GVoiceType = (GVoiceTypes)type;
				}
			}
		}

		// Token: 0x060033A0 RID: 13216 RVA: 0x002DC89C File Offset: 0x002DAA9C
		public void UpdateGVoicePriority(GMCmdData cmdData, bool force = true)
		{
			if (cmdData.Fields.Length >= 4)
			{
				if (!(cmdData.Fields[0] != "-gvoicepriority"))
				{
					int type = Global.SafeConvertToInt32(cmdData.Fields[1]);
					int unitID = Global.SafeConvertToInt32(cmdData.Fields[2]);
					string prioritys = cmdData.Fields[3];
					lock (this.RuntimeData.Mutex)
					{
						if (type == 1)
						{
							this.RuntimeData.ZhanMengGVoiceDict[unitID] = prioritys;
						}
						else if (type == 2)
						{
							this.RuntimeData.JunTuanGVoiceDict[unitID] = prioritys;
						}
					}
					GVoicePriorityData data = new GVoicePriorityData
					{
						ID = unitID,
						Type = type,
						RoleIdList = prioritys
					};
					if (type == 1)
					{
						foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
						{
							if (force || client.ClientData.GVoiceType == GVoiceTypes.ZhanMeng)
							{
								if (client.ClientData.Faction == unitID && client.ClientData.GVoicePrioritys != prioritys)
								{
									client.ClientData.GVoicePrioritys = prioritys;
									client.sendCmd<GVoicePriorityData>(1112, data, false);
								}
							}
						}
					}
					else if (type == 2)
					{
						foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
						{
							if (force || client.ClientData.GVoiceType == GVoiceTypes.JunTuan)
							{
								if (client.ClientData.JunTuanId == unitID && client.ClientData.GVoicePrioritys != prioritys)
								{
									client.ClientData.GVoicePrioritys = prioritys;
									client.sendCmd<GVoicePriorityData>(1112, data, false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060033A1 RID: 13217 RVA: 0x002DCB64 File Offset: 0x002DAD64
		private void TimerProc(object sender, EventArgs e)
		{
			DateTime now = TimeUtil.NowDateTime();
			long nowTicks = TimeUtil.NOW();
			if (this.RuntimeData.NextTicks1 < nowTicks)
			{
				this.RuntimeData.NextTicks1 = nowTicks + 3000L;
			}
		}

		// Token: 0x04003F50 RID: 16208
		private static GVoiceManager instance = new GVoiceManager();

		// Token: 0x04003F51 RID: 16209
		public GVoiceRuntimeData RuntimeData = new GVoiceRuntimeData();
	}
}
