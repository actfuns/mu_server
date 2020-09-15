using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace KF.Remoting
{
    // Token: 0x02000012 RID: 18
    internal class CoupleWishRecordManager
    {
        // Token: 0x06000095 RID: 149 RVA: 0x00008400 File Offset: 0x00006600
        public CoupleWishRecordManager(int week)
        {
            this.UpdateWeek(week);
        }

        // Token: 0x06000096 RID: 150 RVA: 0x00008454 File Offset: 0x00006654
        public void AddWishRecord(KuaFuRoleMiniData from, int wishType, string wishTxt, int toDbCoupleId, KuaFuRoleMiniData toMan, KuaFuRoleMiniData toWife)
        {
            lock (this.Mutex)
            {
                if (this.Persistence.SaveWishRecord(this.ThisWeek, from, wishType, wishTxt, toDbCoupleId, toMan, toWife))
                {
                    this.AddCachedWishOther(from, wishType, wishTxt, toMan, toWife);
                    this.AddCachedBeWished(toMan, toWife, wishType, wishTxt, from);
                    this.AddCachedBeWished(toWife, toMan, wishType, wishTxt, from);
                }
            }
        }

        // Token: 0x06000097 RID: 151 RVA: 0x000084E4 File Offset: 0x000066E4
        public List<CoupleWishWishRecordData> GetWishRecord(int roleId)
        {
            List<CoupleWishWishRecordData> result = null;
            MySqlDataReader sdr = null;
            try
            {
                lock (this.Mutex)
                {
                    Queue<CoupleWishWishRecordData> wishQ = null;
                    if (this.RoleWishRecords.TryGetValue(roleId, out wishQ))
                    {
                        result = wishQ.ToList<CoupleWishWishRecordData>();
                    }
                    if (result == null)
                    {
                        string sql = string.Format("SELECT `from_rid`,`from_zoneid`,`from_rname`,`to_man_rid`,`to_man_zoneid`,`to_man_rname`,`to_wife_rid`,`to_wife_zoneid`,`to_wife_rname`,`wish_type`,`wish_txt`  FROM t_couple_wish_wish_log WHERE `week`={0} AND (`from_rid`={1} OR `to_man_rid`={1} OR `to_wife_rid`={1}) ORDER BY `time` LIMIT {2};", this.ThisWeek, roleId, CoupleWishConsts.MaxWishRecordNum);
                        sdr = DbHelperMySQL.ExecuteReader(sql, false);
                        wishQ = (this.RoleWishRecords[roleId] = new Queue<CoupleWishWishRecordData>());
                        while (sdr != null && sdr.Read())
                        {
                            CoupleWishWishRoleReq req = new CoupleWishWishRoleReq();
                            req.From.RoleId = Convert.ToInt32(sdr["from_rid"]);
                            req.From.ZoneId = Convert.ToInt32(sdr["from_zoneid"]);
                            req.From.RoleName = sdr["from_rname"].ToString();
                            req.ToMan.RoleId = Convert.ToInt32(sdr["to_man_rid"]);
                            req.ToMan.ZoneId = Convert.ToInt32(sdr["to_man_zoneid"]);
                            req.ToMan.RoleName = sdr["to_man_rname"].ToString();
                            req.ToWife.RoleId = Convert.ToInt32(sdr["to_wife_rid"]);
                            req.ToWife.ZoneId = Convert.ToInt32(sdr["to_wife_zoneid"]);
                            req.ToWife.RoleName = sdr["to_wife_rname"].ToString();
                            req.WishType = Convert.ToInt32(sdr["wish_type"]);
                            req.WishTxt = sdr["wish_txt"].ToString();
                            if (req.From.RoleId == roleId)
                            {
                                this.AddCachedWishOther(req.From, req.WishType, req.WishTxt, req.ToMan, req.ToWife);
                            }
                            if (req.ToMan.RoleId == roleId)
                            {
                                this.AddCachedBeWished(req.ToMan, req.ToWife, req.WishType, req.WishTxt, req.From);
                            }
                            if (req.ToWife.RoleId == roleId)
                            {
                                this.AddCachedBeWished(req.ToWife, req.ToMan, req.WishType, req.WishTxt, req.From);
                            }
                        }
                        result = wishQ.ToList<CoupleWishWishRecordData>();
                    }
                    this.RoleLastReadMs[roleId] = TimeUtil.NOW();
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
            return result;
        }

        // Token: 0x06000098 RID: 152 RVA: 0x00008848 File Offset: 0x00006A48
        public void UpdateWeek(int week)
        {
            lock (this.Mutex)
            {
                if (this.ThisWeek != week)
                {
                    this.RoleWishRecords.Clear();
                    this.RoleLastReadMs.Clear();
                    this.ThisWeek = week;
                }
            }
        }

        // Token: 0x06000099 RID: 153 RVA: 0x00008910 File Offset: 0x00006B10
        public void ClearUnActiveRecord()
        {
            lock (this.Mutex)
            {
                long nowMs = TimeUtil.NOW();
                int timeOutMs = 1800000;
                List<int> rmList = this.RoleLastReadMs.Keys.ToList<int>().FindAll((int _r) => this.RoleLastReadMs.ContainsKey(_r) && nowMs - this.RoleLastReadMs[_r] >= (long)timeOutMs);
                if (rmList != null)
                {
                    foreach (int r in rmList)
                    {
                        this.RoleLastReadMs.Remove(r);
                        this.RoleWishRecords.Remove(r);
                    }
                }
            }
        }

        // Token: 0x0600009A RID: 154 RVA: 0x00008A08 File Offset: 0x00006C08
        private void AddCachedWishOther(KuaFuRoleMiniData from, int wishType, string wishTxt, KuaFuRoleMiniData toMan, KuaFuRoleMiniData toWife)
        {
            lock (this.Mutex)
            {
                Queue<CoupleWishWishRecordData> fromRecords = null;
                if (this.RoleWishRecords.TryGetValue(from.RoleId, out fromRecords))
                {
                    CoupleWishWishRecordData rec = new CoupleWishWishRecordData();
                    rec.FromRole = from;
                    rec.TargetRoles = new List<KuaFuRoleMiniData>();
                    if (toMan.RoleId != from.RoleId)
                    {
                        rec.TargetRoles.Add(toMan);
                    }
                    if (toWife.RoleId != from.RoleId)
                    {
                        rec.TargetRoles.Add(toWife);
                    }
                    rec.WishType = wishType;
                    rec.WishTxt = wishTxt;
                    fromRecords.Enqueue(rec);
                    while (fromRecords.Count > CoupleWishConsts.MaxWishRecordNum)
                    {
                        fromRecords.Dequeue();
                    }
                }
            }
        }

        // Token: 0x0600009B RID: 155 RVA: 0x00008B04 File Offset: 0x00006D04
        private void AddCachedBeWished(KuaFuRoleMiniData to, KuaFuRoleMiniData toSpouse, int wishType, string wishTxt, KuaFuRoleMiniData from)
        {
            lock (this.Mutex)
            {
                if (to.RoleId != from.RoleId)
                {
                    Queue<CoupleWishWishRecordData> toRecords = null;
                    if (this.RoleWishRecords.TryGetValue(to.RoleId, out toRecords))
                    {
                        CoupleWishWishRecordData rec = new CoupleWishWishRecordData();
                        rec.FromRole = from;
                        rec.TargetRoles = new List<KuaFuRoleMiniData>();
                        rec.TargetRoles.Add(to);
                        if (toSpouse.RoleId != from.RoleId)
                        {
                            rec.TargetRoles.Add(toSpouse);
                        }
                        rec.WishType = wishType;
                        rec.WishTxt = wishTxt;
                        toRecords.Enqueue(rec);
                        while (toRecords.Count > CoupleWishConsts.MaxWishRecordNum)
                        {
                            toRecords.Dequeue();
                        }
                    }
                }
            }
        }

        // Token: 0x0400005A RID: 90
        private CoupleWishPersistence Persistence = CoupleWishPersistence.getInstance();

        // Token: 0x0400005B RID: 91
        private object Mutex = new object();

        // Token: 0x0400005C RID: 92
        private Dictionary<int, Queue<CoupleWishWishRecordData>> RoleWishRecords = new Dictionary<int, Queue<CoupleWishWishRecordData>>();

        // Token: 0x0400005D RID: 93
        private Dictionary<int, long> RoleLastReadMs = new Dictionary<int, long>();

        // Token: 0x0400005E RID: 94
        private int ThisWeek = 0;
    }
}
