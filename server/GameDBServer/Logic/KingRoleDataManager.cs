using System;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x02000148 RID: 328
	public class KingRoleDataManager : SingletonTemplate<KingRoleDataManager>, IManager, ICmdProcessor
	{
		// Token: 0x0600058C RID: 1420 RVA: 0x0002F130 File Offset: 0x0002D330
		private KingRoleDataManager()
		{
		}

		// Token: 0x0600058D RID: 1421 RVA: 0x0002F13C File Offset: 0x0002D33C
		public bool initialize()
		{
			return true;
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x0002F150 File Offset: 0x0002D350
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13230, SingletonTemplate<KingRoleDataManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13231, SingletonTemplate<KingRoleDataManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13232, SingletonTemplate<KingRoleDataManager>.Instance());
			return true;
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x0002F1A4 File Offset: 0x0002D3A4
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x0002F1B8 File Offset: 0x0002D3B8
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x0002F1CC File Offset: 0x0002D3CC
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 13230)
			{
				this.HandleGet(client, nID, cmdParams, count);
			}
			else if (nID == 13231)
			{
				this.HandlePut(client, nID, cmdParams, count);
			}
			else if (nID == 13232)
			{
				this.HandleClr(client, nID, cmdParams, count);
			}
		}

		// Token: 0x06000592 RID: 1426 RVA: 0x0002F238 File Offset: 0x0002D438
		private void HandleClr(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string cmdDatas = new UTF8Encoding().GetString(cmdParams, 0, count);
			string[] fields = cmdDatas.Split(new char[]
			{
				':'
			});
			int kingType = Convert.ToInt32(fields[0]);
			KingRoleGetData data = DataHelper.BytesToObject<KingRoleGetData>(cmdParams, 0, count);
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE FROM t_king_role_data WHERE king_type={0}", kingType);
				client.sendCmd<bool>(nID, conn.ExecuteNonQueryBool(sql, 0));
			}
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x0002F2D4 File Offset: 0x0002D4D4
		private void HandleGet(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			KingRoleGetData data = DataHelper.BytesToObject<KingRoleGetData>(cmdParams, 0, count);
			string sql = string.Format("SELECT roledata_ex FROM t_king_role_data WHERE king_type={0}", data.KingType);
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				MySQLDataReader reader = conn.ExecuteReader(sql, new MySQLParameter[0]);
				RoleDataEx rd = new RoleDataEx
				{
					RoleID = -1
				};
				if (reader.Read())
				{
					string s = new ASCIIEncoding().GetString((byte[])reader["roledata_ex"]);
					byte[] d = Convert.FromBase64String(s);
					rd = DataHelper.BytesToObject<RoleDataEx>(d, 0, d.Length);
				}
				client.sendCmd<RoleDataEx>(nID, rd);
			}
		}

		// Token: 0x06000594 RID: 1428 RVA: 0x0002F3A0 File Offset: 0x0002D5A0
		private void HandlePut(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			KingRolePutData data = DataHelper.BytesToObject<KingRolePutData>(cmdParams, 0, count);
			byte[] d = DataHelper.ObjectToBytes<RoleDataEx>(data.RoleDataEx);
			string s = Convert.ToBase64String(d);
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_king_role_data(king_type,role_id,mod_time,roledata_ex) VALUES({0},{1},'{2}','{3}')", new object[]
				{
					data.KingType,
					data.RoleDataEx.RoleID,
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
					s
				});
				client.sendCmd<bool>(nID, conn.ExecuteNonQueryBool(sql, 0));
			}
		}
	}
}
