using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Data;
using Server.Tools;

namespace KF.Remoting
{
    
    public class KuaFuLueDuoPersistence
    {
        
        private KuaFuLueDuoPersistence()
        {
        }

        
        public bool LoadDatabase(int seasonID, int seasonIDLast, int minSeasonID)
        {
            try
            {
                if (!this.LoadKuaFuLueDuoServerData(minSeasonID))
                {
                    return false;
                }
                if (!this.LoadKuaFuLueDuoBHData(this.KuaFuLueDuoBHDataDict, minSeasonID))
                {
                    return false;
                }
                if (!this.LoadKuaFuLueDuoBHData_Join(seasonID, this.JingJiaDict))
                {
                    return false;
                }
                if (!this.LoadRankData(minSeasonID, seasonIDLast))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteException(ex.ToString());
            }
            return false;
        }

        
        public bool LoadRankData(int minSeasonID, int seasonIDLast)
        {
            for (int rankLoop = 0; rankLoop < 6; rankLoop++)
            {
                if (!this.LoadKuaFuLueDuoRankInfo(rankLoop, minSeasonID, seasonIDLast, this.KuaFuLueDuoRankInfoDict))
                {
                    return false;
                }
            }
            return true;
        }

        
        public void AddDelayWriteSql(string sql)
        {
            lock (this.Mutex)
            {
                this.DelayWriteSqlQueue.Enqueue(sql);
            }
        }

        
        private void WriteDataToDb(string sql)
        {
            try
            {
                LogManager.WriteLog(LogTypes.SQL, sql, null, true);
                DbHelperMySQL.ExecuteSql(sql);
            }
            catch (Exception ex)
            {
                LogManager.WriteException(string.Format("sql: {0}\r\n{1}", sql, ex.ToString()));
            }
        }

        
        public void DelayWriteDataProc()
        {
            List<string> list = null;
            lock (this.Mutex)
            {
                if (this.DelayWriteSqlQueue.Count == 0)
                {
                    return;
                }
                list = this.DelayWriteSqlQueue.ToList<string>();
                this.DelayWriteSqlQueue.Clear();
            }
            foreach (string sql in list)
            {
                this.WriteDataToDb(sql);
            }
        }

        
        private int ExecuteSqlNoQuery(string sqlCmd)
        {
            int result;
            try
            {
                LogManager.WriteLog(LogTypes.SQL, sqlCmd, null, true);
                result = DbHelperMySQL.ExecuteSql(sqlCmd);
            }
            catch (Exception ex)
            {
                LogManager.WriteException(sqlCmd + ex.ToString());
                result = -1;
            }
            return result;
        }

        
        public void ClearLastSeasonData()
        {
            this.ExecuteSqlNoQuery("delete from t_kfld_role;");
            this.ExecuteSqlNoQuery("update `t_kfld_server` set `mingxing`='',`zhengfu`=0,`sum_ziyuan`=0,`last_ziyuan`=0;");
            this.ExecuteSqlNoQuery("update `t_kfld_banghui` set `sum_ziyuan`='0',`last`=0;");
        }

        
        public void SaveKuaFuLueDuoBHData(KuaFuLueDuoBHData data)
        {
            string sql = string.Format("INSERT INTO t_kfld_banghui(`season`, `bhid`, `bhname`, `zoneid`, `sum_ziyuan`, `last`) VALUES({0},{1},'{2}',{3},{4},{5}) ON DUPLICATE KEY UPDATE `season`='{0}',`bhname`='{2}',`zoneid`={3},`sum_ziyuan`='{4}',`last`={5}", new object[]
            {
                data.season,
                data.bhid,
                data.bhname,
                data.zoneid,
                data.sum_ziyuan,
                data.last_ziyuan
            });
            this.ExecuteSqlNoQuery(sql);
        }

        
        public void SaveKuaFuLueDuoBHSeasonData(int season, KuaFuLueDuoBHData data)
        {
            string sql = string.Format("INSERT INTO t_kfld_banghui_season(`season`, `bhid`, `bhname`, `zoneid`, `jingjia`, `jingjia_sid`, `last_jingjia`,`group`) VALUES({0},{1},'{2}',{3},{4},{5},{6},{7}) ON DUPLICATE KEY UPDATE `bhname`='{2}',`zoneid`={3},`jingjia`='{4}',`jingjia_sid`={5},`last_jingjia`={6}", new object[]
            {
                data.season,
                data.bhid,
                data.bhname,
                data.zoneid,
                data.jingjia,
                data.jingjia_sid,
                data.last_jingjia,
                data.group
            });
            this.ExecuteSqlNoQuery(sql);
        }

        
        public void SaveKuaFuLueDuoRoleData(int season, int rid, string rname, int zoneid, int kill)
        {
            string sql = string.Format("INSERT INTO t_kfld_role(`rid`, `rname`,`zoneid`, `kill`, `season`,`last`) VALUES({0},'{1}',{2},{3},{4},{3}) ON DUPLICATE KEY UPDATE `rname`='{1}', `kill`=`kill`+{3},`last`={3},`season`={4}", new object[]
            {
                rid,
                rname,
                zoneid,
                kill,
                season
            });
            this.AddDelayWriteSql(sql);
        }

        
        public void SaveKuaFuLueDuoServerData(int season, KuaFuLueDuoServerInfo data)
        {
            string sql = string.Format("INSERT INTO t_kfld_server(`serverid`, `sum_ziyuan`, `last_ziyuan`,`mingxing`,`season`) VALUES({0},{1},{2},'{3}',{4}) ON DUPLICATE KEY UPDATE sum_ziyuan='{1}', `last_ziyuan`={2}, `mingxing`='{3}',`season`={4}", new object[]
            {
                data.ServerId,
                data.ZiYuan,
                data.LastZiYuan,
                data.MingXingZhanMengList,
                season
            });
            this.ExecuteSqlNoQuery(sql);
        }

        
        public void SaveKuaFuLueDuoServerMingXing(int serverId, string mingxing)
        {
            string sql = string.Format("update t_kfld_server set `mingxing`='{1}' where serverid={0};", serverId, mingxing);
            this.ExecuteSqlNoQuery(sql);
        }

        
        public void SaveKuaFuLueDuoServerZhengFuData(int season, KuaFuLueDuoServerInfo data)
        {
            string sql = string.Format("UPDATE t_kfld_server set `season`={1},`zhengfu`={2} where `serverid`={0}", data.ServerId, season, (data.ZhengFuList == null) ? 0 : data.ZhengFuList.Count);
            this.AddDelayWriteSql(sql);
        }

        
        public void SaveKuaFuLueDuoServerRankData(int season, int group, KuaFuLueDuoServerInfo data, int destServerId, int percent)
        {
            if (destServerId > 0)
            {
                string sql = string.Format("INSERT IGNORE INTO `t_kfld_zhengfu` (`season`, `serverid`, `fall_sid`, `group`, `info`) VALUES ({0}, {1}, {2}, {3}, {4});", new object[]
                {
                    season,
                    data.ServerId,
                    destServerId,
                    group,
                    percent
                });
                this.AddDelayWriteSql(sql);
            }
        }

        
        public int LoadSeasonCount()
        {
            object value = DbHelperMySQL.GetSingle("select count(*) from `t_kfld_season`");
            int result;
            if (null == value)
            {
                result = 0;
            }
            else
            {
                result = Convert.ToInt32(value);
            }
            return result;
        }

        
        public void SaveSeasonID(int seasonID)
        {
            DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO `t_kfld_season` VALUES({0});", seasonID));
        }

        
        public int[] GetHistSeasonIDs(int maxCount)
        {
            int[] result = new int[3];
            try
            {
                string strSql = string.Format("SELECT `season` FROM `t_kfld_season` order by `season` desc limit {0}", maxCount);
                using (MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false))
                {
                    while (sdr != null && sdr.Read())
                    {
                        int season = Convert.ToInt32(sdr[0].ToString());
                        if (result[0] > 0)
                        {
                            result[1] = season;
                        }
                        if (result[0] == 0)
                        {
                            result[0] = season;
                        }
                        if (--maxCount == 0)
                        {
                            result[2] = season;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteExceptionUseCache(ex.ToString());
            }
            return result;
        }

        
        public void SaveSignUpRound(int signUpRound)
        {
            DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 7, signUpRound));
        }

        
        public int GetSignUpRound()
        {
            object value = DbHelperMySQL.GetSingle("select value from t_async where id = " + 7);
            int result;
            if (null == value)
            {
                result = 0;
            }
            else
            {
                result = Convert.ToInt32(value);
            }
            return result;
        }

        
        private bool LoadKuaFuLueDuoBHData(Dictionary<int, KuaFuData<KuaFuLueDuoBHData>> KuaFuLueDuoBHDataDict, int minSeason)
        {
            try
            {
                long sAge = TimeUtil.AgeByNow();
                string strSql = string.Format("SELECT * FROM t_kfld_banghui where `season`>={0}", minSeason);
                using (MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false))
                {
                    while (sdr != null && sdr.Read())
                    {
                        KuaFuData<KuaFuLueDuoBHData> bhData = new KuaFuData<KuaFuLueDuoBHData>();
                        bhData.V.season = Convert.ToInt32(sdr["season"]);
                        bhData.V.bhid = Convert.ToInt32(sdr["bhid"]);
                        bhData.V.bhname = (sdr["bhname"] as string);
                        bhData.V.zoneid = Convert.ToInt32(sdr["zoneid"]);
                        bhData.V.sum_ziyuan = Convert.ToInt32(sdr["sum_ziyuan"]);
                        bhData.V.last_ziyuan = Convert.ToInt32(sdr["last"]);
                        bhData.Age = sAge;
                        KuaFuLueDuoBHDataDict[bhData.V.bhid] = bhData;
                        int zoneid = bhData.V.zoneid;
                        if (bhData.V.sum_ziyuan > 0 && zoneid > 0 && zoneid < this.ZoneID2ServerIDs.Length)
                        {
                            KuaFuLueDuoServerInfo data;
                            if (this.ServerInfoDict.TryGetValue(this.ZoneID2ServerIDs[zoneid], out data))
                            {
                                string name = KuaFuServerManager.FormatName(bhData.V.bhname, bhData.V.zoneid);
                                data.MingXingList.Add(new KuaFuLueDuoRankInfo
                                {
                                    Key = bhData.V.bhid,
                                    Param1 = name,
                                    Value = bhData.V.sum_ziyuan
                                });
                            }
                        }
                    }
                }
                foreach (KuaFuLueDuoServerInfo data in this.ServerInfoDict.Values)
                {
                    string mingxing = KuaFuLueDuoUtils.RankList2MingXingStr(data.MingXingList, 2);
                    if (mingxing != data.MingXingZhanMengList)
                    {
                        data.MingXingZhanMengList = mingxing;
                        string sql = string.Format("update t_kfld_server set `mingxing`='{1}' where `serverid`={0}", data.ServerId, mingxing);
                        this.ExecuteSqlNoQuery(sql);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteExceptionUseCache(ex.ToString());
                return false;
            }
            return true;
        }

        
        private bool LoadKuaFuLueDuoServerData(int minSeasonID)
        {
            try
            {
                string strSql = string.Format("SELECT * FROM t_kfld_server where `season`>={0}", minSeasonID);
                using (MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false))
                {
                    while (sdr != null && sdr.Read())
                    {
                        int serverId = Convert.ToInt32(sdr["serverid"]);
                        if (serverId >= 0 && serverId < this.ZoneID2ServerIDs.Length)
                        {
                            int sid = this.ZoneID2ServerIDs[serverId];
                            KuaFuLueDuoServerInfo data;
                            if (!this.ServerInfoDict.TryGetValue(sid, out data))
                            {
                                data = new KuaFuLueDuoServerInfo
                                {
                                    ServerId = sid
                                };
                                data.ZhengFuList = new List<int>();
                                data.ShiChouList = new List<int>();
                                this.ServerInfoDict[sid] = data;
                            }
                            int sum_ziyuan = Convert.ToInt32(sdr["sum_ziyuan"]);
                            int last_ziyuan = Convert.ToInt32(sdr["last_ziyuan"]);
                            if (data.ZiYuan <= sum_ziyuan)
                            {
                                data.ZiYuan = sum_ziyuan;
                                data.LastZiYuan = last_ziyuan;
                                data.SeasonId = Convert.ToInt32(sdr["season"]);
                                data.MingXingZhanMengList = sdr["mingxing"].ToString();
                            }
                            if (serverId != sid)
                            {
                                this.ExecuteSqlNoQuery(string.Format("update `t_kfld_server` set `season`=0 where `serverid`={0}", serverId));
                            }
                        }
                    }
                }
                strSql = string.Format("SELECT `serverid`,`fall_sid` FROM t_kfld_zhengfu where `season`>={0}", minSeasonID);
                using (MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false))
                {
                    while (sdr != null && sdr.Read())
                    {
                        int serverId = Convert.ToInt32(sdr["serverid"]);
                        int fall_sid = Convert.ToInt32(sdr["fall_sid"]);
                        if (serverId >= 0 && serverId < this.ZoneID2ServerIDs.Length && fall_sid >= 0 && fall_sid <= this.ZoneID2ServerIDs.Length)
                        {
                            int sa = this.ZoneID2ServerIDs[serverId];
                            KuaFuLueDuoServerInfo data;
                            if (this.ServerInfoDict.TryGetValue(sa, out data))
                            {
                                if (!data.ZhengFuList.Contains(fall_sid))
                                {
                                    data.ZhengFuList.Add(fall_sid);
                                }
                                int sb = this.ZoneID2ServerIDs[fall_sid];
                                if (this.ServerInfoDict.TryGetValue(sb, out data))
                                {
                                    if (!data.ShiChouList.Contains(sa))
                                    {
                                        data.ShiChouList.Add(sa);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (KuaFuLueDuoServerInfo data in this.ServerInfoDict.Values)
                {
                    if (data.SeasonId > 0)
                    {
                        string sql = string.Format("update t_kfld_server set `sum_ziyuan`={1}, `last_ziyuan`={2},`zhengfu`={3} where `serverid`={0}", new object[]
                        {
                            data.ServerId,
                            data.ZiYuan,
                            data.LastZiYuan,
                            data.ZhengFuList.Count
                        });
                        this.ExecuteSqlNoQuery(sql);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteExceptionUseCache(ex.ToString());
                return false;
            }
            return true;
        }

        
        private bool LoadKuaFuLueDuoBHData_Join(int season, Dictionary<int, KuaFuLueDuoServerJingJiaState> JingJiaDict)
        {
            try
            {
                KuaFuData<KuaFuLueDuoBHData> bhData = null;
                string strSql = string.Format("SELECT * FROM t_kfld_banghui_season WHERE `season`={0}", season);
                MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false);
                while (sdr != null && sdr.Read())
                {
                    int bhid = Convert.ToInt32(sdr["bhid"]);
                    if (this.KuaFuLueDuoBHDataDict.TryGetValue(bhid, out bhData))
                    {
                        int sid = Convert.ToInt32(sdr["jingjia_sid"]);
                        int jingjia = Convert.ToInt32(sdr["jingjia"]);
                        int last_jingjia = Convert.ToInt32(sdr["last_jingjia"]);
                        int group = Convert.ToInt32(sdr["group"]);
                        string bhname = sdr["bhname"] as string;
                        int zoneid = Convert.ToInt32(sdr["zoneid"]);
                        bhData.V.jingjia = jingjia;
                        bhData.V.jingjia_sid = sid;
                        bhData.V.last_jingjia = last_jingjia;
                        bhData.V.group = group;
                        if (sid > 0)
                        {
                            KuaFuLueDuoServerJingJiaState jjData;
                            if (!JingJiaDict.TryGetValue(bhData.V.jingjia_sid, out jjData))
                            {
                                jjData = new KuaFuLueDuoServerJingJiaState
                                {
                                    ServerId = sid,
                                    JingJiaList = new List<KuaFuLueDuoBangHuiJingJiaData>()
                                };
                                JingJiaDict[bhData.V.jingjia_sid] = jjData;
                            }
                            KuaFuLueDuoBangHuiJingJiaData jjbhData = jjData.JingJiaList.Find((KuaFuLueDuoBangHuiJingJiaData x) => x.BhId == bhid);
                            if (jjbhData == null)
                            {
                                jjbhData = new KuaFuLueDuoBangHuiJingJiaData
                                {
                                    BhId = bhid,
                                    BhName = bhname,
                                    ZoneId = zoneid,
                                    ServerId = sid,
                                    ZiJin = jingjia
                                };
                                jjData.JingJiaList.Add(jjbhData);
                            }
                            else
                            {
                                jjbhData.ZiJin = jingjia;
                            }
                        }
                    }
                }
                if (sdr != null)
                {
                    sdr.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteExceptionUseCache(ex.ToString());
                return false;
            }
            return true;
        }

        
        private string FormatLoadKuaFuLueDuoRankSql(int rankType, int minSeasonID, int seasonIDLast)
        {
            int RankLimit = KuaFuLueDuoConsts.MatchRankLimit[rankType];
            string strSql = "";
            switch (rankType)
            {
                case 0:
                    strSql = string.Format("SELECT `serverid` a,`zhengfu` b,`serverid` p1,`serverid` z FROM t_kfld_server WHERE season>={0} and `zhengfu`>0 ORDER BY `zhengfu` DESC,`serverid`;", minSeasonID);
                    break;
                case 2:
                    strSql = string.Format("SELECT `bhid` a,sum_ziyuan b,`bhname` p1, `zoneid` z FROM t_kfld_banghui WHERE season>={0} and `sum_ziyuan`>0 ORDER BY `sum_ziyuan` DESC,`bhid` LIMIT {1};", minSeasonID, RankLimit);
                    break;
                case 3:
                    strSql = string.Format("SELECT `bhid` a,last b,`bhname` p1, `zoneid` z FROM t_kfld_banghui WHERE season={0} and `last`>0 ORDER BY `last` DESC,`bhid` LIMIT {1};", seasonIDLast, RankLimit);
                    break;
                case 4:
                    strSql = string.Format("SELECT `rid` a,`kill` b,`rname` p1, `zoneid` z FROM t_kfld_role WHERE season>={0} and `kill`>0 ORDER BY `kill` DESC,rid LIMIT {1};", minSeasonID, RankLimit);
                    break;
                case 5:
                    strSql = string.Format("SELECT `rid` a,`last` b,`rname` p1, `zoneid` z FROM t_kfld_role WHERE season={0} and `last`>0 ORDER BY `last` DESC,rid LIMIT {1};", seasonIDLast, RankLimit);
                    break;
            }
            return strSql;
        }

        
        public int GetRoleKillNum(int minSeasonID, long rid)
        {
            string sql = string.Format("SELECT `kill` FROM t_kfld_role WHERE rid={0} and season>={1};", rid, minSeasonID);
            object value = DbHelperMySQL.GetSingle(sql);
            int result;
            if (null == value)
            {
                result = 0;
            }
            else
            {
                result = Convert.ToInt32(value);
            }
            return result;
        }

        
        private bool LoadKuaFuLueDuoRankInfo(int rankType, int minSeasonID, int seasonLast, Dictionary<int, KuaFuLueDuoRankListData> KuaFuLueDuoRankInfoDict)
        {
            try
            {
                long sAge = TimeUtil.AgeByNow();
                int length = this.ZoneID2GroupIDs.Length;
                KuaFuLueDuoRankListData[] rankArr = new KuaFuLueDuoRankListData[length];
                List<KuaFuLueDuoRankInfo>[] listArr = new List<KuaFuLueDuoRankInfo>[length];
                for (int i = 0; i < this.ZoneID2GroupIDs.Length; i++)
                {
                    int groupID = this.ZoneID2GroupIDs[i];
                    KuaFuLueDuoRankListData rankData;
                    if (!KuaFuLueDuoRankInfoDict.TryGetValue(groupID, out rankData))
                    {
                        rankData = new KuaFuLueDuoRankListData
                        {
                            Age = sAge
                        };
                        rankData.LastInfoDict = new Dictionary<int, KuaFuLueDuoRankInfo>();
                        rankData.SelfInfoDict = new Dictionary<int, KuaFuLueDuoRankInfo>();
                        KuaFuLueDuoRankInfoDict[groupID] = rankData;
                    }
                    List<KuaFuLueDuoRankInfo> rankList;
                    if (!rankData.ListDict.TryGetValue(rankType, out rankList))
                    {
                        rankList = new List<KuaFuLueDuoRankInfo>();
                        rankData.ListDict[rankType] = rankList;
                    }
                    rankArr[i] = rankData;
                    listArr[i] = rankList;
                }
                string strSql = this.FormatLoadKuaFuLueDuoRankSql(rankType, minSeasonID, seasonLast);
                if (!string.IsNullOrEmpty(strSql))
                {
                    int rank = 0;
                    using (MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(strSql, false))
                    {
                        while (sdr != null && sdr.Read())
                        {
                            int zoneID = Convert.ToInt32(sdr[3]);
                            if (zoneID < length)
                            {
                                rank++;
                                KuaFuLueDuoRankListData rankData = rankArr[zoneID];
                                KuaFuLueDuoRankInfo rankInfo = new KuaFuLueDuoRankInfo();
                                int zoneId = Convert.ToInt32(sdr[3]);
                                rankInfo.Key = Convert.ToInt32(sdr[0]);
                                rankInfo.Value = Convert.ToInt32(sdr[1]);
                                rankInfo.Param1 = sdr[2].ToString();
                                switch (rankType)
                                {
                                    case 0:
                                        listArr[zoneID].Add(rankInfo);
                                        if (rank == 1)
                                        {
                                            rankData.LastInfoDict[0] = rankInfo;
                                        }
                                        break;
                                    case 2:
                                    case 4:
                                        rankInfo.Param1 = KuaFuServerManager.FormatName(rankInfo.Param1, zoneId);
                                        listArr[zoneID].Add(rankInfo);
                                        break;
                                    case 3:
                                    case 5:
                                        rankInfo.Param1 = KuaFuServerManager.FormatName(rankInfo.Param1, zoneId);
                                        rankData.LastInfoDict[rankType - 1] = rankInfo;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteExceptionUseCache(ex.ToString());
                return false;
            }
            return true;
        }

        
        public static readonly KuaFuLueDuoPersistence Instance = new KuaFuLueDuoPersistence();

        
        public object Mutex = new object();

        
        public int SeasonCount;

        
        public Dictionary<int, KuaFuLueDuoServerInfo> ServerInfoDict = new Dictionary<int, KuaFuLueDuoServerInfo>();

        
        public int[] ZoneID2GroupIDs = new int[0];

        
        public int[] ZoneID2ServerIDs = new int[0];

        
        public Dictionary<int, KuaFuLueDuoRankListData> KuaFuLueDuoRankInfoDict = new Dictionary<int, KuaFuLueDuoRankListData>();

        
        public Dictionary<int, KuaFuData<KuaFuLueDuoBHData>> KuaFuLueDuoBHDataDict = new Dictionary<int, KuaFuData<KuaFuLueDuoBHData>>();

        
        public Dictionary<int, KuaFuLueDuoServerJingJiaState> JingJiaDict = new Dictionary<int, KuaFuLueDuoServerJingJiaState>();

        
        public Queue<string> DelayWriteSqlQueue = new Queue<string>();
    }
}
