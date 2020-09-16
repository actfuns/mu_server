using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.GameEvent;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.UnionAlly
{
	
	public class AllyManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListenerEx
	{
		
		public static AllyManager getInstance()
		{
			return AllyManager.instance;
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1042, 2, 2, AllyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1043, 1, 1, AllyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1044, 1, 1, AllyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1045, 2, 2, AllyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1046, 1, 1, AllyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1047, 1, 1, AllyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1048, 1, 1, AllyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10026, 10004, AllyManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10027, 10004, AllyManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10028, 10004, AllyManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10029, 10004, AllyManager.getInstance());
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
			return true;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1042:
				result = this.ProcessAllyRequestCmd(client, nID, bytes, cmdParams);
				break;
			case 1043:
				result = this.ProcessAllyCancelCmd(client, nID, bytes, cmdParams);
				break;
			case 1044:
				result = this.ProcessAllyRemoveCmd(client, nID, bytes, cmdParams);
				break;
			case 1045:
				result = this.ProcessAllyAgreeCmd(client, nID, bytes, cmdParams);
				break;
			case 1046:
				result = this.ProcessAllyDataCmd(client, nID, bytes, cmdParams);
				break;
			case 1047:
				result = this.ProcessAllyLogDataCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 10026:
			{
				KFNotifyAllyGameEvent e = eventObject as KFNotifyAllyGameEvent;
				int unionID = e.UnionID;
				List<AllyData> list = AllyClient.getInstance().HAllyDataList(unionID, EAllyDataType.Ally);
				int index = 0;
				GameClient client;
				while ((client = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
				{
					lock (AllyClient.getInstance()._Mutex)
					{
						if (client.ClientData.Faction == unionID)
						{
							client.ClientData.AllyList = list;
						}
					}
				}
				break;
			}
			case 10027:
			{
				KFNotifyAllyLogGameEvent e2 = eventObject as KFNotifyAllyLogGameEvent;
				if (null != e2)
				{
					List<AllyLogData> list2 = (List<AllyLogData>)e2.LogList;
					if (list2 != null && list2.Count > 0)
					{
						foreach (AllyLogData log in list2)
						{
							this.DBAllyLogAdd(log, 0);
						}
					}
					eventObject.Handled = true;
				}
				break;
			}
			case 10028:
			{
				KFNotifyAllyTipGameEvent e3 = eventObject as KFNotifyAllyTipGameEvent;
				if (null != e3)
				{
					int unionID = e3.UnionID;
					int tipID = e3.TipID;
					BangHuiDetailData unionData = Global.GetBangHuiDetailData(-1, unionID, GameManager.ServerId);
					if (unionData != null && this.IsAllyOpen(unionData.QiLevel))
					{
						GameClient client = GameManager.ClientMgr.FindClient(unionData.BZRoleID);
						if (client == null)
						{
							break;
						}
						lock (AllyClient.getInstance()._Mutex)
						{
							if (tipID == 14112)
							{
								int countAlly = AllyClient.getInstance().AllyCount(unionID);
								if (countAlly > 0 && this.IsAllyMax(countAlly))
								{
									break;
								}
							}
							client._IconStateMgr.AddFlushIconState(14111, false);
							client._IconStateMgr.AddFlushIconState((ushort)tipID, false);
							client._IconStateMgr.SendIconStateToClient(client);
							client._IconStateMgr.AddFlushIconState(14111, true);
							client._IconStateMgr.AddFlushIconState((ushort)tipID, true);
							client._IconStateMgr.SendIconStateToClient(client);
							switch (tipID)
							{
							case 14112:
								client.AllyTip[0] = 1;
								break;
							case 14113:
								client.AllyTip[1] = 1;
								break;
							}
						}
					}
					eventObject.Handled = true;
				}
				break;
			}
			case 10029:
			{
				int index = 0;
				GameClient client;
				while ((client = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
				{
					lock (AllyClient.getInstance()._Mutex)
					{
						client.ClientData.AllyList = null;
						this.UnionAllyInit(client);
						AllyClient.getInstance().HRankClear();
						AllyClient.getInstance().HRankTopList(1);
					}
				}
				break;
			}
			}
		}

		
		public bool ProcessAllyDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				EAllyDataType dataType = (EAllyDataType)Convert.ToInt32(cmdParams[0]);
				List<AllyData> data = this.GetAllyData(client, dataType);
				client.sendCmd<List<AllyData>>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessAllyLogDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				List<AllyLogData> data = this.GetAllyLogData(client);
				client.sendCmd<List<AllyLogData>>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessAllyRequestCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 2))
				{
					return false;
				}
				int zoneID = Convert.ToInt32(cmdParams[0]);
				string unionName = cmdParams[1];
				EAlly state = this.AllyRequest(client, zoneID, unionName);
				int num = (int)state;
				client.sendCmd(nID, num.ToString(), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessAllyCancelCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				int unionID = Convert.ToInt32(cmdParams[0]);
				EAlly state = this.AllyOperate(client, unionID, EAllyOperate.Cancel);
				int num = (int)state;
				client.sendCmd(nID, num.ToString(), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessAllyRemoveCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				int unionID = Convert.ToInt32(cmdParams[0]);
				EAlly state = this.AllyOperate(client, unionID, EAllyOperate.Remove);
				int num = (int)state;
				client.sendCmd(nID, num.ToString(), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessAllyAgreeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 2))
				{
					return false;
				}
				int unionID = Convert.ToInt32(cmdParams[0]);
				EAllyOperate operateType = (EAllyOperate)Convert.ToInt32(cmdParams[1]);
				EAlly state = this.AllyOperate(client, unionID, operateType);
				int num = (int)state;
				client.sendCmd(nID, num.ToString(), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private List<AllyData> GetAllyData(GameClient client, EAllyDataType dataType)
		{
			List<AllyData> resultList = new List<AllyData>();
			int unionID = client.ClientData.Faction;
			List<AllyData> result;
			if (unionID <= 0)
			{
				result = resultList;
			}
			else
			{
				BangHuiDetailData unionData = Global.GetBangHuiDetailData(-1, unionID, client.ServerId);
				if (unionData == null || !this.IsAllyOpen(unionData.QiLevel))
				{
					result = resultList;
				}
				else
				{
					switch (dataType)
					{
					case EAllyDataType.Ally:
					{
						List<AllyData> list = AllyClient.getInstance().HAllyDataList(unionID, EAllyDataType.Ally);
						if (list != null && list.Count > 0)
						{
							resultList.AddRange(list);
						}
						list = AllyClient.getInstance().HAllyDataList(unionID, EAllyDataType.Request);
						if (list != null && list.Count > 0)
						{
							resultList.AddRange(list);
						}
						break;
					}
					case EAllyDataType.Accept:
					{
						List<AllyData> list = AllyClient.getInstance().HAllyDataList(unionID, EAllyDataType.Accept);
						if (list != null && list.Count > 0)
						{
							resultList.AddRange(list);
						}
						client.AllyTip[0] = 0;
						if (client.AllyTip[1] <= 0)
						{
							client._IconStateMgr.AddFlushIconState(14111, false);
						}
						client._IconStateMgr.AddFlushIconState(14112, false);
						client._IconStateMgr.SendIconStateToClient(client);
						break;
					}
					}
					result = resultList;
				}
			}
			return result;
		}

		
		private EAlly AllyRequest(GameClient client, int zoneID, string unionName)
		{
			EAlly result2;
			if (zoneID <= 0)
			{
				result2 = EAlly.EZoneID;
			}
			else if (string.IsNullOrEmpty(unionName))
			{
				result2 = EAlly.EName;
			}
			else
			{
				int unionID = client.ClientData.Faction;
				if (unionID <= 0)
				{
					result2 = EAlly.EUnionJoin;
				}
				else
				{
					BangHuiDetailData myUnion = Global.GetBangHuiDetailData(-1, unionID, client.ServerId);
					if (myUnion == null)
					{
						result2 = EAlly.EUnionJoin;
					}
					else if (!this.IsAllyOpen(myUnion.QiLevel))
					{
						result2 = EAlly.EUnionLevel;
					}
					else if (myUnion.ZoneID == zoneID && myUnion.BHName == unionName)
					{
						result2 = EAlly.EIsSelf;
					}
					else if (myUnion.BZRoleID != client.ClientData.RoleID)
					{
						result2 = EAlly.ENotLeader;
					}
					else if (!this.UnionMoneyIsMore(myUnion.TotalMoney))
					{
						result2 = EAlly.EMoney;
					}
					else if (AllyClient.getInstance().UnionIsAlly(unionID, zoneID, unionName))
					{
						result2 = EAlly.EIsAlly;
					}
					else if (AllyClient.getInstance().UnionIsRequest(unionID, zoneID, unionName))
					{
						result2 = EAlly.EMore;
					}
					else if (AllyClient.getInstance().UnionIsAccept(unionID, zoneID, unionName))
					{
						result2 = EAlly.EMore;
					}
					else
					{
						int countAlly = AllyClient.getInstance().AllyCount(unionID);
						int countRequest = AllyClient.getInstance().AllyRequestCount(unionID);
						if (countAlly > 0 && this.IsAllyMax(countAlly))
						{
							result2 = EAlly.EAllyMax;
						}
						else
						{
							int countSum = countAlly + countRequest;
							if (countSum > 0 && this.IsAllyMax(countSum))
							{
								result2 = EAlly.EAllyRequestMax;
							}
							else
							{
								EAlly result = AllyClient.getInstance().HAllyRequest(unionID, zoneID, unionName);
								if (result == EAlly.AllyRequestSucc)
								{
									int bhZoneID = 0;
									if (!GameManager.ClientMgr.SubBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, this.AllyCostMoney, out bhZoneID))
									{
										LogManager.WriteLog(LogTypes.Error, "战盟结盟 申请 扣除战盟资金失败", null, true);
									}
								}
								result2 = result;
							}
						}
					}
				}
			}
			return result2;
		}

		
		private EAlly AllyOperate(GameClient client, int targetID, EAllyOperate operateType)
		{
			EAlly result2;
			if (targetID <= 0)
			{
				result2 = EAlly.ENoTargetUnion;
			}
			else
			{
				int unionID = client.ClientData.Faction;
				if (unionID <= 0)
				{
					result2 = EAlly.EUnionJoin;
				}
				else
				{
					BangHuiDetailData myUnion = Global.GetBangHuiDetailData(-1, unionID, client.ServerId);
					if (myUnion == null)
					{
						result2 = EAlly.EUnionJoin;
					}
					else if (!this.IsAllyOpen(myUnion.QiLevel))
					{
						result2 = EAlly.EUnionLevel;
					}
					else if (myUnion.BZRoleID != client.ClientData.RoleID)
					{
						result2 = EAlly.ENotLeader;
					}
					else
					{
						int countSum = 0;
						if (operateType == EAllyOperate.Agree)
						{
							int countAlly = AllyClient.getInstance().AllyCount(unionID);
							int countRequest = AllyClient.getInstance().AllyRequestCount(unionID);
							if (countAlly > 0 && this.IsAllyMax(countAlly))
							{
								return EAlly.EAllyMax;
							}
							countSum = countAlly + countRequest;
							if (countSum > 0 && this.IsAllyMax(countSum))
							{
								return EAlly.EAllyMax;
							}
						}
						EAlly result = AllyClient.getInstance().HAllyOperate(unionID, targetID, operateType);
						if (result == EAlly.AllyAgree)
						{
							client.sendCmd(1048, (countSum + 1).ToString(), false);
						}
						result2 = result;
					}
				}
			}
			return result2;
		}

		
		private List<AllyLogData> GetAllyLogData(GameClient client)
		{
			List<AllyLogData> resultList = new List<AllyLogData>();
			int unionID = client.ClientData.Faction;
			List<AllyLogData> result;
			if (unionID <= 0)
			{
				result = resultList;
			}
			else
			{
				BangHuiDetailData unionData = Global.GetBangHuiDetailData(-1, unionID, client.ServerId);
				if (unionData == null || !this.IsAllyOpen(unionData.QiLevel))
				{
					result = resultList;
				}
				else
				{
					client.AllyTip[1] = 0;
					if (client.AllyTip[0] <= 0)
					{
						client._IconStateMgr.AddFlushIconState(14111, false);
					}
					client._IconStateMgr.AddFlushIconState(14113, false);
					client._IconStateMgr.SendIconStateToClient(client);
					result = this.DBAllyLogData(unionID, client.ServerId);
				}
			}
			return result;
		}

		
		public List<AllyLogData> DBAllyLogData(int unionID, int serverID)
		{
			List<AllyLogData> items = Global.sendToDB<List<AllyLogData>, int>(13122, unionID, serverID);
			if (items == null)
			{
				items = new List<AllyLogData>();
			}
			return items;
		}

		
		public bool DBAllyLogAdd(AllyLogData logData, int serverID)
		{
			return Global.sendToDB<bool, AllyLogData>(13123, logData, serverID);
		}

		
		public void UnionAllyInit(GameClient client)
		{
			if (!KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				lock (AllyClient.getInstance()._Mutex)
				{
					int unionID = client.ClientData.Faction;
					int serverID = client.ServerId;
					bool isKF = client.ClientSocket.IsKuaFuLogin;
					if (unionID > 0)
					{
						BangHuiDetailData unionData = Global.GetBangHuiDetailData(-1, unionID, serverID);
						if (unionData != null && this.IsAllyOpen(unionData.QiLevel))
						{
							EAlly result = AllyClient.getInstance().HUnionAllyInit(unionID, isKF);
							if (result == EAlly.EAddUnion)
							{
								this.UnionDataChange(unionID, serverID, false, 0);
							}
							else if (result != EAlly.Succ)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("战盟结盟：数据初始化失败 id={0}", result), null, true);
							}
							List<AllyData> list = AllyClient.getInstance().HAllyDataList(unionID, EAllyDataType.Ally);
							if (list != null && list.Count > 0)
							{
								client.ClientData.AllyList = list;
							}
						}
					}
				}
			}
		}

		
		public bool UnionIsAlly(GameClient client, int targetID)
		{
			bool result;
			lock (AllyClient.getInstance()._Mutex)
			{
				if (client.ClientData.AllyList == null || client.ClientData.AllyList.Count <= 0)
				{
					result = false;
				}
				else
				{
					AllyData resultData = client.ClientData.AllyList.Find((AllyData data) => data.UnionID == targetID);
					bool isAllyMap = this.IsAllyMap(client.ClientData.MapCode);
					if (resultData != null && isAllyMap)
					{
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		
		public void UnionDataChange(int unionID, int serverID, bool isDel = false, int unionLevel = 0)
		{
			if (!KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				if (unionID > 0)
				{
					if (isDel)
					{
						if (this.IsAllyOpen(unionLevel))
						{
							EAlly result = AllyClient.getInstance().HUnionDel(unionID);
							if (result != EAlly.Succ)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("战盟结盟：战盟{0}解散失败 id={1}", unionID, result), null, true);
							}
						}
					}
					else
					{
						BangHuiDetailData unionData = Global.GetBangHuiDetailData(-1, unionID, serverID);
						if (unionData != null && this.IsAllyOpen(unionData.QiLevel))
						{
							AllyData data = new AllyData();
							data.UnionID = unionData.BHID;
							data.UnionZoneID = unionData.ZoneID;
							data.UnionName = unionData.BHName;
							data.UnionLevel = unionData.QiLevel;
							data.UnionNum = unionData.TotalNum;
							data.LeaderID = unionData.BZRoleID;
							data.LeaderName = unionData.BZRoleName;
							SafeClientData clientData = Global.GetSafeClientDataFromLocalOrDB(data.LeaderID);
							if (null != clientData)
							{
								data.LeaderZoneID = clientData.ZoneID;
							}
							else
							{
								data.LeaderZoneID = unionData.ZoneID;
							}
							EAlly result = AllyClient.getInstance().HUnionDataChange(data);
							if (result != EAlly.Succ)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("战盟结盟：战盟数据变更失败 id={0}", result), null, true);
							}
						}
					}
				}
			}
		}

		
		public void UnionLeaderChangName(int roleId, string oldName, string newName)
		{
			if (!KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
				{
					SafeClientData clientData = Global.GetSafeClientDataFromLocalOrDB(roleId);
					if (clientData != null && clientData.Faction > 0)
					{
						BangHuiDetailData unionData = Global.GetBangHuiDetailData(-1, clientData.Faction, GameManager.ServerId);
						if (roleId == unionData.BZRoleID)
						{
							this.UnionDataChange(clientData.Faction, GameManager.ServerId, false, 0);
						}
					}
				}
			}
		}

		
		public bool IsAllyOpen(int unionLevel)
		{
			bool result;
			if (KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				result = false;
			}
			else
			{
				int allyOpenLevel = (int)GameManager.systemParamsList.GetParamValueIntByName("AlignZhanMengLevel", -1);
				result = (unionLevel >= allyOpenLevel);
			}
			return result;
		}

		
		private bool IsAllyMax(int numNow)
		{
			int allyMaxNum = (int)GameManager.systemParamsList.GetParamValueIntByName("AlignNum", -1);
			return numNow >= allyMaxNum;
		}

		
		
		private int AllyCostMoney
		{
			get
			{
				return (int)GameManager.systemParamsList.GetParamValueIntByName("AlignCostMoney", -1);
			}
		}

		
		private bool UnionMoneyIsMore(int myMoney)
		{
			int[] moneyArr = GameManager.systemParamsList.GetParamValueIntArrayByName("ZhanMengZiJin", ',');
			return moneyArr != null && myMoney - this.AllyCostMoney > moneyArr[0];
		}

		
		
		private int[] AllyMapArr
		{
			get
			{
				return GameManager.systemParamsList.GetParamValueIntArrayByName("AlignMap", ',');
			}
		}

		
		public bool IsAllyMap(int mapID)
		{
			return this.AllyMapArr.Contains(mapID);
		}

		
		private const int ALLY_LOG_MAX = 20;

		
		public const int _sceneType = 10004;

		
		public object _mutex = new object();

		
		private static AllyManager instance = new AllyManager();
	}
}
