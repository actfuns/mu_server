using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic.UnionAlly
{
	// Token: 0x02000181 RID: 385
	public class AllyManager : SingletonTemplate<AllyManager>, IManager, ICmdProcessor
	{
		// Token: 0x060006BF RID: 1727 RVA: 0x0003E00C File Offset: 0x0003C20C
		public bool initialize()
		{
			return true;
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x0003E020 File Offset: 0x0003C220
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13122, SingletonTemplate<AllyManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13123, SingletonTemplate<AllyManager>.Instance());
			return true;
		}

		// Token: 0x060006C1 RID: 1729 RVA: 0x0003E060 File Offset: 0x0003C260
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060006C2 RID: 1730 RVA: 0x0003E074 File Offset: 0x0003C274
		public bool destroy()
		{
			return true;
		}

		// Token: 0x060006C3 RID: 1731 RVA: 0x0003E088 File Offset: 0x0003C288
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			switch (nID)
			{
			case 13122:
				this.GetAllyLogData(client, nID, cmdParams, count);
				break;
			case 13123:
				this.AllyLogAdd(client, nID, cmdParams, count);
				break;
			}
		}

		// Token: 0x060006C4 RID: 1732 RVA: 0x0003E0CC File Offset: 0x0003C2CC
		private void GetAllyLogData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			lock (this._lock)
			{
				int unionID = DataHelper.BytesToObject<int>(cmdParams, 0, count);
				if (this._allyLogDic.ContainsKey(unionID))
				{
					client.sendCmd<List<AllyLogData>>(nID, this._allyLogDic[unionID]);
				}
				else
				{
					List<AllyLogData> list = this.GetAllyLogData(unionID);
					this._allyLogDic.Add(unionID, list);
					client.sendCmd<List<AllyLogData>>(nID, list);
				}
			}
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x0003E16C File Offset: 0x0003C36C
		private List<AllyLogData> GetAllyLogData(int unionID)
		{
			List<AllyLogData> result;
			lock (this._lock)
			{
				List<AllyLogData> list = new List<AllyLogData>();
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText = string.Format("SELECT unionID,unionZoneID,unionName,logTime,logState FROM t_ally_log WHERE myUnionID={0} ORDER BY logTime DESC LIMIT 20", unionID);
					MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
					while (reader.Read())
					{
						list.Add(new AllyLogData
						{
							MyUnionID = unionID,
							UnionID = int.Parse(reader["unionID"].ToString()),
							UnionZoneID = int.Parse(reader["unionZoneID"].ToString()),
							UnionName = reader["unionName"].ToString(),
							LogTime = DateTime.Parse(reader["logTime"].ToString()),
							LogState = int.Parse(reader["logState"].ToString())
						});
					}
				}
				result = list;
			}
			return result;
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x0003E2D8 File Offset: 0x0003C4D8
		private void AllyLogAdd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool bResult = false;
			AllyLogData item = DataHelper.BytesToObject<AllyLogData>(cmdParams, 0, count);
			if (item == null)
			{
				client.sendCmd<bool>(nID, bResult);
			}
			else
			{
				lock (this._lock)
				{
					if (!this._allyLogDic.ContainsKey(item.MyUnionID))
					{
						List<AllyLogData> list = this.GetAllyLogData(item.MyUnionID);
						this._allyLogDic.Add(item.MyUnionID, list);
					}
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string cmdText = string.Format("INSERT INTO t_ally_log(myUnionID,unionID,unionZoneID,unionName,logTime,logState) VALUE('{0}','{1}','{2}','{3}','{4}','{5}')", new object[]
						{
							item.MyUnionID,
							item.UnionID,
							item.UnionZoneID,
							item.UnionName,
							item.LogTime,
							item.LogState
						});
						bResult = (conn.ExecuteNonQuery(cmdText, 0) > 0);
					}
					if (bResult && item != null && item.MyUnionID > 0)
					{
						if (this._allyLogDic.ContainsKey(item.MyUnionID))
						{
							List<AllyLogData> list = this._allyLogDic[item.MyUnionID];
							list.Insert(0, item);
							if (list.Count > 20)
							{
								list.RemoveRange(20, list.Count - 20);
							}
						}
						else
						{
							List<AllyLogData> list = this.GetAllyLogData(item.MyUnionID);
							list.Add(item);
							this._allyLogDic.Add(item.MyUnionID, list);
						}
					}
					client.sendCmd<bool>(nID, bResult);
				}
			}
		}

		// Token: 0x040008D8 RID: 2264
		private const int ALLY_LOG_COUNT_MAX = 20;

		// Token: 0x040008D9 RID: 2265
		private object _lock = new object();

		// Token: 0x040008DA RID: 2266
		private Dictionary<int, List<AllyLogData>> _allyLogDic = new Dictionary<int, List<AllyLogData>>();
	}
}
