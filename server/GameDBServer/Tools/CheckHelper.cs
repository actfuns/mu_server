using System;
using System.Text;
using GameDBServer.Server;
using Server.Tools;

namespace GameDBServer.Tools
{
	
	public class CheckHelper
	{
		
		public static bool CheckTCPCmdFields(int nID, byte[] data, int count, out string[] fields, int length)
		{
			string cmdData = null;
			fields = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return false;
			}
			fields = cmdData.Split(new char[]
			{
				':'
			});
			bool result;
			if (fields.Length != length)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0},  Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		
		public static bool CheckTCPCmdFields2(int nID, byte[] data, int count, out string[] fields, int length, char span = '|')
		{
			string cmdData = null;
			fields = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return false;
			}
			fields = cmdData.Split(new char[]
			{
				span
			});
			bool result;
			if (fields.Length != length)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0},  Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
}
