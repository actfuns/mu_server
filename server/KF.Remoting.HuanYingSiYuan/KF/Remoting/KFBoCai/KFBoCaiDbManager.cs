using System;
using System.Collections.Generic;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting.KFBoCai
{
	// Token: 0x0200002D RID: 45
	public class KFBoCaiDbManager
	{
		// Token: 0x0600020F RID: 527 RVA: 0x0001F76C File Offset: 0x0001D96C
		public static bool InserOpenLottery(OpenLottery data)
		{
			try
			{
				string sql = string.Format("REPLACE INTO t_bocai_open_lottery(DataPeriods, AllBalance, SurplusBalance, XiaoHaoDaiBi, BocaiType, strWinNum, WinInfo) VALUES({0},{1},{2},{3},{4},'{5}','{6}');", new object[]
				{
					data.DataPeriods,
					data.AllBalance,
					data.SurplusBalance,
					data.XiaoHaoDaiBi,
					data.BocaiType,
					data.strWinNum,
					data.WinInfo
				});
				return DbHelperMySQL.ExecuteSql(sql) > -1;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return false;
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0001F81C File Offset: 0x0001DA1C
		public static void SelectOpenLottery(long DataPeriods, int BocaiType, out OpenLottery data)
		{
			data = null;
			MySqlDataReader sdr = null;
			try
			{
				string sql = string.Format("SELECT `SurplusBalance`,`XiaoHaoDaiBi`,`strWinNum`,`WinInfo`,`AllBalance` FROM t_bocai_open_lottery WHERE `BocaiType`={0} AND `DataPeriods`={1};", BocaiType, DataPeriods);
				sdr = DbHelperMySQL.ExecuteReader(sql, false);
				data = new OpenLottery();
				if (sdr != null && sdr.Read())
				{
					data.AllBalance = Convert.ToInt64(sdr["AllBalance"]);
					data.SurplusBalance = Convert.ToInt64(sdr["SurplusBalance"]);
					data.XiaoHaoDaiBi = Convert.ToInt32(sdr["XiaoHaoDaiBi"]);
					data.strWinNum = sdr["strWinNum"].ToString();
					data.WinInfo = sdr["WinInfo"].ToString();
					data.BocaiType = BocaiType;
					data.DataPeriods = DataPeriods;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
			}
			finally
			{
				if (sdr != null)
				{
					sdr.Close();
				}
			}
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0001F940 File Offset: 0x0001DB40
		public static void SelectOpenLottery(int BocaiType, string cmd, out List<OpenLottery> dList)
		{
			dList = null;
			MySqlDataReader sdr = null;
			try
			{
				string sql = string.Format("SELECT `SurplusBalance`,`XiaoHaoDaiBi`,`strWinNum`,`WinInfo`,`AllBalance`,`DataPeriods` FROM t_bocai_open_lottery WHERE `BocaiType`={0}{1}", BocaiType, cmd);
				sdr = DbHelperMySQL.ExecuteReader(sql, false);
				dList = new List<OpenLottery>();
				while (sdr != null && sdr.Read())
				{
					OpenLottery data = new OpenLottery();
					data.DataPeriods = Convert.ToInt64(sdr["DataPeriods"]);
					data.AllBalance = Convert.ToInt64(sdr["AllBalance"]);
					data.SurplusBalance = Convert.ToInt64(sdr["SurplusBalance"]);
					data.XiaoHaoDaiBi = Convert.ToInt32(sdr["XiaoHaoDaiBi"]);
					data.strWinNum = sdr["strWinNum"].ToString();
					data.WinInfo = sdr["WinInfo"].ToString();
					data.BocaiType = BocaiType;
					dList.Add(data);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
			}
			finally
			{
				if (sdr != null)
				{
					sdr.Close();
				}
			}
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0001FA78 File Offset: 0x0001DC78
		public static long GetMaxPeriods(int BocaiType)
		{
			try
			{
				object value = DbHelperMySQL.GetSingle(string.Format("SELECT MAX(DataPeriods) FROM t_bocai_open_lottery WHERE `BocaiType`={0}", BocaiType));
				if (null == value)
				{
					return 0L;
				}
				return (long)value;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return -1L;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0001FAE0 File Offset: 0x0001DCE0
		public static bool InsertLotteryHistory(BoCaiTypeEnum BocaiType, KFBoCaoHistoryData History)
		{
			try
			{
				string sql = string.Format("REPLACE INTO t_bocai_lottery_history(`rid`, `BocaiType`, `DataPeriods`, `ServerID`, `RoleName`, `ZoneID`, `BuyNum`, `WinNo`, `WinMoney`)VALUES({0},{1},{2},{3},'{4}',{5},{6},{7},{8});", new object[]
				{
					History.RoleID,
					(int)BocaiType,
					History.DataPeriods,
					History.ServerID,
					History.RoleName,
					History.ZoneID,
					History.BuyNum,
					History.WinNo,
					History.WinMoney
				});
				return DbHelperMySQL.ExecuteSql(sql) > -1;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return false;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0001FBAC File Offset: 0x0001DDAC
		public static void LoadLotteryHistory(BoCaiTypeEnum BocaiType, out List<KFBoCaoHistoryData> HistoryList, string cmd = "")
		{
			MySqlDataReader sdr = null;
			HistoryList = new List<KFBoCaoHistoryData>();
			try
			{
				string sql = string.Format("SELECT `rid`,`DataPeriods`,`ServerID`,`RoleName`,`ZoneID`,`BuyNum`,`WinNo`,`WinMoney`FROM t_bocai_lottery_history WHERE `BocaiType`={0} ORDER BY `DataPeriods` DESC, `BuyNum` ASC {1};", (int)BocaiType, cmd);
				sdr = DbHelperMySQL.ExecuteReader(sql, false);
				while (sdr != null && sdr.Read())
				{
					KFBoCaoHistoryData data = new KFBoCaoHistoryData();
					data.RoleID = Convert.ToInt32(sdr["rid"]);
					data.ZoneID = Convert.ToInt32(sdr["ZoneID"]);
					data.ServerID = Convert.ToInt32(sdr["ServerID"]);
					data.RoleName = sdr["RoleName"].ToString();
					data.BuyNum = Convert.ToInt32(sdr["BuyNum"]);
					data.WinNo = Convert.ToInt32(sdr["WinNo"]);
					data.WinMoney = Convert.ToInt64(sdr["WinMoney"]);
					data.DataPeriods = Convert.ToInt64(sdr["DataPeriods"]);
					HistoryList.Add(data);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
			}
			finally
			{
				if (sdr != null)
				{
					sdr.Close();
				}
			}
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0001FD20 File Offset: 0x0001DF20
		public static bool InserBuyBocai(long DataPeriods, KFBuyBocaiData buyDaya)
		{
			try
			{
				string sql = string.Format("REPLACE INTO t_bocai_buy_history(`rid`, `BocaiType`, `DataPeriods`, `ServerID`, `RoleName`, `ZoneID`, `BuyNum`, `BuyValue`) VALUES({0},{1},{2},{3},'{4}',{5},{6},'{7}');", new object[]
				{
					buyDaya.RoleID,
					buyDaya.BocaiType,
					DataPeriods,
					buyDaya.ServerID,
					buyDaya.RoleName,
					buyDaya.ZoneID,
					buyDaya.BuyNum,
					buyDaya.BuyValue
				});
				return DbHelperMySQL.ExecuteSql(sql) > -1;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return false;
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0001FDD8 File Offset: 0x0001DFD8
		public static bool UpdateBuyBocai(long DataPeriods, KFBuyBocaiData buyDaya)
		{
			try
			{
				string sql = string.Format("UPDATE t_bocai_buy_history SET BuyNum='{0}' WHERE rid='{1}' AND BocaiType='{2}' AND DataPeriods='{3}' AND ServerID='{4}' AND BuyValue='{5}'", new object[]
				{
					buyDaya.BuyNum,
					buyDaya.RoleID,
					buyDaya.BocaiType,
					DataPeriods,
					buyDaya.ServerID,
					buyDaya.BuyValue
				});
				return DbHelperMySQL.ExecuteSql(sql) > -1;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return false;
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0001FE78 File Offset: 0x0001E078
		public static bool LoadBuyHistory(int BocaiType, long DataPeriods, out List<KFBuyBocaiData> HistoryList)
		{
			bool flag = false;
			MySqlDataReader sdr = null;
			HistoryList = new List<KFBuyBocaiData>();
			try
			{
				string sql = string.Format("SELECT `rid`, `BocaiType`,`ServerID`,`RoleName`,`ZoneID`,`BuyNum`,`BuyValue`FROM t_bocai_buy_history WHERE `DataPeriods`={0};", DataPeriods);
				sdr = DbHelperMySQL.ExecuteReader(sql, false);
				while (sdr != null && sdr.Read())
				{
					KFBuyBocaiData data = new KFBuyBocaiData();
					data.RoleID = Convert.ToInt32(sdr["rid"]);
					data.ZoneID = Convert.ToInt32(sdr["ZoneID"]);
					data.ServerID = Convert.ToInt32(sdr["ServerID"]);
					data.RoleName = sdr["RoleName"].ToString();
					data.BuyNum = Convert.ToInt32(sdr["BuyNum"]);
					data.BocaiType = Convert.ToInt32(sdr["BocaiType"]);
					data.BuyValue = sdr["BuyValue"].ToString();
					HistoryList.Add(data);
				}
				flag = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
			}
			finally
			{
				if (sdr != null)
				{
					sdr.Close();
				}
			}
			return flag;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0001FFC8 File Offset: 0x0001E1C8
		public static bool ReplaceBoCaiShop(KFBoCaiShopDB data)
		{
			try
			{
				string sql = string.Format("REPLACE INTO t_bocai_shop(ID, BuyNum, Periods, WuPinID) VALUES({0},{1},{2},'{3}');", new object[]
				{
					data.ID,
					data.BuyNum,
					data.Periods,
					data.WuPinID
				});
				return DbHelperMySQL.ExecuteSql(sql) > -1;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return false;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x00020054 File Offset: 0x0001E254
		public static void SelectBoCaiShop(string Periods, out List<KFBoCaiShopDB> dList)
		{
			dList = null;
			MySqlDataReader sdr = null;
			try
			{
				string sql = string.Format("SELECT `ID`,`BuyNum`,`WuPinID` FROM t_bocai_shop WHERE `Periods`={0}", Periods);
				sdr = DbHelperMySQL.ExecuteReader(sql, false);
				dList = new List<KFBoCaiShopDB>();
				while (sdr != null && sdr.Read())
				{
					KFBoCaiShopDB data = new KFBoCaiShopDB();
					data.ID = Convert.ToInt32(sdr["ID"]);
					data.BuyNum = Convert.ToInt32(sdr["BuyNum"]);
					data.WuPinID = sdr["WuPinID"].ToString();
					data.Periods = Convert.ToInt32(Periods);
					dList.Add(data);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
			}
			finally
			{
				if (sdr != null)
				{
					sdr.Close();
				}
			}
		}

		// Token: 0x0600021A RID: 538 RVA: 0x00020144 File Offset: 0x0001E344
		public static bool DelTableData(string table, string Sql)
		{
			try
			{
				string sql = string.Format("delete from {0} where {1}", table, Sql);
				return DbHelperMySQL.ExecuteSql(sql) > -1;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return false;
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00020194 File Offset: 0x0001E394
		public static void StopServer(string str)
		{
			LogManager.WriteLog(LogTypes.Error, str, null, true);
			LogManager.WriteLog(LogTypes.Fatal, str, null, true);
			LogManager.WriteLog(LogTypes.Fatal, "博彩初始化失败了，停止检查一下,测试阶段可以忽略。任意键继续", null, true);
			Console.ReadKey();
		}

		// Token: 0x0600021C RID: 540 RVA: 0x000201CC File Offset: 0x0001E3CC
		public static string ListInt2String(List<int> iList)
		{
			string str = "";
			try
			{
				foreach (int item in iList)
				{
					if (string.IsNullOrEmpty(str))
					{
						str = string.Format("{0}", item);
					}
					else
					{
						str = string.Format("{0},{1}", str, item);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return str;
		}

		// Token: 0x0600021D RID: 541 RVA: 0x00020284 File Offset: 0x0001E484
		public static void String2ListInt(string str, out List<int> iList)
		{
			iList = new List<int>();
			try
			{
				string[] temp = str.Split(new char[]
				{
					','
				});
				foreach (string item in temp)
				{
					iList.Add(Convert.ToInt32(item));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		// Token: 0x04000133 RID: 307
		private const int result = -1;
	}
}
