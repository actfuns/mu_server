using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using Server.Protocol;
using Server.TCP;

namespace GameServer.Logic
{
	// Token: 0x02000623 RID: 1571
	public class DecorationManager
	{
		// Token: 0x06002015 RID: 8213 RVA: 0x001BB6BC File Offset: 0x001B98BC
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

		// Token: 0x06002016 RID: 8214 RVA: 0x001BB7C4 File Offset: 0x001B99C4
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

		// Token: 0x06002017 RID: 8215 RVA: 0x001BB82C File Offset: 0x001B9A2C
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

		// Token: 0x06002018 RID: 8216 RVA: 0x001BB8A0 File Offset: 0x001B9AA0
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

		// Token: 0x06002019 RID: 8217 RVA: 0x001BB944 File Offset: 0x001B9B44
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

		// Token: 0x0600201A RID: 8218 RVA: 0x001BBA20 File Offset: 0x001B9C20
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

		// Token: 0x0600201B RID: 8219 RVA: 0x001BBA80 File Offset: 0x001B9C80
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

		// Token: 0x0600201C RID: 8220 RVA: 0x001BBAE0 File Offset: 0x001B9CE0
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

		// Token: 0x04002CD2 RID: 11474
		public static int AutoDecoID = 1;

		// Token: 0x04002CD3 RID: 11475
		public static Dictionary<int, Decoration> DictDecos = new Dictionary<int, Decoration>();
	}
}
