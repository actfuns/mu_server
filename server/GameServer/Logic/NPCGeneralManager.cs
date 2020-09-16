using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class NPCGeneralManager
	{
		
		public static bool ReloadMapNPCRoles(int mapCode)
		{
			string fileName = string.Format("Map/{0}/npcs.xml", mapCode);
			GeneralCachingXmlMgr.Reload(Global.ResPath(fileName));
			GameManager.SystemNPCsMgr.ReloadLoadFromXMlFile();
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			return NPCGeneralManager.LoadMapNPCRoles(mapCode, gameMap);
		}

		
		public static bool LoadMapNPCRoles(int mapCode, GameMap gameMap)
		{
			string fileName = string.Format("Map/{0}/npcs.xml", mapCode);
			XElement xml = GeneralCachingXmlMgr.GetXElement(Global.ResPath(fileName));
			bool result;
			if (null == xml)
			{
				result = false;
			}
			else
			{
				IEnumerable<XElement> items = xml.Elements("NPCs").Elements<XElement>();
				foreach (XElement item in items)
				{
					NPC myNpc = new NPC();
					myNpc.NpcID = Convert.ToInt32((string)item.Attribute("Code"));
					myNpc.MapCode = mapCode;
					myNpc.CurrentPos = new Point((double)Convert.ToInt32((string)item.Attribute("X")), (double)Convert.ToInt32((string)item.Attribute("Y")));
					if (item.Attribute("Dir") != null)
					{
						myNpc.CurrentDir = (Dircetions)Global.GetSafeAttributeLong(item, "Dir");
					}
					else
					{
						myNpc.CurrentDir = Dircetions.DR_DOWN;
					}
					myNpc.RoleBufferData = NPCGeneralManager.GenerateNpcRoleBufferData(myNpc);
					if (null != myNpc.RoleBufferData)
					{
						NPCGeneralManager.AddNpcToMap(myNpc);
						int safeGridNum = 2;
						SystemXmlItem npcXmlItem;
						if (GameManager.SystemNPCsMgr.SystemXmlItemDict.TryGetValue(myNpc.NpcID, out npcXmlItem))
						{
							safeGridNum = npcXmlItem.GetIntValue("IsSafe", -1);
						}
						if (safeGridNum > 0)
						{
							gameMap.SetPartialSafeRegion(myNpc.GridPoint, safeGridNum);
						}
					}
				}
				result = true;
			}
			return result;
		}

		
		public static NPC GetNPCFromConfig(int mapCode, int npcID, int toX, int toY, int dir)
		{
			SystemXmlItem systemNPCItem = null;
			NPC result;
			if (!GameManager.SystemNPCsMgr.SystemXmlItemDict.TryGetValue(npcID, out systemNPCItem))
			{
				result = null;
			}
			else
			{
				GameMap gameMap = null;
				if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
				{
					result = null;
				}
				else
				{
					NPC myNpc = new NPC();
					myNpc.NpcID = npcID;
					myNpc.MapCode = mapCode;
					myNpc.CurrentPos = new Point((double)toX, (double)toY);
					myNpc.CurrentDir = (Dircetions)dir;
					myNpc.RoleBufferData = NPCGeneralManager.GenerateNpcRoleBufferData(myNpc);
					if (null == myNpc.RoleBufferData)
					{
						result = null;
					}
					else
					{
						result = myNpc;
					}
				}
			}
			return result;
		}

		
		public static bool AddNpcToMap(NPC myNpc)
		{
			MapGrid mapGrid;
			lock (GameManager.MapGridMgr.DictGrids)
			{
				mapGrid = GameManager.MapGridMgr.GetMapGrid(myNpc.MapCode);
			}
			bool result;
			if (null == mapGrid)
			{
				result = false;
			}
			else
			{
				lock (NPCGeneralManager.mutexAddNPC)
				{
					string sNpcKey = string.Format("{0}_{1}_{2}", myNpc.MapCode, myNpc.GridPoint.X, myNpc.GridPoint.Y);
					NPC oldNpc = null;
					if (NPCGeneralManager.ListNpc.TryGetValue(sNpcKey, out oldNpc))
					{
						NPCGeneralManager.ListNpc.Remove(sNpcKey);
						mapGrid.RemoveObject(oldNpc);
						LogManager.WriteLog(LogTypes.Error, string.Format("地图{0}的({1}, {2})处旧的NPC被替换", myNpc.MapCode, myNpc.GridPoint.X, myNpc.GridPoint.Y), null, true);
					}
					GameMap gameMap = GameManager.MapMgr.DictMaps[myNpc.MapCode];
					if (mapGrid.MoveObject(-1, -1, (int)((double)gameMap.MapGridWidth * myNpc.GridPoint.X), (int)((double)gameMap.MapGridHeight * myNpc.GridPoint.Y), myNpc))
					{
						NPCGeneralManager.ListNpc.Add(sNpcKey, myNpc);
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		
		public static void RemoveMapNpcs(int mapCode)
		{
			if (mapCode > 0)
			{
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
				if (null != mapGrid)
				{
					List<string> keysToDel = new List<string>();
					foreach (KeyValuePair<string, NPC> item in NPCGeneralManager.ListNpc)
					{
						if (item.Value.MapCode == mapCode)
						{
							mapGrid.RemoveObject(item.Value);
							keysToDel.Add(item.Key);
						}
					}
					foreach (string key in keysToDel)
					{
						NPCGeneralManager.ListNpc.Remove(key);
					}
				}
			}
		}

		
		public static void RemoveMapNpc(int mapCode, int npcID)
		{
			if (mapCode > 0)
			{
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
				if (null != mapGrid)
				{
					foreach (KeyValuePair<string, NPC> item in NPCGeneralManager.ListNpc)
					{
						if (item.Value.MapCode == mapCode && item.Value.NpcID == npcID)
						{
							mapGrid.RemoveObject(item.Value);
							NPCGeneralManager.ListNpc.Remove(item.Key);
							break;
						}
					}
				}
			}
		}

		
		public static NPC FindNPC(int mapCode, int npcID)
		{
			foreach (KeyValuePair<string, NPC> item in NPCGeneralManager.ListNpc)
			{
				if (item.Value.MapCode == mapCode && item.Value.NpcID == npcID)
				{
					return item.Value;
				}
			}
			return null;
		}

		
		public static byte[] GenerateNpcRoleBufferData(NPC myNpc)
		{
			SystemXmlItem systemNPC = null;
			byte[] result;
			if (!GameManager.SystemNPCsMgr.SystemXmlItemDict.TryGetValue(myNpc.NpcID, out systemNPC))
			{
				result = null;
			}
			else
			{
				result = DataHelper.ObjectToBytes<NPCRole>(new NPCRole
				{
					NpcID = myNpc.NpcID,
					PosX = (int)myNpc.CurrentPos.X,
					PosY = (int)myNpc.CurrentPos.Y,
					MapCode = myNpc.MapCode,
					Dir = (int)myNpc.CurrentDir,
					RoleString = systemNPC.XMLNode.ToString()
				});
			}
			return result;
		}

		
		public static void SendMySelfNPCs(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					NPC npc = objsList[i] as NPC;
					if (null != npc)
					{
						if (npc.ShowNpc)
						{
							GameManager.ClientMgr.NotifyMySelfNewNPC(sl, pool, client, npc);
						}
					}
				}
			}
		}

		
		public static void DelMySelfNpcs(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					NPC npc = objsList[i] as NPC;
					if (null != npc)
					{
						GameManager.ClientMgr.NotifyMySelfDelNPC(sl, pool, client, npc);
					}
				}
			}
		}

		
		public static Dictionary<string, NPC> ListNpc = new Dictionary<string, NPC>();

		
		public static object mutexAddNPC = new object();
	}
}
