using System;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Activity
{
	// Token: 0x02000106 RID: 262
	internal class JieriPlatChargeKingActHandler : SingletonTemplate<JieriPlatChargeKingActHandler>
	{
		// Token: 0x06000468 RID: 1128 RVA: 0x000234F4 File Offset: 0x000216F4
		public TCPProcessCmdResults ProcGetJieriPlatChargeKingList(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string rankJson = "";
			MySQLConnection conn = null;
			try
			{
				string cmdData = new UTF8Encoding().GetString(data, 0, count);
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				string fromDate = fields[0].Replace('$', ':');
				string toDate = fields[1].Replace('$', ':');
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format("SELECT rank_info FROM t_plat_charge_king_log WHERE start_time='{0}' AND end_time='{1}' ORDER BY Id DESC LIMIT 1", fromDate, toDate);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				if (reader.Read())
				{
					rankJson = Encoding.Default.GetString((byte[])reader["rank_info"]);
				}
				cmd.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<string>(rankJson, pool, nID);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}
	}
}
