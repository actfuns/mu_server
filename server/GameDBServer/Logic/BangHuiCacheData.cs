using System;
using GameDBServer.DB;
using MySQLDriverCS;

namespace GameDBServer.Logic
{
	// Token: 0x02000116 RID: 278
	public class BangHuiCacheData
	{
		// Token: 0x060004A1 RID: 1185 RVA: 0x00025C2C File Offset: 0x00023E2C
		public bool Query(int bhid)
		{
			this.BhId = bhid;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				using (MySQLDataReader reader = conn.ExecuteReader(string.Format("select bhid,rid,voice from t_banghui where bhid={0}", bhid), new MySQLParameter[0]))
				{
					if (!reader.Read())
					{
						return false;
					}
					this.BhId = Convert.ToInt32(reader[0].ToString());
					this.LeaderId = Convert.ToInt64(reader[1].ToString());
					this.GVoicePrioritys = reader[2].ToString();
				}
			}
			return true;
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x00025D00 File Offset: 0x00023F00
		public bool UpdateGVoicePrioritys(string prioritys)
		{
			this.GVoicePrioritys = prioritys;
			bool result;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string sql = string.Format("update t_banghui set voice='{1}' where bhid={0}", this.BhId, prioritys);
				result = conn.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		// Token: 0x04000778 RID: 1912
		public int BhId;

		// Token: 0x04000779 RID: 1913
		public long LeaderId;

		// Token: 0x0400077A RID: 1914
		public string GVoicePrioritys;
	}
}
