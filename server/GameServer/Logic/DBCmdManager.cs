using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Server;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class DBCmdManager
	{
		
		public void AddDBCmd(int cmdID, string cmdText, DBCommandEventHandler dbCommandEvent, int serverId)
		{
			Global.ExecuteDBCmd(cmdID, cmdText, serverId);
		}

		
		public int GetDBCmdCount()
		{
			int count;
			lock (this._DBCmdQueue)
			{
				count = this._DBCmdQueue.Count;
			}
			return count;
		}

		
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

		
		private DBCmdPool _DBCmdPool = new DBCmdPool(1000);

		
		private Queue<DBCommand> _DBCmdQueue = new Queue<DBCommand>(1000);
	}
}
