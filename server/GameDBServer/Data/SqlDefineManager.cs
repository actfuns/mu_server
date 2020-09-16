using System;
using GameDBServer.DB;
using GameDBServer.Logic;
using MySQLDriverCS;

namespace GameDBServer.Data
{
	
	internal class SqlDefineManager
	{
		
		public static SqlDefineManager gInstance()
		{
			return SqlDefineManager.instance;
		}

		
		public static T SqlHandler<T>(string AllSql, Global.SQLDelegate<T> Func)
		{
			T mode = default(T);
			MySQLConnection conn = null;
			try
			{
				conn = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand cmd = new MySQLCommand(AllSql, conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				mode = Func(reader);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", AllSql), EventLevels.Important);
				cmd.Dispose();
			}
			finally
			{
				if (null != conn)
				{
					DBManager.getInstance().DBConns.PushDBConnection(conn);
				}
			}
			return mode;
		}

		
		public static string SelectAllMailList = "SELECT maillid,senderrid, senderrname, senderjob, sendtime, receiverrid, isread, readtime, fumomoney,content FROM t_mailfumo;";

		
		public static string InsertOneMail = "INSERT INTO `t_mailfumo` (`senderrid`, `senderrname`, `senderjob`, `sendtime`, `receiverrid`, `isread`, `readtime`, `fumomoney`, `content`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');";

		
		public static string DeleteMailByID = "DELETE FROM t_mailfumo WHERE maillid={0};";

		
		public static string DeleteMailByIDList = "DELETE FROM t_mailfumo WHERE ({0}) AND receiverrid={1}";

		
		public static string UpdateReadState = "UPDATE t_mailfumo SET isread={0}, readtime='{1}' WHERE maillid='{2}';";

		
		public static string SelectMapStartServer = "SELECT tid, senderid, recid_list, accept, give FROM t_mail_fumo_map WHERE tid>={0};";

		
		public static string SelectMapList = "SELECT tid, senderid, recid_list, accept, give from t_mail_fumo_map where tid='{0}' AND senderid='{1}' ";

		
		public static string InsertMapList = "INSERT INTO `t_mail_fumo_map` (`tid`, `senderid`, `recid_list`, `accept`, `give`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');";

		
		public static string UpdateMapAccept = "UPDATE t_mail_fumo_map SET accept='{0}' WHERE senderid='{1}' and tid='{2}'";

		
		public static string UpdateMapGive = "UPDATE t_mail_fumo_map SET give='{0}', recid_list='{1}' WHERE senderid='{2}' and tid='{3}'";

		
		public static string SelectMaxMailIndex = "SELECT MAX(maillid) as mymaxvalue from t_mailfumo";

		
		public static string SelectMailCount = "SELECT COUNT(maillid) as mymaxvalue from t_mailfumo where receiverrid={0}";

		
		public static string InsertRebornYinJi = "INSERT INTO `t_reborn_stamp` (`rid`, `stamp`, `reset`, `use_point`) VALUES ('{0}', '{1}', '{2}', '{3}');";

		
		public static string SelectRebornYinJiAll = "SELECT `rid`, `stamp`, `reset`, `use_point` FROM `t_reborn_stamp` ";

		
		public static string UpdateRebornYinJi = "UPDATE t_reborn_stamp SET `stamp`='{1}',`reset`='{2}', `use_point`='{3}' WHERE rid='{0}'";

		
		public static string UpdateRebornBagNum = "UPDATE t_roles set reborn_bagnum={0} where rid={1}";

		
		public static string UpdateRebornShow = "UPDATE t_roles set reborn_isshow={0} where rid={1}";

		
		public static string UpdateRebornShowModel = "UPDATE t_roles set reborn_isshow_model={0} where rid={1}";

		
		public static string InsertRebornGridNum = "INSERT INTO t_reborn_storage (rid, extgridnum) VALUES({0}, {1}) ON DUPLICATE KEY UPDATE extgridnum={2}";

		
		public static string RegressGetMinRegtime = "SELECT userid, regtime from t_roles where `userid`='{0}' order by regtime ASC LIMIT 1";

		
		public static string RegressGetSignData = "SELECT userid, activitytype, keystr, hasgettimes, lastgettime, activitydata FROM t_huodongawarduserhist_regress WHERE userid='{0}' AND activitytype={1} AND active_stage='{2}'";

		
		public static string RegressGetDaySignData = "SELECT userid, activitytype, keystr, hasgettimes, lastgettime, activitydata FROM t_huodongawarduserhist_regress WHERE userid='{0}' AND activitytype={1} AND keystr='{2}' AND active_stage='{3}'";

		
		public static string RegressUpdateDaySignData = "UPDATE t_huodongawarduserhist_regress SET activitydata='{0}', lastgettime='{1}' WHERE userid='{2}' AND activitytype={3} AND keystr='{4}' AND active_stage='{5}'";

		
		public static string RegressInsertDaySignData = "INSERT INTO t_huodongawarduserhist_regress (userid, activitytype, keystr, hasgettimes,lastgettime, activitydata, active_stage) VALUES('{0}', {1}, '{2}', {3}, '{4}', '{5}', '{6}')";

		
		public static string RegressInsertStore = "INSERT INTO t_limit_usergoodsbuy (userid, goodsid, dayid, usednum, active_stage) VALUES('{0}', {1}, {2}, {3}, '{4}') ON DUPLICATE KEY UPDATE dayID={2}, usedNum={3}";

		
		public static string RegressSelectStore = "SELECT dayid, usednum FROM t_limit_usergoodsbuy WHERE userid='{0}' AND goodsid={1} AND active_stage='{2}'";

		
		public static string RegressSelectStoreInfo = "SELECT goodsid, usednum FROM t_limit_usergoodsbuy WHERE userid='{0}' AND dayid={1} AND active_stage='{2}'";

		
		public static string RebornEquipHoleSelectInfo = "SELECT holeid, level, able FROM `t_reborn_equiphole` WHERE `rid`={0}";

		
		public static string RebornEquipHoleInsertInfo = "INSERT INTO `t_reborn_equiphole` (`rid`, `holeid`, `level`, `able`) VALUES ('{0}', '{1}', '{2}', '{3}');";

		
		public static string RebornEquipHoleUpdateInfo = "UPDATE t_reborn_equiphole SET level='{0}', able='{1}' WHERE rid='{2}' AND holeid='{3}'";

		
		public static string MazingerStoreSelectInfo = "SELECT type, stage, level, exp FROM `t_mazinger_store` WHERE `rid`={0}";

		
		public static string MazingerStoreInsertInfo = "INSERT INTO `t_mazinger_store` (`rid`, `type`, `stage`, `level`, `exp`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');";

		
		public static string MazingerStoreUpdateInfo = "UPDATE t_mazinger_store SET stage='{2}', level='{3}', exp='{4}'  WHERE `rid`={0} AND `type`={1}";

		
		private static SqlDefineManager instance = new SqlDefineManager();
	}
}
