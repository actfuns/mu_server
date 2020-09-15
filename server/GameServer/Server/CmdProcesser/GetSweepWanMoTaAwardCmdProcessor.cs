using System;
using System.Collections.Generic;
using GameServer.Logic;
using GameServer.Logic.WanMota;
using Server.Data;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x0200089A RID: 2202
	public class GetSweepWanMoTaAwardCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D49 RID: 15689 RVA: 0x00344B7B File Offset: 0x00342D7B
		private GetSweepWanMoTaAwardCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(619, 1, this);
		}

		// Token: 0x06003D4A RID: 15690 RVA: 0x00344B98 File Offset: 0x00342D98
		public static GetSweepWanMoTaAwardCmdProcessor getInstance()
		{
			return GetSweepWanMoTaAwardCmdProcessor.instance;
		}

		// Token: 0x06003D4B RID: 15691 RVA: 0x00344BB0 File Offset: 0x00342DB0
		private int GiveSweepReward(GameClient client)
		{
			int result;
			if (client.ClientData.LayerRewardData == null || client.ClientData.LayerRewardData.WanMoTaLayerRewardList.Count != 1)
			{
				result = -1;
			}
			else
			{
				int nExp = 0;
				int nMoney = 0;
				int nXinHun = 0;
				List<GoodsData> rewardList = null;
				lock (client.ClientData.LayerRewardData)
				{
					nExp = client.ClientData.LayerRewardData.WanMoTaLayerRewardList[0].nExp;
					nMoney = client.ClientData.LayerRewardData.WanMoTaLayerRewardList[0].nMoney;
					nXinHun = client.ClientData.LayerRewardData.WanMoTaLayerRewardList[0].nXinHun;
					rewardList = client.ClientData.LayerRewardData.WanMoTaLayerRewardList[0].sweepAwardGoodsList;
					client.ClientData.LayerRewardData.WanMoTaLayerRewardList.Clear();
				}
				WanMotaCopySceneManager.AddRewardToClient(client, rewardList, nExp, nMoney, nXinHun, "万魔塔扫荡奖励");
				result = 0;
			}
			return result;
		}

		// Token: 0x06003D4C RID: 15692 RVA: 0x00344CE0 File Offset: 0x00342EE0
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int nID = 619;
			int nRoleID = Global.SafeConvertToInt32(cmdParams[0]);
			bool result;
			if (0 != client.ClientData.WanMoTaProp.nSweepLayer)
			{
				string strCmd = string.Format("{0}:{1}", -1, nRoleID);
				client.sendCmd(nID, strCmd, false);
				result = true;
			}
			else if (-1 == WanMoTaDBCommandManager.UpdateSweepAwardDBCommand(client, -1))
			{
				string strCmd = string.Format("{0}:{1}", -1, nRoleID);
				client.sendCmd(nID, strCmd, false);
				result = true;
			}
			else
			{
				client.ClientData.WanMoTaProp.nSweepLayer = -1;
				this.GiveSweepReward(client);
				string strCmd = string.Format("{0}:{1}", 0, nRoleID);
				client.sendCmd(nID, strCmd, false);
				result = true;
			}
			return result;
		}

		// Token: 0x040047AA RID: 18346
		private static GetSweepWanMoTaAwardCmdProcessor instance = new GetSweepWanMoTaAwardCmdProcessor();
	}
}
