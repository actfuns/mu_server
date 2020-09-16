using System;
using MySQLDriverCS;

namespace LogDBServer.DB
{
	
	public class DBManager
	{
		
		private DBManager()
		{
		}

		
		public static DBManager getInstance()
		{
			return DBManager.instance;
		}

		
		
		public DBConnections DBConns
		{
			get
			{
				return this._DBConns;
			}
		}

		
		public int GetMaxConnsCount()
		{
			return this._DBConns.GetDBConnsCount();
		}

		
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

		
		private static DBManager instance = new DBManager();

		
		private DBConnections _DBConns = new DBConnections();
	}
}
