using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000059 RID: 89
	public class WebOldPlayerManager
	{
		// Token: 0x06000141 RID: 321 RVA: 0x000159DC File Offset: 0x00013BDC
		public static WebOldPlayerManager getInstance()
		{
			lock (WebOldPlayerManager.Mutex)
			{
				if (WebOldPlayerManager.instance != null)
				{
					return WebOldPlayerManager.instance;
				}
				WebOldPlayerManager.instance = new WebOldPlayerManager();
			}
			WebOldPlayerManager.LoadWebZhaoHuiXml();
			return WebOldPlayerManager.instance;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00015A50 File Offset: 0x00013C50
		public static void ReloadXml()
		{
			WebOldPlayerManager.LoadWebZhaoHuiXml();
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00015A5C File Offset: 0x00013C5C
		public static void LoadWebZhaoHuiXml()
		{
			try
			{
				string fileName = "Config/WebOldPlayer.xml";
				XElement xml = CheckHelper.LoadXml(Global.GameResPath(fileName), true);
				if (null != xml)
				{
					IEnumerable<XElement> nodes = xml.Elements();
					DateTime now = TimeUtil.NowDateTime();
					Dictionary<int, WebOldPlayerManager.WebZhaoHuiData> runTimeData = new Dictionary<int, WebOldPlayerManager.WebZhaoHuiData>();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							int awardID = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
							DateTime beginTime = DateTime.Parse(Global.GetDefAttributeStr(xmlItem, "BeginTime", ""));
							DateTime endTime = DateTime.Parse(Global.GetDefAttributeStr(xmlItem, "EndTime", ""));
							string goodsStr = Global.GetDefAttributeStr(xmlItem, "GoodsOne", "");
							List<GoodsData> goodsOne = new List<GoodsData>();
							List<GoodsData> goodsTwo = new List<GoodsData>();
							if (goodsStr != "")
							{
								string[] goodList = goodsStr.Split(new char[]
								{
									'|'
								});
								for (int i = 0; i < goodList.Length; i++)
								{
									if (!(goodList[i] == ""))
									{
										string[] goodItem = goodList[i].Split(new char[]
										{
											','
										});
										if (goodItem.Length == 7)
										{
											int[] goods = Global.StringArray2IntArray(goodItem);
											GoodsData goodsData = Global.GetNewGoodsData(goods[0], goods[1], 0, goods[3], goods[2], 0, goods[5], 0, goods[6], goods[4], 0);
											SystemXmlItem systemGoods = null;
											if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
											{
												LogManager.WriteLog(LogTypes.Error, string.Format("系统中不存在{0}", goodsData.GoodsID), null, true);
											}
											else
											{
												goodsOne.Add(goodsData);
											}
										}
									}
								}
							}
							goodsStr = Global.GetDefAttributeStr(xmlItem, "GoodsTwo", "");
							if (goodsStr != "")
							{
								string[] goodList = goodsStr.Split(new char[]
								{
									'|'
								});
								for (int i = 0; i < goodList.Length; i++)
								{
									if (!(goodList[i] == ""))
									{
										string[] goodItem = goodList[i].Split(new char[]
										{
											','
										});
										if (goodItem.Length == 7)
										{
											int[] goods = Global.StringArray2IntArray(goodItem);
											GoodsData goodsData = Global.GetNewGoodsData(goods[0], goods[1], 0, goods[3], goods[2], 0, goods[5], 0, goods[6], goods[4], 0);
											SystemXmlItem systemGoods = null;
											if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
											{
												LogManager.WriteLog(LogTypes.Error, string.Format("系统中不存在{0}", goodsData.GoodsID), null, true);
											}
											else
											{
												goodsTwo.Add(goodsData);
											}
										}
									}
								}
							}
							string mailTitle = Global.GetDefAttributeStr(xmlItem, "MaitTitle", "");
							string mailContent = Global.GetDefAttributeStr(xmlItem, "MailContent", "");
							WebOldPlayerManager.WebZhaoHuiData zhaoHuiData = new WebOldPlayerManager.WebZhaoHuiData
							{
								BegionTime = beginTime,
								EndTime = endTime,
								GoodsOne = goodsOne,
								GoodsTwo = goodsTwo,
								MailTitle = mailTitle,
								MailContent = mailContent
							};
							runTimeData[awardID] = zhaoHuiData;
						}
					}
					lock (WebOldPlayerManager.Mutex)
					{
						WebOldPlayerManager.RunTimeZhaoHuiData = runTimeData;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/WebZhaoHui.xml解析出现异常", ex, true);
			}
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00015EB8 File Offset: 0x000140B8
		public bool ChouJiangAddCheck(int roleID, int chouJiangType)
		{
			DateTime now = TimeUtil.NowDateTime();
			lock (WebOldPlayerManager.Mutex)
			{
				if (WebOldPlayerManager.RunTimeZhaoHuiData == null || WebOldPlayerManager.RunTimeZhaoHuiData.Count < 1 || now > WebOldPlayerManager.RunTimeZhaoHuiData[0].EndTime || now < WebOldPlayerManager.RunTimeZhaoHuiData[0].BegionTime)
				{
					return false;
				}
			}
			int check = Global.SafeConvertToInt32(Global.GetRoleParamsFromDBByRoleID(roleID, "10167", 0));
			bool result;
			if (check < 1)
			{
				result = false;
			}
			else
			{
				string cmd = string.Format("{0}:{1}:{2}", roleID, chouJiangType, now.Date.ToString().Replace(':', '$'));
				GameManager.DBCmdMgr.AddDBCmd(20305, cmd, null, GameCoreInterface.getinstance().GetLocalServerId());
				result = true;
			}
			return result;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00015FDC File Offset: 0x000141DC
		public void WebOldPlayerCheck(int roleID, int awardID)
		{
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				WebOldPlayerManager.WebZhaoHuiData zhaoHuiData = null;
				lock (WebOldPlayerManager.Mutex)
				{
					if (WebOldPlayerManager.RunTimeZhaoHuiData == null || WebOldPlayerManager.RunTimeZhaoHuiData.Count < 1)
					{
						return;
					}
					if (!WebOldPlayerManager.RunTimeZhaoHuiData.TryGetValue(awardID, out zhaoHuiData))
					{
						return;
					}
					if (now > zhaoHuiData.EndTime || now < zhaoHuiData.BegionTime)
					{
						return;
					}
				}
				GameClient client = GameManager.ClientMgr.FindClient(roleID);
				int check;
				if (null == client)
				{
					check = Global.SafeConvertToInt32(Global.GetRoleParamsFromDBByRoleID(roleID, "10167", 0));
					if (check < 1)
					{
						GameManager.DBCmdMgr.AddDBCmd(10100, string.Format("{0}:{1}:{2}", roleID, "10167", 1), null, GameCoreInterface.getinstance().GetLocalServerId());
					}
				}
				else
				{
					check = Global.GetRoleParamsInt32FromDB(client, "10167");
					if (check < 1)
					{
						Global.SaveRoleParamsInt32ValueToDB(client, "10167", 1, true);
					}
				}
				if (check < 1)
				{
					List<GoodsData> awardList = new List<GoodsData>();
					string mailTitle = "";
					string mailContent = "";
					lock (WebOldPlayerManager.Mutex)
					{
						foreach (GoodsData item in zhaoHuiData.GoodsOne)
						{
							awardList.Add(item);
						}
						foreach (GoodsData item in zhaoHuiData.GoodsTwo)
						{
							awardList.Add(item);
						}
						mailTitle = zhaoHuiData.MailTitle;
						mailContent = zhaoHuiData.MailContent;
					}
					Global.UseMailGivePlayerAward3(roleID, awardList, Global.GetLang(mailTitle), Global.GetLang(mailContent), 0, 0, 0);
				}
			}
			catch
			{
			}
		}

		// Token: 0x040001EB RID: 491
		public static Dictionary<int, WebOldPlayerManager.WebZhaoHuiData> RunTimeZhaoHuiData = new Dictionary<int, WebOldPlayerManager.WebZhaoHuiData>();

		// Token: 0x040001EC RID: 492
		private static object Mutex = new object();

		// Token: 0x040001ED RID: 493
		private static WebOldPlayerManager instance = null;

		// Token: 0x0200005A RID: 90
		public class WebZhaoHuiData
		{
			// Token: 0x040001EE RID: 494
			public DateTime BegionTime;

			// Token: 0x040001EF RID: 495
			public DateTime EndTime;

			// Token: 0x040001F0 RID: 496
			public List<GoodsData> GoodsOne;

			// Token: 0x040001F1 RID: 497
			public List<GoodsData> GoodsTwo;

			// Token: 0x040001F2 RID: 498
			public string MailTitle;

			// Token: 0x040001F3 RID: 499
			public string MailContent;
		}
	}
}
