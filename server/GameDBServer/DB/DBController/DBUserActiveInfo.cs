using System;
using GameDBServer.Data;

namespace GameDBServer.DB.DBController
{
	// Token: 0x020000EF RID: 239
	internal class DBUserActiveInfo : DBController<AccountActiveData>
	{
		// Token: 0x0600041A RID: 1050 RVA: 0x0001FEEA File Offset: 0x0001E0EA
		private DBUserActiveInfo()
		{
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x0001FEF8 File Offset: 0x0001E0F8
		public static DBUserActiveInfo getInstance()
		{
			return DBUserActiveInfo.instance;
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x0001FF10 File Offset: 0x0001E110
		public AccountActiveData GetAccountActiveInfo(DBManager dbMgr, string strAccountID)
		{
			string sql = string.Format("select * from t_user_active_info where Account = '{0}';", strAccountID);
			return base.queryForObject(sql);
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x0001FF38 File Offset: 0x0001E138
		public bool UpdateAccountActiveInfo(DBManager dbMgr, string strAccountID)
		{
			bool ret = false;
			string today = DateTime.Now.ToString("yyyy-MM-dd");
			AccountActiveData dataActive = this.GetAccountActiveInfo(dbMgr, strAccountID);
			if (null == dataActive)
			{
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText = string.Format("INSERT INTO t_user_active_info(Account, createTime, seriesLoginCount, lastSeriesLoginTime) VALUES('{0}', '{1}', {2}, '{3}')", new object[]
					{
						strAccountID,
						today,
						1,
						today
					});
					ret = conn.ExecuteNonQueryBool(cmdText, 0);
				}
			}
			else
			{
				DateTime datePreDay = DateTime.Now.AddDays(-1.0);
				DateTime dateLastLogin = DateTime.Parse(dataActive.strLastSeriesLoginTime + " 00:00:00");
				if (datePreDay.DayOfYear == dateLastLogin.DayOfYear)
				{
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string cmdText = string.Format("UPDATE t_user_active_info SET seriesLoginCount={0}, lastSeriesLoginTime='{1}' WHERE Account='{2}'", dataActive.nSeriesLoginCount + 1, today, dataActive.strAccount);
						ret = conn.ExecuteNonQueryBool(cmdText, 0);
					}
				}
			}
			return ret;
		}

		// Token: 0x040006D1 RID: 1745
		private static DBUserActiveInfo instance = new DBUserActiveInfo();
	}
}
