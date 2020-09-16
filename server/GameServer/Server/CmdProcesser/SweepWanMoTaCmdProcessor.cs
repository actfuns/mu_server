using System;
using GameServer.Logic;
using GameServer.Logic.WanMota;

namespace GameServer.Server.CmdProcesser
{
	
	public class SweepWanMoTaCmdProcessor : ICmdProcessor
	{
		
		private SweepWanMoTaCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(616, 2, this);
		}

		
		public static SweepWanMoTaCmdProcessor getInstance()
		{
			return SweepWanMoTaCmdProcessor.instance;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int nID = 616;
			int nRoleID = Global.SafeConvertToInt32(cmdParams[0]);
			int nSweepBeginOrder = Global.SafeConvertToInt32(cmdParams[1]);
			bool result;
			if (client.ClientData.WanMoTaProp.nPassLayerCount < SweepWanMotaManager.nSweepReqMinLayerOrder)
			{
				string strCmd = string.Format("{0}:{1}", -2, nRoleID);
				client.sendCmd(nID, strCmd, false);
				result = true;
			}
			else if (0 == client.ClientData.WanMoTaProp.nSweepLayer)
			{
				string strCmd = string.Format("{0}:{1}", -4, nRoleID);
				client.sendCmd(nID, strCmd, false);
				result = true;
			}
			else
			{
				string strCmd;
				if (client.ClientData.WanMoTaProp.nSweepLayer > 0)
				{
					SweepWanMotaManager.SweepContinue(client);
				}
				else
				{
					if (SweepWanMotaManager.GetSweepCount(client) >= SweepWanMotaManager.nWanMoTaMaxSweepNum)
					{
						strCmd = string.Format("{0}:{1}", -1, nRoleID);
						client.sendCmd(nID, strCmd, false);
						return true;
					}
					SweepWanMotaManager.SweepBegin(client);
				}
				strCmd = string.Format("{0}:{1}", 0, nRoleID);
				client.sendCmd(nID, strCmd, false);
				result = true;
			}
			return result;
		}

		
		private static SweepWanMoTaCmdProcessor instance = new SweepWanMoTaCmdProcessor();
	}
}
