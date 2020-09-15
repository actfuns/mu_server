using System;
using GameServer.Logic;
using GameServer.Logic.WanMota;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x0200089B RID: 2203
	public class GetWanMoTaDetailCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D4E RID: 15694 RVA: 0x00344DC8 File Offset: 0x00342FC8
		private GetWanMoTaDetailCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(618, 1, this);
		}

		// Token: 0x06003D4F RID: 15695 RVA: 0x00344DE8 File Offset: 0x00342FE8
		public static GetWanMoTaDetailCmdProcessor getInstance()
		{
			return GetWanMoTaDetailCmdProcessor.instance;
		}

		// Token: 0x06003D50 RID: 15696 RVA: 0x00344E00 File Offset: 0x00343000
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int nID = 618;
			int nRoleID = Global.SafeConvertToInt32(cmdParams[0]);
			WanMotaInfo data = WanMotaCopySceneManager.GetWanMoTaDetail(client, false);
			bool result;
			if (null == data)
			{
				string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
				{
					-1,
					nRoleID,
					0,
					0,
					0,
					0,
					0
				});
				client.sendCmd(nID, strCmd, false);
				result = true;
			}
			else
			{
				if (data.nPassLayerCount != client.ClientData.WanMoTaNextLayerOrder)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("角色roleid={0} 万魔塔层数不一致 nPassLayerCount={1}, WanMoTaNextLayerOrder={2}", client.ClientData.RoleID, data.nPassLayerCount, client.ClientData.WanMoTaNextLayerOrder), null, true);
					client.ClientData.WanMoTaNextLayerOrder = data.nPassLayerCount;
					GameManager.ClientMgr.SaveWanMoTaPassLayerValue(client, data.nPassLayerCount, true);
				}
				string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
				{
					0,
					nRoleID,
					data.nPassLayerCount,
					data.nTopPassLayerCount,
					SweepWanMotaManager.GetSweepCount(client),
					data.nSweepLayer,
					WanMotaCopySceneManager.WanmotaIsSweeping(client)
				});
				SingletonTemplate<WanMoTaTopLayerManager>.Instance().CheckNeedUpdate(data.nTopPassLayerCount);
				client.sendCmd(nID, strCmd, false);
				result = true;
			}
			return result;
		}

		// Token: 0x040047AB RID: 18347
		private static GetWanMoTaDetailCmdProcessor instance = new GetWanMoTaDetailCmdProcessor();
	}
}
