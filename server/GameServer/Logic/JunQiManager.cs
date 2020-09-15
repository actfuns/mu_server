using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x0200073B RID: 1851
	public class JunQiManager
	{
		// Token: 0x06002E53 RID: 11859 RVA: 0x00287B34 File Offset: 0x00285D34
		public static void AddKillJunQiItem(int mapCode, int npcID, int bhid)
		{
			string key = string.Format("{0}_{1}", mapCode, npcID);
			lock (JunQiManager.KillJunQiDict)
			{
				JunQiManager.KillJunQiDict[key] = new KillJunQiItem
				{
					BHID = bhid,
					KillJunQiTicks = TimeUtil.NOW()
				};
			}
		}

		// Token: 0x06002E54 RID: 11860 RVA: 0x00287BB8 File Offset: 0x00285DB8
		public static bool CanInstallJunQiNow(int mapCode, int npcExtentionID, int bhid)
		{
			KillJunQiItem killJunQiItem = null;
			long ticks = TimeUtil.NOW();
			string key = string.Format("{0}_{1}", mapCode, npcExtentionID);
			lock (JunQiManager.KillJunQiDict)
			{
				if (!JunQiManager.KillJunQiDict.TryGetValue(key, out killJunQiItem))
				{
					return true;
				}
				if (killJunQiItem.BHID == bhid)
				{
					return true;
				}
				if (ticks - killJunQiItem.KillJunQiTicks >= 10000L)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002E55 RID: 11861 RVA: 0x00287C74 File Offset: 0x00285E74
		public static void LoadBangHuiJunQiItemsDictFromDBServer()
		{
			byte[] bytesData = null;
			if (TCPProcessCmdResults.RESULT_FAILED != Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10073, string.Format("{0}", GameManager.ServerLineID), out bytesData, 0))
			{
				if (bytesData != null && bytesData.Length > 6)
				{
					int length = BitConverter.ToInt32(bytesData, 0);
					Dictionary<int, BangHuiJunQiItemData> oldBangHuiJunQiItemsDict = JunQiManager._BangHuiJunQiItemsDict;
					Dictionary<int, BangHuiJunQiItemData> newBangHuiJunQiItemsDict = DataHelper.BytesToObject<Dictionary<int, BangHuiJunQiItemData>>(bytesData, 6, length - 2);
					if (null != newBangHuiJunQiItemsDict)
					{
						foreach (int key in newBangHuiJunQiItemsDict.Keys)
						{
							if (oldBangHuiJunQiItemsDict == null || !oldBangHuiJunQiItemsDict.ContainsKey(key))
							{
								BangHuiJunQiItemData bangHuiJunQiItemData = newBangHuiJunQiItemsDict[key];
							}
							else
							{
								BangHuiJunQiItemData bangHuiJunQiItemData = newBangHuiJunQiItemsDict[key];
								BangHuiJunQiItemData odlBangHuiLingDiItemData = oldBangHuiJunQiItemsDict[key];
								if (bangHuiJunQiItemData.QiLevel != odlBangHuiLingDiItemData.QiLevel)
								{
								}
							}
						}
					}
					JunQiManager._BangHuiJunQiItemsDict = newBangHuiJunQiItemsDict;
				}
			}
		}

		// Token: 0x06002E56 RID: 11862 RVA: 0x00287DB0 File Offset: 0x00285FB0
		public static void NotifySyncBangHuiJunQiItemsDict(GameClient client)
		{
			string gmCmdData = string.Format("-syncjunqi", new object[0]);
			GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				(client != null) ? client.ClientData.RoleID : -1,
				"",
				0,
				"",
				0,
				gmCmdData,
				0,
				0,
				-1
			}), null, 0);
		}

		// Token: 0x06002E57 RID: 11863 RVA: 0x00287E50 File Offset: 0x00286050
		public static int GetJunQiLevelByBHID(int bhid)
		{
			int result;
			if (null == JunQiManager._BangHuiJunQiItemsDict)
			{
				result = 0;
			}
			else
			{
				BangHuiJunQiItemData bangHuiJunQiItemData = null;
				if (!JunQiManager._BangHuiJunQiItemsDict.TryGetValue(bhid, out bangHuiJunQiItemData))
				{
					result = 0;
				}
				else
				{
					result = bangHuiJunQiItemData.QiLevel;
				}
			}
			return result;
		}

		// Token: 0x06002E58 RID: 11864 RVA: 0x00287E94 File Offset: 0x00286094
		public static string GetJunQiNameByBHID(int bhid)
		{
			string result;
			if (null == JunQiManager._BangHuiJunQiItemsDict)
			{
				result = GLang.GetLang(393, new object[0]);
			}
			else
			{
				BangHuiJunQiItemData bangHuiJunQiItemData = null;
				if (!JunQiManager._BangHuiJunQiItemsDict.TryGetValue(bhid, out bangHuiJunQiItemData))
				{
					result = GLang.GetLang(393, new object[0]);
				}
				else
				{
					result = bangHuiJunQiItemData.QiName;
				}
			}
			return result;
		}

		// Token: 0x06002E59 RID: 11865 RVA: 0x00287EF8 File Offset: 0x002860F8
		public static Dictionary<int, BangHuiLingDiItemData> LoadBangHuiLingDiItemsDictFromDBServer(int serverId)
		{
			byte[] bytesData = null;
			Dictionary<int, BangHuiLingDiItemData> result;
			if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10074, string.Format("{0}", GameManager.ServerLineID), out bytesData, serverId))
			{
				result = null;
			}
			else if (bytesData == null || bytesData.Length <= 6)
			{
				result = null;
			}
			else
			{
				int length = BitConverter.ToInt32(bytesData, 0);
				Dictionary<int, BangHuiLingDiItemData> newBangHuiLingDiItemsDict = DataHelper.BytesToObject<Dictionary<int, BangHuiLingDiItemData>>(bytesData, 6, length - 2);
				result = newBangHuiLingDiItemsDict;
			}
			return result;
		}

		// Token: 0x06002E5A RID: 11866 RVA: 0x00287F80 File Offset: 0x00286180
		public static void LoadBangHuiLingDiItemsDictFromDBServer()
		{
			Dictionary<int, BangHuiLingDiItemData> oldBangHuiLingDiItemsDict = JunQiManager._BangHuiLingDiItemsDict;
			Dictionary<int, BangHuiLingDiItemData> newBangHuiLingDiItemsDict = JunQiManager.LoadBangHuiLingDiItemsDictFromDBServer(0);
			bool luoLanChengZhuBHIDChanged = false;
			if (null != newBangHuiLingDiItemsDict)
			{
				foreach (int key in newBangHuiLingDiItemsDict.Keys)
				{
					if (oldBangHuiLingDiItemsDict == null || !oldBangHuiLingDiItemsDict.ContainsKey(key))
					{
						BangHuiLingDiItemData bangHuiLingDiItemData = newBangHuiLingDiItemsDict[key];
						GameManager.ClientMgr.NotifyAllLingDiForBHMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, bangHuiLingDiItemData.LingDiID, bangHuiLingDiItemData.BHID, bangHuiLingDiItemData.ZoneID, bangHuiLingDiItemData.BHName, bangHuiLingDiItemData.LingDiTax);
						if (key == 7)
						{
							luoLanChengZhuBHIDChanged = true;
						}
					}
					else
					{
						BangHuiLingDiItemData bangHuiLingDiItemData = newBangHuiLingDiItemsDict[key];
						BangHuiLingDiItemData odlBangHuiLingDiItemData = oldBangHuiLingDiItemsDict[key];
						if (bangHuiLingDiItemData.BHID != odlBangHuiLingDiItemData.BHID || bangHuiLingDiItemData.LingDiTax != odlBangHuiLingDiItemData.LingDiTax)
						{
							GameManager.ClientMgr.NotifyAllLingDiForBHMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, bangHuiLingDiItemData.LingDiID, bangHuiLingDiItemData.BHID, bangHuiLingDiItemData.ZoneID, bangHuiLingDiItemData.BHName, bangHuiLingDiItemData.LingDiTax);
							if (key == 7)
							{
								luoLanChengZhuBHIDChanged = true;
							}
						}
					}
				}
			}
			JunQiManager._BangHuiLingDiItemsDict = newBangHuiLingDiItemsDict;
			if (luoLanChengZhuBHIDChanged)
			{
				LuoLanChengZhanManager.getInstance().BangHuiLingDiItemsDictFromDBServer();
			}
			Global.UpdateWangChengZhanWeekDays(true);
		}

		// Token: 0x06002E5B RID: 11867 RVA: 0x00288130 File Offset: 0x00286330
		public static Dictionary<int, BangHuiLingDiItemData> GetBangHuiLingDiItemsDict()
		{
			return JunQiManager._BangHuiLingDiItemsDict;
		}

		// Token: 0x06002E5C RID: 11868 RVA: 0x00288148 File Offset: 0x00286348
		public static void NotifySyncBangHuiLingDiItemsDict()
		{
			JunQiManager.LoadBangHuiLingDiItemsDictFromDBServer();
			string gmCmdData = string.Format("-synclingdi", new object[0]);
			GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				0,
				"",
				0,
				"",
				0,
				gmCmdData,
				0,
				0,
				-1
			}), null, 0);
		}

		// Token: 0x06002E5D RID: 11869 RVA: 0x002881DC File Offset: 0x002863DC
		public static BangHuiLingDiItemData GetItemByLingDiID(int lingDiID)
		{
			BangHuiLingDiItemData result;
			if (null == JunQiManager._BangHuiLingDiItemsDict)
			{
				result = null;
			}
			else
			{
				BangHuiLingDiItemData bangHuiLingDiItemData = null;
				if (!JunQiManager._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiItemData))
				{
					result = null;
				}
				else
				{
					result = bangHuiLingDiItemData;
				}
			}
			return result;
		}

		// Token: 0x06002E5E RID: 11870 RVA: 0x0028821C File Offset: 0x0028641C
		public static int GetBHIDByLingDiID(int lingDiID)
		{
			int result;
			if (null == JunQiManager._BangHuiLingDiItemsDict)
			{
				result = 0;
			}
			else
			{
				BangHuiLingDiItemData bangHuiLingDiItemData = null;
				if (!JunQiManager._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiItemData))
				{
					result = 0;
				}
				else
				{
					result = bangHuiLingDiItemData.BHID;
				}
			}
			return result;
		}

		// Token: 0x06002E5F RID: 11871 RVA: 0x00288260 File Offset: 0x00286460
		public static int GetTaxByLingDiID(int lingDiID)
		{
			int result;
			if (null == JunQiManager._BangHuiLingDiItemsDict)
			{
				result = 0;
			}
			else
			{
				BangHuiLingDiItemData bangHuiLingDiItemData = null;
				if (!JunQiManager._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiItemData))
				{
					result = 0;
				}
				else
				{
					result = bangHuiLingDiItemData.LingDiTax;
				}
			}
			return result;
		}

		// Token: 0x06002E60 RID: 11872 RVA: 0x002882A4 File Offset: 0x002864A4
		public static BangHuiLingDiItemData GetFirstLingDiItemDataByBHID(int bhid)
		{
			BangHuiLingDiItemData result;
			if (null == JunQiManager._BangHuiLingDiItemsDict)
			{
				result = null;
			}
			else
			{
				int lingDiID = 10000;
				BangHuiLingDiItemData bangHuiLingDiItemData = null;
				foreach (BangHuiLingDiItemData val in JunQiManager._BangHuiLingDiItemsDict.Values)
				{
					if (val.LingDiID != 1)
					{
						if (val.BHID == bhid)
						{
							if (val.LingDiID < lingDiID)
							{
								lingDiID = val.LingDiID;
								bangHuiLingDiItemData = val;
							}
						}
					}
				}
				result = bangHuiLingDiItemData;
			}
			return result;
		}

		// Token: 0x06002E61 RID: 11873 RVA: 0x0028836C File Offset: 0x0028656C
		public static BangHuiLingDiItemData GetAnyLingDiItemDataByBHID(int bhid)
		{
			BangHuiLingDiItemData result;
			if (null == JunQiManager._BangHuiLingDiItemsDict)
			{
				result = null;
			}
			else
			{
				BangHuiLingDiItemData bangHuiLingDiItemData = null;
				foreach (BangHuiLingDiItemData val in JunQiManager._BangHuiLingDiItemsDict.Values)
				{
					if (val.LingDiID != 1)
					{
						if (val.BHID == bhid)
						{
							bangHuiLingDiItemData = val;
							break;
						}
					}
				}
				result = bangHuiLingDiItemData;
			}
			return result;
		}

		// Token: 0x06002E62 RID: 11874 RVA: 0x00288408 File Offset: 0x00286608
		private static Point GetQiZuoNPCPosition(int mapCode, int npcID)
		{
			SystemXmlItem systemQiZuoItem = null;
			Point result;
			if (!GameManager.systemQiZuoMgr.SystemXmlItemDict.TryGetValue(mapCode, out systemQiZuoItem))
			{
				result = new Point(0.0, 0.0);
			}
			else
			{
				for (int i = 1; i <= JunQiManager.MaxInstallQiNum; i++)
				{
					if (npcID == systemQiZuoItem.GetIntValue(string.Format("NPC{0}", i), -1))
					{
						return new Point((double)systemQiZuoItem.GetIntValue(string.Format("NPC{0}PosX", i), -1), (double)systemQiZuoItem.GetIntValue(string.Format("NPC{0}PosY", i), -1));
					}
				}
				result = new Point(0.0, 0.0);
			}
			return result;
		}

		// Token: 0x06002E63 RID: 11875 RVA: 0x002884DC File Offset: 0x002866DC
		private static Point GetNormalMapJunQiPosition(int mapCode)
		{
			SystemXmlItem systemQiZiItem = null;
			Point result;
			if (!GameManager.systemLingQiMapQiZhiMgr.SystemXmlItemDict.TryGetValue(mapCode, out systemQiZiItem))
			{
				result = new Point(0.0, 0.0);
			}
			else
			{
				result = new Point((double)systemQiZiItem.GetIntValue("PosX", -1), (double)systemQiZiItem.GetIntValue("PosY", -1));
			}
			return result;
		}

		// Token: 0x06002E64 RID: 11876 RVA: 0x00288540 File Offset: 0x00286740
		private static JunQiItem AddJunQi(int mapCode, int bhid, int zoneID, string bhName, int npcID, string junQiName, int junQiLevel, SceneUIClasses sceneType = SceneUIClasses.Normal)
		{
			SystemXmlItem systemJunQiItem = null;
			JunQiItem result;
			if (!GameManager.systemJunQiMgr.SystemXmlItemDict.TryGetValue(junQiLevel, out systemJunQiItem))
			{
				result = null;
			}
			else
			{
				Point junQiPos = new Point(0.0, 0.0);
				if (sceneType == SceneUIClasses.LuoLanChengZhan)
				{
					if (-1 != npcID)
					{
						junQiPos = JunQiManager.GetQiZuoNPCPosition(mapCode, npcID);
					}
				}
				else
				{
					junQiPos = JunQiManager.GetNormalMapJunQiPosition(mapCode);
				}
				if (0.0 == junQiPos.X && 0.0 == junQiPos.Y)
				{
					result = null;
				}
				else
				{
					JunQiItem JunQiItem = new JunQiItem
					{
						JunQiID = (int)GameManager.JunQiIDMgr.GetNewID(),
						QiName = junQiName,
						ZoneID = zoneID,
						BHID = bhid,
						BHName = bhName,
						JunQiLevel = junQiLevel,
						QiZuoNPC = npcID,
						MapCode = mapCode,
						PosX = (int)junQiPos.X,
						PosY = (int)junQiPos.Y,
						Direction = 0,
						LifeV = systemJunQiItem.GetIntValue("Lifev", -1),
						StartTime = TimeUtil.NOW(),
						CurrentLifeV = systemJunQiItem.GetIntValue("Lifev", -1),
						CutLifeV = systemJunQiItem.GetIntValue("CutLifeV", -1),
						BodyCode = systemJunQiItem.GetIntValue("BodyCode", -1),
						PicCode = systemJunQiItem.GetIntValue("PicCode", -1),
						ManagerType = sceneType
					};
					if (-1 != npcID)
					{
						lock (JunQiManager._NPCID2JunQiDict)
						{
							JunQiManager._NPCID2JunQiDict[npcID] = JunQiItem;
						}
					}
					lock (JunQiManager._ID2JunQiDict)
					{
						JunQiManager._ID2JunQiDict[JunQiItem.JunQiID] = JunQiItem;
					}
					result = JunQiItem;
				}
			}
			return result;
		}

		// Token: 0x06002E65 RID: 11877 RVA: 0x002887A0 File Offset: 0x002869A0
		public static JunQiItem FindJunQiByNpcID(int npcID)
		{
			JunQiItem JunQiItem = null;
			lock (JunQiManager._NPCID2JunQiDict)
			{
				JunQiManager._NPCID2JunQiDict.TryGetValue(npcID, out JunQiItem);
			}
			return JunQiItem;
		}

		// Token: 0x06002E66 RID: 11878 RVA: 0x002887FC File Offset: 0x002869FC
		public static JunQiItem FindJunQiByID(int JunQiID)
		{
			JunQiItem JunQiItem = null;
			lock (JunQiManager._ID2JunQiDict)
			{
				JunQiManager._ID2JunQiDict.TryGetValue(JunQiID, out JunQiItem);
			}
			return JunQiItem;
		}

		// Token: 0x06002E67 RID: 11879 RVA: 0x00288858 File Offset: 0x00286A58
		public static JunQiItem FindJunQiByBHID(int bhid)
		{
			JunQiItem JunQiItem = null;
			lock (JunQiManager._ID2JunQiDict)
			{
				foreach (JunQiItem val in JunQiManager._ID2JunQiDict.Values)
				{
					if (val.BHID == bhid)
					{
						JunQiItem = val;
						break;
					}
				}
			}
			return JunQiItem;
		}

		// Token: 0x06002E68 RID: 11880 RVA: 0x00288908 File Offset: 0x00286B08
		private static void RemoveJunQi(int JunQiID)
		{
			JunQiItem JunQiItem = null;
			lock (JunQiManager._ID2JunQiDict)
			{
				JunQiManager._ID2JunQiDict.TryGetValue(JunQiID, out JunQiItem);
				if (null != JunQiItem)
				{
					JunQiManager._ID2JunQiDict.Remove(JunQiItem.JunQiID);
				}
			}
			if (null != JunQiItem)
			{
				if (-1 != JunQiItem.QiZuoNPC)
				{
					lock (JunQiManager._NPCID2JunQiDict)
					{
						JunQiManager._NPCID2JunQiDict.Remove(JunQiItem.QiZuoNPC);
					}
				}
			}
		}

		// Token: 0x06002E69 RID: 11881 RVA: 0x002889E0 File Offset: 0x00286BE0
		private static int CalcJunQiNumByMapCode(int mapCode, out int bhid)
		{
			bhid = 0;
			Dictionary<int, int> calcDict = new Dictionary<int, int>();
			lock (JunQiManager._ID2JunQiDict)
			{
				foreach (JunQiItem val in JunQiManager._ID2JunQiDict.Values)
				{
					if (val.MapCode == mapCode)
					{
						if (calcDict.ContainsKey(val.BHID))
						{
							calcDict[val.BHID] = calcDict[val.BHID] + 1;
						}
						else
						{
							calcDict[val.BHID] = 1;
						}
					}
				}
			}
			int maxNum = 0;
			foreach (int key in calcDict.Keys)
			{
				if (calcDict[key] > maxNum)
				{
					maxNum = calcDict[key];
					bhid = key;
				}
			}
			return maxNum;
		}

		// Token: 0x06002E6A RID: 11882 RVA: 0x00288B44 File Offset: 0x00286D44
		private static string GetBHNameByNPCID(int npcID)
		{
			JunQiItem junQiItem = JunQiManager.FindJunQiByNpcID(npcID);
			string result;
			if (null == junQiItem)
			{
				result = "";
			}
			else
			{
				result = Global.FormatBangHuiName(junQiItem.ZoneID, junQiItem.BHName);
			}
			return result;
		}

		// Token: 0x06002E6B RID: 11883 RVA: 0x00288B84 File Offset: 0x00286D84
		public static bool ProcessNewJunQi(SocketListener sl, TCPOutPacketPool pool, int mapCode, int bhid, int zoneID, string bhName, int npcID, string junQiName, int junQiLevel, SceneUIClasses sceneType = SceneUIClasses.Normal)
		{
			JunQiItem JunQiItem = JunQiManager.AddJunQi(mapCode, bhid, zoneID, bhName, npcID, junQiName, junQiLevel, sceneType);
			bool result;
			if (null == JunQiItem)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("为RoleID生成帮旗对象时失败, MapCode={0}, BHID={1}, BHName={2}, NPCID={3}, QiName={4}, QiLevel={5}", new object[]
				{
					mapCode,
					bhid,
					bhName,
					npcID,
					junQiName,
					junQiLevel
				}), null, true);
				result = false;
			}
			else
			{
				GameManager.MapGridMgr.DictGrids[JunQiItem.MapCode].MoveObject(-1, -1, JunQiItem.PosX, JunQiItem.PosY, JunQiItem);
				result = true;
			}
			return result;
		}

		// Token: 0x06002E6C RID: 11884 RVA: 0x00288C30 File Offset: 0x00286E30
		public static void ProcessDelJunQi(SocketListener sl, TCPOutPacketPool pool, int JunQiID)
		{
			JunQiItem JunQiItem = JunQiManager.FindJunQiByID(JunQiID);
			if (null != JunQiItem)
			{
				JunQiManager.RemoveJunQi(JunQiID);
				GameManager.MapGridMgr.DictGrids[JunQiItem.MapCode].RemoveObject(JunQiItem);
			}
		}

		// Token: 0x06002E6D RID: 11885 RVA: 0x00288C78 File Offset: 0x00286E78
		public static void ProcessDelAllJunQiByMapCode(SocketListener sl, TCPOutPacketPool pool, int mapCode)
		{
			List<JunQiItem> junQiItemList = new List<JunQiItem>();
			lock (JunQiManager._ID2JunQiDict)
			{
				foreach (JunQiItem val in JunQiManager._ID2JunQiDict.Values)
				{
					if (val.MapCode == mapCode)
					{
						junQiItemList.Add(val);
					}
				}
			}
			for (int i = 0; i < junQiItemList.Count; i++)
			{
				JunQiManager.RemoveJunQi(junQiItemList[i].JunQiID);
				GameManager.MapGridMgr.DictGrids[junQiItemList[i].MapCode].RemoveObject(junQiItemList[i]);
			}
		}

		// Token: 0x06002E6E RID: 11886 RVA: 0x00288D80 File Offset: 0x00286F80
		public static void ProcessDelAllJunQiByBHID(SocketListener sl, TCPOutPacketPool pool, int bhid)
		{
			List<JunQiItem> junQiItemList = new List<JunQiItem>();
			lock (JunQiManager._ID2JunQiDict)
			{
				foreach (JunQiItem val in JunQiManager._ID2JunQiDict.Values)
				{
					if (val.BHID == bhid)
					{
						junQiItemList.Add(val);
					}
				}
			}
			for (int i = 0; i < junQiItemList.Count; i++)
			{
				JunQiManager.RemoveJunQi(junQiItemList[i].JunQiID);
				GameManager.MapGridMgr.DictGrids[junQiItemList[i].MapCode].RemoveObject(junQiItemList[i]);
			}
		}

		// Token: 0x06002E6F RID: 11887 RVA: 0x00288E88 File Offset: 0x00287088
		private static List<int> GetQiZuoNPCIDList(int mapCode)
		{
			SystemXmlItem systemQiZuoItem = null;
			List<int> result;
			if (!GameManager.systemQiZuoMgr.SystemXmlItemDict.TryGetValue(mapCode, out systemQiZuoItem))
			{
				result = null;
			}
			else
			{
				List<int> list = new List<int>();
				for (int i = 1; i <= JunQiManager.MaxInstallQiNum; i++)
				{
					list.Add(systemQiZuoItem.GetIntValue(string.Format("NPC{0}", i), -1));
				}
				result = list;
			}
			return result;
		}

		// Token: 0x06002E70 RID: 11888 RVA: 0x00288EFC File Offset: 0x002870FC
		public static void ProcessAllNewJunQiByMapCode(int mapCode, int bhid, int zoneID, string bhName)
		{
			List<int> list = JunQiManager.GetQiZuoNPCIDList(mapCode);
			if (null != list)
			{
				string junQiName = JunQiManager.GetJunQiNameByBHID(bhid);
				int junQiLevel = JunQiManager.GetJunQiLevelByBHID(bhid);
				for (int i = 0; i < list.Count; i++)
				{
					JunQiManager.ProcessNewJunQi(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, mapCode, bhid, zoneID, bhName, list[i], junQiName, junQiLevel, SceneUIClasses.Normal);
				}
			}
		}

		// Token: 0x06002E71 RID: 11889 RVA: 0x00288F70 File Offset: 0x00287170
		public static void NotifyOthersShowJunQi(SocketListener sl, TCPOutPacketPool pool, JunQiItem JunQiItem)
		{
			if (null != JunQiItem)
			{
				GameManager.MapGridMgr.DictGrids[JunQiItem.MapCode].MoveObject(-1, -1, JunQiItem.PosX, JunQiItem.PosY, JunQiItem);
			}
		}

		// Token: 0x06002E72 RID: 11890 RVA: 0x00288FB8 File Offset: 0x002871B8
		public static void NotifyOthersHideJunQi(SocketListener sl, TCPOutPacketPool pool, JunQiItem JunQiItem)
		{
			if (null != JunQiItem)
			{
				GameManager.MapGridMgr.DictGrids[JunQiItem.MapCode].RemoveObject(JunQiItem);
			}
		}

		// Token: 0x06002E73 RID: 11891 RVA: 0x00288FF0 File Offset: 0x002871F0
		private static bool ProcessJunQiDead(SocketListener sl, TCPOutPacketPool pool, long nowTicks, JunQiItem JunQiItem)
		{
			bool result;
			if (JunQiItem.CurrentLifeV > 0)
			{
				result = false;
			}
			else
			{
				long subTicks = nowTicks - JunQiItem.JunQiDeadTicks;
				if (subTicks < 2000L)
				{
					result = false;
				}
				else
				{
					JunQiManager.ProcessDelJunQi(sl, pool, JunQiItem.JunQiID);
					JunQiManager.NotifyAllLingDiMapInfoData(JunQiItem.MapCode);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002E74 RID: 11892 RVA: 0x00289050 File Offset: 0x00287250
		public static void ProcessAllJunQiItems(SocketListener sl, TCPOutPacketPool pool)
		{
			if (Global.GetBangHuiFightingLineID() == GameManager.ServerLineID)
			{
				List<JunQiItem> JunQiItemList = new List<JunQiItem>();
				lock (JunQiManager._ID2JunQiDict)
				{
					foreach (JunQiItem val in JunQiManager._ID2JunQiDict.Values)
					{
						JunQiItemList.Add(val);
					}
				}
				long nowTicks = TimeUtil.NOW();
				for (int i = 0; i < JunQiItemList.Count; i++)
				{
					JunQiItem JunQiItem = JunQiItemList[i];
					if (JunQiManager.ProcessJunQiDead(sl, pool, nowTicks, JunQiItem))
					{
					}
				}
			}
		}

		// Token: 0x06002E75 RID: 11893 RVA: 0x0028914C File Offset: 0x0028734C
		public static void SendMySelfJunQiItems(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is JunQiItem)
					{
						if ((objsList[i] as JunQiItem).CurrentLifeV > 0)
						{
							JunQiItem JunQiItem = objsList[i] as JunQiItem;
							GameManager.ClientMgr.NotifyMySelfNewJunQi(sl, pool, client, JunQiItem);
						}
					}
				}
			}
		}

		// Token: 0x06002E76 RID: 11894 RVA: 0x002891D0 File Offset: 0x002873D0
		public static void DelMySelfJunQiItems(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is JunQiItem)
					{
						JunQiItem JunQiItem = objsList[i] as JunQiItem;
						GameManager.ClientMgr.NotifyMySelfDelJunQi(sl, pool, client, JunQiItem.JunQiID);
					}
				}
			}
		}

		// Token: 0x06002E77 RID: 11895 RVA: 0x0028923C File Offset: 0x0028743C
		public static void LookupEnemiesInCircle(GameClient client, int mapCode, int toX, int toY, int radius, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objList = mapGrid.FindObjects(toX, toY, radius);
			if (null != objList)
			{
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < objList.Count; i++)
				{
					if (objList[i] is JunQiItem)
					{
						if (client == null || Global.IsOpposition(client, objList[i] as JunQiItem))
						{
							if (client == null || client.ClientData.CopyMapID == (objList[i] as JunQiItem).CopyMapID)
							{
								Point pt = new Point((double)(objList[i] as JunQiItem).PosX, (double)(objList[i] as JunQiItem).PosY);
								if (Global.InCircle(pt, center, (double)radius))
								{
									enemiesList.Add(objList[i]);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002E78 RID: 11896 RVA: 0x00289364 File Offset: 0x00287564
		public static void LookupEnemiesInCircleByAngle(GameClient client, int direction, int mapCode, int toX, int toY, int radius, List<JunQiItem> enemiesList, double angle)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objList = mapGrid.FindObjects(toX, toY, radius);
			if (null != objList)
			{
				double loAngle = 0.0;
				double hiAngle = 0.0;
				Global.GetAngleRangeByDirection(direction, angle, out loAngle, out hiAngle);
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < objList.Count; i++)
				{
					if (objList[i] is JunQiItem)
					{
						if (client == null || Global.IsOpposition(client, objList[i] as JunQiItem))
						{
							if (client == null || client.ClientData.CopyMapID == (objList[i] as JunQiItem).CopyMapID)
							{
								Point pt = new Point((double)(objList[i] as JunQiItem).PosX, (double)(objList[i] as JunQiItem).PosY);
								if (Global.InCircleByAngle(pt, center, (double)radius, loAngle, hiAngle))
								{
									enemiesList.Add(objList[i] as JunQiItem);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002E79 RID: 11897 RVA: 0x002894C4 File Offset: 0x002876C4
		public static void LookupEnemiesAtGridXY(IObject attacker, int gridX, int gridY, List<object> enemiesList)
		{
			int mapCode = attacker.CurrentMapCode;
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objList = mapGrid.FindObjects(gridX, gridY);
			if (null != objList)
			{
				for (int i = 0; i < objList.Count; i++)
				{
					if (objList[i] is JunQiItem)
					{
						if (attacker == null || attacker.CurrentCopyMapID == (objList[i] as JunQiItem).CopyMapID)
						{
							enemiesList.Add(objList[i]);
						}
					}
				}
			}
		}

		// Token: 0x06002E7A RID: 11898 RVA: 0x0028956C File Offset: 0x0028776C
		public static void LookupAttackEnemies(IObject attacker, int direction, List<object> enemiesList)
		{
			int mapCode = attacker.CurrentMapCode;
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			Point grid = attacker.CurrentGrid;
			int gridX = (int)grid.X;
			int gridY = (int)grid.Y;
			Point p = Global.GetGridPointByDirection(direction, gridX, gridY);
			JunQiManager.LookupEnemiesAtGridXY(attacker, (int)p.X, (int)p.Y, enemiesList);
		}

		// Token: 0x06002E7B RID: 11899 RVA: 0x002895D0 File Offset: 0x002877D0
		public static void LookupAttackEnemyIDs(IObject attacker, int direction, List<int> enemiesList)
		{
			List<object> objList = new List<object>();
			JunQiManager.LookupAttackEnemies(attacker, direction, objList);
			for (int i = 0; i < objList.Count; i++)
			{
				enemiesList.Add((objList[i] as JunQiItem).JunQiID);
			}
		}

		// Token: 0x06002E7C RID: 11900 RVA: 0x0028961C File Offset: 0x0028781C
		public static void LookupRangeAttackEnemies(IObject obj, int toX, int toY, int direction, string rangeMode, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
			int gridX = toX / mapGrid.MapGridWidth;
			int gridY = toY / mapGrid.MapGridHeight;
			List<Point> gridList = Global.GetGridPointByDirection(direction, gridX, gridY, rangeMode, true);
			if (gridList.Count > 0)
			{
				for (int i = 0; i < gridList.Count; i++)
				{
					JunQiManager.LookupEnemiesAtGridXY(obj, (int)gridList[i].X, (int)gridList[i].Y, enemiesList);
				}
			}
		}

		// Token: 0x06002E7D RID: 11901 RVA: 0x002896BC File Offset: 0x002878BC
		public static bool CanAttack(JunQiItem enemy)
		{
			bool result;
			if (null == enemy)
			{
				result = false;
			}
			else if (JunQiManager.JugeLingDiZhanEndByMapCode(enemy.MapCode))
			{
				result = false;
			}
			else
			{
				int lingDiID = JunQiManager.GetLingDiIDBy2MapCode(enemy.MapCode);
				result = (lingDiID == 3 && LingDiZhanStates.Fighting == JunQiManager.LingDiZhanState);
			}
			return result;
		}

		// Token: 0x06002E7E RID: 11902 RVA: 0x00289718 File Offset: 0x00287918
		public static int Monster_NotifyInjured(SocketListener sl, TCPOutPacketPool pool, Monster monster, JunQiItem enemy, int burst, int injure, double injurePercent, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0)
		{
			int ret = 0;
			if ((enemy as JunQiItem).CurrentLifeV > 0 && null != monster.OwnerClient)
			{
				GameClient client = monster.OwnerClient;
				injure = (enemy as JunQiItem).CutLifeV;
				injure = (int)((double)injure * baseRate + (double)addVlue);
				injure = (int)((double)injure * injurePercent);
				ret = injure;
				(enemy as JunQiItem).CurrentLifeV -= injure;
				(enemy as JunQiItem).CurrentLifeV = Global.GMax((enemy as JunQiItem).CurrentLifeV, 0);
				int enemyLife = (enemy as JunQiItem).CurrentLifeV;
				(enemy as JunQiItem).AttackedRoleID = client.ClientData.RoleID;
				GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, client, injure);
				(enemy as JunQiItem).AddAttacker(client.ClientData.RoleID, Global.GMax(0, injure), monster);
				GameManager.SystemServerEvents.AddEvent(string.Format("帮旗减血, Injure={0}, Life={1}", injure, enemyLife), EventLevels.Debug);
				if ((enemy as JunQiItem).CurrentLifeV <= 0)
				{
					GameManager.SystemServerEvents.AddEvent(string.Format("帮旗死亡, roleID={0}", (enemy as JunQiItem).JunQiID), EventLevels.Debug);
					JunQiManager.ProcessJunQiDead(sl, pool, client, enemy as JunQiItem);
				}
				if ((enemy as JunQiItem).AttackedRoleID >= 0 && (enemy as JunQiItem).AttackedRoleID != client.ClientData.RoleID)
				{
					GameClient findClient = GameManager.ClientMgr.FindClient((enemy as JunQiItem).AttackedRoleID);
					if (null != findClient)
					{
						GameManager.ClientMgr.NotifySpriteInjured(sl, pool, findClient, findClient.ClientData.MapCode, findClient.ClientData.RoleID, (enemy as JunQiItem).JunQiID, 0, 0, (double)enemyLife, findClient.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
						ClientManager.NotifySelfEnemyInjured(sl, pool, findClient, findClient.ClientData.RoleID, enemy.JunQiID, 0, 0, (double)enemyLife, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
					}
				}
				GameManager.ClientMgr.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (enemy as JunQiItem).JunQiID, burst, injure, (double)enemyLife, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
				GameManager.ClientMgr.NotifySpriteInjured(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.RoleID, (enemy as JunQiItem).JunQiID, burst, injure, (double)enemyLife, monster.MonsterInfo.VLevel, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
				ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, enemy.JunQiID, burst, injure, (double)enemyLife, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
				if (!client.ClientData.DisableChangeRolePurpleName)
				{
					GameManager.ClientMgr.ForceChangeRolePurpleName2(sl, pool, client);
				}
			}
			return ret;
		}

		// Token: 0x06002E7F RID: 11903 RVA: 0x00289A4C File Offset: 0x00287C4C
		public static int NotifyInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, JunQiItem enemy, int burst, int injure, double injurePercent, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0)
		{
			int ret = 0;
			if ((enemy as JunQiItem).CurrentLifeV > 0)
			{
				injure = (enemy as JunQiItem).CutLifeV;
				injure = (int)((double)injure * baseRate + (double)addVlue);
				injure = (int)((double)injure * injurePercent);
				ret = injure;
				(enemy as JunQiItem).CurrentLifeV -= injure;
				(enemy as JunQiItem).CurrentLifeV = Global.GMax((enemy as JunQiItem).CurrentLifeV, 0);
				int enemyLife = (enemy as JunQiItem).CurrentLifeV;
				(enemy as JunQiItem).AttackedRoleID = client.ClientData.RoleID;
				GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, client, injure);
				(enemy as JunQiItem).AddAttacker(client.ClientData.RoleID, Global.GMax(0, injure), null);
				client.ClientData.RoleIDAttackebByMyself = enemy.GetObjectID();
				GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(client, enemy);
				GameManager.SystemServerEvents.AddEvent(string.Format("帮旗减血, Injure={0}, Life={1}", injure, enemyLife), EventLevels.Debug);
				if ((enemy as JunQiItem).CurrentLifeV <= 0)
				{
					GameManager.SystemServerEvents.AddEvent(string.Format("帮旗死亡, roleID={0}", (enemy as JunQiItem).JunQiID), EventLevels.Debug);
					JunQiManager.ProcessJunQiDead(sl, pool, client, enemy as JunQiItem);
				}
				if ((enemy as JunQiItem).AttackedRoleID >= 0 && (enemy as JunQiItem).AttackedRoleID != client.ClientData.RoleID)
				{
					GameClient findClient = GameManager.ClientMgr.FindClient((enemy as JunQiItem).AttackedRoleID);
					if (null != findClient)
					{
						GameManager.ClientMgr.NotifySpriteInjured(sl, pool, findClient, findClient.ClientData.MapCode, findClient.ClientData.RoleID, (enemy as JunQiItem).JunQiID, 0, 0, (double)enemyLife, findClient.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
						ClientManager.NotifySelfEnemyInjured(sl, pool, findClient, findClient.ClientData.RoleID, enemy.JunQiID, 0, 0, (double)enemyLife, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
					}
				}
				GameManager.ClientMgr.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (enemy as JunQiItem).JunQiID, burst, injure, (double)enemyLife, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
				ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, enemy.JunQiID, burst, injure, (double)enemyLife, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
				if (!client.ClientData.DisableChangeRolePurpleName)
				{
					GameManager.ClientMgr.ForceChangeRolePurpleName2(sl, pool, client);
				}
			}
			return ret;
		}

		// Token: 0x06002E80 RID: 11904 RVA: 0x00289D30 File Offset: 0x00287F30
		public static void NotifyInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int enemy, int enemyX, int enemyY, int burst, int injure, double attackPercent, int addAttack, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0)
		{
			object obj = JunQiManager.FindJunQiByID(enemy);
			if (null != obj)
			{
				if ((obj as JunQiItem).CurrentLifeV > 0)
				{
					injure = (obj as JunQiItem).CutLifeV;
					(obj as JunQiItem).CurrentLifeV -= injure;
					(obj as JunQiItem).CurrentLifeV = Global.GMax((obj as JunQiItem).CurrentLifeV, 0);
					int enemyLife = (obj as JunQiItem).CurrentLifeV;
					(obj as JunQiItem).AttackedRoleID = client.ClientData.RoleID;
					GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, client, injure);
					GameManager.SystemServerEvents.AddEvent(string.Format("帮旗减血, Injure={0}, Life={1}", injure, enemyLife), EventLevels.Debug);
					if ((obj as JunQiItem).CurrentLifeV <= 0)
					{
						GameManager.SystemServerEvents.AddEvent(string.Format("帮旗死亡, roleID={0}", (obj as JunQiItem).JunQiID), EventLevels.Debug);
						JunQiManager.ProcessJunQiDead(sl, pool, client, obj as JunQiItem);
					}
					int ownerRoleID = (obj as JunQiItem).GetAttackerFromList();
					if (ownerRoleID >= 0 && ownerRoleID != client.ClientData.RoleID)
					{
						GameClient findClient = GameManager.ClientMgr.FindClient(ownerRoleID);
						if (null != findClient)
						{
							GameManager.ClientMgr.NotifySpriteInjured(sl, pool, findClient, findClient.ClientData.MapCode, findClient.ClientData.RoleID, (obj as JunQiItem).JunQiID, 0, 0, (double)enemyLife, findClient.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
							ClientManager.NotifySelfEnemyInjured(sl, pool, findClient, findClient.ClientData.RoleID, (obj as JunQiItem).JunQiID, 0, 0, (double)enemyLife, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
						}
					}
					GameManager.ClientMgr.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (obj as JunQiItem).JunQiID, burst, injure, (double)enemyLife, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
					ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, (obj as JunQiItem).JunQiID, burst, injure, (double)enemyLife, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
					if (!client.ClientData.DisableChangeRolePurpleName)
					{
						GameManager.ClientMgr.ForceChangeRolePurpleName2(sl, pool, client);
					}
				}
			}
		}

		// Token: 0x06002E81 RID: 11905 RVA: 0x00289FC0 File Offset: 0x002881C0
		private static void ProcessJunQiDead(SocketListener sl, TCPOutPacketPool pool, GameClient client, JunQiItem junQiItem)
		{
			if (!junQiItem.HandledDead)
			{
				junQiItem.HandledDead = true;
				junQiItem.JunQiDeadTicks = TimeUtil.NOW();
				int ownerRoleID = junQiItem.GetAttackerFromList();
				if (ownerRoleID >= 0 && ownerRoleID != client.ClientData.RoleID)
				{
					GameClient findClient = GameManager.ClientMgr.FindClient(ownerRoleID);
					if (null != findClient)
					{
						client = findClient;
					}
				}
				JunQiManager.AddKillJunQiItem(client.ClientData.MapCode, junQiItem.QiZuoNPC, client.ClientData.Faction);
				if (junQiItem.ManagerType == SceneUIClasses.LuoLanChengZhan)
				{
					LuoLanChengZhanManager.getInstance().OnProcessJunQiDead(junQiItem.QiZuoNPC, junQiItem.BHID);
				}
				if (client.ClientData.Faction > 0)
				{
					Global.BroadcastBangHuiMsg(-1, client.ClientData.Faction, StringUtil.substitute(GLang.GetLang(394, new object[0]), new object[]
					{
						Global.FormatRoleName(client, client.ClientData.RoleName),
						Global.GetServerLineName2(),
						Global.GetMapName(client.ClientData.MapCode),
						junQiItem.BHName
					}), true, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlySysHint);
				}
				Global.BroadcastBangHuiMsg(-1, junQiItem.BHID, StringUtil.substitute(GLang.GetLang(395, new object[0]), new object[]
				{
					Global.GetServerLineName2(),
					Global.GetMapName(client.ClientData.MapCode),
					string.IsNullOrEmpty(client.ClientData.BHName) ? "" : StringUtil.substitute(GLang.GetLang(396, new object[0]), new object[]
					{
						client.ClientData.BHName
					}),
					Global.FormatRoleName(client, client.ClientData.RoleName)
				}), true, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlySysHint);
			}
		}

		// Token: 0x06002E82 RID: 11906 RVA: 0x0028A1A8 File Offset: 0x002883A8
		public static void ParseWeekDaysTimes()
		{
			string lingDiZhanWeekDays_str = GameManager.systemParamsList.GetParamValueByName("LingDiZhanWeekDays");
			if (!string.IsNullOrEmpty(lingDiZhanWeekDays_str))
			{
				string[] lingDiZhanWeekDays_fields = lingDiZhanWeekDays_str.Split(new char[]
				{
					','
				});
				int[] weekDays = new int[lingDiZhanWeekDays_fields.Length];
				for (int i = 0; i < lingDiZhanWeekDays_fields.Length; i++)
				{
					weekDays[i] = Global.SafeConvertToInt32(lingDiZhanWeekDays_fields[i]);
				}
				JunQiManager.LingDiZhanWeekDays = weekDays;
			}
			string lingDiIDs2MapCodes_str = GameManager.systemParamsList.GetParamValueByName("LingDiIDs2MapCodes");
			if (!string.IsNullOrEmpty(lingDiIDs2MapCodes_str))
			{
				string[] lingDiIDs2MapCodes_fields = lingDiIDs2MapCodes_str.Split(new char[]
				{
					','
				});
				int[] mapCodes = new int[lingDiIDs2MapCodes_fields.Length];
				for (int i = 0; i < lingDiIDs2MapCodes_fields.Length; i++)
				{
					mapCodes[i] = Global.SafeConvertToInt32(lingDiIDs2MapCodes_fields[i]);
				}
				JunQiManager.LingDiIDs2MapCodes = mapCodes;
			}
			string lingDiZhanFightingDayTimes_str = GameManager.systemParamsList.GetParamValueByName("LingDiZhanFightingDayTimes");
			JunQiManager.LingDiZhanFightingDayTimes = Global.ParseDateTimeRangeStr(lingDiZhanFightingDayTimes_str);
			string lingDiZhanEndDayTimes_str = GameManager.systemParamsList.GetParamValueByName("LingDiZhanEndDayTimes");
			JunQiManager.LingDiZhanEndDayTimes = Global.ParseDateTimeRangeStr(lingDiZhanEndDayTimes_str);
		}

		// Token: 0x06002E83 RID: 11907 RVA: 0x0028A2C4 File Offset: 0x002884C4
		private static bool IsDayOfWeek(int weekDayID)
		{
			bool result;
			if (null == JunQiManager.LingDiZhanWeekDays)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < JunQiManager.LingDiZhanWeekDays.Length; i++)
				{
					if (JunQiManager.LingDiZhanWeekDays[i] == weekDayID)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06002E84 RID: 11908 RVA: 0x0028A318 File Offset: 0x00288518
		public static bool IsInLingDiZhanFightingTime()
		{
			DateTime now = TimeUtil.NowDateTime();
			int weekDayID = (int)now.DayOfWeek;
			bool result;
			if (!JunQiManager.IsDayOfWeek(weekDayID))
			{
				result = false;
			}
			else
			{
				int endMinute = 0;
				result = Global.JugeDateTimeInTimeRange(now, JunQiManager.LingDiZhanFightingDayTimes, out endMinute, false);
			}
			return result;
		}

		// Token: 0x06002E85 RID: 11909 RVA: 0x0028A35C File Offset: 0x0028855C
		private static bool IsInLingDiZhanEndTime()
		{
			DateTime now = TimeUtil.NowDateTime();
			int weekDayID = (int)now.DayOfWeek;
			bool result;
			if (!JunQiManager.IsDayOfWeek(weekDayID))
			{
				result = false;
			}
			else
			{
				int endMinute = 0;
				result = Global.JugeDateTimeInTimeRange(now, JunQiManager.LingDiZhanEndDayTimes, out endMinute, false);
			}
			return result;
		}

		// Token: 0x06002E86 RID: 11910 RVA: 0x0028A3A0 File Offset: 0x002885A0
		public static int GetLingDiIDBy2MapCode(int mapCode)
		{
			int result;
			if (null == JunQiManager.LingDiIDs2MapCodes)
			{
				result = 0;
			}
			else
			{
				for (int i = 0; i < JunQiManager.LingDiIDs2MapCodes.Length; i++)
				{
					if (JunQiManager.LingDiIDs2MapCodes[i] == mapCode)
					{
						return i + 1;
					}
				}
				if (Global.GetWangChengMapCode() == mapCode)
				{
					result = 2;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x06002E87 RID: 11911 RVA: 0x0028A40C File Offset: 0x0028860C
		public static int GetMapCodeByLingDiID(int lingDiID)
		{
			int result;
			if (null == JunQiManager.LingDiIDs2MapCodes)
			{
				result = 0;
			}
			else
			{
				result = JunQiManager.LingDiIDs2MapCodes[lingDiID];
			}
			return result;
		}

		// Token: 0x06002E88 RID: 11912 RVA: 0x0028A438 File Offset: 0x00288638
		public static bool CanInstallJunQi(GameClient client)
		{
			return !JunQiManager.JugeLingDiZhanEndByMapCode(client.ClientData.MapCode) && LingDiZhanStates.Fighting == JunQiManager.LingDiZhanState;
		}

		// Token: 0x06002E89 RID: 11913 RVA: 0x0028A470 File Offset: 0x00288670
		private static bool JugeLingDiZhanEndByMapCode(int mapCode)
		{
			bool result = false;
			lock (JunQiManager.LingDiZhanResultsDict)
			{
				if (!JunQiManager.LingDiZhanResultsDict.TryGetValue(mapCode, out result))
				{
					return false;
				}
			}
			return result;
		}

		// Token: 0x06002E8A RID: 11914 RVA: 0x0028A4D8 File Offset: 0x002886D8
		private static void AddLingDiZhanEndResultByMapCode(int mapCode, bool result)
		{
			lock (JunQiManager.LingDiZhanResultsDict)
			{
				JunQiManager.LingDiZhanResultsDict[mapCode] = result;
			}
		}

		// Token: 0x06002E8B RID: 11915 RVA: 0x0028A52C File Offset: 0x0028872C
		public static void ProcessLingDiZhanResult()
		{
			if (Global.GetBangHuiFightingLineID() == GameManager.ServerLineID)
			{
				if (JunQiManager.LingDiIDs2MapCodes != null && JunQiManager.LingDiIDs2MapCodes.Length == 7)
				{
					if (LingDiZhanStates.None == JunQiManager.LingDiZhanState)
					{
						if (JunQiManager.IsInLingDiZhanFightingTime())
						{
							JunQiManager.LingDiZhanResultsDict.Clear();
							JunQiManager.LingDiZhanState = LingDiZhanStates.Fighting;
							for (int i = 3; i <= 3; i++)
							{
								JunQiManager.NotifyAllLingDiMapInfoData(JunQiManager.LingDiIDs2MapCodes[i - 1]);
							}
						}
					}
					else if (JunQiManager.IsInLingDiZhanFightingTime())
					{
						if (JunQiManager.IsInLingDiZhanEndTime())
						{
							for (int i = 3; i <= 3; i++)
							{
								if (!JunQiManager.JugeLingDiZhanEndByMapCode(JunQiManager.LingDiIDs2MapCodes[i - 1]))
								{
									int bhid = 0;
									int totalJunQiNum = JunQiManager.CalcJunQiNumByMapCode(JunQiManager.LingDiIDs2MapCodes[i - 1], out bhid);
									if (totalJunQiNum >= JunQiManager.MaxInstallQiNum)
									{
										JunQiManager.AddLingDiZhanEndResultByMapCode(JunQiManager.LingDiIDs2MapCodes[i - 1], true);
										JunQiManager.HandleLingDiZhanResultByMapCode(i, JunQiManager.LingDiIDs2MapCodes[i - 1], bhid, true, true);
										JunQiManager.ProcessHuangChengFightingEndAwards();
									}
								}
							}
						}
						else
						{
							JunQiManager.ProcessTimeAddRoleExp();
						}
					}
					else
					{
						JunQiManager.LingDiZhanState = LingDiZhanStates.None;
						for (int i = 3; i <= 3; i++)
						{
							if (!JunQiManager.JugeLingDiZhanEndByMapCode(JunQiManager.LingDiIDs2MapCodes[i - 1]))
							{
								int bhid = 0;
								int totalJunQiNum = JunQiManager.CalcJunQiNumByMapCode(JunQiManager.LingDiIDs2MapCodes[i - 1], out bhid);
								JunQiManager.AddLingDiZhanEndResultByMapCode(JunQiManager.LingDiIDs2MapCodes[i - 1], true);
								JunQiManager.HandleLingDiZhanResultByMapCode(i, JunQiManager.LingDiIDs2MapCodes[i - 1], bhid, true, true);
							}
						}
						JunQiManager.ProcessHuangChengFightingEndAwards();
					}
				}
			}
		}

		// Token: 0x06002E8C RID: 11916 RVA: 0x0028A6E0 File Offset: 0x002888E0
		public static void NotifySyncBangHuiLingDiZhanResult(int lingDiID, int mapCode, int bhid, int zoneID, string bhName)
		{
			string gmCmdData = string.Format("-syncldzresult {0} {1} {2} {3} {4}", new object[]
			{
				lingDiID,
				mapCode,
				bhid,
				zoneID,
				bhName
			});
			GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				0,
				"",
				0,
				"",
				0,
				gmCmdData,
				0,
				0,
				GameManager.ServerLineID
			}), null, 0);
		}

		// Token: 0x06002E8D RID: 11917 RVA: 0x0028A7A0 File Offset: 0x002889A0
		public static void HandleLingDiZhanResultByMapCode(int lingDiID, int mapCode, int bhid, bool sendToOtherLine, bool lingDiOkHint = true)
		{
			JunQiItem junQiItem = null;
			if (bhid > 0)
			{
				junQiItem = JunQiManager.FindJunQiByBHID(bhid);
			}
			if (sendToOtherLine)
			{
				Global.UpdateLingDiForBH(lingDiID, bhid);
				JunQiManager.ProcessDelAllJunQiByMapCode(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, mapCode);
				if (null != junQiItem)
				{
					JunQiManager.ProcessAllNewJunQiByMapCode(mapCode, bhid, junQiItem.ZoneID, junQiItem.BHName);
				}
			}
			if (null != junQiItem)
			{
				BangHuiLingDiItemData bangHuiLingDiItemData = JunQiManager.GetItemByLingDiID(lingDiID);
				if (null != bangHuiLingDiItemData)
				{
					JunQiManager.InstallJunQiOnNormalMap(lingDiID, bhid, junQiItem.ZoneID, junQiItem.BHName, true);
				}
			}
			else
			{
				JunQiManager.ClearJunQiOnNormalMap(lingDiID);
			}
			if (sendToOtherLine)
			{
				if (null != junQiItem)
				{
					JunQiManager.NotifySyncBangHuiLingDiZhanResult(lingDiID, mapCode, bhid, junQiItem.ZoneID, junQiItem.BHName);
				}
			}
			JunQiManager.NotifyAllLingDiMapInfoData(mapCode);
			if (lingDiOkHint)
			{
				if (null != junQiItem)
				{
					Global.BroadcastLingDiOkHint(junQiItem.BHName, mapCode);
				}
			}
		}

		// Token: 0x06002E8E RID: 11918 RVA: 0x0028A89C File Offset: 0x00288A9C
		public static void HandleLuoLanChengZhanResult(int lingDiID, int mapCode, int bhid, string bhName, bool sendToOtherLine, bool lingDiOkHint = true)
		{
			Global.UpdateLingDiForBH(lingDiID, bhid);
			JunQiManager.NotifyAllLingDiMapInfoData(mapCode);
			if (lingDiOkHint)
			{
				Global.BroadcastLingDiOkHint(bhName, mapCode);
			}
		}

		// Token: 0x06002E8F RID: 11919 RVA: 0x0028A8CC File Offset: 0x00288ACC
		public static void HandleLingDiZhanResultByMapCode2(int lingDiID, int mapCode, int bhid, int zoneID, string bhName)
		{
			if (bhid > 0)
			{
				BangHuiLingDiItemData bangHuiLingDiItemData = JunQiManager.GetItemByLingDiID(lingDiID);
				if (null != bangHuiLingDiItemData)
				{
					JunQiManager.InstallJunQiOnNormalMap(lingDiID, bhid, zoneID, bhName, true);
				}
			}
			else
			{
				JunQiManager.ClearJunQiOnNormalMap(lingDiID);
			}
			JunQiManager.NotifyAllLingDiMapInfoData(mapCode);
		}

		// Token: 0x06002E90 RID: 11920 RVA: 0x0028A918 File Offset: 0x00288B18
		public static void InitLingDiJunQi()
		{
			if (JunQiManager.LingDiIDs2MapCodes != null && JunQiManager.LingDiIDs2MapCodes.Length == 7)
			{
				if (Global.GetBangHuiFightingLineID() == GameManager.ServerLineID)
				{
					for (int i = 3; i <= 3; i++)
					{
						BangHuiLingDiItemData bangHuiLingDiItemData = JunQiManager.GetItemByLingDiID(i);
						if (null != bangHuiLingDiItemData)
						{
							if (bangHuiLingDiItemData.BHID > 0)
							{
								JunQiManager.ProcessAllNewJunQiByMapCode(JunQiManager.LingDiIDs2MapCodes[i - 1], bangHuiLingDiItemData.BHID, bangHuiLingDiItemData.ZoneID, bangHuiLingDiItemData.BHName);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002E91 RID: 11921 RVA: 0x0028A9B0 File Offset: 0x00288BB0
		public static void InstallJunQiOnNormalMap(int lingDiID, int bhid, int zoneID, string bhName, bool forceClean = true)
		{
			List<int> mapCodesList = new List<int>();
			foreach (int key in GameManager.systemLingQiMapQiZhiMgr.SystemXmlItemDict.Keys)
			{
				SystemXmlItem systemQiZiItem = GameManager.systemLingQiMapQiZhiMgr.SystemXmlItemDict[key];
				if (lingDiID == systemQiZiItem.GetIntValue("LingDiID", -1))
				{
					mapCodesList.Add(key);
				}
			}
			string junQiName = JunQiManager.GetJunQiNameByBHID(bhid);
			int junQiLevel = JunQiManager.GetJunQiLevelByBHID(bhid);
			for (int i = 0; i < mapCodesList.Count; i++)
			{
				if (forceClean)
				{
					JunQiManager.ProcessDelAllJunQiByMapCode(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, mapCodesList[i]);
				}
				JunQiManager.ProcessNewJunQi(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, mapCodesList[i], bhid, zoneID, bhName, -1, junQiName, junQiLevel, SceneUIClasses.Normal);
			}
		}

		// Token: 0x06002E92 RID: 11922 RVA: 0x0028AAD0 File Offset: 0x00288CD0
		public static void ClearJunQiOnNormalMap(int lingDiID)
		{
			List<int> mapCodesList = new List<int>();
			foreach (int key in GameManager.systemLingQiMapQiZhiMgr.SystemXmlItemDict.Keys)
			{
				SystemXmlItem systemQiZiItem = GameManager.systemLingQiMapQiZhiMgr.SystemXmlItemDict[key];
				if (lingDiID == systemQiZiItem.GetIntValue("LingDiID", -1))
				{
					mapCodesList.Add(key);
				}
			}
			for (int i = 0; i < mapCodesList.Count; i++)
			{
				JunQiManager.ProcessDelAllJunQiByMapCode(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, mapCodesList[i]);
			}
		}

		// Token: 0x06002E93 RID: 11923 RVA: 0x0028ABA8 File Offset: 0x00288DA8
		public static void SendClearJunQiCmd(int bhid)
		{
			if (JunQiManager.LingDiIDs2MapCodes != null && JunQiManager.LingDiIDs2MapCodes.Length == 7)
			{
				string gmCmdData = string.Format("-clearmap {0}", bhid);
				GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
				{
					-1,
					"",
					0,
					"",
					0,
					gmCmdData,
					0,
					0,
					-1
				}), null, 0);
			}
		}

		// Token: 0x06002E94 RID: 11924 RVA: 0x0028AC58 File Offset: 0x00288E58
		public static LingDiMapInfoData GetLingDiMapData(GameClient client)
		{
			int lingDiID = JunQiManager.GetLingDiIDBy2MapCode(client.ClientData.MapCode);
			LingDiMapInfoData result;
			if (lingDiID != 3)
			{
				result = null;
			}
			else
			{
				result = JunQiManager.FormatLingDiMapData(client.ClientData.MapCode);
			}
			return result;
		}

		// Token: 0x06002E95 RID: 11925 RVA: 0x0028AC98 File Offset: 0x00288E98
		private static LingDiMapInfoData FormatLingDiMapData(int mapCode)
		{
			DateTime now = TimeUtil.NowDateTime();
			long fightingEndTime = 0L;
			long fightingStartTime = 0L;
			if (JunQiManager.LingDiZhanFightingDayTimes != null && JunQiManager.LingDiZhanFightingDayTimes.Length > 0)
			{
				if (!JunQiManager.JugeLingDiZhanEndByMapCode(mapCode))
				{
					if (LingDiZhanStates.Fighting == JunQiManager.LingDiZhanState)
					{
						DateTime endDateTime = new DateTime(now.Year, now.Month, now.Day, JunQiManager.LingDiZhanFightingDayTimes[0].EndHour, JunQiManager.LingDiZhanFightingDayTimes[0].EndMinute, 0);
						fightingEndTime = endDateTime.Ticks / 10000L;
						DateTime startDateTime = new DateTime(now.Year, now.Month, now.Day, JunQiManager.LingDiZhanFightingDayTimes[0].FromHour, JunQiManager.LingDiZhanFightingDayTimes[0].FromMinute, 0);
						fightingStartTime = startDateTime.Ticks / 10000L;
					}
				}
			}
			LingDiMapInfoData lingDiMapInfoData = new LingDiMapInfoData
			{
				FightingEndTime = fightingEndTime,
				FightingStartTime = fightingStartTime,
				BHNameDict = new Dictionary<int, string>()
			};
			List<int> npcList = JunQiManager.GetQiZuoNPCIDList(mapCode);
			if (null != npcList)
			{
				for (int i = 0; i < npcList.Count; i++)
				{
					lingDiMapInfoData.BHNameDict[npcList[i]] = JunQiManager.GetBHNameByNPCID(npcList[i]);
				}
			}
			return lingDiMapInfoData;
		}

		// Token: 0x06002E96 RID: 11926 RVA: 0x0028AE0C File Offset: 0x0028900C
		public static void NotifyAllLingDiMapInfoData(int mapCode)
		{
			LingDiMapInfoData lingDiMapInfoData = JunQiManager.FormatLingDiMapData(mapCode);
			GameManager.ClientMgr.NotifyAllLingDiMapInfoData(mapCode, lingDiMapInfoData);
		}

		// Token: 0x06002E97 RID: 11927 RVA: 0x0028AE30 File Offset: 0x00289030
		private static void ProcessTimeAddRoleExp()
		{
			long ticks = TimeUtil.NOW();
			if (ticks - JunQiManager.LastAddBangZhanAwardsTicks >= 10000L)
			{
				JunQiManager.LastAddBangZhanAwardsTicks = ticks;
				List<object> objsList = GameManager.ClientMgr.GetMapClients(JunQiManager.LingDiIDs2MapCodes[2]);
				if (null != objsList)
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient c = objsList[i] as GameClient;
						if (c != null)
						{
							BangZhanAwardsMgr.ProcessBangZhanAwards(c);
						}
					}
				}
			}
		}

		// Token: 0x06002E98 RID: 11928 RVA: 0x0028AEC4 File Offset: 0x002890C4
		private static int GetExperienceAwards(GameClient client, bool success)
		{
			int result;
			if (success)
			{
				result = 5000000;
			}
			else
			{
				result = 2500000;
			}
			return result;
		}

		// Token: 0x06002E99 RID: 11929 RVA: 0x0028AEEC File Offset: 0x002890EC
		private static int GetRongYuAwards(GameClient client, bool success)
		{
			int result;
			if (success)
			{
				result = 5000;
			}
			else
			{
				result = 2500;
			}
			return result;
		}

		// Token: 0x06002E9A RID: 11930 RVA: 0x0028AF14 File Offset: 0x00289114
		private static void ProcessRoleExperienceAwards(GameClient client, bool success)
		{
			int experience = JunQiManager.GetExperienceAwards(client, success);
			GameManager.ClientMgr.ProcessRoleExperience(client, (long)experience, true, false, false, "none");
		}

		// Token: 0x06002E9B RID: 11931 RVA: 0x0028AF40 File Offset: 0x00289140
		private static void ProcessRoleBangGongAwards(GameClient client, bool success)
		{
			int rongYu = JunQiManager.GetRongYuAwards(client, success);
			if (rongYu > 0)
			{
				GameManager.ClientMgr.ModifyRongYuValue(client, rongYu, true, true);
				GameManager.SystemServerEvents.AddEvent(string.Format("角色获取荣誉, roleID={0}({1}), BangGong={2}, newBangGong={3}", new object[]
				{
					client.ClientData.RoleID,
					client.ClientData.RoleName,
					client.ClientData.BangGong,
					rongYu
				}), EventLevels.Record);
			}
		}

		// Token: 0x06002E9C RID: 11932 RVA: 0x0028AFD0 File Offset: 0x002891D0
		private static bool CanGetAWards(GameClient client, long nowTicks)
		{
			return nowTicks - client.ClientData.EnterMapTicks >= (long)JunQiManager.MaxHavingAwardsSecs;
		}

		// Token: 0x06002E9D RID: 11933 RVA: 0x0028B004 File Offset: 0x00289204
		private static void ProcessHuangChengFightingEndAwards()
		{
			List<object> objsList = GameManager.ClientMgr.GetMapClients(JunQiManager.LingDiIDs2MapCodes[2]);
			if (null != objsList)
			{
				int successBHID = -1;
				int lingDiID = 3;
				if (lingDiID > 0)
				{
					BangHuiLingDiItemData bangHuiLingDiItemData = JunQiManager.GetItemByLingDiID(lingDiID);
					if (bangHuiLingDiItemData != null && bangHuiLingDiItemData.BHID > 0)
					{
						successBHID = bangHuiLingDiItemData.BHID;
					}
				}
				long nowTicks = TimeUtil.NOW();
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i] as GameClient;
					if (c != null)
					{
						if (c.ClientData.CurrentLifeV > 0)
						{
							if (JunQiManager.CanGetAWards(c, nowTicks))
							{
								JunQiManager.ProcessRoleExperienceAwards(c, successBHID == c.ClientData.Faction);
								JunQiManager.ProcessRoleBangGongAwards(c, successBHID == c.ClientData.Faction);
							}
						}
					}
				}
			}
		}

		// Token: 0x04003C35 RID: 15413
		public static object JunQiMutex = new object();

		// Token: 0x04003C36 RID: 15414
		private static Dictionary<string, KillJunQiItem> KillJunQiDict = new Dictionary<string, KillJunQiItem>();

		// Token: 0x04003C37 RID: 15415
		private static Dictionary<int, BangHuiJunQiItemData> _BangHuiJunQiItemsDict = null;

		// Token: 0x04003C38 RID: 15416
		private static Dictionary<int, BangHuiLingDiItemData> _BangHuiLingDiItemsDict = null;

		// Token: 0x04003C39 RID: 15417
		private static Dictionary<int, JunQiItem> _NPCID2JunQiDict = new Dictionary<int, JunQiItem>();

		// Token: 0x04003C3A RID: 15418
		private static Dictionary<int, JunQiItem> _ID2JunQiDict = new Dictionary<int, JunQiItem>();

		// Token: 0x04003C3B RID: 15419
		private static int MaxInstallQiNum = 3;

		// Token: 0x04003C3C RID: 15420
		private static int[] LingDiZhanWeekDays = null;

		// Token: 0x04003C3D RID: 15421
		private static DateTimeRange[] LingDiZhanFightingDayTimes = null;

		// Token: 0x04003C3E RID: 15422
		private static DateTimeRange[] LingDiZhanEndDayTimes = null;

		// Token: 0x04003C3F RID: 15423
		private static int[] LingDiIDs2MapCodes = null;

		// Token: 0x04003C40 RID: 15424
		public static LingDiZhanStates LingDiZhanState = LingDiZhanStates.None;

		// Token: 0x04003C41 RID: 15425
		private static Dictionary<int, bool> LingDiZhanResultsDict = new Dictionary<int, bool>();

		// Token: 0x04003C42 RID: 15426
		private static long LastAddBangZhanAwardsTicks = 0L;

		// Token: 0x04003C43 RID: 15427
		private static int MaxHavingAwardsSecs = 1200000;
	}
}
