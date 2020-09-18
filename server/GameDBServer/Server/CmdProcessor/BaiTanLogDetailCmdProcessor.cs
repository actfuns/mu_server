using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.Logic.BaiTan;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	
	public class BaiTanLogDetailCmdProcessor : ICmdProcessor
	{
		
		private BaiTanLogDetailCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(603, this);
		}

		
		public static BaiTanLogDetailCmdProcessor getInstance()
		{
			return BaiTanLogDetailCmdProcessor.instance;
		}

		
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

		
		private static BaiTanLogDetailCmdProcessor instance = new BaiTanLogDetailCmdProcessor();
	}
}
