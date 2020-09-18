using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	public class BossHomeManager
	{
		
		public void HeartBeatBossHomeScene()
		{
			int nRoleNum = GameManager.ClientMgr.GetMapClientsCount(Data.BosshomeData.MapID);
			if (nRoleNum > 0)
			{
				List<object> objsList = GameManager.ClientMgr.GetMapClients(Data.BosshomeData.MapID);
				if (objsList != null)
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient client = objsList[i] as GameClient;
						if (client != null)
						{
							if (client.ClientData.MapCode == Data.BosshomeData.MapID)
							{
								this.SubDiamond(client);
							}
						}
					}
				}
			}
		}

		
		public void SubDiamond(GameClient client)
		{
			if (0 != Data.BosshomeData.OneMinuteNeedDiamond)
			{
				long lTicks = TimeUtil.NOW();
				if (lTicks >= this.m_SubMoneyTick + (long)this.m_SubMoneyInterval)
				{
					this.m_SubMoneyTick = lTicks;
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, Data.BosshomeData.OneMinuteNeedDiamond, "BOSS之家扣除", true, true, false, DaiBiSySType.None))
					{
						this.KickOutScene(client);
					}
				}
			}
		}

		
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

		
		public int m_SubMoneyInterval = 60000;

		
		public long m_SubMoneyTick = 0L;
	}
}
