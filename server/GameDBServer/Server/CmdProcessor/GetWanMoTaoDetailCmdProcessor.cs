using System;
using GameDBServer.Logic.WanMoTa;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001ED RID: 493
	public class GetWanMoTaoDetailCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A4B RID: 2635 RVA: 0x00061F73 File Offset: 0x00060173
		private GetWanMoTaoDetailCmdProcessor()
		{
		}

		// Token: 0x06000A4C RID: 2636 RVA: 0x00061F80 File Offset: 0x00060180
		public static GetWanMoTaoDetailCmdProcessor getInstance()
		{
			return GetWanMoTaoDetailCmdProcessor.instance;
		}

		// Token: 0x06000A4D RID: 2637 RVA: 0x00061F98 File Offset: 0x00060198
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			WanMotaInfo data = WanMoTaManager.getInstance().getWanMoTaData(roleId);
			client.sendCmd<WanMotaInfo>(10159, data);
		}

		// Token: 0x04000C4F RID: 3151
		private static GetWanMoTaoDetailCmdProcessor instance = new GetWanMoTaoDetailCmdProcessor();
	}
}
