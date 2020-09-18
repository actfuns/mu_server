using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic.UnionAlly
{
	
	public class AllyManager : SingletonTemplate<AllyManager>, IManager, ICmdProcessor
	{
		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13122, SingletonTemplate<AllyManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13123, SingletonTemplate<AllyManager>.Instance());
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

		
		private const int ALLY_LOG_COUNT_MAX = 20;

		
		private object _lock = new object();

		
		private Dictionary<int, List<AllyLogData>> _allyLogDic = new Dictionary<int, List<AllyLogData>>();
	}
}
