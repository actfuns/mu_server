using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x020001C8 RID: 456
	public class FuBenHistManager
	{
		// Token: 0x0600092E RID: 2350 RVA: 0x00058BB8 File Offset: 0x00056DB8
		public static void LoadFuBenHist(DBManager dbMgr)
		{
			lock (FuBenHistManager._Mutex)
			{
				FuBenHistManager._FuBenHistDict = DBQuery.QueryFuBenHistDict(dbMgr);
			}
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x00058C08 File Offset: 0x00056E08
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

		// Token: 0x06000930 RID: 2352 RVA: 0x00058C6C File Offset: 0x00056E6C
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

		// Token: 0x06000931 RID: 2353 RVA: 0x00058CE4 File Offset: 0x00056EE4
		public static TCPOutPacket GetFuBenHistListData(TCPOutPacketPool pool, int cmdID)
		{
			TCPOutPacket result;
			lock (FuBenHistManager._Mutex)
			{
				result = DataHelper.ObjectToTCPOutPacket<Dictionary<int, FuBenHistData>>(FuBenHistManager._FuBenHistDict, pool, cmdID);
			}
			return result;
		}

		// Token: 0x06000932 RID: 2354 RVA: 0x00058D38 File Offset: 0x00056F38
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

		// Token: 0x04000BB6 RID: 2998
		private static object _Mutex = new object();

		// Token: 0x04000BB7 RID: 2999
		private static Dictionary<int, FuBenHistData> _FuBenHistDict = null;
	}
}
