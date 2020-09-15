using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020007C2 RID: 1986
	public class WuXingMapMgr
	{
		// Token: 0x0600340D RID: 13325 RVA: 0x002E098C File Offset: 0x002DEB8C
		private static List<int> RandomIntList(List<int> list)
		{
			List<int> result;
			if (null == list)
			{
				result = null;
			}
			else
			{
				List<int> newList = new List<int>();
				foreach (int item in list)
				{
					int index = Global.GetRandomNumber(0, newList.Count);
					newList.Insert(index, item);
				}
				result = newList;
			}
			return result;
		}

		// Token: 0x0600340E RID: 13326 RVA: 0x002E0A10 File Offset: 0x002DEC10
		public static int GetNextMapCodeByNPCID(int mapCode, int npcID)
		{
			WuXingNPCItem wuXingNPCItem = null;
			string key = string.Format("{0}_{1}", mapCode, npcID);
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			lock (WuXingMapMgr.WuXingNPCDict)
			{
				if (!WuXingMapMgr.WuXingNPCDict.TryGetValue(key, out wuXingNPCItem))
				{
					return -1;
				}
				if (null == wuXingNPCItem.MapItem)
				{
					return -1;
				}
				if (dayID != wuXingNPCItem.MapItem.DayID)
				{
					wuXingNPCItem.MapItem.DayID = dayID;
					wuXingNPCItem.MapItem.GoToMapCodeList = WuXingMapMgr.RandomIntList(wuXingNPCItem.MapItem.GoToMapCodeList);
				}
				if (wuXingNPCItem.MapItem.GoToMapCodeList == null || wuXingNPCItem.MapItem.OtherNPCIDList == null || wuXingNPCItem.MapItem.GoToMapCodeList.Count != wuXingNPCItem.MapItem.OtherNPCIDList.Count)
				{
					return -1;
				}
				for (int i = 0; i < wuXingNPCItem.MapItem.OtherNPCIDList.Count; i++)
				{
					if (npcID == wuXingNPCItem.MapItem.OtherNPCIDList[i])
					{
						return wuXingNPCItem.MapItem.GoToMapCodeList[i];
					}
				}
			}
			return -1;
		}

		// Token: 0x0600340F RID: 13327 RVA: 0x002E0BAC File Offset: 0x002DEDAC
		public static int GetNeedGoodsIDByNPCID(int mapCode, int npcID)
		{
			WuXingNPCItem wuXingNPCItem = null;
			string key = string.Format("{0}_{1}", mapCode, npcID);
			int result;
			lock (WuXingMapMgr.WuXingNPCDict)
			{
				if (!WuXingMapMgr.WuXingNPCDict.TryGetValue(key, out wuXingNPCItem))
				{
					result = -1;
				}
				else
				{
					result = wuXingNPCItem.NeedGoodsID;
				}
			}
			return result;
		}

		// Token: 0x06003410 RID: 13328 RVA: 0x002E0C2C File Offset: 0x002DEE2C
		private static List<int> Str2IntArray(string str)
		{
			List<int> result;
			if (string.IsNullOrEmpty(str))
			{
				result = null;
			}
			else
			{
				List<int> intList = new List<int>();
				string[] fields = str.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < fields.Length; i++)
				{
					try
					{
						intList.Add(Convert.ToInt32(fields[i]));
					}
					catch (Exception)
					{
					}
				}
				result = intList;
			}
			return result;
		}

		// Token: 0x06003411 RID: 13329 RVA: 0x002E0CB0 File Offset: 0x002DEEB0
		private static WuXingMapItem ParseGlobalConfigItem(int globalID, string otherNPCIDs, string goToMaps)
		{
			WuXingMapItem wuXingMapItem = null;
			WuXingMapItem result;
			if (WuXingMapMgr.WuXingMapDict.TryGetValue(globalID, out wuXingMapItem))
			{
				result = wuXingMapItem;
			}
			else
			{
				wuXingMapItem = new WuXingMapItem
				{
					GlobalID = globalID,
					OtherNPCIDList = WuXingMapMgr.Str2IntArray(otherNPCIDs),
					GoToMapCodeList = WuXingMapMgr.Str2IntArray(goToMaps)
				};
				if (wuXingMapItem.OtherNPCIDList == null || wuXingMapItem.GoToMapCodeList == null || wuXingMapItem.OtherNPCIDList.Count != wuXingMapItem.GoToMapCodeList.Count)
				{
					throw new Exception(string.Format("解析五行奇阵配置文件时，解析NPC列表或者地图列表失败, GlobalID={0}", globalID));
				}
				WuXingMapMgr.WuXingMapDict[globalID] = wuXingMapItem;
				result = wuXingMapItem;
			}
			return result;
		}

		// Token: 0x06003412 RID: 13330 RVA: 0x002E0D5C File Offset: 0x002DEF5C
		private static void ParseWuXingXmlItem(SystemXmlItem systemXmlItem)
		{
			int npcID = systemXmlItem.GetIntValue("NPCID", -1);
			int mapCode = systemXmlItem.GetIntValue("MapCode", -1);
			int needGoodsID = systemXmlItem.GetIntValue("NeedGoodsID", -1);
			int globalID = systemXmlItem.GetIntValue("GlobalID", -1);
			string otherNPCIDs = systemXmlItem.GetStringValue("OtherNPCIDs");
			string goToMaps = systemXmlItem.GetStringValue("GoToMaps");
			WuXingMapItem wuXingMapItem = WuXingMapMgr.ParseGlobalConfigItem(globalID, otherNPCIDs, goToMaps);
			WuXingNPCItem wuXingNPCItem = new WuXingNPCItem
			{
				NPCID = npcID,
				MapCode = mapCode,
				NeedGoodsID = needGoodsID,
				MapItem = wuXingMapItem
			};
			string key = string.Format("{0}_{1}", mapCode, npcID);
			WuXingMapMgr.WuXingNPCDict[key] = wuXingNPCItem;
		}

		// Token: 0x06003413 RID: 13331 RVA: 0x002E0E18 File Offset: 0x002DF018
		public static void LoadXuXingConfig()
		{
			XElement xml = null;
			string fileName = "Config/WuXing.xml";
			try
			{
				xml = XElement.Load(Global.GameResPath(fileName));
				if (null == xml)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
			}
			IEnumerable<XElement> nodes = xml.Elements();
			foreach (XElement node in nodes)
			{
				SystemXmlItem systemXmlItem = new SystemXmlItem
				{
					XMLNode = node
				};
				WuXingMapMgr.ParseWuXingXmlItem(systemXmlItem);
			}
		}

		// Token: 0x06003414 RID: 13332 RVA: 0x002E0EEC File Offset: 0x002DF0EC
		private static List<GoodsData> ParseGoodsDataList(string[] fields)
		{
			List<GoodsData> goodsDataList = new List<GoodsData>();
			for (int i = 0; i < fields.Length; i++)
			{
				string[] sa = fields[i].Split(new char[]
				{
					','
				});
				if (sa.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析WuXingAwards.xml文件中的奖励项时失败, 物品配置项个数错误", new object[0]), null, true);
				}
				else
				{
					int[] goodsFields = Global.StringArray2IntArray(sa);
					GoodsData goodsData = Global.GetNewGoodsData(goodsFields[0], goodsFields[1], goodsFields[2], goodsFields[3], goodsFields[4], goodsFields[5], 0, 0, 0, 0, 0);
					goodsDataList.Add(goodsData);
				}
			}
			return goodsDataList;
		}

		// Token: 0x06003415 RID: 13333 RVA: 0x002E0F90 File Offset: 0x002DF190
		public static void ParseWuXingAwardItem(SystemXmlItem systemXmlItem)
		{
			List<GoodsData> goodsDataList = null;
			string goodsIDs = systemXmlItem.GetStringValue("GoodsIDs");
			if (!string.IsNullOrEmpty(goodsIDs))
			{
				string[] fields = goodsIDs.Split(new char[]
				{
					'|'
				});
				if (fields.Length > 0)
				{
					goodsDataList = WuXingMapMgr.ParseGoodsDataList(fields);
				}
				else
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析WuXingAwards.xml配置项中的物品奖励失败, MapCode={0}", systemXmlItem.GetIntValue("MapCode", -1)), null, true);
				}
			}
			else
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析WuXingAwards.xml配置项中的物品奖励失败, MapCode={0}", systemXmlItem.GetIntValue("MapCode", -1)), null, true);
			}
			WuXingMapMgr.TheWuXingMapAwardItem = new WuXingMapAwardItem
			{
				MapCode = systemXmlItem.GetIntValue("MapCode", -1),
				Money1 = systemXmlItem.GetIntValue("Moneyaward", -1),
				ExpXiShu = systemXmlItem.GetDoubleValue("ExpXiShu"),
				GoodsDataList = goodsDataList,
				MinBlessPoint = systemXmlItem.GetIntValue("MinBlessPoint", -1),
				MaxBlessPoint = systemXmlItem.GetIntValue("MaxBlessPoint", -1)
			};
		}

		// Token: 0x06003416 RID: 13334 RVA: 0x002E10A8 File Offset: 0x002DF2A8
		public static void LoadWuXingAward()
		{
			XElement xml = null;
			string fileName = "Config/WuXingAwards.xml";
			try
			{
				xml = XElement.Load(Global.GameResPath(fileName));
				if (null == xml)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
			}
			IEnumerable<XElement> nodes = xml.Elements();
			using (IEnumerator<XElement> enumerator = nodes.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					XElement node = enumerator.Current;
					SystemXmlItem systemXmlItem = new SystemXmlItem
					{
						XMLNode = node
					};
					WuXingMapMgr.ParseWuXingAwardItem(systemXmlItem);
				}
			}
			if (null == WuXingMapMgr.TheWuXingMapAwardItem)
			{
				throw new Exception(string.Format("加载五行奇阵的最顶层奖励项失败!", new object[0]));
			}
		}

		// Token: 0x06003417 RID: 13335 RVA: 0x002E11A4 File Offset: 0x002DF3A4
		public static bool CanGetWuXingAward(GameClient client)
		{
			int currentDayID = TimeUtil.NowDateTime().DayOfYear;
			int wuXingDayID = -1;
			int wuXingNum = 0;
			if (null != client.ClientData.MyRoleDailyData)
			{
				wuXingDayID = client.ClientData.MyRoleDailyData.WuXingDayID;
				wuXingNum = client.ClientData.MyRoleDailyData.WuXingNum;
			}
			return currentDayID != wuXingDayID || wuXingNum <= 0;
		}

		// Token: 0x06003418 RID: 13336 RVA: 0x002E1218 File Offset: 0x002DF418
		public static void ProcessWuXingAward(GameClient client)
		{
			if (WuXingMapMgr.CanGetWuXingAward(client))
			{
				if (null != WuXingMapMgr.TheWuXingMapAwardItem)
				{
					if (null != WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList)
					{
						if (!Global.CanAddGoodsDataList(client, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList))
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(574, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 1);
							return;
						}
					}
					int blessPoint = 0;
					if (WuXingMapMgr.TheWuXingMapAwardItem.MinBlessPoint >= 0 && WuXingMapMgr.TheWuXingMapAwardItem.MaxBlessPoint >= 0)
					{
						blessPoint = Global.GetRandomNumber(WuXingMapMgr.TheWuXingMapAwardItem.MinBlessPoint, WuXingMapMgr.TheWuXingMapAwardItem.MaxBlessPoint);
						if (blessPoint > 0)
						{
							if (client.ClientData.HorseDbID <= 0)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(575, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								return;
							}
						}
					}
					if (Global.FilterFallGoods(client))
					{
						if (null != WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList)
						{
							for (int i = 0; i < WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].GoodsID, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].GCount, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].Quality, "", WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].Forge_level, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].Binding, 0, "", true, 1, "五行奇阵奖励物品", "1900-01-01 12:00:00", WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].AddPropIndex, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].BornIndex, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].Lucky, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].Strong, 0, 0, 0, null, null, 0, true);
							}
						}
					}
					GameManager.ClientMgr.UpdateRoleDailyData_WuXingNum(client, 1);
					double expXiShu = WuXingMapMgr.TheWuXingMapAwardItem.ExpXiShu;
					int experience = (int)Math.Pow((double)client.ClientData.Level, expXiShu);
					if (DBRoleBufferManager.ProcessMonthVIP(client) > 0.0)
					{
						experience = (int)((double)experience * 1.5);
					}
					GameManager.ClientMgr.ProcessRoleExperience(client, (long)experience, true, false, false, "none");
					Global.BroadcastWuXingExperience(client, experience);
					int money = WuXingMapMgr.TheWuXingMapAwardItem.Money1;
					if (-1 != money)
					{
						money = Global.FilterValue(client, money);
						GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, money, "五行奇阵", false);
						GameManager.SystemServerEvents.AddEvent(string.Format("角色获取金钱, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
						{
							client.ClientData.RoleID,
							client.ClientData.RoleName,
							client.ClientData.Money1,
							money
						}), EventLevels.Record);
					}
					int currentHorseBlessPoint = ProcessHorse.GetCurrentHorseBlessPoint(client);
					if (currentHorseBlessPoint > 0 && blessPoint > 0)
					{
						double blessPointPercent = (double)blessPoint / 100.0;
						blessPoint = (int)(blessPointPercent * (double)currentHorseBlessPoint);
						blessPoint = Global.FilterValue(client, blessPoint);
						ProcessHorse.ProcessAddHorseAwardLucky(client, blessPoint, true, "五行奇阵奖励");
					}
					Global.AddWuXingAwardEvent(client, experience, blessPoint);
				}
			}
		}

		// Token: 0x04003FB0 RID: 16304
		private static Dictionary<int, WuXingMapItem> WuXingMapDict = new Dictionary<int, WuXingMapItem>();

		// Token: 0x04003FB1 RID: 16305
		private static Dictionary<string, WuXingNPCItem> WuXingNPCDict = new Dictionary<string, WuXingNPCItem>();

		// Token: 0x04003FB2 RID: 16306
		private static Dictionary<int, int> ClientsAwardsDict = new Dictionary<int, int>();

		// Token: 0x04003FB3 RID: 16307
		private static WuXingMapAwardItem TheWuXingMapAwardItem = null;
	}
}
