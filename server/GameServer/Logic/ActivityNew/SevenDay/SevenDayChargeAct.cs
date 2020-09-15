using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.ActivityNew.SevenDay
{
	// Token: 0x020001A6 RID: 422
	public class SevenDayChargeAct
	{
		// Token: 0x060004F1 RID: 1265 RVA: 0x000439F4 File Offset: 0x00041BF4
		public void LoadConfig()
		{
			Dictionary<int, SevenDayChargeAct._DayAward> tmpDict = new Dictionary<int, SevenDayChargeAct._DayAward>();
			try
			{
				XElement xml = XElement.Load(Global.GameResPath("Config/SevenDay/SevenDayChongZhi.xml")).Element("GiftList");
				foreach (XElement xmlItem in xml.Elements())
				{
					int day = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					SevenDayChargeAct._DayAward award = new SevenDayChargeAct._DayAward();
					award.NeedCharge = (int)Global.GetSafeAttributeLong(xmlItem, "MinZhuanShi");
					string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
					if (!string.IsNullOrEmpty(goodsIDs))
					{
						string[] fields = goodsIDs.Split(new char[]
						{
							'|'
						});
						if (fields.Length <= 0)
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}活动配置文件中的物品配置项失败", "Config/SevenDay/SevenDayChongZhi.xml"), null, true);
						}
						else
						{
							award.AllAward.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "Config/SevenDay/SevenDayChongZhi.xml");
						}
					}
					goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
					if (!string.IsNullOrEmpty(goodsIDs))
					{
						string[] fields = goodsIDs.Split(new char[]
						{
							'|'
						});
						if (fields.Length <= 0)
						{
							LogManager.WriteLog(LogTypes.Warning, "Config/SevenDay/SevenDayChongZhi.xml", null, true);
						}
						else
						{
							award.OccAward.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "Config/SevenDay/SevenDayChongZhi.xml");
						}
					}
					tmpDict[day] = award;
				}
				lock (this.ConfigMutex)
				{
					this.DayAwardDict = tmpDict;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("七日登录活动加载配置失败{0}", "Config/SevenDay/SevenDayChongZhi.xml"), ex, true);
			}
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x00043C54 File Offset: 0x00041E54
		public ESevenDayActErrorCode HandleGetAward(GameClient client, int day)
		{
			int currDay;
			ESevenDayActErrorCode result;
			if (!SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client, out currDay))
			{
				result = ESevenDayActErrorCode.NotInActivityTime;
			}
			else if (day < 0 || day > currDay)
			{
				result = ESevenDayActErrorCode.VisitParamsWrong;
			}
			else
			{
				SevenDayChargeAct._DayAward dayAward = null;
				lock (this.ConfigMutex)
				{
					if (this.DayAwardDict == null || !this.DayAwardDict.TryGetValue(day, out dayAward))
					{
						return ESevenDayActErrorCode.ServerConfigError;
					}
				}
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Charge);
				if (itemDict == null)
				{
					result = ESevenDayActErrorCode.NotReachCondition;
				}
				else
				{
					lock (itemDict)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(day, out itemData))
						{
							result = ESevenDayActErrorCode.ServerConfigError;
						}
						else if (itemData.Params1 * GameManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 10) < dayAward.NeedCharge || itemData.AwardFlag != 0)
						{
							result = ESevenDayActErrorCode.NotReachCondition;
						}
						else
						{
							int awardGoodsCnt = 0;
							if (dayAward.AllAward != null && dayAward.AllAward.GoodsDataList != null)
							{
								awardGoodsCnt += dayAward.AllAward.GoodsDataList.Count;
							}
							if (dayAward.OccAward != null && dayAward.OccAward.GoodsDataList != null)
							{
								awardGoodsCnt += dayAward.OccAward.GoodsDataList.Count((GoodsData _goods) => Global.IsRoleOccupationMatchGoods(client, _goods.GoodsID));
							}
							if (awardGoodsCnt <= 0 || !Global.CanAddGoodsNum(client, awardGoodsCnt))
							{
								result = ESevenDayActErrorCode.NoBagSpace;
							}
							else
							{
								itemData.AwardFlag = 1;
								if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(client.ClientData.RoleID, ESevenDayActType.Charge, day, itemData, client.ServerId))
								{
									itemData.AwardFlag = 0;
									result = ESevenDayActErrorCode.DBFailed;
								}
								else
								{
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().GiveAward(client, dayAward.AllAward, ESevenDayActType.Charge) || !SingletonTemplate<SevenDayActivityMgr>.Instance().GiveAward(client, dayAward.OccAward, ESevenDayActType.Charge))
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("玩家领取七日充值奖励，设置领奖了但是发奖失败 roleid={0}, day={1}", client.ClientData.RoleID, day), null, true);
									}
									result = ESevenDayActErrorCode.Success;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x00043F44 File Offset: 0x00042144
		public void Update(GameClient client)
		{
			if (client != null)
			{
				int currDay;
				if (SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client, out currDay))
				{
					DateTime startDate = Global.GetRegTime(client.ClientData);
					startDate -= startDate.TimeOfDay;
					DateTime endDate = startDate.AddDays((double)(currDay - 1)).AddHours(23.0).AddMinutes(59.0).AddSeconds(59.0);
					StringBuilder sb = new StringBuilder();
					sb.Append(client.ClientData.RoleID);
					sb.Append(':').Append(startDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'));
					sb.Append(':').Append(endDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'));
					Dictionary<string, int> eachDayChargeDict = Global.sendToDB<Dictionary<string, int>, string>(13222, sb.ToString(), client.ServerId);
					if (eachDayChargeDict != null)
					{
						Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Charge);
						lock (itemDict)
						{
							for (int i = 0; i < 7; i++)
							{
								string szKey = startDate.AddDays((double)i).ToString("yyyy-MM-dd");
								int charge;
								if (eachDayChargeDict.TryGetValue(szKey, out charge))
								{
									SevenDayItemData itemData;
									if (!itemDict.TryGetValue(i + 1, out itemData) || itemData.Params1 != charge)
									{
										SevenDayItemData tmpData = new SevenDayItemData();
										tmpData.AwardFlag = ((itemData != null) ? itemData.AwardFlag : 0);
										tmpData.Params1 = charge;
										if (SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(client.ClientData.RoleID, ESevenDayActType.Charge, i + 1, tmpData, client.ServerId))
										{
											itemDict[i + 1] = tmpData;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x00044180 File Offset: 0x00042380
		public bool HasAnyAwardCanGet(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (!SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client))
			{
				result = false;
			}
			else
			{
				Dictionary<int, SevenDayChargeAct._DayAward> tmpDayAwardDict = null;
				lock (this.ConfigMutex)
				{
					tmpDayAwardDict = this.DayAwardDict;
				}
				if (tmpDayAwardDict == null)
				{
					result = false;
				}
				else
				{
					Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Charge);
					if (itemDict == null)
					{
						result = false;
					}
					else
					{
						lock (itemDict)
						{
							foreach (KeyValuePair<int, SevenDayItemData> kvp in itemDict)
							{
								int day = kvp.Key;
								SevenDayItemData itemData = kvp.Value;
								SevenDayChargeAct._DayAward award = null;
								if (tmpDayAwardDict.TryGetValue(day, out award) && itemData.Params1 * GameManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 10) >= award.NeedCharge && itemData.AwardFlag != 1)
								{
									return true;
								}
							}
						}
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x04000959 RID: 2393
		private Dictionary<int, SevenDayChargeAct._DayAward> DayAwardDict = null;

		// Token: 0x0400095A RID: 2394
		private object ConfigMutex = new object();

		// Token: 0x020001A7 RID: 423
		private class _DayAward
		{
			// Token: 0x0400095B RID: 2395
			public int NeedCharge;

			// Token: 0x0400095C RID: 2396
			public AwardItem AllAward = new AwardItem();

			// Token: 0x0400095D RID: 2397
			public AwardItem OccAward = new AwardItem();
		}
	}
}
