using System;
using System.Collections.Generic;
using GameServer.Logic;
using GameServer.Logic.BangHui.ZhanMengShiJian;
using Server.Tools;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008B1 RID: 2225
	public class ZhanMengShiJianDetailCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003DB4 RID: 15796 RVA: 0x0034BEC5 File Offset: 0x0034A0C5
		private ZhanMengShiJianDetailCmdProcessor()
		{
		}

		// Token: 0x06003DB5 RID: 15797 RVA: 0x0034BED0 File Offset: 0x0034A0D0
		public static ZhanMengShiJianDetailCmdProcessor getInstance()
		{
			return ZhanMengShiJianDetailCmdProcessor.instance;
		}

		// Token: 0x06003DB6 RID: 15798 RVA: 0x0034BEE8 File Offset: 0x0034A0E8
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int bhId = client.ClientData.Faction;
			int pageIndex = Convert.ToInt32(cmdParams[1]);
			byte[] cmd = DataHelper.ObjectToBytes<int[]>(new int[]
			{
				bhId,
				pageIndex
			});
			List<ZhanMengShiJianData> dataList = Global.sendToDB<List<ZhanMengShiJianData>, byte[]>(10139, cmd, client.ServerId);
			client.sendCmd<List<ZhanMengShiJianData>>(566, dataList, false);
			return true;
		}

		// Token: 0x040047C2 RID: 18370
		private static ZhanMengShiJianDetailCmdProcessor instance = new ZhanMengShiJianDetailCmdProcessor();
	}
}
