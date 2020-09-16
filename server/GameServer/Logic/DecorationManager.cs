using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using Server.Protocol;
using Server.TCP;

namespace GameServer.Logic
{
	
	public class DecorationManager
	{
		
		public static bool AddDecoToMap(int mapCode, int copyMapID, Point pos, int decoID, int maxLiveTicks, int alphaTicks, bool notifyClients)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			bool result;
			if (null == mapGrid)
			{
				result = false;
			}
			else
			{
				Decoration deco = new Decoration
				{
					AutoID = DecorationManager.AutoDecoID++,
					DecoID = decoID,
					MapCode = mapCode,
					CopyMapID = copyMapID,
					Pos = pos,
					StartTicks = TimeUtil.NOW(),
					MaxLiveTicks = maxLiveTicks,
					AlphaTicks = alphaTicks
				};
				lock (DecorationManager.DictDecos)
				{
					DecorationManager.DictDecos[deco.AutoID] = deco;
				}
				mapGrid.MoveObject(-1, -1, (int)pos.X, (int)pos.Y, deco);
				if (notifyClients)
				{
					DecorationManager.NotifyNearClientsToAddSelf(deco);
				}
				result = false;
			}
			return result;
		}

		
		public static Decoration GetDecoration(int mapCode, int copyMapID, Point pos, int decoID, int maxLiveTicks, int alphaTicks)
		{
			return new Decoration
			{
				AutoID = DecorationManager.AutoDecoID++,
				DecoID = decoID,
				MapCode = mapCode,
				CopyMapID = copyMapID,
				Pos = pos,
				StartTicks = TimeUtil.NOW(),
				MaxLiveTicks = maxLiveTicks,
				AlphaTicks = alphaTicks
			};
		}

		
		public static Decoration FindDeco(int autoID)
		{
			Decoration deco = null;
			lock (DecorationManager.DictDecos)
			{
				if (DecorationManager.DictDecos.TryGetValue(autoID, out deco))
				{
					DecorationManager.DictDecos.Remove(autoID);
				}
			}
			return deco;
		}

		
		public static void RemoveDeco(int autoID)
		{
			Decoration deco = null;
			lock (DecorationManager.DictDecos)
			{
				if (DecorationManager.DictDecos.TryGetValue(autoID, out deco))
				{
					DecorationManager.DictDecos.Remove(autoID);
				}
			}
			if (null != deco)
			{
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[deco.MapCode];
				if (null != mapGrid)
				{
					mapGrid.RemoveObject(deco);
				}
			}
		}

		
		protected static void NotifyNearClientsToAddSelf(Decoration deco)
		{
			List<object> objsList = Global.GetAll9Clients(deco);
			if (null != objsList)
			{
				GameClient client = null;
				for (int i = 0; i < objsList.Count; i++)
				{
					client = (objsList[i] as GameClient);
					if (null != client)
					{
						lock (client.ClientData.VisibleGrid9Objects)
						{
							client.ClientData.VisibleGrid9Objects[client] = 1;
						}
						GameManager.ClientMgr.NotifyMySelfNewDeco(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, deco);
					}
				}
			}
		}

		
		public static void SendMySelfDecos(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					Decoration deco = objsList[i] as Decoration;
					if (null != deco)
					{
						GameManager.ClientMgr.NotifyMySelfNewDeco(sl, pool, client, deco);
					}
				}
			}
		}

		
		public static void DelMySelfDecos(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					Decoration deco = objsList[i] as Decoration;
					if (null != deco)
					{
						GameManager.ClientMgr.NotifyMySelfDelDeco(sl, pool, client, deco);
					}
				}
			}
		}

		
		public static void ProcessAllDecos(SocketListener sl, TCPOutPacketPool pool)
		{
			List<Decoration> decorationList = new List<Decoration>();
			lock (DecorationManager.DictDecos)
			{
				foreach (Decoration val in DecorationManager.DictDecos.Values)
				{
					decorationList.Add(val);
				}
			}
			long nowTicks = TimeUtil.NOW();
			Decoration decoration = null;
			for (int i = 0; i < decorationList.Count; i++)
			{
				decoration = decorationList[i];
				if (decoration.MaxLiveTicks > 0)
				{
					if (nowTicks - decoration.StartTicks >= (long)decoration.MaxLiveTicks)
					{
						lock (DecorationManager.DictDecos)
						{
							DecorationManager.DictDecos.Remove(decoration.AutoID);
						}
						GameManager.MapGridMgr.DictGrids[decoration.MapCode].RemoveObject(decoration);
						List<object> objList = Global.GetAll9Clients(decoration);
						GameManager.ClientMgr.NotifyOthersDelDeco(sl, pool, objList, decoration.MapCode, decoration.AutoID);
					}
				}
			}
		}

		
		public static int AutoDecoID = 1;

		
		public static Dictionary<int, Decoration> DictDecos = new Dictionary<int, Decoration>();
	}
}
