using System;
using GameServer.Logic;
using Server.Data;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x0200089C RID: 2204
	public class GetWingInfoCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D52 RID: 15698 RVA: 0x00344FC7 File Offset: 0x003431C7
		private GetWingInfoCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(678, 1, this);
		}

		// Token: 0x06003D53 RID: 15699 RVA: 0x00344FE4 File Offset: 0x003431E4
		public static GetWingInfoCmdProcessor getInstance()
		{
			return GetWingInfoCmdProcessor.instance;
		}

		// Token: 0x06003D54 RID: 15700 RVA: 0x00344FFC File Offset: 0x003431FC
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int nID = 678;
			int roleID = Global.SafeConvertToInt32(cmdParams[0]);
			bool result;
			if (null == client.ClientData.MyWingData)
			{
				WingData wingData = new WingData();
				client.sendCmd<WingData>(nID, wingData, false);
				result = true;
			}
			else
			{
				client.sendCmd<WingData>(nID, client.ClientData.MyWingData, false);
				result = true;
			}
			return result;
		}

		// Token: 0x040047AC RID: 18348
		private static GetWingInfoCmdProcessor instance = new GetWingInfoCmdProcessor();
	}
}
