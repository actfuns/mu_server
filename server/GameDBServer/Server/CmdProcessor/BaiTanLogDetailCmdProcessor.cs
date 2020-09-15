using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.Logic.BaiTan;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001EA RID: 490
	public class BaiTanLogDetailCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A38 RID: 2616 RVA: 0x00061980 File Offset: 0x0005FB80
		private BaiTanLogDetailCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(603, this);
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x0006199C File Offset: 0x0005FB9C
		public static BaiTanLogDetailCmdProcessor getInstance()
		{
			return BaiTanLogDetailCmdProcessor.instance;
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x000619B4 File Offset: 0x0005FBB4
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd(30767, "0");
				return;
			}
			List<BaiTanLogItemData> list = new List<BaiTanLogItemData>();
			string[] fields = cmdData.Split(new char[]
			{
				':'
			});
			if (fields.Length != 2)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
				client.sendCmd<List<BaiTanLogItemData>>(30767, list);
			}
			else
			{
				int rid = Convert.ToInt32(fields[0]);
				int pageIndex = Convert.ToInt32(fields[1]);
				client.sendCmd<List<BaiTanLogItemData>>(603, BaiTanManager.getInstance().getDetailByPageIndex(rid, pageIndex));
			}
		}

		// Token: 0x04000C4B RID: 3147
		private static BaiTanLogDetailCmdProcessor instance = new BaiTanLogDetailCmdProcessor();
	}
}
