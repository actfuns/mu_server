using System;
using System.Collections.Generic;
using GameDBServer.DB;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic.Name
{
	// Token: 0x0200014B RID: 331
	public class NameUsedMgr : SingletonTemplate<NameUsedMgr>
	{
		// Token: 0x060005A2 RID: 1442 RVA: 0x0002FE54 File Offset: 0x0002E054
		private NameUsedMgr()
		{
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x0002FE78 File Offset: 0x0002E078
		public void LoadFromDatabase(DBManager dbMgr)
		{
			MySQLConnection conn = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format("SELECT oldname FROM t_change_name", new object[0]);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				try
				{
					MySQLDataReader reader = cmd.ExecuteReaderEx();
					lock (this.cannotUse)
					{
						this.cannotUse.Clear();
						while (reader.Read())
						{
							string name = reader["oldname"].ToString();
							if (!string.IsNullOrEmpty(name) && !this.cannotUse.Contains(name))
							{
								this.cannotUse.Add(name);
							}
						}
					}
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查询数据库失败: {0}", cmdText), null, true);
				}
				cmd.Dispose();
				cmd = null;
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format("SELECT old_name FROM t_change_name_banghui", new object[0]);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				try
				{
					MySQLDataReader reader = cmd.ExecuteReaderEx();
					lock (this.cannotUse_BangHui)
					{
						this.cannotUse_BangHui.Clear();
						while (reader.Read())
						{
							string name = reader["old_name"].ToString();
							if (!string.IsNullOrEmpty(name) && !this.cannotUse_BangHui.Contains(name))
							{
								this.cannotUse_BangHui.Add(name);
							}
						}
					}
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查询数据库失败: {0}", cmdText), null, true);
				}
				cmd.Dispose();
				cmd = null;
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x00030100 File Offset: 0x0002E300
		public bool AddCannotUse_Ex(string name)
		{
			bool result;
			if (string.IsNullOrEmpty(name))
			{
				result = false;
			}
			else
			{
				lock (this.cannotUse)
				{
					if (this.cannotUse.Contains(name))
					{
						return false;
					}
					this.cannotUse.Add(name);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x00030184 File Offset: 0x0002E384
		public bool DelCannotUse_Ex(string name)
		{
			bool result;
			if (string.IsNullOrEmpty(name))
			{
				result = false;
			}
			else
			{
				lock (this.cannotUse)
				{
					if (this.cannotUse.Contains(name))
					{
						this.cannotUse.Remove(name);
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x00030208 File Offset: 0x0002E408
		public bool AddCannotUse_BangHui_Ex(string name)
		{
			bool result;
			if (string.IsNullOrEmpty(name))
			{
				result = false;
			}
			else
			{
				lock (this.cannotUse_BangHui)
				{
					if (this.cannotUse_BangHui.Contains(name))
					{
						return false;
					}
					this.cannotUse_BangHui.Add(name);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x0003028C File Offset: 0x0002E48C
		public bool DelCannotUse_BangHui_Ex(string name)
		{
			bool result;
			if (string.IsNullOrEmpty(name))
			{
				result = false;
			}
			else
			{
				lock (this.cannotUse_BangHui)
				{
					if (this.cannotUse_BangHui.Contains(name))
					{
						this.cannotUse_BangHui.Remove(name);
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x04000829 RID: 2089
		private HashSet<string> cannotUse = new HashSet<string>();

		// Token: 0x0400082A RID: 2090
		private HashSet<string> cannotUse_BangHui = new HashSet<string>();
	}
}
