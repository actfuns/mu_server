using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020006C5 RID: 1733
	public class FuBenManager
	{
		// Token: 0x060020DF RID: 8415 RVA: 0x001C2E94 File Offset: 0x001C1094
		public static int FindFuBenSeqIDByRoleID(int roleID)
		{
			int fuBenSeqID = 0;
			int result;
			lock (FuBenManager._FuBenSeqIDDict)
			{
				if (FuBenManager._FuBenSeqIDDict.TryGetValue(roleID, out fuBenSeqID))
				{
					result = fuBenSeqID;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x060020E0 RID: 8416 RVA: 0x001C2EFC File Offset: 0x001C10FC
		public static FuBenInfoItem FindFuBenInfoBySeqID(int fuBenSeqID)
		{
			FuBenInfoItem fuBenInfoItem = null;
			FuBenInfoItem result;
			lock (FuBenManager._FuBenSeqID2InfoDict)
			{
				if (!FuBenManager._FuBenSeqID2InfoDict.TryGetValue(fuBenSeqID, out fuBenInfoItem))
				{
					result = null;
				}
				else
				{
					result = fuBenInfoItem;
				}
			}
			return result;
		}

		// Token: 0x060020E1 RID: 8417 RVA: 0x001C2F60 File Offset: 0x001C1160
		public static void AddFuBenSeqID(int roleID, int fuBenSeqID, int goodsBinding, int fuBenID)
		{
			lock (FuBenManager._FuBenSeqIDDict)
			{
				FuBenManager._FuBenSeqIDDict[roleID] = fuBenSeqID;
			}
			lock (FuBenManager._FuBenSeqID2InfoDict)
			{
				if (!FuBenManager._FuBenSeqID2InfoDict.ContainsKey(fuBenSeqID))
				{
					FuBenManager._FuBenSeqID2InfoDict[fuBenSeqID] = new FuBenInfoItem
					{
						FuBenSeqID = fuBenSeqID,
						StartTicks = TimeUtil.NOW(),
						EndTicks = 0L,
						GoodsBinding = goodsBinding,
						FuBenID = fuBenID
					};
				}
			}
		}

		// Token: 0x060020E2 RID: 8418 RVA: 0x001C303C File Offset: 0x001C123C
		public static void RemoveFuBenSeqID(int roleID)
		{
			int fuBenSeqID = -1;
			lock (FuBenManager._FuBenSeqIDDict)
			{
				if (FuBenManager._FuBenSeqIDDict.TryGetValue(roleID, out fuBenSeqID))
				{
					FuBenManager._FuBenSeqIDDict.Remove(roleID);
				}
				else
				{
					fuBenSeqID = -1;
				}
			}
		}

		// Token: 0x060020E3 RID: 8419 RVA: 0x001C30AC File Offset: 0x001C12AC
		public static void RemoveFuBenInfoBySeqID(int fuBenSeqID)
		{
			if (fuBenSeqID != -1)
			{
				lock (FuBenManager._FuBenSeqID2InfoDict)
				{
					FuBenManager._FuBenSeqID2InfoDict.Remove(fuBenSeqID);
				}
			}
		}

		// Token: 0x060020E4 RID: 8420 RVA: 0x001C3108 File Offset: 0x001C1308
		public static int GetFuBenSeqId(int tag = 0)
		{
			string[] dbFields = Global.ExecuteDBCmd(10049, string.Format("{0}", tag), 0);
			if (dbFields != null && dbFields.Length >= 2)
			{
				int nSeqID = Global.SafeConvertToInt32(dbFields[1]);
				if (nSeqID > 0)
				{
					return nSeqID;
				}
			}
			return 0;
		}

		// Token: 0x060020E5 RID: 8421 RVA: 0x001C3164 File Offset: 0x001C1364
		public static FuBenMapItem FindMapCodeByFuBenID(int fuBenID, int mapCode)
		{
			FuBenMapItem fuBenMapItem = null;
			string key = string.Format("{0}_{1}", fuBenID, mapCode);
			FuBenMapItem result;
			if (!FuBenManager._FuBenMapCode2MapItemDict.TryGetValue(key, out fuBenMapItem))
			{
				result = null;
			}
			else
			{
				result = fuBenMapItem;
			}
			return result;
		}

		// Token: 0x060020E6 RID: 8422 RVA: 0x001C31A8 File Offset: 0x001C13A8
		public static List<FuBenMapItem> GetAllFubenMapItem()
		{
			List<FuBenMapItem> list = new List<FuBenMapItem>();
			lock (FuBenManager._FuBenMapCode2MapItemDict)
			{
				list.AddRange(FuBenManager._FuBenMapCode2MapItemDict.Values);
			}
			return list;
		}

		// Token: 0x060020E7 RID: 8423 RVA: 0x001C320C File Offset: 0x001C140C
		public static List<int> FindMapCodeListByFuBenID(int fuBenID)
		{
			List<int> mapCodeList = null;
			List<int> result;
			if (!FuBenManager._FuBen2MapCodeListDict.TryGetValue(fuBenID, out mapCodeList))
			{
				result = null;
			}
			else
			{
				result = mapCodeList;
			}
			return result;
		}

		// Token: 0x060020E8 RID: 8424 RVA: 0x001C3238 File Offset: 0x001C1438
		public static int FindFuBenIDByMapCode(int mapCode)
		{
			int fuBenID = -1;
			int result;
			if (!FuBenManager._MapCode2FuBenDict.TryGetValue(mapCode, out fuBenID))
			{
				result = -1;
			}
			else
			{
				result = fuBenID;
			}
			return result;
		}

		// Token: 0x060020E9 RID: 8425 RVA: 0x001C3264 File Offset: 0x001C1464
		public static bool IsFuBenMap(int mapCode)
		{
			bool isFuBenMap = FuBenManager.FindFuBenIDByMapCode(mapCode) > 0;
			if (Global.GetMapType(mapCode) == MapTypes.HuanYingSiYuan)
			{
				isFuBenMap = true;
			}
			return isFuBenMap;
		}

		// Token: 0x060020EA RID: 8426 RVA: 0x001C329C File Offset: 0x001C149C
		public static int FindNextMapCodeByFuBenID(int mapCode)
		{
			int fuBenID = FuBenManager.FindFuBenIDByMapCode(mapCode);
			int result;
			if (fuBenID <= 0)
			{
				result = -1;
			}
			else
			{
				List<int> mapCodeList = FuBenManager.FindMapCodeListByFuBenID(fuBenID);
				if (null == mapCodeList)
				{
					result = -1;
				}
				else
				{
					int findIndex = mapCodeList.IndexOf(mapCode);
					if (-1 == findIndex)
					{
						result = -1;
					}
					else if (findIndex >= mapCodeList.Count - 1)
					{
						result = -1;
					}
					else
					{
						result = mapCodeList[findIndex + 1];
					}
				}
			}
			return result;
		}

		// Token: 0x060020EB RID: 8427 RVA: 0x001C3318 File Offset: 0x001C1518
		public static int FindMapCodeIndexByFuBenID(int mapCode)
		{
			int fuBenID = FuBenManager.FindFuBenIDByMapCode(mapCode);
			int result;
			if (fuBenID <= 0)
			{
				result = 0;
			}
			else
			{
				List<int> mapCodeList = FuBenManager.FindMapCodeListByFuBenID(fuBenID);
				if (null == mapCodeList)
				{
					result = 0;
				}
				else
				{
					int findIndex = mapCodeList.IndexOf(mapCode);
					if (-1 == findIndex)
					{
						result = 0;
					}
					else
					{
						result = findIndex + 1;
					}
				}
			}
			return result;
		}

		// Token: 0x060020EC RID: 8428 RVA: 0x001C3378 File Offset: 0x001C1578
		private static List<GoodsData> ParseGoodsDataList(string[] fields)
		{
			List<GoodsData> goodsDataList = new List<GoodsData>();
			for (int i = 0; i < fields.Length; i++)
			{
				string[] sa = fields[i].Split(new char[]
				{
					','
				});
				if (!(fields[i] == "1") && !(fields[i] == ""))
				{
					if (sa.Length != 7)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("解析FuBenMap.xml文件中的奖励项时失败, 物品配置项个数错误", new object[0]), null, true);
					}
					else
					{
						int[] goodsFields = Global.StringArray2IntArray(sa);
						GoodsData goodsData = Global.GetNewGoodsData(goodsFields[0], goodsFields[1], 0, goodsFields[3], goodsFields[2], 0, goodsFields[5], 0, goodsFields[6], goodsFields[4], 0);
						goodsDataList.Add(goodsData);
					}
				}
			}
			return goodsDataList;
		}

		// Token: 0x060020ED RID: 8429 RVA: 0x001C344C File Offset: 0x001C164C
		private static void ParseXmlItem(SystemXmlItem systemXmlItem)
		{
			int mapCode = systemXmlItem.GetIntValue("MapCode", -1);
			int fuBenID = systemXmlItem.GetIntValue("CopyID", -1);
			int maxTime = systemXmlItem.GetIntValue("MaxTime", -1);
			int money = systemXmlItem.GetIntValue("Moneyaward", -1);
			int experience = systemXmlItem.GetIntValue("Experienceaward", -1);
			int nTmpFirstGold = systemXmlItem.GetIntValue("FirstGold", -1);
			int nTmpFirstExp = systemXmlItem.GetIntValue("FirstExp", -1);
			int nMinSaoDangTimer = systemXmlItem.GetIntValue("MinSaoDangTime", -1);
			int nTmpXingHunAward = systemXmlItem.GetIntValue("XingHunaward", -1);
			int nTmpFirstXingHunAward = systemXmlItem.GetIntValue("FirstXingHun", -1);
			int nTmpZhanGongaward = systemXmlItem.GetIntValue("ZhanGongaward", -1);
			int YuanSuFenMoaward = systemXmlItem.GetIntValue("YuanSuFenMoaward", -1);
			int lightAward = systemXmlItem.GetIntValue("YingGuangaward", -1);
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
					goodsDataList = FuBenManager.ParseGoodsDataList(fields);
				}
			}
			List<GoodsData> goodsFirstDataList = null;
			string goodsFirstIDs = systemXmlItem.GetStringValue("FirstGoodsID");
			if (!string.IsNullOrEmpty(goodsFirstIDs))
			{
				string[] fields = goodsFirstIDs.Split(new char[]
				{
					'|'
				});
				if (fields.Length > 0)
				{
					goodsFirstDataList = FuBenManager.ParseGoodsDataList(fields);
				}
			}
			FuBenMapItem fuBenMapItem = new FuBenMapItem
			{
				FuBenID = fuBenID,
				MapCode = mapCode,
				MaxTime = maxTime,
				Money1 = money,
				Experience = experience,
				GoodsDataList = goodsDataList,
				FirstGoodsDataList = goodsFirstDataList,
				MinSaoDangTimer = nMinSaoDangTimer,
				nFirstExp = nTmpFirstExp,
				nFirstGold = nTmpFirstGold,
				nXingHunAward = nTmpXingHunAward,
				nFirstXingHunAward = nTmpFirstXingHunAward,
				nZhanGongaward = nTmpZhanGongaward,
				YuanSuFenMoaward = YuanSuFenMoaward,
				LightAward = lightAward
			};
			string key = string.Format("{0}_{1}", fuBenID, mapCode);
			lock (FuBenManager._FuBenMapCode2MapItemDict)
			{
				FuBenManager._FuBenMapCode2MapItemDict[key] = fuBenMapItem;
			}
			List<int> mapCodeList = null;
			if (!FuBenManager._FuBen2MapCodeListDict.TryGetValue(fuBenID, out mapCodeList))
			{
				mapCodeList = new List<int>();
				FuBenManager._FuBen2MapCodeListDict[fuBenID] = mapCodeList;
			}
			mapCodeList.Add(mapCode);
			FuBenManager._MapCode2FuBenDict[mapCode] = fuBenID;
		}

		// Token: 0x060020EE RID: 8430 RVA: 0x001C36E8 File Offset: 0x001C18E8
		public static void LoadFuBenMap()
		{
			XElement xml = null;
			string fileName = "Config/FuBenMap.xml";
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
				FuBenManager.ParseXmlItem(systemXmlItem);
			}
		}

		// Token: 0x060020EF RID: 8431 RVA: 0x001C37BC File Offset: 0x001C19BC
		public static int GetFuBenMaxTimeSecs(int fuBenId)
		{
			SystemXmlItem copyItem = null;
			FuBenMapItem mapItem = null;
			int result;
			if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(fuBenId, out copyItem) || (mapItem = FuBenManager.FindMapCodeByFuBenID(fuBenId, copyItem.GetIntValue("MapCode", -1))) == null)
			{
				result = 15;
			}
			else
			{
				result = mapItem.MaxTime * 60;
			}
			return result;
		}

		// Token: 0x060020F0 RID: 8432 RVA: 0x001C3818 File Offset: 0x001C1A18
		public static bool CanFuBenMapFallGoodsAutoGet(GameClient client)
		{
			int fuBenSeqID = client.ClientData.FuBenSeqID;
			FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(fuBenSeqID);
			bool result;
			if (null == fuBenInfoItem)
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemFuBenItem = null;
				if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(fuBenInfoItem.FuBenID, out systemFuBenItem))
				{
					result = false;
				}
				else
				{
					int copyType = systemFuBenItem.GetIntValue("CopyType", -1);
					result = (0 == copyType);
				}
			}
			return result;
		}

		// Token: 0x060020F1 RID: 8433 RVA: 0x001C3888 File Offset: 0x001C1A88
		public static int GetFuBenMapAwardsGoodsBinding(GameClient client)
		{
			int fuBenSeqID = client.ClientData.FuBenSeqID;
			return FuBenManager.GetFuBenMapAwardsGoodsBinding(fuBenSeqID);
		}

		// Token: 0x060020F2 RID: 8434 RVA: 0x001C38AC File Offset: 0x001C1AAC
		public static int GetFuBenMapAwardsGoodsBinding(int fuBenSeqID)
		{
			FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(fuBenSeqID);
			int result;
			if (null == fuBenInfoItem)
			{
				result = 0;
			}
			else
			{
				result = fuBenInfoItem.GoodsBinding;
			}
			return result;
		}

		// Token: 0x060020F3 RID: 8435 RVA: 0x001C38DC File Offset: 0x001C1ADC
		public static bool CanGetFuBenMapAwards(GameClient client)
		{
			int fuBenID = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
			bool result;
			if (fuBenID <= 0)
			{
				result = false;
			}
			else if (client.ClientData.FuBenSeqID <= 0)
			{
				result = false;
			}
			else
			{
				FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(client.ClientData.FuBenSeqID);
				if (null == fuBenInfoItem)
				{
					result = false;
				}
				else if (fuBenID != fuBenInfoItem.FuBenID)
				{
					result = false;
				}
				else
				{
					FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(fuBenID, client.ClientData.MapCode);
					if (null == fuBenMapItem)
					{
						result = false;
					}
					else
					{
						if (fuBenMapItem.GoodsDataList == null || fuBenMapItem.GoodsDataList.Count <= 0)
						{
							if (fuBenMapItem.Experience <= 0)
							{
								if (fuBenMapItem.Money1 <= 0)
								{
									return false;
								}
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x060020F4 RID: 8436 RVA: 0x001C39D4 File Offset: 0x001C1BD4
		public static bool CanAutoGetFuBenMapAwards(GameClient client)
		{
			int fuBenID = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
			bool result;
			if (fuBenID <= 0)
			{
				result = false;
			}
			else if (client.ClientData.FuBenSeqID <= 0)
			{
				result = false;
			}
			else
			{
				FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(client.ClientData.FuBenSeqID);
				if (null == fuBenInfoItem)
				{
					result = false;
				}
				else if (fuBenID != fuBenInfoItem.FuBenID)
				{
					result = false;
				}
				else
				{
					FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(fuBenID, client.ClientData.MapCode);
					if (null == fuBenMapItem)
					{
						result = false;
					}
					else
					{
						if (fuBenMapItem.GoodsDataList == null || fuBenMapItem.GoodsDataList.Count <= 0)
						{
							if (fuBenMapItem.Experience <= 0)
							{
								if (fuBenMapItem.Money1 <= 0)
								{
									return false;
								}
							}
						}
						result = (fuBenMapItem.GoodsDataList == null || fuBenMapItem.GoodsDataList.Count <= 0);
					}
				}
			}
			return result;
		}

		// Token: 0x060020F5 RID: 8437 RVA: 0x001C3AF4 File Offset: 0x001C1CF4
		public static bool ProcessFuBenMapAwards(GameClient client, bool notifyClient = false)
		{
			bool result;
			if (client.ClientData.FuBenSeqID < 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(113, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = false;
			}
			else
			{
				int awardState = GameManager.CopyMapMgr.FindAwardState(client.ClientData.RoleID, client.ClientData.FuBenSeqID, client.ClientData.MapCode);
				if (awardState > 0)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(21, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = false;
				}
				else
				{
					int fuBenID = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
					if (fuBenID <= 0)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(114, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = false;
					}
					else
					{
						FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(client.ClientData.FuBenSeqID);
						if (null == fuBenInfoItem)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(115, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							result = false;
						}
						else if (fuBenID != fuBenInfoItem.FuBenID)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(116, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							result = false;
						}
						else
						{
							FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(fuBenID, client.ClientData.MapCode);
							if (null == fuBenMapItem)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(117, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								result = false;
							}
							else
							{
								CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(client.ClientData.MapCode);
								if (copyMap == null)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(118, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
									result = false;
								}
								else
								{
									GameManager.CopyMapMgr.AddAwardState(client.ClientData.RoleID, client.ClientData.FuBenSeqID, client.ClientData.MapCode, 1);
									int nMaxTime = fuBenMapItem.MaxTime * 60;
									long startTicks = fuBenInfoItem.StartTicks;
									long endTicks = fuBenInfoItem.EndTicks;
									int nFinishTimer = (int)(endTicks - startTicks) / 1000;
									int killedNum = 0;
									int nDieCount = fuBenInfoItem.nDieCount;
									FuBenTongGuanData fubenTongGuanData = Global.GiveCopyMapGiftForScore(client, fuBenID, client.ClientData.MapCode, nMaxTime, nFinishTimer, killedNum, nDieCount, (int)((double)fuBenMapItem.Experience * fuBenInfoItem.AwardRate), (int)((double)fuBenMapItem.Money1 * fuBenInfoItem.AwardRate), fuBenMapItem, null);
									if (fubenTongGuanData != null)
									{
										TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<FuBenTongGuanData>(fubenTongGuanData, Global._TCPManager.TcpOutPacketPool, 521);
										if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
										{
										}
									}
									Global.AddFuBenAwardEvent(client, fuBenID);
									result = true;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x040036A8 RID: 13992
		private static Dictionary<int, int> _FuBenSeqIDDict = new Dictionary<int, int>();

		// Token: 0x040036A9 RID: 13993
		private static Dictionary<int, FuBenInfoItem> _FuBenSeqID2InfoDict = new Dictionary<int, FuBenInfoItem>();

		// Token: 0x040036AA RID: 13994
		private static Dictionary<string, FuBenMapItem> _FuBenMapCode2MapItemDict = new Dictionary<string, FuBenMapItem>();

		// Token: 0x040036AB RID: 13995
		private static Dictionary<int, List<int>> _FuBen2MapCodeListDict = new Dictionary<int, List<int>>();

		// Token: 0x040036AC RID: 13996
		private static Dictionary<int, int> _MapCode2FuBenDict = new Dictionary<int, int>();
	}
}
