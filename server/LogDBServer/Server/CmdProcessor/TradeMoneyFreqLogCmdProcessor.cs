using System;
using System.Text;
using LogDBServer.DB;
using Server.Tools;

namespace LogDBServer.Server.CmdProcessor
{
	// Token: 0x02000028 RID: 40
	public class TradeMoneyFreqLogCmdProcessor : ICmdProcessor
	{
		// Token: 0x060000DD RID: 221 RVA: 0x00006254 File Offset: 0x00004454
		private TradeMoneyFreqLogCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(20001, this);
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00006270 File Offset: 0x00004470
		public static TradeMoneyFreqLogCmdProcessor getInstance()
		{
			return TradeMoneyFreqLogCmdProcessor.instance;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00006288 File Offset: 0x00004488
		public void processCmd(GameServerClient client, byte[] cmdParams, int count)
		{
			string cmdData = null;
			int nID = 20001;
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
			if (fields.Length != 14)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData));
				client.sendCmd(32767, "0");
			}
			else
			{
				DBItemLogWriter.getInstance().insertTradeFreqLog(DBManager.getInstance(), fields);
				string strcmd = string.Format("{0}", 1);
				client.sendCmd(nID, strcmd);
			}
		}

		// Token: 0x04000062 RID: 98
		private static TradeMoneyFreqLogCmdProcessor instance = new TradeMoneyFreqLogCmdProcessor();
	}
}
