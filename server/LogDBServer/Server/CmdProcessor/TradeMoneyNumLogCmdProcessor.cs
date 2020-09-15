using System;
using System.Text;
using LogDBServer.DB;
using Server.Tools;

namespace LogDBServer.Server.CmdProcessor
{
	// Token: 0x02000029 RID: 41
	public class TradeMoneyNumLogCmdProcessor : ICmdProcessor
	{
		// Token: 0x060000E1 RID: 225 RVA: 0x00006388 File Offset: 0x00004588
		private TradeMoneyNumLogCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(20002, this);
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000063A4 File Offset: 0x000045A4
		public static TradeMoneyNumLogCmdProcessor getInstance()
		{
			return TradeMoneyNumLogCmdProcessor.instance;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000063BC File Offset: 0x000045BC
		public void processCmd(GameServerClient client, byte[] cmdParams, int count)
		{
			string cmdData = null;
			int nID = 20002;
			try
			{
				cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID));
				client.sendCmd(32767, "0");
				return;
			}
			string[] fields = cmdData.Split(new char[]
			{
				':'
			});
			if (fields.Length != 24)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData));
				client.sendCmd(32767, "0");
			}
			else
			{
				DBItemLogWriter.getInstance().insertTradeNumLog(DBManager.getInstance(), fields);
				string strcmd = string.Format("{0}", 1);
				client.sendCmd(nID, strcmd);
			}
		}

		// Token: 0x04000063 RID: 99
		private static TradeMoneyNumLogCmdProcessor instance = new TradeMoneyNumLogCmdProcessor();
	}
}
