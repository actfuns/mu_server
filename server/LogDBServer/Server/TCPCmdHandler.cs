using System;
using System.Text;
using LogDBServer.DB;
using Server.Protocol;
using Server.Tools;

namespace LogDBServer.Server
{
	
	internal class TCPCmdHandler
	{
		
		public static TCPProcessCmdResults ProcessCmd(GameServerClient client, DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			TCPProcessCmdResults result;
			if (nID != 11001)
			{
				result = TCPCmdDispatcher.getInstance().dispathProcessor(client, nID, data, count);
			}
			else
			{
				result = TCPCmdHandler.ProcessOnlineServerHeartCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
			}
			return result;
		}

		
		private static TCPProcessCmdResults ProcessOnlineServerHeartCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误 , CMD={0}", (TCPGameServerCmds)nID));
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 32767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData));
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 32767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int serverLineID = Convert.ToInt32(fields[0]);
				int serverLineNum = Convert.ToInt32(fields[1]);
				int serverLineCount = Convert.ToInt32(fields[2]);
				string strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 32767);
			return TCPProcessCmdResults.RESULT_DATA;
		}
	}
}
