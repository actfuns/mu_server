using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class FuBenHistManager
	{
		
		public static void LoadFuBenHist(DBManager dbMgr)
		{
			lock (FuBenHistManager._Mutex)
			{
				FuBenHistManager._FuBenHistDict = DBQuery.QueryFuBenHistDict(dbMgr);
			}
		}

		
		public static FuBenHistData FindFuBenHistDataByID(int fuBenID)
		{
			FuBenHistData result;
			lock (FuBenHistManager._Mutex)
			{
				FuBenHistData fuBenHistData = null;
				if (!FuBenHistManager._FuBenHistDict.TryGetValue(fuBenID, out fuBenHistData))
				{
					result = null;
				}
				else
				{
					result = fuBenHistData;
				}
			}
			return result;
		}

		
		public static void AddFuBenHistData(int fuBenID, int roleID, string roleName, int usedSecs)
		{
			FuBenHistData fuBenHistData = new FuBenHistData
			{
				FuBenID = fuBenID,
				RoleID = roleID,
				RoleName = roleName,
				UsedSecs = usedSecs
			};
			lock (FuBenHistManager._Mutex)
			{
				FuBenHistManager._FuBenHistDict[fuBenID] = fuBenHistData;
			}
		}

		
		public static TCPOutPacket GetFuBenHistListData(TCPOutPacketPool pool, int cmdID)
		{
			TCPOutPacket result;
			lock (FuBenHistManager._Mutex)
			{
				result = DataHelper.ObjectToTCPOutPacket<Dictionary<int, FuBenHistData>>(FuBenHistManager._FuBenHistDict, pool, cmdID);
			}
			return result;
		}

		
		public static void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				lock (FuBenHistManager._Mutex)
				{
					if (FuBenHistManager._FuBenHistDict == null)
					{
						return;
					}
					foreach (KeyValuePair<int, FuBenHistData> kvp in FuBenHistManager._FuBenHistDict)
					{
						FuBenHistData data = kvp.Value;
						if (data.RoleID == roleId)
						{
							data.RoleName = newName;
						}
					}
				}
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string sql = string.Format("UPDATE t_fubenhist SET rname='{0}' WHERE rid={1}", newName, roleId);
					conn.ExecuteNonQuery(sql, 0);
				}
			}
		}

		
		private static object _Mutex = new object();

		
		private static Dictionary<int, FuBenHistData> _FuBenHistDict = null;
	}
}
