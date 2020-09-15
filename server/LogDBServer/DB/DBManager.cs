using System;
using MySQLDriverCS;

namespace LogDBServer.DB
{
	// Token: 0x02000011 RID: 17
	public class DBManager
	{
		// Token: 0x0600003E RID: 62 RVA: 0x000037C1 File Offset: 0x000019C1
		private DBManager()
		{
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000037D8 File Offset: 0x000019D8
		public static DBManager getInstance()
		{
			return DBManager.instance;
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000040 RID: 64 RVA: 0x000037F0 File Offset: 0x000019F0
		public DBConnections DBConns
		{
			get
			{
				return this._DBConns;
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003808 File Offset: 0x00001A08
		public int GetMaxConnsCount()
		{
			return this._DBConns.GetDBConnsCount();
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003828 File Offset: 0x00001A28
		public void LoadDatabase(MySQLConnectionString connstr, int MaxConns, int codePage)
		{
			TianMaCharSet.ConvertToCodePage = codePage;
			this._DBConns.BuidConnections(connstr, MaxConns);
			MySQLConnection conn = this._DBConns.PopDBConnection();
			try
			{
			}
			finally
			{
				this._DBConns.PushDBConnection(conn);
			}
		}

		// Token: 0x04000025 RID: 37
		private static DBManager instance = new DBManager();

		// Token: 0x04000026 RID: 38
		private DBConnections _DBConns = new DBConnections();
	}
}
