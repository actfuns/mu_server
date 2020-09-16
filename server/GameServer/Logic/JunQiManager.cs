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
	
	public class JunQiManager
	{
		
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

		
		public static Dictionary<int, BangHuiLingDiItemData> GetBangHuiLingDiItemsDict()
		{
			return JunQiManager._BangHuiLingDiItemsDict;
		}

		
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

		
		public static JunQiItem FindJunQiByNpcID(int npcID)
		{
			JunQiItem JunQiItem = null;
			lock (JunQiManager._NPCID2JunQiDict)
			{
				JunQiManager._NPCID2JunQiDict.TryGetValue(npcID, out JunQiItem);
			}
			return JunQiItem;
		}

		
		public static JunQiItem FindJunQiByID(int JunQiID)
		{
			JunQiItem JunQiItem = null;
			lock (JunQiManager._ID2JunQiDict)
			{
				JunQiManager._ID2JunQiDict.TryGetValue(JunQiID, out JunQiItem);
			}
			return JunQiItem;
		}

		
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

		
		public static void ProcessDelJunQi(SocketListener sl, TCPOutPacketPool pool, int JunQiID)
		{
			JunQiItem JunQiItem = JunQiManager.FindJunQiByID(JunQiID);
			if (null != JunQiItem)
			{
				JunQiManager.RemoveJunQi(JunQiID);
				GameManager.MapGridMgr.DictGrids[JunQiItem.MapCode].RemoveObject(JunQiItem);
			}
		}

		
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

		
		public static void NotifyOthersShowJunQi(SocketListener sl, TCPOutPacketPool pool, JunQiItem JunQiItem)
		{
			if (null != JunQiItem)
			{
				GameManager.MapGridMgr.DictGrids[JunQiItem.MapCode].MoveObject(-1, -1, JunQiItem.PosX, JunQiItem.PosY, JunQiItem);
			}
		}

		
		public static void NotifyOthersHideJunQi(SocketListener sl, TCPOutPacketPool pool, JunQiItem JunQiItem)
		{
			if (null != JunQiItem)
			{
				GameManager.MapGridMgr.DictGrids[JunQiItem.MapCode].RemoveObject(JunQiItem);
			}
		}

		
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

		
		public static void LookupAttackEnemyIDs(IObject attacker, int direction, List<int> enemiesList)
		{
			List<object> objList = new List<object>();
			JunQiManager.LookupAttackEnemies(attacker, direction, objList);
			for (int i = 0; i < objList.Count; i++)
			{
				enemiesList.Add((objList[i] as JunQiItem).JunQiID);
			}
		}

		
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

		
		public static bool CanInstallJunQi(GameClient client)
		{
			return !JunQiManager.JugeLingDiZhanEndByMapCode(client.ClientData.MapCode) && LingDiZhanStates.Fighting == JunQiManager.LingDiZhanState;
		}

		
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

		
		private static void AddLingDiZhanEndResultByMapCode(int mapCode, bool result)
		{
			lock (JunQiManager.LingDiZhanResultsDict)
			{
				JunQiManager.LingDiZhanResultsDict[mapCode] = result;
			}
		}

		
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

		
		public static void HandleLuoLanChengZhanResult(int lingDiID, int mapCode, int bhid, string bhName, bool sendToOtherLine, bool lingDiOkHint = true)
		{
			Global.UpdateLingDiForBH(lingDiID, bhid);
			JunQiManager.NotifyAllLingDiMapInfoData(mapCode);
			if (lingDiOkHint)
			{
				Global.BroadcastLingDiOkHint(bhName, mapCode);
			}
		}

		
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

		
		public static void NotifyAllLingDiMapInfoData(int mapCode)
		{
			LingDiMapInfoData lingDiMapInfoData = JunQiManager.FormatLingDiMapData(mapCode);
			GameManager.ClientMgr.NotifyAllLingDiMapInfoData(mapCode, lingDiMapInfoData);
		}

		
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

		
		private static void ProcessRoleExperienceAwards(GameClient client, bool success)
		{
			int experience = JunQiManager.GetExperienceAwards(client, success);
			GameManager.ClientMgr.ProcessRoleExperience(client, (long)experience, true, false, false, "none");
		}

		
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

		
		private static bool CanGetAWards(GameClient client, long nowTicks)
		{
			return nowTicks - client.ClientData.EnterMapTicks >= (long)JunQiManager.MaxHavingAwardsSecs;
		}

		
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

		
		public static object JunQiMutex = new object();

		
		private static Dictionary<string, KillJunQiItem> KillJunQiDict = new Dictionary<string, KillJunQiItem>();

		
		private static Dictionary<int, BangHuiJunQiItemData> _BangHuiJunQiItemsDict = null;

		
		private static Dictionary<int, BangHuiLingDiItemData> _BangHuiLingDiItemsDict = null;

		
		private static Dictionary<int, JunQiItem> _NPCID2JunQiDict = new Dictionary<int, JunQiItem>();

		
		private static Dictionary<int, JunQiItem> _ID2JunQiDict = new Dictionary<int, JunQiItem>();

		
		private static int MaxInstallQiNum = 3;

		
		private static int[] LingDiZhanWeekDays = null;

		
		private static DateTimeRange[] LingDiZhanFightingDayTimes = null;

		
		private static DateTimeRange[] LingDiZhanEndDayTimes = null;

		
		private static int[] LingDiIDs2MapCodes = null;

		
		public static LingDiZhanStates LingDiZhanState = LingDiZhanStates.None;

		
		private static Dictionary<int, bool> LingDiZhanResultsDict = new Dictionary<int, bool>();

		
		private static long LastAddBangZhanAwardsTicks = 0L;

		
		private static int MaxHavingAwardsSecs = 1200000;
	}
}
