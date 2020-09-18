using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.UserMoneyCharge;
using Server.Data;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	
	public class JieriSuperInputActivity : Activity
	{
		
		public bool Init()
		{
			try
			{
				string activityTm = GameManager.systemParamsList.GetParamValueByName("SuperChongZhiFanLi");
				if (string.IsNullOrEmpty(activityTm))
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型节日充值返利活动配置文件中的SuperChongZhiFanLi失败", new object[0]), null, true);
					return false;
				}
				string[] TmFields = activityTm.Split(new char[]
				{
					'|'
				});
				if (TmFields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型节日充值返利活动配置文件中的SuperChongZhiFanLi失败", new object[0]), null, true);
					return false;
				}
				this.FromDate = TmFields[0];
				this.ToDate = TmFields[1];
				this.ActivityType = 71;
				this.AwardStartDate = this.FromDate;
				this.AwardEndDate = this.ToDate;
				base.PredealDateTime();
				string strCmd = GameManager.GameConfigMgr.GetGameConfigItemStr("platformtype", "app");
				strCmd = strCmd.ToLower();
				string sectionKey = string.Empty;
				if (strCmd == "app")
				{
					sectionKey = "dl_app";
				}
				else if (strCmd == "yueyu")
				{
					sectionKey = "dl_yueyu";
				}
				else if (strCmd == "andrid" || strCmd == "android" || strCmd == "yyb")
				{
					sectionKey = "dl_android";
				}
				else
				{
					sectionKey = "dl_app";
				}
				this.JieriSuperInputDict.Clear();
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/MU_ChongZhiFanLi.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/MU_ChongZhiFanLi.xml"));
				if (xml != null)
				{
					IEnumerable<XElement> xmlItems = xml.Elements().First((XElement _xml) => _xml.Attribute("TypeID").Value.ToString().ToLower() == sectionKey).Elements();
					foreach (XElement item in xmlItems)
					{
						if (null != item)
						{
							JieriSuperInputData superInputData = new JieriSuperInputData();
							superInputData.ID = (int)Global.GetSafeAttributeLong(item, "ID");
							superInputData.MutiNum = (int)Global.GetSafeAttributeLong(item, "Num");
							superInputData.PurchaseNum = (int)Global.GetSafeAttributeLong(item, "SinglePurchase");
							superInputData.FullPurchaseNum = (int)Global.GetSafeAttributeLong(item, "FullPurchase");
							DateTime actDate;
							DateTime.TryParse(Global.GetSafeAttributeStr(item, "Data"), out actDate);
							TimeSpan actBegin;
							TimeSpan.TryParse(Global.GetSafeAttributeStr(item, "BeginTime"), out actBegin);
							TimeSpan actEnd;
							TimeSpan.TryParse(Global.GetSafeAttributeStr(item, "EndTime"), out actEnd);
							superInputData.BeginTime = actDate + actBegin;
							superInputData.EndTime = actDate + actEnd;
							this.JieriSuperInputDict[superInputData.ID] = superInputData;
						}
					}
				}
				Dictionary<int, int> OpenStateDict = new Dictionary<int, int>();
				string strPlatformOpen = GameManager.systemParamsList.GetParamValueByName("SuperChongZhiFanLiOpen");
				if (!string.IsNullOrEmpty(strPlatformOpen))
				{
					string[] Fields = strPlatformOpen.Split(new char[]
					{
						'|'
					});
					foreach (string dat in Fields)
					{
						string[] State = dat.Split(new char[]
						{
							','
						});
						if (State.Length == 2)
						{
							OpenStateDict[Global.SafeConvertToInt32(State[0])] = Global.SafeConvertToInt32(State[1]);
						}
					}
				}
				OpenStateDict.TryGetValue(UserMoneyMgr.getInstance().GetActivityPlatformType(), out this.PlatformOpenStateVavle);
				if (!this.InActivityTime())
				{
					GameManager.ClientMgr.NotifyAllActivityState(11, 0, "", "", 0);
				}
				else
				{
					GameManager.ClientMgr.NotifyAllActivityState(11, this.PlatformOpenStateVavle, "", "", 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/MU_ChongZhiFanLi.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public void SaveFullPurchaseList(List<int> countList)
		{
			lock (JieriSuperInputActivity._SuperInputMutex)
			{
				string strValue = string.Join<int>(",", countList.ToArray());
				GameManager.GameConfigMgr.SetGameConfigItem("czfl_fullpurnum", strValue);
				Global.UpdateDBGameConfigg("czfl_fullpurnum", strValue);
			}
		}

		
		public List<int> GetFullPurchaseList(DateTime now)
		{
			List<int> result;
			lock (JieriSuperInputActivity._SuperInputMutex)
			{
				string strValue = GameManager.GameConfigMgr.GetGameConfigItemStr("czfl_fullpurnum", "");
				string[] strFields = strValue.Split(new char[]
				{
					','
				});
				List<int> countList = new List<int>();
				foreach (string item in strFields)
				{
					countList.Add(Global.SafeConvertToInt32(item));
				}
				if (countList.Count != 5)
				{
					for (int i = countList.Count; i < 5; i++)
					{
						countList.Add(0);
					}
				}
				int dayId = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
				JieriSuperInputData superInputData = this.GetJieriSuperInputDataByNowDateTime(now, false);
				if (null != superInputData)
				{
					if (countList[0] != dayId)
					{
						countList[0] = dayId;
						countList[1] = superInputData.ID;
						countList[2] = superInputData.FullPurchaseNum;
						countList[3] = 0;
						countList[4] = 0;
						this.SaveFullPurchaseList(countList);
					}
					else if (countList[1] != superInputData.ID)
					{
						if (now >= superInputData.BeginTime && now <= superInputData.EndTime)
						{
							countList[1] = superInputData.ID;
							int unhandleNum = countList[3] - countList[4];
							countList[2] = superInputData.FullPurchaseNum + unhandleNum;
							countList[3] = 0;
							countList[4] = 0;
							this.SaveFullPurchaseList(countList);
						}
					}
				}
				result = countList;
			}
			return result;
		}

		
		public void OnRoleLogin(GameClient client)
		{
			if (!this.InActivityTime())
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					11,
					0,
					"",
					0,
					0
				});
				client.sendCmd(770, strcmd, false);
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					11,
					this.PlatformOpenStateVavle,
					"",
					0,
					0
				});
				client.sendCmd(770, strcmd, false);
			}
		}

		
		private JieriSuperInputData GetJieriSuperInputDataByNowDateTime(DateTime now, bool skipBegin = false)
		{
			JieriSuperInputData configData = null;
			List<JieriSuperInputData> openActList = this.JieriSuperInputDict.Values.ToList<JieriSuperInputData>().FindAll((JieriSuperInputData x) => x.BeginTime.DayOfYear == now.DayOfYear);
			openActList.Sort(delegate(JieriSuperInputData left, JieriSuperInputData right)
			{
				int result2;
				if (left.BeginTime.Ticks < right.BeginTime.Ticks)
				{
					result2 = -1;
				}
				else if (left.BeginTime.Ticks > right.BeginTime.Ticks)
				{
					result2 = 1;
				}
				else
				{
					result2 = 0;
				}
				return result2;
			});
			JieriSuperInputData result;
			if (openActList.Count == 0)
			{
				result = configData;
			}
			else
			{
				foreach (JieriSuperInputData config in openActList)
				{
					if (!skipBegin || !(now >= config.BeginTime))
					{
						if (now <= config.EndTime)
						{
							configData = config;
							break;
						}
					}
				}
				if (null == configData)
				{
					configData = openActList[openActList.Count - 1];
				}
				result = configData;
			}
			return result;
		}

		
		public string ExecuteSuperInput(GameClient client)
		{
			DateTime now = TimeUtil.NowDateTime();
			string cmdData = "";
			int fullPur = 0;
			int fullPurReserve = 0;
			int hasGetTimes = 0;
			int result = 0;
			lock (JieriSuperInputActivity._SuperInputMutex)
			{
				if (!this.InActivityTime() || this.PlatformOpenStateVavle == 0)
				{
					result = -400;
				}
				else
				{
					JieriSuperInputData superInputData = this.GetJieriSuperInputDataByNowDateTime(now, false);
					if (superInputData == null || now < superInputData.BeginTime)
					{
						result = -400;
					}
					else
					{
						List<int> countList = this.GetFullPurchaseList(now);
						fullPur = countList[2];
						fullPurReserve = countList[3];
						if (fullPurReserve >= fullPur)
						{
							result = -16;
						}
						else
						{
							string beginStr = superInputData.BeginTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
							string endStr = superInputData.EndTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
							string keyHasStr = string.Format("has_{0}_{1}_{2}", beginStr, endStr, superInputData.ID);
							if (superInputData.PurchaseNum > 0)
							{
								string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									client.ClientData.RoleID,
									keyHasStr,
									this.ActivityType,
									"0"
								});
								string[] dbResult = Global.ExecuteDBCmd(10221, strcmd, 0);
								if (dbResult == null || dbResult.Length == 0)
								{
									return cmdData;
								}
								hasGetTimes = Global.SafeConvertToInt32(dbResult[3]);
							}
							if (superInputData.PurchaseNum > 0 && hasGetTimes >= superInputData.PurchaseNum)
							{
								result = -16;
							}
							else
							{
								string keyResStr = string.Format("res_{0}_{1}_{2}", beginStr, endStr, superInputData.ID);
								string[] dbResult = Global.QeuryUserActivityInfo(client, keyResStr, this.ActivityType, "0");
								if (dbResult == null || dbResult.Length == 0)
								{
									return cmdData;
								}
								int reverseTimes = Global.SafeConvertToInt32(dbResult[3]);
								fullPurReserve++;
								List<int> list;
								(list = countList)[3] = list[3] + 1;
								this.SaveFullPurchaseList(countList);
								Global.UpdateUserActivityInfo(client, keyHasStr, 71, (long)(++hasGetTimes), now.ToString("yyyy-MM-dd HH$mm$ss"));
								Global.UpdateUserActivityInfo(client, keyResStr, 71, (long)(reverseTimes + 1), now.ToString("yyyy-MM-dd HH$mm$ss"));
							}
						}
					}
				}
			}
			return string.Format("{0},{1},{2},{3}", new object[]
			{
				result,
				hasGetTimes,
				fullPur,
				fullPurReserve
			});
		}

		
		public string BuildSuperInputFanLiActInfoForClient(GameClient client)
		{
			string cmdData = "";
			string result2;
			if (!this.InActivityTime() || this.PlatformOpenStateVavle == 0)
			{
				cmdData = string.Format("{0},{1},{2}", 0, 0, 0);
				result2 = cmdData;
			}
			else
			{
				DateTime now = TimeUtil.NowDateTime();
				JieriSuperInputData superInputData = this.GetJieriSuperInputDataByNowDateTime(now, false);
				if (null == superInputData)
				{
					cmdData = string.Format("{0},{1},{2}", 0, 0, 0);
					result2 = cmdData;
				}
				else if (now < superInputData.BeginTime)
				{
					cmdData = string.Format("{0},{1},{2}", 0, superInputData.FullPurchaseNum, 0);
					result2 = cmdData;
				}
				else
				{
					List<int> countList = this.GetFullPurchaseList(now);
					int fullPur = countList[2];
					int fullPurReserve = countList[3];
					int hasGetTimes = 0;
					string beginStr = superInputData.BeginTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
					string endStr = superInputData.EndTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
					string keyStr = string.Format("has_{0}_{1}_{2}", beginStr, endStr, superInputData.ID);
					if (superInputData.PurchaseNum > 0)
					{
						string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							client.ClientData.RoleID,
							keyStr,
							this.ActivityType,
							"0"
						});
						string[] result = Global.ExecuteDBCmd(10221, strcmd, 0);
						if (result == null || result.Length == 0)
						{
							return cmdData;
						}
						hasGetTimes = Global.SafeConvertToInt32(result[3]);
					}
					if (countList[3] >= countList[2])
					{
						JieriSuperInputData nextSuperInputData = this.GetJieriSuperInputDataByNowDateTime(now, true);
						if (null != nextSuperInputData)
						{
							return string.Format("{0},{1},{2}", 0, nextSuperInputData.FullPurchaseNum, nextSuperInputData.FullPurchaseNum);
						}
					}
					cmdData = string.Format("{0},{1},{2}", hasGetTimes, fullPur, fullPurReserve);
					result2 = cmdData;
				}
			}
			return result2;
		}

		
		public void FilterSingleChargeData(SingleChargeData data)
		{
			if (null != data)
			{
				if (this.PlatformOpenStateVavle == 0)
				{
					data.SuperInputFanLiKey = "";
					data.SuperInputFanLiDict.Clear();
				}
				else
				{
					data.SuperInputFanLiDict.Clear();
					data.SuperInputFanLiKey = string.Format("{0}_{1}", this.FromDate, this.ToDate);
					foreach (JieriSuperInputData item in this.JieriSuperInputDict.Values)
					{
						data.SuperInputFanLiDict[item.ID] = item;
					}
				}
			}
		}

		
		public void OnMoneyChargeEvent(string userid, int roleid, int addMoney, int superInputFanLi)
		{
			DateTime now = TimeUtil.NowDateTime();
			JieriSuperInputData superInputData = null;
			if (this.JieriSuperInputDict.TryGetValue(superInputFanLi, out superInputData))
			{
				GameClient otherClient = null;
				TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserID(userid);
				if (null != clientSocket)
				{
					otherClient = GameManager.ClientMgr.FindClient(clientSocket);
				}
				lock (JieriSuperInputActivity._SuperInputMutex)
				{
					List<int> countList = this.GetFullPurchaseList(now);
					if (countList[0] == TimeUtil.GetOffsetDay(now) && countList[1] == superInputFanLi)
					{
						List<int> list;
						(list = countList)[4] = list[4] + 1;
						this.SaveFullPurchaseList(countList);
					}
				}
				if (null == otherClient)
				{
					if (superInputData.MutiNum > 0)
					{
						GameManager.ClientMgr.AddOfflineUserMoney(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, roleid, roleid.ToString(), (superInputData.MutiNum - 1) * Global.TransMoneyToYuanBao(addMoney), "节日超级充值返利钻石(离线)", 0, userid);
					}
				}
				else
				{
					if (superInputData.MutiNum > 0)
					{
						GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, (superInputData.MutiNum - 1) * Global.TransMoneyToYuanBao(addMoney), "节日超级充值返利钻石", ActivityTypes.None, "");
					}
					if (superInputData.PurchaseNum > 0 && now >= superInputData.BeginTime && now <= superInputData.EndTime)
					{
						string cmd = this.BuildSuperInputFanLiActInfoForClient(otherClient);
						otherClient.sendCmd(1622, cmd, false);
					}
				}
			}
		}

		
		protected const string JieriSuperInputActivityData_fileName = "Config/MU_ChongZhiFanLi.xml";

		
		private static object _SuperInputMutex = new object();

		
		protected Dictionary<int, JieriSuperInputData> JieriSuperInputDict = new Dictionary<int, JieriSuperInputData>();

		
		public int PlatformOpenStateVavle = 0;
	}
}
