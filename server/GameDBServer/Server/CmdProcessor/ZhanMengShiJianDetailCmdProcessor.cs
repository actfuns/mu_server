using System;
using System.Collections.Generic;
using GameDBServer.Data;
using GameDBServer.Logic;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x02000200 RID: 512
	public class ZhanMengShiJianDetailCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A93 RID: 2707 RVA: 0x000654E0 File Offset: 0x000636E0
		private ZhanMengShiJianDetailCmdProcessor()
		{
		}

		// Token: 0x06000A94 RID: 2708 RVA: 0x000654EC File Offset: 0x000636EC
		public static ZhanMengShiJianDetailCmdProcessor getInstance()
		{
			return ZhanMengShiJianDetailCmdProcessor.instance;
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x00065504 File Offset: 0x00063704
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int[] param = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);
			int bhId = param[0];
			int pageIndex = param[1];
			client.sendCmd<List<ZhanMengShiJianData>>(10139, ZhanMengShiJianManager.getInstance().getDetailByPageIndex(bhId, pageIndex));
		}

		// Token: 0x04000C6F RID: 3183
		private static ZhanMengShiJianDetailCmdProcessor instance = new ZhanMengShiJianDetailCmdProcessor();
	}
}
