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
	
	public abstract class DBController<T>
	{
		
		protected DBController()
		{
			this.mapper = new DBMapper(typeof(T));
		}

		
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

		
		protected DBManager dbMgr = DBManager.getInstance();

		
		private DBMapper mapper = null;
	}
}
