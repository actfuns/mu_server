using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.Ornament;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class TianTiManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		
		public static TianTiManager getInstance()
		{
			return TianTiManager.instance;
		}

		
		public bool initialize()
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("TianTiManager.TimerProc", new EventHandler(this.TimerProc)), 20000, 10000);
			return this.InitConfig(false);
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(950, 1, 1, TianTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(951, 1, 1, TianTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(952, 2, 2, TianTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(954, 1, 1, TianTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(955, 1, 1, TianTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(956, 1, 1, TianTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(969, 1, 1, TianTiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10001, 26, TianTiManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, TianTiManager.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10001, 26, TianTiManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, TianTiManager.getInstance());
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
			case 950:
				return this.ProcessTianTiJoinCmd(client, nID, bytes, cmdParams);
			case 951:
				return this.ProcessTianTiQuitCmd(client, nID, bytes, cmdParams);
			case 952:
				return this.ProcessTianTiEnterCmd(client, nID, bytes, cmdParams);
			case 953:
				break;
			case 954:
				return this.ProcessGetTianTiDataAndDayPaiHangCmd(client, nID, bytes, cmdParams);
			case 955:
				return this.ProcessGetTianTiMonthPaiHangDataCmd(client, nID, bytes, cmdParams);
			case 956:
				return this.ProcessTianTiGetPaiHangAwardsCmd(client, nID, bytes, cmdParams);
			default:
				if (nID == 969)
				{
					return this.ProcessTianTiGeLogCmd(client, nID, bytes, cmdParams);
				}
				break;
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
		}

		
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
						GameClient client = GameManager.ClientMgr.FindClient(kuaFuServerLoginData.RoleId);
						if (null != client)
						{
							KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
							if (null != clientKuaFuServerLoginData)
							{
								clientKuaFuServerLoginData.RoleId = kuaFuServerLoginData.RoleId;
								clientKuaFuServerLoginData.GameId = kuaFuServerLoginData.GameId;
								clientKuaFuServerLoginData.GameType = kuaFuServerLoginData.GameType;
								clientKuaFuServerLoginData.EndTicks = kuaFuServerLoginData.EndTicks;
								clientKuaFuServerLoginData.ServerId = kuaFuServerLoginData.ServerId;
								clientKuaFuServerLoginData.ServerIp = kuaFuServerLoginData.ServerIp;
								clientKuaFuServerLoginData.ServerPort = kuaFuServerLoginData.ServerPort;
								clientKuaFuServerLoginData.FuBenSeqId = kuaFuServerLoginData.FuBenSeqId;
								client.sendCmd<long>(952, kuaFuServerLoginData.GameId, false);
							}
						}
					}
					eventObject.Handled = true;
				}
			}
		}

		
		public bool InitConfig(bool reload = false)
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					if (reload)
					{
						this.RuntimeData.TianTiCD = (int)GameManager.systemParamsList.GetParamValueIntByName("TianTiCD", -1);
						return success;
					}
					this.RuntimeData.TianTiDuanWeiDict.Clear();
					this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.Clear();
					int preJiFen = 0;
					int perDuanWeiId = 0;
					int maxDuanWeiId = 0;
					fileName = "Config/DuanWei.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						TianTiDuanWei item = new TianTiDuanWei();
						item.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item.NeedDuanWeiJiFen = (int)Global.GetSafeAttributeLong(node, "NeedDuanWeiJiFen");
						item.WinJiFen = (int)Global.GetSafeAttributeLong(node, "WinJiFen");
						item.LoseJiFen = (int)Global.GetSafeAttributeLong(node, "LoseJiFen");
						item.RongYaoNum = (int)Global.GetSafeAttributeLong(node, "RongYaoNum");
						item.WinRongYu = (int)Global.GetSafeAttributeLong(node, "WinRongYu");
						item.LoseRongYu = (int)Global.GetSafeAttributeLong(node, "LoseRongYu");
						if (perDuanWeiId > 0)
						{
							this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict[new RangeKey(preJiFen, item.NeedDuanWeiJiFen - 1, null)] = perDuanWeiId;
						}
						preJiFen = item.NeedDuanWeiJiFen;
						perDuanWeiId = item.ID;
						maxDuanWeiId = item.ID;
						this.RuntimeData.TianTiDuanWeiDict[item.ID] = item;
					}
					if (maxDuanWeiId > 0 && preJiFen > 0)
					{
						this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict[new RangeKey(preJiFen, int.MaxValue, null)] = maxDuanWeiId;
					}
					this.RuntimeData.MapBirthPointDict.Clear();
					fileName = "Config/TianTiBirthPoint.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						TianTiBirthPoint item2 = new TianTiBirthPoint();
						item2.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item2.PosX = (int)Global.GetSafeAttributeLong(node, "PosX");
						item2.PosY = (int)Global.GetSafeAttributeLong(node, "PosY");
						item2.BirthRadius = (int)Global.GetSafeAttributeLong(node, "BirthRadius");
						this.RuntimeData.MapBirthPointDict[item2.ID] = item2;
					}
					this.RuntimeData.DuanWeiRankAwardDict.Clear();
					fileName = "Config/DuanWeiRankAward.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						DuanWeiRankAward item3 = new DuanWeiRankAward();
						item3.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item3.StarRank = (int)Global.GetSafeAttributeLong(node, "StarRank");
						item3.EndRank = (int)Global.GetSafeAttributeLong(node, "EndRank");
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "Award"), ref item3.Award, '|', ',');
						if (item3.EndRank < 0)
						{
							item3.EndRank = int.MaxValue;
						}
						this.RuntimeData.DuanWeiRankAwardDict[new RangeKey(item3.StarRank, item3.EndRank, null)] = item3;
					}
					this.RuntimeData.MapCodeDict.Clear();
					fileName = "Config/TianTi.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						int mapCode = (int)Global.GetSafeAttributeLong(node, "MapCode");
						if (!this.RuntimeData.MapCodeDict.ContainsKey(mapCode))
						{
							this.RuntimeData.MapCodeDict[mapCode] = 1;
							this.RuntimeData.MapCodeList.Add(mapCode);
						}
						this.RuntimeData.WaitingEnterSecs = (int)Global.GetSafeAttributeLong(node, "WaitingEnterSecs");
						this.RuntimeData.FightingSecs = (int)Global.GetSafeAttributeLong(node, "FightingSecs");
						this.RuntimeData.ClearRolesSecs = (int)Global.GetSafeAttributeLong(node, "ClearRolesSecs");
						if (!ConfigParser.ParserTimeRangeList(this.RuntimeData.TimePoints, Global.GetSafeAttributeStr(node, "TimePoints"), true, '|', '-'))
						{
							success = false;
							LogManager.WriteLog(LogTypes.Fatal, "读取跨服天梯系统时间配置(TimePoints)出错", null, true);
						}
						GameMap gameMap = null;
						if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
						{
							success = false;
							LogManager.WriteLog(LogTypes.Fatal, string.Format("缺少跨服天梯系统地图 {0}", mapCode), null, true);
						}
					}
					this.RuntimeData.DuanWeiJiFenNum = (int)GameManager.systemParamsList.GetParamValueIntByName("DuanWeiJiFenNum", -1);
					this.RuntimeData.WinDuanWeiJiFen = (int)GameManager.systemParamsList.GetParamValueIntByName("WinDuanWeiJiFen", -1);
					this.RuntimeData.LoseDuanWeiJiFen = (int)GameManager.systemParamsList.GetParamValueIntByName("LoseDuanWeiJiFen", -1);
					this.RuntimeData.MaxTianTiJiFen = (int)GameManager.systemParamsList.GetParamValueIntByName("MaxTianTiJiFen", -1);
				}
				catch (Exception ex)
				{
					success = false;
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		
		public void GMStartHuoDongNow(int v)
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					if (v == 0)
					{
						ConfigParser.ParserTimeRangeList(this.RuntimeData.TimePoints, this.RuntimeData.TimePointsStr, true, '|', '-');
					}
					else
					{
						ConfigParser.ParserTimeRangeList(this.RuntimeData.TimePoints, "00:00-23:59:59", true, '|', '-');
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		
		public void GMSetRoleData(GameClient client, int duanWeiId, int duanWeiJiFen, int rongYao, int monthDuanWeiRank, int lianSheng, int successCount, int fightCount)
		{
			RoleTianTiData roleTianTiData = client.ClientData.TianTiData;
			roleTianTiData.DuanWeiId = duanWeiId;
			roleTianTiData.DuanWeiJiFen = duanWeiJiFen;
			roleTianTiData.RongYao = rongYao;
			roleTianTiData.MonthDuanWeiRank = monthDuanWeiRank;
			roleTianTiData.LianSheng = lianSheng;
			roleTianTiData.SuccessCount = successCount;
			roleTianTiData.FightCount = fightCount;
			int selfDuanWeiId;
			if (this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.TryGetValue(roleTianTiData.DuanWeiJiFen, out selfDuanWeiId))
			{
				roleTianTiData.DuanWeiId = selfDuanWeiId;
			}
			Global.sendToDB<int, RoleTianTiData>(10201, roleTianTiData, client.ServerId);
			TianTiPaiHangRoleData tianTiPaiHangRoleData = new TianTiPaiHangRoleData();
			tianTiPaiHangRoleData.DuanWeiId = roleTianTiData.DuanWeiId;
			tianTiPaiHangRoleData.RoleId = roleTianTiData.RoleId;
			tianTiPaiHangRoleData.RoleName = client.ClientData.RoleName;
			tianTiPaiHangRoleData.Occupation = client.ClientData.Occupation;
			tianTiPaiHangRoleData.ZhanLi = client.ClientData.CombatForce;
			tianTiPaiHangRoleData.ZoneId = client.ClientData.ZoneID;
			tianTiPaiHangRoleData.DuanWeiJiFen = roleTianTiData.DuanWeiJiFen;
			RoleData4Selector roleInfo = Global.sendToDB<RoleData4Selector, string>(512, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
			if (roleInfo != null || roleInfo.RoleID < 0)
			{
				tianTiPaiHangRoleData.RoleData4Selector = roleInfo;
			}
			PlayerJingJiData jingJiData = JingJiChangManager.getInstance().createJingJiData(client);
			TianTiRoleInfoData tianTiRoleInfoData = new TianTiRoleInfoData();
			tianTiRoleInfoData.RoleId = tianTiPaiHangRoleData.RoleId;
			tianTiRoleInfoData.ZoneId = tianTiPaiHangRoleData.ZoneId;
			tianTiRoleInfoData.ZhanLi = tianTiPaiHangRoleData.ZhanLi;
			tianTiRoleInfoData.RoleName = tianTiPaiHangRoleData.RoleName;
			tianTiRoleInfoData.DuanWeiId = tianTiPaiHangRoleData.DuanWeiId;
			tianTiRoleInfoData.DuanWeiJiFen = tianTiPaiHangRoleData.DuanWeiJiFen;
			tianTiRoleInfoData.DuanWeiRank = tianTiPaiHangRoleData.DuanWeiRank;
			tianTiRoleInfoData.TianTiPaiHangRoleData = DataHelper.ObjectToBytes<TianTiPaiHangRoleData>(tianTiPaiHangRoleData);
			tianTiRoleInfoData.PlayerJingJiMirrorData = DataHelper.ObjectToBytes<PlayerJingJiData>(jingJiData);
			TianTiClient.getInstance().UpdateRoleInfoData(tianTiRoleInfoData);
			GameManager.ClientMgr.ModifyTianTiRongYaoValue(client, rongYao, "GM添加", true);
		}

		
		public void TimerProc(object sender, EventArgs e)
		{
			bool modify = false;
			TianTiRankData tianTiRankData = TianTiClient.getInstance().GetRankingData();
			lock (this.RuntimeData.Mutex)
			{
				if (tianTiRankData != null && tianTiRankData.ModifyTime > this.RuntimeData.ModifyTime)
				{
					modify = true;
				}
			}
			if (modify)
			{
				Dictionary<int, TianTiPaiHangRoleData> tianTiPaiHangRoleDataDict = new Dictionary<int, TianTiPaiHangRoleData>();
				List<TianTiPaiHangRoleData> tianTiPaiHangRoleDataList = new List<TianTiPaiHangRoleData>();
				Dictionary<int, TianTiPaiHangRoleData> tianTiMonthPaiHangRoleDataDict = new Dictionary<int, TianTiPaiHangRoleData>();
				List<TianTiPaiHangRoleData> tianTiMonthPaiHangRoleDataList = new List<TianTiPaiHangRoleData>();
				if (null != tianTiRankData.TianTiRoleInfoDataList)
				{
					foreach (TianTiRoleInfoData data in tianTiRankData.TianTiRoleInfoDataList)
					{
						TianTiPaiHangRoleData tianTiPaiHangRoleData;
						if (null != data.TianTiPaiHangRoleData)
						{
							tianTiPaiHangRoleData = DataHelper.BytesToObject<TianTiPaiHangRoleData>(data.TianTiPaiHangRoleData, 0, data.TianTiPaiHangRoleData.Length);
						}
						else
						{
							tianTiPaiHangRoleData = new TianTiPaiHangRoleData
							{
								RoleId = data.RoleId
							};
						}
						if (null != tianTiPaiHangRoleData)
						{
							tianTiPaiHangRoleData.RoleId = data.RoleId;
							tianTiPaiHangRoleData.DuanWeiRank = data.DuanWeiRank;
							tianTiPaiHangRoleDataDict[tianTiPaiHangRoleData.RoleId] = tianTiPaiHangRoleData;
							if (tianTiPaiHangRoleDataList.Count < this.RuntimeData.MaxDayPaiMingListCount)
							{
								tianTiPaiHangRoleDataList.Add(tianTiPaiHangRoleData);
							}
						}
					}
				}
				if (null != tianTiRankData.TianTiMonthRoleInfoDataList)
				{
					foreach (TianTiRoleInfoData data in tianTiRankData.TianTiMonthRoleInfoDataList)
					{
						TianTiPaiHangRoleData tianTiPaiHangRoleData;
						if (null != data.TianTiPaiHangRoleData)
						{
							tianTiPaiHangRoleData = DataHelper.BytesToObject<TianTiPaiHangRoleData>(data.TianTiPaiHangRoleData, 0, data.TianTiPaiHangRoleData.Length);
						}
						else
						{
							tianTiPaiHangRoleData = new TianTiPaiHangRoleData
							{
								RoleId = data.RoleId
							};
						}
						if (null != tianTiPaiHangRoleData)
						{
							tianTiPaiHangRoleData.RoleId = data.RoleId;
							tianTiPaiHangRoleData.DuanWeiRank = data.DuanWeiRank;
							tianTiMonthPaiHangRoleDataDict[tianTiPaiHangRoleData.RoleId] = tianTiPaiHangRoleData;
							if (tianTiMonthPaiHangRoleDataList.Count < this.RuntimeData.MaxMonthPaiMingListCount)
							{
								tianTiMonthPaiHangRoleDataList.Add(tianTiPaiHangRoleData);
							}
						}
					}
				}
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.ModifyTime = tianTiRankData.ModifyTime;
					this.RuntimeData.MaxPaiMingRank = tianTiRankData.MaxPaiMingRank;
					this.RuntimeData.TianTiPaiHangRoleDataDict = tianTiPaiHangRoleDataDict;
					this.RuntimeData.TianTiPaiHangRoleDataList = tianTiPaiHangRoleDataList;
					this.RuntimeData.TianTiMonthPaiHangRoleDataDict = tianTiMonthPaiHangRoleDataDict;
					this.RuntimeData.TianTiMonthPaiHangRoleDataList = tianTiMonthPaiHangRoleDataList;
				}
			}
		}

		
		public bool ProcessTianTiJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				if (sceneType != SceneUIClasses.Normal)
				{
					client.sendCmd<int>(nID, -21, false);
					return true;
				}
				if (!this.IsGongNengOpened(client, true))
				{
					client.sendCmd<int>(nID, -2001, false);
					return true;
				}
				long lastTicks;
				if (this.RuntimeData.TianTiCDDict.TryGetValue(client.ClientData.RoleID, out lastTicks) && lastTicks > 0L)
				{
					if (lastTicks + (long)(this.RuntimeData.TianTiCD * 1000) > TimeUtil.NOW())
					{
						GameManager.ClientMgr.NotifySkillCDTime(client, -2, lastTicks + (long)(this.RuntimeData.TianTiCD * 1000), false);
						client.sendCmd<int>(nID, -2004, false);
						return true;
					}
					this.RuntimeData.TianTiCDDict[client.ClientData.RoleID] = 0L;
				}
				int result = -2001;
				int gropuIndex = client.ClientData.TianTiData.DuanWeiId;
				TimeSpan time = TimeUtil.NowDateTime().TimeOfDay;
				lock (this.RuntimeData.Mutex)
				{
					for (int i = 0; i < this.RuntimeData.TimePoints.Count - 1; i += 2)
					{
						if (time >= this.RuntimeData.TimePoints[i] && time < this.RuntimeData.TimePoints[i + 1])
						{
							result = 1;
							break;
						}
					}
				}
				if (result >= 0)
				{
					result = TianTiClient.getInstance().TianTiSignUp(client.strUserID, client.ClientData.RoleID, client.ClientData.ZoneID, 2, gropuIndex, client.ClientData.CombatForce);
					if (result > 0)
					{
						client.ClientData.SignUpGameType = 2;
						GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 1, 0, 0, 0, 2);
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

		
		public bool ProcessGetTianTiDataAndDayPaiHangCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				TianTiDataAndDayPaiHang tianTiDataAndDayPaiHang = new TianTiDataAndDayPaiHang();
				if (this.IsGongNengOpened(client, false))
				{
					this.InitRoleTianTiData(client);
					tianTiDataAndDayPaiHang.TianTiData = client.ClientData.TianTiData;
					lock (this.RuntimeData.Mutex)
					{
						int count = this.RuntimeData.TianTiPaiHangRoleDataList.Count;
						if (count > 0)
						{
							tianTiDataAndDayPaiHang.PaiHangRoleDataList = this.RuntimeData.TianTiPaiHangRoleDataList.GetRange(0, count);
						}
					}
				}
				client.sendCmd<TianTiDataAndDayPaiHang>(nID, tianTiDataAndDayPaiHang, false);
				long lastTicks;
				if (this.RuntimeData.TianTiCDDict.TryGetValue(client.ClientData.RoleID, out lastTicks))
				{
					if (lastTicks + (long)(this.RuntimeData.TianTiCD * 1000) > TimeUtil.NOW())
					{
						GameManager.ClientMgr.NotifySkillCDTime(client, -2, lastTicks + (long)(this.RuntimeData.TianTiCD * 1000), false);
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessGetTianTiMonthPaiHangDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				TianTiMonthPaiHangData tianTiMonthPaiHangData = new TianTiMonthPaiHangData();
				if (this.IsGongNengOpened(client, false))
				{
					tianTiMonthPaiHangData.SelfPaiHangRoleData = new TianTiPaiHangRoleData
					{
						RoleId = client.ClientData.RoleID,
						RoleName = client.ClientData.RoleName,
						DuanWeiId = client.ClientData.TianTiData.DuanWeiId,
						DuanWeiJiFen = client.ClientData.TianTiData.DuanWeiJiFen,
						DuanWeiRank = client.ClientData.TianTiData.DuanWeiRank
					};
					lock (this.RuntimeData.Mutex)
					{
						if (null != this.RuntimeData.TianTiMonthPaiHangRoleDataList)
						{
							tianTiMonthPaiHangData.PaiHangRoleDataList = new List<TianTiPaiHangRoleData>(this.RuntimeData.TianTiMonthPaiHangRoleDataList);
						}
					}
				}
				client.sendCmd<TianTiMonthPaiHangData>(nID, tianTiMonthPaiHangData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessTianTiGetPaiHangAwardsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = -20;
				DuanWeiRankAward duanWeiRankAward = null;
				if (this.CanGetMonthRankAwards(client, out duanWeiRankAward))
				{
					List<GoodsData> goodsDataList = Global.ConvertToGoodsDataList(duanWeiRankAward.Award.Items, -1);
					if (!Global.CanAddGoodsDataList(client, goodsDataList))
					{
						result = -100;
					}
					else
					{
						result = 0;
						client.ClientData.TianTiData.FetchMonthDuanWeiRankAwardsTime = TimeUtil.NowDateTime();
						Global.sendToDB<int, RoleTianTiData>(10201, client.ClientData.TianTiData, client.ServerId);
						for (int i = 0; i < goodsDataList.Count; i++)
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsDataList[i].Quality, "", goodsDataList[i].Forge_level, goodsDataList[i].Binding, 0, "", true, 1, "天梯月段位排名奖励", "1900-01-01 12:00:00", 0, goodsDataList[i].BornIndex, goodsDataList[i].Lucky, 0, goodsDataList[i].ExcellenceInfo, goodsDataList[i].AppendPropLev, 0, null, null, 0, true);
						}
					}
				}
				else if (duanWeiRankAward != null)
				{
					if (client.CodeRevision <= 2)
					{
						result = 1;
						GameManager.ClientMgr.NotifyHintMsg(client, GLang.GetLang(537, new object[0]));
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

		
		public bool ProcessTianTiGeLogCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<TianTiLogItemData> logList = new List<TianTiLogItemData>();
				logList = Global.sendToDB<List<TianTiLogItemData>, int>(969, client.ClientData.RoleID, client.ServerId);
				client.sendCmd<List<TianTiLogItemData>>(nID, logList, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessTianTiEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				int flag = Global.SafeConvertToInt32(cmdParams[1]);
				if (flag > 0)
				{
					int result = TianTiClient.getInstance().ChangeRoleState(client.ClientData.RoleID, KuaFuRoleStates.EnterGame, false);
					if (result >= 0)
					{
						GlobalNew.RecordSwitchKuaFuServerLog(client);
						client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
					}
					else
					{
						flag = 0;
					}
				}
				else
				{
					TianTiClient.getInstance().RoleChangeState(GameManager.ServerId, client.ClientData.RoleID, 0);
					long nowTicks = TimeUtil.NOW();
					this.RuntimeData.TianTiCDDict[client.ClientData.RoleID] = nowTicks;
					GameManager.ClientMgr.NotifySkillCDTime(client, -2, nowTicks + (long)(this.RuntimeData.TianTiCD * 1000), false);
				}
				client.ClientData.SignUpGameType = 0;
				if (flag <= 0)
				{
					Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
					client.sendCmd<int>(951, 0, false);
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessTianTiQuitCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				int result = 1;
				if (result >= 0)
				{
					result = TianTiClient.getInstance().ChangeRoleState(client.ClientData.RoleID, KuaFuRoleStates.None, false);
					client.ClientData.SignUpGameType = 0;
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

		
		public bool InitRoleTianTiData(GameClient client)
		{
			bool result;
			if (KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				result = false;
			}
			else
			{
				bool rankChanged = false;
				DateTime now = TimeUtil.NowDateTime();
				DateTime lastMonth = now.AddMonths(-1);
				lastMonth = new DateTime(lastMonth.Year, lastMonth.Month, 1);
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.ModifyTime > lastMonth)
					{
						int newRank = this.RuntimeData.MaxPaiMingRank + 1;
						TianTiPaiHangRoleData tianTiPaiHangRoleData;
						if (this.RuntimeData.TianTiPaiHangRoleDataDict.TryGetValue(client.ClientData.RoleID, out tianTiPaiHangRoleData))
						{
							newRank = tianTiPaiHangRoleData.DuanWeiRank;
						}
						if (client.ClientData.TianTiData.DuanWeiRank != newRank)
						{
							rankChanged = true;
							client.ClientData.TianTiData.DuanWeiRank = newRank;
						}
						newRank = this.RuntimeData.MaxPaiMingRank + 1;
						if (this.RuntimeData.TianTiMonthPaiHangRoleDataDict.TryGetValue(client.ClientData.RoleID, out tianTiPaiHangRoleData))
						{
							newRank = tianTiPaiHangRoleData.DuanWeiRank;
						}
						if (client.ClientData.TianTiData.MonthDuanWeiRank != newRank)
						{
							rankChanged = true;
							client.ClientData.TianTiData.MonthDuanWeiRank = newRank;
						}
					}
					DateTime lastFightDay = Global.GetRealDate(client.ClientData.TianTiData.LastFightDayId);
					if (this.RuntimeData.ModifyTime > lastFightDay && lastFightDay.Month != this.RuntimeData.ModifyTime.Month)
					{
						client.ClientData.TianTiData.LianSheng = 0;
						client.ClientData.TianTiData.SuccessCount = 0;
						client.ClientData.TianTiData.FightCount = 0;
						client.ClientData.TianTiData.DuanWeiJiFen = 0;
					}
					int selfDuanWeiId;
					if (this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.TryGetValue(client.ClientData.TianTiData.DuanWeiJiFen, out selfDuanWeiId))
					{
						client.ClientData.TianTiData.DuanWeiId = selfDuanWeiId;
					}
					if (!client.ClientSocket.IsKuaFuLogin && lastFightDay.Date != now.Subtract(this.RuntimeData.RefreshTime).Date)
					{
						client.ClientData.TianTiData.TodayFightCount = 0;
					}
					if (lastFightDay < lastMonth)
					{
						client.ClientData.TianTiData.FetchMonthDuanWeiRankAwardsTime = lastMonth.AddMonths(1);
					}
					client.ClientData.TianTiData.RankUpdateTime = this.RuntimeData.ModifyTime;
				}
				if (client.ClientData.TianTiData.TodayFightCount == 0)
				{
					client.ClientData.TianTiData.DayDuanWeiJiFen = 0;
					Global.SaveRoleParamsInt32ValueToDB(client, "TianTiDayScore", 0, true);
				}
				else
				{
					client.ClientData.TianTiData.DayDuanWeiJiFen = Global.GetRoleParamsInt32FromDB(client, "TianTiDayScore");
				}
				result = rankChanged;
			}
			return result;
		}

		
		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			int side = client.ClientData.BattleWhichSide;
			if (side <= 0)
			{
				KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
				side = TianTiClient.getInstance().GetRoleBattleWhichSide((int)clientKuaFuServerLoginData.GameId, clientKuaFuServerLoginData.RoleId);
				if (side > 0)
				{
					client.ClientData.BattleWhichSide = side;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				TianTiBirthPoint TianTiBirthPoint = null;
				if (this.RuntimeData.MapBirthPointDict.TryGetValue(side, out TianTiBirthPoint))
				{
					posX = TianTiBirthPoint.PosX;
					posY = TianTiBirthPoint.PosY;
					return side;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		
		public bool OnInitGame(GameClient client)
		{
			int posX;
			int posY;
			int side = this.GetBirthPoint(client, out posX, out posY);
			bool result;
			if (side <= 0)
			{
				result = false;
			}
			else
			{
				KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
				int index = (int)kuaFuServerLoginData.GameId % this.RuntimeData.MapCodeList.Count;
				client.ClientData.MapCode = this.RuntimeData.MapCodeList[index];
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				client.ClientData.BattleWhichSide = side;
				int fuBenSeq = 0;
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.GameId2FuBenSeq.TryGetValue((int)kuaFuServerLoginData.GameId, out fuBenSeq))
					{
						fuBenSeq = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						this.RuntimeData.GameId2FuBenSeq[(int)kuaFuServerLoginData.GameId] = fuBenSeq;
					}
				}
				kuaFuServerLoginData.FuBenSeqId = fuBenSeq;
				client.ClientData.FuBenSeqID = kuaFuServerLoginData.FuBenSeqId;
				result = true;
			}
			return result;
		}

		
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("TianTi") && !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6) && GlobalNew.IsGongNengOpened(client, GongNengIDs.TianTi, hint);
		}

		
		public bool CanGetMonthRankAwards(GameClient client, out DuanWeiRankAward duanWeiRankAward)
		{
			duanWeiRankAward = null;
			lock (this.RuntimeData.Mutex)
			{
				if (client.ClientData.TianTiData.MonthDuanWeiRank > 0)
				{
					if (this.RuntimeData.DuanWeiRankAwardDict.TryGetValue(client.ClientData.TianTiData.MonthDuanWeiRank, out duanWeiRankAward))
					{
						DateTime fetchTime = client.ClientData.TianTiData.FetchMonthDuanWeiRankAwardsTime;
						DateTime now = TimeUtil.NowDateTime();
						if (fetchTime.Month != now.Month || fetchTime.Year != now.Year)
						{
							if (new DateTime(fetchTime.Year, fetchTime.Month, 1) < new DateTime(this.RuntimeData.ModifyTime.Year, this.RuntimeData.ModifyTime.Month, 1))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		
		public bool AddTianTiCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.TianTi)
			{
				int fuBenSeqId = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (this.RuntimeData.Mutex)
				{
					TianTiScene tianTiScene = null;
					if (!this.TianTiSceneDict.TryGetValue(fuBenSeqId, out tianTiScene))
					{
						tianTiScene = new TianTiScene();
						tianTiScene.CopyMap = copyMap;
						tianTiScene.CleanAllInfo();
						tianTiScene.GameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
						tianTiScene.m_nMapCode = mapCode;
						tianTiScene.CopyMapId = copyMap.CopyMapID;
						tianTiScene.FuBenSeqId = fuBenSeqId;
						tianTiScene.m_nPlarerCount = 1;
						this.TianTiSceneDict[fuBenSeqId] = tianTiScene;
					}
					else
					{
						tianTiScene.m_nPlarerCount++;
					}
					copyMap.IsKuaFuCopy = true;
					this.SaveClientBattleSide(tianTiScene, client);
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(this.RuntimeData.TotalSecs * 1000));
					if (tianTiScene.SuccessSide == -1)
					{
						client.sendCmd<TianTiAwardsData>(953, new TianTiAwardsData
						{
							Success = -1
						}, false);
					}
				}
				TianTiClient.getInstance().GameFuBenRoleChangeState(client.ClientData.RoleID, 5, 0, 0);
				GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 1, 0, 0, 2);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public bool RemoveTianTiCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.TianTi)
			{
				lock (this.RuntimeData.Mutex)
				{
					TianTiScene tianTiScene;
					if (this.TianTiSceneDict.TryRemove(copyMap.FuBenSeqID, out tianTiScene))
					{
						this.RuntimeData.GameId2FuBenSeq.Remove(tianTiScene.GameId);
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

		
		private void SaveClientBattleSide(TianTiScene tianTiScene, GameClient client)
		{
			TianTiRoleMiniData tianTiRoleMiniData;
			if (!tianTiScene.RoleIdDuanWeiIdDict.TryGetValue(client.ClientData.RoleID, out tianTiRoleMiniData))
			{
				tianTiRoleMiniData = new TianTiRoleMiniData();
				tianTiScene.RoleIdDuanWeiIdDict[client.ClientData.RoleID] = tianTiRoleMiniData;
			}
			tianTiRoleMiniData.RoleId = client.ClientData.RoleID;
			tianTiRoleMiniData.RoleName = client.ClientData.RoleName;
			tianTiRoleMiniData.BattleWitchSide = client.ClientData.BattleWhichSide;
			tianTiRoleMiniData.ZoneId = client.ClientData.ZoneID;
			tianTiRoleMiniData.DuanWeiId = client.ClientData.TianTiData.DuanWeiId;
		}

		
		private TianTiRoleMiniData GetEnemyBattleSide(TianTiScene tianTiScene, GameClient client)
		{
			foreach (KeyValuePair<int, TianTiRoleMiniData> kv in tianTiScene.RoleIdDuanWeiIdDict)
			{
				if (client.ClientData.RoleID != kv.Key)
				{
					return kv.Value;
				}
			}
			return null;
		}

		
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= TianTiManager.NextHeartBeatTicks)
			{
				TianTiManager.NextHeartBeatTicks = nowTicks + 1020L;
				foreach (TianTiScene tianTiScene in this.TianTiSceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int nID = tianTiScene.FuBenSeqId;
						int nCopyID = tianTiScene.CopyMapId;
						int nMapCodeID = tianTiScene.m_nMapCode;
						if (nID >= 0 && nCopyID >= 0 && nMapCodeID >= 0)
						{
							CopyMap copyMap = tianTiScene.CopyMap;
							DateTime now = TimeUtil.NowDateTime();
							long ticks = TimeUtil.NOW();
							if (tianTiScene.m_eStatus == GameSceneStatuses.STATUS_NULL)
							{
								tianTiScene.m_lPrepareTime = ticks;
								tianTiScene.m_lBeginTime = ticks + (long)(this.RuntimeData.WaitingEnterSecs * 1000);
								tianTiScene.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
								tianTiScene.StateTimeData.GameType = 2;
								tianTiScene.StateTimeData.State = (int)tianTiScene.m_eStatus;
								tianTiScene.StateTimeData.EndTicks = tianTiScene.m_lBeginTime;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, tianTiScene.StateTimeData, tianTiScene.CopyMap);
							}
							else if (tianTiScene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
							{
								bool flag2;
								if (copyMap.GetGameClientCount() >= 2)
								{
									flag2 = !copyMap.GetClientsList().All((GameClient x) => !x.ClientData.FirstPlayStart);
								}
								else
								{
									flag2 = true;
								}
								if (!flag2)
								{
									tianTiScene.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
									tianTiScene.m_lEndTime = ticks + (long)(this.RuntimeData.FightingSecs * 1000);
									tianTiScene.StateTimeData.GameType = 2;
									tianTiScene.StateTimeData.State = (int)tianTiScene.m_eStatus;
									tianTiScene.StateTimeData.EndTicks = tianTiScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, tianTiScene.StateTimeData, tianTiScene.CopyMap);
									copyMap.AddGuangMuEvent(1, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 1, 0);
									copyMap.AddGuangMuEvent(2, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 2, 0);
								}
								else if (ticks >= tianTiScene.m_lBeginTime || this.CancledGameIdDict.Contains(tianTiScene.GameId))
								{
									this.CompleteTianTiScene(tianTiScene, -1);
								}
							}
							else if (tianTiScene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (ticks >= tianTiScene.m_lEndTime)
								{
									this.CompleteTianTiScene(tianTiScene, 0);
								}
							}
							else if (tianTiScene.m_eStatus == GameSceneStatuses.STATUS_END)
							{
								this.ProcessEnd(tianTiScene, now, nowTicks);
							}
							else if (tianTiScene.m_eStatus == GameSceneStatuses.STATUS_AWARD)
							{
								if (ticks >= tianTiScene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(tianTiScene.m_lLeaveTime);
									tianTiScene.m_eStatus = GameSceneStatuses.STATUS_CLEAR;
									try
									{
										List<GameClient> objsList = copyMap.GetClientsList();
										if (objsList != null && objsList.Count > 0)
										{
											for (int i = 0; i < objsList.Count; i++)
											{
												GameClient c = objsList[i];
												if (c != null)
												{
													KuaFuManager.getInstance().GotoLastMap(c);
												}
											}
										}
									}
									catch (Exception ex)
									{
										DataHelper.WriteExceptionLogEx(ex, "跨服天梯系统清场调度异常");
									}
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
				TianTiScene tianTiScene;
				if (this.TianTiSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out tianTiScene))
				{
					client.sendCmd<GameSceneStateTimeData>(827, tianTiScene.StateTimeData, false);
				}
			}
		}

		
		public void CompleteTianTiScene(TianTiScene tianTiScene, int successSide)
		{
			tianTiScene.m_eStatus = GameSceneStatuses.STATUS_END;
			tianTiScene.SuccessSide = successSide;
		}

		
		public void CancleTianTiScene(int gameId)
		{
			lock (this.RuntimeData.Mutex)
			{
				this.CancledGameIdDict.Add(gameId);
			}
		}

		
		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				TianTiScene tianTiScene;
				if (this.TianTiSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out tianTiScene))
				{
					if (tianTiScene.m_eStatus < GameSceneStatuses.STATUS_END)
					{
						this.CompleteTianTiScene(tianTiScene, client.ClientData.BattleWhichSide);
					}
					int posX;
					int posY;
					int side = this.GetBirthPoint(other, out posX, out posY);
					if (side > 0)
					{
						other.ClientData.PosX = posX;
						other.ClientData.PosY = posY;
						GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, other.ClientData.RoleID, posX, posY, -1);
						Global.ClientRealive(other, posX, posY, -1);
					}
				}
			}
		}

		
		private void ProcessEnd(TianTiScene tianTiScene, DateTime now, long nowTicks)
		{
			tianTiScene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
			tianTiScene.m_lEndTime = nowTicks;
			tianTiScene.m_lLeaveTime = tianTiScene.m_lEndTime + (long)(this.RuntimeData.ClearRolesSecs * 1000);
			TianTiClient.getInstance().GameFuBenChangeState(tianTiScene.GameId, GameFuBenState.End, now);
			tianTiScene.StateTimeData.GameType = 2;
			tianTiScene.StateTimeData.State = 3;
			tianTiScene.StateTimeData.EndTicks = tianTiScene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, tianTiScene.StateTimeData, tianTiScene.CopyMap);
			if (tianTiScene.SuccessSide == -1)
			{
				this.GameCanceled(tianTiScene);
			}
			else
			{
				this.GiveAwards(tianTiScene);
			}
		}

		
		public void GiveAwards(TianTiScene tianTiScene)
		{
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				DateTime startTime = now.Subtract(this.RuntimeData.RefreshTime);
				List<GameClient> objsList = tianTiScene.CopyMap.GetClientsList();
				if (objsList != null && objsList.Count > 0)
				{
					int nowDayId = Global.GetOffsetDayNow();
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient client = objsList[i];
						if (client != null && client == GameManager.ClientMgr.FindClient(client.ClientData.RoleID))
						{
							RoleTianTiData roleTianTiData = client.ClientData.TianTiData;
							bool success = client.ClientData.BattleWhichSide == tianTiScene.SuccessSide;
							int selfDuanWeiId = roleTianTiData.DuanWeiId;
							TianTiRoleMiniData enemyMiniData = this.GetEnemyBattleSide(tianTiScene, client);
							int addDuanWeiJiFen = 0;
							int addLianShengJiFen = 0;
							int addRongYao = 0;
							int dayId = Global.GetOffsetDay(startTime);
							if (dayId != roleTianTiData.LastFightDayId)
							{
								roleTianTiData.LastFightDayId = dayId;
								roleTianTiData.TodayFightCount = 1;
							}
							else
							{
								roleTianTiData.TodayFightCount++;
							}
							if (roleTianTiData.DayDuanWeiJiFen < this.RuntimeData.MaxTianTiJiFen)
							{
								if (success)
								{
									roleTianTiData.LianSheng++;
									roleTianTiData.SuccessCount++;
									TianTiDuanWei tianTiDuanWei;
									if (this.RuntimeData.TianTiDuanWeiDict.TryGetValue(enemyMiniData.DuanWeiId, out tianTiDuanWei))
									{
										addDuanWeiJiFen = tianTiDuanWei.WinJiFen;
										addLianShengJiFen = (int)((double)tianTiDuanWei.WinJiFen * Math.Min(2.0, (double)(roleTianTiData.LianSheng - 1) * 0.2));
										if (roleTianTiData.TodayFightCount <= tianTiDuanWei.RongYaoNum)
										{
											addRongYao = tianTiDuanWei.WinRongYu;
										}
									}
								}
								else
								{
									roleTianTiData.LianSheng = 0;
									TianTiDuanWei tianTiDuanWei;
									if (this.RuntimeData.TianTiDuanWeiDict.TryGetValue(roleTianTiData.DuanWeiId, out tianTiDuanWei))
									{
										addDuanWeiJiFen = tianTiDuanWei.LoseJiFen;
										if (roleTianTiData.TodayFightCount <= tianTiDuanWei.RongYaoNum)
										{
											addRongYao = tianTiDuanWei.LoseRongYu;
										}
									}
								}
								if (addDuanWeiJiFen != 0)
								{
									roleTianTiData.DuanWeiJiFen += addDuanWeiJiFen + addLianShengJiFen;
									roleTianTiData.DuanWeiJiFen = Math.Max(0, roleTianTiData.DuanWeiJiFen);
									roleTianTiData.DayDuanWeiJiFen += addDuanWeiJiFen + addLianShengJiFen;
									roleTianTiData.DayDuanWeiJiFen = Math.Max(0, roleTianTiData.DayDuanWeiJiFen);
									Global.SaveRoleParamsInt32ValueToDB(client, "TianTiDayScore", roleTianTiData.DayDuanWeiJiFen, true);
								}
							}
							else
							{
								GameManager.ClientMgr.NotifyHintMsg(client, GLang.GetLang(538, new object[0]));
							}
							if (addRongYao != 0)
							{
								GameManager.ClientMgr.ModifyTianTiRongYaoValue(client, addRongYao, "天梯系统获得荣耀", true);
							}
							roleTianTiData.FightCount++;
							if (this.RuntimeData.DuanWeiJiFenRangeDuanWeiIdDict.TryGetValue(roleTianTiData.DuanWeiJiFen, out selfDuanWeiId))
							{
								roleTianTiData.DuanWeiId = selfDuanWeiId;
							}
							TianTiAwardsData awardsData = new TianTiAwardsData();
							awardsData.DuanWeiJiFen = addDuanWeiJiFen;
							awardsData.LianShengJiFen = addLianShengJiFen;
							awardsData.RongYao = addRongYao;
							awardsData.DuanWeiId = roleTianTiData.DuanWeiId;
							if (success)
							{
								JunTuanManager.getInstance().AddJunTuanTaskValue(client, 3, 1);
								awardsData.Success = 1;
								GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 0, 1, 0, 2);
							}
							else
							{
								GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 0, 0, 1, 2);
							}
							client.sendCmd<TianTiAwardsData>(953, awardsData, false);
							Global.sendToDB<int, RoleTianTiData>(10201, roleTianTiData, client.ServerId);
							TianTiLogItemData tianTiLogItemData = new TianTiLogItemData
							{
								Success = awardsData.Success,
								ZoneId1 = client.ClientData.ZoneID,
								RoleName1 = client.ClientData.RoleName,
								ZoneId2 = enemyMiniData.ZoneId,
								RoleName2 = enemyMiniData.RoleName,
								DuanWeiJiFenAward = addDuanWeiJiFen + addLianShengJiFen,
								RongYaoAward = addRongYao,
								RoleId = client.ClientData.RoleID,
								EndTime = now
							};
							Global.sendToDB<int, TianTiLogItemData>(10200, tianTiLogItemData, client.ServerId);
							TianTiPaiHangRoleData tianTiPaiHangRoleData = new TianTiPaiHangRoleData();
							tianTiPaiHangRoleData.DuanWeiId = roleTianTiData.DuanWeiId;
							tianTiPaiHangRoleData.RoleId = roleTianTiData.RoleId;
							tianTiPaiHangRoleData.RoleName = client.ClientData.RoleName;
							tianTiPaiHangRoleData.Occupation = client.ClientData.Occupation;
							tianTiPaiHangRoleData.ZhanLi = client.ClientData.CombatForce;
							tianTiPaiHangRoleData.ZoneId = client.ClientData.ZoneID;
							tianTiPaiHangRoleData.DuanWeiJiFen = roleTianTiData.DuanWeiJiFen;
							RoleData4Selector roleInfo = Global.sendToDB<RoleData4Selector, string>(512, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
							if (roleInfo != null || roleInfo.RoleID < 0)
							{
								tianTiPaiHangRoleData.RoleData4Selector = roleInfo;
							}
							PlayerJingJiData jingJiData = JingJiChangManager.getInstance().createJingJiData(client);
							TianTiRoleInfoData tianTiRoleInfoData = new TianTiRoleInfoData();
							tianTiRoleInfoData.RoleId = tianTiPaiHangRoleData.RoleId;
							tianTiRoleInfoData.ZoneId = tianTiPaiHangRoleData.ZoneId;
							tianTiRoleInfoData.ZhanLi = tianTiPaiHangRoleData.ZhanLi;
							tianTiRoleInfoData.RoleName = tianTiPaiHangRoleData.RoleName;
							tianTiRoleInfoData.DuanWeiId = tianTiPaiHangRoleData.DuanWeiId;
							tianTiRoleInfoData.DuanWeiJiFen = tianTiPaiHangRoleData.DuanWeiJiFen;
							tianTiRoleInfoData.DuanWeiRank = tianTiPaiHangRoleData.DuanWeiRank;
							tianTiRoleInfoData.TianTiPaiHangRoleData = DataHelper.ObjectToBytes<TianTiPaiHangRoleData>(tianTiPaiHangRoleData);
							tianTiRoleInfoData.PlayerJingJiMirrorData = DataHelper.ObjectToBytes<PlayerJingJiData>(jingJiData);
							TianTiClient.getInstance().UpdateRoleInfoData(tianTiRoleInfoData);
						}
						GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_TianTiPT, new int[0]));
						GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_TianTiDiamond, new int[0]));
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "天梯系统清场调度异常");
			}
		}

		
		public void GameCanceled(TianTiScene tianTiScene)
		{
			try
			{
				List<GameClient> objsList = tianTiScene.CopyMap.GetClientsList();
				if (objsList != null && objsList.Count > 0)
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient client = objsList[i];
						if (client != null && client == GameManager.ClientMgr.FindClient(client.ClientData.RoleID))
						{
							client.sendCmd<TianTiAwardsData>(953, new TianTiAwardsData
							{
								Success = -1
							}, false);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "天梯系统清场调度异常");
			}
		}

		
		public void LeaveFuBen(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				TianTiScene tianTiScene = null;
				if (this.TianTiSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out tianTiScene))
				{
					if (tianTiScene.m_eStatus < GameSceneStatuses.STATUS_END)
					{
						if (tianTiScene.CopyMap.GetGameClientCount() >= 2)
						{
							if (client.ClientData.BattleWhichSide == 1)
							{
								this.CompleteTianTiScene(tianTiScene, 2);
							}
							else
							{
								this.CompleteTianTiScene(tianTiScene, 1);
							}
						}
						else
						{
							this.CompleteTianTiScene(tianTiScene, -1);
						}
						this.ProcessEnd(tianTiScene, TimeUtil.NowDateTime(), TimeUtil.NOW());
					}
				}
			}
			TianTiClient.getInstance().GameFuBenRoleChangeState(client.ClientData.RoleID, 0, 0, 0);
		}

		
		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		
		public const SceneUIClasses ManagerType = SceneUIClasses.TianTi;

		
		private static TianTiManager instance = new TianTiManager();

		
		public TianTiData RuntimeData = new TianTiData();

		
		public ConcurrentDictionary<int, TianTiScene> TianTiSceneDict = new ConcurrentDictionary<int, TianTiScene>();

		
		public HashSet<int> CancledGameIdDict = new HashSet<int>();

		
		private static long NextHeartBeatTicks = 0L;
	}
}
