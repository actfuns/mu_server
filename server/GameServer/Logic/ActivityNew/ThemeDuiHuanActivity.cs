using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x02000446 RID: 1094
	public class ThemeDuiHuanActivity : Activity
	{
		// Token: 0x060013FC RID: 5116 RVA: 0x0013A508 File Offset: 0x00138708
		public List<int> GetIndexAll()
		{
			List<int> IndexList = new List<int>();
			foreach (KeyValuePair<int, ThemeDuiHuan> item in this.ThemeDuiHuanDict)
			{
				IndexList.Add(item.Key);
			}
			return IndexList;
		}

		// Token: 0x060013FD RID: 5117 RVA: 0x0013A578 File Offset: 0x00138778
		public ThemeDuiHuan GetAwardConfig(int id)
		{
			ThemeDuiHuan config = null;
			if (this.ThemeDuiHuanDict.ContainsKey(id))
			{
				config = this.ThemeDuiHuanDict[id];
			}
			return config;
		}

		// Token: 0x060013FE RID: 5118 RVA: 0x0013A5B0 File Offset: 0x001387B0
		public override bool GiveAward(GameClient client, int _params)
		{
			ThemeDuiHuan config = this.GetAwardConfig(_params);
			return null != config && base.GiveAward(client, config.MyAwardItem);
		}

		// Token: 0x060013FF RID: 5119 RVA: 0x0013A5E8 File Offset: 0x001387E8
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _params)
		{
			ThemeDuiHuan config = this.GetAwardConfig(_params);
			return null != config && null != config.MyAwardItem && (config.MyAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, config.MyAwardItem.GoodsDataList));
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x0013A654 File Offset: 0x00138854
		public int GetThemeDHTodayLeftMergeNum(GameClient client, int index)
		{
			ThemeDuiHuan config = this.GetAwardConfig(index);
			int result;
			if (null == config)
			{
				result = 0;
			}
			else
			{
				int currday = Global.GetOffsetDay(TimeUtil.NowDateTime());
				int lastday = 0;
				int count = 0;
				string strFlag = (Global.SafeConvertToInt32("50") + index - 1).ToString();
				string ThemeExchargeFlag = Global.GetRoleParamByName(client, strFlag);
				if (!string.IsNullOrEmpty(ThemeExchargeFlag))
				{
					string[] fields = ThemeExchargeFlag.Split(new char[]
					{
						','
					});
					if (2 == fields.Length)
					{
						lastday = Convert.ToInt32(fields[0]);
						count = Convert.ToInt32(fields[1]);
					}
				}
				if (currday == lastday)
				{
					result = config.DayMaxTimes - count;
				}
				else
				{
					result = config.DayMaxTimes;
				}
			}
			return result;
		}

		// Token: 0x06001401 RID: 5121 RVA: 0x0013A728 File Offset: 0x00138928
		public int ModifyThemeTodayLeftMergeNum(GameClient client, int index, int addNum = 1)
		{
			int currday = Global.GetOffsetDay(TimeUtil.NowDateTime());
			string strFlag = (Global.SafeConvertToInt32("50") + index - 1).ToString();
			string ThemeExchargeFlag = Global.GetRoleParamByName(client, strFlag);
			int lastday = 0;
			int count = 0;
			if (!string.IsNullOrEmpty(ThemeExchargeFlag))
			{
				string[] fields = ThemeExchargeFlag.Split(new char[]
				{
					','
				});
				if (2 == fields.Length)
				{
					lastday = Convert.ToInt32(fields[0]);
					count = Convert.ToInt32(fields[1]);
				}
			}
			if (currday == lastday)
			{
				count += addNum;
			}
			else
			{
				lastday = currday;
				count = addNum;
			}
			string result = string.Format("{0},{1}", lastday, count);
			Global.SaveRoleParamsStringToDB(client, strFlag, result, true);
			return count;
		}

		// Token: 0x06001402 RID: 5122 RVA: 0x0013A800 File Offset: 0x00138A00
		public string MergeGoods(GameClient client, int index)
		{
			string strcmd = string.Format("{0}:{1}:{2}", 0, client.ClientData.RoleID, 154);
			string result;
			if (this.GetThemeDHTodayLeftMergeNum(client, index) <= 0)
			{
				strcmd = string.Format("{0}:{1}:{2}", -20000, client.ClientData.RoleID, 154);
				result = strcmd;
			}
			else
			{
				ThemeDuiHuan config = this.GetAwardConfig(index);
				if (null == config)
				{
					strcmd = string.Format("{0}:{1}:{2}", -20001, client.ClientData.RoleID, 154);
					result = strcmd;
				}
				else if (null == config.MyAwardItem)
				{
					strcmd = string.Format("{0}:{1}:{2}", -20001, client.ClientData.RoleID, 154);
					result = strcmd;
				}
				else if (null == config.MyAwardItem.GoodsDataList)
				{
					strcmd = string.Format("{0}:{1}:{2}", -20001, client.ClientData.RoleID, 154);
					result = strcmd;
				}
				else
				{
					if (null != config.NeedGoodsList)
					{
						for (int i = 0; i < config.NeedGoodsList.Count; i++)
						{
							if (Global.GetTotalGoodsNotUsingCountByID(client, config.NeedGoodsList[i].GoodsID) < config.NeedGoodsList[i].GCount)
							{
								return string.Format("{0}:{1}:{2}", -20003, client.ClientData.RoleID, 154);
							}
						}
					}
					string castResList = "";
					if (null != config.NeedGoodsList)
					{
						for (int i = 0; i < config.NeedGoodsList.Count; i++)
						{
							bool usedBinding = false;
							bool usedTimeLimited = false;
							if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, config.NeedGoodsList[i].GoodsID, config.NeedGoodsList[i].GCount, false, out usedBinding, out usedTimeLimited, true))
							{
								return string.Format("{0}:{1}:{2}", -20004, client.ClientData.RoleID, 154);
							}
							castResList += EventLogManager.AddGoodsDataPropString(config.NeedGoodsList[i]);
						}
					}
					if (!this.GiveAward(client, index))
					{
						strcmd = string.Format("{0}:{1}:{2}", -20005, client.ClientData.RoleID, 154);
						result = strcmd;
					}
					else
					{
						if (castResList.Length > 0)
						{
							castResList = castResList.Remove(0, 1);
						}
						string strResList = EventLogManager.MakeGoodsDataPropString(config.MyAwardItem.GoodsDataList);
						EventLogManager.AddPurchaseEvent(client, 8, index, castResList, strResList);
						int leftNum = Math.Max(0, config.DayMaxTimes - this.ModifyThemeTodayLeftMergeNum(client, index, 1));
						strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							1,
							client.ClientData.RoleID,
							154,
							leftNum,
							index
						});
						result = strcmd;
					}
				}
			}
			return result;
		}

		// Token: 0x06001403 RID: 5123 RVA: 0x0013ABEC File Offset: 0x00138DEC
		public bool Init()
		{
			try
			{
				string fileName = "Config/ThemeActivityDuiHuan.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return false;
				}
				this.ActivityType = 154;
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						ThemeDuiHuan config = new ThemeDuiHuan();
						config.id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						config.DayMaxTimes = (int)Global.GetSafeAttributeLong(xmlItem, "DayMaxTimes");
						string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "DuiHuanGoodsIDs");
						if (!string.IsNullOrEmpty(goodsIDs))
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型主题服兑换活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								config.NeedGoodsList = HuodongCachingMgr.ParseGoodsDataList2(fields, "大型主题服兑换配置1");
							}
						}
						goodsIDs = Global.GetSafeAttributeStr(xmlItem, "NewGoodsID");
						if (string.IsNullOrEmpty(goodsIDs))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型主题服兑换活动配置文件中的合成物品配置项2失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型主题服兑换活动配置文件中的合成物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								config.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型主题服兑换合成配置2");
							}
						}
						this.ThemeDuiHuanDict[config.id] = config;
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/ThemeActivityDuiHuan.xml解析出现异常", ex, true);
				return false;
			}
			return true;
		}

		// Token: 0x04001D85 RID: 7557
		public Dictionary<int, ThemeDuiHuan> ThemeDuiHuanDict = new Dictionary<int, ThemeDuiHuan>();
	}
}
