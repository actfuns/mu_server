using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class LingDiCaiJiManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx
	{
		
		public static LingDiCaiJiManager getInstance()
		{
			return LingDiCaiJiManager.instance;
		}

		
		public bool initialize()
		{
			this.LoadConfig();
			return true;
		}

		
		public bool LoadConfig()
		{
			this.LoadCollectXml();
			this.InitDefaultXml();
			this.InitMap();
			this.InitShouWei();
			lock (this.CaiJiRunTimeData.Mutex)
			{
				this.CaiJiRunTimeData.DoubleOpenState.Clear();
				this.CaiJiRunTimeData.DoubleOpenState.Add(false);
				this.CaiJiRunTimeData.DoubleOpenState.Add(false);
			}
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1826, 1, 1, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1827, 1, 1, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1829, 2, 2, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1831, 1, 1, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1832, 1, 1, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1833, 1, 1, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1834, 3, 3, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1835, 1, 1, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1836, 2, 2, LingDiCaiJiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			if (!this.IsGongNengOpened())
			{
				result = false;
			}
			else
			{
				switch (nID)
				{
				case 1826:
					return this.ProcesDoubleOpenCmd(client, nID, bytes, cmdParams);
				case 1827:
					return this.ProcessMainDataCmd(client, nID, bytes, cmdParams);
				case 1829:
					return this.ProcessLingDiEnterCmd(client, nID, bytes, cmdParams);
				case 1831:
					return this.ProcessLingZhuGetDoubleOpenCmd(client, nID, bytes, cmdParams);
				case 1832:
					return this.ProcessLingZhuSetDoubleOpenCmd(client, nID, bytes, cmdParams);
				case 1833:
					return this.ProcessLingDiGetShouWeiCmd(client, nID, bytes, cmdParams);
				case 1834:
					return this.ProcessLingDiSetShouWeiCmd(client, nID, bytes, cmdParams);
				case 1835:
					return this.ProcessLingDiGetAdmireDataCmd(client, nID, bytes, cmdParams);
				case 1836:
					return this.ProcessLingDiAdmireCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		
		public void processEvent(EventObject eventObject)
		{
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
		}

		
		public void NotifyJunTuanRequest(LingDiData lingDi, int eventType)
		{
			switch (eventType)
			{
			case 28:
				if (!this.SyncLingDi(lingDi))
				{
					lock (this.CaiJiRunTimeData.Mutex)
					{
						this.CaiJiRunTimeData.LingDiDataList[lingDi.LingDiType] = lingDi;
					}
				}
				break;
			case 29:
				this.UpdateDoubleOpenTime(lingDi);
				break;
			}
		}

		
		public bool ProcesDoubleOpenCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				DateTime data = DateTime.MinValue;
				data = this.GetDoubleOpenTime();
				client.sendCmd<DateTime>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi :: 获取双倍开启时间错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		
		public bool ProcessMainDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				LingDiCaiJiMainData data = new LingDiCaiJiMainData();
				List<LingDiCaiJiData> lingDiList = new List<LingDiCaiJiData>();
				List<LingDiData> kuaFuLingDiList = new List<LingDiData>();
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (this.CaiJiRunTimeData.KuaFuSyncNeed)
					{
						lingDiList = null;
					}
					else
					{
						kuaFuLingDiList = this.CaiJiRunTimeData.LingDiDataList;
						foreach (LingDiData item in kuaFuLingDiList)
						{
							lingDiList.Add(new LingDiCaiJiData
							{
								LingDiType = item.LingDiType,
								BeginTime = item.BeginTime,
								EndTime = item.EndTime,
								HaveJunTuan = (item.RoleId > 0),
								ZhanLingName = item.JunTuanName
							});
						}
					}
					data.LingDiCaiJiDataList = lingDiList;
					int leftCount = LingDiCaiJiManager.WeeklyNum - client.ClientData.LingDiCaiJiNum;
					if (leftCount < 0)
					{
						leftCount = 0;
					}
					data.LingDiCaiJiLeftCount = leftCount;
				}
				client.sendCmd<LingDiCaiJiMainData>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi :: 获取地图信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		
		public bool ProcessLingDiEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int roleid = Convert.ToInt32(cmdParams[0]);
				int lingDiType = Convert.ToInt32(cmdParams[1]);
				if (client.ClientData.ChangeLifeCount < this.ChangeLifeLimit || (client.ClientData.ChangeLifeCount == this.ChangeLifeLimit && client.ClientData.Level < this.LevelLimit))
				{
					client.sendCmd<int>(nID, -19, false);
					return true;
				}
				long nowMs = TimeUtil.NOW();
				int result = -10;
				lock (this.DataMutex)
				{
					if (this.NextCheckNumTicks > nowMs)
					{
						client.sendCmd<int>(nID, result, false);
						return true;
					}
				}
				result = JunTuanClient.getInstance().CanEnterKuaFuMap(roleid, lingDiType);
				client.sendCmd<int>(nID, (result > 0) ? 1 : result, false);
				if (result > 0)
				{
					string dbIp;
					int dbPort;
					string logIp;
					int logPort;
					string gsIp;
					int gsPort;
					if (!KuaFuManager.getInstance().GetKuaFuDbServerInfo(result, out dbIp, out dbPort, out logIp, out logPort, out gsIp, out gsPort))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("领地采集被分配到服务器ServerId={0}, 但是找不到该跨服活动服务器", result), null, true);
						return true;
					}
					client.ClientSocket.ClientKuaFuServerLoginData.RoleId = roleid;
					client.ClientSocket.ClientKuaFuServerLoginData.GameId = (long)(lingDiType + 1);
					client.ClientSocket.ClientKuaFuServerLoginData.GameType = 22;
					client.ClientSocket.ClientKuaFuServerLoginData.EndTicks = 0L;
					client.ClientSocket.ClientKuaFuServerLoginData.FuBenSeqId = 0;
					client.ClientSocket.ClientKuaFuServerLoginData.ServerId = GameCoreInterface.getinstance().GetLocalServerId();
					client.ClientSocket.ClientKuaFuServerLoginData.ServerIp = gsIp;
					client.ClientSocket.ClientKuaFuServerLoginData.ServerPort = gsPort;
					GlobalNew.RecordSwitchKuaFuServerLog(client);
					client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
				}
				else
				{
					lock (this.DataMutex)
					{
						this.NextCheckNumTicks = nowMs + 5020L;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi :: 获取地图信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		
		public bool ProcessLingZhuGetDoubleOpenCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				LingDiData lingDi = new LingDiData();
				LingZhuOpenData result = new LingZhuOpenData
				{
					BeginTime = LingDiCaiJiManager.OpenTime,
					EndTime = LingDiCaiJiManager.CloseTime,
					OpenType = this.CanOpenDouble(client, out lingDi),
					DoubleOpenEndTime = lingDi.EndTime,
					LeftCount = LingDiCaiJiManager.OpenCountWeekly - lingDi.OpenCount
				};
				client.sendCmd<LingZhuOpenData>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi :: 获取双倍开启时间错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessLingZhuSetDoubleOpenCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				return false;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi :: 设置双倍开启时间错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		
		public bool ProcessLingDiGetShouWeiCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				LingZhuShouWeiData data = this.GetShouWeiData(client);
				client.sendCmd<LingZhuShouWeiData>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi :: 获取地图信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		
		public bool ProcessLingDiSetShouWeiCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				int index = Convert.ToInt32(cmdParams[1]);
				int useZuanShi = Convert.ToInt32(cmdParams[2]);
				int result = -1;
				DateTime now = TimeUtil.NowDateTime();
				LingDiData lingDi = new LingDiData();
				result = this.CanSetShouWei(client, index, now, useZuanShi);
				if (result != 1)
				{
					client.sendCmd(nID, result + ":" + index, false);
					return true;
				}
				int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
				int fanRongCost = (useZuanShi > 0) ? 0 : this.FanRongCost;
				result = JunTuanClient.getInstance().SetShouWeiTime(client.ClientData.RoleID, client.ClientData.Faction, lingDiType, now, index, fanRongCost);
				if (result == 1)
				{
					int zuanShiCost = 0;
					if (useZuanShi > 0)
					{
						lock (this.CaiJiRunTimeData.Mutex)
						{
							int dt = (int)(this.CaiJiRunTimeData.LingDiDataList[this.GetLingDiType(client.ClientData.MapCode)].ShouWeiList[index].FreeBuShuTime - now).TotalSeconds;
							if (dt > this.FuHuoSeconds)
							{
								dt = this.FuHuoSeconds;
							}
							zuanShiCost = this.ZuanShiCost * dt / this.FuHuoSeconds + 1;
						}
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, zuanShiCost, "复活守卫_钻石", true, true, false, DaiBiSySType.None))
						{
							result = -13;
							client.sendCmd(nID, result + ":" + index, false);
							return true;
						}
					}
					LingDiShouWeiMonsterItem shouWei = new LingDiShouWeiMonsterItem();
					lock (this.CaiJiRunTimeData.Mutex)
					{
						this.CaiJiRunTimeData.LingDiDataList[lingDiType].ShouWeiList[index].State = 2;
						this.CaiJiRunTimeData.LingDiDataList[lingDiType].ShouWeiList[index].FreeBuShuTime = DateTime.MaxValue;
						shouWei = this.CaiJiRunTimeData.ShouWeiQueue[LingDiCaiJiManager.MapCode[lingDiType]][index];
					}
					GameManager.MonsterZoneMgr.AddDynamicMonsters(LingDiCaiJiManager.MapCode[lingDiType], shouWei.Code, -1, 1, shouWei.PosX / 100, shouWei.PosY / 100, 0, 0, SceneUIClasses.LingDiCaiJi, shouWei, null);
				}
				else
				{
					if (result == -1030)
					{
						result = -14;
					}
					result = -2;
				}
				client.sendCmd(nID, result + ":" + index, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi :: 获取地图信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		
		public bool ProcessLingDiGetAdmireDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				try
				{
					int roleID = Convert.ToInt32(cmdParams[0]);
					int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
					LingDiLingZhuShowData showData = new LingDiLingZhuShowData();
					if (lingDiType == 0)
					{
						showData.AdmireCount = Global.GetRoleParamsInt32FromDB(client, "10161");
					}
					else
					{
						showData.AdmireCount = Global.GetRoleParamsInt32FromDB(client, "10164");
					}
					lock (this.CaiJiRunTimeData.Mutex)
					{
						showData.RoleData4Selector = (this.CaiJiRunTimeData.KuaFuSyncNeed ? null : this.CaiJiRunTimeData.LingZhuRoleDataList[lingDiType]);
					}
					client.sendCmd<LingDiLingZhuShowData>(nID, showData, false);
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi :: 获取地图信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		
		public bool ProcessLingDiAdmireCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				int admireType = Convert.ToInt32(cmdParams[1]);
				int result = 0;
				int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
				if (lingDiType == 2)
				{
					result = -3;
				}
				else
				{
					int moBaiType = (lingDiType == 0) ? 4 : 5;
					int nowDayId = TimeUtil.GetOffsetDayNow();
					MoBaiData MoBaiConfig = null;
					if (!Data.MoBaiDataInfoList.TryGetValue(moBaiType, out MoBaiConfig))
					{
						result = -3;
					}
					else if (client.ClientData.ChangeLifeCount < MoBaiConfig.MinZhuanSheng || (client.ClientData.ChangeLifeCount == MoBaiConfig.MinZhuanSheng && client.ClientData.Level < MoBaiConfig.MinLevel))
					{
						result = -19;
					}
					else
					{
						int maxAdmireNum = MoBaiConfig.AdrationMaxLimit;
						lock (this.CaiJiRunTimeData.Mutex)
						{
							if (this.CaiJiRunTimeData.LingDiDataList[lingDiType].RoleId == roleID)
							{
								maxAdmireNum += MoBaiConfig.ExtraNumber;
							}
						}
						string addRoleParam;
						if (lingDiType == 0)
						{
							addRoleParam = "10161";
						}
						else
						{
							addRoleParam = "10164";
						}
						int hadAdmireCount = Global.GetRoleParamsInt32FromDB(client, addRoleParam);
						if (hadAdmireCount >= maxAdmireNum)
						{
							result = -16;
						}
						else if (admireType == 1 && Global.GetTotalBindTongQianAndTongQianVal(client) < MoBaiConfig.NeedJinBi)
						{
							result = -9;
						}
						else if (admireType == 2 && client.ClientData.UserMoney < MoBaiConfig.NeedZuanShi)
						{
							result = -10;
						}
						else
						{
							double nRate = (client.ClientData.ChangeLifeCount == 0) ? 1.0 : Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount];
							if (admireType == 1)
							{
								if (!Global.SubBindTongQianAndTongQian(client, MoBaiConfig.NeedJinBi, "膜拜军团领主" + lingDiType))
								{
									result = -9;
									goto IL_4B8;
								}
								long nExp = (long)(nRate * (double)MoBaiConfig.JinBiExpAward);
								if (nExp > 0L)
								{
									GameManager.ClientMgr.ProcessRoleExperience(client, nExp, true, true, false, "none");
								}
								if (MoBaiConfig.JinBiZhanGongAward > 0)
								{
									GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref MoBaiConfig.JinBiZhanGongAward, AddBangGongTypes.JunTuanLingZhuMoBai, 0);
								}
								if (MoBaiConfig.LingJingAwardByJinBi > 0)
								{
									GameManager.ClientMgr.ModifyMUMoHeValue(client, MoBaiConfig.LingJingAwardByJinBi, "膜拜军团领主" + lingDiType, true, true, false);
								}
								if (MoBaiConfig.ShenLiJingHuaByJinBi > 0)
								{
									GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, MoBaiConfig.ShenLiJingHuaByJinBi, "膜拜军团领主" + lingDiType, true, true);
								}
							}
							if (admireType == 2)
							{
								GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, MoBaiConfig.NeedZuanShi, "膜拜军团领主" + lingDiType, true, true, false, DaiBiSySType.None);
								int nExp2 = (int)(nRate * (double)MoBaiConfig.ZuanShiExpAward);
								if (nExp2 > 0)
								{
									GameManager.ClientMgr.ProcessRoleExperience(client, (long)nExp2, true, true, false, "none");
								}
								if (MoBaiConfig.ZuanShiZhanGongAward > 0)
								{
									GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref MoBaiConfig.ZuanShiZhanGongAward, AddBangGongTypes.JunTuanLingZhuMoBai, 0);
								}
								if (MoBaiConfig.LingJingAwardByZuanShi > 0)
								{
									GameManager.ClientMgr.ModifyMUMoHeValue(client, MoBaiConfig.LingJingAwardByZuanShi, "膜拜军团领主" + lingDiType, true, true, false);
								}
								if (MoBaiConfig.ShenLiJingHuaByZuanShi > 0)
								{
									GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, MoBaiConfig.ShenLiJingHuaByZuanShi, "膜拜军团领主" + lingDiType, true, true);
								}
							}
							hadAdmireCount++;
							Global.SaveRoleParamsInt64ValueToDB(client, addRoleParam, (long)hadAdmireCount, true);
						}
					}
				}
				IL_4B8:
				client.sendCmd(nID, result.ToString(), false);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi :: 雕像膜拜信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return true;
		}

		
		public void LoadCollectXml()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(LingDiCaiJiConsts.CollectMonster);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					IEnumerable<XElement> nodes = xml.Elements();
					this.CollectMonsterXml.Clear();
					foreach (XElement xmlItem in nodes)
					{
						int monsterId = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MonsterID", "0"));
						this.CollectMonsterXml[monsterId] = new ManorCollectMonster
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0")),
							MonsterID = monsterId,
							Type = (CryStealType)Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Type", "0")),
							Name = Global.GetDefAttributeStr(xmlItem, "Name", ""),
							GatherTime = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GatherTime", "0")),
							FuHuoTime = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "FuHuoTime", "0")),
							ShenLiJingHua = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ShenLiJingHua", "0")),
							YuanSuFenMo = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "YuanSuFenMo", "0")),
							YingShiFenMo = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "YingShiFenMo", "0")),
							LangHunFenMo = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "LangHunFenMo", "0"))
						};
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。ex:{1}", fileName, ex.Message), ex, true);
			}
		}

		
		public int GetCaiJiMonsterTime(GameClient client, Monster monster)
		{
			LingDiCrystalMonsterItem tag = (monster != null) ? (monster.Tag as LingDiCrystalMonsterItem) : null;
			int result;
			if (tag == null)
			{
				result = -200;
			}
			else
			{
				int caiJiNum = Global.GetRoleParamsInt32FromDB(client, "10158");
				if (caiJiNum >= LingDiCaiJiManager.WeeklyNum)
				{
					result = -5;
				}
				else
				{
					int gatherTime = this.CollectMonsterXml[tag.Code].GatherTime;
					BufferData bufferData = Global.GetBufferDataByID(client.ClientData, 115);
					if (null != bufferData)
					{
						gatherTime *= 100 - (int)bufferData.BufferVal;
						gatherTime /= 100;
					}
					result = ((gatherTime > 1) ? gatherTime : 1);
				}
			}
			return result;
		}

		
		public void OnCaiJiFinish(GameClient client, Monster monster)
		{
			LingDiCrystalMonsterItem item = (monster != null) ? (monster.Tag as LingDiCrystalMonsterItem) : null;
			if (null != item)
			{
				try
				{
					ManorCollectMonster collectItem = this.CollectMonsterXml[item.Code];
					int AwardRate = 1;
					DateTime now = TimeUtil.NowDateTime();
					int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
					if (lingDiType != 2)
					{
						lock (this.CaiJiRunTimeData.Mutex)
						{
							if (now > this.CaiJiRunTimeData.LingDiDataList[lingDiType].BeginTime && now < this.CaiJiRunTimeData.LingDiDataList[lingDiType].EndTime)
							{
								AwardRate = LingDiCaiJiManager.BeiLv;
							}
							if (client.ClientData.JunTuanId != 0 && client.ClientData.JunTuanId == this.CaiJiRunTimeData.LingDiDataList[lingDiType].JunTuanId)
							{
								DateTime joinTime = Global.GetRoleParamsDateTimeFromDB(client, "10182");
								DateTime endFightTime = KarenBattleManager.getInstance().GetLastStartTime(this.ConvertCaiJiLingDiTypeToMapCode(lingDiType));
								if (joinTime < endFightTime)
								{
									AwardRate += LingDiCaiJiManager.ZhanLingBeiLv;
								}
							}
						}
						int shenLiJingHua = collectItem.ShenLiJingHua * AwardRate;
						int yuanSuFenMo = collectItem.YuanSuFenMo * AwardRate;
						int yingShiFenMo = collectItem.YingShiFenMo * AwardRate;
						int langHunFenMo = collectItem.LangHunFenMo * AwardRate;
						if (shenLiJingHua > 0)
						{
							GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, shenLiJingHua, "领地采集_神力精华", true, true);
						}
						if (yuanSuFenMo > 0)
						{
							GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, yuanSuFenMo, "领地采集获得元素粉末", true, false);
						}
						if (yingShiFenMo > 0)
						{
							GameManager.FluorescentGemMgr.AddFluorescentPoint(client, yingShiFenMo, "领地采集获得荧光粉末", true);
						}
						if (langHunFenMo > 0)
						{
							GameManager.ClientMgr.ModifyLangHunFenMoValue(client, langHunFenMo, "领地采集活动狼魂粉末", true, true);
						}
						client.ClientData.LingDiCaiJiNum++;
						Global.SaveRoleParamsInt32ValueToDB(client, "10158", client.ClientData.LingDiCaiJiNum, true);
						int leftCount = LingDiCaiJiManager.WeeklyNum - client.ClientData.LingDiCaiJiNum;
						if (leftCount < 0)
						{
							leftCount = 0;
						}
						client.sendCmd(1828, leftCount.ToString(), false);
						int mapCode = client.ClientData.MapCode;
						long tick = TimeUtil.NOW() + (long)(collectItem.FuHuoTime * 1000);
						lock (this.CaiJiRunTimeData.Mutex)
						{
							SortedList<long, List<object>> monsterSortList = null;
							List<object> monsterList = null;
							if (collectItem.Type == CryStealType.Chao)
							{
								if (!this.CaiJiRunTimeData.ChaoShuiJingQueue.TryGetValue(mapCode, out monsterSortList) || null == monsterSortList)
								{
									monsterSortList = new SortedList<long, List<object>>();
									this.CaiJiRunTimeData.ChaoShuiJingQueue[mapCode] = monsterSortList;
								}
							}
							else if (!this.CaiJiRunTimeData.NormalShuiJingQueue.TryGetValue(mapCode, out monsterSortList) || null == monsterSortList)
							{
								monsterSortList = new SortedList<long, List<object>>();
								this.CaiJiRunTimeData.NormalShuiJingQueue[mapCode] = monsterSortList;
							}
							if (!monsterSortList.TryGetValue(tick, out monsterList) || null == monsterList)
							{
								monsterList = new List<object>();
								monsterSortList.Add(tick, monsterList);
							}
							monsterList.Add(item);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi :: 发放采集奖励信息错误 ex:{0}", ex.Message), null, true);
				}
			}
		}

		
		public bool IsOpposition(GameClient me, int monsterType)
		{
			bool result;
			if (me.ClientData.HideGM > 0)
			{
				result = false;
			}
			else
			{
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (this.CaiJiRunTimeData.KuaFuSyncNeed)
					{
						result = false;
					}
					else
					{
						int lingDiType = this.GetLingDiType(me.ClientData.MapCode);
						result = (me.ClientData.JunTuanId != this.CaiJiRunTimeData.LingDiDataList[lingDiType].JunTuanId);
					}
				}
			}
			return result;
		}

		
		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			if (monster.MonsterType == 2101 || monster.MonsterType == 2102)
			{
				LingDiShouWeiMonsterItem tagInfo = monster.Tag as LingDiShouWeiMonsterItem;
				if (null != tagInfo)
				{
					if (monster.HandledDead)
					{
						tagInfo.ShouWeiData.State = 1;
						tagInfo.ShouWeiData.FreeBuShuTime = TimeUtil.NowDateTime().AddSeconds((double)this.FuHuoSeconds);
						int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
						lock (this.CaiJiRunTimeData.Mutex)
						{
							this.CaiJiRunTimeData.ShouWeiQueue[client.ClientData.MapCode][tagInfo.ID - 1] = tagInfo;
							this.CaiJiRunTimeData.LingDiDataList[lingDiType].ShouWeiList[tagInfo.ID - 1] = tagInfo.ShouWeiData;
							JunTuanClient.getInstance().SetShouWei(lingDiType, this.CaiJiRunTimeData.LingDiDataList[lingDiType].ShouWeiList);
						}
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "领地守卫", "守卫被击杀", client.ClientData.MapCode + ":" + tagInfo.ID, client.ClientData.RoleName, "被击杀", 1, client.ClientData.ZoneID, client.strUserID, 0, client.ServerId, null);
					}
				}
			}
		}

		
		public DateTime GetDoubleOpenTime()
		{
			DateTime data = DateTime.MinValue;
			DateTime now = TimeUtil.NowDateTime();
			List<LingDiData> lingDiList = new List<LingDiData>();
			lock (this.CaiJiRunTimeData.Mutex)
			{
				lingDiList = this.CaiJiRunTimeData.LingDiDataList;
				if (lingDiList == null || lingDiList.Count < 2)
				{
					data = DateTime.MinValue;
				}
				else if (lingDiList[0].EndTime.DayOfYear != DateTime.Today.DayOfYear)
				{
					data = ((lingDiList[1].EndTime.DayOfYear == DateTime.Today.DayOfYear && now > lingDiList[1].BeginTime) ? lingDiList[1].EndTime : DateTime.MinValue);
				}
				else if (lingDiList[1].EndTime.DayOfYear != DateTime.Today.DayOfYear)
				{
					data = ((lingDiList[0].EndTime.DayOfYear == DateTime.Today.DayOfYear && now > lingDiList[0].BeginTime) ? lingDiList[0].EndTime : DateTime.MinValue);
				}
				else if (now < lingDiList[0].BeginTime)
				{
					data = ((now > lingDiList[1].BeginTime) ? lingDiList[1].EndTime : DateTime.MinValue);
				}
				else if (now < lingDiList[1].BeginTime)
				{
					data = ((now > lingDiList[0].BeginTime) ? lingDiList[0].EndTime : DateTime.MinValue);
				}
				else
				{
					data = ((lingDiList[0].EndTime < lingDiList[1].EndTime) ? lingDiList[1].EndTime : lingDiList[0].EndTime);
				}
			}
			return data;
		}

		
		public LingZhuShouWeiData GetShouWeiData(GameClient client)
		{
			LingZhuShouWeiData ret = new LingZhuShouWeiData
			{
				Result = -8,
				ShouWeiList = new List<LingDiShouWeiData>()
			};
			int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
			LingZhuShouWeiData result;
			if (lingDiType == 2)
			{
				result = new LingZhuShouWeiData
				{
					Result = -9
				};
			}
			else
			{
				List<LingDiData> lingDiList = new List<LingDiData>();
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (this.CaiJiRunTimeData.KuaFuSyncNeed)
					{
						return new LingZhuShouWeiData
						{
							Result = -1
						};
					}
					lingDiList = this.CaiJiRunTimeData.LingDiDataList;
					if (lingDiList.Count < 2 || null == lingDiList[lingDiType])
					{
						return new LingZhuShouWeiData
						{
							Result = -8
						};
					}
					if (client.ClientData.RoleID != lingDiList[lingDiType].RoleId)
					{
						return new LingZhuShouWeiData
						{
							Result = -2
						};
					}
					List<LingDiShouWei> shouWeiList = lingDiList[lingDiType].ShouWeiList;
					ret.Result = 1;
					DateTime now = TimeUtil.NowDateTime();
					foreach (LingDiShouWei item in shouWeiList)
					{
						int zuanShiCost = 0;
						if (item.FreeBuShuTime > now)
						{
							int dt = (int)(item.FreeBuShuTime - now).TotalSeconds;
							if (dt > this.FuHuoSeconds)
							{
								dt = this.FuHuoSeconds;
							}
							zuanShiCost = this.ZuanShiCost * dt / this.FuHuoSeconds + 1;
						}
						LingDiShouWeiData addItem = new LingDiShouWeiData
						{
							State = item.State,
							FreeBuShuTime = item.FreeBuShuTime,
							ZuanShiCost = zuanShiCost
						};
						ret.ShouWeiList.Add(addItem);
					}
				}
				result = ret;
			}
			return result;
		}

		
		public void UpdateDoubleOpenTime(LingDiData openItem)
		{
			string strHint = "";
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				lock (this.CaiJiRunTimeData.Mutex)
				{
					this.CaiJiRunTimeData.LingDiDataList[openItem.LingDiType].EndTime = openItem.EndTime;
					this.CaiJiRunTimeData.LingDiDataList[openItem.LingDiType].BeginTime = openItem.BeginTime;
					if (now < openItem.BeginTime || now > openItem.EndTime)
					{
						return;
					}
					if (openItem.LingDiType == 0)
					{
						strHint = GLang.GetLang(2612, new object[0]);
					}
					else if (openItem.LingDiType == 1)
					{
						strHint = GLang.GetLang(2613, new object[0]);
					}
				}
				if (strHint != "")
				{
					int index = 0;
					GameClient gc;
					while ((gc = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gc, strHint, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0);
						if (this.GetLingDiType(gc.ClientData.MapCode) == openItem.LingDiType)
						{
							gc.sendCmd<DateTime>(1830, openItem.EndTime, false);
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		
		public void OnLoadDynamicMonsters(int mapCode, Monster monster)
		{
			LingDiShouWeiMonsterItem tag = null;
			if (monster != null && (tag = (monster.Tag as LingDiShouWeiMonsterItem)) != null)
			{
				lock (this.CaiJiRunTimeData.Mutex)
				{
					this.CaiJiRunTimeData.ShouWeiMonster[mapCode][tag.ID] = monster;
				}
			}
		}

		
		public int SetLingZhu(int lingDiType, int rid, int junTuanId, string junTuanName, RoleData4Selector client)
		{
			int result;
			try
			{
				int lingZhu = 0;
				if (rid != 0 && junTuanId != 0)
				{
					lingZhu = 1;
				}
				byte[] roledata = DataHelper.ObjectToBytes<RoleData4Selector>(client);
				int ret = JunTuanClient.getInstance().SetLingZhu(rid, lingDiType, junTuanId, junTuanName, lingZhu, roledata);
				if (ret <= 0)
				{
					result = ret;
				}
				else
				{
					ret = JunTuanClient.getInstance().UpdateJunTuanLingDi(junTuanId, lingDiType + 1);
					if (ret <= 0)
					{
						result = ret;
					}
					else
					{
						lock (this.CaiJiRunTimeData.Mutex)
						{
							this.CaiJiRunTimeData.KuaFuSyncNeed = true;
						}
						result = ret;
					}
				}
			}
			catch
			{
				result = 0;
			}
			return result;
		}

		
		public void InitRoleLingDiCaiJiData(GameClient client, bool isNewDay)
		{
			client.ClientData.LingDiCaiJiNum = Global.GetRoleParamsInt32FromDB(client, "10158");
			DateTime now = TimeUtil.NowDateTime();
			DateTime endTime = this.GetDoubleOpenTime();
			if (now < endTime)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					5,
					2,
					GLang.GetLang(2614, new object[0]),
					0
				});
				client.SendCmdAfterStartPlayGame(194, strcmd);
			}
			if (isNewDay)
			{
				Global.SaveRoleParamsInt32ValueToDB(client, "10161", 0, true);
				Global.SaveRoleParamsInt32ValueToDB(client, "10164", 0, true);
				string strParam = Global.GetRoleParamByName(client, "RoleLoginRecorde");
				string[] strFields = (strParam == null) ? null : strParam.Split(new char[]
				{
					','
				});
				int recordDayID;
				if (strFields != null && strFields.Length == 2)
				{
					recordDayID = Convert.ToInt32(strFields[0]);
				}
				else
				{
					recordDayID = 0;
				}
				int currDayID = Global.GetOffsetDayNow();
				int currDayWek = (int)((TimeUtil.NowDateTime().DayOfWeek == DayOfWeek.Sunday) ? ((DayOfWeek)7) : TimeUtil.NowDateTime().DayOfWeek);
				if (currDayWek - currDayID + recordDayID < 1)
				{
					if (client.ClientData.LingDiCaiJiNum > 0)
					{
						client.ClientData.LingDiCaiJiNum = 0;
					}
					Global.SaveRoleParamsInt32ValueToDB(client, "10158", client.ClientData.LingDiCaiJiNum, true);
					SceneUIClasses mapCode = Global.GetMapSceneType(client.ClientData.MapCode);
					if (mapCode == SceneUIClasses.LingDiCaiJi)
					{
						client.sendCmd(1828, (LingDiCaiJiManager.WeeklyNum - client.ClientData.LingDiCaiJiNum).ToString(), false);
					}
				}
			}
		}

		
		public void CleanKuaFuData()
		{
			lock (this.CaiJiRunTimeData.Mutex)
			{
				this.CaiJiRunTimeData.LingDiDataList.Clear();
				this.CaiJiRunTimeData.KuaFuSyncNeed = true;
			}
		}

		
		public bool SyncKuaFuData()
		{
			try
			{
				List<LingDiData> lingDiList = JunTuanClient.getInstance().GetLingDiData();
				if (lingDiList == null || lingDiList.Count < 2)
				{
					return true;
				}
				for (int i = 0; i < lingDiList.Count; i++)
				{
					LingDiData lingDi = lingDiList[i];
					if (this.SyncLingDi(lingDi))
					{
						return true;
					}
					lock (this.CaiJiRunTimeData.Mutex)
					{
						if (this.CaiJiRunTimeData.LingDiDataList == null || this.CaiJiRunTimeData.LingDiDataList.Count < 2)
						{
							this.CaiJiRunTimeData.LingDiDataList = lingDiList;
						}
						this.CaiJiRunTimeData.LingDiDataList[lingDi.LingDiType] = lingDi;
					}
				}
				lock (this.CaiJiRunTimeData.Mutex)
				{
					this.CaiJiRunTimeData.KuaFuSyncNeed = false;
				}
				return false;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi :: 同步中心信息错误 ex:{0}", ex.Message), null, true);
			}
			return true;
		}

		
		public bool SyncLingDi(LingDiData lingDi)
		{
			try
			{
				if (lingDi == null)
				{
					return true;
				}
				RoleData4Selector lingZhuData = (lingDi.RoleData == null) ? null : DataHelper.BytesToObject<RoleData4Selector>(lingDi.RoleData, 0, lingDi.RoleData.Length);
				try
				{
					NPC npc = NPCGeneralManager.FindNPC(LingDiCaiJiManager.MapCode[lingDi.LingDiType], FakeRoleNpcId.JunTuanLingZhu + lingDi.LingDiType);
					if (null != npc)
					{
						npc.ShowNpc = true;
						GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
						FakeRoleTypes type = (lingDi.LingDiType == 0) ? FakeRoleTypes.LingDiDiGongLingZhu : FakeRoleTypes.LingDiHuangMoLingZhu;
						FakeRoleManager.ProcessDelFakeRoleByType(type, true);
						if (lingZhuData != null)
						{
							lingZhuData.BuffFashionID = ((lingDi.LingDiType == 0) ? 10010 : 10008);
							npc.ShowNpc = false;
							FakeRoleManager.ProcessNewFakeRole(lingZhuData, LingDiCaiJiManager.MapCode[lingDi.LingDiType], type, (int)npc.CurrentDir, (int)npc.CurrentPos.X, (int)npc.CurrentPos.Y, FakeRoleNpcId.JunTuanLingZhu + lingDi.LingDiType);
						}
						else
						{
							if (lingDi.JunTuanId > 0)
							{
								RoleDataEx dbRd = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, lingDi.RoleId), 0);
								if (dbRd != null && dbRd.RoleID > 0)
								{
									RoleData4Selector leaderShowInfo = Global.RoleDataEx2RoleData4Selector(dbRd);
									LingDiCaiJiManager.getInstance().SetLingZhu(lingDi.LingDiType, dbRd.RoleID, lingDi.JunTuanId, lingDi.JunTuanName, leaderShowInfo);
								}
								return true;
							}
							GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
						}
					}
					lock (this.CaiJiRunTimeData.Mutex)
					{
						if (this.CaiJiRunTimeData.LingZhuRoleDataList.Count < 2)
						{
							this.CaiJiRunTimeData.LingZhuRoleDataList = new List<RoleData4Selector>();
							this.CaiJiRunTimeData.LingZhuRoleDataList.Add(null);
							this.CaiJiRunTimeData.LingZhuRoleDataList.Add(null);
						}
						this.CaiJiRunTimeData.LingZhuRoleDataList[lingDi.LingDiType] = lingZhuData;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi :: 设置领主雕像信息错误 ex:{0}", ex.Message), null, true);
					return true;
				}
				lock (this.CaiJiRunTimeData.Mutex)
				{
					List<LingDiShouWeiMonsterItem> shouWeiList = new List<LingDiShouWeiMonsterItem>();
					if (!this.CaiJiRunTimeData.ShouWeiQueue.TryGetValue(LingDiCaiJiManager.MapCode[lingDi.LingDiType], out shouWeiList))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi :: 守卫配置错误 lingDiType:{0}, mapcode:{1}", lingDi.LingDiType, LingDiCaiJiManager.MapCode[lingDi.LingDiType]), null, true);
					}
					for (int i = 0; i < lingDi.ShouWeiList.Count; i++)
					{
						shouWeiList[i].ShouWeiData = lingDi.ShouWeiList[i];
						Dictionary<int, Monster> mosterDic = this.CaiJiRunTimeData.ShouWeiMonster[LingDiCaiJiManager.MapCode[lingDi.LingDiType]];
						Monster _shouwei = null;
						if (mosterDic.TryGetValue(shouWeiList[i].ID, out _shouwei))
						{
							this.CaiJiRunTimeData.ShouWeiMonster[LingDiCaiJiManager.MapCode[lingDi.LingDiType]].Remove(shouWeiList[i].ID);
							GameManager.MonsterMgr.DeadMonsterImmediately(_shouwei);
						}
						if (lingDi.ShouWeiList[i].State == 2 && lingDi.RoleId > 0)
						{
							Monster tmp = GameManager.MonsterZoneMgr.AddDynamicMonsters(LingDiCaiJiManager.MapCode[lingDi.LingDiType], shouWeiList[i].Code, -1, 1, shouWeiList[i].PosX / 100, shouWeiList[i].PosY / 100, 0, 0, SceneUIClasses.LingDiCaiJi, shouWeiList[i], null);
						}
					}
				}
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (lingDi.RoleId <= 0)
					{
						DateTime now = TimeUtil.NowDateTime();
						foreach (DoubleOpenItem item in this.DoubleOpenTimeDefaultList)
						{
							if (now.DayOfWeek == (DayOfWeek)item.WeekDay)
							{
								if (now > DateTime.Today.AddTicks(item.DayStartTicks) && now < DateTime.Today.AddTicks(item.DayEndTicks))
								{
									lingDi.BeginTime = DateTime.Today.AddTicks(item.DayStartTicks);
									lingDi.EndTime = DateTime.Today.AddTicks(item.DayEndTicks);
								}
								break;
							}
						}
					}
				}
				int oldRid = 0;
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (this.CaiJiRunTimeData.LingDiDataList != null && this.CaiJiRunTimeData.LingDiDataList.Count > lingDi.LingDiType)
					{
						if (this.CaiJiRunTimeData.LingDiDataList[lingDi.LingDiType] != null)
						{
							oldRid = this.CaiJiRunTimeData.LingDiDataList[lingDi.LingDiType].RoleId;
						}
					}
				}
				if (oldRid == lingDi.RoleId)
				{
					return false;
				}
				int index = 0;
				BufferItemTypes lingZhuBuffType = BufferItemTypes.DiGongLingZhu;
				BufferItemTypes chenMingBuffType = BufferItemTypes.DiGongChenMin;
				if (lingDi.LingDiType == 1)
				{
					lingZhuBuffType = BufferItemTypes.HuangMoLingZhu;
					chenMingBuffType = BufferItemTypes.HuangMoChenMin;
				}
				GameClient gc;
				while ((gc = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
				{
					if (this.GetLingDiType(gc.ClientData.MapCode) == lingDi.LingDiType && gc.ClientData.JunTuanId == lingDi.JunTuanId && lingDi.JunTuanId != 0)
					{
						Global.UpdateBufferData(gc, BufferItemTypes.JunTuanCaiJiBuff, this.BuffParam, 1, false);
						if (oldRid == lingDi.RoleId)
						{
							continue;
						}
						if (gc.ClientData.RoleID == lingDi.RoleId)
						{
							gc.sendCmd(1837, "1", false);
						}
					}
					else
					{
						if (this.GetLingDiType(gc.ClientData.MapCode) == lingDi.LingDiType && gc.ClientData.RoleID == oldRid)
						{
							gc.sendCmd(1837, "0", false);
						}
						Global.RemoveBufferData(gc, 115);
					}
					if (gc.ClientData.JunTuanId == lingDi.JunTuanId && lingDi.JunTuanId != 0)
					{
						if (gc.ClientData.RoleID == lingDi.RoleId)
						{
							Global.UpdateBufferData(gc, lingZhuBuffType, new double[]
							{
								1.0
							}, 1, false);
						}
						else
						{
							Global.UpdateBufferData(gc, chenMingBuffType, new double[]
							{
								1.0
							}, 1, false);
						}
					}
					else if (gc.ClientData.RoleID == oldRid)
					{
						GameClient client = gc;
						BufferItemTypes bufferItemType = lingZhuBuffType;
						double[] actionParams = new double[1];
						Global.UpdateBufferData(client, bufferItemType, actionParams, 1, false);
					}
					else
					{
						GameClient client2 = gc;
						BufferItemTypes bufferItemType2 = chenMingBuffType;
						double[] actionParams = new double[1];
						Global.UpdateBufferData(client2, bufferItemType2, actionParams, 1, false);
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi :: 同步中心信息错误 ex:{0}", ex.Message), null, true);
			}
			return true;
		}

		
		private bool IsGongNengOpened()
		{
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot4);
		}

		
		public void TimerProc()
		{
			if (this.IsGongNengOpened())
			{
				DateTime now = TimeUtil.NowDateTime();
				long nowMs = TimeUtil.NOW();
				if (this.NextSyncTicks < nowMs)
				{
					if (JunTuanClient.getInstance().GetClientCacheItems(GameCoreInterface.getinstance().GetLocalServerId()))
					{
						this.SetSync();
					}
				}
				bool needSync = true;
				lock (this.CaiJiRunTimeData.Mutex)
				{
					needSync = this.CaiJiRunTimeData.KuaFuSyncNeed;
				}
				lock (this.DataMutex)
				{
					if (needSync && this.NextSyncTicks < nowMs)
					{
						this.NextSyncTicks = nowMs + 5020L;
						if (this.SyncKuaFuData())
						{
							return;
						}
					}
				}
				try
				{
					lock (this.DataMutex)
					{
						if (this.NextHeartBeatTicks > nowMs)
						{
							return;
						}
						this.NextHeartBeatTicks = nowMs + 1020L;
					}
					lock (this.CaiJiRunTimeData.Mutex)
					{
						for (int i = 0; i < LingDiCaiJiManager.MapCode.Length; i++)
						{
							int mapCode = LingDiCaiJiManager.MapCode[i];
							while (this.CaiJiRunTimeData.NormalShuiJingQueue[mapCode].Count > 0)
							{
								KeyValuePair<long, List<object>> pair = this.CaiJiRunTimeData.NormalShuiJingQueue[mapCode].First<KeyValuePair<long, List<object>>>();
								if (nowMs < pair.Key)
								{
									break;
								}
								try
								{
									foreach (object obj in pair.Value)
									{
										if (obj is LingDiCrystalMonsterItem)
										{
											LingDiCrystalMonsterItem crystal = obj as LingDiCrystalMonsterItem;
											if (this.CollectMonsterXml.ContainsKey(crystal.Code))
											{
												GameManager.MonsterZoneMgr.AddDynamicMonsters(mapCode, crystal.Code, -1, 1, crystal.PosX / 100, crystal.PosY / 100, 0, 0, SceneUIClasses.LingDiCaiJi, crystal, null);
											}
										}
									}
								}
								finally
								{
									this.CaiJiRunTimeData.NormalShuiJingQueue[mapCode].RemoveAt(0);
								}
							}
							if (this.CaiJiRunTimeData.LingDiDataList != null && this.CaiJiRunTimeData.LingDiDataList.Count != 0)
							{
								int lingDiType = this.GetLingDiType(mapCode);
								if (!(now < this.CaiJiRunTimeData.LingDiDataList[lingDiType].BeginTime) && !(now > this.CaiJiRunTimeData.LingDiDataList[lingDiType].EndTime))
								{
									while (this.CaiJiRunTimeData.ChaoShuiJingQueue[mapCode].Count > 0)
									{
										KeyValuePair<long, List<object>> pair = this.CaiJiRunTimeData.ChaoShuiJingQueue[mapCode].First<KeyValuePair<long, List<object>>>();
										if (nowMs < pair.Key)
										{
											break;
										}
										try
										{
											foreach (object obj in pair.Value)
											{
												if (obj is LingDiCrystalMonsterItem)
												{
													LingDiCrystalMonsterItem crystal = obj as LingDiCrystalMonsterItem;
													if (this.CollectMonsterXml.ContainsKey(crystal.Code))
													{
														GameManager.MonsterZoneMgr.AddDynamicMonsters(mapCode, crystal.Code, -1, 1, crystal.PosX / 100, crystal.PosY / 100, 0, 0, SceneUIClasses.LingDiCaiJi, crystal, null);
													}
												}
											}
										}
										finally
										{
											this.CaiJiRunTimeData.ChaoShuiJingQueue[mapCode].RemoveAt(0);
										}
									}
								}
							}
						}
						for (int i = 0; i < LingDiCaiJiManager.MapCode.Length; i++)
						{
							if (this.CaiJiRunTimeData.LingDiDataList != null && this.CaiJiRunTimeData.LingDiDataList.Count != 0)
							{
								int lingDiType = this.GetLingDiType(LingDiCaiJiManager.MapCode[i]);
								foreach (DoubleOpenItem item in this.DoubleOpenTimeDefaultList)
								{
									if (now.DayOfWeek == (DayOfWeek)item.WeekDay)
									{
										if (!this.CaiJiRunTimeData.DoubleOpenState[lingDiType])
										{
											if (now > DateTime.Today.AddTicks(item.DayStartTicks) && now < DateTime.Today.AddTicks(item.DayEndTicks))
											{
												this.CaiJiRunTimeData.LingDiDataList[lingDiType].BeginTime = DateTime.Today.AddTicks(item.DayStartTicks);
												this.CaiJiRunTimeData.LingDiDataList[lingDiType].EndTime = DateTime.Today.AddTicks(item.DayEndTicks);
												this.UpdateDoubleOpenTime(this.CaiJiRunTimeData.LingDiDataList[lingDiType]);
												this.CaiJiRunTimeData.DoubleOpenState[lingDiType] = true;
											}
										}
										else if (now > DateTime.Today.AddTicks(item.DayEndTicks))
										{
											this.CaiJiRunTimeData.DoubleOpenState[lingDiType] = false;
										}
										break;
									}
								}
								if (this.CaiJiRunTimeData.LingDiDataList[lingDiType].RoleId > 0)
								{
									bool sync = false;
									List<LingDiShouWeiMonsterItem> shouWeiList = new List<LingDiShouWeiMonsterItem>();
									if (!this.CaiJiRunTimeData.ShouWeiQueue.TryGetValue(LingDiCaiJiManager.MapCode[i], out shouWeiList))
									{
										return;
									}
									for (int j = 0; j < shouWeiList.Count; j++)
									{
										if (shouWeiList[j].ShouWeiData.State == 1 && shouWeiList[j].ShouWeiData.FreeBuShuTime < now)
										{
											sync = true;
											shouWeiList[j].ShouWeiData.State = 2;
											this.CaiJiRunTimeData.LingDiDataList[lingDiType].ShouWeiList[j] = shouWeiList[j].ShouWeiData;
											Monster tmp = GameManager.MonsterZoneMgr.AddDynamicMonsters(LingDiCaiJiManager.MapCode[i], shouWeiList[j].Code, -1, 1, shouWeiList[j].PosX / 100, shouWeiList[j].PosY / 100, 0, 0, SceneUIClasses.LingDiCaiJi, shouWeiList[j], null);
										}
									}
									if (sync)
									{
										JunTuanClient.getInstance().SetShouWei(lingDiType, this.CaiJiRunTimeData.LingDiDataList[lingDiType].ShouWeiList);
									}
								}
							}
						}
						lock (this.DataMutex)
						{
							if (this.NextSyncRoleNumTicks > nowMs)
							{
								return;
							}
							this.NextSyncRoleNumTicks = nowMs + 5020L;
						}
						for (int i = 0; i < LingDiCaiJiManager.MapCode.Length; i++)
						{
							int roleNum = GameManager.ClientMgr.GetMapClientsCount(LingDiCaiJiManager.MapCode[i]);
							int serverId = JunTuanClient.getInstance().UpdateMapRoleNum(this.GetLingDiType(LingDiCaiJiManager.MapCode[i]), roleNum, GameCoreInterface.getinstance().GetLocalServerId());
							if (serverId > 0 && serverId != GameCoreInterface.getinstance().GetLocalServerId())
							{
								this.NextSyncRoleNumTicks = nowMs + 180000L;
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi :: 刷怪错误ex:{0}", ex.Message), null, true);
				}
			}
		}

		
		public int GetLingDiType(int mapCode)
		{
			for (int i = 0; i < LingDiCaiJiManager.MapCode.Length; i++)
			{
				if (mapCode == LingDiCaiJiManager.MapCode[i])
				{
					return i;
				}
			}
			return 2;
		}

		
		public int CanOpenDouble(GameClient client, out LingDiData lingDi)
		{
			lingDi = new LingDiData
			{
				EndTime = DateTime.MinValue,
				OpenCount = int.MaxValue
			};
			int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
			int result;
			if (lingDiType == 2)
			{
				result = -9;
			}
			else
			{
				List<LingDiData> lingDiList = new List<LingDiData>();
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (this.CaiJiRunTimeData.KuaFuSyncNeed)
					{
						result = -1;
					}
					else
					{
						lingDiList = this.CaiJiRunTimeData.LingDiDataList;
						if (lingDiList.Count < 2 || null == lingDiList[lingDiType])
						{
							result = -8;
						}
						else
						{
							lingDi = lingDiList[lingDiType];
							if (lingDi.RoleId != client.ClientData.RoleID || lingDi.JunTuanId == 0)
							{
								result = -2;
							}
							else
							{
								DateTime now = TimeUtil.NowDateTime();
								if (now > lingDi.BeginTime && now < lingDi.EndTime)
								{
									result = -4;
								}
								else if (LingDiCaiJiManager.OpenCountWeekly - lingDi.OpenCount < 1)
								{
									result = -5;
								}
								else
								{
									result = 1;
								}
							}
						}
					}
				}
			}
			return result;
		}

		
		public int CanSetShouWei(GameClient client, int index, DateTime now, int useZhuanShi)
		{
			int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
			int result;
			if (lingDiType == 2)
			{
				result = -9;
			}
			else
			{
				List<LingDiData> lingDiList = new List<LingDiData>();
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (this.CaiJiRunTimeData.KuaFuSyncNeed)
					{
						return -1;
					}
					lingDiList = this.CaiJiRunTimeData.LingDiDataList;
					if (lingDiList.Count < 2)
					{
						return -1;
					}
					LingDiData lingDi = lingDiList[lingDiType];
					if (lingDi.RoleId != client.ClientData.RoleID)
					{
						return -2;
					}
					if (lingDi.ShouWeiList.Count < index + 1)
					{
						return -11;
					}
					if (lingDi.ShouWeiList[index].State == 2)
					{
						return -15;
					}
					if (lingDi.ShouWeiList[index].FreeBuShuTime > now)
					{
						int dt = (int)(lingDi.ShouWeiList[index].FreeBuShuTime - now).TotalSeconds;
						if (dt > this.FuHuoSeconds)
						{
							dt = this.FuHuoSeconds;
						}
						int zuanShiCost = this.ZuanShiCost * dt / this.FuHuoSeconds + 1;
						if (useZhuanShi <= 0)
						{
							return -12;
						}
						if (client.ClientData.UserMoney < zuanShiCost)
						{
							return -13;
						}
					}
					if (useZhuanShi > 0)
					{
						if (lingDi.ShouWeiList[index].State == 0)
						{
							return -16;
						}
						return 1;
					}
					else
					{
						if (this.FanRongCost <= 0)
						{
							return 1;
						}
						int junTuanPoint = JunTuanClient.getInstance().GetJunTuanPoint(client.ClientData.Faction, client.ClientData.JunTuanId);
						if (junTuanPoint < 0)
						{
							if (junTuanPoint == -11000)
							{
								return -1;
							}
							return -2;
						}
						else if (junTuanPoint < this.FanRongCost)
						{
							return -14;
						}
					}
				}
				result = 1;
			}
			return result;
		}

		
		public bool isLingZhu(int rid)
		{
			bool result;
			lock (this.CaiJiRunTimeData.Mutex)
			{
				if (this.CaiJiRunTimeData.LingDiDataList == null)
				{
					result = false;
				}
				else
				{
					foreach (LingDiData item in this.CaiJiRunTimeData.LingDiDataList)
					{
						if (item.RoleId == rid)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		
		public void OnLogin(GameClient client)
		{
			this.UpdateChengHaoBuff(client);
		}

		
		public void UpdateChengHaoBuff(GameClient client)
		{
			lock (this.CaiJiRunTimeData.Mutex)
			{
				if (!this.CaiJiRunTimeData.KuaFuSyncNeed)
				{
					int junTuanID = client.ClientData.JunTuanId;
					if (junTuanID == this.CaiJiRunTimeData.LingDiDataList[0].JunTuanId && junTuanID > 0)
					{
						if (client.ClientData.RoleID == this.CaiJiRunTimeData.LingDiDataList[0].RoleId)
						{
							Global.UpdateBufferData(client, BufferItemTypes.DiGongLingZhu, new double[]
							{
								1.0
							}, 1, false);
						}
						else
						{
							Global.UpdateBufferData(client, BufferItemTypes.DiGongChenMin, new double[]
							{
								1.0
							}, 1, false);
						}
					}
					else
					{
						BufferItemTypes bufferItemType = BufferItemTypes.DiGongLingZhu;
						double[] actionParams = new double[1];
						Global.UpdateBufferData(client, bufferItemType, actionParams, 1, false);
						BufferItemTypes bufferItemType2 = BufferItemTypes.DiGongChenMin;
						actionParams = new double[1];
						Global.UpdateBufferData(client, bufferItemType2, actionParams, 1, false);
					}
					if (junTuanID == this.CaiJiRunTimeData.LingDiDataList[1].JunTuanId && junTuanID > 0)
					{
						if (client.ClientData.RoleID == this.CaiJiRunTimeData.LingDiDataList[1].RoleId)
						{
							Global.UpdateBufferData(client, BufferItemTypes.HuangMoLingZhu, new double[]
							{
								1.0
							}, 1, false);
						}
						else
						{
							Global.UpdateBufferData(client, BufferItemTypes.HuangMoChenMin, new double[]
							{
								1.0
							}, 1, false);
						}
					}
					else
					{
						BufferItemTypes bufferItemType3 = BufferItemTypes.HuangMoLingZhu;
						double[] actionParams = new double[1];
						Global.UpdateBufferData(client, bufferItemType3, actionParams, 1, false);
						BufferItemTypes bufferItemType4 = BufferItemTypes.HuangMoChenMin;
						actionParams = new double[1];
						Global.UpdateBufferData(client, bufferItemType4, actionParams, 1, false);
					}
				}
			}
		}

		
		private int ConvertCaiJiLingDiTypeToMapCode(int lingDiType)
		{
			int result;
			if (lingDiType == 0)
			{
				result = 83003;
			}
			else if (lingDiType == 1)
			{
				result = 83002;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		public int GetLingDiRoleNum(int lingDiType)
		{
			return JunTuanClient.getInstance().GetLingDiRoleNum(lingDiType);
		}

		
		public void SetSync()
		{
			lock (this.CaiJiRunTimeData.Mutex)
			{
				this.CaiJiRunTimeData.KuaFuSyncNeed = true;
			}
		}

		
		public bool KuaFuInitGame(GameClient client)
		{
			KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			client.ClientData.MapCode = LingDiCaiJiManager.MapCode[Convert.ToInt32(kuaFuServerLoginData.GameId) - 1];
			int _posx = 0;
			int _posy = 0;
			bool result;
			if (!this.GetBirthPoint(client.ClientData.MapCode, out _posx, out _posy))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("找不到出生点mapcode={0},side={1}", client.ClientData.MapCode, client.ClientData.BattleWhichSide), null, true);
				result = false;
			}
			else
			{
				client.ClientData.PosX = _posx;
				client.ClientData.PosY = _posy;
				result = true;
			}
			return result;
		}

		
		public void InitDefaultXml()
		{
			LingDiCaiJiManager.MapCode = new int[2];
			LingDiCaiJiManager.MapCode[0] = 83000;
			LingDiCaiJiManager.MapCode[1] = 83001;
			try
			{
				LingDiCaiJiManager.WeeklyNum = (int)GameManager.systemParamsList.GetParamValueIntByName("ManorCollectNum", -1);
				string ManorCollectDoubleAward = GameManager.systemParamsList.GetParamValueByName("ManorCollectDoubleAward");
				string[] strArray = ManorCollectDoubleAward.Split(new char[]
				{
					','
				});
				if (strArray.Length < 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("LingDICaiJiManager 获取跨服领地特权配置出错:: ManorCollectDoubleAward", new object[0]), null, true);
				}
				else
				{
					LingDiCaiJiManager.OpenTime = DateTime.Parse(strArray[0]).TimeOfDay;
					LingDiCaiJiManager.CloseTime = DateTime.Parse(strArray[1]).TimeOfDay;
					LingDiCaiJiManager.OpenSeconds = Convert.ToInt32(strArray[2]);
					LingDiCaiJiManager.BeiLv = Convert.ToInt32(strArray[3]);
					LingDiCaiJiManager.OpenCountWeekly = Convert.ToInt32(strArray[4]);
					string[] cost = GameManager.systemParamsList.GetParamValueByName("GuardCost").Split(new char[]
					{
						','
					});
					this.FanRongCost = Convert.ToInt32(cost[0]);
					this.ZuanShiCost = Convert.ToInt32(cost[1]);
					this.FuHuoSeconds = Convert.ToInt32(cost[2]);
					string[] level = GameManager.systemParamsList.GetParamValueByName("ManorMinLevel").Split(new char[]
					{
						','
					});
					this.ChangeLifeLimit = Convert.ToInt32(level[0]);
					this.LevelLimit = Convert.ToInt32(level[1]);
					string[] args = GameManager.systemParamsList.GetParamValueByName("ManorBuffID").Split(new char[]
					{
						','
					});
					this.BuffParam[0] = Convert.ToDouble(args[0]);
					this.BuffParam[1] = Convert.ToDouble(args[1]);
					string[] fields = GameManager.systemParamsList.GetParamValueByName("ManorCollectDoubleAwardDefault").Split(new char[]
					{
						',',
						'|'
					});
					this.DoubleOpenTimeDefaultList.Clear();
					this.DoubleOpenTimeDefaultList.Add(new DoubleOpenItem
					{
						WeekDay = Convert.ToInt32(fields[2]),
						DayStartTicks = DateTime.Parse(fields[0]).TimeOfDay.Ticks,
						DayEndTicks = DateTime.Parse(fields[1]).TimeOfDay.Ticks
					});
					this.DoubleOpenTimeDefaultList.Add(new DoubleOpenItem
					{
						WeekDay = Convert.ToInt32(fields[5]),
						DayStartTicks = DateTime.Parse(fields[3]).TimeOfDay.Ticks,
						DayEndTicks = DateTime.Parse(fields[4]).TimeOfDay.Ticks
					});
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDICaiJiClient 获取领地特权配置出错:: ex:{0}", ex.Message), null, true);
			}
		}

		
		public void InitMap()
		{
			try
			{
				foreach (int mapCode in LingDiCaiJiManager.MapCode)
				{
					string fileName = string.Format("Config/ManorCrystal/{0}/CrystalMonster_{0}.xml", mapCode);
					XElement xml = CheckHelper.LoadXml(Global.GameResPath(fileName), true);
					if (null == xml)
					{
						break;
					}
					IEnumerable<XElement> nodes = Global.GetSafeXElement(xml, "Monsters").Elements();
					List<object> normalShuiJingList = new List<object>();
					List<object> chaoShuiJIngList = new List<object>();
					foreach (XElement node in nodes)
					{
						LingDiCrystalMonsterItem item = new LingDiCrystalMonsterItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(node, "ID", "0")),
							Code = Convert.ToInt32(Global.GetDefAttributeStr(node, "Code", "0")),
							PosX = Convert.ToInt32(Global.GetDefAttributeStr(node, "X", "0")),
							PosY = Convert.ToInt32(Global.GetDefAttributeStr(node, "Y", "0"))
						};
						if (this.CollectMonsterXml.ContainsKey(item.Code))
						{
							if (this.CollectMonsterXml[item.Code].Type == CryStealType.Chao)
							{
								chaoShuiJIngList.Add(item);
							}
							else
							{
								GameManager.MonsterZoneMgr.AddDynamicMonsters(mapCode, item.Code, -1, 1, item.PosX / 100, item.PosY / 100, 0, 0, SceneUIClasses.LingDiCaiJi, item, null);
							}
						}
					}
					lock (this.CaiJiRunTimeData.Mutex)
					{
						SortedList<long, List<object>> chaoList = null;
						if (!this.CaiJiRunTimeData.ChaoShuiJingQueue.TryGetValue(mapCode, out chaoList) || null == chaoList)
						{
							chaoList = new SortedList<long, List<object>>();
							this.CaiJiRunTimeData.ChaoShuiJingQueue[mapCode] = chaoList;
						}
						this.CaiJiRunTimeData.ChaoShuiJingQueue[mapCode].Add(TimeUtil.NOW(), chaoShuiJIngList);
						SortedList<long, List<object>> normalList = null;
						if (!this.CaiJiRunTimeData.NormalShuiJingQueue.TryGetValue(mapCode, out normalList) || null == normalList)
						{
							normalList = new SortedList<long, List<object>>();
							this.CaiJiRunTimeData.NormalShuiJingQueue[mapCode] = normalList;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJiManager_Copy :: 初始化刷怪异常：{0}", ex.Message.ToString()), null, true);
			}
		}

		
		public void InitShouWei()
		{
			try
			{
				Dictionary<int, List<LingDiShouWeiMonsterItem>> monsterDic = new Dictionary<int, List<LingDiShouWeiMonsterItem>>();
				Dictionary<int, Dictionary<int, Monster>> shouWeiDic = new Dictionary<int, Dictionary<int, Monster>>();
				foreach (int mapCode in LingDiCaiJiManager.MapCode)
				{
					string fileName = string.Format("Config/ManorCrystal/{0}/ShouWeiMonster.xml", mapCode);
					XElement xml = CheckHelper.LoadXml(Global.GameResPath(fileName), true);
					if (null == xml)
					{
						return;
					}
					IEnumerable<XElement> nodes = Global.GetSafeXElement(xml, "Monsters").Elements();
					List<LingDiShouWeiMonsterItem> shouWeiList = new List<LingDiShouWeiMonsterItem>();
					foreach (XElement node in nodes)
					{
						LingDiShouWeiMonsterItem item = new LingDiShouWeiMonsterItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(node, "ID", "0")),
							Code = Convert.ToInt32(Global.GetDefAttributeStr(node, "Code", "0")),
							PosX = Convert.ToInt32(Global.GetDefAttributeStr(node, "X", "0")),
							PosY = Convert.ToInt32(Global.GetDefAttributeStr(node, "Y", "0")),
							ShouWeiData = new LingDiShouWei
							{
								State = 0,
								FreeBuShuTime = DateTime.MinValue
							}
						};
						shouWeiList.Add(item);
					}
					monsterDic[mapCode] = shouWeiList;
					Dictionary<int, Monster> itemDic = new Dictionary<int, Monster>();
					shouWeiDic[mapCode] = itemDic;
				}
				lock (this.CaiJiRunTimeData.Mutex)
				{
					this.CaiJiRunTimeData.ShouWeiQueue = monsterDic;
					this.CaiJiRunTimeData.ShouWeiMonster = shouWeiDic;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJiManager_Copy :: 初始化刷怪异常：{0}", ex.Message.ToString()), null, true);
			}
		}

		
		private bool GetBirthPoint(int mapCode, out int toPosX, out int toPosY)
		{
			toPosX = -1;
			toPosY = -1;
			GameMap gameMap = null;
			bool result;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDICaiJiManager 获取跨服地图配置出错:: GetBirthPoint", new object[0]), null, true);
				result = false;
			}
			else
			{
				Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, mapCode, gameMap.DefaultBirthPosX, gameMap.DefaultBirthPosY, gameMap.BirthRadius);
				toPosX = (int)newPos.X;
				toPosY = (int)newPos.Y;
				result = true;
			}
			return result;
		}

		
		public void NotifyPlayGame(GameClient client)
		{
			int lingDiType = this.GetLingDiType(client.ClientData.MapCode);
			if (lingDiType != 2)
			{
				lock (this.CaiJiRunTimeData.Mutex)
				{
					if (this.CaiJiRunTimeData.KuaFuSyncNeed)
					{
						return;
					}
					if (this.CaiJiRunTimeData.LingDiDataList[lingDiType].JunTuanId == client.ClientData.JunTuanId && client.ClientData.JunTuanId != 0)
					{
						Global.UpdateBufferData(client, BufferItemTypes.JunTuanCaiJiBuff, this.BuffParam, 1, false);
					}
					string lingZhu = (this.CaiJiRunTimeData.LingDiDataList[lingDiType].RoleId == client.ClientData.RoleID) ? "1" : "0";
					client.sendCmd(1837, lingZhu, false);
				}
				int leftCount = LingDiCaiJiManager.WeeklyNum - client.ClientData.LingDiCaiJiNum;
				if (leftCount < 0)
				{
					leftCount = 0;
				}
				client.sendCmd(1828, leftCount.ToString(), false);
				DateTime leftTime = DateTime.MinValue;
				lock (this.CaiJiRunTimeData.Mutex)
				{
					leftTime = this.CaiJiRunTimeData.LingDiDataList[lingDiType].EndTime;
				}
				client.sendCmd<DateTime>(1830, leftTime, false);
			}
		}

		
		public void OnLeaveFuBen(GameClient client)
		{
			Global.RemoveBufferData(client, 115);
		}

		
		public LingDiCaiJiRunData CaiJiRunTimeData = new LingDiCaiJiRunData();

		
		public Dictionary<int, ManorCollectMonster> CollectMonsterXml = new Dictionary<int, ManorCollectMonster>();

		
		private static LingDiCaiJiManager instance = new LingDiCaiJiManager();

		
		private object DataMutex = new object();

		
		private long NextHeartBeatTicks = 0L;

		
		private long NextSyncTicks = 0L;

		
		private long NextCheckNumTicks = 0L;

		
		private long NextSyncRoleNumTicks = 0L;

		
		public static int[] MapCode;

		
		public static int WeeklyNum = 0;

		
		public static int OpenSeconds = 0;

		
		public static int BeiLv = 1;

		
		public static int ZhanLingBeiLv = 1;

		
		public static int OpenCountWeekly = 0;

		
		public static TimeSpan OpenTime = DateTime.MaxValue.TimeOfDay;

		
		public static TimeSpan CloseTime = DateTime.MinValue.TimeOfDay;

		
		public List<DoubleOpenItem> DoubleOpenTimeDefaultList = new List<DoubleOpenItem>();

		
		public int FanRongCost = int.MaxValue;

		
		public int ZuanShiCost = int.MaxValue;

		
		public int FuHuoSeconds = int.MaxValue;

		
		public int ChangeLifeLimit = int.MaxValue;

		
		public int LevelLimit = int.MaxValue;

		
		public double[] BuffParam = new double[2];
	}
}
