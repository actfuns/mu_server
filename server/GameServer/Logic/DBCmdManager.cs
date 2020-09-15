using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Server;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200061B RID: 1563
	public class DBCmdManager
	{
		// Token: 0x06001FB6 RID: 8118 RVA: 0x001B7674 File Offset: 0x001B5874
		public void AddDBCmd(int cmdID, string cmdText, DBCommandEventHandler dbCommandEvent, int serverId)
		{
			Global.ExecuteDBCmd(cmdID, cmdText, serverId);
		}

		// Token: 0x06001FB7 RID: 8119 RVA: 0x001B7690 File Offset: 0x001B5890
		public int GetDBCmdCount()
		{
			int count;
			lock (this._DBCmdQueue)
			{
				count = this._DBCmdQueue.Count;
			}
			return count;
		}

		// Token: 0x06001FB8 RID: 8120 RVA: 0x001B76E4 File Offset: 0x001B58E4
		private TCPProcessCmdResults DoDBCmd(TCPClientPool tcpClientPool, TCPOutPacketPool pool, DBCommand dbCmd, out byte[] bytesData)
		{
			bytesData = Global.SendAndRecvData<string>(dbCmd.DBCommandID, dbCmd.DBCommandText, dbCmd.ServerId, 0);
			TCPProcessCmdResults result;
			if (bytesData == null || bytesData.Length <= 0)
			{
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				result = TCPProcessCmdResults.RESULT_OK;
			}
			return result;
		}

		// Token: 0x06001FB9 RID: 8121 RVA: 0x001B772C File Offset: 0x001B592C
		public void ExecuteDBCmd(TCPClientPool tcpClientPool, TCPOutPacketPool pool)
		{
			lock (this._DBCmdQueue)
			{
				if (this._DBCmdQueue.Count <= 0)
				{
					return;
				}
			}
			List<DBCommand> dbCmdList = new List<DBCommand>();
			lock (this._DBCmdQueue)
			{
				while (this._DBCmdQueue.Count > 0)
				{
					dbCmdList.Add(this._DBCmdQueue.Dequeue());
				}
			}
			string[] fieldsData = null;
			byte[] bytesData = null;
			for (int i = 0; i < dbCmdList.Count; i++)
			{
				TCPProcessCmdResults result = this.DoDBCmd(tcpClientPool, pool, dbCmdList[i], out bytesData);
				if (result == TCPProcessCmdResults.RESULT_FAILED)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("向DBServer请求执行命令失败, CMD={0}", (TCPGameServerCmds)dbCmdList[i].DBCommandID), null, true);
				}
				else
				{
					int length = BitConverter.ToInt32(bytesData, 0);
					string strData = new UTF8Encoding().GetString(bytesData, 6, length - 2);
					fieldsData = strData.Split(new char[]
					{
						':'
					});
				}
				dbCmdList[i].DoDBCommandEvent(new DBCommandEventArgs
				{
					Result = result,
					fields = fieldsData
				});
				this._DBCmdPool.Push(dbCmdList[i]);
			}
		}

		// Token: 0x04002CBE RID: 11454
		private DBCmdPool _DBCmdPool = new DBCmdPool(1000);

		// Token: 0x04002CBF RID: 11455
		private Queue<DBCommand> _DBCmdQueue = new Queue<DBCommand>(1000);
	}
}
