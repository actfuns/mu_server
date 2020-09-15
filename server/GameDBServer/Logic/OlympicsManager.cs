using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Logic.Olympics;
using GameDBServer.Server;
using GameDBServer.Tools;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x02000159 RID: 345
	public class OlympicsManager : SingletonTemplate<OlympicsManager>, IManager, ICmdProcessor
	{
		// Token: 0x060005E5 RID: 1509 RVA: 0x0003482C File Offset: 0x00032A2C
		public bool initialize()
		{
			return true;
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x00034840 File Offset: 0x00032A40
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13124, SingletonTemplate<OlympicsManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13125, SingletonTemplate<OlympicsManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13126, SingletonTemplate<OlympicsManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13127, SingletonTemplate<OlympicsManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13128, SingletonTemplate<OlympicsManager>.Instance());
			return true;
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x000348BC File Offset: 0x00032ABC
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x000348D0 File Offset: 0x00032AD0
		public bool destroy()
		{
			return true;
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x000348E4 File Offset: 0x00032AE4
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			switch (nID)
			{
			case 13124:
				this.OlympicsShopList(client, nID, cmdParams, count);
				break;
			case 13125:
				this.OlympicsShopUpdate(client, nID, cmdParams, count);
				break;
			case 13126:
				this.OlympicsGuess(client, nID, cmdParams, count);
				break;
			case 13127:
				this.OlympicsGuessList(client, nID, cmdParams, count);
				break;
			case 13128:
				this.OlympicsGuessUpdate(client, nID, cmdParams, count);
				break;
			}
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x0003495C File Offset: 0x00032B5C
		private void OlympicsGuess(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string[] fields = null;
			int length = 2;
			if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out fields, length))
			{
				client.sendCmd<bool>(nID, false);
			}
			else
			{
				int roleID = int.Parse(fields[0]);
				int dayID = int.Parse(fields[1]);
				OlympicsGuessDataDB data = new OlympicsGuessDataDB();
				data.RoleID = roleID;
				data.DayID = dayID;
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText = string.Format("SELECT a1,a2,a3,award1,award2,award3 FROM t_olympics_guess WHERE roleID='{0}' and dayID='{1}' ", roleID, dayID);
					MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
					while (reader.Read())
					{
						data.A1 = int.Parse(reader["a1"].ToString());
						data.A2 = int.Parse(reader["a2"].ToString());
						data.A3 = int.Parse(reader["a3"].ToString());
						data.Award1 = int.Parse(reader["award1"].ToString());
						data.Award2 = int.Parse(reader["award2"].ToString());
						data.Award3 = int.Parse(reader["award3"].ToString());
					}
				}
				client.sendCmd<OlympicsGuessDataDB>(nID, data);
			}
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x00034AE4 File Offset: 0x00032CE4
		private void OlympicsGuessList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int roleID = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			List<OlympicsGuessDataDB> list = new List<OlympicsGuessDataDB>();
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("SELECT roleID,dayID,a1,a2,a3,award1,award2,award3 FROM t_olympics_guess WHERE roleID={0} ORDER BY dayID ", roleID);
				MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
				while (reader.Read())
				{
					list.Add(new OlympicsGuessDataDB
					{
						RoleID = int.Parse(reader["roleID"].ToString()),
						DayID = int.Parse(reader["dayID"].ToString()),
						A1 = int.Parse(reader["a1"].ToString()),
						A2 = int.Parse(reader["a2"].ToString()),
						A3 = int.Parse(reader["a3"].ToString()),
						Award1 = int.Parse(reader["award1"].ToString()),
						Award2 = int.Parse(reader["award2"].ToString()),
						Award3 = int.Parse(reader["award3"].ToString())
					});
				}
			}
			client.sendCmd<List<OlympicsGuessDataDB>>(nID, list);
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x00034C74 File Offset: 0x00032E74
		private void OlympicsGuessUpdate(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			OlympicsGuessDataDB data = DataHelper.BytesToObject<OlympicsGuessDataDB>(cmdParams, 0, count);
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_olympics_guess(roleID,dayID,a1,a2,a3,award1,award2,award3) \r\n                                            VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')", new object[]
				{
					data.RoleID,
					data.DayID,
					data.A1,
					data.A2,
					data.A3,
					data.Award1,
					data.Award2,
					data.Award3
				});
				bool isDB = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			client.sendCmd<bool>(nID, true);
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x00034D60 File Offset: 0x00032F60
		private void OlympicsShopList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int dayID = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			Dictionary<int, int> dic = new Dictionary<int, int>();
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("SELECT shopID,count FROM t_olympics_shop WHERE dayID={0} ORDER BY shopID ", dayID);
				MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
				while (reader.Read())
				{
					int shopID = int.Parse(reader["shopID"].ToString());
					int fullCount = int.Parse(reader["count"].ToString());
					if (!dic.ContainsKey(shopID))
					{
						dic.Add(shopID, fullCount);
					}
				}
			}
			client.sendCmd<Dictionary<int, int>>(nID, dic);
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x00034E30 File Offset: 0x00033030
		private void OlympicsShopUpdate(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string[] fields = null;
			int length = 3;
			if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out fields, length))
			{
				client.sendCmd<bool>(nID, false);
			}
			else
			{
				int dayID = int.Parse(fields[0]);
				int shopID = int.Parse(fields[1]);
				int shopCount = int.Parse(fields[2]);
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText = string.Format("REPLACE INTO t_olympics_shop(dayID,shopID,count) VALUES('{0}', '{1}', '{2}')", dayID, shopID, shopCount);
					bool isDB = conn.ExecuteNonQueryBool(cmdText, 0);
				}
				client.sendCmd<bool>(nID, true);
			}
		}
	}
}
