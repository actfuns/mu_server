using System;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic.FluorescentGem
{
	
	public class FluorescentGemDBOperate
	{
		
		public static bool EquipFluorescentGem(DBManager dbMgr, FluorescentGemSaveDBData data)
		{
			bool result;
			if (null == data)
			{
				result = false;
			}
			else
			{
				bool ret = false;
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string equipTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					string cmdText = string.Format("INSERT INTO t_fluorescent_gem_equip(roleid, goodsid, position, type, equiptime, bind) VALUES({0}, {1}, {2}, {3}, '{4}', {5})", new object[]
					{
						data._RoleID,
						data._GoodsID,
						data._Position,
						data._GemType,
						equipTime,
						data._Bind
					});
					ret = conn.ExecuteNonQueryBool(cmdText, 0);
				}
				result = ret;
			}
			return result;
		}

		
		public static bool UnEquipFluorescentGem(DBManager dbMgr, FluorescentGemSaveDBData data)
		{
			bool result;
			if (null == data)
			{
				result = false;
			}
			else
			{
				bool ret = false;
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText = string.Format("DELETE FROM t_fluorescent_gem_equip WHERE roleid={0} and position={1} and type={2}", data._RoleID, data._Position, data._GemType);
					ret = conn.ExecuteNonQueryBool(cmdText, 0);
				}
				result = ret;
			}
			return result;
		}

		
		public static void ForceUnEquipFluorescentGem(DBManager dbMgr, ulong id)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE FROM t_fluorescent_gem_equip WHERE id={0}", id);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		
		public static bool UpdateFluorescentPoint(DBManager dbMgr, int nRoleID, int nPoint)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET fluorescent_point={0} where rid={1};", nPoint, nRoleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}
	}
}
