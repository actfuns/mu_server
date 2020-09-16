using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using AutoCSer.Net.TcpServer;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Contract.Data;
using KF.Remoting;
using KF.TcpCall;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	public class ZhanDuiZhengBaManager : IManager, ICmdProcessorEx, ICmdProcessor, IManager2, IEventListener, IEventListenerEx, ICopySceneManager
	{
		
		public static ZhanDuiZhengBaManager getInstance()
		{
			return ZhanDuiZhengBaManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("ZhanDuiZhengBaManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1272, 1, 2, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1273, 1, 2, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1274, 2, 3, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1275, 2, 3, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1276, 1, 2, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1277, 1, 2, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1278, 1, 2, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1279, 0, 0, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1280, 1, 1, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(10, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource.getInstance().registerListener(13, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource.getInstance().registerListener(64, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(61, 10007, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(62, 10007, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(60, 56, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(63, 10000, ZhanDuiZhengBaManager.getInstance());
			this.NotifyEnterHandler = new EventSourceEx<KFCallMsg>.HandlerData
			{
				ID = 56,
				EventType = 10034,
				Handler = new Func<KFCallMsg, bool>(this.KFCallMsgFunc)
			};
			KFCallManager.MsgSource.registerListener(10034, this.NotifyEnterHandler);
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(10, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource.getInstance().removeListener(13, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(61, 10007, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(62, 10007, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(60, 56, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(63, 10000, ZhanDuiZhengBaManager.getInstance());
			KFCallManager.MsgSource.removeListener(10034, this.NotifyEnterHandler);
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
			switch (nID)
			{
			case 1272:
				return this.ProcessGetMainInfoListCmd(client, nID, bytes, cmdParams);
			case 1273:
				return this.ProcessGetZhanDuiListCmd(client, nID, bytes, cmdParams);
			case 1275:
				return this.ProcessSupportCmd(client, nID, bytes, cmdParams);
			case 1276:
				return this.ProcessGetLogCmd(client, nID, bytes, cmdParams);
			case 1277:
				return this.ProcessZhanDuiZhengBaEnterCmd(client, nID, bytes, cmdParams);
			case 1280:
				return this.ProcessSupportListCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 10)
			{
				PlayerDeadEventObject playerDeadEvent = eventObject as PlayerDeadEventObject;
				if (null != playerDeadEvent)
				{
					if (playerDeadEvent.Type == PlayerDeadEventTypes.ByRole)
					{
						this.OnKillRole(playerDeadEvent.getAttackerRole(), playerDeadEvent.getPlayer());
					}
				}
			}
			else if (eventType == 28)
			{
				OnStartPlayGameEventObject e = eventObject as OnStartPlayGameEventObject;
				if (null != e)
				{
					this.UpdateChengHaoBuffer(e.Client);
					this.GiveSupportAwards(e.Client);
					this.GiveRankAwards(e.Client);
				}
			}
			else if (eventObject.getEventType() == 13)
			{
				PlayerLeaveFuBenEventObject eventObj = (PlayerLeaveFuBenEventObject)eventObject;
				this.RoleLeaveFuBen(eventObj.getPlayer());
			}
			else if (eventType == 64)
			{
				this.UpdateChengHaoBuffer(eventObject.Params[0] as GameClient);
			}
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 60:
				this.NotifyTimeStateInfoAndScoreInfo(eventObject.Sender as GameClient);
				break;
			case 61:
			{
				EventObjectEx_I1 data = eventObject as EventObjectEx_I1;
				if (data != null && data.Param1 == 35)
				{
					eventObject.Handled = true;
					if (this.OnKuaFuLogin(eventObject.Sender as KuaFuServerLoginData))
					{
						eventObject.Result = true;
					}
				}
				break;
			}
			case 62:
			{
				EventObjectEx_I1 data = eventObject as EventObjectEx_I1;
				if (data != null && data.Param1 == 35)
				{
					eventObject.Handled = true;
					if (this.OnKuaFuInitGame(eventObject.Sender as GameClient))
					{
						eventObject.Handled = true;
						eventObject.Result = true;
					}
				}
				break;
			}
			case 63:
			{
				PreZhanDuiChangeMemberEventObject eventObj = (PreZhanDuiChangeMemberEventObject)eventObject;
				eventObj.Handled = this.OnPreZhanDuiChangeMember(eventObj);
				break;
			}
			}
		}

		
		public bool KFCallMsgFunc(KFCallMsg msg)
		{
			int kuaFuEventType = msg.KuaFuEventType;
			if (kuaFuEventType == 10034)
			{
				ZhanDuiZhengBaNtfEnterData data = msg.Get<ZhanDuiZhengBaNtfEnterData>();
				if (null != data)
				{
					GameManager.ClientMgr.BroadZhanDuiMessage<int>(1281, 1, data.ZhanDuiID1);
					if (data.ZhanDuiID2 != data.ZhanDuiID1)
					{
						GameManager.ClientMgr.BroadZhanDuiMessage<int>(1281, 1, data.ZhanDuiID2);
					}
				}
			}
			return true;
		}

		
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.TeamBattleName = GameManager.systemParamsList.GetParamValueIntArrayByName("TeamBattleName", ',');
					if (!this.RuntimeData.Config.Load(Global.GameResPath("Config\\TeamMatch.xml"), Global.GameResPath("Config\\TeamMatchBirthPoint.xml")))
					{
						return false;
					}
					List<ZhanDuiZhengBaAwardsConfig> awardsConfigList = new List<ZhanDuiZhengBaAwardsConfig>();
					fileName = "Config/TeamMatchAward.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						ZhanDuiZhengBaAwardsConfig item = new ZhanDuiZhengBaAwardsConfig();
						item.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item.Rank = (int)ConfigHelper.GetElementAttributeValueLong(node, "Rank", 0L);
						item.TeamPoint = (int)ConfigHelper.GetElementAttributeValueLong(node, "TeamPoint", 0L);
						string str = Global.GetSafeAttributeStr(node, "Award");
						if (!string.IsNullOrEmpty(str))
						{
							ConfigParser.ParseAwardsItemList(str, ref item.Award, '|', ',');
						}
						awardsConfigList.Add(item);
					}
					this.RuntimeData.AwardsConfig = awardsConfigList;
					this.RuntimeData.StartTime = this.RuntimeData.Config.MatchConfigList[0].TimePoints[0];
				}
				catch (Exception ex)
				{
					success = false;
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		
		public bool ProcessGetMainInfoListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				long age = 0L;
				if (cmdParams.Length >= 2)
				{
					age = Convert.ToInt64(cmdParams[1]);
				}
				AgeDataT<List<ZhanDuiZhengBaZhanDuiData>> result = new AgeDataT<List<ZhanDuiZhengBaZhanDuiData>>();
				lock (this.RuntimeData.Mutex)
				{
					if (age != this.RuntimeData.SyncData.RoleModTime.Ticks)
					{
						result.Age = this.RuntimeData.SyncData.RoleModTime.Ticks;
						if (this.RuntimeData.SyncData.RealActID >= 2 || this.RuntimeData.SyncData.HasSeasonEnd)
						{
							List<ZhanDuiZhengBaZhanDuiData> list = this.RuntimeData.SyncData.ZhanDuiList;
							if (null != list)
							{
								list.Sort(delegate(ZhanDuiZhengBaZhanDuiData x, ZhanDuiZhengBaZhanDuiData y)
								{
									int result2;
									if (x.Grade < y.Grade)
									{
										result2 = -1;
									}
									else
									{
										result2 = 1;
									}
									return result2;
								});
								result.V = list.TakeWhile((ZhanDuiZhengBaZhanDuiData x) => x.Grade <= 16).ToList<ZhanDuiZhengBaZhanDuiData>();
							}
						}
					}
				}
				client.sendCmd<AgeDataT<List<ZhanDuiZhengBaZhanDuiData>>>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessGetZhanDuiListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<ZhanDuiZhengBaZhanDuiData> result = new List<ZhanDuiZhengBaZhanDuiData>();
				lock (this.RuntimeData.Mutex)
				{
					List<ZhanDuiZhengBaZhanDuiData> list = this.RuntimeData.SyncData.ZhanDuiList;
					if (null != list)
					{
						result = list.GetRange(0, list.Count);
					}
				}
				client.sendCmd<List<ZhanDuiZhengBaZhanDuiData>>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessSupportCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int zhanDuiID = Global.SafeConvertToInt32(cmdParams[1]);
				DateTime now = TimeUtil.NowDateTime();
				long monthAndGetFlags = Global.GetRoleParamsInt64FromDB(client, "10221");
				int month = (int)(monthAndGetFlags >> 32);
				int getFlags = (int)(monthAndGetFlags & (long) 0xffff_ffffUL);
				int[] ids = new int[2];
				if (month == this.RuntimeData.SyncData.Month)
				{
					long zhanDuiIDs = Global.GetRoleParamsInt64FromDB(client, "10222");
					ids[0] = (int)(zhanDuiIDs >> 32);
					ids[1] = (int)(zhanDuiIDs & (long) 0xffff_ffffUL);
				}
				else
				{
					getFlags = 0;
				}
				if (ids[0] == zhanDuiID || ids[1] == zhanDuiID)
				{
					result = -4032;
				}
				else if (ids[0] > 0 && ids[1] > 0)
				{
					result = -4033;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						int nowActivityMonth = this.RuntimeData.SyncData.Month;
						if (nowActivityMonth <= 0)
						{
							result = -400;
							goto IL_2DE;
						}
						List<ZhanDuiZhengBaZhanDuiData> list = this.RuntimeData.SyncData.ZhanDuiList;
						if (list == null || list.Count == 0)
						{
							result = -400;
							goto IL_2DE;
						}
						if (!list.Any((ZhanDuiZhengBaZhanDuiData x) => x.ZhanDuiID == zhanDuiID))
						{
							result = -4031;
							goto IL_2DE;
						}
						ZhanDuiZhengBaMatchConfig config = this.RuntimeData.Config.MatchConfigList.First<ZhanDuiZhengBaMatchConfig>();
						TimeSpan ts = new TimeSpan(now.Day, now.Hour, now.Minute, now.Second);
						if (ts < config.LotteryTime[0] || ts > config.LotteryTime[1])
						{
							result = -2001;
							goto IL_2DE;
						}
						if (!MoneyUtil.CheckHasMoney(client, 8, config.LotteryMoney))
						{
							result = -9;
							goto IL_2DE;
						}
						string str = "";
						MoneyUtil.CostMoney(client, 8, config.LotteryMoney, ref str, "战队争霸押注", true);
						if (ids[0] == 0)
						{
							ids[0] = zhanDuiID;
						}
						else if (ids[1] == 0)
						{
							ids[1] = zhanDuiID;
						}
						monthAndGetFlags = ((long)nowActivityMonth << 32) + (long)getFlags;
					}
					long zhanDuiIDs = ((long)ids[0] << 32) + (long)ids[1];
					Global.SaveRoleParamsInt64ValueToDB(client, "10221", monthAndGetFlags, true);
					Global.SaveRoleParamsInt64ValueToDB(client, "10222", zhanDuiIDs, true);
				}
				IL_2DE:
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessSupportListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<ZhanDuiZhengBaZhanDuiData> result = new List<ZhanDuiZhengBaZhanDuiData>();
				long monthAndGetFlags = Global.GetRoleParamsInt64FromDB(client, "10221");
				int month = (int)(monthAndGetFlags >> 32);
				int[] ids = new int[2];
				if (month == this.RuntimeData.SyncData.Month)
				{
					long zhanDuiIDs = Global.GetRoleParamsInt64FromDB(client, "10222");
					ids[0] = (int)(zhanDuiIDs >> 32);
					ids[1] = (int)(zhanDuiIDs & (long) 0xffff_ffffUL);
					lock (this.RuntimeData.Mutex)
					{
						List<ZhanDuiZhengBaZhanDuiData> list = this.RuntimeData.SyncData.ZhanDuiList;
						if (null != list)
						{
							foreach (ZhanDuiZhengBaZhanDuiData data in list)
							{
								if (data.ZhanDuiID == ids[0] || data.ZhanDuiID == ids[1])
								{
									result.Add(data);
								}
							}
						}
					}
				}
				client.sendCmd<List<ZhanDuiZhengBaZhanDuiData>>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessGetLogCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<ZhanDuiZhengBaPkLogData> result = new List<ZhanDuiZhengBaPkLogData>();
				lock (this.RuntimeData.Mutex)
				{
					List<ZhanDuiZhengBaPkLogData> list = this.RuntimeData.SyncData.PKLogList;
					if (null != list)
					{
						result = list.GetRange(0, list.Count);
					}
				}
				client.sendCmd<List<ZhanDuiZhengBaPkLogData>>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessZhanDuiZhengBaEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				if (!this.IsGongNengOpened(client, false))
				{
					result = -12;
				}
				else if (!this.CheckMap(client))
				{
					result = -12;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.SyncData.ZhanDuiList.Any((ZhanDuiZhengBaZhanDuiData x) => x.ZhanDuiID == client.ClientData.ZhanDuiID))
						{
							result = -12;
							goto IL_1F3;
						}
					}
					int gameID;
					int kuafuServerID;
					string[] ips;
					int[] ports;
					ReturnValue<int> rt = TcpCall.ZhanDuiZhengBa_K.ZhengBaRequestEnter(client.ClientData.ZhanDuiID, out gameID, out kuafuServerID, out ips, out ports);
					if (rt.Type != ReturnType.Success || rt.Value < 0)
					{
						result = rt;
					}
					else
					{
						long monthAndGetFlags = (long)this.RuntimeData.SyncData.Month << 32;
						Global.SaveRoleParamsInt64ValueToDB(client, "10225", monthAndGetFlags, true);
						KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
						kuaFuServerLoginData.RoleId = client.ClientData.RoleID;
						kuaFuServerLoginData.ServerId = client.ServerId;
						kuaFuServerLoginData.GameType = 35;
						kuaFuServerLoginData.GameId = (long)gameID;
						kuaFuServerLoginData.EndTicks = TimeUtil.UTCTicks();
						kuaFuServerLoginData.TargetServerID = kuafuServerID;
						kuaFuServerLoginData.ServerIp = ips[0];
						kuaFuServerLoginData.ServerPort = ports[0];
						kuaFuServerLoginData.Param1 = client.ClientData.ZhanDuiID;
						GlobalNew.RecordSwitchKuaFuServerLog(client);
						client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
					}
				}
				IL_1F3:
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, GongNengIDs.ZhanDuiZhengBa, false);
		}

		
		private bool CheckMap(GameClient client)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return sceneType == SceneUIClasses.Normal;
		}

		
		private bool GetBirthPoint(int mapCode, int side, out int toPosX, out int toPosY)
		{
			toPosX = -1;
			toPosY = -1;
			GameMap gameMap = null;
			bool result;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
			{
				result = false;
			}
			else
			{
				int defaultBirthPosX = this.RuntimeData.Config.BirthPointList[side % this.RuntimeData.Config.BirthPointList.Count].X;
				int defaultBirthPosY = this.RuntimeData.Config.BirthPointList[side % this.RuntimeData.Config.BirthPointList.Count].Y;
				int defaultBirthRadius = this.RuntimeData.Config.BirthPointList[side % this.RuntimeData.Config.BirthPointList.Count].Radius;
				Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, mapCode, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
				toPosX = (int)newPos.X;
				toPosY = (int)newPos.Y;
				result = true;
			}
			return result;
		}

		
		private bool OnKuaFuLogin(KuaFuServerLoginData data)
		{
			ZhanDuiZhengBaFuBenData copyInfo = null;
			int zhanDuiID = data.Param1;
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.KuaFuCopyDataDict.TryGetValue(data.GameId, out copyInfo);
			}
			if (null == copyInfo)
			{
				ReturnValue<int> rt = TcpCall.ZhanDuiZhengBa_K.ZhengBaKuaFuLogin(zhanDuiID, (int)data.GameId, data.ServerId, out copyInfo);
				if (!rt.IsReturn || rt.Value < 0)
				{
					return false;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.KuaFuCopyDataDict.ContainsKey(data.GameId))
					{
						this.RuntimeData.KuaFuCopyDataDict[data.GameId] = copyInfo;
					}
				}
			}
			bool result;
			if (copyInfo != null && GameManager.ServerId == copyInfo.ServerID && copyInfo.SideDict.ContainsKey((long)zhanDuiID))
			{
				data.ips = copyInfo.IPs;
				data.ports = copyInfo.Ports;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public bool OnKuaFuInitGame(GameClient client)
		{
			int zhanDuiID = client.ClientData.ZhanDuiID;
			int gameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
			bool result;
			if (gameId <= 0 || zhanDuiID <= 0)
			{
				result = false;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					ZhanDuiZhengBaMatchConfig matchConfig = this.RuntimeData.Config.MatchConfigList.First<ZhanDuiZhengBaMatchConfig>();
					ZhanDuiZhengBaFuBenData copyInfo = null;
					if (!this.RuntimeData.KuaFuCopyDataDict.TryGetValue((long)gameId, out copyInfo))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("未找到活动KuaFuCopyData数据,roleid={0},gameid={1},mapcode={2]", client.ClientData.RoleID, gameId, matchConfig.MapCode), null, true);
						result = false;
					}
					else
					{
						if (copyInfo.FuBenSeqID == 0)
						{
							copyInfo.FuBenSeqID = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						}
						ZhanDuiZhengBaScene scene = null;
						if (this.ZhanDuiZhengBaSceneDict.TryGetValue(copyInfo.FuBenSeqID, out scene) && scene.m_eStatus >= GameSceneStatuses.STATUS_BEGIN)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("当前场次战队争霸已经过准备时间,拒绝进入,roleid={0},gameid={1},mapcode={2]", client.ClientData.RoleID, gameId, matchConfig.MapCode), null, true);
							client.ClientData.PushMessageID = GLang.GetLang(8012, new object[0]);
							result = false;
						}
						else
						{
							int toX = 0;
							int toY = 0;
							int side = 0;
							if (!copyInfo.SideDict.TryGetValue((long)client.ClientData.ZhanDuiID, out side))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("未找到活动阵营数据KuaFuCopyData,roleid={0},gameid={1},mapcode={2]", client.ClientData.RoleID, gameId, matchConfig.MapCode), null, true);
								result = false;
							}
							else if (!this.GetBirthPoint(matchConfig.MapCode, side, out toX, out toY))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("roleid={0},mapcode={1},side={2} 未找到出生点", client.ClientData.RoleID, matchConfig.MapCode, side), null, true);
								result = false;
							}
							else
							{
								Global.GetClientKuaFuServerLoginData(client).FuBenSeqId = copyInfo.FuBenSeqID;
								client.ClientData.MapCode = matchConfig.MapCode;
								client.ClientData.PosX = toX;
								client.ClientData.PosY = toY;
								client.ClientData.FuBenSeqID = copyInfo.FuBenSeqID;
								client.ClientData.BattleWhichSide = side;
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		
		public void TimerProc(object sender, EventArgs e)
		{
			long nowTicks = TimeUtil.NOW();
			bool hasSeasonEnd = false;
			while (this.RuntimeData.SyncDataByTime.RunByInterval(nowTicks))
			{
				ZhanDuiZhengBaSyncData syncDataRequest = this.RuntimeData.SyncDataRequest;
				ReturnValue<ZhanDuiZhengBaSyncData> result = TcpCall.ZhanDuiZhengBa_K.SyncZhengBaData(syncDataRequest);
				if (!result.IsReturn)
				{
					break;
				}
				ZhanDuiZhengBaSyncData syncData = result.Value;
				if (syncData == null)
				{
					break;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (syncDataRequest != this.RuntimeData.SyncDataRequest)
					{
						break;
					}
					this.RuntimeData.SyncData.Month = syncData.Month;
					this.RuntimeData.SyncData.RealActID = syncData.RealActID;
					this.RuntimeData.SyncData.IsThisMonthInActivity = syncData.IsThisMonthInActivity;
					this.RuntimeData.SyncData.CenterTime = syncData.CenterTime;
					this.RuntimeData.SyncData.TopZhanDui = syncData.TopZhanDui;
					if (!this.RuntimeData.SyncDataRequest.HasSeasonEnd && syncData.HasSeasonEnd)
					{
						hasSeasonEnd = true;
					}
					this.RuntimeData.SyncData.HasSeasonEnd = syncData.HasSeasonEnd;
					this.RuntimeData.SyncDataRequest.HasSeasonEnd = syncData.HasSeasonEnd;
					if (syncData.RoleModTime != syncDataRequest.RoleModTime)
					{
						this.RuntimeData.SyncData.RoleModTime = syncData.RoleModTime;
						this.RuntimeData.SyncDataRequest.RoleModTime = syncData.RoleModTime;
						this.RuntimeData.SyncData.ZhanDuiList = syncData.ZhanDuiList;
					}
					if (syncData.PKLogModTime != syncDataRequest.PKLogModTime)
					{
						this.RuntimeData.SyncData.PKLogModTime = syncData.PKLogModTime;
						this.RuntimeData.SyncDataRequest.PKLogModTime = syncData.PKLogModTime;
						this.RuntimeData.SyncData.PKLogList = syncData.PKLogList;
					}
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < this.RuntimeData.PKResultQueue.Count; i++)
				{
					Tuple<int, int, int> pkResult = this.RuntimeData.PKResultQueue.Peek();
					if (TcpCall.ZhanDuiZhengBa_K.ZhengBaPkResult(pkResult.Item1, pkResult.Item2).Type != ReturnType.Success)
					{
						break;
					}
					this.RuntimeData.PKResultQueue.Dequeue();
				}
			}
			if (hasSeasonEnd)
			{
				this.GiveSupportAwards();
				foreach (GameClient client in GameManager.ClientMgr.GetAllClients(false))
				{
					this.UpdateChengHaoBuffer(client);
				}
			}
		}

		
		private void GiveSupportAwards()
		{
			foreach (GameClient client in GameManager.ClientMgr.GetAllClients(false))
			{
				this.GiveSupportAwards(client);
				this.GiveRankAwards(client);
			}
		}

		
		private void GiveSupportAwards(GameClient client)
		{
			try
			{
				if (this.RuntimeData.SyncData.HasSeasonEnd)
				{
					long monthAndGetFlags = Global.GetRoleParamsInt64FromDB(client, "10221");
					int month = (int)(monthAndGetFlags >> 32);
					int getFlags = (int)(monthAndGetFlags & (long) 0xffff_ffffUL);
					int[] ids = new int[2];
					if (getFlags == 0 && month == this.RuntimeData.SyncData.Month)
					{
						long zhanDuiIDs = Global.GetRoleParamsInt64FromDB(client, "10222");
						ids[0] = (int)(zhanDuiIDs >> 32);
						ids[1] = (int)(zhanDuiIDs & (long) 0xffff_ffffUL);
						int totalLotteryMoney = 0;
						lock (this.RuntimeData.Mutex)
						{
							List<ZhanDuiZhengBaZhanDuiData> list = this.RuntimeData.SyncData.ZhanDuiList;
							if (null != list)
							{
								using (List<ZhanDuiZhengBaZhanDuiData>.Enumerator enumerator = list.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										ZhanDuiZhengBaZhanDuiData data = enumerator.Current;
										if (data.ZhanDuiID == ids[0] || data.ZhanDuiID == ids[1])
										{
											ZhanDuiZhengBaAwardsConfig config = this.RuntimeData.AwardsConfig.Find((ZhanDuiZhengBaAwardsConfig x) => x.Rank == data.Grade);
											if (config != null)
											{
												totalLotteryMoney += config.TeamPoint;
											}
										}
									}
								}
							}
						}
						getFlags = 1;
						monthAndGetFlags = ((long)month << 32) + (long)getFlags;
						Global.SaveRoleParamsInt64ValueToDB(client, "10221", monthAndGetFlags, true);
						GameManager.ClientMgr.ModifyTeamPointValue(client, totalLotteryMoney, "战队争霸押注奖励", false);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		private void GiveRankAwards(GameClient client)
		{
			try
			{
				if (this.RuntimeData.SyncData.HasSeasonEnd)
				{
					ZhanDuiZhengBaAwardsConfig awardsData = null;
					long monthAndGetFlags = Global.GetRoleParamsInt64FromDB(client, "10225");
					int month = (int)(monthAndGetFlags >> 32);
					int getFlags = (int)(monthAndGetFlags & (long) 0xffff_ffffUL);
					int[] ids = new int[2];
					int grade = 0;
					if (getFlags == 0 && month == this.RuntimeData.SyncData.Month)
					{
						lock (this.RuntimeData.Mutex)
						{
							List<ZhanDuiZhengBaZhanDuiData> list = this.RuntimeData.SyncData.ZhanDuiList;
							if (null != list)
							{
								foreach (ZhanDuiZhengBaZhanDuiData data in list)
								{
									if (data.ZhanDuiID == client.ClientData.ZhanDuiID)
									{
										grade = data.Grade;
										break;
									}
								}
							}
							if (grade > 0)
							{
								awardsData = this.RuntimeData.AwardsConfig.Find((ZhanDuiZhengBaAwardsConfig x) => x.Rank == grade);
							}
						}
						if (awardsData != null && Global.CanAddGoodsNum(client, awardsData.Award.Items.Count))
						{
							foreach (AwardsItemData item in awardsData.Award.Items)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, "战盟联赛排行榜奖励", "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
							}
						}
						else
						{
							Global.UseMailGivePlayerAward2(client, awardsData.Award.Items, GLang.GetLang(8011, new object[0]), GLang.GetLang(8011, new object[0]), 0, 0, 0);
						}
						getFlags = 1;
						monthAndGetFlags = ((long)month << 32) + (long)getFlags;
						Global.SaveRoleParamsInt64ValueToDB(client, "10225", monthAndGetFlags, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public void UpdateChengHaoBuffer(GameClient client)
		{
			TimeSpan ts = TimeUtil.GetTimeOfWeekNow2();
			int secs = (int)(this.RuntimeData.StartTime - ts).TotalSeconds % 604800;
			if (secs < 0)
			{
				secs += 604800;
			}
			int bufferID = FashionManager.getInstance().GetBufferIDBySpecialTitleID(this.RuntimeData.TeamBattleName[0]);
			if (this.RuntimeData.SyncData.TopZhanDui > 0 && client.ClientData.ZhanDuiID == this.RuntimeData.SyncData.TopZhanDui && secs > 3)
			{
				Global.UpdateBufferDataTitle(client, bufferID, 1L, TimeUtil.NOW(), secs);
			}
			else
			{
				Global.UpdateBufferDataTitle(client, bufferID, 0L, 0L, 0);
			}
		}

		
		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.ZhanDuiZhengBa)
			{
				int fuBenSeqId = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (this.RuntimeData.Mutex)
				{
					long ticks = TimeUtil.NOW();
					ZhanDuiZhengBaScene scene = null;
					if (!this.ZhanDuiZhengBaSceneDict.TryGetValue(fuBenSeqId, out scene))
					{
						scene = new ZhanDuiZhengBaScene();
						scene.CopyMap = copyMap;
						scene.CleanAllInfo();
						scene.GameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
						scene.m_nMapCode = mapCode;
						scene.CopyMapId = copyMap.CopyMapID;
						scene.FuBenSeqId = fuBenSeqId;
						ZhanDuiZhengBaFuBenData fuBenData;
						if (this.RuntimeData.KuaFuCopyDataDict.TryGetValue((long)scene.GameId, out fuBenData))
						{
							scene.FuBenData = fuBenData;
						}
						scene.SceneConfig = this.RuntimeData.Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig x) => x.ID == fuBenData.ConfigID);
						this.ZhanDuiZhengBaSceneDict[fuBenSeqId] = scene;
						scene.m_lPrepareTime = ticks;
						scene.m_lBeginTime = ticks + (long)(scene.SceneConfig.WaitSeconds * 1000);
						scene.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
						scene.StateTimeData.GameType = 35;
						scene.StateTimeData.State = (int)scene.m_eStatus;
						scene.StateTimeData.EndTicks = scene.m_lBeginTime;
					}
					copyMap.IsKuaFuCopy = true;
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(this.RuntimeData.TotalSecs * 1000));
					scene.RoleSideStateDict[client.ClientData.RoleID] = new Tuple<int, bool>(client.ClientData.ZhanDuiID, true);
					scene.ClientDict[client.ClientData.RoleID] = client;
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.ZhanDuiZhengBa)
			{
				lock (this.RuntimeData.Mutex)
				{
					ZhanDuiZhengBaScene scene;
					if (this.ZhanDuiZhengBaSceneDict.TryRemove(copyMap.FuBenSeqID, out scene))
					{
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= ZhanDuiZhengBaManager.NextHeartBeatTicks)
			{
				ZhanDuiZhengBaManager.NextHeartBeatTicks = nowTicks + 1020L;
				foreach (ZhanDuiZhengBaScene scene in this.ZhanDuiZhengBaSceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int nID = scene.FuBenSeqId;
						int nCopyID = scene.CopyMapId;
						int nMapCodeID = scene.m_nMapCode;
						if (nID >= 0 && nCopyID >= 0 && nMapCodeID >= 0)
						{
							CopyMap copyMap = scene.CopyMap;
							DateTime now = TimeUtil.NowDateTime();
							long ticks = TimeUtil.NOW();
							scene.ScoreInfoData.Count1 = 0L;
							scene.ScoreInfoData.Count2 = 0;
							List<KeyValuePair<int, Tuple<int, bool>>> updateList = new List<KeyValuePair<int, Tuple<int, bool>>>();
							foreach (KeyValuePair<int, Tuple<int, bool>> kv in scene.RoleSideStateDict.ToList<KeyValuePair<int, Tuple<int, bool>>>())
							{
								if (kv.Value.Item2)
								{
									int rid = kv.Key;
									Tuple<int, bool> v = kv.Value;
									GameClient gc = GameManager.ClientMgr.FindClient(rid);
									if (null == gc)
									{
										if (v.Item2)
										{
											updateList.Add(new KeyValuePair<int, Tuple<int, bool>>(rid, new Tuple<int, bool>(v.Item1, false)));
										}
									}
									else if (!gc.ClientData.FirstPlayStart)
									{
										int side = 0;
										if (scene.FuBenData.SideDict.TryGetValue((long)v.Item1, out side))
										{
											if (side == 1)
											{
												scene.ScoreInfoData.Count1 += 1L;
											}
											else if (side == 2)
											{
												scene.ScoreInfoData.Count2++;
											}
										}
									}
								}
							}
							foreach (KeyValuePair<int, Tuple<int, bool>> kv in updateList)
							{
								scene.RoleSideStateDict[kv.Key] = kv.Value;
							}
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<ZhanDuiZhengBaScoreInfoData>(1278, scene.ScoreInfoData, copyMap);
							if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
							{
								bool gotoNextStep = false;
								if (ticks >= scene.m_lBeginTime)
								{
									gotoNextStep = true;
								}
								else
								{
									bool flag2;
									if (scene.RoleSideStateDict.Count >= scene.FuBenData.RoleDict.Count)
									{
										flag2 = !copyMap.GetClientsList().All((GameClient x) => !x.ClientData.FirstPlayStart);
									}
									else
									{
										flag2 = true;
									}
									if (!flag2)
									{
										gotoNextStep = true;
									}
								}
								if (gotoNextStep)
								{
									scene.m_lBeginTime = ticks;
									scene.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
									scene.m_lEndTime = ticks + (long)(scene.SceneConfig.FightSeconds * 1000);
									scene.StateTimeData.GameType = 35;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
									copyMap.AddGuangMuEvent(1, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 1, 0);
									copyMap.AddGuangMuEvent(2, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 2, 0);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (ticks >= scene.m_lEndTime)
								{
									this.CompleteZhanDuiZhengBaScene(scene, scene.FuBenData.BetterZhanDuiID);
								}
								else if (ticks - scene.m_lBeginTime > 1000L)
								{
									this.SceneCheckComplete(scene, true);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_END)
							{
								this.ProcessEnd(scene, now, nowTicks);
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_AWARD)
							{
								if (ticks >= scene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(scene.m_lLeaveTime);
									scene.m_eStatus = GameSceneStatuses.STATUS_CLEAR;
									KuaFuManager.getInstance().ClearCopyMapClients(copyMap);
								}
							}
						}
					}
				}
			}
		}

		
		public void NotifyTimeStateInfoAndScoreInfo(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				ZhanDuiZhengBaScene scene;
				if (this.ZhanDuiZhengBaSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
				}
			}
		}

		
		public void CompleteZhanDuiZhengBaScene(ZhanDuiZhengBaScene scene, int successSide)
		{
			scene.m_eStatus = GameSceneStatuses.STATUS_END;
			scene.SuccessSide = successSide;
		}

		
		private int SceneCheckComplete(ZhanDuiZhengBaScene scene, bool complete = true)
		{
			int side = 0;
			if (scene.RoleSideStateDict.Count > 0)
			{
				foreach (Tuple<int, bool> tp in scene.RoleSideStateDict.Values)
				{
					if (tp.Item2)
					{
						if (side == 0)
						{
							side = tp.Item1;
						}
						else if (side != tp.Item1)
						{
							side = 0;
							break;
						}
					}
				}
			}
			else
			{
				side = scene.LastLeaveZhanDuiID;
			}
			if (side != 0 && complete)
			{
				this.CompleteZhanDuiZhengBaScene(scene, side);
			}
			return side;
		}

		
		private void SceneRemoveRole(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				ZhanDuiZhengBaScene scene;
				if (this.ZhanDuiZhengBaSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (scene.m_eStatus < GameSceneStatuses.STATUS_END)
					{
						scene.RoleSideStateDict[client.ClientData.RoleID] = new Tuple<int, bool>(client.ClientData.ZhanDuiID, false);
						if (scene.RoleSideStateDict.Count((KeyValuePair<int, Tuple<int, bool>> x) => x.Value.Item2) == 0)
						{
							scene.LastLeaveZhanDuiID = client.ClientData.ZhanDuiID;
						}
					}
				}
			}
		}

		
		public void OnKillRole(GameClient client, GameClient other)
		{
			if (client.SceneType == 56)
			{
				this.SceneRemoveRole(other);
				GameManager.ClientMgr.ChangePosition(TCPManager.getInstance().MySocketListener, TCPManager.getInstance().TcpOutPacketPool, other, this.RuntimeData.TeamBattleWatch[0], this.RuntimeData.TeamBattleWatch[1], 4, 159, 0);
			}
		}

		
		public void RoleLeaveFuBen(GameClient client)
		{
			if (client.SceneType == 56)
			{
				this.SceneRemoveRole(client);
			}
		}

		
		private void ProcessEnd(ZhanDuiZhengBaScene scene, DateTime now, long nowTicks)
		{
			scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
			scene.m_lEndTime = nowTicks;
			scene.m_lLeaveTime = scene.m_lEndTime + (long)(scene.SceneConfig.ClearSeconds * 1000);
			this.RuntimeData.PKResultQueue.Enqueue(new Tuple<int, int, int>(scene.GameId, scene.SuccessSide, 0));
			scene.StateTimeData.GameType = 35;
			scene.StateTimeData.State = 3;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
			this.GiveAwards(scene);
		}

		
		public void GiveAwards(ZhanDuiZhengBaScene scene)
		{
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				List<GameClient> objsList = scene.ClientDict.Values.ToList<GameClient>();
				HashSet<int> processedZhanDuiHashSet = new HashSet<int>();
				if (objsList != null && objsList.Count > 0)
				{
					int nowDayId = Global.GetOffsetDayNow();
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient client = objsList[i];
						if (client != null)
						{
							bool online = false;
							GameClient c = GameManager.ClientMgr.FindClient(client.ClientData.RoleID);
							if (c != null && c.SceneType == 56)
							{
								online = true;
							}
							ZhanDuiZhengBaAwardsData awardsData = new ZhanDuiZhengBaAwardsData();
							if (client.ClientData.ZhanDuiID == scene.SuccessSide)
							{
								awardsData.Success = 1;
								awardsData.NewGrade = scene.FuBenData.NewGrade;
							}
							else
							{
								awardsData.NewGrade = scene.FuBenData.JoinGrade;
							}
							if (online)
							{
								c.sendCmd<ZhanDuiZhengBaAwardsData>(1279, awardsData, false);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "组队竞技清场调度异常");
			}
		}

		
		public bool OnPreZhanDuiChangeMember(PreZhanDuiChangeMemberEventObject e)
		{
			DateTime now = TimeUtil.NowDateTime();
			long tsTicks = now.TimeOfDay.Ticks;
			lock (this.RuntimeData.Mutex)
			{
				foreach (ZhanDuiZhengBaMatchConfig config in this.RuntimeData.Config.MatchConfigList)
				{
					if (now.Day != config.TimePoints[0].Days)
					{
						return false;
					}
					if (tsTicks < config.DayBeginTick || tsTicks > config.DayEndTick)
					{
						return false;
					}
				}
				ZhanDuiZhengBaZhanDuiData zhanDuiData = this.RuntimeData.SyncData.ZhanDuiList.Find((ZhanDuiZhengBaZhanDuiData x) => x.ZhanDuiID == e.ZhanDuiID);
				if (zhanDuiData == null || zhanDuiData.State == 2)
				{
					return false;
				}
				e.Result = false;
			}
			bool result;
			if (!e.Result)
			{
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(8001, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public const SceneUIClasses ManagerType = SceneUIClasses.ZhanDuiZhengBa;

		
		public const GameTypes GameType = GameTypes.ZhanDuiZhengBa;

		
		private static ZhanDuiZhengBaManager instance = new ZhanDuiZhengBaManager();

		
		private EventSourceEx<KFCallMsg>.HandlerData NotifyEnterHandler = null;

		
		public ZhanDuiZhengBaData RuntimeData = new ZhanDuiZhengBaData();

		
		public ConcurrentDictionary<int, ZhanDuiZhengBaScene> ZhanDuiZhengBaSceneDict = new ConcurrentDictionary<int, ZhanDuiZhengBaScene>();

		
		public HashSet<int> CancledGameIdDict = new HashSet<int>();

		
		private static long NextHeartBeatTicks = 0L;
	}
}
