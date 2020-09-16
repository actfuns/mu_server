using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Tools;
using Server.Tools;

namespace GameServer.Logic.GoldAuction
{
	
	public class GoldAuctionConfigModel
	{
		
		public static int LoadConfig()
		{
			try
			{
				Dictionary<int, AuctionConfig> _AuctionDict;
				GoldAuctionConfigModel.LoadAuctionData(out _AuctionDict);
				List<AuctionAwardConfig> _AuctionAwardList;
				GoldAuctionConfigModel.LoadAngelTempleAuctionAwardData(out _AuctionAwardList);
				lock (GoldAuctionConfigModel.AuctionDict)
				{
					GoldAuctionConfigModel.AuctionDict = _AuctionDict;
				}
				lock (GoldAuctionConfigModel.AuctionAwardList)
				{
					GoldAuctionConfigModel.AuctionAwardList = _AuctionAwardList;
				}
				return 1;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return 0;
		}

		
		public static AuctionConfig GetAuctionConfig(int id)
		{
			AuctionConfig cfg = null;
			try
			{
				lock (GoldAuctionConfigModel.AuctionDict)
				{
					GoldAuctionConfigModel.AuctionDict.TryGetValue(id, out cfg);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return cfg;
		}

		
		public static AuctionAwardConfig RandAuctionAwardConfig()
		{
			try
			{
				lock (GoldAuctionConfigModel.AuctionAwardList)
				{
					if (GoldAuctionConfigModel.AuctionAwardList.Count < 1)
					{
						LogManager.WriteLog(LogTypes.Error, "[ljl]AuctionAwardList.Count < 1", null, true);
						return null;
					}
					List<int> weight = new List<int>();
					foreach (AuctionAwardConfig item in GoldAuctionConfigModel.AuctionAwardList)
					{
						int temp = item.EndValues - item.StartValues + 1;
						if (1 == temp && 0 == item.EndValues)
						{
							temp = 0;
						}
						else if (temp < 0)
						{
							temp = 0;
						}
						weight.Add(temp);
					}
					return GoldAuctionConfigModel.AuctionAwardList[RandomWeight.GetWeightIndex(weight, "金团随机")];
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return null;
		}

		
		private static bool LoadAngelTempleAuctionAwardData(out List<AuctionAwardConfig> _AuctionAwardList)
		{
			_AuctionAwardList = new List<AuctionAwardConfig>();
			try
			{
				XElement xml = CheckHelper.LoadXml(Global.GameResPath("Config/AngelTempleAuctionAward.xml"), true);
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("读取 {0} null == xml", "Config/AngelTempleAuctionAward.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						AuctionAwardConfig myData = new AuctionAwardConfig();
						myData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myData.Name = Global.GetSafeAttributeStr(xmlItem, "Name");
						string strTemp = Global.GetSafeAttributeStr(xmlItem, "GoodsID");
						if (!string.IsNullOrEmpty(strTemp))
						{
							string[] temp = strTemp.Split(new char[]
							{
								'|'
							});
							myData.strGoodsList = temp.ToList<string>();
							HuodongCachingMgr.ParseGoodsDataList(temp, "Config/AngelTempleAuctionAward.xml");
						}
						myData.StartValues = (int)Global.GetSafeAttributeLong(xmlItem, "StartValues");
						myData.EndValues = (int)Global.GetSafeAttributeLong(xmlItem, "EndValues");
						if (myData.EndValues - myData.StartValues < 0 || myData.StartValues < 0)
						{
							LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现问题, 第{1} 条数据 StartValues EndValues 值不对 ", "Config/AngelTempleAuctionAward.xml", _AuctionAwardList.Count + 1), null, true);
						}
						_AuctionAwardList.Add(myData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/AngelTempleAuctionAward.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		private static bool LoadAuctionData(out Dictionary<int, AuctionConfig> _AuctionAwardDict)
		{
			_AuctionAwardDict = new Dictionary<int, AuctionConfig>();
			try
			{
				XElement xml = CheckHelper.LoadXml(Global.GameResPath("Config/Auction.xml"), true);
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("读取 {0} null == xml", "Config/Auction.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						AuctionConfig myData = new AuctionConfig();
						myData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myData.Name = Global.GetSafeAttributeStr(xmlItem, "Name");
						myData.OriginPrice = (int)Global.GetSafeAttributeLong(xmlItem, "OriginPrice");
						myData.UnitPrice = (int)Global.GetSafeAttributeLong(xmlItem, "UnitPrice");
						myData.MaxPrice = (int)Global.GetSafeAttributeLong(xmlItem, "MaxPrice");
						myData.SuccessTitle = Global.GetSafeAttributeStr(xmlItem, "SuccessTitle");
						myData.SuccessIntro = Global.GetSafeAttributeStr(xmlItem, "SuccessIntro");
						myData.FailTitle = Global.GetSafeAttributeStr(xmlItem, "FailTitle");
						myData.FailIntro = Global.GetSafeAttributeStr(xmlItem, "FailIntro");
						string strTemp = Global.GetSafeAttributeStr(xmlItem, "List");
						if (!string.IsNullOrEmpty(strTemp))
						{
							foreach (int item in Global.StringArray2IntArray(strTemp.Split(new char[]
							{
								'|'
							})))
							{
								myData.OrderList.Add(item);
							}
						}
						strTemp = Global.GetSafeAttributeStr(xmlItem, "Time");
						if (!string.IsNullOrEmpty(strTemp))
						{
							foreach (int item in Global.StringArray2IntArray(strTemp.Split(new char[]
							{
								'|'
							})))
							{
								myData.TimeList.Add(item);
							}
						}
						if (myData.TimeList.Count != myData.OrderList.Count)
						{
							LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现问题, 第{1} 条数据 进入拍卖行顺序 竞拍时间长度不同 ", "Config/Auction.xml", _AuctionAwardDict.Count + 1), null, true);
						}
						_AuctionAwardDict.Add(myData.ID, myData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/Auction.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		private const string Auction = "Config/Auction.xml";

		
		private const string AngelTempleAuctionAward = "Config/AngelTempleAuctionAward.xml";

		
		public const string AngelTempleAuction = "AngelTempleAuction";

		
		public const string AngelTempleAuctionMin = "AngelTempleAuctionMin";

		
		public const string AuctionZhanMengOpen = "AuctionZhanMengOpen";

		
		public const string BuyTitle = "购买成功";

		
		public const string BuyIntro = "在金团拍卖购买成功";

		
		public const string BuyFailTitle = "竞拍失败";

		
		public const string BuyFailIntro = "您参与的活动奖励{0}，在竞价中被超过,返还您{1}钻石";

		
		private static Dictionary<int, AuctionConfig> AuctionDict = new Dictionary<int, AuctionConfig>();

		
		private static List<AuctionAwardConfig> AuctionAwardList = new List<AuctionAwardConfig>();
	}
}
