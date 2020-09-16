using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class LogDBCmdManager
	{
		
		public void AddDBLogInfo(int nGoodDBID, string strObjName, string strFrom, string strCurrEnvName, string strTarEnvName, string strOptType, int nAmount, int nZoneID, string userid, int nSurplus, int serverId, GoodsData goodsData = null)
		{
			if (!("" == strObjName))
			{
				this.AddGameDBLogInfo(nGoodDBID, strObjName, strFrom, strCurrEnvName, strTarEnvName, strOptType, nAmount, nZoneID, userid, nSurplus, serverId);
				int disableDBLog = GameManager.GameConfigMgr.GetGameConfigItemInt("disable-dblog", 0);
				if (disableDBLog <= 0)
				{
					string extData = "";
					if (null != goodsData)
					{
						extData = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", new object[]
						{
							goodsData.ExcellenceInfo,
							goodsData.Forge_level,
							goodsData.AppendPropLev,
							goodsData.Binding,
							goodsData.Lucky,
							Convert.ToBase64String(DataHelper.ObjectToBytes<List<int>>(goodsData.WashProps)),
							Convert.ToBase64String(DataHelper.ObjectToBytes<List<int>>(goodsData.ElementhrtsProps)),
							goodsData.JuHunID
						});
					}
					strFrom = strFrom.Replace(':', '-');
					string strLogInfo = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
					{
						nGoodDBID,
						strObjName,
						strFrom,
						strCurrEnvName,
						strTarEnvName,
						strOptType,
						nAmount,
						nZoneID,
						nSurplus,
						extData
					});
					this.AddDBCmd(20000, strLogInfo, null, serverId);
				}
			}
		}

		
		public void AddMessageLog(int dbid, string strObjName, string strFrom, string strCurrEnvName, string strTarEnvName, string strOptType, int nAmount, int nZoneID, string userid, int nSurplus, int serverId, string extData)
		{
			strFrom = strFrom.Replace(':', '-');
			string strLogInfo = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
			{
				dbid,
				strObjName,
				strFrom,
				strCurrEnvName,
				strTarEnvName,
				strOptType,
				nAmount,
				nZoneID,
				nSurplus,
				extData
			});
			this.AddDBCmd(20000, strLogInfo, null, serverId);
		}

		
		public void AddGameDBLogInfo(int nGoodDBID, string strObjName, string strFrom, string strCurrEnvName, string strTarEnvName, string strOptType, int nAmount, int nZoneID, string userid, int nSurplus, int serverId)
		{
			if (!("钻石" != strObjName))
			{
				strFrom = strFrom.Replace(':', '-');
				string strLogInfo = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
				{
					nGoodDBID,
					strObjName,
					strFrom,
					strCurrEnvName,
					strTarEnvName,
					strOptType,
					nAmount,
					nZoneID,
					userid,
					nSurplus
				});
				Global.ExecuteDBCmd(20000, strLogInfo, serverId);
			}
		}

		
		public void AddTradeNumberInfo(int type, int money, int roleid1, int roleid2, int serverId = 0)
		{
			string today = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
			string strLogInfo = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				type,
				money,
				DataHelper.ConvertToTicks(today),
				GameManager.ServerLineID,
				this.CombClientInfo(roleid1, serverId),
				this.CombClientInfo(roleid2, serverId)
			});
			this.AddDBCmd(20002, strLogInfo, null, serverId);
		}

		
		public void AddTradeFreqInfo(int type, int count, int roleid, int serverId = 0)
		{
			string today = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
			string strLogInfo = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				type,
				count,
				DataHelper.ConvertToTicks(today),
				GameManager.ServerLineID,
				this.CombClientInfo(roleid, serverId)
			});
			this.AddDBCmd(20001, strLogInfo, null, serverId);
		}

		
		public string CombClientInfo(int roleid, int serverId)
		{
			string dbcmd = string.Format("{0}", roleid);
			string[] fields = Global.ExecuteDBCmd(10179, dbcmd, serverId);
			string result;
			if (fields == null || fields.Length != 9)
			{
				GameClient client = GameManager.ClientMgr.FindClient(roleid);
				if (null == client)
				{
					result = "-1:-1:-1:-1:-1:-1:-1:-1:-1:-1";
				}
				else
				{
					int TotalOnlineSecs = client.ClientData.TotalOnlineSecs;
					result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
					{
						client.strUserID,
						roleid,
						client.ClientData.RoleName,
						-1,
						-1,
						client.ClientData.UserMoney,
						TotalOnlineSecs,
						client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level,
						client.ClientData.RegTime,
						Global.GetSocketRemoteIP(client, false)
					});
				}
			}
			else
			{
				GameClient client = GameManager.ClientMgr.FindClient(roleid);
				int TotalOnlineSecs = (client != null) ? client.ClientData.TotalOnlineSecs : Convert.ToInt32(fields[6]);
				result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
				{
					fields[0],
					fields[1],
					fields[2],
					fields[3],
					fields[4],
					fields[5],
					TotalOnlineSecs,
					fields[7],
					fields[8],
					(client != null) ? Global.GetSocketRemoteIP(client, false) : ""
				});
			}
			return result;
		}

		
		private void AddDBCmd(int cmdID, string cmdText, DBCommandEventHandler dbCommandEvent, int serverId)
		{
			DBCommand dbCmd = this._DBCmdPool.Pop();
			if (null == dbCmd)
			{
				dbCmd = new DBCommand();
			}
			dbCmd.DBCommandID = cmdID;
			dbCmd.DBCommandText = cmdText;
			dbCmd.ServerId = serverId;
			if (null != dbCommandEvent)
			{
				dbCmd.DBCommandEvent += dbCommandEvent;
			}
			lock (this._DBCmdQueue)
			{
				this._DBCmdQueue.Enqueue(dbCmd);
			}
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
			bytesData = Global.SendAndRecvData<string>(dbCmd.DBCommandID, dbCmd.DBCommandText, dbCmd.ServerId, 1);
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
			byte[] bytesData = null;
			for (int i = 0; i < dbCmdList.Count; i++)
			{
				TCPProcessCmdResults result = this.DoDBCmd(tcpClientPool, pool, dbCmdList[i], out bytesData);
				if (result == TCPProcessCmdResults.RESULT_FAILED)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("向LogDBServer请求执行命令失败, CMD={0}", (TCPGameServerCmds)dbCmdList[i].DBCommandID), null, true);
				}
				this._DBCmdPool.Push(dbCmdList[i]);
			}
		}

		
		private DBCmdPool _DBCmdPool = new DBCmdPool(2000);

		
		private Queue<DBCommand> _DBCmdQueue = new Queue<DBCommand>(2000);
	}
}
