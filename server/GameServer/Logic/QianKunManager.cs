using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew.SevenDay;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000780 RID: 1920
	public class QianKunManager
	{
		// Token: 0x0600314C RID: 12620 RVA: 0x002C112C File Offset: 0x002BF32C
		public static void LoadImpetrateItemsInfo()
		{
			lock (QianKunManager.m_mutex)
			{
				QianKunManager.m_ImpetrateDataInfo = null;
				QianKunManager.m_ImpetrateDataInfo = new Dictionary<int, Dictionary<int, SystemXmlItem>>();
				string fileName = "";
				XElement xml = null;
				try
				{
					fileName = string.Format("Config/NewDig.xml", new object[0]);
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
				IEnumerable<XElement> jiNengXmlItems = xml.Elements("Type");
				foreach (XElement jiNengItem in jiNengXmlItems)
				{
					int nType = (int)Global.GetSafeAttributeLong(jiNengItem, "TypeID");
					Dictionary<int, SystemXmlItem> dicTmp = new Dictionary<int, SystemXmlItem>();
					IEnumerable<XElement> items = jiNengItem.Elements("Item");
					foreach (XElement item in items)
					{
						SystemXmlItem systemXmlItem = new SystemXmlItem
						{
							XMLNode = item
						};
						int nKey = (int)Global.GetSafeAttributeLong(item, "ID");
						dicTmp[nKey] = systemXmlItem;
					}
					QianKunManager.m_ImpetrateDataInfo.Add(nType, dicTmp);
				}
			}
		}

		// Token: 0x0600314D RID: 12621 RVA: 0x002C1328 File Offset: 0x002BF528
		public static void LoadImpetrateItemsInfoFree()
		{
			lock (QianKunManager.m_mutex)
			{
				QianKunManager.m_ImpetrateDataInfoFree = null;
				QianKunManager.m_ImpetrateDataInfoFree = new Dictionary<int, Dictionary<int, SystemXmlItem>>();
				string fileName = "";
				XElement xml = null;
				try
				{
					fileName = string.Format("Config/FreeNewDig.xml", new object[0]);
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
				IEnumerable<XElement> jiNengXmlItems = xml.Elements("Type");
				foreach (XElement jiNengItem in jiNengXmlItems)
				{
					int nType = (int)Global.GetSafeAttributeLong(jiNengItem, "TypeID");
					Dictionary<int, SystemXmlItem> dicTmp = new Dictionary<int, SystemXmlItem>();
					IEnumerable<XElement> items = jiNengItem.Elements("Item");
					foreach (XElement item in items)
					{
						SystemXmlItem systemXmlItem = new SystemXmlItem
						{
							XMLNode = item
						};
						int nKey = (int)Global.GetSafeAttributeLong(item, "ID");
						dicTmp[nKey] = systemXmlItem;
					}
					QianKunManager.m_ImpetrateDataInfoFree.Add(nType, dicTmp);
				}
			}
		}

		// Token: 0x0600314E RID: 12622 RVA: 0x002C1524 File Offset: 0x002BF724
		public static void LoadImpetrateItemsInfoTeQuan()
		{
			lock (QianKunManager.m_mutex)
			{
				QianKunManager.m_ImpetrateDataTeQuan = null;
				QianKunManager.m_ImpetrateDataTeQuan = new Dictionary<int, Dictionary<int, SystemXmlItem>>();
				string fileName = "";
				XElement xml = null;
				try
				{
					fileName = string.Format("Config/TeQuanNewDig.xml", new object[0]);
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
				IEnumerable<XElement> jiNengXmlItems = xml.Elements("Type");
				foreach (XElement jiNengItem in jiNengXmlItems)
				{
					int nType = (int)Global.GetSafeAttributeLong(jiNengItem, "TypeID");
					Dictionary<int, SystemXmlItem> dicTmp = new Dictionary<int, SystemXmlItem>();
					IEnumerable<XElement> items = jiNengItem.Elements("Item");
					foreach (XElement item in items)
					{
						SystemXmlItem systemXmlItem = new SystemXmlItem
						{
							XMLNode = item
						};
						int nKey = (int)Global.GetSafeAttributeLong(item, "ID");
						dicTmp[nKey] = systemXmlItem;
					}
					QianKunManager.m_ImpetrateDataTeQuan.Add(nType, dicTmp);
				}
			}
		}

		// Token: 0x0600314F RID: 12623 RVA: 0x002C1720 File Offset: 0x002BF920
		public static void LoadImpetrateItemsInfoFreeTeQuan()
		{
			lock (QianKunManager.m_mutex)
			{
				QianKunManager.m_ImpetrateDataTeQuanFree = null;
				QianKunManager.m_ImpetrateDataTeQuanFree = new Dictionary<int, Dictionary<int, SystemXmlItem>>();
				string fileName = "";
				XElement xml = null;
				try
				{
					fileName = string.Format("Config/TeQuanFreeNewDig.xml", new object[0]);
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
				IEnumerable<XElement> jiNengXmlItems = xml.Elements("Type");
				foreach (XElement jiNengItem in jiNengXmlItems)
				{
					int nType = (int)Global.GetSafeAttributeLong(jiNengItem, "TypeID");
					Dictionary<int, SystemXmlItem> dicTmp = new Dictionary<int, SystemXmlItem>();
					IEnumerable<XElement> items = jiNengItem.Elements("Item");
					foreach (XElement item in items)
					{
						SystemXmlItem systemXmlItem = new SystemXmlItem
						{
							XMLNode = item
						};
						int nKey = (int)Global.GetSafeAttributeLong(item, "ID");
						dicTmp[nKey] = systemXmlItem;
					}
					QianKunManager.m_ImpetrateDataTeQuanFree.Add(nType, dicTmp);
				}
			}
		}

		// Token: 0x06003150 RID: 12624 RVA: 0x002C191C File Offset: 0x002BFB1C
		public static void LoadImpetrateItemsInfoHuodong()
		{
			lock (QianKunManager.m_mutex)
			{
				QianKunManager.m_ImpetrateDataHuoDong = null;
				QianKunManager.m_ImpetrateDataHuoDong = new Dictionary<int, Dictionary<int, SystemXmlItem>>();
				string fileName = "";
				XElement xml = null;
				try
				{
					fileName = string.Format("Config/HuoDongNewDig.xml", new object[0]);
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
				IEnumerable<XElement> jiNengXmlItems = xml.Elements("Type");
				foreach (XElement jiNengItem in jiNengXmlItems)
				{
					int nType = (int)Global.GetSafeAttributeLong(jiNengItem, "TypeID");
					Dictionary<int, SystemXmlItem> dicTmp = new Dictionary<int, SystemXmlItem>();
					IEnumerable<XElement> items = jiNengItem.Elements("Item");
					foreach (XElement item in items)
					{
						SystemXmlItem systemXmlItem = new SystemXmlItem
						{
							XMLNode = item
						};
						int nKey = (int)Global.GetSafeAttributeLong(item, "ID");
						dicTmp[nKey] = systemXmlItem;
					}
					QianKunManager.m_ImpetrateDataHuoDong.Add(nType, dicTmp);
				}
			}
		}

		// Token: 0x06003151 RID: 12625 RVA: 0x002C1B18 File Offset: 0x002BFD18
		public static void LoadImpetrateItemsInfoFreeHuoDong()
		{
			lock (QianKunManager.m_mutex)
			{
				QianKunManager.m_ImpetrateDataHuoDongFree = null;
				QianKunManager.m_ImpetrateDataHuoDongFree = new Dictionary<int, Dictionary<int, SystemXmlItem>>();
				string fileName = "";
				XElement xml = null;
				try
				{
					fileName = string.Format("Config/HuoDongFreeNewDig.xml", new object[0]);
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
				IEnumerable<XElement> jiNengXmlItems = xml.Elements("Type");
				foreach (XElement jiNengItem in jiNengXmlItems)
				{
					int nType = (int)Global.GetSafeAttributeLong(jiNengItem, "TypeID");
					Dictionary<int, SystemXmlItem> dicTmp = new Dictionary<int, SystemXmlItem>();
					IEnumerable<XElement> items = jiNengItem.Elements("Item");
					foreach (XElement item in items)
					{
						SystemXmlItem systemXmlItem = new SystemXmlItem
						{
							XMLNode = item
						};
						int nKey = (int)Global.GetSafeAttributeLong(item, "ID");
						dicTmp[nKey] = systemXmlItem;
					}
					QianKunManager.m_ImpetrateDataHuoDongFree.Add(nType, dicTmp);
				}
			}
		}

		// Token: 0x06003152 RID: 12626 RVA: 0x002C1D14 File Offset: 0x002BFF14
		public static void ProcessRandomWaBao(GameClient client, int binding, Dictionary<int, SystemXmlItem> SystemXmlItemDict, int nType)
		{
			int randomNum = Global.GetRandomNumber(1, 10001);
			List<SystemXmlItem> itemList = new List<SystemXmlItem>();
			foreach (SystemXmlItem systemWaBaoItem in SystemXmlItemDict.Values)
			{
				if (randomNum >= systemWaBaoItem.GetIntValue("StartValues", -1) && randomNum <= systemWaBaoItem.GetIntValue("EndValues", -1))
				{
					itemList.Add(systemWaBaoItem);
				}
			}
			if (itemList.Count > 0)
			{
				List<string> mstTextList = new List<string>();
				int index = Global.GetRandomNumber(0, itemList.Count);
				SystemXmlItem waBaoItem = itemList[index];
				int goodsID = waBaoItem.GetIntValue("GoodsID", -1);
				if (goodsID > 0)
				{
					if (Global.CanAddGoods(client, goodsID, 1, binding, "1900-01-01 12:00:00", true, false))
					{
						int dbRet = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsID, 1, 0, "", 0, binding, 0, "", true, 1, "乾坤袋挖宝获取道具", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
						if (dbRet < 0)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("使用乾坤袋挖宝时背包满，放入物品时错误, RoleID={0}, GoodsID={1}, Binding={2}, Ret={3}", new object[]
							{
								client.ClientData.RoleID,
								goodsID,
								binding,
								dbRet
							}), null, true);
						}
						else
						{
							Global.BroadcastQianKunDaiGoodsHint(client, goodsID, nType);
							string msgText = string.Format(GLang.GetLang(519, new object[0]), Global.GetGoodsNameByID(goodsID));
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
						}
					}
					else
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("使用乾坤袋挖宝时背包满，无法放入物品, RoleID={0}, GoodsID={1}, Binding={2}", client.ClientData.RoleID, goodsID, binding), null, true);
					}
				}
				int minMoney = waBaoItem.GetIntValue("MinMoney", -1);
				int maxMoney = waBaoItem.GetIntValue("MaxMoney", -1);
				if (minMoney >= 0 && maxMoney > minMoney)
				{
					int giveMoney = Global.GetRandomNumber(minMoney, maxMoney);
					if (giveMoney > 0)
					{
						GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, giveMoney, "开启乾坤袋一", false);
						string msgText = string.Format(GLang.GetLang(520, new object[0]), giveMoney);
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
					}
				}
				int minBindYuanBao = waBaoItem.GetIntValue("MinBindYuanBao", -1);
				int maxBindYuanBao = waBaoItem.GetIntValue("MaxBindYuanBao", -1);
				if (minBindYuanBao >= 0 && maxBindYuanBao > minBindYuanBao)
				{
					int giveBingYuanBao = Global.GetRandomNumber(minBindYuanBao, maxBindYuanBao);
					if (giveBingYuanBao > 0)
					{
						GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, giveBingYuanBao, "开启乾坤袋");
						string msgText = string.Format(GLang.GetLang(521, new object[0]), giveBingYuanBao);
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
					}
				}
				int minExp = waBaoItem.GetIntValue("MinExp", -1);
				int maxExp = waBaoItem.GetIntValue("MaxExp", -1);
				if (minExp >= 0 && maxExp > minExp)
				{
					int giveExp = Global.GetRandomNumber(minExp, maxExp);
					if (giveExp > 0)
					{
						GameManager.ClientMgr.ProcessRoleExperience(client, (long)giveExp, false, true, false, "none");
						string msgText = string.Format(GLang.GetLang(522, new object[0]), giveExp);
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
					}
				}
			}
		}

		// Token: 0x06003153 RID: 12627 RVA: 0x002C2188 File Offset: 0x002C0388
		public static string ProcessRandomWaBaoByZaDan(GameClient client, Dictionary<int, SystemXmlItem> SystemXmlItemDic, int nType, out string strRecord, int binding = 0, bool bMuProject = false)
		{
			strRecord = null;
			int gainGoodsID = 0;
			int gainGoodsNum = 0;
			int gainGold = 0;
			int gainYinLiang = 0;
			int gainExp = 0;
			int randomNum = Global.GetRandomNumber(1, 100001);
			List<SystemXmlItem> itemList = new List<SystemXmlItem>();
			foreach (SystemXmlItem systemWaBaoItem in SystemXmlItemDic.Values)
			{
				if (randomNum >= systemWaBaoItem.GetIntValue("StartValues", -1) && randomNum <= systemWaBaoItem.GetIntValue("EndValues", -1))
				{
					itemList.Add(systemWaBaoItem);
					break;
				}
			}
			string result;
			if (itemList.Count <= 0)
			{
				result = "";
			}
			else
			{
				List<string> mstTextList = new List<string>();
				int index = Global.GetRandomNumber(0, itemList.Count);
				SystemXmlItem waBaoItem = itemList[index];
				int nGoodsLevel = 0;
				int nAppendProp = 0;
				int nLuckyProp = 0;
				int nExcellenceProp = 0;
				int nGoodCount = waBaoItem.GetIntValue("Num", -1);
				int goodsID = waBaoItem.GetIntValue("GoodsID", -1);
				if (goodsID > 0)
				{
					if (Global.CanAddGoodsToJinDanCangKu(client, goodsID, 1, binding, "1900-01-01 12:00:00", true))
					{
						int nForgeFallId = waBaoItem.GetIntValue("QiangHuaFallID", -1);
						if (nForgeFallId != -1)
						{
							nGoodsLevel = GameManager.GoodsPackMgr.GetFallGoodsLevel(nForgeFallId);
						}
						int nAppendPropFallId = waBaoItem.GetIntValue("ZhuiJiaFallID", -1);
						if (nAppendPropFallId != -1)
						{
							nAppendProp = GameManager.GoodsPackMgr.GetZhuiJiaGoodsLevelID(nAppendPropFallId);
						}
						int nLuckyPropFallId = waBaoItem.GetIntValue("LckyProbability", -1);
						if (nLuckyPropFallId != -1)
						{
							int nValue = GameManager.GoodsPackMgr.GetLuckyGoodsID(nLuckyPropFallId);
							if (nValue >= 1)
							{
								nLuckyProp = 1;
							}
						}
						int nExcellencePropFallId = waBaoItem.GetIntValue("ZhuoYueFallID", -1);
						if (nExcellencePropFallId != -1)
						{
							nExcellenceProp = GameManager.GoodsPackMgr.GetExcellencePropertysID(goodsID, nExcellencePropFallId);
						}
						int dbRet = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsID, nGoodCount, 0, "", nGoodsLevel, binding, 2000, "", true, 1, "乾坤袋挖宝获取道具", "1900-01-01 12:00:00", 0, 0, nLuckyProp, 0, nExcellenceProp, nAppendProp, 0, null, null, 0, true);
						if (dbRet < 0)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("使用乾坤袋挖宝时背包满，放入物品时错误, RoleID={0}, GoodsID={1}, Binding={2}, Ret={3}", new object[]
							{
								client.ClientData.RoleID,
								goodsID,
								binding,
								dbRet
							}), null, true);
						}
						else
						{
							Global.BroadcastQianKunDaiGoodsHint(client, goodsID, nType);
							gainGoodsID = goodsID;
							gainGoodsNum = 1;
							SevenDayGoalEventObject evObj = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.GetEquipCountByQiFu);
							evObj.Arg1 = goodsID;
							evObj.Arg2 = gainGoodsNum;
							GlobalEventSource.getInstance().fireEvent(evObj);
						}
					}
					else
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("使用乾坤袋挖宝时背包满，无法放入物品, RoleID={0}, GoodsID={1}, Binding={2}", client.ClientData.RoleID, goodsID, binding), null, true);
					}
				}
				int minMoney = waBaoItem.GetIntValue("MinMoney", -1);
				int maxMoney = waBaoItem.GetIntValue("MaxMoney", -1);
				if (minMoney >= 0 && maxMoney > minMoney)
				{
					int giveMoney = Global.GetRandomNumber(minMoney, maxMoney);
					if (giveMoney > 0)
					{
						GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, giveMoney, "开启乾坤袋二", false);
						gainYinLiang = giveMoney;
					}
				}
				int minBindYuanBao = waBaoItem.GetIntValue("MinBindYuanBao", -1);
				int maxBindYuanBao = waBaoItem.GetIntValue("MaxBindYuanBao", -1);
				if (minBindYuanBao >= 0 && maxBindYuanBao > minBindYuanBao)
				{
					int giveBingYuanBao = Global.GetRandomNumber(minBindYuanBao, maxBindYuanBao);
					if (giveBingYuanBao > 0)
					{
						GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, giveBingYuanBao, "开启乾坤袋二");
						gainGold = giveBingYuanBao;
					}
				}
				int minExp = waBaoItem.GetIntValue("MinExp", -1);
				int maxExp = waBaoItem.GetIntValue("MaxExp", -1);
				if (minExp >= 0 && maxExp > minExp)
				{
					int giveExp = Global.GetRandomNumber(minExp, maxExp);
					if (giveExp > 0)
					{
						GameManager.ClientMgr.ProcessRoleExperience(client, (long)giveExp, false, true, false, "none");
						gainExp = giveExp;
					}
				}
				string strProp = string.Format("{0}|{1}|{2}|{3}", new object[]
				{
					nGoodsLevel,
					nAppendProp,
					nLuckyProp,
					nExcellenceProp
				});
				string sResult;
				if (bMuProject)
				{
					sResult = string.Format("{0},{1},{2},{3},{4},{5},{6}", new object[]
					{
						gainGoodsID,
						nGoodCount,
						binding,
						nGoodsLevel,
						nAppendProp,
						nLuckyProp,
						nExcellenceProp
					});
				}
				else
				{
					sResult = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", new object[]
					{
						gainGoodsID,
						gainGoodsNum,
						gainGold,
						gainYinLiang,
						gainExp,
						strProp
					});
				}
				strRecord = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", new object[]
				{
					gainGoodsID,
					gainGoodsNum,
					gainGold,
					gainYinLiang,
					gainExp,
					strProp
				});
				if (gainGoodsID > 0)
				{
					EventLogManager.AddRoleQiFuEvent(client, "【{0}】在祈福抽取中获得了【{1}】", new object[]
					{
						client.ClientData.RoleName,
						Global.GetGoodsLogName(new GoodsData
						{
							GoodsID = gainGoodsID,
							ExcellenceInfo = nExcellenceProp
						})
					});
				}
				result = sResult;
			}
			return result;
		}

		// Token: 0x06003154 RID: 12628 RVA: 0x002C27F4 File Offset: 0x002C09F4
		public static string ProcessRandomWaBaoByZaDanSP(GameClient client, Dictionary<int, SystemXmlItem> SystemXmlItemDic, int nType, out string strRecord, int binding = 0, bool bMuProject = false)
		{
			strRecord = null;
			int gainGoodsID = 0;
			int gainGoodsNum = 0;
			int gainGold = 0;
			int gainYinLiang = 0;
			int gainExp = 0;
			int nGoodsLevel = 0;
			int nAppendProp = 0;
			int nLuckyProp = 0;
			int nExcellenceProp = 0;
			int[] goodsInfo = Global.GetRandomGoods(GameManager.systemParamsList.GetParamValueByName("QiFuTen"));
			int goodsID = goodsInfo[0];
			int nGoodCount = goodsInfo[1];
			if (Global.CanAddGoodsToJinDanCangKu(client, goodsID, 1, binding, "1900-01-01 12:00:00", true))
			{
				int nForgeFallId = goodsInfo[3];
				nGoodsLevel = GameManager.GoodsPackMgr.GetFallGoodsLevel(nForgeFallId);
				int nAppendPropFallId = goodsInfo[4];
				nAppendProp = GameManager.GoodsPackMgr.GetZhuiJiaGoodsLevelID(nAppendPropFallId);
				int nLuckyPropFallId = goodsInfo[5];
				nLuckyProp = GameManager.GoodsPackMgr.GetLuckyGoodsID(nLuckyPropFallId);
				int nExcellencePropFallId = goodsInfo[6];
				nExcellenceProp = GameManager.GoodsPackMgr.GetExcellencePropertysID(goodsID, nExcellencePropFallId);
				int dbRet = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsID, nGoodCount, 0, "", nGoodsLevel, binding, 2000, "", true, 1, "乾坤袋挖宝获取道具", "1900-01-01 12:00:00", 0, 0, nLuckyProp, 0, nExcellenceProp, nAppendProp, 0, null, null, 0, true);
				if (dbRet < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("使用乾坤袋挖宝时背包满，放入物品时错误, RoleID={0}, GoodsID={1}, Binding={2}, Ret={3}", new object[]
					{
						client.ClientData.RoleID,
						goodsID,
						binding,
						dbRet
					}), null, true);
				}
				else
				{
					Global.BroadcastQianKunDaiGoodsHint(client, goodsID, nType);
					gainGoodsID = goodsID;
					gainGoodsNum = 1;
					SevenDayGoalEventObject evObj = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.GetEquipCountByQiFu);
					evObj.Arg1 = goodsID;
					evObj.Arg2 = gainGoodsNum;
					GlobalEventSource.getInstance().fireEvent(evObj);
				}
			}
			else
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("使用乾坤袋挖宝时背包满，无法放入物品, RoleID={0}, GoodsID={1}, Binding={2}", client.ClientData.RoleID, goodsID, binding), null, true);
			}
			string strProp = string.Format("{0}|{1}|{2}|{3}", new object[]
			{
				nGoodsLevel,
				nAppendProp,
				nLuckyProp,
				nExcellenceProp
			});
			string sResult;
			if (bMuProject)
			{
				sResult = string.Format("{0},{1},{2},{3},{4},{5},{6}", new object[]
				{
					gainGoodsID,
					nGoodCount,
					binding,
					nGoodsLevel,
					nAppendProp,
					nLuckyProp,
					nExcellenceProp
				});
			}
			else
			{
				sResult = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", new object[]
				{
					gainGoodsID,
					gainGoodsNum,
					gainGold,
					gainYinLiang,
					gainExp,
					strProp
				});
			}
			strRecord = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", new object[]
			{
				gainGoodsID,
				gainGoodsNum,
				gainGold,
				gainYinLiang,
				gainExp,
				strProp
			});
			if (gainGoodsID > 0)
			{
				EventLogManager.AddRoleQiFuEvent(client, "【{0}】在祈福抽取中获得了【{1}】", new object[]
				{
					client.ClientData.RoleName,
					Global.GetGoodsLogName(new GoodsData
					{
						GoodsID = gainGoodsID,
						ExcellenceInfo = nExcellenceProp
					})
				});
			}
			return sResult;
		}

		// Token: 0x04003DAC RID: 15788
		public static Dictionary<int, Dictionary<int, SystemXmlItem>> m_ImpetrateDataInfo = null;

		// Token: 0x04003DAD RID: 15789
		public static Dictionary<int, Dictionary<int, SystemXmlItem>> m_ImpetrateDataInfoFree = null;

		// Token: 0x04003DAE RID: 15790
		public static Dictionary<int, Dictionary<int, SystemXmlItem>> m_ImpetrateDataHuoDong = null;

		// Token: 0x04003DAF RID: 15791
		public static Dictionary<int, Dictionary<int, SystemXmlItem>> m_ImpetrateDataHuoDongFree = null;

		// Token: 0x04003DB0 RID: 15792
		public static Dictionary<int, Dictionary<int, SystemXmlItem>> m_ImpetrateDataTeQuan = null;

		// Token: 0x04003DB1 RID: 15793
		public static Dictionary<int, Dictionary<int, SystemXmlItem>> m_ImpetrateDataTeQuanFree = null;

		// Token: 0x04003DB2 RID: 15794
		public static object m_mutex = new object();
	}
}
