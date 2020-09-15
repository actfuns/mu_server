using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GameDBServer.Core;
using GameDBServer.Data;
using GameDBServer.Logic;
using GameDBServer.Logic.Ten;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB
{
    // Token: 0x020000EB RID: 235
    public class DBQuery
    {
        // Token: 0x0600022B RID: 555 RVA: 0x0000BF48 File Offset: 0x0000A148
        public static bool GetFriendData(DBManager dbMgr, FriendData friendData)
        {
            DBRoleInfo otherDbRoleInfo = new DBRoleInfo();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT rname, level, occupation, position, changelifecount, combatforce, zoneid,zhanduiid \r\n                                                FROM t_roles WHERE isdel=0 AND rid={0}", friendData.OtherRoleID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    otherDbRoleInfo.RoleID = friendData.OtherRoleID;
                    otherDbRoleInfo.RoleName = reader["rname"].ToString();
                    otherDbRoleInfo.Level = Convert.ToInt32(reader["level"].ToString());
                    otherDbRoleInfo.Occupation = Convert.ToInt32(reader["occupation"].ToString());
                    otherDbRoleInfo.Position = reader["position"].ToString();
                    otherDbRoleInfo.ChangeLifeCount = Convert.ToInt32(reader["changelifecount"].ToString());
                    otherDbRoleInfo.CombatForce = Convert.ToInt32(reader["combatforce"].ToString());
                    otherDbRoleInfo.ZoneID = Convert.ToInt32(reader["zoneid"].ToString());
                    otherDbRoleInfo.ZhanDuiID = Convert.ToInt32(reader["zhanduiid"].ToString());
                    MySQLConnection conn2 = conn;
                    string[] fields = new string[]
                    {
                        "spouseid",
                        "marrytype",
                        "ringid",
                        "goodwillexp",
                        "goodwillstar",
                        "goodwilllevel",
                        "givenrose",
                        "lovemessage",
                        "autoreject",
                        "changtime"
                    };
                    string[] tables = new string[]
                    {
                        "t_marry"
                    };
                    object[,] array = new object[1, 3];
                    array[0, 0] = "roleid";
                    array[0, 1] = "=";
                    array[0, 2] = friendData.OtherRoleID;
                    MySQLSelectCommand cmdNext = new MySQLSelectCommand(conn2, fields, tables, array, null, null);
                    DBRoleInfo.DBTableRow2RoleInfo_MarriageData(otherDbRoleInfo, cmdNext);
                    friendData.OtherRoleName = Global.FormatRoleName(otherDbRoleInfo);
                    friendData.OtherLevel = otherDbRoleInfo.Level;
                    friendData.Occupation = otherDbRoleInfo.Occupation;
                    friendData.OnlineState = Global.GetRoleOnlineState(otherDbRoleInfo);
                    friendData.Position = otherDbRoleInfo.Position;
                    friendData.FriendChangeLifeLev = otherDbRoleInfo.ChangeLifeCount;
                    friendData.FriendCombatForce = otherDbRoleInfo.CombatForce;
                    friendData.SpouseId = ((otherDbRoleInfo.MyMarriageData != null) ? otherDbRoleInfo.MyMarriageData.nSpouseID : 0);
                    friendData.ZhanDuiID = otherDbRoleInfo.ZhanDuiID;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return otherDbRoleInfo.RoleID > 0;
        }

        // Token: 0x0600022C RID: 556 RVA: 0x0000C244 File Offset: 0x0000A444
        public static void QueryDJPointData(DBManager dbMgr, DBRoleInfo dbRoleInfo)
        {
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT Id, rid, djpoint, total, wincnt, yestoday, lastweek, lastmonth, dayupdown, weekupdown, monthupdown FROM t_djpoints WHERE rid={0}", dbRoleInfo.RoleID);
                MySQLConnection conn2 = conn;
                string[] fields = new string[]
                {
                    "Id",
                    "djpoint",
                    "total",
                    "wincnt",
                    "yestoday",
                    "lastweek",
                    "lastmonth",
                    "dayupdown",
                    "weekupdown",
                    "monthupdown"
                };
                string[] tables = new string[]
                {
                    "t_djpoints"
                };
                object[,] array = new object[1, 3];
                array[0, 0] = "rid";
                array[0, 1] = "=";
                array[0, 2] = dbRoleInfo.RoleID;
                MySQLSelectCommand cmd = new MySQLSelectCommand(conn2, fields, tables, array, null, null);
                if (cmd.Table.Rows.Count > 0)
                {
                    lock (dbRoleInfo)
                    {
                        DBRoleInfo obj = dbRoleInfo;
                        dbRoleInfo.LastDJPointDataTikcs = DateTime.Now.Ticks / 10000L;
                        dbRoleInfo.RoleDJPointData = new DJPointData
                        {
                            DbID = Convert.ToInt32(cmd.Table.Rows[0]["Id"].ToString()),
                            RoleID = dbRoleInfo.RoleID,
                            DJPoint = Convert.ToInt32(cmd.Table.Rows[0]["djpoint"].ToString()),
                            Total = Convert.ToInt32(cmd.Table.Rows[0]["total"].ToString()),
                            Wincnt = Convert.ToInt32(cmd.Table.Rows[0]["wincnt"].ToString()),
                            Yestoday = Convert.ToInt32(cmd.Table.Rows[0]["yestoday"].ToString()),
                            Lastweek = Convert.ToInt32(cmd.Table.Rows[0]["lastweek"].ToString()),
                            Lastmonth = Convert.ToInt32(cmd.Table.Rows[0]["lastmonth"].ToString()),
                            Dayupdown = Convert.ToInt32(cmd.Table.Rows[0]["dayupdown"].ToString()),
                            Weekupdown = Convert.ToInt32(cmd.Table.Rows[0]["weekupdown"].ToString()),
                            Monthupdown = Convert.ToInt32(cmd.Table.Rows[0]["monthupdown"].ToString())
                        };
                    }
                }
                else
                {
                    lock (dbRoleInfo)
                    {
                        DBRoleInfo obj = dbRoleInfo;
                        dbRoleInfo.LastDJPointDataTikcs = DateTime.Now.Ticks / 10000L;
                        dbRoleInfo.RoleDJPointData = new DJPointData
                        {
                            DbID = -1,
                            RoleID = dbRoleInfo.RoleID,
                            DJPoint = 0,
                            Total = 0,
                            Wincnt = 0,
                            Yestoday = 0,
                            Lastweek = 0,
                            Lastmonth = 0,
                            Dayupdown = 0,
                            Weekupdown = 0,
                            Monthupdown = 0
                        };
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
        }

        // Token: 0x0600022D RID: 557 RVA: 0x0000C6A0 File Offset: 0x0000A8A0
        public static List<DJPointData> QueryDJPointData(DBManager dbMgr)
        {
            List<DJPointData> djPointsHostList = new List<DJPointData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "SELECT Id, rid, djpoint, total, wincnt, yestoday, lastweek, lastmonth, dayupdown, weekupdown, monthupdown FROM t_djpoints ORDER BY djpoint DESC LIMIT 100";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                int count = 0;
                while (reader.Read() && count < 100)
                {
                    djPointsHostList.Add(new DJPointData
                    {
                        DbID = Convert.ToInt32(reader["Id"].ToString()),
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        DJPoint = Convert.ToInt32(reader["djpoint"].ToString()),
                        Total = Convert.ToInt32(reader["total"].ToString()),
                        Wincnt = Convert.ToInt32(reader["wincnt"].ToString()),
                        Yestoday = Convert.ToInt32(reader["yestoday"].ToString()),
                        Lastweek = Convert.ToInt32(reader["lastweek"].ToString()),
                        Lastmonth = Convert.ToInt32(reader["lastmonth"].ToString()),
                        Dayupdown = Convert.ToInt32(reader["dayupdown"].ToString()),
                        Weekupdown = Convert.ToInt32(reader["weekupdown"].ToString()),
                        Monthupdown = Convert.ToInt32(reader["monthupdown"].ToString())
                    });
                    count++;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return djPointsHostList;
        }

        // Token: 0x0600022E RID: 558 RVA: 0x0000C8B8 File Offset: 0x0000AAB8
        public static Dictionary<string, BulletinMsgData> QueryBulletinMsgDict(DBManager dbMgr)
        {
            Dictionary<string, BulletinMsgData> dict = new Dictionary<string, BulletinMsgData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "SELECT msgid, intervals, bulletintext, fromdate, todate FROM t_bulletin";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    BulletinMsgData bulletinMsgData = new BulletinMsgData
                    {
                        MsgID = reader["msgid"].ToString(),
                        Interval = Convert.ToInt32(reader["intervals"].ToString()),
                        BulletinText = reader["bulletintext"].ToString(),
                        BulletinTicks = DataHelper.ConvertToTicks(reader["fromdate"].ToString())
                    };
                    long PlayTicks = DataHelper.ConvertToTicks(reader["todate"].ToString()) - bulletinMsgData.BulletinTicks;
                    bulletinMsgData.PlayMinutes = (int)(PlayTicks / 60000L);
                    dict[bulletinMsgData.MsgID] = bulletinMsgData;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return dict;
        }

        // Token: 0x0600022F RID: 559 RVA: 0x0000CA24 File Offset: 0x0000AC24
        public static Dictionary<string, string> QueryGameConfigDict(DBManager dbMgr)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "SELECT paramname, paramvalue FROM t_config";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    string paramName = reader["paramname"].ToString();
                    string paramVal = reader["paramvalue"].ToString();
                    dict[paramName] = paramVal;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return dict;
        }

        // Token: 0x06000230 RID: 560 RVA: 0x0000CAF8 File Offset: 0x0000ACF8
        public static List<TempItemChargeInfo> QueryTempItemChargeInfo(DBManager dbMgr, int rid, int serialID = 0, byte HandleDel = 0)
        {
            MySQLConnection conn = null;
            List<TempItemChargeInfo> result;
            try
            {
                List<TempItemChargeInfo> tempItemInfoList = new List<TempItemChargeInfo>();
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText;
                if (serialID == 0)
                {
                    cmdText = string.Format("SELECT id, uid, rid, addmoney, itemid, chargetime, isdel FROM t_tempitem WHERE rid = {0}", rid);
                }
                else
                {
                    cmdText = string.Format("SELECT id, uid, rid, addmoney, itemid, chargetime, isdel FROM t_tempitem WHERE id = {0}", serialID);
                }
                if (HandleDel == 0)
                {
                    cmdText += string.Format(" AND isdel = 0", new object[0]);
                }
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    TempItemChargeInfo tempItemCharge = new TempItemChargeInfo();
                    tempItemCharge.ID = Convert.ToInt32(reader["id"].ToString());
                    tempItemCharge.userID = reader["uid"].ToString();
                    tempItemCharge.chargeRoleID = Convert.ToInt32(reader["rid"].ToString());
                    tempItemCharge.addUserMoney = Convert.ToInt32(reader["addmoney"].ToString());
                    tempItemCharge.zhigouID = Convert.ToInt32(reader["itemid"].ToString());
                    tempItemCharge.chargeTime = reader["chargetime"].ToString();
                    tempItemCharge.isDel = Convert.ToByte(reader["isdel"].ToString());
                    tempItemInfoList.Add(tempItemCharge);
                    if (serialID == 0 && tempItemCharge.isDel == 0)
                    {
                        LogManager.WriteLog(LogTypes.Error, string.Format("从t_tempitem 找到 UID={0}，RID={1}，money={2}", reader["uid"].ToString(), reader["rid"].ToString(), Convert.ToInt32(reader["addmoney"].ToString())), null, true);
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
                result = tempItemInfoList;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return result;
        }

        // Token: 0x06000231 RID: 561 RVA: 0x0000CD40 File Offset: 0x0000AF40
        public static void QueryTempMoney(DBManager dbMgr, List<TempMoneyInfo> tempMoneyInfoList)
        {
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "SELECT id, uid, rid, addmoney, itemid, DATE_FORMAT(chargetime,'%Y-%m-%d %H:%i:%s') AS chargetime,cc FROM t_tempmoney";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                int minId = int.MaxValue;
                int maxId = 0;
                while (reader.Read())
                {
                    TempMoneyInfo tempMoneyItem = new TempMoneyInfo();
                    minId = Math.Min(minId, Convert.ToInt32(reader["id"].ToString()));
                    maxId = Math.Max(maxId, Convert.ToInt32(reader["id"].ToString()));
                    tempMoneyItem.userID = reader["uid"].ToString();
                    tempMoneyItem.chargeRoleID = Convert.ToInt32(reader["rid"].ToString());
                    tempMoneyItem.addUserMoney = Convert.ToInt32(reader["addmoney"].ToString());
                    tempMoneyItem.addUserItem = Convert.ToInt32(reader["itemid"].ToString());
                    tempMoneyItem.chargeTm = reader["chargetime"].ToString();
                    bool skip = false;
                    if (reader.FieldCount < 7)
                    {
                        LogManager.WriteLog(LogTypes.DataCheck, string.Format("DataCheckFaild#t_tempmoney#userid={0},money={1},cc={2}", tempMoneyItem.userID, tempMoneyItem.addUserMoney, ""), null, true);
                        LogManager.WriteLog(LogTypes.Error, string.Format("从t_tempmoney 找到 UID={0}，money={1},数据库需升级", tempMoneyItem.userID, tempMoneyItem.addUserMoney), null, true);
                        skip = true;
                    }
                    else
                    {
                        tempMoneyItem.cc = reader["cc"].ToString();
                        if (string.IsNullOrWhiteSpace(tempMoneyItem.cc) || tempMoneyItem.cc.Length < 32)
                        {
                            LogManager.WriteLog(LogTypes.DataCheck, string.Format("DataCheckFaild#t_tempmoney#userid={0},money={1},cc={2}", tempMoneyItem.userID, tempMoneyItem.addUserMoney, ""), null, true);
                            LogManager.WriteLog(LogTypes.Error, string.Format("从t_tempmoney 找到 UID={0}，money={1},后台接口需升级", tempMoneyItem.userID, tempMoneyItem.addUserMoney), null, true);
                            skip = true;
                        }
                        else
                        {
                            string c = tempMoneyItem.cc.Substring(24, 8);
                            string cc = Global.GCC(4, new object[]
                            {
                                c,
                                tempMoneyItem.userID,
                                tempMoneyItem.addUserMoney,
                                tempMoneyItem.chargeTm
                            });
                            if (cc.Substring(0, 24) != tempMoneyItem.cc.Substring(0, 24))
                            {
                                LogManager.WriteLog(LogTypes.DataCheck, string.Format("DataCheckFaild#t_tempmoney#userid={0},money={1},cc={2}", tempMoneyItem.userID, tempMoneyItem.addUserMoney, tempMoneyItem.cc), null, true);
                                LogManager.WriteLog(LogTypes.Error, string.Format("从t_tempmoney 找到 UID={0}，money={1},校验失败", tempMoneyItem.userID, tempMoneyItem.addUserMoney), null, true);
                                skip = true;
                            }
                            else
                            {
                                string uc = tempMoneyItem.userID + tempMoneyItem.chargeTm + tempMoneyItem.cc;
                                if (!Global.AddCC(uc))
                                {
                                    LogManager.WriteLog(LogTypes.Error, string.Format("从t_tempmoney 找到 UID={0}，money={1},校验重复", tempMoneyItem.userID, tempMoneyItem.addUserMoney), null, true);
                                    skip = true;
                                }
                            }
                        }
                    }
                    if (!skip)
                    {
                        tempMoneyInfoList.Add(tempMoneyItem);
                        LogManager.WriteLog(LogTypes.Error, string.Format("从t_tempmoney 找到 UID={0}，money={1}", tempMoneyItem.userID, tempMoneyItem.addUserMoney), null, true);
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
                if (maxId > 0)
                {
                    cmdText = string.Format("DELETE FROM t_tempmoney WHERE id<={0}", maxId);
                    cmd = new MySQLCommand(cmdText, conn);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                }
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
        }

        // Token: 0x06000232 RID: 562 RVA: 0x0000D174 File Offset: 0x0000B374
        public static Dictionary<string, LiPinMaItem> QueryLiPinMaDict(DBManager dbMgr)
        {
            Dictionary<string, LiPinMaItem> dict = new Dictionary<string, LiPinMaItem>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "SELECT lipinma, huodongid, maxnum, usednum, ptid, ptrepeat FROM t_linpinma";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    LiPinMaItem liPinMaItem = new LiPinMaItem
                    {
                        LiPinMa = reader["lipinma"].ToString(),
                        HuodongID = Convert.ToInt32(reader["huodongid"].ToString()),
                        MaxNum = Convert.ToInt32(reader["maxnum"].ToString()),
                        UsedNum = Convert.ToInt32(reader["usednum"].ToString()),
                        PingTaiID = Convert.ToInt32(reader["ptid"].ToString()),
                        PingTaiRepeat = Convert.ToInt32(reader["ptrepeat"].ToString())
                    };
                    dict[liPinMaItem.LiPinMa] = liPinMaItem;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return dict;
        }

        // Token: 0x06000233 RID: 563 RVA: 0x0000D2EC File Offset: 0x0000B4EC
        public static void QueryPreNames(DBManager dbMgr, Dictionary<string, PreNameItem> preNamesDict, List<PreNameItem> malePreNamesList, List<PreNameItem> femalePreNamesList)
        {
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "SELECT name, sex FROM t_prenames WHERE used=0";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    PreNameItem preNameItem = new PreNameItem
                    {
                        Name = reader["name"].ToString(),
                        Sex = Convert.ToInt32(reader["sex"].ToString()),
                        Used = 0
                    };
                    preNamesDict[preNameItem.Name] = preNameItem;
                    if (0 == preNameItem.Sex)
                    {
                        malePreNamesList.Add(preNameItem);
                    }
                    else
                    {
                        femalePreNamesList.Add(preNameItem);
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
        }

        // Token: 0x06000234 RID: 564 RVA: 0x0000D408 File Offset: 0x0000B608
        public static Dictionary<int, FuBenHistData> QueryFuBenHistDict(DBManager dbMgr)
        {
            Dictionary<int, FuBenHistData> dict = new Dictionary<int, FuBenHistData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "SELECT fubenid, t_fubenhist.rid, t_roles.rname, usedsecs FROM t_fubenhist, t_roles WHERE t_roles.rid=t_fubenhist.rid";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    FuBenHistData fuBenHistData = new FuBenHistData
                    {
                        FuBenID = Convert.ToInt32(reader["fubenid"].ToString()),
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = reader["rname"].ToString(),
                        UsedSecs = Convert.ToInt32(reader["usedsecs"].ToString())
                    };
                    dict[fuBenHistData.FuBenID] = fuBenHistData;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return dict;
        }

        // Token: 0x06000235 RID: 565 RVA: 0x0000D53C File Offset: 0x0000B73C
        public static string QueryUserIDByRoleName(DBManager dbMgr, string otherRoleName, int zoneID)
        {
            string ret = "";
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT userid FROM t_roles WHERE rname='{0}' AND zoneid={1}", otherRoleName, zoneID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    ret = reader["userid"].ToString();
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return ret;
        }

        // Token: 0x06000236 RID: 566 RVA: 0x0000D5FC File Offset: 0x0000B7FC
        public static void QueryUserMoneyByUserID(DBManager dbMgr, string userID, out int userMoney, out int realMoney)
        {
            userMoney = 0;
            realMoney = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT money, realmoney FROM t_money WHERE userid='{0}'", userID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    userMoney = Convert.ToInt32(reader["money"].ToString());
                    realMoney = Convert.ToInt32(reader["realmoney"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
        }

        // Token: 0x06000237 RID: 567 RVA: 0x0000D6CC File Offset: 0x0000B8CC
        public static void QueryUserUserIdValue(DBManager dbMgr, string userID, out int realMoney, out int unionLevel)
        {
            unionLevel = 0;
            realMoney = 0;
            using (MyDbConnection3 conn = new MyDbConnection3(false))
            {
                string cmdText = string.Format("SELECT realmoney FROM t_money WHERE userid='{0}'", userID);
                realMoney = conn.GetSingleInt(cmdText, 0, new MySQLParameter[0]);
                cmdText = string.Format("SELECT MAX(changelifecount*0xffff+`level`) FROM t_roles WHERE userid='{0}'", userID);
                unionLevel = conn.GetSingleInt(cmdText, 0, new MySQLParameter[0]);
            }
        }

        // Token: 0x06000238 RID: 568 RVA: 0x0000D744 File Offset: 0x0000B944
        public static void QueryTodayUserMoneyByUserID(DBManager dbMgr, string userID, int zoneID, out int userMoney, out int realMoney)
        {
            userMoney = 0;
            realMoney = 0;
            DateTime now = DateTime.Now;
            string todayStart = string.Format("{0:0000}-{1:00}-{2:00} 00:00:00", now.Year, now.Month, now.Day);
            string todayEnd = string.Format("{0:0000}-{1:00}-{2:00} 23:59:59", now.Year, now.Month, now.Day);
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT SUM(amount) AS totalmoney FROM t_inputlog WHERE u='{0}' AND inputtime>='{2}' AND inputtime<='{3}'", new object[]
                {
                    userID,
                    zoneID,
                    todayStart,
                    todayEnd
                });
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    try
                    {
                        userMoney = Convert.ToInt32(reader["totalmoney"].ToString());
                        realMoney = userMoney;
                    }
                    catch (Exception)
                    {
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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
        }

        // Token: 0x06000239 RID: 569 RVA: 0x0000D8A8 File Offset: 0x0000BAA8
        public static void QueryTodayUserMoneyByUserID2(DBManager dbMgr, string userID, int zoneID, out int userMoney, out int realMoney)
        {
            userMoney = 0;
            realMoney = 0;
            DateTime now = DateTime.Now;
            string todayStart = string.Format("{0:0000}-{1:00}-{2:00} 00:01:01", now.Year, now.Month, now.Day);
            string todayEnd = string.Format("{0:0000}-{1:00}-{2:00} 23:59:59", now.Year, now.Month, now.Day);
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT SUM(amount) AS totalmoney FROM t_inputlog2 WHERE u='{0}' AND inputtime>='{2}' AND inputtime<='{3}'", new object[]
                {
                    userID,
                    zoneID,
                    todayStart,
                    todayEnd
                });
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    try
                    {
                        userMoney = Convert.ToInt32(reader["totalmoney"].ToString());
                        realMoney = userMoney;
                    }
                    catch (Exception)
                    {
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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
        }

        // Token: 0x0600023A RID: 570 RVA: 0x0000DA0C File Offset: 0x0000BC0C
        public static List<SearchRoleData> SearchOnlineRoleByName(DBManager dbMgr, string searchText, int startIndex, int limitNum)
        {
            List<SearchRoleData> searchRoleDataList = new List<SearchRoleData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT rid, rname, sex, occupation, level, zoneid, faction, bhname, changelifecount FROM t_roles WHERE rname LIKE '%{0}%' AND rid>{1} AND lasttime>logofftime AND isdel=0 LIMIT {2}", searchText, startIndex, limitNum);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    int nChangLifeCount = Convert.ToInt32(reader["changelifecount"].ToString());
                    int nLevel = Convert.ToInt32(reader["level"].ToString());
                    if (100 * nChangLifeCount + nLevel >= 50)
                    {
                        SearchRoleData searchRoleData = new SearchRoleData
                        {
                            RoleID = Convert.ToInt32(reader["rid"].ToString()),
                            RoleName = Global.FormatRoleName(Convert.ToInt32(reader["zoneid"].ToString()), reader["rname"].ToString()),
                            RoleSex = Convert.ToInt32(reader["sex"].ToString()),
                            Level = nLevel,
                            Occupation = Convert.ToInt32(reader["occupation"].ToString()),
                            Faction = Convert.ToInt32(reader["faction"].ToString()),
                            BHName = reader["bhname"].ToString()
                        };
                        searchRoleDataList.Add(searchRoleData);
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return searchRoleDataList;
        }

        // Token: 0x0600023B RID: 571 RVA: 0x0000DBF8 File Offset: 0x0000BDF8
        public static string GetRoleParamByName(DBManager dbMgr, int roleID, string paramName)
        {
            List<PaiHangItemData> list = new List<PaiHangItemData>();
            MySQLConnection conn = null;
            string sValue = "";
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType(paramName, null);
                string cmdText = string.Format("SELECT p.rid, p.{2} FROM {3} as p  where p.{4}={0} and p.rid={1}", new object[]
                {
                    roleParamType.KeyString,
                    roleID,
                    roleParamType.ColumnName,
                    roleParamType.TableName,
                    roleParamType.IdxName
                });
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    sValue = reader[1].ToString();
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return sValue;
        }

        // Token: 0x0600023C RID: 572 RVA: 0x0000DD00 File Offset: 0x0000BF00
        public static List<PaiHangItemData> GetRoleParamsTablePaiHang(DBManager dbMgr, string paramName)
        {
            List<PaiHangItemData> list = new List<PaiHangItemData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType(paramName, null);
                string cmdText = string.Format("SELECT p.rid, {0}, rname, zoneid FROM {1} as p, t_roles as r  where p.{2}={3} and p.rid=r.rid ORDER BY p.{0} DESC LIMIT 100", new object[]
                {
                    roleParamType.ColumnName,
                    roleParamType.TableName,
                    roleParamType.IdxName,
                    roleParamType.KeyString
                });
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                int count = 0;
                while (reader.Read() && count < 100)
                {
                    PaiHangItemData paiHangItemData = new PaiHangItemData
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(reader["zoneid"].ToString(), reader["rname"].ToString()),
                        Val1 = Convert.ToInt32(reader[1].ToString())
                    };
                    list.Add(paiHangItemData);
                    count++;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x0600023D RID: 573 RVA: 0x0000DE84 File Offset: 0x0000C084
        private static List<PaiHangItemData> GetUserMoneyTablePaiHang(DBManager dbMgr, string fieldVal1, string otherCondition)
        {
            List<PaiHangItemData> list = new List<PaiHangItemData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT userid, {0} FROM t_money{1} ORDER BY {0} DESC LIMIT 100", fieldVal1, otherCondition);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                int count = 0;
                while (reader.Read() && count < 100)
                {
                    PaiHangItemData paiHangItemData = new PaiHangItemData
                    {
                        uid = reader["userid"].ToString(),
                        Val1 = Convert.ToInt32(reader[fieldVal1].ToString())
                    };
                    list.Add(paiHangItemData);
                    count++;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x0600023E RID: 574 RVA: 0x0000DF88 File Offset: 0x0000C188
        private static List<PaiHangItemData> GetRoleTablePaiHang(DBManager dbMgr, string fieldVal1, string otherCondition)
        {
            List<PaiHangItemData> list = new List<PaiHangItemData>();
            List<PaiHangItemData> result;
            if (!GameDBManager.GameConfigMgr.IsPaiHangKey(fieldVal1))
            {
                result = list;
            }
            else
            {
                MySQLConnection conn = null;
                try
                {
                    conn = dbMgr.DBConns.PopDBConnection();
                    string cmdText = string.Format("SELECT rid, rname, zoneid, admiredcount, {0} FROM t_roles{1} ORDER BY {0} DESC LIMIT 100", fieldVal1, otherCondition);
                    MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                    MySQLDataReader reader = cmd.ExecuteReaderEx();
                    int count = 0;
                    while (reader.Read() && count < 100)
                    {
                        PaiHangItemData paiHangItemData = new PaiHangItemData
                        {
                            RoleID = Convert.ToInt32(reader["rid"].ToString()),
                            RoleName = Global.FormatRoleName(reader["zoneid"].ToString(), reader["rname"].ToString()),
                            Val1 = Convert.ToInt32(reader[fieldVal1].ToString()),
                            Val2 = Convert.ToInt32(reader["admiredcount"].ToString())
                        };
                        list.Add(paiHangItemData);
                        count++;
                    }
                    GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                    cmd.Dispose();
                }
                finally
                {
                    if (null != conn)
                    {
                        dbMgr.DBConns.PushDBConnection(conn);
                    }
                }
                result = list;
            }
            return result;
        }

        // Token: 0x0600023F RID: 575 RVA: 0x0000E108 File Offset: 0x0000C308
        public static List<PaiHangItemData> GetRoleEquipPaiHang(DBManager dbMgr)
        {
            return DBQuery.GetRoleTablePaiHang(dbMgr, "equipjifen", " WHERE equipjifen>0 AND isdel=0 AND isflashplayer=0");
        }

        // Token: 0x06000240 RID: 576 RVA: 0x0000E12C File Offset: 0x0000C32C
        public static List<PaiHangItemData> GetRoleXueWeiNumPaiHang(DBManager dbMgr)
        {
            return DBQuery.GetRoleTablePaiHang(dbMgr, "xueweinum", " WHERE xueweinum>=20 AND isdel=0");
        }

        // Token: 0x06000241 RID: 577 RVA: 0x0000E150 File Offset: 0x0000C350
        public static List<PaiHangItemData> GetRoleSkillLevelPaiHang(DBManager dbMgr)
        {
            return DBQuery.GetRoleTablePaiHang(dbMgr, "skilllearnednum", " WHERE skilllearnednum>=60 AND isdel=0");
        }

        // Token: 0x06000242 RID: 578 RVA: 0x0000E174 File Offset: 0x0000C374
        public static List<PaiHangItemData> GetRoleHorseJiFenPaiHang(DBManager dbMgr)
        {
            return DBQuery.GetRoleTablePaiHang(dbMgr, "horsejifen", " WHERE horsejifen>=54 AND isdel=0 AND isflashplayer=0");
        }

        // Token: 0x06000243 RID: 579 RVA: 0x0000E198 File Offset: 0x0000C398
        public static List<PaiHangItemData> GetRoleLevelPaiHang(DBManager dbMgr)
        {
            List<PaiHangItemData> list = new List<PaiHangItemData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT rid, rname, zoneid, level, changelifecount, admiredcount FROM t_roles WHERE level>0 AND isdel=0 AND isflashplayer=0 ORDER BY changelifecount DESC, level DESC, experience DESC LIMIT 100", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                int count = 0;
                while (reader.Read() && count < 100)
                {
                    PaiHangItemData paiHangItemData = new PaiHangItemData
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(reader["zoneid"].ToString(), reader["rname"].ToString()),
                        Val1 = Convert.ToInt32(reader["level"].ToString()),
                        Val2 = Convert.ToInt32(reader["changelifecount"].ToString()),
                        Val3 = Convert.ToInt32(reader["admiredcount"].ToString())
                    };
                    list.Add(paiHangItemData);
                    count++;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x06000244 RID: 580 RVA: 0x0000E324 File Offset: 0x0000C524
        public static List<PaiHangItemData> GetRoleYinLiangPaiHang(DBManager dbMgr)
        {
            return DBQuery.GetRoleTablePaiHang(dbMgr, "yinliang", " WHERE yinliang>0 AND isdel=0 AND isflashplayer=0");
        }

        // Token: 0x06000245 RID: 581 RVA: 0x0000E348 File Offset: 0x0000C548
        public static List<PaiHangItemData> GetRoleGoldPaiHang(DBManager dbMgr)
        {
            return DBQuery.GetRoleTablePaiHang(dbMgr, "money2", " WHERE money2>0 AND isdel=0 AND isflashplayer=0");
        }

        // Token: 0x06000246 RID: 582 RVA: 0x0000E36C File Offset: 0x0000C56C
        public static List<PaiHangItemData> GetRoleLianZhanPaiHang(DBManager dbMgr)
        {
            return DBQuery.GetRoleTablePaiHang(dbMgr, "lianzhan", " WHERE lianzhan>=100 AND isdel=0 AND isflashplayer=0");
        }

        // Token: 0x06000247 RID: 583 RVA: 0x0000E390 File Offset: 0x0000C590
        public static List<PaiHangItemData> GetRoleKillBossPaiHang(DBManager dbMgr)
        {
            return DBQuery.GetRoleTablePaiHang(dbMgr, "killboss", " WHERE killboss>=1 AND isdel=0 AND isflashplayer=0");
        }

        // Token: 0x06000248 RID: 584 RVA: 0x0000E3B4 File Offset: 0x0000C5B4
        public static List<PaiHangItemData> GetRoleBattleNumPaiHang(DBManager dbMgr)
        {
            return DBQuery.GetRoleTablePaiHang(dbMgr, "battlenum", " WHERE battlenum>=1 AND isdel=0 AND isflashplayer=0");
        }

        // Token: 0x06000249 RID: 585 RVA: 0x0000E3D8 File Offset: 0x0000C5D8
        public static List<PaiHangItemData> GetRoleHeroIndexPaiHang(DBManager dbMgr)
        {
            return DBQuery.GetRoleTablePaiHang(dbMgr, "heroindex", " WHERE heroindex>=1 AND isdel=0 AND isflashplayer=0");
        }

        // Token: 0x0600024A RID: 586 RVA: 0x0000E3FC File Offset: 0x0000C5FC
        public static List<PaiHangItemData> GetRoleCombatForcePaiHang(DBManager dbMgr)
        {
            return DBQuery.GetRoleTablePaiHang(dbMgr, "combatforce", " WHERE combatforce>=1 AND isdel=0 AND isflashplayer=0");
        }

        // Token: 0x0600024B RID: 587 RVA: 0x0000E420 File Offset: 0x0000C620
        public static List<PaiHangItemData> GetUserMoneyPaiHang(DBManager dbMgr)
        {
            return DBQuery.GetUserMoneyTablePaiHang(dbMgr, "money", " WHERE money>0");
        }

        // Token: 0x0600024C RID: 588 RVA: 0x0000E444 File Offset: 0x0000C644
        public static List<PaiHangItemData> GetRoleGuardStatuePaiHang(DBManager dbMgr)
        {
            List<PaiHangItemData> list = new List<PaiHangItemData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT roleid, suit, level FROM t_guard_statue ORDER BY suit DESC, level DESC, roleid ASC LIMIT 100", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                int count = 0;
                while (reader.Read() && count < 100)
                {
                    PaiHangItemData paiHangItemData = new PaiHangItemData
                    {
                        RoleID = Convert.ToInt32(reader["roleid"].ToString()),
                        Val1 = Convert.ToInt32(reader["suit"].ToString()),
                        Val2 = Convert.ToInt32(reader["level"].ToString())
                    };
                    list.Add(paiHangItemData);
                    count++;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x0600024D RID: 589 RVA: 0x0000E574 File Offset: 0x0000C774
        public static List<PaiHangItemData> GetRoleHolyItemPaiHang(DBManager dbMgr)
        {
            List<PaiHangItemData> list = new List<PaiHangItemData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT roleid, SUM(part_suit) AS lev FROM t_holyitem GROUP BY roleid ORDER BY lev DESC, roleid ASC LIMIT 100", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                int count = 0;
                while (reader.Read() && count < 100)
                {
                    PaiHangItemData paiHangItemData = new PaiHangItemData
                    {
                        RoleID = Convert.ToInt32(reader["roleid"].ToString()),
                        Val1 = Convert.ToInt32(reader["lev"].ToString())
                    };
                    list.Add(paiHangItemData);
                    count++;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x0600024E RID: 590 RVA: 0x0000E684 File Offset: 0x0000C884
        public static BangHuiListData GetBangHuiItemDataList(DBManager dbMgr, int isVerify, int startIndex, int endIndex)
        {
            BangHuiListData bangHuiListData = new BangHuiListData();
            bangHuiListData.TotalBangHuiItemNum = 0;
            bangHuiListData.BangHuiItemDataList = new List<BangHuiItemData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT b.bhid, b.bhname, b.zoneid, b.rid, r.rname, r.occupation, b.totalnum, b.totallevel, b.qilevel, b.isverfiy, b.totalcombatforce FROM t_banghui AS b, t_roles AS r WHERE b.isdel=0 AND b.rid=r.rid ORDER BY b.totalcombatforce DESC", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    if (isVerify < 0 || Convert.ToInt32(reader["isverfiy"].ToString()) == isVerify)
                    {
                        BangHuiItemData bangHuiItemData = new BangHuiItemData
                        {
                            BHID = Convert.ToInt32(reader["bhid"].ToString()),
                            BHName = reader["bhname"].ToString(),
                            ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
                            BZRoleID = Convert.ToInt32(reader["rid"].ToString()),
                            BZRoleName = reader["rname"].ToString(),
                            BZOccupation = Convert.ToInt32(reader["occupation"].ToString()),
                            TotalNum = Convert.ToInt32(reader["totalnum"].ToString()),
                            TotalLevel = Convert.ToInt32(reader["totallevel"].ToString()),
                            QiLevel = Convert.ToInt32(reader["qilevel"].ToString()),
                            IsVerfiy = Convert.ToInt32(reader["isverfiy"].ToString()),
                            TotalCombatForce = Convert.ToInt32(reader["totalcombatforce"].ToString())
                        };
                        bangHuiListData.BangHuiItemDataList.Add(bangHuiItemData);
                        bangHuiListData.TotalBangHuiItemNum++;
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return bangHuiListData;
        }

        // Token: 0x0600024F RID: 591 RVA: 0x0000E8E0 File Offset: 0x0000CAE0
        public static int FindBangHuiByRoleID(DBManager dbMgr, int roleID)
        {
            int bhid = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT bhid FROM t_banghui WHERE isdel=0 AND rid={0}", roleID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    bhid = Convert.ToInt32(reader["bhid"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return bhid;
        }

        // Token: 0x06000250 RID: 592 RVA: 0x0000E9A0 File Offset: 0x0000CBA0
        public static int FindJoinBangHuiByRoleID(DBManager dbMgr, int roleID)
        {
            int bhid = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT faction FROM t_roles WHERE rid={0}", roleID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    bhid = Convert.ToInt32(reader["faction"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return bhid;
        }

        // Token: 0x06000251 RID: 593 RVA: 0x0000EA60 File Offset: 0x0000CC60
        public static bool QueryBHMemberSumData(int bhid, out int totalNum, out int totalLevel, out long totalCombatforce)
        {
            totalCombatforce = (long)(totalNum = (totalLevel = 0));
            try
            {
                using (MyDbConnection3 conn = new MyDbConnection3(false))
                {
                    string sqlText = string.Format("SELECT SUM(combatforce) AS totalcombatforce,SUM(LEVEL) AS totallevel,COUNT(rid) AS totalnum FROM t_roles WHERE isdel=0 and faction={0}", bhid);
                    MySQLDataReader reader = conn.ExecuteReader(sqlText, new MySQLParameter[0]);
                    if (reader.Read())
                    {
                        totalCombatforce = Convert.ToInt64(reader["totalcombatforce"].ToString());
                        totalLevel = Convert.ToInt32(reader["totallevel"].ToString());
                        totalNum = Convert.ToInt32(reader["totalnum"].ToString());
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteException(ex.ToString());
            }
            return false;
        }

        // Token: 0x06000252 RID: 594 RVA: 0x0000EB4C File Offset: 0x0000CD4C
        public static int QueryBHMemberNum(DBManager dbMgr, int bhid)
        {
            int totalNum = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT COUNT(rid) AS totalnum FROM t_roles WHERE isdel=0 AND faction={0}", bhid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    totalNum = Convert.ToInt32(reader["totalnum"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return totalNum;
        }

        // Token: 0x06000253 RID: 595 RVA: 0x0000EC0C File Offset: 0x0000CE0C
        public static int QueryBHMemberLevel(DBManager dbMgr, int bhid)
        {
            int totalLevel = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT SUM(level) AS totallevel FROM t_roles WHERE isdel=0 AND faction={0}", bhid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    try
                    {
                        totalLevel = Convert.ToInt32(reader["totallevel"].ToString());
                    }
                    catch (Exception)
                    {
                        totalLevel = 0;
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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
            return totalLevel;
        }

        // Token: 0x06000254 RID: 596 RVA: 0x0000ECE4 File Offset: 0x0000CEE4
        public static int QueryBHMemberTotalCombatForce(DBManager dbMgr, int bhid)
        {
            int totalcombatforce = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT SUM(combatforce) AS totalcombatforce FROM t_roles WHERE isdel=0 AND faction={0}", bhid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    try
                    {
                        totalcombatforce = Convert.ToInt32(reader["totalcombatforce"].ToString());
                    }
                    catch (Exception)
                    {
                        totalcombatforce = 0;
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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
            return totalcombatforce;
        }

        // Token: 0x06000255 RID: 597 RVA: 0x0000EDBC File Offset: 0x0000CFBC
        public static int[] QueryZhengDuoUsedTime(DBManager dbMgr, int bhid)
        {
            int[] args = new int[2];
            using (MyDbConnection3 conn = new MyDbConnection3(false))
            {
                string cmdText = string.Format("select zhengduoweek,zhengduousedtime from t_banghui WHERE bhid={0}", bhid);
                MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
                if (reader.Read())
                {
                    args[0] = Convert.ToInt32(reader["zhengduoweek"].ToString());
                    args[1] = Convert.ToInt32(reader["zhengduousedtime"].ToString());
                }
            }
            return args;
        }

        // Token: 0x06000256 RID: 598 RVA: 0x0000EE68 File Offset: 0x0000D068
        public static BangHuiDetailData QueryBangHuiInfoByID(DBManager dbMgr, int bhid)
        {
            BangHuiDetailData bangHuiDetailData = null;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT b.bhid, b.bhname, b.zoneid, b.rid, r.rname, r.occupation, b.totalnum, b.totallevel, b.bhbulletin, b.buildtime, b.qiname, b.qilevel, b.isverfiy, b.tongqian, b.jitan, b.junxie, b.guanghuan, b.can_mod_name_times,b.totalcombatforce,b.zhengduousedtime FROM t_banghui AS b, t_roles AS r WHERE b.isdel=0 AND b.rid=r.rid AND b.bhid={0}", bhid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    bangHuiDetailData = new BangHuiDetailData();
                    bangHuiDetailData.BHID = Convert.ToInt32(reader["bhid"].ToString());
                    bangHuiDetailData.BHName = reader["bhname"].ToString();
                    bangHuiDetailData.ZoneID = Convert.ToInt32(reader["zoneid"].ToString());
                    bangHuiDetailData.BZRoleID = Convert.ToInt32(reader["rid"].ToString());
                    bangHuiDetailData.BZRoleName = reader["rname"].ToString();
                    bangHuiDetailData.BZOccupation = Convert.ToInt32(reader["occupation"].ToString());
                    bangHuiDetailData.TotalNum = Convert.ToInt32(reader["totalnum"].ToString());
                    bangHuiDetailData.TotalLevel = Convert.ToInt32(reader["totallevel"].ToString());
                    bangHuiDetailData.BHBulletin = reader["bhbulletin"].ToString();
                    bangHuiDetailData.BuildTime = reader["buildtime"].ToString();
                    bangHuiDetailData.QiName = reader["qiname"].ToString();
                    bangHuiDetailData.QiLevel = Convert.ToInt32(reader["qilevel"].ToString());
                    bangHuiDetailData.IsVerify = Convert.ToInt32(reader["isverfiy"].ToString());
                    bangHuiDetailData.TotalMoney = Convert.ToInt32(reader["tongqian"].ToString());
                    bangHuiDetailData.JiTan = Convert.ToInt32(reader["jitan"].ToString());
                    bangHuiDetailData.JunXie = Convert.ToInt32(reader["junxie"].ToString());
                    bangHuiDetailData.GuangHuan = Convert.ToInt32(reader["guanghuan"].ToString());
                    bangHuiDetailData.CanModNameTimes = Convert.ToInt32(reader["can_mod_name_times"].ToString());
                    bangHuiDetailData.TotalCombatForce = (long)Convert.ToInt32(reader["totalcombatforce"].ToString());
                    bangHuiDetailData.ZhengDuoUsedTime = (long)Convert.ToInt32(reader["zhengduousedtime"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return bangHuiDetailData;
        }

        // Token: 0x06000257 RID: 599 RVA: 0x0000F140 File Offset: 0x0000D340
        public static List<BHMatchSupportData> LoadBHMatchSupportFlagData(DBManager dbMgr, int rid, int minSeasonID, int minRound)
        {
            List<BHMatchSupportData> bhMatchSupportData = new List<BHMatchSupportData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT * FROM t_banghui_match_support_flag WHERE rid={0} AND (season>{1} OR (season={1} AND `round`>={2}))", rid, minSeasonID, minRound);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    bhMatchSupportData.Add(new BHMatchSupportData
                    {
                        season = Convert.ToInt32(reader["season"].ToString()),
                        round = Convert.ToInt32(reader["round"].ToString()),
                        bhid1 = Convert.ToInt32(reader["bhid1"].ToString()),
                        bhid2 = Convert.ToInt32(reader["bhid2"].ToString()),
                        guess = Convert.ToInt32(reader["guess"].ToString()),
                        rid = Convert.ToInt32(reader["rid"].ToString()),
                        isaward = Convert.ToByte(reader["is_award"].ToString())
                    });
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return bhMatchSupportData;
        }

        // Token: 0x06000258 RID: 600 RVA: 0x0000F2E8 File Offset: 0x0000D4E8
        public static List<BangHuiMgrItemData> GetBangHuiMgrItemItemDataList(DBManager dbMgr, int bhid)
        {
            List<BangHuiMgrItemData> list = new List<BangHuiMgrItemData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT r.zoneid, r.rid, r.rname, r.occupation, r.bhzhiwu, r.chenghao, r.banggong, r.level FROM t_roles AS r WHERE r.bhzhiwu>0 AND r.faction={0} and r.isdel=0", bhid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    BangHuiMgrItemData bangHuiMgrItemData = new BangHuiMgrItemData
                    {
                        ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = reader["rname"].ToString(),
                        Occupation = Convert.ToInt32(reader["occupation"].ToString()),
                        BHZhiwu = Convert.ToInt32(reader["bhzhiwu"].ToString()),
                        ChengHao = reader["chenghao"].ToString(),
                        BangGong = Convert.ToInt32(reader["banggong"].ToString()),
                        Level = Convert.ToInt32(reader["level"].ToString())
                    };
                    list.Add(bangHuiMgrItemData);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x06000259 RID: 601 RVA: 0x0000F498 File Offset: 0x0000D698
        public static BangHuiMgrItemData GetBangHuiMgrItemItemDataByID(DBManager dbMgr, int bhid, int roleID)
        {
            BangHuiMgrItemData bangHuiMgrItemData = null;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT r.zoneid, r.rid, r.rname, r.occupation, r.bhzhiwu, r.chenghao, r.banggong, r.level FROM t_roles AS r WHERE r.bhzhiwu>0 AND r.faction={0} AND r.rid={1}", bhid, roleID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    bangHuiMgrItemData = new BangHuiMgrItemData
                    {
                        ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = reader["rname"].ToString(),
                        Occupation = Convert.ToInt32(reader["occupation"].ToString()),
                        BHZhiwu = Convert.ToInt32(reader["bhzhiwu"].ToString()),
                        ChengHao = reader["chenghao"].ToString(),
                        BangGong = Convert.ToInt32(reader["banggong"].ToString()),
                        Level = Convert.ToInt32(reader["level"].ToString())
                    };
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return bangHuiMgrItemData;
        }

        // Token: 0x0600025A RID: 602 RVA: 0x0000F640 File Offset: 0x0000D840
        public static List<BangHuiMemberData> GetBangHuiMemberDataList(DBManager dbMgr, int bhid)
        {
            List<BangHuiMemberData> list = new List<BangHuiMemberData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT r.zoneid, r.rid, r.rname, r.occupation, r.bhzhiwu, r.chenghao, r.banggong, r.level, r.xueweinum, r.skilllearnednum, r.combatforce, r.changelifecount,r.juntuanzhiwu FROM t_roles AS r WHERE r.faction={0} AND r.isdel=0", bhid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    BangHuiMemberData bangHuiMemberData = new BangHuiMemberData
                    {
                        ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = reader["rname"].ToString(),
                        Occupation = Convert.ToInt32(reader["occupation"].ToString()),
                        BHZhiwu = Convert.ToInt32(reader["bhzhiwu"].ToString()),
                        ChengHao = reader["chenghao"].ToString(),
                        BangGong = Convert.ToInt32(reader["banggong"].ToString()),
                        Level = Convert.ToInt32(reader["level"].ToString()),
                        XueWeiNum = Convert.ToInt32(reader["xueweinum"].ToString()),
                        SkillLearnedNum = Convert.ToInt32(reader["skilllearnednum"].ToString()),
                        BangHuiMemberCombatForce = Convert.ToInt32(reader["combatforce"].ToString()),
                        BangHuiMemberChangeLifeLev = Convert.ToInt32(reader["changelifecount"].ToString()),
                        JunTuanZhiWu = Convert.ToInt32(reader["juntuanzhiwu"].ToString())
                    };
                    list.Add(bangHuiMemberData);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x0600025B RID: 603 RVA: 0x0000F884 File Offset: 0x0000DA84
        public static string GetBangHuiMgrItemItemStringList(DBManager dbMgr, int bhid)
        {
            StringBuilder sb = new StringBuilder();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT r.zoneid, r.rid, r.rname, r.occupation, r.bhzhiwu, r.chenghao, r.banggong FROM t_roles AS r WHERE r.bhzhiwu>0 AND r.faction={0}", bhid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    sb.AppendFormat("{0},", reader["rid"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return sb.ToString();
        }

        // Token: 0x0600025C RID: 604 RVA: 0x0000F954 File Offset: 0x0000DB54
        public static BangHuiBagData QueryBangHuiBagDataByID(DBManager dbMgr, int bhid)
        {
            BangHuiBagData bangHuiBagData = new BangHuiBagData();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT goods1num, goods2num, goods3num, goods4num, goods5num, tongqian FROM t_banghui WHERE isdel=0 AND bhid={0}", bhid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    bangHuiBagData.Goods1Num = Convert.ToInt32(reader["goods1num"].ToString());
                    bangHuiBagData.Goods2Num = Convert.ToInt32(reader["goods2num"].ToString());
                    bangHuiBagData.Goods3Num = Convert.ToInt32(reader["goods3num"].ToString());
                    bangHuiBagData.Goods4Num = Convert.ToInt32(reader["goods4num"].ToString());
                    bangHuiBagData.Goods5Num = Convert.ToInt32(reader["goods5num"].ToString());
                    bangHuiBagData.TongQian = Convert.ToInt32(reader["tongqian"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return bangHuiBagData;
        }

        // Token: 0x0600025D RID: 605 RVA: 0x0000FAB8 File Offset: 0x0000DCB8
        public static List<BangGongHistData> GetBangHuiBagHistList(DBManager dbMgr, int bhid)
        {
            List<BangGongHistData> bangGongHistDataList = new List<BangGongHistData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT r.zoneid, r.rid, r.rname, r.occupation, r.level, r.bhzhiwu, r.chenghao, b.goods1num, b.goods2num, b.goods3num, b.goods4num, b.goods5num, b.tongqian, b.banggong FROM t_banggonghist AS b, t_roles AS r WHERE b.bhid={0} AND b.rid=r.rid ORDER BY b.banggong DESC", bhid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    BangGongHistData bangGongHistData = new BangGongHistData
                    {
                        ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = reader["rname"].ToString(),
                        Occupation = Convert.ToInt32(reader["occupation"].ToString()),
                        RoleLevel = Convert.ToInt32(reader["level"].ToString()),
                        BHZhiWu = Convert.ToInt32(reader["bhzhiwu"].ToString()),
                        BHChengHao = reader["chenghao"].ToString(),
                        Goods1Num = Convert.ToInt32(reader["goods1num"].ToString()),
                        Goods2Num = Convert.ToInt32(reader["goods2num"].ToString()),
                        Goods3Num = Convert.ToInt32(reader["goods3num"].ToString()),
                        Goods4Num = Convert.ToInt32(reader["goods4num"].ToString()),
                        Goods5Num = Convert.ToInt32(reader["goods5num"].ToString()),
                        TongQian = Convert.ToInt32(reader["tongqian"].ToString()),
                        BangGong = Convert.ToInt32(reader["banggong"].ToString())
                    };
                    bangGongHistDataList.Add(bangGongHistData);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return bangGongHistDataList;
        }

        // Token: 0x0600025E RID: 606 RVA: 0x0000FD18 File Offset: 0x0000DF18
        public static BangQiInfoData QueryBangQiInfoByID(DBManager dbMgr, int bhid)
        {
            BangQiInfoData bangQiInfoData = new BangQiInfoData();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT b.qiname, b.qilevel FROM t_banghui AS b WHERE b.isdel=0 AND b.bhid={0}", bhid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    bangQiInfoData.BangQiName = reader["qiname"].ToString();
                    bangQiInfoData.BangQiLevel = Convert.ToInt32(reader["qilevel"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return bangQiInfoData;
        }

        // Token: 0x0600025F RID: 607 RVA: 0x0000FDF8 File Offset: 0x0000DFF8
        public static Dictionary<int, BHLingDiOwnData> GetBHLingDiOwnDataDict(DBManager dbMgr)
        {
            Dictionary<int, BHLingDiOwnData> bhLingDiOwnDataDict = new Dictionary<int, BHLingDiOwnData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT l.lingdi, b.zoneid, b.bhid, b.bhname, b.qiname, b.qilevel FROM t_banghui AS b, t_lingdi AS l WHERE b.bhid=l.bhid AND b.isdel=0", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    BHLingDiOwnData bhLingDiOwnData = new BHLingDiOwnData
                    {
                        LingDiID = Convert.ToInt32(reader["lingdi"].ToString()),
                        ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
                        BHID = Convert.ToInt32(reader["bhid"].ToString()),
                        BHName = reader["bhname"].ToString(),
                        BangQiName = reader["qiname"].ToString(),
                        BangQiLevel = Convert.ToInt32(reader["qilevel"].ToString())
                    };
                    bhLingDiOwnDataDict[bhLingDiOwnData.LingDiID] = bhLingDiOwnData;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return bhLingDiOwnDataDict;
        }

        // Token: 0x06000260 RID: 608 RVA: 0x0000FF78 File Offset: 0x0000E178
        public static void QueryPreDeleteRoleDict(DBManager dbMgr, Dictionary<int, DateTime> preDeleteRoleDict)
        {
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT rid, predeltime FROM t_roles WHERE isdel=0 and predeltime IS NOT NULL", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    int rid = Convert.ToInt32(reader["rid"].ToString());
                    DateTime preDelTime;
                    if (DateTime.TryParse(reader["predeltime"].ToString(), out preDelTime))
                    {
                        preDeleteRoleDict[rid] = preDelTime;
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
        }

        // Token: 0x06000261 RID: 609 RVA: 0x0001005C File Offset: 0x0000E25C
        public static void QueryBangQiDict(DBManager dbMgr, Dictionary<int, BangHuiJunQiItemData> bangHuiJunQiItemDcit)
        {
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT b.bhid, b.qiname, b.qilevel FROM t_banghui AS b WHERE b.isdel=0", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    int bhid = Convert.ToInt32(reader["bhid"].ToString());
                    bangHuiJunQiItemDcit[bhid] = new BangHuiJunQiItemData
                    {
                        BHID = bhid,
                        QiName = reader["qiname"].ToString(),
                        QiLevel = Convert.ToInt32(reader["qilevel"].ToString())
                    };
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
        }

        // Token: 0x06000262 RID: 610 RVA: 0x00010160 File Offset: 0x0000E360
        public static void QueryBHLingDiInfoDict(DBManager dbMgr, Dictionary<int, BangHuiLingDiInfoData> bangHuiLingDiItemsDict)
        {
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT l.lingdi, l.bhid, 1 as zoneid, \"\" as bhname, l.tax, l.takedayid, l.takedaynum, l.yestodaytax, l.taxdayid, l.todaytax, l.totaltax, l.warrequest, l.awardfetchday FROM t_lingdi AS l", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    BangHuiLingDiInfoData bangHuiLingDiInfoData = new BangHuiLingDiInfoData
                    {
                        LingDiID = Convert.ToInt32(reader["lingdi"].ToString()),
                        BHID = Convert.ToInt32(reader["bhid"].ToString()),
                        ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
                        BHName = reader["bhname"].ToString(),
                        LingDiTax = Convert.ToInt32(reader["tax"].ToString()),
                        TakeDayID = Convert.ToInt32(reader["takedayid"].ToString()),
                        TakeDayNum = Convert.ToInt32(reader["takedaynum"].ToString()),
                        YestodayTax = Convert.ToInt32(reader["yestodaytax"].ToString()),
                        TaxDayID = Convert.ToInt32(reader["taxdayid"].ToString()),
                        TodayTax = Convert.ToInt32(reader["todaytax"].ToString()),
                        TotalTax = Convert.ToInt32(reader["TotalTax"].ToString()),
                        AwardFetchDay = Convert.ToInt32(reader["awardfetchday"].ToString())
                    };
                    byte[] warReqArr = reader["warrequest"] as byte[];
                    if (null == warReqArr)
                    {
                        bangHuiLingDiInfoData.WarRequest = "";
                    }
                    else
                    {
                        bangHuiLingDiInfoData.WarRequest = Encoding.Default.GetString(warReqArr);
                    }
                    bangHuiLingDiItemsDict[bangHuiLingDiInfoData.LingDiID] = bangHuiLingDiInfoData;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
        }

        // Token: 0x06000263 RID: 611 RVA: 0x000103BC File Offset: 0x0000E5BC
        public static string QueryBangFuBenByID(DBManager dbMgr, int bhid)
        {
            string result = "";
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT b.fubenid, b.fubenstate, b.openday, b.killers FROM t_banghui AS b WHERE b.isdel=0 AND b.bhid={0}", bhid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    result = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
                    {
                        bhid,
                        Convert.ToInt32(reader["fubenid"].ToString()),
                        Convert.ToInt32(reader["fubenstate"].ToString()),
                        Convert.ToInt32(reader["openday"].ToString()),
                        reader["killers"].ToString()
                    });
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return result;
        }

        // Token: 0x06000264 RID: 612 RVA: 0x00010500 File Offset: 0x0000E700
        public static int QueryHuangFeiCount(DBManager dbMgr)
        {
            int ret = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT COUNT(rid) AS huanghounum FROM t_roles WHERE isdel=0 AND huanghou=1", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    ret = Convert.ToInt32(reader["huanghounum"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return ret;
        }

        // Token: 0x06000265 RID: 613 RVA: 0x000105C0 File Offset: 0x0000E7C0
        public static List<SearchRoleData> QueryHuangFeiDataList(DBManager dbMgr)
        {
            List<SearchRoleData> huangFeiDataList = new List<SearchRoleData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT rid, rname, sex, occupation, level, zoneid, faction, bhname FROM t_roles WHERE isdel=0 AND huanghou=1", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    SearchRoleData searchRoleData = new SearchRoleData
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(Convert.ToInt32(reader["zoneid"].ToString()), reader["rname"].ToString()),
                        RoleSex = Convert.ToInt32(reader["sex"].ToString()),
                        Level = Convert.ToInt32(reader["level"].ToString()),
                        Occupation = Convert.ToInt32(reader["occupation"].ToString()),
                        Faction = Convert.ToInt32(reader["faction"].ToString()),
                        BHName = reader["bhname"].ToString()
                    };
                    huangFeiDataList.Add(searchRoleData);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return huangFeiDataList;
        }

        // Token: 0x06000266 RID: 614 RVA: 0x00010770 File Offset: 0x0000E970
        public static HuangDiTeQuanItem LoadHuangDiTeQuan(DBManager dbMgr)
        {
            HuangDiTeQuanItem huangDiTeQuanItem = null;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT Id, tolaofangdayid, tolaofangnum, offlaofangdayid, offlaofangnum, bancatdayid, bancatnum FROM t_hdtequan WHERE Id=1", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    huangDiTeQuanItem = new HuangDiTeQuanItem
                    {
                        ID = Convert.ToInt32(reader["Id"].ToString()),
                        ToLaoFangDayID = Convert.ToInt32(reader["tolaofangdayid"].ToString()),
                        ToLaoFangNum = Convert.ToInt32(reader["tolaofangnum"].ToString()),
                        OffLaoFangDayID = Convert.ToInt32(reader["offlaofangdayid"].ToString()),
                        OffLaoFangNum = Convert.ToInt32(reader["offlaofangnum"].ToString()),
                        BanCatDayID = Convert.ToInt32(reader["bancatdayid"].ToString()),
                        BanCatNum = Convert.ToInt32(reader["bancatnum"].ToString())
                    };
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return huangDiTeQuanItem;
        }

        // Token: 0x06000267 RID: 615 RVA: 0x00010900 File Offset: 0x0000EB00
        public static List<int> GetNoMoneyBangHuiList(DBManager dbMgr, int maxQiLevel)
        {
            List<int> noMoneyBangHuiList = new List<int>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT bhid FROM t_banghui WHERE tongqian<0 and isdel=0 and qilevel<{0}", maxQiLevel);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    noMoneyBangHuiList.Add(Convert.ToInt32(reader["bhid"].ToString()));
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return noMoneyBangHuiList;
        }

        // Token: 0x06000268 RID: 616 RVA: 0x000109CC File Offset: 0x0000EBCC
        public static List<QizhenGeBuItemData> QueryQizhenGeBuItemDataList(DBManager dbMgr)
        {
            List<QizhenGeBuItemData> list = new List<QizhenGeBuItemData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT r.rid, r.rname, r.zoneid, q.goodsid, q.goodsnum FROM t_roles AS r, t_qizhengebuy AS q WHERE q.rid=r.rid ORDER BY buytime DESC LIMIT 10", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                int count = 0;
                while (reader.Read() && count < 100)
                {
                    QizhenGeBuItemData qizhenGeBuItemData = new QizhenGeBuItemData
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(reader["zoneid"].ToString(), reader["rname"].ToString()),
                        GoodsID = Convert.ToInt32(reader["goodsid"].ToString()),
                        GoodsNum = Convert.ToInt32(reader["goodsnum"].ToString())
                    };
                    list.Add(qizhenGeBuItemData);
                    count++;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x06000269 RID: 617 RVA: 0x00010B38 File Offset: 0x0000ED38
        public static int QueryQiangGouBuyItemNumByRoleID(DBManager dbMgr, int roleID, int goodsID, int qiangGouID, int random, int actStartDay)
        {
            int count = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "";
                if (random <= 0)
                {
                    cmdText = string.Format("SELECT SUM(goodsnum) AS totalgoodsnum FROM t_qianggoubuy WHERE rid={0} and goodsid={1} and qianggouid={2} and actstartday={3}", new object[]
                    {
                        roleID,
                        goodsID,
                        qiangGouID,
                        actStartDay
                    });
                }
                else
                {
                    DateTime now = DateTime.Now;
                    string todayStart = string.Format("{0:0000}-{1:00}-{2:00} 00:01:01", now.Year, now.Month, now.Day);
                    string todayEnd = string.Format("{0:0000}-{1:00}-{2:00} 23:59:59", now.Year, now.Month, now.Day);
                    cmdText = string.Format("SELECT SUM(goodsnum) AS totalgoodsnum FROM t_qianggoubuy WHERE rid={0} and goodsid={1} and qianggouid={2} and buytime>='{3}' and buytime<='{4}'", new object[]
                    {
                        roleID,
                        goodsID,
                        qiangGouID,
                        todayStart,
                        todayEnd
                    });
                }
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    try
                    {
                        count = Convert.ToInt32(reader["totalgoodsnum"].ToString());
                    }
                    catch (Exception)
                    {
                        count = 0;
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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
            return count;
        }

        // Token: 0x0600026A RID: 618 RVA: 0x00010D18 File Offset: 0x0000EF18
        public static int QueryQiangGouBuyItemNum(DBManager dbMgr, int goodsID, int qiangGouID, int random, int actStartDay)
        {
            int count = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "";
                if (random <= 0)
                {
                    cmdText = string.Format("SELECT SUM(goodsnum) AS totalgoodsnum FROM t_qianggoubuy WHERE goodsid={0} and qianggouid={1} and actstartday={2}", goodsID, qiangGouID, actStartDay);
                }
                else
                {
                    DateTime now = DateTime.Now;
                    string todayStart = string.Format("{0:0000}-{1:00}-{2:00} 00:01:01", now.Year, now.Month, now.Day);
                    string todayEnd = string.Format("{0:0000}-{1:00}-{2:00} 23:59:59", now.Year, now.Month, now.Day);
                    cmdText = string.Format("SELECT SUM(goodsnum) AS totalgoodsnum FROM t_qianggoubuy WHERE goodsid={0} and qianggouid={1} and buytime>='{2}' and buytime<='{3}'", new object[]
                    {
                        goodsID,
                        qiangGouID,
                        todayStart,
                        todayEnd
                    });
                }
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    try
                    {
                        count = Convert.ToInt32(reader["totalgoodsnum"].ToString());
                    }
                    catch (Exception)
                    {
                        count = 0;
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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
            return count;
        }

        // Token: 0x0600026B RID: 619 RVA: 0x00010ED0 File Offset: 0x0000F0D0
        public static List<ShengXiaoGuessHistory> QueryShengXiaoGuessHistoryDataList(DBManager dbMgr, int roleID = -1)
        {
            List<ShengXiaoGuessHistory> list = new List<ShengXiaoGuessHistory>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string sWhere = " Where gainnum>0 ";
                if (roleID > 0)
                {
                    sWhere += string.Format(" and rid={0} ", roleID);
                }
                string cmdText = string.Format("SELECT * FROM t_shengxiaoguesshist {0} ORDER BY guesstime DESC LIMIT 15", sWhere);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                int count = 0;
                while (reader.Read() && count < 100)
                {
                    ShengXiaoGuessHistory historyData = new ShengXiaoGuessHistory
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(reader["zoneid"].ToString(), reader["rname"].ToString()),
                        GuessKey = Convert.ToInt32(reader["guesskey"].ToString()),
                        Mortgage = Convert.ToInt32(reader["mortgage"].ToString()),
                        ResultKey = Convert.ToInt32(reader["resultkey"].ToString()),
                        GainNum = Convert.ToInt32(reader["gainnum"].ToString()),
                        LeftMortgage = Convert.ToInt32(reader["leftmortgage"].ToString()),
                        GuessTime = reader["guesstime"].ToString()
                    };
                    list.Add(historyData);
                    count++;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x0600026C RID: 620 RVA: 0x000110D8 File Offset: 0x0000F2D8
        public static int QueryPingTaiIDByHuoDongID(DBManager dbMgr, int huodongID, int rid, int pingTaiID)
        {
            int ret = -1;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT ptid FROM t_usedlipinma WHERE huodongid={0} AND rid={1} AND ptid={2}", huodongID, rid, pingTaiID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    ret = Convert.ToInt32(reader["ptid"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return ret;
        }

        // Token: 0x0600026D RID: 621 RVA: 0x000111A4 File Offset: 0x0000F3A4
        public static int QueryUseNumByHuoDongID(DBManager dbMgr, int huodongID, int rid, int pingTaiID)
        {
            int ret = -1;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT count(ptid) as ptidCount FROM t_usedlipinma WHERE huodongid={0} AND rid={1} AND ptid={2}", huodongID, rid, pingTaiID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    ret = Convert.ToInt32(reader["ptidCount"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return ret;
        }

        // Token: 0x0600026E RID: 622 RVA: 0x00011270 File Offset: 0x0000F470
        public static int QueryTotalChongZhiMoney(DBManager dbMgr, string userID, int zoneID)
        {
            int ret = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT SUM(amount) AS totalmoney FROM t_inputlog WHERE u='{0}'", userID, zoneID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    try
                    {
                        ret = Convert.ToInt32(reader["totalmoney"].ToString());
                    }
                    catch (Exception)
                    {
                        ret = 0;
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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
            return ret;
        }

        // Token: 0x0600026F RID: 623 RVA: 0x0001134C File Offset: 0x0000F54C
        public static int QueryChargeMoney(DBManager dbMgr, string userID, int zoneID, int addmoney)
        {
            int ret = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT count(amount) as num FROM t_inputlog WHERE u='{0}' AND amount={2}", userID, zoneID, addmoney);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    try
                    {
                        ret = Convert.ToInt32(reader["num"].ToString());
                    }
                    catch (Exception)
                    {
                        ret = 0;
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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
            return ret;
        }

        // Token: 0x06000270 RID: 624 RVA: 0x0001142C File Offset: 0x0000F62C
        public static int QueryLastScanInputLogID(DBManager dbMgr)
        {
            int ret = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT lastid FROM t_inputhist WHERE Id=1", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    try
                    {
                        ret = Convert.ToInt32(reader["lastid"].ToString());
                    }
                    catch (Exception)
                    {
                        ret = 0;
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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
            return ret;
        }

        // Token: 0x06000271 RID: 625 RVA: 0x00011504 File Offset: 0x0000F704
        public static int ScanInputLogFromTable(DBManager dbMgr, int lastScanID)
        {
            int ret = lastScanID;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT Id, amount, u, inputtime, zoneid FROM t_inputlog WHERE Id>{0} and result='success'", lastScanID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    ret = Math.Max(ret, Convert.ToInt32(reader["Id"].ToString()));
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return ret;
        }

        // Token: 0x06000272 RID: 626 RVA: 0x000115CC File Offset: 0x0000F7CC
        public static long QueryServerTotalUserMoney()
        {
            long singleLong;
            using (MyDbConnection3 conn = new MyDbConnection3(false))
            {
                singleLong = conn.GetSingleLong("SELECT IFNULL(SUM(money),0) as money FROM t_money", 0, new MySQLParameter[0]);
            }
            return singleLong;
        }

        // Token: 0x06000273 RID: 627 RVA: 0x0001161C File Offset: 0x0000F81C
        public static List<InputKingPaiHangData> GetUserInputPaiHang(DBManager dbMgr, string startTime, string endTime, int maxPaiHang = 3)
        {
            List<InputKingPaiHangData> lsPaiHang = new List<InputKingPaiHangData>();
            List<InputKingPaiHangData> result;
            if (maxPaiHang < 1)
            {
                result = lsPaiHang;
            }
            else
            {
                MySQLConnection conn = null;
                try
                {
                    conn = dbMgr.DBConns.PopDBConnection();
                    string cmdText = string.Format("SELECT u, sum(totalmoney) as totalmoney, max(time) from\r\n                    (\r\n                    SELECT u, sum(amount) as totalmoney, max(time) as time from t_inputlog where t_inputlog.u IN (select DISTINCT  userid from t_roles where t_roles.isdel=0) and inputtime>='{0}' and inputtime<='{1}' and result='success' \r\n                    GROUP by u   \r\n                    union ALL\r\n                    SELECT u, sum(amount) as totalmoney, max(time) as time from t_inputlog2 where t_inputlog2.u IN (select DISTINCT  userid from t_roles where t_roles.isdel=0) and inputtime>='{0}' and inputtime<='{1}' and result='success' \r\n                    GROUP by u  \r\n                    ) a group by u order by  sum(totalmoney) desc,max(time) ASC limit {2};", startTime, endTime, maxPaiHang);
                    MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                    MySQLDataReader reader = cmd.ExecuteReaderEx();
                    int count = 0;
                    string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    while (reader.Read())
                    {
                        count++;
                        string userid = reader["u"].ToString();
                        int totalmoney = Convert.ToInt32(reader["totalmoney"].ToString());
                        InputKingPaiHangData phData = new InputKingPaiHangData
                        {
                            UserID = userid,
                            PaiHang = count,
                            PaiHangTime = now,
                            PaiHangValue = totalmoney
                        };
                        lsPaiHang.Add(phData);
                    }
                    Comparison<InputKingPaiHangData> com = new Comparison<InputKingPaiHangData>(DBQuery.InputKingPaiHangDataCompare);
                    lsPaiHang.Sort(com);
                    for (int i = 0; i < lsPaiHang.Count; i++)
                    {
                        lsPaiHang[i].PaiHang = i + 1;
                        lsPaiHang[i].PaiHangValue = Global.TransMoneyToYuanBao(lsPaiHang[i].PaiHangValue);
                    }
                    GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                    cmd.Dispose();
                }
                finally
                {
                    if (null != conn)
                    {
                        dbMgr.DBConns.PushDBConnection(conn);
                    }
                }
                result = lsPaiHang;
            }
            return result;
        }

        // Token: 0x06000274 RID: 628 RVA: 0x000117DC File Offset: 0x0000F9DC
        private static int InputKingPaiHangDataCompare(InputKingPaiHangData left, InputKingPaiHangData right)
        {
            return right.PaiHangValue - left.PaiHangValue;
        }

        // Token: 0x06000275 RID: 629 RVA: 0x000117FC File Offset: 0x0000F9FC
        public static int GetUserInputMoney(DBManager dbMgr, string userid, int zoneid, string startTime, string endTime)
        {
            int totalmoney = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT u, sum(amount) as totalmoney, max(time) as time from t_inputlog where inputtime>='{0}' and inputtime<='{1}' and u='{2}' and result='success' GROUP by u  union  SELECT u, sum(amount) as totalmoney, max(time) as time from t_inputlog2 where inputtime>='{0}' and inputtime<='{1}' and u='{2}' and result='success' GROUP by u ", new object[]
                {
                    startTime,
                    endTime,
                    userid,
                    zoneid
                });
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    totalmoney += Convert.ToInt32(reader["totalmoney"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return totalmoney;
        }

        // Token: 0x06000276 RID: 630 RVA: 0x000118DC File Offset: 0x0000FADC
        public static Dictionary<int, int> GetUserDanBiInputMoneyCount(DBManager dbMgr, string userid, int zoneid, string startTime, string endTime)
        {
            Dictionary<int, int> DanBiInputCountDic = new Dictionary<int, int>();
            using (MyDbConnection3 conn = new MyDbConnection3(false))
            {
                string cmdText = string.Format(" SELECT amount,SUM(moneyCount) AS moneyTotalCount  from (SELECT amount, COUNT(amount)  as moneyCount from t_inputlog where inputtime>='{0}' and inputtime<='{1}' and u='{2}' and result='success' GROUP by amount UNION SELECT amount, COUNT(amount) as moneyCount from t_inputlog2 where inputtime>='{0}' and inputtime<='{1}' and u='{2}' and result='success' GROUP by amount) moneyChargeInfo GROUP BY amount", new object[]
                {
                    startTime,
                    endTime,
                    userid,
                    zoneid
                });
                MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
                while (reader.Read())
                {
                    int money = Global.TransMoneyToYuanBao(Convert.ToInt32(reader["amount"].ToString()));
                    int moneyTotalCount = Convert.ToInt32(reader["moneyTotalCount"].ToString());
                    DanBiInputCountDic[money] = moneyTotalCount;
                }
            }
            return DanBiInputCountDic;
        }

        // Token: 0x06000277 RID: 631 RVA: 0x000119B0 File Offset: 0x0000FBB0
        public static int GetAwardHistoryForRole(DBManager dbMgr, int rid, int zoneid, int activitytype, string keystr, out int hasgettimes, out string lastgettime)
        {
            hasgettimes = 0;
            lastgettime = "";
            int ret = -1;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT hasgettimes, lastgettime from t_huodongawardrolehist where rid={0} and zoneid={1} and activitytype={2} and keystr='{3}' ", new object[]
                {
                    rid,
                    zoneid,
                    activitytype,
                    keystr
                });
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    hasgettimes = Convert.ToInt32(reader["hasgettimes"].ToString());
                    lastgettime = reader["lastgettime"].ToString();
                    ret = 0;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return ret;
        }

        // Token: 0x06000278 RID: 632 RVA: 0x00011ABC File Offset: 0x0000FCBC
        public static int GetAwardHistoryForUser(DBManager dbMgr, string userid, int activitytype, string keystr, out long hasgettimes, out string lastgettime)
        {
            hasgettimes = 0L;
            lastgettime = "";
            int ret = -1;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT hasgettimes, lastgettime from t_huodongawarduserhist where userid='{0}' and activitytype={1} and keystr='{2}' ", userid, activitytype, keystr);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    hasgettimes = Convert.ToInt64(reader["hasgettimes"].ToString());
                    lastgettime = reader["lastgettime"].ToString();
                    ret = 0;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return ret;
        }

        // Token: 0x06000279 RID: 633 RVA: 0x00011BA4 File Offset: 0x0000FDA4
        public static int GetAwardHistoryForUser(DBManager dbMgr, string userid, int activitytype, string keystr, out int hasgettimes, out string lastgettime)
        {
            hasgettimes = 0;
            lastgettime = "";
            int ret = -1;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT hasgettimes, lastgettime from t_huodongawarduserhist where userid='{0}' and activitytype={1} and keystr='{2}' ", userid, activitytype, keystr);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    hasgettimes = Convert.ToInt32(reader["hasgettimes"].ToString());
                    lastgettime = reader["lastgettime"].ToString();
                    ret = 0;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return ret;
        }

        // Token: 0x0600027A RID: 634 RVA: 0x00011C8C File Offset: 0x0000FE8C
        public static List<HuoDongPaiHangData> GetActivityPaiHangListNearMidTime(DBManager dbMgr, int huoDongType, string midTime, int maxPaiHang = 10)
        {
            List<HuoDongPaiHangData> lsPaiHang = new List<HuoDongPaiHangData>();
            MySQLConnection conn = null;
            try
            {
                string minTime = DateTime.Parse(midTime).AddHours(-36.0).ToString();
                string maxTime = DateTime.Parse(midTime).AddHours(36.0).ToString();
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT rid, rname, zoneid, type, paihang, phvalue, paihangtime, ABS(datediff(paihangtime, '{0}')) as diff  from t_huodongpaihang where type={1} and paihangtime<='{2}' and paihangtime>='{3}' ORDER by diff ASC, paihangtime desc, paihang ASC LIMIT 0, {4}", new object[]
                {
                    midTime,
                    huoDongType,
                    maxTime,
                    minTime,
                    maxPaiHang
                });
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                string sPaiHangTime = "";
                while (reader.Read())
                {
                    HuoDongPaiHangData ph = new HuoDongPaiHangData();
                    ph.RoleID = Convert.ToInt32(reader["rid"].ToString());
                    ph.RoleName = reader["rname"].ToString();
                    ph.ZoneID = Convert.ToInt32(reader["zoneid"].ToString());
                    ph.Type = Convert.ToInt32(reader["type"].ToString());
                    ph.PaiHang = Convert.ToInt32(reader["paihang"].ToString());
                    ph.PaiHangValue = Convert.ToInt32(reader["phvalue"].ToString());
                    ph.PaiHangTime = reader["paihangtime"].ToString();
                    if (string.IsNullOrEmpty(sPaiHangTime))
                    {
                        sPaiHangTime = ph.PaiHangTime;
                    }
                    else if (string.Compare(sPaiHangTime, ph.PaiHangTime) != 0)
                    {
                        break;
                    }
                    lsPaiHang.Add(ph);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return lsPaiHang;
        }

        // Token: 0x0600027B RID: 635 RVA: 0x00011ED8 File Offset: 0x000100D8
        public static int QueryLimitGoodsUsedNumByRoleID(DBManager dbMgr, int roleID, int goodsID, out int dayID, out int usedNum)
        {
            dayID = 0;
            usedNum = 0;
            int ret = -1;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT dayid, usednum FROM t_limitgoodsbuy WHERE rid={0} AND goodsid={1}", roleID, goodsID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    dayID = Convert.ToInt32(reader["dayid"].ToString());
                    usedNum = Convert.ToInt32(reader["usednum"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
                ret = 0;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return ret;
        }

        // Token: 0x0600027C RID: 636 RVA: 0x00011FC4 File Offset: 0x000101C4
        public static List<MailData> GetMailItemDataList(DBManager dbMgr, int rid)
        {
            List<MailData> list = new List<MailData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT mailid,senderrid,senderrname,sendtime,receiverrid,reveiverrname,readtime,isread, mailtype,hasfetchattachment,subject,content,yinliang,tongqian,yuanbao from t_mail where receiverrid={0} ORDER by sendtime desc limit 100", rid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    MailData mailItemData = new MailData
                    {
                        MailID = Convert.ToInt32(reader["mailid"].ToString()),
                        SenderRID = Convert.ToInt32(reader["senderrid"].ToString()),
                        SenderRName = reader["senderrname"].ToString(),
                        SendTime = reader["sendtime"].ToString(),
                        ReceiverRID = Convert.ToInt32(reader["receiverrid"].ToString()),
                        ReveiverRName = reader["reveiverrname"].ToString(),
                        ReadTime = reader["readtime"].ToString(),
                        IsRead = Convert.ToInt32(reader["isread"].ToString()),
                        MailType = Convert.ToInt32(reader["mailtype"].ToString()),
                        Hasfetchattachment = Convert.ToInt32(reader["hasfetchattachment"].ToString()),
                        Subject = reader["subject"].ToString(),
                        Content = "",
                        Yinliang = Convert.ToInt32(reader["yinliang"].ToString()),
                        Tongqian = Convert.ToInt32(reader["tongqian"].ToString()),
                        YuanBao = Convert.ToInt32(reader["yuanbao"].ToString())
                    };
                    list.Add(mailItemData);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x0600027D RID: 637 RVA: 0x00012220 File Offset: 0x00010420
        public static int GetMailItemDataCount(DBManager dbMgr, int rid, int excludeReadState = 0, int limitCount = 1)
        {
            MySQLConnection conn = null;
            int count = 0;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT mailid from t_mail where receiverrid={0} and isread<>{1} LIMIT 0,{2}", rid, excludeReadState, limitCount);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    count++;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return count;
        }

        // Token: 0x0600027E RID: 638 RVA: 0x000122D8 File Offset: 0x000104D8
        public static MailData GetMailItemData(DBManager dbMgr, int rid, int mailID)
        {
            MailData mailItemData = null;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT mailid,senderrid,senderrname,sendtime,receiverrid,reveiverrname,readtime,isread, mailtype,hasfetchattachment,subject,content,yinliang,tongqian,yuanbao from t_mail where receiverrid={0} and mailid={1} ORDER by sendtime desc", rid, mailID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    mailItemData = new MailData
                    {
                        MailID = Convert.ToInt32(reader["mailid"].ToString()),
                        SenderRID = Convert.ToInt32(reader["senderrid"].ToString()),
                        SenderRName = reader["senderrname"].ToString(),
                        SendTime = reader["sendtime"].ToString(),
                        ReceiverRID = Convert.ToInt32(reader["receiverrid"].ToString()),
                        ReveiverRName = reader["reveiverrname"].ToString(),
                        ReadTime = reader["readtime"].ToString(),
                        IsRead = Convert.ToInt32(reader["isread"].ToString()),
                        MailType = Convert.ToInt32(reader["mailtype"].ToString()),
                        Hasfetchattachment = Convert.ToInt32(reader["hasfetchattachment"].ToString()),
                        Subject = reader["subject"].ToString(),
                        Content = Global.GetSysEncoding().GetString((byte[])reader["content"]),
                        Yinliang = Convert.ToInt32(reader["yinliang"].ToString()),
                        Tongqian = Convert.ToInt32(reader["tongqian"].ToString()),
                        YuanBao = Convert.ToInt32(reader["yuanbao"].ToString())
                    };
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            if (null != mailItemData)
            {
                mailItemData.GoodsList = DBQuery.GetMailGoodsDataList(dbMgr, mailID);
            }
            return mailItemData;
        }

        // Token: 0x0600027F RID: 639 RVA: 0x00012558 File Offset: 0x00010758
        public static List<MailGoodsData> GetMailGoodsDataList(DBManager dbMgr, int mailID)
        {
            List<MailGoodsData> list = new List<MailGoodsData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT id,mailid,goodsid,forge_level,quality,Props,gcount,binding,origholenum,rmbholenum,jewellist,addpropindex,bornindex,lucky,\r\n                                                        strong,excellenceinfo,appendproplev,equipchangelife from t_mailgoods where mailid={0}", mailID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    MailGoodsData mailItemData = new MailGoodsData
                    {
                        Id = Convert.ToInt32(reader["id"].ToString()),
                        MailID = Convert.ToInt32(reader["mailid"].ToString()),
                        GoodsID = Convert.ToInt32(reader["goodsid"].ToString()),
                        Forge_level = Convert.ToInt32(reader["forge_level"].ToString()),
                        Quality = Convert.ToInt32(reader["quality"].ToString()),
                        Props = reader["Props"].ToString(),
                        GCount = Convert.ToInt32(reader["gcount"].ToString()),
                        Binding = Convert.ToInt32(reader["binding"].ToString()),
                        OrigHoleNum = Convert.ToInt32(reader["origholenum"].ToString()),
                        RMBHoleNum = Convert.ToInt32(reader["rmbholenum"].ToString()),
                        Jewellist = reader["jewellist"].ToString(),
                        AddPropIndex = Convert.ToInt32(reader["addpropindex"].ToString()),
                        BornIndex = Convert.ToInt32(reader["bornindex"].ToString()),
                        Lucky = Convert.ToInt32(reader["lucky"].ToString()),
                        Strong = Convert.ToInt32(reader["strong"].ToString()),
                        ExcellenceInfo = Convert.ToInt32(reader["excellenceinfo"].ToString()),
                        AppendPropLev = Convert.ToInt32(reader["appendproplev"].ToString()),
                        EquipChangeLifeLev = Convert.ToInt32(reader["equipchangelife"].ToString())
                    };
                    list.Add(mailItemData);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x06000280 RID: 640 RVA: 0x0001282C File Offset: 0x00010A2C
        public static Dictionary<int, int> ScanLastMailIDListFromTable(DBManager dbMgr)
        {
            Dictionary<int, int> lastMailDct = new Dictionary<int, int>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT MAX(mailid) as mailid, receiverrid from t_mailtemp  GROUP by mailid,receiverrid ORDER by receiverrid asc limit 0, 20", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    int receiverrid = Convert.ToInt32(reader["receiverrid"].ToString());
                    if (!lastMailDct.ContainsKey(receiverrid))
                    {
                        lastMailDct.Add(receiverrid, Convert.ToInt32(reader["mailid"].ToString()));
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return lastMailDct;
        }

        // Token: 0x06000281 RID: 641 RVA: 0x00012920 File Offset: 0x00010B20
        public static Dictionary<int, FuMoMailData> GetFuMoMailCached(DBManager dbMgr)
        {
            MySQLConnection conn = null;
            Dictionary<int, FuMoMailData> dict = new Dictionary<int, FuMoMailData>();
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                MySQLCommand cmd = new MySQLCommand(SqlDefineManager.SelectAllMailList, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    FuMoMailData mailItemData = new FuMoMailData
                    {
                        MaillID = Convert.ToInt32(reader["maillid"].ToString()),
                        SenderRID = Convert.ToInt32(reader["senderrid"].ToString()),
                        SenderRName = reader["senderrname"].ToString(),
                        SenderJob = Convert.ToInt32(reader["senderjob"].ToString()),
                        SendTime = reader["sendtime"].ToString(),
                        ReceiverRID = Convert.ToInt32(reader["receiverrid"].ToString()),
                        IsRead = Convert.ToInt32(reader["isread"].ToString()),
                        ReadTime = reader["readtime"].ToString(),
                        FuMoMoney = Convert.ToInt32(reader["fumomoney"].ToString()),
                        Content = Global.GetSysEncoding().GetString((byte[])reader["content"])
                    };
                    dict[mailItemData.MaillID] = mailItemData;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", SqlDefineManager.SelectAllMailList), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return dict;
        }

        // Token: 0x06000282 RID: 642 RVA: 0x00012AFC File Offset: 0x00010CFC
        public static Dictionary<int, Dictionary<int, FuMoMailTemp>> GetAllMap(MySQLDataReader reader)
        {
            Dictionary<int, Dictionary<int, FuMoMailTemp>> dict = new Dictionary<int, Dictionary<int, FuMoMailTemp>>();
            Dictionary<int, FuMoMailTemp> dictTemp = new Dictionary<int, FuMoMailTemp>();
            int tempToday = 0;
            while (reader.Read())
            {
                FuMoMailTemp mailItemData = new FuMoMailTemp
                {
                    TodayID = Convert.ToInt32(reader["tid"].ToString()),
                    SenderRID = Convert.ToInt32(reader["senderid"].ToString()),
                    ReceiverRID = reader["recid_list"].ToString(),
                    Accept = Convert.ToInt32(reader["accept"].ToString()),
                    Give = Convert.ToInt32(reader["give"].ToString())
                };
                tempToday = mailItemData.TodayID;
                dictTemp.Add(mailItemData.SenderRID, mailItemData);
            }
            if (tempToday != 0)
            {
                dict.Add(tempToday, dictTemp);
            }
            return dict;
        }

        // Token: 0x06000283 RID: 643 RVA: 0x00012BF0 File Offset: 0x00010DF0
        public static Dictionary<int, Dictionary<int, FuMoMailTemp>> GetFuMoMailTempCached(DBManager dbMgr)
        {
            string cmdText = string.Format(SqlDefineManager.SelectMapStartServer, TimeUtil.GetOffsetDayNow());
            return SqlDefineManager.SqlHandler<Dictionary<int, Dictionary<int, FuMoMailTemp>>>(cmdText, new Global.SQLDelegate<Dictionary<int, Dictionary<int, FuMoMailTemp>>>(DBQuery.GetAllMap));
        }

        // Token: 0x06000284 RID: 644 RVA: 0x00012C2C File Offset: 0x00010E2C
        public static Dictionary<int, FuMoMailTemp> GetFuMoMailMapDataList(DBManager dbMgr, int rid, int nDate)
        {
            Dictionary<int, FuMoMailTemp> list = new Dictionary<int, FuMoMailTemp>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format(SqlDefineManager.SelectMapList, nDate, rid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    FuMoMailTemp mailItemData = new FuMoMailTemp
                    {
                        TodayID = Convert.ToInt32(reader["tid"].ToString()),
                        SenderRID = Convert.ToInt32(reader["senderid"].ToString()),
                        ReceiverRID = reader["recid_list"].ToString(),
                        Accept = Convert.ToInt32(reader["accept"].ToString()),
                        Give = Convert.ToInt32(reader["give"].ToString())
                    };
                    list.Add(mailItemData.SenderRID, mailItemData);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x06000285 RID: 645 RVA: 0x00012D98 File Offset: 0x00010F98
        public static int GetMailMaxIDFromTable(DBManager dbMgr)
        {
            int num = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                MySQLCommand cmd = new MySQLCommand(SqlDefineManager.SelectMaxMailIndex, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    num = Global.SafeConvertToInt32(reader["mymaxvalue"].ToString(), 10);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", SqlDefineManager.SelectMaxMailIndex), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return num;
        }

        // Token: 0x06000286 RID: 646 RVA: 0x00012E50 File Offset: 0x00011050
        public static int GetMailMaxConutFromTable(DBManager dbMgr, int rid)
        {
            int num = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format(SqlDefineManager.SelectMailCount, rid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    num = Global.SafeConvertToInt32(reader["mymaxvalue"].ToString(), 10);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return num;
        }

        // Token: 0x06000287 RID: 647 RVA: 0x00012F14 File Offset: 0x00011114
        public static Dictionary<int, RebornStampData> GetAllRebornCached(MySQLDataReader reader)
        {
            Dictionary<int, RebornStampData> dict = new Dictionary<int, RebornStampData>();
            while (reader.Read())
            {
                RebornStampData rebornYinJiData = new RebornStampData
                {
                    RoleID = Convert.ToInt32(reader["rid"].ToString()),
                    ResetNum = Convert.ToInt32(reader["reset"].ToString()),
                    UsePoint = Convert.ToInt32(reader["use_point"].ToString()),
                    StampInfo = RebornStampManager.UnMakeYinJiUpdateInfo(reader["stamp"].ToString())
                };
                dict.Add(rebornYinJiData.RoleID, rebornYinJiData);
            }
            return dict;
        }

        // Token: 0x06000288 RID: 648 RVA: 0x00012FC8 File Offset: 0x000111C8
        public static Dictionary<int, RebornStampData> GetRebornYinJiCached(DBManager dbMgr)
        {
            return SqlDefineManager.SqlHandler<Dictionary<int, RebornStampData>>(SqlDefineManager.SelectRebornYinJiAll, new Global.SQLDelegate<Dictionary<int, RebornStampData>>(DBQuery.GetAllRebornCached));
        }

        // Token: 0x06000289 RID: 649 RVA: 0x00012FF0 File Offset: 0x000111F0
        public static int GetRoleIDByRoleName(DBManager dbMgr, string roleName, int zoneid)
        {
            int rid = -1;
            int result;
            if (string.IsNullOrWhiteSpace(roleName))
            {
                result = rid;
            }
            else
            {
                MySQLConnection conn = null;
                try
                {
                    conn = dbMgr.DBConns.PopDBConnection();
                    string cmdText = string.Format("SELECT rid from t_roles WHERE rname='{0}' and zoneid={1}", roleName, zoneid);
                    MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                    MySQLDataReader reader = cmd.ExecuteReaderEx();
                    if (reader.Read())
                    {
                        rid = Convert.ToInt32(reader["rid"].ToString());
                    }
                    GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                    cmd.Dispose();
                }
                finally
                {
                    if (null != conn)
                    {
                        dbMgr.DBConns.PushDBConnection(conn);
                    }
                }
                result = rid;
            }
            return result;
        }

        // Token: 0x0600028A RID: 650 RVA: 0x000130CC File Offset: 0x000112CC
        public static int GetMaxMailID(DBManager dbMgr)
        {
            int maxValue = -1;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT MAX(mailid) as mymaxvalue from t_mail", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    maxValue = Global.SafeConvertToInt32(reader["mymaxvalue"].ToString(), 10);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return maxValue;
        }

        // Token: 0x0600028B RID: 651 RVA: 0x00013190 File Offset: 0x00011390
        public static int GetMaxRoleID(DBManager dbMgr)
        {
            int maxValue = -1;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT MAX(rid) as mymaxvalue from t_roles", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    maxValue = Global.SafeConvertToInt32(reader["mymaxvalue"].ToString(), 10);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return maxValue;
        }

        // Token: 0x0600028C RID: 652 RVA: 0x00013254 File Offset: 0x00011454
        public static int GetMaxBangHuiID(DBManager dbMgr)
        {
            int maxValue = -1;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT MAX(bhid) as mymaxvalue from t_banghui", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    maxValue = Global.SafeConvertToInt32(reader["mymaxvalue"].ToString(), 10);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return maxValue;
        }

        // Token: 0x0600028D RID: 653 RVA: 0x00013318 File Offset: 0x00011518
        public static int GetMaxQiangGouItemID(DBManager dbMgr)
        {
            int maxValue = -1;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT MAX(Id) as mymaxvalue from t_qianggouitem", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    maxValue = Global.SafeConvertToInt32(reader["mymaxvalue"].ToString(), 10);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return maxValue;
        }

        // Token: 0x0600028E RID: 654 RVA: 0x000133DC File Offset: 0x000115DC
        public static List<ZaJinDanHistory> QueryZaJinDanHistoryDataList(DBManager dbMgr, int roleID = -1)
        {
            List<ZaJinDanHistory> list = new List<ZaJinDanHistory>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string sWhere = "";
                if (roleID > 0)
                {
                    sWhere += string.Format(" Where rid={0} ", roleID);
                }
                else
                {
                    sWhere += string.Format(" Where gaingoodsnum>0 ", new object[0]);
                }
                string cmdText = string.Format("SELECT * FROM t_zajindanhist {0} ORDER BY operationtime DESC LIMIT 50", sWhere);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                int count = 0;
                while (reader.Read() && count < 100)
                {
                    ZaJinDanHistory historyData = new ZaJinDanHistory
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(reader["zoneid"].ToString(), reader["rname"].ToString()),
                        TimesSelected = Convert.ToInt32(reader["timesselected"].ToString()),
                        UsedYuanBao = Convert.ToInt32(reader["usedyuanbao"].ToString()),
                        UsedJinDan = Convert.ToInt32(reader["usedjindan"].ToString()),
                        GainGoodsId = Convert.ToInt32(reader["gaingoodsid"].ToString()),
                        GainGoodsNum = Convert.ToInt32(reader["gaingoodsnum"].ToString()),
                        GainGold = Convert.ToInt32(reader["gaingold"].ToString()),
                        GainYinLiang = Convert.ToInt32(reader["gainyinliang"].ToString()),
                        GainExp = Convert.ToInt32(reader["gainexp"].ToString()),
                        GoodPorp = reader["strprop"].ToString(),
                        OperationTime = reader["operationtime"].ToString()
                    };
                    list.Add(historyData);
                    count++;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x0600028F RID: 655 RVA: 0x0001366C File Offset: 0x0001186C
        public static int GetFirstChongZhiDaLiNum(DBManager dbMgr, string userID)
        {
            int totalNum = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT COUNT(rid) AS totalnum from t_roles WHERE userid='{0}' and cztaskid>0", userID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                try
                {
                    if (reader.Read())
                    {
                        totalNum = Convert.ToInt32(reader["totalnum"].ToString());
                    }
                }
                catch (Exception)
                {
                    totalNum = 0;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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
            return totalNum;
        }

        // Token: 0x06000290 RID: 656 RVA: 0x00013740 File Offset: 0x00011940
        public static int GetKaiFuOnlineAwardRoleID(DBManager dbMgr, int dayID, out int totalRoleNum)
        {
            totalRoleNum = 0;
            int roleID = -1;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType("KaiFuOnlineDayID", null);
                string cmdText = string.Format("SELECT rid,{0} FROM {1} WHERE {2}={3}", new object[]
                {
                    roleParamType.ColumnName,
                    roleParamType.TableName,
                    roleParamType.IdxName,
                    roleParamType.KeyString
                });
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                List<int> roleIDs = new List<int>();
                try
                {
                    while (reader.Read())
                    {
                        int pvalue = Global.SafeConvertToInt32(reader[1].ToString(), 10);
                        if (pvalue >= dayID)
                        {
                            roleIDs.Add(Global.SafeConvertToInt32(reader["rid"].ToString(), 10));
                        }
                    }
                }
                catch (Exception)
                {
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
                cmd = null;
                if (roleIDs.Count > 0)
                {
                    Random rand = new Random();
                    roleID = roleIDs[rand.Next(0, roleIDs.Count)];
                    totalRoleNum = roleIDs.Count;
                }
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return roleID;
        }

        // Token: 0x06000291 RID: 657 RVA: 0x000138E0 File Offset: 0x00011AE0
        public static List<KaiFuOnlineAwardData> GetKaiFuOnlineAwardDataList(DBManager dbMgr, int zoneID)
        {
            List<KaiFuOnlineAwardData> kaiFuOnlineAwardDataList = new List<KaiFuOnlineAwardData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT K.rid, R.zoneid, R.rname, K.dayid, K.totalrolenum FROM t_kfonlineawards AS K, t_roles AS R WHERE K.rid=R.rid AND K.zoneid={0}", zoneID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                try
                {
                    while (reader.Read())
                    {
                        kaiFuOnlineAwardDataList.Add(new KaiFuOnlineAwardData
                        {
                            RoleID = Global.SafeConvertToInt32(reader["rid"].ToString(), 10),
                            ZoneID = Global.SafeConvertToInt32(reader["zoneid"].ToString(), 10),
                            RoleName = reader["rname"].ToString(),
                            DayID = Global.SafeConvertToInt32(reader["dayid"].ToString(), 10),
                            TotalRoleNum = Global.SafeConvertToInt32(reader["totalrolenum"].ToString(), 10)
                        });
                    }
                }
                catch (Exception)
                {
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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
            return kaiFuOnlineAwardDataList;
        }

        // Token: 0x06000292 RID: 658 RVA: 0x00013A64 File Offset: 0x00011C64
        public static void ScanGMMsgFromTable(DBManager dbMgr, List<string> msgList)
        {
            MySQLConnection conn = null;
            try
            {
                int Id = 0;
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "SELECT id, msg FROM t_gmmsg";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    Id = Math.Max(Convert.ToInt32(reader["id"].ToString()), Id);
                    string msg = Global.GetSysEncoding().GetString((byte[])reader["msg"]);
                    msgList.Add(msg);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
                if (Id > 0)
                {
                    cmdText = string.Format("DELETE FROM t_gmmsg WHERE id<={0}", Id);
                    cmd = new MySQLCommand(cmdText, conn);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                }
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
        }

        // Token: 0x06000293 RID: 659 RVA: 0x00013B8C File Offset: 0x00011D8C
        public static List<InputKingPaiHangData> GetUserUsedMoneyPaiHang1(DBManager dbMgr, string startTime, string endTime, int maxPaiHang = 3)
        {
            List<InputKingPaiHangData> lsPaiHang = new List<InputKingPaiHangData>();
            List<InputKingPaiHangData> result;
            if (maxPaiHang < 1)
            {
                result = lsPaiHang;
            }
            else
            {
                MySQLConnection conn = null;
                try
                {
                    conn = dbMgr.DBConns.PopDBConnection();
                    string cmdText = string.Format("SELECT t_mallbuy.rid, sum(t_mallbuy.totalprice) as totalmoney, max(t_mallbuy.buytime) as time from t_mallbuy,t_roles  where t_mallbuy.rid=t_roles.rid and buytime>='{0}' and buytime<='{1}' and t_roles.isdel=0 GROUP by rid  union  SELECT t_zajindanhist.rid, sum(usedyuanbao/timesselected) as totalmoney, max(operationtime) as time from t_zajindanhist,t_roles where t_zajindanhist.rid=t_roles.rid and t_roles.isdel=0 and operationtime>='{0}' and operationtime<='{1}' GROUP by rid  union  SELECT t_qizhengebuy.rid, sum(totalprice) as totalmoney, max(buytime) as time from t_qizhengebuy,t_roles where buytime>='{0}' and buytime<='{1}' and t_qizhengebuy.rid=t_roles.rid and t_roles.isdel=0  GROUP by rid order by totalmoney desc,time asc  limit 0, {2} ", startTime, endTime, maxPaiHang * 3);
                    MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                    MySQLDataReader reader = cmd.ExecuteReaderEx();
                    List<int> tmp = new List<int>();
                    int count = 0;
                    string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    while (reader.Read())
                    {
                        count++;
                        int rid = Convert.ToInt32(reader["rid"].ToString());
                        int totalmoney = (int)Convert.ToDouble(reader["totalmoney"].ToString());
                        if (totalmoney > 0)
                        {
                            if (!tmp.Contains(rid))
                            {
                                tmp.Add(rid);
                                InputKingPaiHangData phData = new InputKingPaiHangData
                                {
                                    UserID = rid.ToString(),
                                    PaiHang = count,
                                    PaiHangTime = now,
                                    PaiHangValue = totalmoney
                                };
                                lsPaiHang.Add(phData);
                            }
                            else
                            {
                                InputKingPaiHangData phData = lsPaiHang[tmp.IndexOf(rid)];
                                phData.PaiHangValue += totalmoney;
                            }
                        }
                        if (lsPaiHang.Count >= maxPaiHang)
                        {
                            break;
                        }
                    }
                    Comparison<InputKingPaiHangData> com = new Comparison<InputKingPaiHangData>(DBQuery.InputKingPaiHangDataCompare);
                    lsPaiHang.Sort(com);
                    for (int i = 0; i < lsPaiHang.Count; i++)
                    {
                        lsPaiHang[i].PaiHang = i + 1;
                    }
                    GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                    cmd.Dispose();
                }
                finally
                {
                    if (null != conn)
                    {
                        dbMgr.DBConns.PushDBConnection(conn);
                    }
                }
                result = lsPaiHang;
            }
            return result;
        }

        // Token: 0x06000294 RID: 660 RVA: 0x00013DA4 File Offset: 0x00011FA4
        public static int GetUserUsedMoney1(DBManager dbMgr, int rid, string startTime, string endTime)
        {
            int totalmoney = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT rid, sum(totalprice) as totalmoney, max(buytime) as time from t_mallbuy where buytime>='{0}' and buytime<='{1}' and rid={2} GROUP by rid  union  SELECT rid, sum(usedyuanbao/timesselected) as totalmoney, max(operationtime) as time from t_zajindanhist where operationtime>='{0}' and operationtime<='{1}' and rid={2} GROUP by rid  union  SELECT rid, sum(totalprice) as totalmoney, max(buytime) as time from t_qizhengebuy where buytime>='{0}' and buytime<='{1}' and rid={2} GROUP by rid", startTime, endTime, rid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    totalmoney += (int)Convert.ToDouble(reader["totalmoney"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return totalmoney;
        }

        // Token: 0x06000295 RID: 661 RVA: 0x00013E68 File Offset: 0x00012068
        public static List<YueDuChouJiangData> QueryYueDuChouJiangHistoryDataList(DBManager dbMgr, int roleID = -1)
        {
            List<YueDuChouJiangData> list = new List<YueDuChouJiangData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string sWhere = "";
                if (roleID > 0)
                {
                    sWhere += string.Format(" Where rid={0}", roleID);
                }
                else
                {
                    sWhere += string.Format(" Where gaingoodsnum>0 ", new object[0]);
                }
                string cmdText = string.Format("SELECT * FROM t_yueduchoujianghist {0} ORDER BY operationtime DESC LIMIT 50", sWhere);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                int count = 0;
                while (reader.Read() && count < 100)
                {
                    YueDuChouJiangData historyData = new YueDuChouJiangData
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(reader["zoneid"].ToString(), reader["rname"].ToString()),
                        GainGoodsId = Convert.ToInt32(reader["gaingoodsid"].ToString()),
                        GainGoodsNum = Convert.ToInt32(reader["gaingoodsnum"].ToString()),
                        GainGold = Convert.ToInt32(reader["gaingold"].ToString()),
                        GainYinLiang = Convert.ToInt32(reader["gainyinliang"].ToString()),
                        GainExp = Convert.ToInt32(reader["gainexp"].ToString()),
                        OperationTime = reader["operationtime"].ToString()
                    };
                    list.Add(historyData);
                    count++;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x06000296 RID: 662 RVA: 0x0001408C File Offset: 0x0001228C
        public static int GetBloodCastleEnterCount(DBManager dbMgr, int roleid, int nDate, int activityid)
        {
            int ret = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT triggercount from t_dayactivityinfo where roleid={0} and activityid={1} and timeinfo={2} ", roleid, activityid, nDate);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    ret = Convert.ToInt32(reader["triggercount"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return ret;
        }

        // Token: 0x06000297 RID: 663 RVA: 0x00014158 File Offset: 0x00012358
        public static List<int> GetDayActivityTotlePoint(DBManager dbMgr, int activityid)
        {
            List<int> lData = new List<int>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT roleid, totalpoint FROM t_dayactivityinfo WHERE totalpoint>0 AND activityid = {0} ORDER BY totalpoint DESC LIMIT 1", activityid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                int nRoleid = -1;
                int nPoint = -1;
                if (reader.Read())
                {
                    nRoleid = Convert.ToInt32(reader["roleid"].ToString());
                    nPoint = (int)Math.Min(2147483647L, Convert.ToInt64(reader["totalpoint"].ToString()));
                }
                if (nRoleid != -1 && nPoint != -1)
                {
                    lData.Add(nRoleid);
                    lData.Add(nPoint);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return lData;
        }

        // Token: 0x06000298 RID: 664 RVA: 0x00014270 File Offset: 0x00012470
        public static int GetRoleDayActivityPoint(DBManager dbMgr, int nRole, int activityid)
        {
            int nPoint = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT totalpoint FROM t_dayactivityinfo WHERE roleid = {0} AND activityid = {1}", nRole, activityid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    nPoint = (int)Math.Min(2147483647L, Convert.ToInt64(reader["totalpoint"].ToString()));
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return nPoint;
        }

        // Token: 0x06000299 RID: 665 RVA: 0x00014344 File Offset: 0x00012544
        public static int QueryPlayerAdmiredAnother(DBManager dbMgr, int roleAID, int roleBID, int nDate)
        {
            int nID = -1;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT adorationroleid from t_adorationinfo where roleid={0} and adorationroleid={1} and dayid={2}", roleAID, roleBID, nDate);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    nID = Convert.ToInt32(reader["adorationroleid"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return nID;
        }

        // Token: 0x0600029A RID: 666 RVA: 0x00014410 File Offset: 0x00012610
        public static List<int> QueryPlayerEveryDayOnLineAwardGiftInfo(DBManager dbMgr, int roleID)
        {
            List<int> lData = new List<int>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT everydayonlineawardstep, geteverydayonlineawarddayid from t_huodong where roleid={0}", roleID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    int nValue = Convert.ToInt32(reader["everydayonlineawardstep"].ToString());
                    lData.Add(nValue);
                    nValue = Convert.ToInt32(reader["geteverydayonlineawarddayid"].ToString());
                    lData.Add(nValue);
                }
                else
                {
                    lData = null;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return lData;
        }

        // Token: 0x0600029B RID: 667 RVA: 0x00014508 File Offset: 0x00012708
        public static List<PushMessageData> QueryPushMsgUerList(DBManager dbMgr, int nCondition)
        {
            List<PushMessageData> list = new List<PushMessageData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                DateTime time = DateTime.Now;
                string cmdText = string.Format("SELECT userid, pushid, lastlogintime from t_pushmessageinfo where NOW() <= ADDDATE(lastlogintime, {0})", nCondition + 1);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    PushMessageData PushMsgData = new PushMessageData
                    {
                        UserID = reader["userid"].ToString(),
                        PushID = reader["pushid"].ToString(),
                        LastLoginTime = reader["lastlogintime"].ToString()
                    };
                    list.Add(PushMsgData);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x0600029C RID: 668 RVA: 0x0001461C File Offset: 0x0001281C
        public static Dictionary<int, int> QueryMoJingExchangeDict(DBManager dbMgr, int nRoleid, int nDayID)
        {
            Dictionary<int, int> TmpDict = new Dictionary<int, int>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT exchangeid, exchangenum FROM t_mojingexchangeinfo WHERE roleid = {0} AND dayid = {1}", nRoleid, nDayID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    int nExchangeid = Convert.ToInt32(reader["exchangeid"].ToString());
                    int nNum = Convert.ToInt32(reader["exchangenum"].ToString());
                    TmpDict.Add(nExchangeid, nNum);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return TmpDict;
        }

        // Token: 0x0600029D RID: 669 RVA: 0x0001470C File Offset: 0x0001290C
        public static Dictionary<int, OldResourceInfo> QueryResourceGetInfo(DBManager dbMgr, int nRoleid)
        {
            Dictionary<int, OldResourceInfo> datadict = new Dictionary<int, OldResourceInfo>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                DateTime time = DateTime.Now;
                string cmdText = string.Format("SELECT type, exp, leftCount,mojing,bandmoney,zhangong,chengjiu,shengwang,bangzuan,xinghun,yuansufenmo from t_resourcegetinfo where roleid = {0} AND hasget = {1}", nRoleid, 0);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    OldResourceInfo data = new OldResourceInfo
                    {
                        type = Global.SafeConvertToInt32(reader["type"].ToString(), 10),
                        exp = ((Global.SafeConvertToInt32(reader["exp"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(reader["exp"].ToString(), 10) : 0),
                        leftCount = ((Global.SafeConvertToInt32(reader["leftCount"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(reader["leftCount"].ToString(), 10) : 0),
                        mojing = ((Global.SafeConvertToInt32(reader["mojing"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(reader["mojing"].ToString(), 10) : 0),
                        bandmoney = ((Global.SafeConvertToInt32(reader["bandmoney"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(reader["bandmoney"].ToString(), 10) : 0),
                        zhangong = ((Global.SafeConvertToInt32(reader["zhangong"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(reader["zhangong"].ToString(), 10) : 0),
                        chengjiu = ((Global.SafeConvertToInt32(reader["chengjiu"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(reader["chengjiu"].ToString(), 10) : 0),
                        shengwang = ((Global.SafeConvertToInt32(reader["shengwang"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(reader["shengwang"].ToString(), 10) : 0),
                        bandDiamond = ((Global.SafeConvertToInt32(reader["bangzuan"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(reader["bangzuan"].ToString(), 10) : 0),
                        xinghun = ((Global.SafeConvertToInt32(reader["xinghun"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(reader["xinghun"].ToString(), 10) : 0),
                        yuanSuFenMo = ((Global.SafeConvertToInt32(reader["yuansufenmo"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(reader["yuansufenmo"].ToString(), 10) : 0),
                        roleId = nRoleid
                    };
                    datadict[data.type] = data;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return datadict;
        }

        // Token: 0x0600029E RID: 670 RVA: 0x00014A88 File Offset: 0x00012C88
        public static int GetUserUsedMoney(DBManager dbMgr, int rid, string startTime, string endTime)
        {
            int totalmoney = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT rid, sum(amount) as totalmoney  from t_consumelog where cdate>='{0}' and cdate<='{1}' and rid={2} GROUP by rid ", startTime, endTime, rid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    totalmoney = (int)Convert.ToDouble(reader["totalmoney"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return totalmoney;
        }

        // Token: 0x0600029F RID: 671 RVA: 0x00014B4C File Offset: 0x00012D4C
        public static List<InputKingPaiHangData> GetUserUsedMoneyPaiHang(DBManager dbMgr, string startTime, string endTime, int maxPaiHang = 3)
        {
            List<InputKingPaiHangData> lsPaiHang = new List<InputKingPaiHangData>();
            List<InputKingPaiHangData> result;
            if (maxPaiHang < 1)
            {
                result = lsPaiHang;
            }
            else
            {
                MySQLConnection conn = null;
                try
                {
                    conn = dbMgr.DBConns.PopDBConnection();
                    string cmdText = string.Format("SELECT t_consumelog.rid, sum(t_consumelog.amount) as totalmoney, max(cdate) as time from t_consumelog,t_roles  where t_consumelog.rid=t_roles.rid and cdate>='{0}' and cdate<='{1}' and t_roles.isdel=0 GROUP by rid  order by totalmoney desc,time asc  limit 0, {2} ", startTime, endTime, maxPaiHang * 2);
                    MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                    MySQLDataReader reader = cmd.ExecuteReaderEx();
                    List<int> tmp = new List<int>();
                    int count = 0;
                    string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    while (reader.Read())
                    {
                        count++;
                        int rid = Convert.ToInt32(reader["rid"].ToString());
                        int totalmoney = (int)Convert.ToDouble(reader["totalmoney"].ToString());
                        if (totalmoney > 0)
                        {
                            if (!tmp.Contains(rid))
                            {
                                tmp.Add(rid);
                                InputKingPaiHangData phData = new InputKingPaiHangData
                                {
                                    UserID = rid.ToString(),
                                    PaiHang = count,
                                    PaiHangTime = now,
                                    PaiHangValue = totalmoney
                                };
                                lsPaiHang.Add(phData);
                            }
                            else
                            {
                                InputKingPaiHangData phData = lsPaiHang[tmp.IndexOf(rid)];
                                phData.PaiHangValue += totalmoney;
                            }
                        }
                        if (lsPaiHang.Count >= maxPaiHang)
                        {
                            break;
                        }
                    }
                    Comparison<InputKingPaiHangData> com = new Comparison<InputKingPaiHangData>(DBQuery.InputKingPaiHangDataCompare);
                    lsPaiHang.Sort(com);
                    for (int i = 0; i < lsPaiHang.Count; i++)
                    {
                        lsPaiHang[i].PaiHang = i + 1;
                    }
                    GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                    cmd.Dispose();
                }
                finally
                {
                    if (null != conn)
                    {
                        dbMgr.DBConns.PushDBConnection(conn);
                    }
                }
                result = lsPaiHang;
            }
            return result;
        }

        // Token: 0x060002A0 RID: 672 RVA: 0x00014D64 File Offset: 0x00012F64
        public static int QueryVipLevelAwardFlagInfo(DBManager dbMgr, int nRoldID, int nZoneID)
        {
            int nFlag = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT vipawardflag from t_roles WHERE rid = '{0}' and zoneid={1}", nRoldID, nZoneID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                try
                {
                    if (reader.Read())
                    {
                        nFlag = Convert.ToInt32(reader["vipawardflag"].ToString());
                    }
                }
                catch (Exception)
                {
                    nFlag = 0;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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
            return nFlag;
        }

        // Token: 0x060002A1 RID: 673 RVA: 0x00014E44 File Offset: 0x00013044
        public static int QueryVipLevelAwardFlagInfoByUserID(DBManager dbMgr, string struseid, int nRoleID, int nZoneID)
        {
            int nFlag = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT vipawardflag FROM t_roles WHERE userid = '{0}' and zoneid = {1} and rid != {2}", struseid, nZoneID, nRoleID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                try
                {
                    if (reader.Read())
                    {
                        if (Convert.ToInt32(reader["vipawardflag"].ToString()) > 0)
                        {
                            nFlag = Convert.ToInt32(reader["vipawardflag"].ToString());
                        }
                    }
                }
                catch (Exception)
                {
                    nFlag = 0;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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
            return nFlag;
        }

        // Token: 0x060002A2 RID: 674 RVA: 0x00014F48 File Offset: 0x00013148
        public static int LastLoginRole(DBManager dbMgr, string uid)
        {
            int rid = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT rid FROM t_roles WHERE userid = '{0}' AND isdel=0   ORDER BY lasttime DESC LIMIT 1 ", uid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                try
                {
                    if (reader.Read())
                    {
                        if (Convert.ToInt32(reader["rid"].ToString()) > 0)
                        {
                            rid = Convert.ToInt32(reader["rid"].ToString());
                        }
                    }
                }
                catch (Exception)
                {
                    rid = 0;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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
            return rid;
        }

        // Token: 0x060002A3 RID: 675 RVA: 0x00015040 File Offset: 0x00013240
        public static bool GetUserRole(DBManager dbMgr, string userID, int roleID)
        {
            MySQLConnection conn = null;
            bool result = false;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT rid FROM t_roles WHERE userid = '{0}' AND rid={1} AND isdel=0", userID, roleID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                try
                {
                    if (reader.Read())
                    {
                        if (Convert.ToInt32(reader["rid"].ToString()) == roleID)
                        {
                            result = true;
                        }
                    }
                }
                catch (Exception)
                {
                    result = false;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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
            return result;
        }

        // Token: 0x060002A4 RID: 676 RVA: 0x00015128 File Offset: 0x00013328
        public static MarriageData GetMarriageData(DBManager dbMgr, int nRoleID)
        {
            MySQLConnection conn = null;
            MarriageData clientmarriagedata = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT spouseid, marrytype, ringid, goodwillexp, goodwillstar, goodwilllevel, givenrose, lovemessage, autoreject,changtime FROM t_marry WHERE roleid = {0}", nRoleID);
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                try
                {
                    MySQLDataReader reader = cmd.ExecuteReaderEx();
                    if (reader.Read())
                    {
                        clientmarriagedata = new MarriageData
                        {
                            nSpouseID = Global.SafeConvertToInt32(reader["spouseid"].ToString(), 10),
                            byMarrytype = Convert.ToSByte(reader["marrytype"].ToString()),
                            nRingID = Global.SafeConvertToInt32(reader["ringid"].ToString(), 10),
                            nGoodwillexp = Global.SafeConvertToInt32(reader["goodwillexp"].ToString(), 10),
                            byGoodwillstar = Convert.ToSByte(reader["goodwillstar"].ToString()),
                            byGoodwilllevel = Convert.ToSByte(reader["goodwilllevel"].ToString()),
                            nGivenrose = Global.SafeConvertToInt32(reader["givenrose"].ToString(), 10),
                            strLovemessage = reader["lovemessage"].ToString(),
                            byAutoReject = Convert.ToSByte(reader["autoreject"].ToString()),
                            ChangTime = reader["changtime"].ToString()
                        };
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
            return clientmarriagedata;
        }

        // Token: 0x060002A5 RID: 677 RVA: 0x00015340 File Offset: 0x00013540
        public static void QueryMarryPartyList(DBManager dbMgr, Dictionary<int, MarryPartyData> partyList)
        {
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT roleid, partytype, joincount, starttime, husbandid, wifeid, (SELECT rname FROM t_roles WHERE rid = husbandid) AS husbandname, (SELECT rname FROM t_roles WHERE rid = wifeid) AS wifename FROM t_marryparty", new object[0]);
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                try
                {
                    MySQLDataReader reader = cmd.ExecuteReaderEx();
                    while (reader.Read())
                    {
                        MarryPartyData data = new MarryPartyData
                        {
                            RoleID = Convert.ToInt32(reader["roleid"].ToString()),
                            PartyType = Convert.ToInt32(reader["partytype"].ToString()),
                            JoinCount = Convert.ToInt32(reader["joincount"].ToString()),
                            StartTime = DataHelper.ConvertToTicks(reader["starttime"].ToString()),
                            HusbandRoleID = Convert.ToInt32(reader["husbandid"].ToString()),
                            WifeRoleID = Convert.ToInt32(reader["wifeid"].ToString()),
                            HusbandName = reader["husbandname"].ToString(),
                            WifeName = reader["wifename"].ToString()
                        };
                        partyList.Add(data.RoleID, data);
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
        }

        // Token: 0x060002A6 RID: 678 RVA: 0x00015518 File Offset: 0x00013718
        public static bool CheckOrderNo(DBManager dbMgr, string order_no)
        {
            MySQLConnection conn = null;
            bool bHave = true;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("select * from `t_order` where order_no='{0}';", order_no);
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                bHave = reader.Read();
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return bHave;
        }

        // Token: 0x060002A7 RID: 679 RVA: 0x000155C4 File Offset: 0x000137C4
        public static bool CheckInputLogOrderNo(DBManager dbMgr, string order_no)
        {
            MySQLConnection conn = null;
            bool bHave = true;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("select * from `t_inputlog` where order_no='{0}';", order_no);
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                bHave = reader.Read();
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return bHave;
        }

        // Token: 0x060002A8 RID: 680 RVA: 0x00015670 File Offset: 0x00013870
        public static bool CheckInputLog2OrderNo(DBManager dbMgr, string order_no)
        {
            MySQLConnection conn = null;
            bool bHave = true;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("select * from `t_inputlog2` where order_no='{0}';", order_no);
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                bHave = reader.Read();
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return bHave;
        }

        // Token: 0x060002A9 RID: 681 RVA: 0x0001571C File Offset: 0x0001391C
        public static List<TianTiLogItemData> GetTianTiLogItemDataList(DBManager dbMgr, int roleId, int maxCount)
        {
            List<TianTiLogItemData> list = new List<TianTiLogItemData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT zoneid1,rolename1,zoneid2,rolename2,success,duanweijifenaward,rongyaoaward,endtime from t_kf_tianti_game_log where rid={0} order by endtime desc limit {1}", roleId, maxCount);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    TianTiLogItemData gmailData = new TianTiLogItemData();
                    gmailData.ZoneId1 = Convert.ToInt32(reader["zoneid1"].ToString());
                    gmailData.RoleName1 = reader["rolename1"].ToString();
                    gmailData.ZoneId2 = Convert.ToInt32(reader["zoneid2"].ToString());
                    gmailData.RoleName2 = reader["rolename2"].ToString();
                    gmailData.Success = Convert.ToInt32(reader["success"].ToString());
                    gmailData.DuanWeiJiFenAward = Convert.ToInt32(reader["duanweijifenaward"].ToString());
                    gmailData.RongYaoAward = Convert.ToInt32(reader["rongyaoaward"].ToString());
                    DateTime.TryParse(reader["endtime"].ToString(), out gmailData.EndTime);
                    list.Add(gmailData);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return list;
        }

        // Token: 0x060002AA RID: 682 RVA: 0x000158D0 File Offset: 0x00013AD0
        public static List<TianTiLogItemData> GetT5v5ItemDataList(DBManager dbMgr, int roleId, int maxCount)
        {
            List<TianTiLogItemData> list = new List<TianTiLogItemData>();
            using (MyDbConnection3 conn = new MyDbConnection3(false))
            {
                string cmdText = string.Format("SELECT zoneid1,rolename1,zoneid2,rolename2,success,duanweijifenaward,rongyaoaward,endtime from t_5v5_game_log where rid={0} order by endtime desc limit {1}", roleId, maxCount);
                MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
                while (reader.Read())
                {
                    TianTiLogItemData gmailData = new TianTiLogItemData();
                    gmailData.ZoneId1 = Convert.ToInt32(reader["zoneid1"].ToString());
                    gmailData.RoleName1 = reader["rolename1"].ToString();
                    gmailData.ZoneId2 = Convert.ToInt32(reader["zoneid2"].ToString());
                    gmailData.RoleName2 = reader["rolename2"].ToString();
                    gmailData.Success = Convert.ToInt32(reader["success"].ToString());
                    gmailData.DuanWeiJiFenAward = Convert.ToInt32(reader["duanweijifenaward"].ToString());
                    gmailData.RongYaoAward = Convert.ToInt32(reader["rongyaoaward"].ToString());
                    DateTime.TryParse(reader["endtime"].ToString(), out gmailData.EndTime);
                    list.Add(gmailData);
                }
            }
            return list;
        }

        // Token: 0x060002AB RID: 683 RVA: 0x00015A48 File Offset: 0x00013C48
        public static List<GroupMailData> ScanNewGroupMailFromTable(DBManager dbMgr, int beginID)
        {
            List<GroupMailData> GroupMailList = null;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string cmdText = string.Format("SELECT * from t_groupmail where gmailid > {0} and endtime > '{1}'", beginID, today);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    GroupMailData gmailData = new GroupMailData();
                    gmailData.GMailID = Convert.ToInt32(reader["gmailid"].ToString());
                    gmailData.Subject = reader["subject"].ToString();
                    gmailData.Content = reader["content"].ToString();
                    gmailData.Conditions = reader["conditions"].ToString();
                    gmailData.InputTime = DateTime.Parse(reader["inputtime"].ToString()).Ticks;
                    gmailData.EndTime = DateTime.Parse(reader["endtime"].ToString()).Ticks;
                    gmailData.Yinliang = Convert.ToInt32(reader["yinliang"].ToString());
                    gmailData.Tongqian = Convert.ToInt32(reader["tongqian"].ToString());
                    gmailData.YuanBao = Convert.ToInt32(reader["yuanbao"].ToString());
                    gmailData.GoodsList = Global.ParseGoodsDataList(reader["goodlist"].ToString());
                    if (null == GroupMailList)
                    {
                        GroupMailList = new List<GroupMailData>();
                    }
                    GroupMailList.Add(gmailData);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return GroupMailList;
        }

        // Token: 0x060002AC RID: 684 RVA: 0x00015C68 File Offset: 0x00013E68
        public static bool IsBlackUserID(DBManager dbMgr, string userid)
        {
            bool result;
            using (MyDbConnection3 conn = new MyDbConnection3(false))
            {
                string cmdText = string.Format("SELECT count(*) from t_blackuserid where userid='{0}' limit 1;", userid);
                result = (conn.GetSingleInt(cmdText, 0, new MySQLParameter[0]) > 0);
            }
            return result;
        }

        // Token: 0x060002AD RID: 685 RVA: 0x00015CC0 File Offset: 0x00013EC0
        public static RoleMiniInfo QueryRoleMiniInfo(long rid)
        {
            RoleMiniInfo roleMiniInfo = null;
            using (MyDbConnection3 conn = new MyDbConnection3(false))
            {
                string cmdText = string.Format("SELECT zoneid,userid from t_roles where rid={0};", rid);
                MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
                if (reader.Read())
                {
                    roleMiniInfo = new RoleMiniInfo();
                    roleMiniInfo.roleId = rid;
                    roleMiniInfo.zoneId = Convert.ToInt32(reader["zoneid"].ToString());
                    roleMiniInfo.userId = reader["userid"].ToString();
                }
            }
            return roleMiniInfo;
        }

        // Token: 0x060002AE RID: 686 RVA: 0x00015D74 File Offset: 0x00013F74
        public static List<TenAwardData> ScanNewGroupTenFromTable(DBManager dbMgr)
        {
            List<TenAwardData> groupList = null;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT * FROM t_ten WHERE state=0 ORDER BY id LIMIT 100", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    TenAwardData d = new TenAwardData();
                    d.DbID = Convert.ToInt32(reader["id"].ToString());
                    d.RoleID = Convert.ToInt32(reader["roleID"].ToString());
                    d.AwardID = Convert.ToInt32(reader["giftID"].ToString());
                    d.UserID = reader["uID"].ToString();
                    if (null == groupList)
                    {
                        groupList = new List<TenAwardData>();
                    }
                    groupList.Add(d);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return groupList;
        }

        // Token: 0x060002AF RID: 687 RVA: 0x00015EB4 File Offset: 0x000140B4
        public static int TenOnlyNum(DBManager dbMgr, string userID, int awardID)
        {
            int totalNum = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT COUNT(*) AS totalnum FROM t_ten WHERE giftID='{0}' and uID='{1}' and state>1", awardID, userID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    totalNum = Convert.ToInt32(reader["totalnum"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return totalNum;
        }

        // Token: 0x060002B0 RID: 688 RVA: 0x00015F74 File Offset: 0x00014174
        public static int TenDayNum(DBManager dbMgr, string userID, int awardID)
        {
            int totalNum = 0;
            MySQLConnection conn = null;
            try
            {
                string strBegin = string.Format("{0:yyyy-MM-dd 00:00:00}", DateTime.Now);
                string strEnd = string.Format("{0:yyyy-MM-dd 23:59:59}", DateTime.Now);
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT COUNT(*) AS totalnum FROM t_ten WHERE giftID='{0}' and uID='{1}' and state>1 and updatetime>='{2}' and updatetime<='{3}';", new object[]
                {
                    awardID,
                    userID,
                    strBegin,
                    strEnd
                });
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    totalNum = Convert.ToInt32(reader["totalnum"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return totalNum;
        }

        // Token: 0x060002B1 RID: 689 RVA: 0x00016080 File Offset: 0x00014280
        public static bool ActivateStateGet(DBManager dbMgr, string userID)
        {
            bool isActivate = false;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT * FROM t_activate WHERE userID='{0}' LIMIT 1;", userID);
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    isActivate = true;
                }
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return isActivate;
        }

        // Token: 0x060002B2 RID: 690 RVA: 0x00016124 File Offset: 0x00014324
        public static List<GiftCodeAwardData> ScanNewGiftCodeFromTable(DBManager dbMgr)
        {
            List<GiftCodeAwardData> GiftList = null;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT * FROM t_giftcode WHERE mailid=0 ORDER BY id asc LIMIT 100", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    GiftCodeAwardData data = new GiftCodeAwardData();
                    data.Dbid = Convert.ToInt32(reader["id"].ToString());
                    data.UserId = reader["userid"].ToString();
                    data.RoleID = Convert.ToInt32(reader["rid"].ToString());
                    data.GiftId = reader["giftid"].ToString();
                    data.CodeNo = reader["codeno"].ToString();
                    if (null == GiftList)
                    {
                        GiftList = new List<GiftCodeAwardData>();
                    }
                    GiftList.Add(data);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return GiftList;
        }

        // Token: 0x060002B3 RID: 691 RVA: 0x00016288 File Offset: 0x00014488
        public static List<FacebookAwardData> ScanNewGroupFacebookFromTable(DBManager dbMgr)
        {
            List<FacebookAwardData> groupList = null;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT * FROM t_facebook WHERE state=0 ORDER BY id LIMIT 100", new object[0]);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    FacebookAwardData d = new FacebookAwardData();
                    d.DbID = Convert.ToInt32(reader["id"].ToString());
                    d.RoleID = Convert.ToInt32(reader["roleID"].ToString());
                    d.AwardID = Convert.ToInt32(reader["giftID"].ToString());
                    if (null == groupList)
                    {
                        groupList = new List<FacebookAwardData>();
                    }
                    groupList.Add(d);
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return groupList;
        }

        // Token: 0x060002B4 RID: 692 RVA: 0x000163B0 File Offset: 0x000145B0
        public static int FacebookOnlyNum(DBManager dbMgr, int roleID, int awardID)
        {
            int totalNum = 0;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT COUNT(*) AS totalnum FROM t_facebook WHERE giftID={0} and roleID={1} and state>1", awardID, roleID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    totalNum = Convert.ToInt32(reader["totalnum"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return totalNum;
        }

        // Token: 0x060002B5 RID: 693 RVA: 0x00016474 File Offset: 0x00014674
        public static int FacebookDayNum(DBManager dbMgr, int roleID, int awardID)
        {
            int totalNum = 0;
            MySQLConnection conn = null;
            try
            {
                string strBegin = string.Format("{0:yyyy-MM-dd 00:00:00}", DateTime.Now);
                string strEnd = string.Format("{0:yyyy-MM-dd 23:59:59}", DateTime.Now);
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT COUNT(*) AS totalnum FROM t_facebook WHERE giftID={0} and roleID={1} and state>1 and updatetime>='{2}' and updatetime<='{3}';", new object[]
                {
                    awardID,
                    roleID,
                    strBegin,
                    strEnd
                });
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    totalNum = Convert.ToInt32(reader["totalnum"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return totalNum;
        }

        // Token: 0x060002B6 RID: 694 RVA: 0x00016584 File Offset: 0x00014784
        public static bool GetMinRegtime(DBManager dbMgr, string userid, out string OutUserid, out string Regtime)
        {
            OutUserid = "0";
            Regtime = "0";
            using (MyDbConnection3 conn = new MyDbConnection3(false))
            {
                string cmdText = string.Format(SqlDefineManager.RegressGetMinRegtime, userid);
                MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
                if (reader.Read())
                {
                    OutUserid = reader["userid"].ToString();
                    Regtime = reader["regtime"].ToString();
                    return true;
                }
            }
            return false;
        }

        // Token: 0x060002B7 RID: 695 RVA: 0x00016628 File Offset: 0x00014828
        public static bool GetRegressAwardHistoryForUser(DBManager dbMgr, string userid, int activitytype, string stage, out Dictionary<string, string> SignData)
        {
            SignData = new Dictionary<string, string>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format(SqlDefineManager.RegressGetSignData, userid, activitytype, stage);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    string lastgettime = reader["lastgettime"].ToString();
                    string activitydata = reader["activitydata"].ToString();
                    if (SignData.ContainsKey(lastgettime))
                    {
                        SignData[lastgettime] = activitydata;
                    }
                    else
                    {
                        SignData.Add(lastgettime, activitydata);
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
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

        // Token: 0x060002B8 RID: 696 RVA: 0x00016730 File Offset: 0x00014930
        public static int GetRegressAwardDayHistoryForUser(DBManager dbMgr, string userid, int activitytype, string keystr, string stage, out string lastgettime, out int hasgettimes, out string activitydata)
        {
            int ret = -1;
            lastgettime = "";
            hasgettimes = 0;
            activitydata = "";
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format(SqlDefineManager.RegressGetDaySignData, new object[]
                {
                    userid,
                    activitytype,
                    keystr,
                    stage
                });
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    lastgettime = reader["lastgettime"].ToString();
                    hasgettimes = Convert.ToInt32(reader["hasgettimes"].ToString());
                    activitydata = reader["activitydata"].ToString();
                    ret = 1;
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return ret;
        }

        // Token: 0x060002B9 RID: 697 RVA: 0x00016850 File Offset: 0x00014A50
        public static int QueryUserLimitGoodsUsedNumByRoleID(DBManager dbMgr, string UserID, int goodsID, string stage, out int dayID, out int usedNum)
        {
            dayID = 0;
            usedNum = 0;
            int ret = -1;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format(SqlDefineManager.RegressSelectStore, UserID, goodsID, stage);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                if (reader.Read())
                {
                    dayID = Convert.ToInt32(reader["dayid"].ToString());
                    usedNum = Convert.ToInt32(reader["usednum"].ToString());
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
                ret = 0;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
            return ret;
        }

        // Token: 0x060002BA RID: 698 RVA: 0x00016938 File Offset: 0x00014B38
        public static bool QueryUserLimitGoodsUsedNumInfoByRoleID(DBManager dbMgr, string UserID, int dayID, string stage, out Dictionary<int, int> GoodsInfo)
        {
            GoodsInfo = new Dictionary<int, int>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format(SqlDefineManager.RegressSelectStoreInfo, UserID, dayID, stage);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();
                while (reader.Read())
                {
                    int goodsID = Convert.ToInt32(reader["goodsid"].ToString());
                    int usedNum = Convert.ToInt32(reader["usednum"].ToString());
                    if (GoodsInfo.ContainsKey(goodsID))
                    {
                        Dictionary<int, int> dictionary;
                        int key;
                        (dictionary = GoodsInfo)[key = goodsID] = dictionary[key] + usedNum;
                    }
                    else
                    {
                        GoodsInfo.Add(goodsID, usedNum);
                    }
                }
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd.Dispose();
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

        // Token: 0x060002BB RID: 699 RVA: 0x00016A58 File Offset: 0x00014C58
        public static bool GetAllRebornEquipHole(DBManager dbMgr, int rid, out Dictionary<int, RebornEquipData> data)
        {
            data = new Dictionary<int, RebornEquipData>();
            using (MyDbConnection3 conn = new MyDbConnection3(false))
            {
                string cmdText = string.Format(SqlDefineManager.RebornEquipHoleSelectInfo, rid);
                MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
                while (reader.Read())
                {
                    RebornEquipData temp = new RebornEquipData();
                    temp.RoleID = rid;
                    temp.HoleID = Convert.ToInt32(reader["holeid"].ToString());
                    temp.Level = Convert.ToInt32(reader["level"].ToString());
                    temp.Able = Convert.ToInt32(reader["able"].ToString());
                    if (data.ContainsKey(temp.HoleID))
                    {
                        return false;
                    }
                    data.Add(temp.HoleID, temp);
                }
            }
            return true;
        }

        // Token: 0x060002BC RID: 700 RVA: 0x00016B64 File Offset: 0x00014D64
        public static bool GetMazingerStoreInfo(DBManager dbMgr, int rid, out Dictionary<int, MazingerStoreData> data)
        {
            data = new Dictionary<int, MazingerStoreData>();
            using (MyDbConnection3 conn = new MyDbConnection3(false))
            {
                string cmdText = string.Format(SqlDefineManager.MazingerStoreSelectInfo, rid);
                MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
                while (reader.Read())
                {
                    MazingerStoreData temp = new MazingerStoreData();
                    temp.RoleID = rid;
                    temp.Type = Convert.ToInt32(reader["type"].ToString());
                    temp.Stage = Convert.ToInt32(reader["stage"].ToString());
                    temp.StarLevel = Convert.ToInt32(reader["level"].ToString());
                    temp.Exp = Convert.ToInt32(reader["exp"].ToString());
                    if (data.ContainsKey(temp.Type))
                    {
                        return false;
                    }
                    data.Add(temp.Type, temp);
                }
            }
            return true;
        }
    }
}
