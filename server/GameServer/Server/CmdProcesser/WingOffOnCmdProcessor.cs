using System;
using GameServer.Logic;
using GameServer.Logic.MUWings;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008AC RID: 2220
	public class WingOffOnCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D9F RID: 15775 RVA: 0x00349DEF File Offset: 0x00347FEF
		private WingOffOnCmdProcessor()
		{
			WingStarCacheManager.LoadWingStarItems();
			WingPropsCacheManager.LoadWingPropsItems();
			TCPCmdDispatcher.getInstance().registerProcessor(610, 1, this);
		}

		// Token: 0x06003DA0 RID: 15776 RVA: 0x00349E18 File Offset: 0x00348018
		public static WingOffOnCmdProcessor getInstance()
		{
			return WingOffOnCmdProcessor.instance;
		}

		// Token: 0x06003DA1 RID: 15777 RVA: 0x00349E30 File Offset: 0x00348030
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int nID = 610;
			int roleID = Global.SafeConvertToInt32(cmdParams[0]);
			bool result;
			if (null == client.ClientData.MyWingData)
			{
				string strcmd = string.Format("{0}:{1}", -3, roleID);
				client.sendCmd(nID, strcmd, false);
				result = true;
			}
			else
			{
				int iRet = MUWingsManager.WingOnOffDBCommand(client, client.ClientData.MyWingData.DbID, (client.ClientData.MyWingData.Using == 0) ? 1 : 0);
				if (iRet < 0)
				{
					string strcmd = string.Format("{0}:{1}", -3, roleID);
					client.sendCmd(nID, strcmd, false);
				}
				else
				{
					string strcmd = string.Format("{0}:{1}", 0, roleID);
					client.sendCmd(nID, strcmd, false);
					client.ClientData.MyWingData.Using = ((client.ClientData.MyWingData.Using == 0) ? 1 : 0);
					MUWingsManager.UpdateWingDataProps(client, client.ClientData.MyWingData.Using == 1);
					LingYuManager.UpdateLingYuProps(client);
					ZhuLingZhuHunManager.UpdateZhuLingZhuHunProps(client);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					GameManager.ClientMgr.NotifyOthersChangeEquip(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, null, 1, client.ClientData.MyWingData);
					if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriWing))
					{
						client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x040047BD RID: 18365
		private static WingOffOnCmdProcessor instance = new WingOffOnCmdProcessor();
	}
}
