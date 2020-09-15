using System;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001F6 RID: 502
	public class JingJiSaveDataCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A6F RID: 2671 RVA: 0x00062351 File Offset: 0x00060551
		private JingJiSaveDataCmdProcessor()
		{
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x0006235C File Offset: 0x0006055C
		public static JingJiSaveDataCmdProcessor getInstance()
		{
			return JingJiSaveDataCmdProcessor.instance;
		}

		// Token: 0x06000A71 RID: 2673 RVA: 0x00062374 File Offset: 0x00060574
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			JingJiSaveData data = DataHelper.BytesToObject<JingJiSaveData>(cmdParams, 0, count);
			int winCount;
			JingJiChangManager.getInstance().saveData(data, out winCount);
			client.sendCmd<int>(10145, winCount);
		}

		// Token: 0x04000C58 RID: 3160
		private static JingJiSaveDataCmdProcessor instance = new JingJiSaveDataCmdProcessor();
	}
}
