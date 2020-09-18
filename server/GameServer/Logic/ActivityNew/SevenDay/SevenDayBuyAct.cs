using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.ActivityNew.SevenDay
{
	
	public class SevenDayBuyAct
	{
		
		public void LoadConfig()
		{
			Dictionary<int, SevenDayBuyAct._BuyGoodsData> tmpDict = new Dictionary<int, SevenDayBuyAct._BuyGoodsData>();
			try
			{
				XElement xml = XElement.Load(Global.GameResPath("Config/SevenDay/SevenDayQiangGou.xml"));
				foreach (XElement xmlItem in xml.Elements())
				{
					SevenDayBuyAct._BuyGoodsData data = new SevenDayBuyAct._BuyGoodsData();
					data.Id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					data.Day = (int)Global.GetSafeAttributeLong(xmlItem, "Day");
					data.OriginPrice = (int)Global.GetSafeAttributeLong(xmlItem, "OrigPrice");
					data.CurrPrice = (int)Global.GetSafeAttributeLong(xmlItem, "Price");
					data.MaxBuyCount = (int)Global.GetSafeAttributeLong(xmlItem, "Purchase");
					data.Goods = Global.ParseGoodsFromStr_7(Global.GetSafeAttributeStr(xmlItem, "GoodsID").Split(new char[]
					{
						','
					}), 0);
					tmpDict[data.Id] = data;
				}
				lock (this.ConfigMutex)
				{
					this._BuyGoodsDict = tmpDict;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("七日登录活动加载配置失败{0}", "Config/SevenDay/SevenDayQiangGou.xml"), ex, true);
			}
		}

		
		public ESevenDayActErrorCode HandleClientBuy(GameClient client, int id, int cnt)
		{
			int currDay;
			ESevenDayActErrorCode result;
			if (!SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client, out currDay))
			{
				result = ESevenDayActErrorCode.NotInActivityTime;
			}
			else
			{
				SevenDayBuyAct._BuyGoodsData goodsConfig = null;
				lock (this.ConfigMutex)
				{
					if (this._BuyGoodsDict == null || !this._BuyGoodsDict.TryGetValue(id, out goodsConfig))
					{
						return ESevenDayActErrorCode.ServerConfigError;
					}
				}
				if (goodsConfig == null || goodsConfig.Goods == null)
				{
					result = ESevenDayActErrorCode.ServerConfigError;
				}
				else if (goodsConfig.Day > currDay)
				{
					result = ESevenDayActErrorCode.NotReachCondition;
				}
				else
				{
					Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Buy);
					lock (itemDict)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(id, out itemData))
						{
							itemData = new SevenDayItemData();
							itemDict[id] = itemData;
						}
						if (cnt <= 0 || itemData.Params1 + cnt > goodsConfig.MaxBuyCount)
						{
							result = ESevenDayActErrorCode.NoEnoughGoodsCanBuy;
						}
						else if (client.ClientData.UserMoney < cnt * goodsConfig.CurrPrice)
						{
							result = ESevenDayActErrorCode.ZuanShiNotEnough;
						}
						else if (!Global.CanAddGoods(client, goodsConfig.Goods.GoodsID, goodsConfig.Goods.GCount * cnt, goodsConfig.Goods.Binding, "1900-01-01 12:00:00", true, false))
						{
							result = ESevenDayActErrorCode.NoBagSpace;
						}
						else
						{
							itemData.Params1 += cnt;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(client.ClientData.RoleID, ESevenDayActType.Buy, id, itemData, client.ServerId))
							{
								itemData.Params1 -= cnt;
								result = ESevenDayActErrorCode.DBFailed;
							}
							else
							{
								if (!GameManager.ClientMgr.SubUserMoney(client, cnt * goodsConfig.CurrPrice, "七日抢购", true, true, true, true, DaiBiSySType.None))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("玩家七日抢购物品，检查钻石足够，但是扣除失败,roleid={0}, id={1}", client.ClientData.RoleID, id), null, true);
								}
								GoodsData goodsData = goodsConfig.Goods;
								Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount * cnt, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, string.Format("七日抢购", new object[0]), false, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
								result = ESevenDayActErrorCode.Success;
							}
						}
					}
				}
			}
			return result;
		}

		
		public bool HasAnyCanBuy(GameClient client)
		{
			int currDay;
			bool result;
			if (!SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client, out currDay))
			{
				result = false;
			}
			else
			{
				Dictionary<int, SevenDayBuyAct._BuyGoodsData> tmpConfigDict = null;
				lock (this.ConfigMutex)
				{
					if ((tmpConfigDict = this._BuyGoodsDict) == null || tmpConfigDict.Count <= 0)
					{
						return false;
					}
				}
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Buy);
				lock (itemDict)
				{
					foreach (KeyValuePair<int, SevenDayBuyAct._BuyGoodsData> kvp in tmpConfigDict)
					{
						SevenDayBuyAct._BuyGoodsData goodsConfig = kvp.Value;
						if (goodsConfig != null && goodsConfig.Day <= currDay)
						{
							int hasBuy = 0;
							SevenDayItemData itemData = null;
							if (itemDict.TryGetValue(kvp.Key, out itemData))
							{
								hasBuy = itemData.Params1;
							}
							if (goodsConfig.MaxBuyCount > hasBuy)
							{
								return true;
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		
		private Dictionary<int, SevenDayBuyAct._BuyGoodsData> _BuyGoodsDict = null;

		
		private object ConfigMutex = new object();

		
		private class _BuyGoodsData
		{
			
			public int Id;

			
			public int Day;

			
			public int OriginPrice;

			
			public int CurrPrice;

			
			public int MaxBuyCount;

			
			public GoodsData Goods;
		}
	}
}
