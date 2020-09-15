using System;
using System.Collections.Generic;
using GameServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x0200089D RID: 2205
	public class JingJiChallengeInfoCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D56 RID: 15702 RVA: 0x00345074 File Offset: 0x00343274
		private JingJiChallengeInfoCmdProcessor()
		{
		}

		// Token: 0x06003D57 RID: 15703 RVA: 0x00345080 File Offset: 0x00343280
		public static JingJiChallengeInfoCmdProcessor getInstance()
		{
			return JingJiChallengeInfoCmdProcessor.instance;
		}

		// Token: 0x06003D58 RID: 15704 RVA: 0x00345098 File Offset: 0x00343298
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			bool result;
			if (client.ClientData.CurrentLifeV <= 0 || client.ClientData.CurrentAction == 12)
			{
				result = true;
			}
			else
			{
				int pageIndex = Convert.ToInt32(cmdParams[1]);
				int roleId = client.ClientData.RoleID;
				List<JingJiChallengeInfoData> dataList = Global.sendToDB<List<JingJiChallengeInfoData>, byte[]>(10146, DataHelper.ObjectToBytes<int[]>(new int[]
				{
					roleId,
					pageIndex
				}), client.ServerId);
				client.sendCmd<List<JingJiChallengeInfoData>>(582, dataList, false);
				result = true;
			}
			return result;
		}

		// Token: 0x040047AD RID: 18349
		private static JingJiChallengeInfoCmdProcessor instance = new JingJiChallengeInfoCmdProcessor();
	}
}
