using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Core;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic.CoupleArena
{
	// Token: 0x02000128 RID: 296
	internal class CoupleArenaDbManager : SingletonTemplate<CoupleArenaDbManager>, IManager, ICmdProcessor
	{
		// Token: 0x060004E1 RID: 1249 RVA: 0x000281B8 File Offset: 0x000263B8
		public bool initialize()
		{
			return true;
		}

		// Token: 0x060004E2 RID: 1250 RVA: 0x000281CC File Offset: 0x000263CC
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(1382, SingletonTemplate<CoupleArenaDbManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1371, SingletonTemplate<CoupleArenaDbManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1383, SingletonTemplate<CoupleArenaDbManager>.Instance());
			return true;
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x00028220 File Offset: 0x00026420
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060004E4 RID: 1252 RVA: 0x00028234 File Offset: 0x00026434
		public bool destroy()
		{
			return true;
		}

		// Token: 0x060004E5 RID: 1253 RVA: 0x00028248 File Offset: 0x00026448
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 1382)
			{
				this.HandleSaveZhanBao(client, nID, cmdParams, count);
			}
			else if (nID == 1371)
			{
				this.HandleGetZhanBao(client, nID, cmdParams, count);
			}
			else if (nID == 1383)
			{
				this.HandleClrZhanBao(client, nID, cmdParams, count);
			}
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x000282B4 File Offset: 0x000264B4
		private long GetUnionCouple(int a1, int a2)
		{
			int min = Math.Min(a1, a2);
			int max = Math.Max(a1, a2);
			long v = (long)min;
			v <<= 32;
			return v | (long)((ulong)max);
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x000282E4 File Offset: 0x000264E4
		private Queue<CoupleArenaZhanBaoItemData> GetZhanBao(long unionCouple)
		{
			Queue<CoupleArenaZhanBaoItemData> result2;
			lock (this.Mutex)
			{
				Queue<CoupleArenaZhanBaoItemData> result = null;
				if (!this.CoupleZhanBaoDict.TryGetValue(unionCouple, out result))
				{
					result = new Queue<CoupleArenaZhanBaoItemData>();
					MySQLConnection conn = null;
					try
					{
						string sql = string.Format("SELECT `to_man_zoneid`,`to_man_rname`,`to_wife_zoneid`,`to_wife_rname`,`is_win`,`get_jifen` FROM t_couple_arena_zhan_bao WHERE `union_couple`={0} ORDER BY `time` DESC LIMIT {1};", unionCouple, this.MaxZhanBaoNum);
						GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
						conn = DBManager.getInstance().DBConns.PopDBConnection();
						MySQLCommand cmd = new MySQLCommand(sql, conn);
						MySQLDataReader reader = cmd.ExecuteReaderEx();
						List<CoupleArenaZhanBaoItemData> tmpList = new List<CoupleArenaZhanBaoItemData>();
						while (reader != null && reader.Read())
						{
							tmpList.Add(new CoupleArenaZhanBaoItemData
							{
								TargetManZoneId = Convert.ToInt32(reader["to_man_zoneid"].ToString()),
								TargetManRoleName = reader["to_man_rname"].ToString(),
								TargetWifeZoneId = Convert.ToInt32(reader["to_wife_zoneid"].ToString()),
								TargetWifeRoleName = reader["to_wife_rname"].ToString(),
								IsWin = (Convert.ToInt32(reader["is_win"].ToString()) > 0),
								GetJiFen = Convert.ToInt32(reader["get_jifen"].ToString())
							});
						}
						tmpList.Reverse();
						foreach (CoupleArenaZhanBaoItemData item in tmpList)
						{
							result.Enqueue(item);
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(LogTypes.Error, "CoupleArenaDbManager.GetZhanBao " + ex.Message, null, true);
					}
					finally
					{
						if (conn != null)
						{
							DBManager.getInstance().DBConns.PushDBConnection(conn);
						}
					}
					this.CoupleZhanBaoDict[unionCouple] = result;
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x0002857C File Offset: 0x0002677C
		private void HandleGetZhanBao(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					int role = Convert.ToInt32(fields[0]);
					int role2 = Convert.ToInt32(fields[1]);
					long unionCouple = this.GetUnionCouple(role, role2);
					List<CoupleArenaZhanBaoItemData> result = this.GetZhanBao(unionCouple).ToList<CoupleArenaZhanBaoItemData>();
					result.Reverse();
					client.sendCmd<List<CoupleArenaZhanBaoItemData>>(nID, result);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "HandleGetZhanBao failed, " + ex.Message, null, true);
				client.sendCmd(30767, "0");
			}
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x00028678 File Offset: 0x00026878
		private void HandleClrZhanBao(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			MySQLConnection conn = null;
			try
			{
				string cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					int role = Convert.ToInt32(fields[0]);
					int role2 = Convert.ToInt32(fields[1]);
					long unionCouple = this.GetUnionCouple(role, role2);
					string sql = string.Format("DELETE FROM t_couple_arena_zhan_bao WHERE `union_couple`={0};", unionCouple);
					conn = DBManager.getInstance().DBConns.PopDBConnection();
					MySQLCommand cmd = new MySQLCommand(sql, conn);
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
					cmd.ExecuteNonQuery();
					cmd.Dispose();
					lock (this.Mutex)
					{
						this.CoupleZhanBaoDict.Remove(unionCouple);
					}
					client.sendCmd<bool>(nID, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "HandleClrZhanBao failed, " + ex.Message, null, true);
				client.sendCmd<bool>(nID, false);
			}
			finally
			{
				if (null != conn)
				{
					DBManager.getInstance().DBConns.PushDBConnection(conn);
				}
			}
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x0002884C File Offset: 0x00026A4C
		private void HandleSaveZhanBao(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool bResult = false;
			MySQLConnection conn = null;
			try
			{
				CoupleArenaZhanBaoSaveDbData data = DataHelper.BytesToObject<CoupleArenaZhanBaoSaveDbData>(cmdParams, 0, count);
				lock (this.Mutex)
				{
					long union = this.GetUnionCouple(data.FromMan, data.FromWife);
					Queue<CoupleArenaZhanBaoItemData> q = this.GetZhanBao(union);
					string sql = string.Format("INSERT INTO t_couple_arena_zhan_bao(`union_couple`,`man_rid`,`wife_rid`,`to_man_rid`,`to_man_zoneid`,`to_man_rname`,`to_wife_rid`,`to_wife_zoneid`,`to_wife_rname`,`is_win`,`get_jifen`,`week`,`time`) VALUES({0},{1},{2},{3},{4},'{5}',{6},{7},'{8}',{9},{10},{11},'{12}');", new object[]
					{
						union,
						data.FromMan,
						data.FromWife,
						data.ToMan,
						data.ZhanBao.TargetManZoneId,
						data.ZhanBao.TargetManRoleName,
						data.ToWife,
						data.ZhanBao.TargetWifeZoneId,
						data.ZhanBao.TargetWifeRoleName,
						data.ZhanBao.IsWin ? 1 : 0,
						data.ZhanBao.GetJiFen,
						data.FirstWeekday,
						TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss")
					});
					conn = DBManager.getInstance().DBConns.PopDBConnection();
					MySQLCommand cmd = new MySQLCommand(sql, conn);
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
					cmd.ExecuteNonQuery();
					cmd.Dispose();
					q.Enqueue(data.ZhanBao);
					while (q.Count > this.MaxZhanBaoNum)
					{
						q.Dequeue();
					}
					bResult = true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "CoupleArenaDbManager.HandleSaveZhanBao " + ex.Message, null, true);
				bResult = false;
			}
			finally
			{
				if (null != conn)
				{
					DBManager.getInstance().DBConns.PushDBConnection(conn);
				}
			}
			client.sendCmd<bool>(nID, bResult);
		}

		// Token: 0x040007B5 RID: 1973
		private object Mutex = new object();

		// Token: 0x040007B6 RID: 1974
		private readonly int MaxZhanBaoNum = 50;

		// Token: 0x040007B7 RID: 1975
		private Dictionary<long, Queue<CoupleArenaZhanBaoItemData>> CoupleZhanBaoDict = new Dictionary<long, Queue<CoupleArenaZhanBaoItemData>>();
	}
}
