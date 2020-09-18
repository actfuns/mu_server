using System;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class KingRoleDataManager : SingletonTemplate<KingRoleDataManager>, IManager, ICmdProcessor
	{
		
		private KingRoleDataManager()
		{
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13230, SingletonTemplate<KingRoleDataManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13231, SingletonTemplate<KingRoleDataManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13232, SingletonTemplate<KingRoleDataManager>.Instance());
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
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
