using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.UserMoneyCharge;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class RegressActiveDayBuy : Activity, IEventListener
	{
		
		public void Dispose()
		{
			GlobalEventSource.getInstance().removeListener(36, this);
		}

		
		public bool Init()
		{
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config\\HuiGuiDayZhiGou.xml"));
			XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config\\HuiGuiDayZhiGou.xml"));
			bool result;
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", "Config\\HuiGuiDayZhiGou.xml"), null, true);
				result = false;
			}
			else
			{
				this.ActivityType = 113;
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				try
				{
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						RegressActiveDayBuyXML Regress = new RegressActiveDayBuyXML();
						Dictionary<int, int> TotalYuanBao = new Dictionary<int, int>();
						Regress.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
						string[] Price = Global.GetSafeAttributeStr(xmlItem, "Price").Split(new char[]
						{
							'|'
						});
						Regress.ZhiGouID = Convert.ToInt32(Price[2]);
						Regress.HuoDongLevel = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "HuoDongLevel"));
						string[] StrRegress = Global.GetSafeAttributeStr(xmlItem, "TotalYuanBao").Split(new char[]
						{
							','
						});
						TotalYuanBao.Add(Convert.ToInt32(StrRegress[0]), Convert.ToInt32(StrRegress[1]));
						Regress.TotalYuanBao = TotalYuanBao;
						Regress.Day = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Day"));
						Regress.Max = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Max"));
						string goods = Global.GetSafeAttributeStr(xmlItem, "GoodsID1");
						string goods2 = Global.GetSafeAttributeStr(xmlItem, "GoodsID2");
						this.regressActiveDayBuyXML.Add(Regress.ID, Regress);
						this.ActZhiGouIDSet.Add(Regress.ZhiGouID);
						UserMoneyMgr.getInstance().CheckChargeItemConfigLogic(Regress.ZhiGouID, Regress.Max, goods, goods2, string.Format("三周年直购 ID={0}", Regress.ID));
					}
					if (this.regressActiveDayBuyXML == null || this.ActZhiGouIDSet == null)
					{
						return false;
					}
					base.PredealDateTime();
					GlobalEventSource.getInstance().registerListener(36, this);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				result = true;
			}
			return result;
		}

		
		public void OnRoleLogin(GameClient client)
		{
			if (!this.InActivityTime())
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					16,
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
					16,
					RegressActiveOpen.OpenStateVavle,
					"",
					0,
					0
				});
				client.sendCmd(770, strcmd, false);
			}
		}

		
		public bool CheckValidChargeItem(GameClient client, int zhigouID)
		{
			bool result;
			if (!this.ActZhiGouIDSet.Contains(zhigouID))
			{
				result = true;
			}
			else
			{
				RegressActiveOpen iflAct = HuodongCachingMgr.GetRegressActiveOpen();
				if (null == iflAct)
				{
					result = false;
				}
				else if (!iflAct.InActivityTime())
				{
					result = true;
				}
				else if (!iflAct.CanGiveAward())
				{
					result = false;
				}
				else
				{
					DateTime now = TimeUtil.NowDateTime();
					string newtime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).ToString("yyyy-MM-dd HH:mm:ss");
					int CDate = Global.GetOffsetDay(DateTime.Parse(newtime)) - Global.GetOffsetDay(DateTime.Parse(iflAct.FromDate));
					string ZeroTime = "2011-11-11 00$00$00";
					string GetInfoStr = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, ZeroTime, iflAct.FromDate.Replace(':', '$'));
					string[] dbResult;
					if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14136, GetInfoStr, out dbResult, 0))
					{
						result = false;
					}
					else if (dbResult == null || dbResult.Length != 2 || Convert.ToInt32(dbResult[0]) != client.ClientData.RoleID)
					{
						result = false;
					}
					else
					{
						int Money = Convert.ToInt32(dbResult[1]);
						if (Money < 0)
						{
							Money = 0;
						}
						string Regtime;
						if (!UserRegressActiveManager.GetRegressMinRegtime(client, out Regtime) || Regtime == null || Regtime.Equals(""))
						{
							result = false;
						}
						else
						{
							int ConfID;
							int CaleLevel = iflAct.GetUserActiveFile(Regtime, out ConfID);
							if (0 == CaleLevel)
							{
								result = false;
							}
							else
							{
								foreach (RegressActiveDayBuyXML item in this.regressActiveDayBuyXML.Values)
								{
									if (item.ZhiGouID == zhigouID)
									{
										foreach (KeyValuePair<int, int> it in item.TotalYuanBao)
										{
											if (it.Value == -1 && Money < it.Key)
											{
												return false;
											}
											if ((it.Value != -1 && Money > it.Value) || Money < it.Key)
											{
												return false;
											}
										}
										if (CDate + 1 != item.Day)
										{
											return false;
										}
										return true;
									}
								}
								result = false;
							}
						}
					}
				}
			}
			return result;
		}

		
		public Dictionary<int, int> BuildRegressZhiGouInfoForClient(GameClient client)
		{
			RegressActiveOpen iflAct = HuodongCachingMgr.GetRegressActiveOpen();
			Dictionary<int, int> result;
			if (iflAct == null || !iflAct.InActivityTime())
			{
				result = null;
			}
			else if (!iflAct.CanGiveAward())
			{
				result = null;
			}
			else
			{
				DateTime now = TimeUtil.NowDateTime();
				string newtime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).ToString("yyyy-MM-dd HH:mm:ss");
				int CDate = Global.GetOffsetDay(DateTime.Parse(newtime)) - Global.GetOffsetDay(DateTime.Parse(iflAct.FromDate));
				string ZeroTime = "2011-11-11 00$00$00";
				string GetInfoStr = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, ZeroTime, iflAct.FromDate.Replace(':', '$'));
				string[] dbResult;
				if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14136, GetInfoStr, out dbResult, 0))
				{
					result = null;
				}
				else if (dbResult == null || dbResult.Length != 2 || Convert.ToInt32(dbResult[0]) != client.ClientData.RoleID)
				{
					result = null;
				}
				else
				{
					int Money = Convert.ToInt32(dbResult[1]);
					if (Money < 0)
					{
						Money = 0;
					}
					Dictionary<int, int> ZhiGouInfoDict = new Dictionary<int, int>();
					foreach (RegressActiveDayBuyXML item in this.regressActiveDayBuyXML.Values)
					{
						bool flag = true;
						foreach (KeyValuePair<int, int> it in item.TotalYuanBao)
						{
							if (it.Value == -1 && Money < it.Key)
							{
								flag = false;
								break;
							}
							if (it.Value != -1 && (Money > it.Value || Money < it.Key))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							if (CDate + 1 == item.Day)
							{
								ZhiGouInfoDict[item.ID] = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, item.ZhiGouID);
							}
						}
					}
					result = ZhiGouInfoDict;
				}
			}
			return result;
		}

		
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 36)
			{
				ChargeItemBaseEventObject obj = eventObject as ChargeItemBaseEventObject;
				if (this.CheckValidChargeItem(obj.Player, obj.ChargeItemConfig.ChargeItemID))
				{
					Dictionary<int, int> ZhiGouInfoDict = this.BuildRegressZhiGouInfoForClient(obj.Player);
					obj.Player.sendCmd<Dictionary<int, int>>(2077, ZhiGouInfoDict, false);
					if (obj.Player._IconStateMgr.CheckThemeZhiGou(obj.Player))
					{
						obj.Player._IconStateMgr.SendIconStateToClient(obj.Player);
					}
				}
			}
		}

		
		public bool CheckClientCanBuy(GameClient client)
		{
			RegressActiveOpen iflAct = HuodongCachingMgr.GetRegressActiveOpen();
			bool result;
			if (iflAct == null || !iflAct.InActivityTime())
			{
				result = false;
			}
			else if (!iflAct.CanGiveAward())
			{
				result = false;
			}
			else
			{
				DateTime now = TimeUtil.NowDateTime();
				string newtime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).ToString("yyyy-MM-dd HH:mm:ss");
				int CDate = Global.GetOffsetDay(DateTime.Parse(newtime)) - Global.GetOffsetDay(DateTime.Parse(iflAct.FromDate));
				string ZeroTime = "2011-11-11 00$00$00";
				string GetInfoStr = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, ZeroTime, iflAct.FromDate.Replace(':', '$'));
				string[] dbResult;
				if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14136, GetInfoStr, out dbResult, 0))
				{
					result = false;
				}
				else if (dbResult == null || dbResult.Length != 2 || Convert.ToInt32(dbResult[0]) != client.ClientData.RoleID)
				{
					result = false;
				}
				else
				{
					int Money = Convert.ToInt32(dbResult[1]);
					if (Money < 0)
					{
						Money = 0;
					}
					foreach (RegressActiveDayBuyXML item in this.regressActiveDayBuyXML.Values)
					{
						bool flag = true;
						foreach (KeyValuePair<int, int> it in item.TotalYuanBao)
						{
							if (it.Value == -1 && Money < it.Key)
							{
								flag = false;
								break;
							}
							if (it.Value != -1 && (Money > it.Value || Money < it.Key))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							if (CDate == item.Day)
							{
								int PurNum = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, item.ZhiGouID);
								if (item.Max <= 0 || PurNum < item.Max)
								{
									return true;
								}
							}
						}
					}
					result = false;
				}
			}
			return result;
		}

		
		protected const string RegressActiveDayBuyXml = "Config\\HuiGuiDayZhiGou.xml";

		
		private Dictionary<int, RegressActiveDayBuyXML> regressActiveDayBuyXML = new Dictionary<int, RegressActiveDayBuyXML>();

		
		public HashSet<int> ActZhiGouIDSet = new HashSet<int>();
	}
}
