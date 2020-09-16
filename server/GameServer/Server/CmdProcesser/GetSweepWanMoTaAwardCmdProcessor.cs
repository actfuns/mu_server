using System;
using System.Collections.Generic;
using GameServer.Logic;
using GameServer.Logic.WanMota;
using Server.Data;

namespace GameServer.Server.CmdProcesser
{
	
	public class GetSweepWanMoTaAwardCmdProcessor : ICmdProcessor
	{
		
		private GetSweepWanMoTaAwardCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(619, 1, this);
		}

		
		public static GetSweepWanMoTaAwardCmdProcessor getInstance()
		{
			return GetSweepWanMoTaAwardCmdProcessor.instance;
		}

		
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

		
		private static GetSweepWanMoTaAwardCmdProcessor instance = new GetSweepWanMoTaAwardCmdProcessor();
	}
}
