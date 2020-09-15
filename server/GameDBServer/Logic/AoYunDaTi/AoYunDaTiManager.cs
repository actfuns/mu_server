using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.AoYunDaTi
{
	// Token: 0x02000114 RID: 276
	public class AoYunDaTiManager : SingletonTemplate<AoYunDaTiManager>, IManager, ICmdProcessor
	{
		// Token: 0x0600048C RID: 1164 RVA: 0x00024F84 File Offset: 0x00023184
		public bool initialize()
		{
			return true;
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x00024F98 File Offset: 0x00023198
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(20300, SingletonTemplate<AoYunDaTiManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20301, SingletonTemplate<AoYunDaTiManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20302, SingletonTemplate<AoYunDaTiManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20303, SingletonTemplate<AoYunDaTiManager>.Instance());
			return true;
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x00025000 File Offset: 0x00023200
		public bool showdown()
		{
			return true;
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x00025014 File Offset: 0x00023214
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x00025028 File Offset: 0x00023228
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 20300)
			{
				this.GetAoyunDaTiRoleList(client, nID, cmdParams, count);
			}
			else if (nID == 20301)
			{
				this.GetAoyunLastRankDic(client, nID, cmdParams, count);
			}
			else if (nID == 20302)
			{
				this.SetAoyunLastRankDic(client, nID, cmdParams, count);
			}
			else if (nID == 20303)
			{
				this.CleanAoyunPoint(client, nID, cmdParams, count);
			}
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x000250B0 File Offset: 0x000232B0
		private void GetAoyunDaTiRoleList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			List<AoyunPaiHangRoleData> roleList = new List<AoyunPaiHangRoleData>();
			MySQLConnection conn = null;
			try
			{
				RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType("20008", null);
				string cmdText = string.Format("SELECT * from {0} where pname='{1}'", roleParamType.TableName, "20008");
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				conn = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				while (reader.Read())
				{
					int rid = int.Parse(reader["rid"].ToString());
					int lastPoint = int.Parse(reader["pvalue"].ToString());
					if (lastPoint > 0)
					{
						cmdText = string.Format("select zoneid,rname from t_roles where rid={0}", rid);
						cmd = new MySQLCommand(cmdText, conn);
						MySQLDataReader nameReader = cmd.ExecuteReaderEx();
						if (nameReader.Read())
						{
							int zoneid = Convert.ToInt32(nameReader["zoneid"].ToString());
							string rname = nameReader["rname"].ToString();
							AoyunPaiHangRoleData item = new AoyunPaiHangRoleData
							{
								ZoneId = zoneid,
								RoleId = rid,
								RoleName = rname,
								RolePoint = lastPoint,
								RoleLastPoint = 0
							};
							roleList.Add(item);
						}
					}
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
					DBManager.getInstance().DBConns.PushDBConnection(conn);
				}
			}
			client.sendCmd<List<AoyunPaiHangRoleData>>(nID, roleList);
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x0002529C File Offset: 0x0002349C
		private void GetAoyunLastRankDic(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			Dictionary<int, int> rankDic = new Dictionary<int, int>();
			MySQLConnection conn = null;
			try
			{
				RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType("20009", null);
				string cmdText = string.Format("SELECT * from {0} where pname='{1}'", roleParamType.TableName, "20009");
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				conn = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				while (reader.Read())
				{
					int rid = int.Parse(reader["rid"].ToString());
					int rank = int.Parse(reader["pvalue"].ToString());
					if (rank > 0)
					{
						rankDic[rid] = rank;
					}
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
					DBManager.getInstance().DBConns.PushDBConnection(conn);
				}
			}
			client.sendCmd<Dictionary<int, int>>(nID, rankDic);
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x000253D4 File Offset: 0x000235D4
		private void SetAoyunLastRankDic(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			MySQLConnection conn = null;
			try
			{
				Dictionary<int, int> rankDic = DataHelper.BytesToObject<Dictionary<int, int>>(cmdParams, 0, count);
				RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType("20009", null);
				string cmdText = string.Format("DELETE from {0} where pname='{1}'", roleParamType.TableName, "20009");
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				conn = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				cmd.Dispose();
				string insert = "";
				foreach (KeyValuePair<int, int> item in rankDic)
				{
					insert += string.Format("({0},{1},{2}),", item.Key, "20009", item.Value);
				}
				if (insert != "")
				{
					insert = insert.Substring(0, insert.Length - 1);
					string sql = string.Format("INSERT INTO {0} VALUES {1}", roleParamType.TableName, insert);
					cmd = new MySQLCommand(sql, conn);
					cmd.ExecuteReaderEx();
					cmd.Dispose();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			finally
			{
				if (null != conn)
				{
					DBManager.getInstance().DBConns.PushDBConnection(conn);
				}
			}
			client.sendCmd<int>(nID, 1);
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x000255AC File Offset: 0x000237AC
		private void CleanAoyunPoint(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			MySQLConnection conn = null;
			try
			{
				RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType("20008", null);
				string cmdText = string.Format("DELETE from {0} where pname='{1}'", roleParamType.TableName, "20008");
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				conn = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
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
					DBManager.getInstance().DBConns.PushDBConnection(conn);
				}
			}
			client.sendCmd<int>(nID, 1);
		}
	}
}
