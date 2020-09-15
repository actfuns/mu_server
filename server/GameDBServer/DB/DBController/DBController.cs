using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using GameDBServer.Logic;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.DB.DBController
{
	// Token: 0x020000E0 RID: 224
	public abstract class DBController<T>
	{
		// Token: 0x060001D3 RID: 467 RVA: 0x000097FB File Offset: 0x000079FB
		protected DBController()
		{
			this.mapper = new DBMapper(typeof(T));
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x00009830 File Offset: 0x00007A30
		protected T queryForObject(string sql)
		{
			MySQLConnection conn = null;
			T obj = default(T);
			try
			{
				conn = this.dbMgr.DBConns.PopDBConnection();
				MySQLCommand cmd = new MySQLCommand(sql, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				int columnNum = reader.FieldCount;
				if (reader.Read())
				{
					int index = 0;
					for (int i = 0; i < columnNum; i++)
					{
						int _index = index++;
						string columnName = reader.GetName(_index);
						object columnValue = reader.GetValue(_index);
						if (null == obj)
						{
							obj = Activator.CreateInstance<T>();
						}
						this.setValue(obj, columnName, columnValue);
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				cmd.Dispose();
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("查询数据库失败: {0}", sql), null, true);
				return default(T);
			}
			finally
			{
				if (null != conn)
				{
					this.dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return obj;
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00009974 File Offset: 0x00007B74
		protected List<T> queryForList(string sql)
		{
			MySQLConnection conn = null;
			List<T> list = null;
			try
			{
				conn = this.dbMgr.DBConns.PopDBConnection();
				MySQLCommand cmd = new MySQLCommand(sql, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				int columnNum = reader.FieldCount;
				string[] nameArray = new string[columnNum];
				while (reader.Read())
				{
					int index = 0;
					T obj = Activator.CreateInstance<T>();
					for (int i = 0; i < columnNum; i++)
					{
						int _index = index++;
						if (null == nameArray[_index])
						{
							nameArray[_index] = reader.GetName(_index);
						}
						string columnName = nameArray[_index];
						object columnValue = reader.GetValue(_index);
						if (null == list)
						{
							list = new List<T>();
						}
						this.setValue(obj, columnName, columnValue);
					}
					list.Add(obj);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				cmd.Dispose();
			}
			catch (Exception e)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("查询数据库失败: {0},exception:{1}", sql, e), null, true);
				return null;
			}
			finally
			{
				if (null != conn)
				{
					this.dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return list;
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x00009AFC File Offset: 0x00007CFC
		protected Dictionary<object, T> queryForDictionary(string sql, string keyName)
		{
			MySQLConnection conn = null;
			Dictionary<object, T> objDictionary = null;
			try
			{
				conn = this.dbMgr.DBConns.PopDBConnection();
				MySQLCommand cmd = new MySQLCommand(sql, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				int columnNum = reader.FieldCount;
				objDictionary = new Dictionary<object, T>();
				string[] nameArray = new string[columnNum];
				while (reader.Read())
				{
					int index = 0;
					T obj = Activator.CreateInstance<T>();
					object key = null;
					for (int i = 0; i < columnNum; i++)
					{
						int _index = index++;
						if (null == nameArray[_index])
						{
							nameArray[_index] = reader.GetName(_index);
						}
						string columnName = nameArray[_index];
						object columnValue = reader.GetValue(_index);
						this.setValue(obj, columnName, columnValue);
						if (null != key)
						{
							if (keyName.Equals(reader.GetName(_index)) || keyName == reader.GetName(_index))
							{
								key = reader.GetValue(_index);
							}
						}
					}
					if (null != key)
					{
						objDictionary.Add(key, obj);
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				cmd.Dispose();
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("查询数据库失败: {0}", sql), null, true);
				return null;
			}
			finally
			{
				if (null != conn)
				{
					this.dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return objDictionary;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x00009CD4 File Offset: 0x00007ED4
		private void setValue(object obj, string columnName, object columnValue)
		{
			MemberInfo member = this.mapper.getMemberInfo(columnName);
			if (!(null == member))
			{
				MemberTypes memberType = member.MemberType;
				if (memberType != MemberTypes.Field)
				{
					if (memberType == MemberTypes.Property)
					{
						PropertyInfo property = (PropertyInfo)member;
						if (columnValue.GetType().Equals(typeof(DBNull)))
						{
							if (property.PropertyType.Equals(typeof(string)))
							{
								property.SetValue(obj, "", null);
							}
						}
						else
						{
							if (property.PropertyType.Equals(typeof(long)) && columnValue.GetType().Equals(typeof(string)))
							{
								columnValue = Convert.ToInt64(columnValue);
							}
							if (property.PropertyType.Equals(typeof(byte)) && (columnValue.GetType().Equals(typeof(string)) || columnValue.GetType().Equals(typeof(short)) || columnValue.GetType().Equals(typeof(int)) || columnValue.GetType().Equals(typeof(long))))
							{
								columnValue = Convert.ToByte(columnValue);
							}
							if (property.PropertyType.Equals(typeof(string)) && columnValue.GetType().Equals(typeof(byte[])))
							{
								columnValue = Convert.ToString(columnValue);
							}
							property.SetValue(obj, columnValue, null);
						}
					}
				}
				else
				{
					FieldInfo field = (FieldInfo)member;
					if (columnValue.GetType().Equals(typeof(DBNull)))
					{
						if (field.FieldType.Equals(typeof(string)))
						{
							field.SetValue(obj, "");
						}
					}
					else
					{
						if (field.FieldType.Equals(typeof(long)) && columnValue.GetType().Equals(typeof(string)))
						{
							columnValue = Convert.ToInt64(columnValue);
						}
						if (field.FieldType.Equals(typeof(byte)) && (columnValue.GetType().Equals(typeof(string)) || columnValue.GetType().Equals(typeof(short)) || columnValue.GetType().Equals(typeof(int)) || columnValue.GetType().Equals(typeof(long))))
						{
							columnValue = Convert.ToByte(columnValue);
						}
						if (field.FieldType.Equals(typeof(string)) && (columnValue.GetType().Equals(typeof(byte[])) || columnValue.GetType().Equals(typeof(byte[]))))
						{
							byte[] _columnValue = (byte[])columnValue;
							columnValue = Convert.ToString(new UTF8Encoding().GetString(_columnValue, 0, _columnValue.Length));
						}
						field.SetValue(obj, columnValue);
					}
				}
			}
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000A044 File Offset: 0x00008244
		protected int insert(string sql)
		{
			MySQLConnection conn = null;
			int resultCount = -1;
			try
			{
				conn = this.dbMgr.DBConns.PopDBConnection();
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(sql, conn);
				try
				{
					resultCount = cmd.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("向数据库写入数据失败: {0},{1}", sql, ex), null, true);
				}
				cmd.Dispose();
				cmd = null;
			}
			finally
			{
				if (null != conn)
				{
					this.dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return resultCount;
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000A100 File Offset: 0x00008300
		protected int update(string sql)
		{
			MySQLConnection conn = null;
			int resultCount = -1;
			try
			{
				conn = this.dbMgr.DBConns.PopDBConnection();
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(sql, conn);
				try
				{
					resultCount = cmd.ExecuteNonQuery();
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("向数据库更新数据失败: {0}", sql), null, true);
				}
				cmd.Dispose();
				cmd = null;
			}
			finally
			{
				if (null != conn)
				{
					this.dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return resultCount;
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000A1B8 File Offset: 0x000083B8
		protected int delete(string sql)
		{
			MySQLConnection conn = null;
			int resultCount = -1;
			try
			{
				conn = this.dbMgr.DBConns.PopDBConnection();
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(sql, conn);
				try
				{
					resultCount = cmd.ExecuteNonQuery();
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("向数据库删除数据失败: {0}", sql), null, true);
				}
				cmd.Dispose();
				cmd = null;
			}
			finally
			{
				if (null != conn)
				{
					this.dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return resultCount;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000A270 File Offset: 0x00008470
		protected int delete(string sql, object[] param)
		{
			MySQLConnection conn = null;
			int resultCount = -1;
			try
			{
				conn = this.dbMgr.DBConns.PopDBConnection();
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand();
				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = sql;
				try
				{
					resultCount = cmd.ExecuteNonQuery();
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("向数据库删除数据失败: {0}", sql), null, true);
				}
				cmd.Dispose();
				cmd = null;
			}
			finally
			{
				if (null != conn)
				{
					this.dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return resultCount;
		}

		// Token: 0x0400061A RID: 1562
		protected DBManager dbMgr = DBManager.getInstance();

		// Token: 0x0400061B RID: 1563
		private DBMapper mapper = null;
	}
}
