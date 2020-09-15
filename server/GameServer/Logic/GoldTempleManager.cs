using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x020006DA RID: 1754
	public class GoldTempleManager
	{
		// Token: 0x060029DA RID: 10714 RVA: 0x00258F84 File Offset: 0x00257184
		public void HeartBeatGoldtempleScene()
		{
			int nRoleNum = GameManager.ClientMgr.GetMapClientsCount(Data.GoldtempleData.MapID);
			if (nRoleNum > 0)
			{
				List<object> objsList = GameManager.ClientMgr.GetMapClients(Data.GoldtempleData.MapID);
				if (objsList != null)
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient client = objsList[i] as GameClient;
						if (client != null)
						{
							if (client.ClientData.MapCode == Data.GoldtempleData.MapID)
							{
								this.SubDiamond(client);
							}
						}
					}
				}
			}
		}

		// Token: 0x060029DB RID: 10715 RVA: 0x00259034 File Offset: 0x00257234
		public void SubDiamond(GameClient client)
		{
			long lTicks = TimeUtil.NOW();
			if (lTicks >= this.m_SubMoneyTick + (long)this.m_SubMoneyInterval)
			{
				this.m_SubMoneyTick = lTicks;
				if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, Data.GoldtempleData.OneMinuteNeedDiamond, "黄金神庙扣除", true, true, false, DaiBiSySType.None))
				{
					this.KickOutScene(client);
				}
			}
		}

		// Token: 0x060029DC RID: 10716 RVA: 0x002590B0 File Offset: 0x002572B0
		public void KickOutScene(GameClient client)
		{
			int toMapCode = GameManager.MainMapCode;
			int toPosX = -1;
			int toPosY = -1;
			if (MapTypes.Normal == Global.GetMapType(client.ClientData.LastMapCode))
			{
				if (GameManager.BattleMgr.BattleMapCode != client.ClientData.LastMapCode || GameManager.ArenaBattleMgr.BattleMapCode != client.ClientData.LastMapCode)
				{
					toMapCode = client.ClientData.LastMapCode;
					toPosX = client.ClientData.LastPosX;
					toPosY = client.ClientData.LastPosY;
				}
			}
			GameMap gameMap = null;
			if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
			{
				GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, toMapCode, toPosX, toPosY, -1, 0);
			}
		}

		// Token: 0x04003978 RID: 14712
		public int m_SubMoneyInterval = 60000;

		// Token: 0x04003979 RID: 14713
		public long m_SubMoneyTick = 0L;
	}
}
