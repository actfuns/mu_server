using System;
using System.Configuration;
using Server.Tools;

namespace Maticsoft.DBUtility
{
	// Token: 0x02000051 RID: 81
	public class PubConstant
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060003BC RID: 956 RVA: 0x00031990 File Offset: 0x0002FB90
		public static string ConnectionString
		{
			get
			{
				string _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
				if (PubConstant._CSE != _connectionString)
				{
					PubConstant._CSE = _connectionString;
					string ConStringEncrypt = ConfigurationManager.AppSettings["ConStringEncrypt"];
					if (ConStringEncrypt != "false")
					{
						_connectionString = StringEncrypt.Decrypt(_connectionString, "eabcix675u49,/", "3&3i4x4^+-0");
					}
					PubConstant._CS = _connectionString;
				}
				return PubConstant._CS;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060003BD RID: 957 RVA: 0x00031A0C File Offset: 0x0002FC0C
		public static string ConnectionLogString
		{
			get
			{
				return ConfigurationManager.AppSettings["ConnectionLogString"];
			}
		}

		// Token: 0x060003BE RID: 958 RVA: 0x00031A30 File Offset: 0x0002FC30
		public static string GetDatabaseName(string dbKey)
		{
			return ConfigurationManager.AppSettings[dbKey];
		}

		// Token: 0x060003BF RID: 959 RVA: 0x00031A50 File Offset: 0x0002FC50
		public static string GetConnectionString(string configName)
		{
			string connectionString = ConfigurationManager.AppSettings[configName];
			string ConStringEncrypt = ConfigurationManager.AppSettings["ConStringEncrypt"];
			if (ConStringEncrypt == "true")
			{
				connectionString = DESEncrypt.Decrypt(connectionString);
			}
			return connectionString;
		}

		// Token: 0x040001F6 RID: 502
		private static string _CSE;

		// Token: 0x040001F7 RID: 503
		private static string _CS;
	}
}
