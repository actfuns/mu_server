using System;
using GameDBServer.DB;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.MerlinMagicBook
{
	
	public class MerlinDBOperate
	{
		
		public static bool InsertMerlinData(DBManager dbMgr, MerlinGrowthSaveDBData merlinData, string addTime)
		{
			bool result;
			if (null == merlinData)
			{
				result = false;
			}
			else
			{
				bool ret = false;
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string endTime = new DateTime(merlinData._ToTicks * 10000L).ToString("yyyy-MM-dd HH:mm:ss");
					string cmdText = string.Format("INSERT INTO t_merlin_magic_book(roleID, occupation, level, level_up_fail_num, starNum, starExp, luckyPoint, toTicks, addTime, activeFrozen, activePalsy, activeSpeedDown, activeBlow, unActiveFrozen, unActivePalsy, unActiveSpeedDown, unActiveBlow) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, '{7}', '{8}', {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16})", new object[]
					{
						merlinData._RoleID,
						merlinData._Occupation,
						merlinData._Level,
						merlinData._LevelUpFailNum,
						merlinData._StarNum,
						merlinData._StarExp,
						merlinData._LuckyPoint,
						endTime,
						addTime,
						merlinData._ActiveAttr[0] * 100.0,
						merlinData._ActiveAttr[1] * 100.0,
						merlinData._ActiveAttr[2] * 100.0,
						merlinData._ActiveAttr[3] * 100.0,
						merlinData._UnActiveAttr[0] * 100.0,
						merlinData._UnActiveAttr[1] * 100.0,
						merlinData._UnActiveAttr[2] * 100.0,
						merlinData._UnActiveAttr[3] * 100.0
					});
					ret = conn.ExecuteNonQueryBool(cmdText, 0);
				}
				result = ret;
			}
			return result;
		}

		
		public static bool UpdateMerlinData(DBManager dbMgr, int nRoleID, string[] fields, int nStartIndex)
		{
			bool result;
			if (fields == null || fields.Length != 15 || nStartIndex >= fields.Length)
			{
				result = false;
			}
			else
			{
				bool ret = false;
				MySQLConnection conn = null;
				try
				{
					conn = dbMgr.DBConns.PopDBConnection();
					if (fields[6] != "*")
					{
						string endTime = new DateTime(Convert.ToInt64(fields[6]) * 10000L).ToString("yyyy-MM-dd HH:mm:ss");
						fields[6] = endTime;
					}
					for (int i = 7; i <= 14; i++)
					{
						if (fields[i] != "*")
						{
							fields[i] = (Convert.ToDouble(fields[i]) * 100.0).ToString();
						}
					}
					string cmdText = DBWriter.FormatUpdateSQL(nRoleID, fields, nStartIndex, MerlinDBOperate.t_fieldNames, "t_merlin_magic_book", MerlinDBOperate.t_fieldTypes, "roleID");
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
					MySQLCommand cmd = new MySQLCommand(cmdText, conn);
					try
					{
						cmd.ExecuteNonQuery();
						ret = true;
					}
					catch (Exception)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText), null, true);
					}
					cmd.Dispose();
					cmd = null;
				}
				finally
				{
					if (null != conn)
					{
						dbMgr.DBConns.PushDBConnection(conn);
					}
				}
				result = ret;
			}
			return result;
		}

		
		public static MerlinGrowthSaveDBData QueryMerlinData(DBManager dbMgr, int nRoleID)
		{
			MySQLConnection conn = null;
			MerlinGrowthSaveDBData MerlinData = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format("SELECT roleID, (SELECT rname  FROM t_roles WHERE rid = roleID ) AS roleName, occupation, level, level_up_fail_num, starNum, starExp, luckyPoint, toTicks, addTime, activeFrozen, activePalsy, activeSpeedDown, activeBlow, unActiveFrozen, unActivePalsy, unActiveSpeedDown, unActiveBlow FROM t_merlin_magic_book WHERE roleID = {0}", nRoleID);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				try
				{
					MySQLDataReader reader = cmd.ExecuteReaderEx();
					if (reader.Read())
					{
						MerlinData = new MerlinGrowthSaveDBData();
						MerlinData._RoleID = Global.SafeConvertToInt32(reader["roleID"].ToString(), 10);
						MerlinData._Occupation = Global.SafeConvertToInt32(reader["occupation"].ToString(), 10);
						MerlinData._Level = Global.SafeConvertToInt32(reader["level"].ToString(), 10);
						MerlinData._LevelUpFailNum = Global.SafeConvertToInt32(reader["level_up_fail_num"].ToString(), 10);
						MerlinData._StarNum = Global.SafeConvertToInt32(reader["starNum"].ToString(), 10);
						MerlinData._StarExp = Global.SafeConvertToInt32(reader["starExp"].ToString(), 10);
						MerlinData._LuckyPoint = Global.SafeConvertToInt32(reader["luckyPoint"].ToString(), 10);
						MerlinData._ToTicks = DataHelper.ConvertToTicks(reader["toTicks"].ToString());
						MerlinData._AddTime = DataHelper.ConvertToTicks(reader["addTime"].ToString());
						MerlinData._ActiveAttr[0] = (double)(Global.SafeConvertToInt32(reader["activeFrozen"].ToString(), 10) / 100);
						MerlinData._ActiveAttr[1] = (double)(Global.SafeConvertToInt32(reader["activePalsy"].ToString(), 10) / 100);
						MerlinData._ActiveAttr[2] = (double)(Global.SafeConvertToInt32(reader["activeSpeedDown"].ToString(), 10) / 100);
						MerlinData._ActiveAttr[3] = (double)(Global.SafeConvertToInt32(reader["activeBlow"].ToString(), 10) / 100);
						MerlinData._UnActiveAttr[0] = (double)(Global.SafeConvertToInt32(reader["unActiveFrozen"].ToString(), 10) / 100);
						MerlinData._UnActiveAttr[1] = (double)(Global.SafeConvertToInt32(reader["unActivePalsy"].ToString(), 10) / 100);
						MerlinData._UnActiveAttr[2] = (double)(Global.SafeConvertToInt32(reader["unActiveSpeedDown"].ToString(), 10) / 100);
						MerlinData._UnActiveAttr[3] = (double)(Global.SafeConvertToInt32(reader["unActiveBlow"].ToString(), 10) / 100);
					}
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查询数据库失败: {0}", cmdText), null, true);
				}
				cmd.Dispose();
				cmd = null;
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return MerlinData;
		}

		
		// Note: this type is marked as 'beforefieldinit'.
		static MerlinDBOperate()
		{
			byte[] array = new byte[14];
			array[5] = 1;
			MerlinDBOperate.t_fieldTypes = array;
		}

		
		private static readonly string[] t_fieldNames = new string[]
		{
			"level",
			"level_up_fail_num",
			"starNum",
			"starExp",
			"luckyPoint",
			"toTicks",
			"activeFrozen",
			"activePalsy",
			"activeSpeedDown",
			"activeBlow",
			"unActiveFrozen",
			"unActivePalsy",
			"unActiveSpeedDown",
			"unActiveBlow"
		};

		
		private static readonly byte[] t_fieldTypes;
	}
}
