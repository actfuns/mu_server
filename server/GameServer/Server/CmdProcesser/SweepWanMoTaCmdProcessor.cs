using System;
using GameServer.Logic;
using GameServer.Logic.WanMota;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008A9 RID: 2217
	public class SweepWanMoTaCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D95 RID: 15765 RVA: 0x00349ACA File Offset: 0x00347CCA
		private SweepWanMoTaCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(616, 2, this);
		}

		// Token: 0x06003D96 RID: 15766 RVA: 0x00349AE8 File Offset: 0x00347CE8
		public static SweepWanMoTaCmdProcessor getInstance()
		{
			return SweepWanMoTaCmdProcessor.instance;
		}

		// Token: 0x06003D97 RID: 15767 RVA: 0x00349B00 File Offset: 0x00347D00
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

		// Token: 0x040047BA RID: 18362
		private static SweepWanMoTaCmdProcessor instance = new SweepWanMoTaCmdProcessor();
	}
}
