using System;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;

namespace Tmsk.DbHelper
{
	
	public class MyDataReader
	{
		
		public MyDataReader(MyDbConnection2 conn, MySqlDataReader dataReader)
		{
			this.MyConnection = conn;
			this.DataReader = dataReader;
		}

		
		public MySqlDataReader GetDataReader()
		{
			return this.DataReader;
		}

		
		public object this[int i]
		{
			get
			{
				return this.DataReader[i];
			}
		}

		
		public object this[string name]
		{
			get
			{
				return this.DataReader[name];
			}
		}

		
		public bool Read()
		{
			return this.DataReader.Read();
		}

		
		public bool IsDBNull(int i)
		{
			return this.DataReader.IsDBNull(i);
		}

		
		public int GetOrdinal(string name)
		{
			return this.DataReader.GetOrdinal(name);
		}

		
		public void Close()
		{
			try
			{
				this.DataReader.Close();
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(this.MyConnection);
			}
		}

		
		private MyDbConnection2 MyConnection;

		
		public MySqlDataReader DataReader;
	}
}
