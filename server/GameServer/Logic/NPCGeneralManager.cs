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
	// Token: 0x0200076D RID: 1901
	public class NPCGeneralManager
	{
		// Token: 0x060030E1 RID: 12513 RVA: 0x002B5A38 File Offset: 0x002B3C38
		public static bool ReloadMapNPCRoles(int mapCode)
		{
			string fileName = string.Format("Map/{0}/npcs.xml", mapCode);
			GeneralCachingXmlMgr.Reload(Global.ResPath(fileName));
			GameManager.SystemNPCsMgr.ReloadLoadFromXMlFile();
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			return NPCGeneralManager.LoadMapNPCRoles(mapCode, gameMap);
		}

		// Token: 0x060030E2 RID: 12514 RVA: 0x002B5A8C File Offset: 0x002B3C8C
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

		// Token: 0x060030E3 RID: 12515 RVA: 0x002B5C78 File Offset: 0x002B3E78
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

		// Token: 0x060030E4 RID: 12516 RVA: 0x002B5D1C File Offset: 0x002B3F1C
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

		// Token: 0x060030E5 RID: 12517 RVA: 0x002B5EEC File Offset: 0x002B40EC
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

		// Token: 0x060030E6 RID: 12518 RVA: 0x002B6000 File Offset: 0x002B4200
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

		// Token: 0x060030E7 RID: 12519 RVA: 0x002B60D0 File Offset: 0x002B42D0
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

		// Token: 0x060030E8 RID: 12520 RVA: 0x002B6160 File Offset: 0x002B4360
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

		// Token: 0x060030E9 RID: 12521 RVA: 0x002B6204 File Offset: 0x002B4404
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

		// Token: 0x060030EA RID: 12522 RVA: 0x002B6270 File Offset: 0x002B4470
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

		// Token: 0x04003D6D RID: 15725
		public static Dictionary<string, NPC> ListNpc = new Dictionary<string, NPC>();

		// Token: 0x04003D6E RID: 15726
		public static object mutexAddNPC = new object();
	}
}
