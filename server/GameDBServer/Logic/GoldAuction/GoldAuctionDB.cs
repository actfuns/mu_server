using System;
using System.Collections.Generic;
using GameDBServer.DB;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic.GoldAuction
{
	// Token: 0x0200013D RID: 317
	public class GoldAuctionDB
	{
		// Token: 0x06000545 RID: 1349 RVA: 0x0002C090 File Offset: 0x0002A290
		public static bool Insert(GoldAuctionDBItem Item)
		{
			int ret = -1;
			try
			{
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string strAttackerList = DataHelper.ObjectToHexString<List<AuctionRoleData>>(Item.RoleList);
					string temp = "INSERT INTO t_gold_auction(BuyerData, AuctionTime, AuctionType, AuctionSource, ProductionTime,";
					temp += "StrGoods, BossLife, KillBossRoleID, UpDBWay, UpdateTime, AttackerList)";
					temp += " VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', {10})";
					string cmdText = string.Format(temp, new object[]
					{
						Item.BuyerData.getAttackerValue(),
						Item.AuctionTime,
						Item.AuctionType,
						Item.AuctionSource,
						Item.ProductionTime,
						Item.StrGoods,
						Item.BossLife,
						Item.KillBossRoleID,
						((DBAuctionUpEnum)Item.UpDBWay).ToString(),
						DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
						strAttackerList
					});
					ret = conn.ExecuteNonQuery(cmdText, 0);
					LjlLog.WriteLogFormat(LogTypes.Info, new string[]
					{
						"SQL  Insert ",
						(ret > -1).ToString()
					});
				}
			}
			catch (Exception ex)
			{
				LjlLog.WriteLog(LogTypes.Exception, ex.ToString(), "");
			}
			return ret > -1;
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x0002C228 File Offset: 0x0002A428
		public static bool Update(GoldAuctionDBItem Item)
		{
			int ret = -1;
			try
			{
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText = string.Format("UPDATE t_gold_auction SET BuyerData='{0}', AuctionTime='{1}', AuctionType='{2}', UpDBWay='{3}', UpdateTime='{4}' WHERE AuctionSource='{5}' AND ProductionTime='{6}'", new object[]
					{
						Item.BuyerData.getAttackerValue(),
						Item.AuctionTime,
						Item.AuctionType,
						((DBAuctionUpEnum)Item.UpDBWay).ToString(),
						DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
						Item.AuctionSource,
						Item.ProductionTime
					});
					ret = conn.ExecuteNonQuery(cmdText, 0);
					LjlLog.WriteLogFormat(LogTypes.Info, new string[]
					{
						"金团拍卖行更新",
						(ret > -1).ToString(),
						"  ",
						cmdText
					});
				}
			}
			catch (Exception ex)
			{
				LjlLog.WriteLog(LogTypes.Exception, ex.ToString(), "");
			}
			return ret > -1;
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x0002C35C File Offset: 0x0002A55C
		public static bool DelData(string Sql)
		{
			int ret = -1;
			try
			{
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					ret = conn.ExecuteNonQuery(string.Format("delete from t_gold_auction where {0}", Sql), 0);
					LjlLog.WriteLogFormat(LogTypes.Info, new string[]
					{
						"DelData ",
						(ret > -1).ToString(),
						" ,delete from t_gold_auction where ",
						Sql
					});
				}
			}
			catch (Exception ex)
			{
				LjlLog.WriteLog(LogTypes.Exception, ex.ToString(), "");
			}
			return ret > -1;
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x0002C418 File Offset: 0x0002A618
		public static bool Select(out List<GoldAuctionDBItem> dList, int type)
		{
			MySQLConnection conn = null;
			DBManager dbMgr = DBManager.getInstance();
			dList = new List<GoldAuctionDBItem>();
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = "SELECT BuyerData, AuctionTime, AuctionType, AuctionSource, ProductionTime, StrGoods, BossLife, KillBossRoleID, AttackerList FROM t_gold_auction WHERE UpDBWay!='Del' AND AuctionType='{0}';";
				MySQLCommand cmd = new MySQLCommand(string.Format(cmdText, type), conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				while (reader.Read())
				{
					GoldAuctionDBItem AuctionItem = new GoldAuctionDBItem
					{
						AuctionTime = reader["AuctionTime"].ToString(),
						AuctionType = Convert.ToInt32(reader["AuctionType"].ToString()),
						AuctionSource = Convert.ToInt32(reader["AuctionSource"].ToString()),
						ProductionTime = reader["ProductionTime"].ToString(),
						StrGoods = reader["StrGoods"].ToString(),
						BossLife = Convert.ToInt64(reader["BossLife"].ToString()),
						KillBossRoleID = Convert.ToInt32(reader["KillBossRoleID"].ToString())
					};
					string BuyerData = reader["BuyerData"].ToString();
					string[] fields = BuyerData.Split(new char[]
					{
						','
					});
					AuctionItem.BuyerData = new AuctionRoleData();
					if (6 == fields.Length)
					{
						AuctionItem.BuyerData.m_RoleID = Convert.ToInt32(fields[0]);
						AuctionItem.BuyerData.Value = Convert.ToInt64(fields[1]);
						AuctionItem.BuyerData.m_RoleName = fields[2];
						AuctionItem.BuyerData.ZoneID = Convert.ToInt32(fields[3]);
						AuctionItem.BuyerData.strUserID = fields[4];
						AuctionItem.BuyerData.ServerId = Convert.ToInt32(fields[5]);
					}
					AuctionItem.OldAuctionType = AuctionItem.AuctionType;
					byte[] bytes = (reader["AttackerList"] as byte[]) ?? new byte[0];
					AuctionItem.RoleList = DataHelper.BytesToObject<List<AuctionRoleData>>(bytes, 0, bytes.Length);
					if (null == AuctionItem.RoleList)
					{
						AuctionItem.RoleList = new List<AuctionRoleData>();
					}
					dList.Add(AuctionItem);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", string.Format(cmdText, type)), EventLevels.Important);
				cmd.Dispose();
			}
			catch (Exception ex)
			{
				LjlLog.WriteLog(LogTypes.Exception, ex.ToString(), "");
				return false;
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return true;
		}

		// Token: 0x04000808 RID: 2056
		private const string strTime = "yyyy-MM-dd HH:mm:ss";
	}
}
