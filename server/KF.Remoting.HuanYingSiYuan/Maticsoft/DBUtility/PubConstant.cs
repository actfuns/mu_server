using System;
using System.Configuration;
using Server.Tools;

namespace Maticsoft.DBUtility
{
	
	public class PubConstant
	{
		
		
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

		
		
		public static string ConnectionLogString
		{
			get
			{
				return ConfigurationManager.AppSettings["ConnectionLogString"];
			}
		}

		
		public static string GetDatabaseName(string dbKey)
		{
			return ConfigurationManager.AppSettings[dbKey];
		}

		
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

		
		private static string _CSE;

		
		private static string _CS;
	}
}
