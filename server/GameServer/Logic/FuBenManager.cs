using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class FuBenManager
	{
		
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

		
		public static List<FuBenMapItem> GetAllFubenMapItem()
		{
			List<FuBenMapItem> list = new List<FuBenMapItem>();
			lock (FuBenManager._FuBenMapCode2MapItemDict)
			{
				list.AddRange(FuBenManager._FuBenMapCode2MapItemDict.Values);
			}
			return list;
		}

		
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

		
		public static bool IsFuBenMap(int mapCode)
		{
			bool isFuBenMap = FuBenManager.FindFuBenIDByMapCode(mapCode) > 0;
			if (Global.GetMapType(mapCode) == MapTypes.HuanYingSiYuan)
			{
				isFuBenMap = true;
			}
			return isFuBenMap;
		}

		
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

		
		public static int GetFuBenMapAwardsGoodsBinding(GameClient client)
		{
			int fuBenSeqID = client.ClientData.FuBenSeqID;
			return FuBenManager.GetFuBenMapAwardsGoodsBinding(fuBenSeqID);
		}

		
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

		
		private static Dictionary<int, int> _FuBenSeqIDDict = new Dictionary<int, int>();

		
		private static Dictionary<int, FuBenInfoItem> _FuBenSeqID2InfoDict = new Dictionary<int, FuBenInfoItem>();

		
		private static Dictionary<string, FuBenMapItem> _FuBenMapCode2MapItemDict = new Dictionary<string, FuBenMapItem>();

		
		private static Dictionary<int, List<int>> _FuBen2MapCodeListDict = new Dictionary<int, List<int>>();

		
		private static Dictionary<int, int> _MapCode2FuBenDict = new Dictionary<int, int>();
	}
}
