using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic.CheatGuard
{
	
	public class TradeBlackManager : SingletonTemplate<TradeBlackManager>
	{
		
		private TradeBlackManager()
		{
		}

		
		public bool LoadConfig()
		{
			bool bResult = true;
			try
			{
				if (!File.Exists(Global.GameResPath("Config\\Blacklist.xml")))
				{
					return false;
				}
				Dictionary<int, int> tmpGoodsPriceDict = new Dictionary<int, int>();
				XElement xml = XElement.Load(Global.GameResPath("Config\\Blacklist.xml"));
				foreach (XElement xmlItem in xml.Elements())
				{
					int goods = (int)Global.GetSafeAttributeLong(xmlItem, "GoodsID");
					int price = (int)Global.GetSafeAttributeLong(xmlItem, "Price");
					tmpGoodsPriceDict.Add(goods, price);
				}
				this.GoodsPriceDict = tmpGoodsPriceDict;
			}
			catch (Exception ex)
			{
				bResult = false;
				LogManager.WriteLog(LogTypes.Error, "load Config\\Blacklist.xml exception!", ex, true);
			}
			try
			{
				List<TradeConfigItem> tmpTradeCfgItems = new List<TradeConfigItem>();
				XElement xml = XElement.Load(Global.GameResPath("Config\\TradeConfig.xml"));
				foreach (XElement xmlItem in xml.Elements())
				{
					TradeConfigItem item = new TradeConfigItem();
					item.Id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					item.MinVip = (int)Global.GetSafeAttributeLong(xmlItem, "MinVip");
					item.MaxVip = (int)Global.GetSafeAttributeLong(xmlItem, "MaxVip");
					item.MaxPrice = (int)Global.GetSafeAttributeLong(xmlItem, "MaxPrice");
					item.MaxTimes = (int)Global.GetSafeAttributeLong(xmlItem, "MaxNum");
					int cl = (int)Global.GetSafeAttributeLong(xmlItem, "MinZhuanSheng");
					int lvl = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel");
					item.UnionMinLevel = Global.GetUnionLevel(cl, lvl, false);
					cl = (int)Global.GetSafeAttributeLong(xmlItem, "MaxZhuanSheng");
					lvl = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel");
					item.UnionMaxLevel = Global.GetUnionLevel(cl, lvl, false);
					tmpTradeCfgItems.Add(item);
				}
				this.TradeCfgItems = tmpTradeCfgItems;
			}
			catch (Exception ex)
			{
				bResult = false;
				LogManager.WriteLog(LogTypes.Error, "load Config\\TradeConfig.xml exception!", ex, true);
			}
			string szPlatFlag = string.Empty;
			PlatformTypes pt = GameCoreInterface.getinstance().GetPlatformType();
			if (pt == PlatformTypes.Android)
			{
				szPlatFlag = "Android";
			}
			else if (pt == PlatformTypes.YueYu)
			{
				szPlatFlag = "YueYu";
			}
			else if (pt == PlatformTypes.APP)
			{
				szPlatFlag = "APP";
			}
			else if (pt == PlatformTypes.YYB)
			{
				szPlatFlag = "YYB";
			}
			this.BanTradeSec = (int)GameManager.systemParamsList.GetParamValueIntByName("NoTrade_" + szPlatFlag, -1);
			this.BanTradeLog = (int)GameManager.systemParamsList.GetParamValueIntByName("TradeLog_" + szPlatFlag, -1);
			this.BanTradeLogin = (int)GameManager.systemParamsList.GetParamValueIntByName("TradeKill_" + szPlatFlag, -1);
			return bResult;
		}

		
		public bool IsBanTrade(int roleId)
		{
			bool bBan = false;
			TradeBlackObject obj = this.LoadTradeBlackObject(roleId, true);
			if (obj != null)
			{
				bBan = (obj.BanTradeToTicks > 0L && obj.BanTradeToTicks > TimeUtil.NowDateTime().Ticks);
			}
			return bBan;
		}

		
		public void UpdateObjectExtData(GameClient client)
		{
			if (client != null)
			{
				TradeBlackObject obj = this.LoadTradeBlackObject(client.ClientData.RoleID, false);
				if (obj != null)
				{
					lock (obj)
					{
						obj.ChangeLife = client.ClientData.ChangeLifeCount;
						obj.Level = client.ClientData.Level;
						obj.VipLevel = client.ClientData.VipLevel;
						obj.ZoneId = client.ClientData.ZoneID;
						obj.RoleName = client.ClientData.RoleName;
					}
				}
			}
		}

		
		public void SetBanTradeToTicks(int roleid, long toTicks)
		{
			toTicks = Math.Max(0L, toTicks);
			GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", roleid, 3, toTicks), null, 0);
			TradeBlackObject obj = this.LoadTradeBlackObject(roleid, true);
			if (obj != null)
			{
				lock (obj)
				{
					obj.BanTradeToTicks = toTicks;
				}
			}
			long banTradeSec = 0L;
			if (toTicks > TimeUtil.NowDateTime().Ticks)
			{
				banTradeSec = (long)(new DateTime(toTicks) - TimeUtil.NowDateTime()).TotalSeconds;
				banTradeSec = Math.Max(0L, banTradeSec);
			}
			if (banTradeSec > 0L)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("roleid={0} 被封禁交易，秒数={1}", roleid, banTradeSec), null, true);
			}
			GameClient client = GameManager.ClientMgr.FindClient(roleid);
			if (client != null)
			{
				client.ClientData.BanTradeToTicks = toTicks;
				if (banTradeSec > 0L)
				{
					string tip = string.Format(GLang.GetLang(35, new object[0]), banTradeSec);
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				}
			}
		}

		
		private void CheckBanTrade(int roleId)
		{
			TradeBlackObject obj = this.LoadTradeBlackObject(roleId, true);
			if (obj != null)
			{
				int kick_out_minutes = -1;
				lock (obj)
				{
					if (obj.BanTradeToTicks <= 0L)
					{
						List<TradeConfigItem> items = this.TradeCfgItems;
						TradeConfigItem tradeConfigItem;
						if (items == null)
						{
							tradeConfigItem = null;
						}
						else
						{
							tradeConfigItem = items.Find((TradeConfigItem _i) => _i.MinVip <= obj.VipLevel && _i.MaxVip >= obj.VipLevel && _i.UnionMinLevel <= Global.GetUnionLevel(obj.ChangeLife, obj.Level, false) && _i.UnionMaxLevel >= Global.GetUnionLevel(obj.ChangeLife, obj.Level, false));
						}
						TradeConfigItem item = tradeConfigItem;
						if (item != null)
						{
							long totalInPrice = 0L;
							long totalOutPrice = 0L;
							long totalTimes = 0L;
							foreach (TradeBlackHourItem hourItem in obj.HourItems)
							{
								if (hourItem != null)
								{
									totalInPrice += hourItem.MarketInPrice + hourItem.TradeInPrice;
									totalOutPrice += hourItem.MarketOutPrice + hourItem.TradeOutPrice;
									totalTimes += (long)(hourItem.MarketTimes + hourItem.TradeTimes);
								}
							}
							if (totalInPrice >= (long)item.MaxPrice || totalOutPrice >= (long)item.MaxPrice || totalTimes >= (long)item.MaxTimes)
							{
								int _banTradeSec = Math.Max(this.BanTradeSec, 0);
								if (_banTradeSec > 0)
								{
									long toTicks = TimeUtil.NowDateTime().AddSeconds((double)_banTradeSec).Ticks;
									this.SetBanTradeToTicks(roleId, toTicks);
								}
								if (this.BanTradeLog == 1)
								{
									LogManager.WriteLog(LogTypes.Analysis, string.Format("tradeblack player={0} inprice={1} outprice={2} times={3} bansec={4}", new object[]
									{
										roleId,
										totalInPrice,
										totalOutPrice,
										totalTimes,
										_banTradeSec
									}), null, true);
								}
								kick_out_minutes = Math.Max(this.BanTradeLogin, 0) / 60;
								if (kick_out_minutes > 0)
								{
									BanManager.BanRoleName(Global.FormatRoleName3(obj.ZoneId, obj.RoleName), kick_out_minutes, 3);
								}
							}
						}
					}
				}
				if (kick_out_minutes > 0)
				{
					GameClient client = GameManager.ClientMgr.FindClient(roleId);
					if (client != null)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(36, new object[0]), new object[]
						{
							kick_out_minutes
						}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
						Global.ForceCloseClient(client, "交易封禁", true);
					}
				}
			}
		}

		
		private void CheckUnBanTrade(int roleId)
		{
			TradeBlackObject obj = this.LoadTradeBlackObject(roleId, true);
			if (obj != null)
			{
				lock (obj)
				{
					if (obj.BanTradeToTicks > 0L && obj.BanTradeToTicks < TimeUtil.NowDateTime().Ticks)
					{
						this.SetBanTradeToTicks(roleId, 0L);
					}
				}
			}
		}

		
		public void Update()
		{
			if ((TimeUtil.NowDateTime() - this.lastCheckUnBanTime).TotalSeconds > 60.0)
			{
				this.lastCheckUnBanTime = TimeUtil.NowDateTime();
				List<int> roleIds = null;
				lock (this.TradeBlackObjs)
				{
					roleIds = this.TradeBlackObjs.Keys.ToList<int>();
				}
				if (roleIds != null)
				{
					roleIds.ForEach(delegate(int _r)
					{
						this.CheckUnBanTrade(_r);
					});
				}
			}
			if ((TimeUtil.NowDateTime() - this.lastFreeUnusedTime).TotalHours > 1.0)
			{
				this.lastFreeUnusedTime = TimeUtil.NowDateTime();
				List<int> roleIds = null;
				lock (this.TradeBlackObjs)
				{
					roleIds = this.TradeBlackObjs.Keys.ToList<int>();
				}
				List<int> list;
				if (roleIds == null)
				{
					list = null;
				}
				else
				{
					list = roleIds.FindAll(delegate(int _r)
					{
						TradeBlackObject obj = this.LoadTradeBlackObject(_r, true);
						return obj != null && obj.BanTradeToTicks <= 0L && (TimeUtil.NowDateTime() - obj.LastFlushTime).TotalHours > 12.0;
					});
				}
				List<int> removeIds = list;
				if (removeIds != null)
				{
					lock (this.TradeBlackObjs)
					{
						foreach (int id in removeIds)
						{
							this.TradeBlackObjs.Remove(id);
						}
					}
				}
			}
		}

		
		public void OnExchange(int roleid1, int roleid2, List<GoodsData> gdList1, List<GoodsData> gdList2, int zuanshi1, int zuanshi2)
		{
			long price = (long)((zuanshi1 > 0) ? zuanshi1 : 0);
			long price2 = (long)((zuanshi2 > 0) ? zuanshi2 : 0);
			Func<List<GoodsData>, Dictionary<int, int>, long> _GetGoodsPrice = delegate(List<GoodsData> gdList, Dictionary<int, int> priceDict)
			{
				long totalPrice = 0L;
				if (gdList != null && priceDict != null)
				{
					gdList.ForEach(delegate(GoodsData _g)
					{
						totalPrice += (long)(priceDict.ContainsKey(_g.GoodsID) ? (priceDict[_g.GoodsID] * _g.GCount) : 0);
					});
				}
				return totalPrice;
			};
			price += _GetGoodsPrice(gdList1, this.GoodsPriceDict);
			price2 += _GetGoodsPrice(gdList2, this.GoodsPriceDict);
			DateTime now = TimeUtil.NowDateTime();
			TradeBlackObject obj = this.LoadTradeBlackObject(roleid1, true);
			if (obj != null)
			{
				lock (obj)
				{
					TradeBlackHourItem item = this.GetBlackHourItem(obj, now);
					item.TradeTimes++;
					item.TradeOutPrice += price;
					item.TradeInPrice += price2;
					if (!item.TradeRoles.Contains(roleid2))
					{
						item.TradeRoles.Add(roleid2);
						item.TradeDistinctRoleCount++;
					}
					TradeBlackHourItem itemCopy = item.SimpleClone();
					this.SaveTradeBlackObject(itemCopy);
				}
			}
			TradeBlackObject obj2 = this.LoadTradeBlackObject(roleid2, true);
			if (obj2 != null)
			{
				lock (obj2)
				{
					TradeBlackHourItem item = this.GetBlackHourItem(obj2, now);
					item.TradeTimes++;
					item.TradeInPrice += price;
					item.TradeOutPrice += price2;
					if (!item.TradeRoles.Contains(roleid1))
					{
						item.TradeRoles.Add(roleid1);
						item.TradeDistinctRoleCount++;
					}
					TradeBlackHourItem itemCopy = item.SimpleClone();
					this.SaveTradeBlackObject(itemCopy);
				}
			}
			this.CheckBanTrade(roleid1);
			this.CheckBanTrade(roleid2);
		}

		
		public void OnMarketBuy(int whoBuy, int whoSale, GoodsData saleGoods)
		{
			if (saleGoods != null)
			{
				int pay = Math.Max(saleGoods.SaleYuanBao + saleGoods.SaleYinPiao, 0);
				Dictionary<int, int> tmpPriceDict = this.GoodsPriceDict;
				int price = (tmpPriceDict != null && tmpPriceDict.ContainsKey(saleGoods.GoodsID)) ? (tmpPriceDict[saleGoods.GoodsID] * saleGoods.GCount) : 0;
				DateTime now = TimeUtil.NowDateTime();
				TradeBlackObject buyer = this.LoadTradeBlackObject(whoBuy, true);
				if (buyer != null)
				{
					lock (buyer)
					{
						TradeBlackHourItem item = this.GetBlackHourItem(buyer, now);
						item.MarketTimes++;
						item.MarketInPrice += (long)price;
						item.MarketOutPrice += (long)pay;
						if (!item.TradeRoles.Contains(whoSale))
						{
							item.TradeRoles.Add(whoSale);
							item.TradeDistinctRoleCount++;
						}
						TradeBlackHourItem itemCopy = item.SimpleClone();
						this.SaveTradeBlackObject(itemCopy);
					}
				}
				TradeBlackObject saler = this.LoadTradeBlackObject(whoSale, true);
				if (saler != null)
				{
					lock (saler)
					{
						TradeBlackHourItem item = this.GetBlackHourItem(saler, now);
						item.MarketTimes++;
						item.MarketOutPrice += (long)price;
						item.MarketInPrice += (long)pay;
						if (!item.TradeRoles.Contains(whoBuy))
						{
							item.TradeRoles.Add(whoBuy);
							item.TradeDistinctRoleCount++;
						}
						TradeBlackHourItem itemCopy = item.SimpleClone();
						this.SaveTradeBlackObject(itemCopy);
					}
				}
				this.CheckBanTrade(whoBuy);
				this.CheckBanTrade(whoSale);
			}
		}

		
		private TradeBlackObject LoadTradeBlackObject(int roleid, bool loadDbIfNotExist = true)
		{
			DateTime now = TimeUtil.NowDateTime();
			TradeBlackObject obj = null;
			int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
			lock (this.TradeBlackObjs)
			{
				if (this.TradeBlackObjs.TryGetValue(roleid, out obj))
				{
					obj.LastFlushTime = now;
				}
			}
			if (obj == null && loadDbIfNotExist)
			{
				string reqCmd = string.Format("{0}:{1}:{2}", roleid, now.ToString("yyyy-MM-dd"), now.Hour);
				List<TradeBlackHourItem> items = Global.sendToDB<List<TradeBlackHourItem>, string>(14007, reqCmd, 0);
				obj = new TradeBlackObject();
				obj.RoleId = roleid;
				obj.LastFlushTime = now;
				obj.HourItems = new TradeBlackHourItem[24];
				GameClient client = GameManager.ClientMgr.FindClient(roleid);
				if (client != null)
				{
					obj.VipLevel = client.ClientData.VipLevel;
					obj.ChangeLife = client.ClientData.ChangeLifeCount;
					obj.Level = client.ClientData.Level;
					obj.BanTradeToTicks = client.ClientData.BanTradeToTicks;
					obj.ZoneId = client.ClientData.ZoneID;
					obj.RoleName = client.ClientData.RoleName;
				}
				else
				{
					SafeClientData clientData = Global.GetSafeClientDataFromLocalOrDB(roleid);
					if (clientData != null)
					{
						obj.VipLevel = Global.CalcVipLevelByZuanShi(Global.GetUserInputAllYuanBao(clientData.RoleID, clientData.RoleName, 0));
						obj.ChangeLife = clientData.ChangeLifeCount;
						obj.Level = clientData.Level;
						obj.BanTradeToTicks = clientData.BanTradeToTicks;
						obj.ZoneId = clientData.ZoneID;
						obj.RoleName = clientData.RoleName;
					}
				}
				if (items != null)
				{
					foreach (TradeBlackHourItem item in items)
					{
						int idx = item.Hour % 24;
						obj.HourItems[idx] = item;
						item.TradeRoles = (item.TradeRoles ?? new HashSet<int>());
					}
				}
				lock (this.TradeBlackObjs)
				{
					if (!this.TradeBlackObjs.ContainsKey(roleid))
					{
						this.TradeBlackObjs[roleid] = obj;
					}
					else
					{
						obj = this.TradeBlackObjs[roleid];
					}
				}
			}
			return obj;
		}

		
		private TradeBlackHourItem GetBlackHourItem(TradeBlackObject obj, DateTime date)
		{
			TradeBlackHourItem item = obj.HourItems[date.Hour];
			if (item == null || item.Day != date.ToString("yyyy-MM-dd"))
			{
				item = new TradeBlackHourItem();
				item.RoleId = obj.RoleId;
				item.Day = date.ToString("yyyy-MM-dd");
				item.Hour = date.Hour;
				item.TradeRoles = new HashSet<int>();
				obj.HourItems[date.Hour] = item;
			}
			return item;
		}

		
		private void SaveTradeBlackObject(TradeBlackHourItem obj)
		{
			if (obj != null)
			{
				if (!Global.sendToDB<bool, TradeBlackHourItem>(14008, obj, 0))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("TradeBlackManager.SaveTradeBlackObject failed!, roleid={0}", obj.RoleId), null, true);
				}
			}
		}

		
		public bool CheckTrade(GameClient client, MoneyTypes moneyType, bool notify = true)
		{
			if (moneyType == MoneyTypes.YuanBao)
			{
				if ((Data.OpenData.paimaihangzuanshi == 2 && client.ClientSocket.session.IsAdult == 0) || Data.OpenData.paimaihangzuanshi <= 0)
				{
					if (notify)
					{
						string tip = GLang.GetLang(37, new object[0]);
						GameManager.ClientMgr.NotifyHintMsg(client, tip);
					}
					return false;
				}
			}
			else if (moneyType == MoneyTypes.MoBi)
			{
				if ((Data.OpenData.paimaihangmobi == 2 && client.ClientSocket.session.IsAdult != 0) || Data.OpenData.paimaihangmobi <= 0)
				{
					if (notify)
					{
						string tip = GLang.GetLang(38, new object[0]);
						GameManager.ClientMgr.NotifyHintMsg(client, tip);
					}
					return false;
				}
			}
			else if (moneyType == MoneyTypes.YinLiang)
			{
				if ((Data.OpenData.paimaihangjinbi == 2 && client.ClientSocket.session.IsAdult != 0) || Data.OpenData.paimaihangjinbi <= 0)
				{
					if (notify)
					{
						string tip = GLang.GetLang(39, new object[0]);
						GameManager.ClientMgr.NotifyHintMsg(client, tip);
					}
					return false;
				}
			}
			return true;
		}

		
		private const string GoodsPriceCfgFile = "Config\\Blacklist.xml";

		
		private const string TradeBlackCfgFile = "Config\\TradeConfig.xml";

		
		private Dictionary<int, TradeBlackObject> TradeBlackObjs = new Dictionary<int, TradeBlackObject>();

		
		private DateTime lastCheckUnBanTime = TimeUtil.NowDateTime();

		
		private DateTime lastFreeUnusedTime = TimeUtil.NowDateTime();

		
		private Dictionary<int, int> GoodsPriceDict = null;

		
		private List<TradeConfigItem> TradeCfgItems = null;

		
		private int BanTradeSec = -1;

		
		private int BanTradeLog = 0;

		
		private int BanTradeLogin = -1;
	}
}
