using System;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;

namespace Tmsk.DbHelper
{
	// Token: 0x0200001D RID: 29
	public class MyDataReader
	{
		// Token: 0x060000D1 RID: 209 RVA: 0x0000AE66 File Offset: 0x00009066
		public MyDataReader(MyDbConnection2 conn, MySqlDataReader dataReader)
		{
			this.MyConnection = conn;
			this.DataReader = dataReader;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x0000AE80 File Offset: 0x00009080
		public MySqlDataReader GetDataReader()
		{
			return this.DataReader;
		}

		// Token: 0x17000009 RID: 9
		public object this[int i]
		{
			get
			{
				return this.DataReader[i];
			}
		}

		// Token: 0x1700000A RID: 10
		public object this[string name]
		{
			get
			{
				return this.DataReader[name];
			}
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x0000AED8 File Offset: 0x000090D8
		public bool Read()
		{
			return this.DataReader.Read();
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000AEF8 File Offset: 0x000090F8
		public bool IsDBNull(int i)
		{
			return this.DataReader.IsDBNull(i);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000AF18 File Offset: 0x00009118
		public int GetOrdinal(string name)
		{
			return this.DataReader.GetOrdinal(name);
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x0000AF38 File Offset: 0x00009138
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

		// Token: 0x040000AF RID: 175
		private MyDbConnection2 MyConnection;

		// Token: 0x040000B0 RID: 176
		public MySqlDataReader DataReader;
	}
}
