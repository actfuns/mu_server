using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Tools;
using Server.Tools;

namespace GameServer.Logic.GoldAuction
{
	// Token: 0x020000A7 RID: 167
	public class GoldAuctionConfigModel
	{
		// Token: 0x0600028D RID: 653 RVA: 0x0002B914 File Offset: 0x00029B14
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

		// Token: 0x0600028E RID: 654 RVA: 0x0002B9EC File Offset: 0x00029BEC
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

		// Token: 0x0600028F RID: 655 RVA: 0x0002BA7C File Offset: 0x00029C7C
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

		// Token: 0x06000290 RID: 656 RVA: 0x0002BBD0 File Offset: 0x00029DD0
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

		// Token: 0x06000291 RID: 657 RVA: 0x0002BDE0 File Offset: 0x00029FE0
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

		// Token: 0x040003E5 RID: 997
		private const string Auction = "Config/Auction.xml";

		// Token: 0x040003E6 RID: 998
		private const string AngelTempleAuctionAward = "Config/AngelTempleAuctionAward.xml";

		// Token: 0x040003E7 RID: 999
		public const string AngelTempleAuction = "AngelTempleAuction";

		// Token: 0x040003E8 RID: 1000
		public const string AngelTempleAuctionMin = "AngelTempleAuctionMin";

		// Token: 0x040003E9 RID: 1001
		public const string AuctionZhanMengOpen = "AuctionZhanMengOpen";

		// Token: 0x040003EA RID: 1002
		public const string BuyTitle = "购买成功";

		// Token: 0x040003EB RID: 1003
		public const string BuyIntro = "在金团拍卖购买成功";

		// Token: 0x040003EC RID: 1004
		public const string BuyFailTitle = "竞拍失败";

		// Token: 0x040003ED RID: 1005
		public const string BuyFailIntro = "您参与的活动奖励{0}，在竞价中被超过,返还您{1}钻石";

		// Token: 0x040003EE RID: 1006
		private static Dictionary<int, AuctionConfig> AuctionDict = new Dictionary<int, AuctionConfig>();

		// Token: 0x040003EF RID: 1007
		private static List<AuctionAwardConfig> AuctionAwardList = new List<AuctionAwardConfig>();
	}
}
