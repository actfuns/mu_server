using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.Building;
using GameServer.Logic.Damon;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class YaoSaiMissionManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		
		public static YaoSaiMissionManager getInstance()
		{
			return YaoSaiMissionManager.instance;
		}

		
		public bool initialize()
		{
			this.LoadConfig();
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1859, 2, 2, YaoSaiMissionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1860, 1, 1, YaoSaiMissionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1861, 3, 3, YaoSaiMissionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1862, 2, 2, YaoSaiMissionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1863, 2, 2, YaoSaiMissionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1859:
					result = this.ProcessGetMissionInfoCmd(client, nID, bytes, cmdParams);
					break;
				case 1860:
					result = this.ProcessRefreshMissionCmd(client, nID, bytes, cmdParams);
					break;
				case 1861:
					result = this.ProcessExcuteMissionCmd(client, nID, bytes, cmdParams);
					break;
				case 1862:
					result = this.ProcessQuitMissionCmd(client, nID, bytes, cmdParams);
					break;
				case 1863:
					result = this.ProcessGetMissionAwardCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		
		public bool ProcessGetMissionInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int otherRoleId = Global.SafeConvertToInt32(cmdParams[1]);
				List<YaoSaiMissionData> missionDataList = this.GetRoleMissionDataList(otherRoleId);
				YaoSaiMissionMainData data = new YaoSaiMissionMainData
				{
					MissionDataList = missionDataList,
					ExcuteMissionCount = Global.GetRoleParamsInt32FromDB(client, "10180"),
					FreeRefreshTime = Global.GetRoleParamsDateTimeFromDB(client, "10181").AddSeconds((double)YaoSaiMissionManager.MissionRefreshSeconds)
				};
				client.sendCmd<YaoSaiMissionMainData>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 获取主页面任务信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessRefreshMissionCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				DateTime now = TimeUtil.NowDateTime();
				int dayCount = Global.GetRoleParamsInt32FromDB(client, "10180");
				int result;
				if (dayCount >= YaoSaiMissionManager.MissionCountLimit)
				{
					result = 2;
				}
				else
				{
					List<YaoSaiMissionData> missionWaitList = this.GetRoleMissionWaitList(roleID);
					if (missionWaitList == null || 0 == missionWaitList.Count)
					{
						result = 3;
					}
					else
					{
						bool needZuanShi = Global.GetRoleParamsDateTimeFromDB(client, "10181").AddSeconds((double)YaoSaiMissionManager.MissionRefreshSeconds) >= now;
						if (needZuanShi && client.ClientData.UserMoney < YaoSaiMissionManager.RefreshMissionCost)
						{
							result = 4;
						}
						else
						{
							for (int i = 0; i < missionWaitList.Count; i++)
							{
								if (!this.RandomYaoSaiMission(client, missionWaitList[i]))
								{
									break;
								}
							}
							if (needZuanShi && YaoSaiMissionManager.RefreshMissionCost > 0)
							{
								if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, YaoSaiMissionManager.RefreshMissionCost, "要塞刷新任务", true, true, false, DaiBiSySType.JingLingYaoSaiShuaXin))
								{
									result = 4;
									goto IL_18A;
								}
							}
							if (!needZuanShi)
							{
								Global.SaveRoleParamsDateTimeToDB(client, "10181", now, true);
							}
							result = this.UpdateYaoSaiMissionDataDB(client, missionWaitList);
						}
					}
				}
				IL_18A:
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 处理刷新任务信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessExcuteMissionCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int siteID = Global.SafeConvertToInt32(cmdParams[1]);
				if (siteID < 0 || siteID > 5)
				{
					return false;
				}
				int dayCount = Global.GetRoleParamsInt32FromDB(client, "10180");
				int result;
				if (dayCount >= YaoSaiMissionManager.MissionCountLimit)
				{
					result = 2;
				}
				else
				{
					List<YaoSaiMissionData> missionList = this.GetRoleMissionDataList(roleID);
					if (missionList == null || 0 == missionList.Count)
					{
						result = 3;
					}
					else
					{
						YaoSaiMissionData mission = missionList.Find((YaoSaiMissionData x) => x.SiteID == siteID);
						if (mission == null || mission.State != 0)
						{
							result = 6;
						}
						else if (!this.CanZhiPai(client, cmdParams[2]))
						{
							result = 7;
						}
						else
						{
							List<YaoSaiMissionData> excuteList = new List<YaoSaiMissionData>();
							mission.StartTime = TimeUtil.NowDateTime();
							mission.ZhiPaiJingLing = cmdParams[2];
							mission.State = 3;
							excuteList.Add(mission);
							string[] jingLing = cmdParams[2].Split(new char[]
							{
								'|'
							});
							foreach (string one in jingLing)
							{
								GoodsData goods = JingLingYaoSaiManager.GetPaiZhuDamonGoodsDataByDbID(client, Global.SafeConvertToInt32(one));
								string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
								{
									client.ClientData.RoleID,
									3,
									goods.Id,
									goods.GoodsID,
									goods.Using,
									10001,
									goods.GCount,
									goods.BagIndex,
									""
								});
								Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
							}
							dayCount++;
							Global.SaveRoleParamsInt32ValueToDB(client, "10180", dayCount, true);
							this.UpdateYaoSaiMissionSortList(client.ClientData.RoleID, mission);
							result = this.UpdateYaoSaiMissionDataDB(client, excuteList);
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "missionid=" + mission.MissionID, "要塞任务_Site=" + mission.SiteID, client.ClientData.RoleName, "系统", "精灵", jingLing.Length, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
						}
					}
				}
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 执行执行任务信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessQuitMissionCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int siteID = Global.SafeConvertToInt32(cmdParams[1]);
				if (siteID < 0 || siteID > 5)
				{
					return false;
				}
				List<YaoSaiMissionData> missionList = this.GetRoleMissionDataList(roleID);
				int result;
				if (missionList == null || 0 == missionList.Count)
				{
					result = 6;
				}
				else
				{
					YaoSaiMissionData mission = missionList.Find((YaoSaiMissionData x) => x.SiteID == siteID);
					if (mission == null || mission.State != 3)
					{
						result = 9;
					}
					else
					{
						string[] jingLing = mission.ZhiPaiJingLing.Split(new char[]
						{
							'|'
						});
						foreach (string one in jingLing)
						{
							GoodsData goods = JingLingYaoSaiManager.GetPaiZhuDamonGoodsDataByDbID(client, Global.SafeConvertToInt32(one));
							if (null != goods)
							{
								string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
								{
									client.ClientData.RoleID,
									3,
									goods.Id,
									goods.GoodsID,
									goods.Using,
									10000,
									goods.GCount,
									goods.BagIndex,
									""
								});
								Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
							}
						}
						List<YaoSaiMissionData> quitList = new List<YaoSaiMissionData>();
						mission.StartTime = DateTime.MinValue;
						mission.ZhiPaiJingLing = "";
						mission.State = 0;
						quitList.Add(mission);
						this.UpdateYaoSaiMissionSortList(client.ClientData.RoleID, mission);
						result = this.UpdateYaoSaiMissionDataDB(client, quitList);
					}
				}
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 执行放弃任务信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessGetMissionAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int siteID = Global.SafeConvertToInt32(cmdParams[1]);
				if (siteID < 0 || siteID > 5)
				{
					return false;
				}
				List<YaoSaiMissionData> missionList = this.GetRoleMissionDataList(roleID);
				int result;
				if (missionList == null || 0 == missionList.Count)
				{
					result = 6;
				}
				else
				{
					YaoSaiMissionData mission = missionList.Find((YaoSaiMissionData x) => x.SiteID == siteID);
					if (mission == null || (mission.State != 1 && mission.State != 2))
					{
						result = 8;
					}
					else
					{
						PetMissionItem missionXml = null;
						if (!this.PetMissionIDXmlDIct.TryGetValue(mission.MissionID, out missionXml))
						{
							result = 5;
						}
						else
						{
							int rate = (mission.State == 1) ? 100 : YaoSaiMissionManager.FailAwardRate;
							List<YaoSaiMissionData> quitList = new List<YaoSaiMissionData>();
							mission.StartTime = DateTime.MinValue;
							mission.ZhiPaiJingLing = "";
							mission.State = 0;
							quitList.Add(mission);
							result = this.DeleteYaoSaiMissionDataDB(client, quitList);
							if (result == 0)
							{
								int lingJing = missionXml.CrystalNum * rate / 100;
								if (lingJing > 0)
								{
									GameManager.ClientMgr.ModifyMUMoHeValue(client, lingJing, "领取要塞任务奖励", true, true, false);
								}
								int shenJiPoint = missionXml.SignNum * rate / 100;
								if (shenJiPoint > 0)
								{
									GameManager.ClientMgr.ModifyShenJiJiFenValue(client, shenJiPoint, "领取要塞任务奖励", true, true);
								}
								string[] nengLang = missionXml.Activator.Split(new char[]
								{
									'|'
								});
								foreach (string one in nengLang)
								{
									string[] item = one.Split(new char[]
									{
										','
									});
									if (item.Length >= 2)
									{
										int type = Global.SafeConvertToInt32(item[0]);
										int addVal = Global.SafeConvertToInt32(item[1]) * rate / 100;
										if (addVal > 0)
										{
											BuildingManager.getInstance().ModifyNengLiangPointsValue(client, type, addVal, "领取要塞任务奖励", true, true);
										}
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
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 执行放弃任务信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public List<YaoSaiMissionData> GetRoleMissionDataList(int rid)
		{
			List<YaoSaiMissionData> missionDataList = null;
			List<YaoSaiMissionData> result;
			try
			{
				lock (this.RunTimeData.Mutex)
				{
					if (!this.RunTimeData.RoleMissionCacheDict.TryGetValue(rid, out missionDataList))
					{
						missionDataList = Global.sendToDB<List<YaoSaiMissionData>, string>(20311, rid.ToString(), GameCoreInterface.getinstance().GetLocalServerId());
						if (null != missionDataList)
						{
							this.RunTimeData.RoleMissionCacheDict[rid] = missionDataList;
							foreach (YaoSaiMissionData item in missionDataList)
							{
								if (item.State == 3)
								{
									PetMissionItem mission = null;
									if (this.PetMissionIDXmlDIct.TryGetValue(item.MissionID, out mission))
									{
										long tick = item.StartTime.Ticks / 10000L + (long)(mission.Time * 1000);
										while (this.RunTimeData.MissionSortList.ContainsKey(tick))
										{
											tick += 1L;
										}
										this.RunTimeData.MissionSortList.Add(tick, new RoleMissionData
										{
											RoleID = rid,
											MissionData = item
										});
									}
								}
							}
						}
					}
				}
				result = missionDataList;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 获取角色要塞任务列表信息错误 roleid={0} ex:{1}", rid, ex.Message), null, true);
				result = null;
			}
			return result;
		}

		
		public List<YaoSaiMissionData> GetRoleMissionWaitList(int rid)
		{
			List<YaoSaiMissionData> missionWaitList = new List<YaoSaiMissionData>();
			List<YaoSaiMissionData> missionList = this.GetRoleMissionDataList(rid);
			List<YaoSaiMissionData> result;
			try
			{
				if (null == missionList)
				{
					missionList = new List<YaoSaiMissionData>();
				}
				int i;
				for (i = 0; i < 5; i++)
				{
					YaoSaiMissionData mission = missionList.Find((YaoSaiMissionData x) => x.SiteID == i + 1);
					if (null == mission)
					{
						mission = new YaoSaiMissionData
						{
							SiteID = i + 1,
							ZhiPaiJingLing = ""
						};
					}
					if (mission.State == 0)
					{
						missionWaitList.Add(mission);
					}
				}
				result = missionWaitList;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 获取角色要塞未开始任务列表信息错误 roleid={0} ex:{1}", rid, ex.Message), null, true);
				result = null;
			}
			return result;
		}

		
		public void OnLogin(GameClient client, bool isNewDay)
		{
			if (isNewDay)
			{
				Global.SaveRoleParamsInt32ValueToDB(client, "10180", 0, true);
			}
		}

		
		public bool RandomYaoSaiMission(GameClient client, YaoSaiMissionData missionData)
		{
			bool result;
			try
			{
				int shenJiPoint = ShenJiFuWenManager.getInstance().GetAllShenJiPointNum(client);
				int levelID = 0;
				foreach (KeyValuePair<int, ShenJiLevelItem> item in this.ShenJiLevelDict)
				{
					if (shenJiPoint >= item.Value.StartValue && (shenJiPoint <= item.Value.EndValue || item.Value.EndValue < 0))
					{
						levelID = item.Key;
						break;
					}
				}
				int random = Global.GetRandomNumber(1, 100001);
				List<PetMissionItem> missionList = null;
				if (!this.PetMissionXmlDict.TryGetValue(levelID, out missionList))
				{
					result = false;
				}
				else
				{
					foreach (PetMissionItem item2 in missionList)
					{
						if (item2.Type == missionData.SiteID && random >= item2.RateStartVal && random <= item2.RateEndVal)
						{
							missionData.MissionID = item2.ID;
							return true;
						}
					}
					result = false;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 刷新角色要塞任务列表信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
				result = false;
			}
			return result;
		}

		
		public int UpdateYaoSaiMissionDataDB(GameClient client, List<YaoSaiMissionData> missionList)
		{
			int result;
			if (null == missionList)
			{
				result = 5;
			}
			else
			{
				int rid = client.ClientData.RoleID;
				lock (this.RunTimeData.Mutex)
				{
					List<YaoSaiMissionData> roleMissionList = null;
					if (!this.RunTimeData.RoleMissionCacheDict.TryGetValue(rid, out roleMissionList))
					{
						this.RunTimeData.RoleMissionCacheDict[rid] = missionList;
					}
					else
					{
						using (List<YaoSaiMissionData>.Enumerator enumerator = missionList.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								YaoSaiMissionData item = enumerator.Current;
								YaoSaiMissionData mission = roleMissionList.Find((YaoSaiMissionData x) => x.SiteID == item.SiteID);
								if (null == mission)
								{
									roleMissionList.Add(item);
								}
								else
								{
									mission = item;
								}
							}
						}
					}
				}
				Dictionary<int, List<YaoSaiMissionData>> data = new Dictionary<int, List<YaoSaiMissionData>>();
				data[rid] = missionList;
				Global.sendToDB<int, Dictionary<int, List<YaoSaiMissionData>>>(20312, data, 0);
				result = 0;
			}
			return result;
		}

		
		public int DeleteYaoSaiMissionDataDB(GameClient client, List<YaoSaiMissionData> missionList)
		{
			int result;
			if (null == missionList)
			{
				result = 5;
			}
			else
			{
				int rid = client.ClientData.RoleID;
				lock (this.RunTimeData.Mutex)
				{
					List<YaoSaiMissionData> roleMissionList = null;
					if (!this.RunTimeData.RoleMissionCacheDict.TryGetValue(rid, out roleMissionList))
					{
						return 6;
					}
					foreach (YaoSaiMissionData delMission in missionList)
					{
						for (int index = 0; index < roleMissionList.Count; index++)
						{
							if (roleMissionList[index].SiteID == delMission.SiteID)
							{
								roleMissionList.RemoveAt(index);
								break;
							}
						}
					}
				}
				Dictionary<int, List<YaoSaiMissionData>> data = new Dictionary<int, List<YaoSaiMissionData>>();
				data[rid] = missionList;
				Global.sendToDB<int, Dictionary<int, List<YaoSaiMissionData>>>(20313, data, 0);
				result = 0;
			}
			return result;
		}

		
		public int UpdateYaoSaiMissionSortList(int rid, YaoSaiMissionData mission)
		{
			int result;
			if (null == mission)
			{
				result = 5;
			}
			else
			{
				PetMissionItem missionXml = null;
				if (!this.PetMissionIDXmlDIct.TryGetValue(mission.MissionID, out missionXml))
				{
					result = 5;
				}
				else
				{
					lock (this.RunTimeData.Mutex)
					{
						if (mission.State != 3)
						{
							int index = 0;
							foreach (KeyValuePair<long, RoleMissionData> pair in this.RunTimeData.MissionSortList)
							{
								if (pair.Value.RoleID == rid && pair.Value.MissionData.SiteID == mission.SiteID)
								{
									break;
								}
								index++;
							}
							if (index < this.RunTimeData.MissionSortList.Count)
							{
								this.RunTimeData.MissionSortList.RemoveAt(index);
							}
						}
						else
						{
							long tick = mission.StartTime.Ticks / 10000L + (long)(missionXml.Time * 1000);
							while (this.RunTimeData.MissionSortList.ContainsKey(tick))
							{
								tick += 1L;
							}
							this.RunTimeData.MissionSortList.Add(tick, new RoleMissionData
							{
								RoleID = rid,
								MissionData = mission
							});
						}
					}
					result = 0;
				}
			}
			return result;
		}

		
		public bool CanZhiPai(GameClient client, string jingLing)
		{
			bool result;
			if (string.IsNullOrEmpty(jingLing))
			{
				result = false;
			}
			else
			{
				string[] jingLingArray = jingLing.Split(new char[]
				{
					'|'
				});
				List<string> tmp = new List<string>();
				if (jingLingArray.Length < 1 || jingLingArray.Length > 3)
				{
					result = false;
				}
				else
				{
					string[] array = jingLingArray;
					for (int i = 0; i < array.Length; i++)
					{
						string item = array[i];
						if (tmp.Contains(item))
						{
							return false;
						}
						tmp.Add(item);
						if (null == client.ClientData.PaiZhuDamonGoodsDataList.Find((GoodsData x) => x.Site == 10000 && x.Id == Convert.ToInt32(item)))
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		
		public int GetMissionRate(GameClient client, YaoSaiMissionData missionData)
		{
			int ret = 0;
			int result;
			if (null == missionData)
			{
				result = 0;
			}
			else
			{
				try
				{
					int missionID = missionData.MissionID;
					PetMissionItem missionXml = null;
					if (!this.PetMissionIDXmlDIct.TryGetValue(missionID, out missionXml))
					{
						result = 0;
					}
					else
					{
						ret += missionXml.SuccessRate;
						bool haveSpecial = false;
						string[] jingLingArr = missionData.ZhiPaiJingLing.Split(new char[]
						{
							'|'
						});
						if (jingLingArr.Length < 1 || jingLingArr.Length > 3)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 计算任务成功率错误 找不到角色精灵，rid={0},siteid={1}", client.ClientData.RoleID, missionData.SiteID), null, true);
							result = 0;
						}
						else
						{
							string[] array = jingLingArr;
							for (int i = 0; i < array.Length; i++)
							{
								string jingling = array[i];
								GoodsData item = client.ClientData.PaiZhuDamonGoodsDataList.Find((GoodsData x) => x.Id == Convert.ToInt32(jingling));
								if (null == item)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 计算任务成功率错误 找不到任务数据，rid={0},siteid={1}", client.ClientData.RoleID, missionData.SiteID), null, true);
									return 0;
								}
								int levelBeiLv = 1 + (1 + item.Forge_level) / missionXml.PetLevelStep;
								int levelAddVal = levelBeiLv * missionXml.PetLevelStepRate;
								ret += levelAddVal;
								int exceBeiLv = 1 + Global.GetEquipExcellencePropNum(item) / missionXml.ExcellentStep;
								int exceAddVal = exceBeiLv * missionXml.ExcellentStepRate;
								ret += exceAddVal;
								if (item.GoodsID == missionXml.SpecialPet)
								{
									haveSpecial = true;
								}
							}
							if (haveSpecial)
							{
								ret += missionXml.SpecialPetRate;
							}
							result = ret + 10;
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 计算任务成功率错误 ex:{0}", ex.Message), null, true);
					result = 0;
				}
			}
			return result;
		}

		
		public void RefReshMission(GameClient client)
		{
			int roleID = client.ClientData.RoleID;
			if (Global.GetRoleParamsInt64FromDB(client, "10184") > 0L)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 已经有角色数据了还瞎刷 rid={0}", roleID), null, true);
			}
			else
			{
				List<YaoSaiMissionData> missionWaitList = this.GetRoleMissionWaitList(roleID);
				int ret;
				if (missionWaitList == null || 0 == missionWaitList.Count)
				{
					ret = 3;
				}
				else
				{
					for (int i = 0; i < missionWaitList.Count; i++)
					{
						if (!this.RandomYaoSaiMission(client, missionWaitList[i]))
						{
							break;
						}
					}
					ret = this.UpdateYaoSaiMissionDataDB(client, missionWaitList);
				}
				if (ret != 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 创建要塞时刷任务失败，rid={0}，errCode={1}", roleID, ret), null, true);
				}
				else
				{
					Global.SaveRoleParamsDateTimeToDB(client, "10184", TimeUtil.NowDateTime(), true);
					Global.SaveRoleParamsDateTimeToDB(client, "10181", TimeUtil.NowDateTime(), true);
				}
			}
		}

		
		public void YaoSaiMissionTimer_Work()
		{
			long nowMs = TimeUtil.NOW();
			try
			{
				lock (this.RunTimeData.Mutex)
				{
					IList<long> keyList = this.RunTimeData.MissionSortList.Keys;
					int index = 0;
					while (index < keyList.Count)
					{
						Dictionary<int, YaoSaiMissionData> finishDict = new Dictionary<int, YaoSaiMissionData>();
						if (nowMs < keyList[index])
						{
							break;
						}
						try
						{
							GameClient client = GameManager.ClientMgr.FindClient(this.RunTimeData.MissionSortList[keyList[index]].RoleID);
							if (null != client)
							{
								int random = Global.GetRandomNumber(0, 101);
								int rateSuccess = this.GetMissionRate(client, this.RunTimeData.MissionSortList[keyList[index]].MissionData);
								if (rateSuccess >= random)
								{
									this.RunTimeData.MissionSortList[keyList[index]].MissionData.State = 1;
								}
								else
								{
									this.RunTimeData.MissionSortList[keyList[index]].MissionData.State = 2;
								}
								YaoSaiMissionData mis = this.RunTimeData.MissionSortList[keyList[index]].MissionData;
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "missionid=" + mis.MissionID, "要塞任务_Site=" + mis.SiteID, client.ClientData.RoleName, "系统", "完成", mis.State, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
								string[] jingLing = this.RunTimeData.MissionSortList[keyList[index]].MissionData.ZhiPaiJingLing.Split(new char[]
								{
									'|'
								});
								foreach (string one in jingLing)
								{
									if (!string.IsNullOrEmpty(one))
									{
										GoodsData goods = JingLingYaoSaiManager.GetPaiZhuDamonGoodsDataByDbID(client, Global.SafeConvertToInt32(one));
										if (null != goods)
										{
											string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
											{
												client.ClientData.RoleID,
												3,
												goods.Id,
												goods.GoodsID,
												goods.Using,
												10000,
												goods.GCount,
												goods.BagIndex,
												""
											});
											Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
										}
									}
								}
								this.UpdateYaoSaiMissionDataDB(client, new List<YaoSaiMissionData>
								{
									this.RunTimeData.MissionSortList[keyList[index]].MissionData
								});
								this.RunTimeData.MissionSortList.RemoveAt(index);
								index--;
							}
						}
						catch (Exception ex)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 定时器错误 ex:{0}", ex.Message), null, true);
						}
						IL_361:
						index++;
						continue;
						goto IL_361;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 定时器刷新要塞任务信息错误 ex:{}", ex.Message), null, true);
			}
		}

		
		public void LoadConfig()
		{
			this.LoadPetMissionXml();
			this.LoadSystemParams();
		}

		
		public void LoadSystemParams()
		{
			try
			{
				string[] PetMissionMax = GameManager.systemParamsList.GetParamValueByName("PetMissionMax").Split(new char[]
				{
					','
				});
				if (PetMissionMax.Length < 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 配置表配置出错 PetMissionMax", new object[0]), null, true);
				}
				else
				{
					YaoSaiMissionManager.MissionCountLimit = Global.SafeConvertToInt32(PetMissionMax[0]);
					YaoSaiMissionManager.MissionRefreshSeconds = Global.SafeConvertToInt32(PetMissionMax[1]);
					YaoSaiMissionManager.RefreshMissionCost = Global.SafeConvertToInt32(GameManager.systemParamsList.GetParamValueByName("RefreshMissionCost"));
					YaoSaiMissionManager.FailAwardRate = Global.SafeConvertToInt32(GameManager.systemParamsList.GetParamValueByName("FailAwardRate"));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。ex:{1}", "SystemParms.xml", ex.Message), ex, true);
			}
		}

		
		public void LoadRunTimeDataFromDB()
		{
			int loop = 0;
			for (;;)
			{
				try
				{
					Dictionary<int, List<YaoSaiMissionData>> roleMissionDict = Global.sendToDB<Dictionary<int, List<YaoSaiMissionData>>, int>(20311, 0, GameCoreInterface.getinstance().GetLocalServerId());
					lock (this.RunTimeData.Mutex)
					{
						if (null != roleMissionDict)
						{
							this.RunTimeData.RoleMissionCacheDict = roleMissionDict;
							foreach (KeyValuePair<int, List<YaoSaiMissionData>> pair in roleMissionDict)
							{
								if (null != pair.Value)
								{
									foreach (YaoSaiMissionData item in pair.Value)
									{
										if (item.State == 3)
										{
											PetMissionItem mission = null;
											if (this.PetMissionIDXmlDIct.TryGetValue(item.MissionID, out mission))
											{
												long tick = item.StartTime.Ticks / 10000L + (long)(mission.Time * 1000);
												while (this.RunTimeData.MissionSortList.ContainsKey(tick))
												{
													tick += 1L;
												}
												this.RunTimeData.MissionSortList.Add(tick, new RoleMissionData
												{
													RoleID = pair.Key,
													MissionData = item
												});
											}
										}
									}
								}
							}
							break;
						}
						if (++loop < 10)
						{
							Thread.Sleep(1000);
						}
						else
						{
							LogManager.WriteLog(LogTypes.Fatal, string.Format("YaoSaiMission :: 初始化数据库数据失败。", new object[0]), null, true);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 获取数据库数据失败。ex:{0}", ex.Message), null, true);
				}
			}
		}

		
		public void LoadPetMissionXml()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath("Config\\PetMission.xml");
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					IEnumerable<XElement> typeNodes = xml.Elements("SignLevel");
					if (null != typeNodes)
					{
						this.ShenJiLevelDict.Clear();
						this.PetMissionXmlDict.Clear();
						foreach (XElement xmlTypeItem in typeNodes)
						{
							int levelID = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlTypeItem, "LevelID", "0"));
							this.ShenJiLevelDict[levelID] = new ShenJiLevelItem
							{
								StartValue = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlTypeItem, "SignLevelStart", "0")),
								EndValue = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlTypeItem, "SignLevelEnd", "0"))
							};
							IEnumerable<XElement> nodes = xmlTypeItem.Elements("PetMission");
							if (null == nodes)
							{
								break;
							}
							foreach (XElement xmlItem in nodes)
							{
								int id = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
								string[] rateInterval = Global.GetDefAttributeStr(xmlItem, "RateInterval", "").Split(new char[]
								{
									','
								});
								if (rateInterval.Length < 2)
								{
									rateInterval = new string[]
									{
										"0",
										"0"
									};
								}
								if (!this.PetMissionXmlDict.ContainsKey(levelID))
								{
									this.PetMissionXmlDict[levelID] = new List<PetMissionItem>();
								}
								PetMissionItem item = new PetMissionItem
								{
									ID = id,
									Type = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "Type", "0")),
									RateStartVal = Global.SafeConvertToInt32(rateInterval[0]),
									RateEndVal = Global.SafeConvertToInt32(rateInterval[1]),
									SuccessRate = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "SuccessRate", "0")),
									PetLevelStep = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "PetLevelStep", "0")),
									PetLevelStepRate = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "PetLevelStepRate", "0")),
									ExcellentStep = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "ExcellentStep", "0")),
									ExcellentStepRate = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "ExcellentStepRate", "0")),
									SpecialPet = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "SpecialPet", "0")),
									SpecialPetRate = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "SpecialPetRate", "0")),
									Time = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "Time", "0")),
									CrystalNum = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "CrystalNum", "0")),
									SignNum = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "SignNum", "0")),
									Activator = Global.GetDefAttributeStr(xmlItem, "Activator", "")
								};
								this.PetMissionXmlDict[levelID].Add(item);
								this.PetMissionIDXmlDIct[id] = item;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。ex:{1}", fileName, ex.Message), ex, true);
			}
		}

		
		public YaoSaiMissionRunTimeData RunTimeData = new YaoSaiMissionRunTimeData();

		
		public Dictionary<int, ShenJiLevelItem> ShenJiLevelDict = new Dictionary<int, ShenJiLevelItem>();

		
		public Dictionary<int, List<PetMissionItem>> PetMissionXmlDict = new Dictionary<int, List<PetMissionItem>>();

		
		public Dictionary<int, PetMissionItem> PetMissionIDXmlDIct = new Dictionary<int, PetMissionItem>();

		
		public static int MissionCountLimit = 0;

		
		public static int MissionRefreshSeconds = 0;

		
		public static int RefreshMissionCost = 0;

		
		public static int FailAwardRate = 0;

		
		private static YaoSaiMissionManager instance = new YaoSaiMissionManager();
	}
}
