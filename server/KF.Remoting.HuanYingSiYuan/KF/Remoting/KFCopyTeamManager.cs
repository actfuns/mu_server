using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Server.Tools;

namespace KF.Remoting
{
    
    public class KFCopyTeamManager
    {
        
        public void SetService(KuaFuCopyService service)
        {
            this._KFCopyService = service;
        }

        
        public KFCopyTeamCreateRsp CreateTeam(KFCopyTeamCreateReq req)
        {
            KFCopyTeamCreateRsp rsp = new KFCopyTeamCreateRsp();
            try
            {
                lock (this.Mutex)
                {
                    this.ForceLeaveRoom(req.Member.RoleID);
                    if (!ClientAgentManager.Instance().IsAnyKfAgentAlive())
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.KFServerIsBusy;
                        return rsp;
                    }
                    KFTeamCountControl control = this._KFCopyService.dbMgr.TeamControl;
                    if (control == null)
                    {
                        LogManager.WriteLog(LogTypes.Error, string.Format("跨服队伍创建失败,  丢失副本上线控制的配置文件 KFTeamCountControl", new object[0]), null, true);
                        rsp.ErrorCode = CopyTeamErrorCodes.ServerException;
                        return rsp;
                    }
                    HashSet<long> teamList = null;
                    if (!this.CopyId2Teams.TryGetValue(req.CopyId, out teamList))
                    {
                        teamList = new HashSet<long>();
                        this.CopyId2Teams[req.CopyId] = teamList;
                    }
                    CopyTeamData td = new CopyTeamData();
                    td.TeamID = req.TeamId;
                    td.LeaderRoleID = req.Member.RoleID;
                    td.FuBenId = req.CopyId;
                    td.MinZhanLi = req.MinCombat;
                    td.AutoStart = (req.AutoStart > 0);
                    td.AutoKick = req.AutoKick;
                    td.TeamRoles.Add(req.Member);
                    td.TeamRoles[0].IsReady = true;
                    td.TeamName = td.TeamRoles[0].RoleName;
                    td.MemberCount = td.TeamRoles.Count;
                    this.CopyTeamDict.Add(td.TeamID, td);
                    teamList.Add(td.TeamID);
                    this.TimeLimitCopy.Add(td.TeamID, TimeUtil.NOW() + (long)(control.TeamMaxWaitMinutes * 60 * 1000));
                    this.RoleId2JoinedTeam[req.Member.RoleID] = td.TeamID;
                    CopyTeamCreateData data = new CopyTeamCreateData();
                    data.Member = req.Member;
                    data.MinCombat = req.MinCombat;
                    data.CopyId = req.CopyId;
                    data.TeamId = td.TeamID;
                    data.AutoStart = req.AutoStart;
                    data.AutoKick = req.AutoKick;
                    this.AddAsyncEvent(new AsyncDataItem
                    {
                        EventType = KuaFuEventTypes.KFCopyTeamCreate,
                        Args = new object[]
                        {
                            req.Member.ServerId,
                            data
                        }
                    });
                    rsp.ErrorCode = CopyTeamErrorCodes.Success;
                    rsp.Data = data;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("跨服队伍创建异常, serverid={0}, role={1}, copyid={2}", req.Member.ServerId, req.Member.RoleID, req.CopyId), ex, true);
                rsp.ErrorCode = CopyTeamErrorCodes.CenterServerFailed;
            }
            return rsp;
        }

        
        public KFCopyTeamJoinRsp JoinTeam(KFCopyTeamJoinReq req)
        {
            KFCopyTeamJoinRsp rsp = new KFCopyTeamJoinRsp();
            try
            {
                lock (this.Mutex)
                {
                    this.ForceLeaveRoom(req.Member.RoleID);
                    CopyTeamData td = null;
                    if (!this.CopyTeamDict.TryGetValue(req.TeamId, out td))
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.TeamIsDestoryed;
                        return rsp;
                    }
                    if (td.StartTime > 0L)
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.TeamAlreadyStart;
                        return rsp;
                    }
                    if (td.MemberCount >= ConstData.CopyRoleMax(req.CopyId))
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.TeamIsFull;
                        return rsp;
                    }
                    req.Member.IsReady = false;
                    td.TeamRoles.Add(req.Member);
                    td.MemberCount = td.TeamRoles.Count;
                    if (!req.Member.IsReady)
                    {
                        req.Member.NoReadyTicks = TimeUtil.NOW();
                    }
                    this.RoleId2JoinedTeam[req.Member.RoleID] = td.TeamID;
                    CopyTeamJoinData data = new CopyTeamJoinData();
                    data.Member = req.Member;
                    data.TeamId = req.TeamId;
                    this.AddAsyncEvent(new AsyncDataItem
                    {
                        EventType = KuaFuEventTypes.KFCopyTeamJoin,
                        Args = new object[]
                        {
                            req.Member.ServerId,
                            data
                        }
                    });
                    rsp.ErrorCode = CopyTeamErrorCodes.Success;
                    rsp.Data = data;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("加入跨服副本队伍异常, serverid={0}, role={1}, teamid={2}", req.Member.ServerId, req.Member.RoleID, req.TeamId), ex, true);
                rsp.ErrorCode = CopyTeamErrorCodes.CenterServerFailed;
            }
            return rsp;
        }

        
        public KFCopyTeamKickoutRsp KickoutTeam(KFCopyTeamKickoutReq req)
        {
            KFCopyTeamKickoutRsp rsp = new KFCopyTeamKickoutRsp();
            try
            {
                lock (this.Mutex)
                {
                    CopyTeamData td = null;
                    if (!this.CopyTeamDict.TryGetValue(req.TeamId, out td))
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.TeamIsDestoryed;
                        return rsp;
                    }
                    if (td.StartTime > 0L)
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.TeamAlreadyStart;
                        return rsp;
                    }
                    if (td.LeaderRoleID != req.FromRoleId)
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.NotTeamLeader;
                        return rsp;
                    }
                    CopyTeamMemberData leader = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == td.LeaderRoleID);
                    if (leader == null || leader.RoleID != req.FromRoleId)
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.NotTeamLeader;
                        return rsp;
                    }
                    CopyTeamMemberData member = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == req.ToRoleId);
                    if (member == null)
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.NotInMyTeam;
                        return rsp;
                    }
                    td.TeamRoles.Remove(member);
                    td.MemberCount = td.TeamRoles.Count;
                    this.RoleId2JoinedTeam.Remove(req.ToRoleId);
                    CopyTeamKickoutData data = new CopyTeamKickoutData();
                    data.FromRoleId = req.FromRoleId;
                    data.ToRoleId = req.ToRoleId;
                    data.TeamId = req.TeamId;
                    this.AddAsyncEvent(new AsyncDataItem
                    {
                        EventType = KuaFuEventTypes.KFCopyTeamKickout,
                        Args = new object[]
                        {
                            leader.ServerId,
                            data
                        }
                    });
                    rsp.ErrorCode = CopyTeamErrorCodes.Success;
                    rsp.Data = data;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("踢出跨服副本队伍异常, role={0}, teamid={1}", req.FromRoleId, req.TeamId), ex, true);
                rsp.ErrorCode = CopyTeamErrorCodes.CenterServerFailed;
            }
            return rsp;
        }

        
        public KFCopyTeamLeaveRsp LeaveTeam(KFCopyTeamLeaveReq req)
        {
            KFCopyTeamLeaveRsp rsp = new KFCopyTeamLeaveRsp();
            try
            {
                lock (this.Mutex)
                {
                    CopyTeamData td = null;
                    if (!this.CopyTeamDict.TryGetValue(req.TeamId, out td))
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.TeamIsDestoryed;
                        return rsp;
                    }
                    if (td.StartTime > 0L)
                    {
                    }
                    CopyTeamMemberData member = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == req.RoleId);
                    if (member == null)
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.NotInMyTeam;
                        return rsp;
                    }
                    this.RoleId2JoinedTeam.Remove(member.RoleID);
                    td.TeamRoles.Remove(member);
                    td.MemberCount = td.TeamRoles.Count;
                    if (td.MemberCount <= 0)
                    {
                        this.RemoveTeam(td.TeamID);
                    }
                    else if (td.LeaderRoleID == member.RoleID)
                    {
                        td.LeaderRoleID = td.TeamRoles[0].RoleID;
                        td.TeamRoles[0].IsReady = true;
                        td.TeamName = td.TeamRoles[0].RoleName;
                    }
                    CopyTeamLeaveData data = new CopyTeamLeaveData();
                    data.TeamId = req.TeamId;
                    data.RoleId = req.RoleId;
                    this.AddAsyncEvent(new AsyncDataItem
                    {
                        EventType = KuaFuEventTypes.KFCopyTeamLeave,
                        Args = new object[]
                        {
                            req.ReqServerId,
                            data
                        }
                    });
                    rsp.ErrorCode = CopyTeamErrorCodes.Success;
                    rsp.Data = data;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("离开跨服副本队伍异常, role={0}, teamid={1}", req.RoleId, req.TeamId), ex, true);
                rsp.ErrorCode = CopyTeamErrorCodes.CenterServerFailed;
            }
            return rsp;
        }

        
        public KFCopyTeamSetReadyRsp TeamSetReady(KFCopyTeamSetReadyReq req)
        {
            KFCopyTeamSetReadyRsp rsp = new KFCopyTeamSetReadyRsp();
            try
            {
                lock (this.Mutex)
                {
                    CopyTeamData td = null;
                    if (!this.CopyTeamDict.TryGetValue(req.TeamId, out td))
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.TeamIsDestoryed;
                        return rsp;
                    }
                    if (req.Ready <= 0)
                    {
                        if (req.RoleId == td.LeaderRoleID)
                        {
                            rsp.ErrorCode = CopyTeamErrorCodes.TeamLeaderCant;
                            return rsp;
                        }
                    }
                    CopyTeamMemberData member = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == req.RoleId);
                    if (member == null)
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.NotInMyTeam;
                        return rsp;
                    }
                    if (td.StartTime > 0L)
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.TeamAlreadyStart;
                        return rsp;
                    }
                    member.IsReady = (req.Ready > 0);
                    if (!member.IsReady)
                    {
                        member.NoReadyTicks = TimeUtil.NOW();
                    }
                    CopyTeamReadyData data = new CopyTeamReadyData();
                    data.RoleId = req.RoleId;
                    data.TeamId = req.TeamId;
                    data.Ready = req.Ready;
                    this.AddAsyncEvent(new AsyncDataItem
                    {
                        EventType = KuaFuEventTypes.KFCopyTeamSetReady,
                        Args = new object[]
                        {
                            member.ServerId,
                            data
                        }
                    });
                    rsp.ErrorCode = CopyTeamErrorCodes.Success;
                    rsp.Data = data;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("更新跨服副本队伍准备状态异常, role={0}, teamid={1}", req.RoleId, req.TeamId), ex, true);
                rsp.ErrorCode = CopyTeamErrorCodes.CenterServerFailed;
            }
            return rsp;
        }

        
        public KFCopyTeamSetFlagRsp TeamSetFlag(KFCopyTeamSetFlagReq req)
        {
            KFCopyTeamSetFlagRsp rsp = new KFCopyTeamSetFlagRsp();
            try
            {
                lock (this.Mutex)
                {
                    CopyTeamData td = null;
                    if (!this.CopyTeamDict.TryGetValue(req.TeamId, out td))
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.TeamIsDestoryed;
                        return rsp;
                    }
                    if (req.RoleId != td.LeaderRoleID)
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.NotTeamLeader;
                        return rsp;
                    }
                    CopyTeamMemberData member = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == req.RoleId);
                    if (member == null)
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.NotInMyTeam;
                        return rsp;
                    }
                    if (td.StartTime > 0L)
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.Success;
                        return rsp;
                    }
                    if (req.AutoStart >= 0)
                    {
                        td.AutoStart = (req.AutoStart > 0);
                    }
                    if (req.AutoKick >= 0)
                    {
                        td.AutoKick = req.AutoKick;
                    }
                    CopyTeamFlagData data = new CopyTeamFlagData();
                    data.RoleId = req.RoleId;
                    data.TeamId = req.TeamId;
                    data.AutoStart = (td.AutoStart ? 1 : 0);
                    data.AutoKick = td.AutoKick;
                    this.AddAsyncEvent(new AsyncDataItem
                    {
                        EventType = KuaFuEventTypes.KFCopyTeamSetFlag,
                        Args = new object[]
                        {
                            member.ServerId,
                            data
                        }
                    });
                    rsp.ErrorCode = CopyTeamErrorCodes.Success;
                    rsp.Data = data;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("更新跨服副本队伍准备状态异常, role={0}, teamid={1}", req.RoleId, req.TeamId), ex, true);
                rsp.ErrorCode = CopyTeamErrorCodes.CenterServerFailed;
            }
            return rsp;
        }

        
        public KFCopyTeamStartRsp StartGame(KFCopyTeamStartReq req)
        {
            KFCopyTeamStartRsp rsp = new KFCopyTeamStartRsp();
            try
            {
                lock (this.Mutex)
                {
                    CopyTeamData td = null;
                    if (!this.CopyTeamDict.TryGetValue(req.TeamId, out td))
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.TeamIsDestoryed;
                        return rsp;
                    }
                    if (td.StartTime > 0L)
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.TeamAlreadyStart;
                        return rsp;
                    }
                    if (td.LeaderRoleID != req.RoleId)
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.NotTeamLeader;
                        return rsp;
                    }
                    CopyTeamMemberData leader = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == td.LeaderRoleID);
                    if (leader == null || leader.RoleID != req.RoleId)
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.NotTeamLeader;
                        return rsp;
                    }
                    if (td.TeamRoles.Exists((CopyTeamMemberData _role) => !_role.IsReady))
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.MemeberNotReady;
                        return rsp;
                    }
                    int kfSrvId;
                    if (!ClientAgentManager.Instance().AssginKfFuben(GameTypes.KuaFuCopy, td.TeamID, td.TeamRoles.Count, out kfSrvId))
                    {
                        rsp.ErrorCode = CopyTeamErrorCodes.KFServerIsBusy;
                        return rsp;
                    }
                    td.StartTime = TimeUtil.NOW();
                    td.KFServerId = kfSrvId;
                    td.FuBenSeqID = 0;
                    CopyTeamStartData data = new CopyTeamStartData();
                    data.TeamId = req.TeamId;
                    data.StartMs = td.StartTime;
                    data.ToServerId = kfSrvId;
                    data.FuBenSeqId = td.FuBenSeqID;
                    this.AddAsyncEvent(new AsyncDataItem
                    {
                        EventType = KuaFuEventTypes.KFCopyTeamStart,
                        Args = new object[]
                        {
                            leader.ServerId,
                            data
                        }
                    });
                    this.TimeLimitCopy[td.TeamID] = td.StartTime + (long)req.LastMs + 180000L;
                    rsp.ErrorCode = CopyTeamErrorCodes.Success;
                    rsp.Data = data;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("开始跨服副本队伍异常, role={0}, teamid={1}", req.RoleId, req.TeamId), ex, true);
                rsp.ErrorCode = CopyTeamErrorCodes.CenterServerFailed;
            }
            return rsp;
        }

        
        private void ForceLeaveRoom(int roleId)
        {
            lock (this.Mutex)
            {
                long joinedTeam = -1L;
                if (this.RoleId2JoinedTeam.TryGetValue(roleId, out joinedTeam))
                {
                    this.LeaveTeam(new KFCopyTeamLeaveReq
                    {
                        ReqServerId = int.MaxValue,
                        RoleId = roleId,
                        TeamId = joinedTeam
                    });
                    this.RoleId2JoinedTeam.Remove(roleId);
                }
            }
        }

        
        private void AddAsyncEvent(AsyncDataItem evItem)
        {
            if (evItem != null)
            {
                lock (this.RoomEventQ)
                {
                    this.RoomEventQ.Enqueue(evItem);
                }
            }
        }

        
        public AsyncDataItem[] PopAsyncEvent()
        {
            AsyncDataItem[] itemArray = null;
            lock (this.RoomEventQ)
            {
                itemArray = this.RoomEventQ.ToArray();
                this.RoomEventQ.Clear();
            }
            return itemArray;
        }

        
        public void Update()
        {
            long nowMs = TimeUtil.NOW();
            lock (this.Mutex)
            {
                if (nowMs >= this.TimeLimitCopyLastCheckMs + 30000L)
                {
                    this.TimeLimitCopyLastCheckMs = nowMs;
                    List<KeyValuePair<long, long>> overTimeList = this.TimeLimitCopy.ToList<KeyValuePair<long, long>>();
                    overTimeList.Sort(delegate (KeyValuePair<long, long> _left, KeyValuePair<long, long> _right)
                    {
                        int result;
                        if (_left.Value - _right.Value < 0L)
                        {
                            result = -1;
                        }
                        else if (_left.Value - _right.Value > 0L)
                        {
                            result = 1;
                        }
                        else
                        {
                            result = 0;
                        }
                        return result;
                    });
                    for (int i = 0; i < overTimeList.Count; i++)
                    {
                        long teamId = overTimeList[i].Key;
                        long deadlineMs = overTimeList[i].Value;
                        if (deadlineMs > nowMs)
                        {
                            break;
                        }
                        this.RemoveTeam(teamId);
                        this.TimeLimitCopy.Remove(teamId);
                    }
                }
                if (nowMs >= this.TimeLimitMemberNoReadyMs + 1000L)
                {
                    foreach (CopyTeamData copyTeam in this.CopyTeamDict.Values)
                    {
                        if (copyTeam.StartTime <= 0L)
                        {
                            if (copyTeam.AutoKick > 0)
                            {
                                List<int> needDelList = new List<int>();
                                foreach (CopyTeamMemberData member in copyTeam.TeamRoles)
                                {
                                    if (!member.IsReady && nowMs > member.NoReadyTicks + 30000L)
                                    {
                                        needDelList.Add(member.RoleID);
                                    }
                                }
                                foreach (int roleid in needDelList)
                                {
                                    this.RemoveMember(copyTeam.TeamID, roleid);
                                }
                            }
                        }
                    }
                }
            }
        }

        
        public void RemoveTeam(long teamId)
        {
            lock (this.Mutex)
            {
                CopyTeamData td = null;
                if (this.CopyTeamDict.TryGetValue(teamId, out td))
                {
                    this.CopyTeamDict.Remove(teamId);
                    HashSet<long> teamList = null;
                    if (this.CopyId2Teams.TryGetValue(td.FuBenId, out teamList))
                    {
                        teamList.Remove(teamId);
                    }
                    this.TimeLimitCopy.Remove(td.TeamID);
                    if (td.KFServerId > 0)
                    {
                        this._KFCopyService.RemoveGameTeam(td.KFServerId, td.TeamID);
                    }
                    foreach (CopyTeamMemberData role in td.TeamRoles)
                    {
                        this.RoleId2JoinedTeam.Remove(role.RoleID);
                    }
                    CopyTeamDestroyData data = new CopyTeamDestroyData();
                    data.TeamId = teamId;
                    this.AddAsyncEvent(new AsyncDataItem
                    {
                        EventType = KuaFuEventTypes.KFCopyTeamDestroty,
                        Args = new object[]
                        {
                            data
                        }
                    });
                }
            }
        }

        
        public CopyTeamErrorCodes RemoveMember(long teamId, int roleid)
        {
            CopyTeamErrorCodes result;
            try
            {
                lock (this.Mutex)
                {
                    CopyTeamData td = null;
                    if (!this.CopyTeamDict.TryGetValue(teamId, out td))
                    {
                        result = CopyTeamErrorCodes.TeamIsDestoryed;
                    }
                    else if (td.StartTime > 0L)
                    {
                        result = CopyTeamErrorCodes.TeamAlreadyStart;
                    }
                    else
                    {
                        CopyTeamMemberData leader = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == td.LeaderRoleID);
                        if (leader == null)
                        {
                            result = CopyTeamErrorCodes.NotTeamLeader;
                        }
                        else
                        {
                            CopyTeamMemberData member = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == roleid);
                            if (member == null)
                            {
                                result = CopyTeamErrorCodes.NotInMyTeam;
                            }
                            else
                            {
                                td.TeamRoles.Remove(member);
                                td.MemberCount = td.TeamRoles.Count;
                                this.RoleId2JoinedTeam.Remove(roleid);
                                CopyTeamKickoutData data = new CopyTeamKickoutData();
                                data.FromRoleId = td.LeaderRoleID;
                                data.ToRoleId = roleid;
                                data.TeamId = teamId;
                                this.AddAsyncEvent(new AsyncDataItem
                                {
                                    EventType = KuaFuEventTypes.KFCopyTeamKickout,
                                    Args = new object[]
                                    {
                                        0,
                                        data
                                    }
                                });
                                result = CopyTeamErrorCodes.Success;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("系统踢出跨服副本队伍异常, role={0}, teamid={1}", roleid, teamId), ex, true);
                result = CopyTeamErrorCodes.CenterServerFailed;
            }
            return result;
        }

        
        public CopyTeamData GetTeamData(long teamid)
        {
            CopyTeamData result;
            lock (this.Mutex)
            {
                CopyTeamData td = null;
                if (!this.CopyTeamDict.TryGetValue(teamid, out td))
                {
                    result = null;
                }
                else
                {
                    result = td;
                }
            }
            return result;
        }

        
        public KFCopyTeamAnalysis BuildAnalysisData()
        {
            KFCopyTeamAnalysis data = new KFCopyTeamAnalysis();
            lock (this.Mutex)
            {
                foreach (KeyValuePair<long, CopyTeamData> kvp in this.CopyTeamDict)
                {
                    long teamId = kvp.Key;
                    CopyTeamData td = kvp.Value;
                    KFCopyTeamAnalysis.Item item = null;
                    if (!data.AnalysisDict.TryGetValue(td.FuBenId, out item))
                    {
                        item = new KFCopyTeamAnalysis.Item();
                        data.AnalysisDict[td.FuBenId] = item;
                    }
                    item.TotalCopyCount++;
                    item.TotalRoleCount += td.TeamRoles.Count;
                    if (td.StartTime > 0L)
                    {
                        item.StartCopyCount++;
                        item.StartRoleCount += td.TeamRoles.Count;
                    }
                    else
                    {
                        item.UnStartCopyCount++;
                        item.UnStartRoleCount += td.TeamRoles.Count;
                    }
                }
            }
            return data;
        }

        
        private object Mutex = new object();

        
        private Dictionary<int, long> RoleId2JoinedTeam = new Dictionary<int, long>();

        
        private Dictionary<long, CopyTeamData> CopyTeamDict = new Dictionary<long, CopyTeamData>();

        
        private Dictionary<int, HashSet<long>> CopyId2Teams = new Dictionary<int, HashSet<long>>();

        
        private long TimeLimitCopyLastCheckMs = 0L;

        
        private Dictionary<long, long> TimeLimitCopy = new Dictionary<long, long>();

        
        private Queue<AsyncDataItem> RoomEventQ = new Queue<AsyncDataItem>();

        
        private KuaFuCopyService _KFCopyService = null;

        
        public long TimeLimitMemberNoReadyMs = 0L;
    }
}
