using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic.Copy
{
    
    public class CopyTeamManager : SingletonTemplate<CopyTeamManager>, IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, ICopySceneManager
    {
        
        public bool IsKuaFuCopy(int copyId)
        {
            SystemXmlItem systemFuBenItem = null;
            bool result;
            if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyId, out systemFuBenItem))
            {
                result = false;
            }
            else
            {
                int nMapCode = systemFuBenItem.GetIntValue("MapCode", -1);
                SceneUIClasses sceneType = Global.GetMapSceneType(nMapCode);
                SceneUIClasses sceneUIClasses = sceneType;
                if (sceneUIClasses <= SceneUIClasses.MoRiJudge)
                {
                    if (sceneUIClasses != SceneUIClasses.KaLiMaTemple)
                    {
                        switch (sceneUIClasses)
                        {
                            case SceneUIClasses.ElementWar:
                            case SceneUIClasses.MoRiJudge:
                                break;
                            default:
                                goto IL_72;
                        }
                    }
                }
                else if (sceneUIClasses != SceneUIClasses.CopyWolf && sceneUIClasses != SceneUIClasses.WanMoXiaGu && sceneUIClasses != SceneUIClasses.KFXuHuanFuBen)
                {
                    goto IL_72;
                }
                return true;
                IL_72:
                result = false;
            }
            return result;
        }

        
        public bool HandleKuaFuLogin(KuaFuServerLoginData data)
        {
            bool result;
            if (data == null)
            {
                result = false;
            }
            else
            {
                lock (this.Mutex)
                {
                    CopyTeamData td = null;
                    if (!this.CopyTeamDict.TryGetValue(data.GameId, out td) || td.StartTime <= 0L)
                    {
                        td = KFCopyRpcClient.getInstance().GetTeamData(data.GameId);
                        if (td == null)
                        {
                            return false;
                        }
                        this.CopyTeamDict[td.TeamID] = td;
                        HashSet<long> teamList = null;
                        if (this.FuBenId2Teams.TryGetValue(td.FuBenId, out teamList) && !teamList.Contains(td.TeamID))
                        {
                            teamList.Add(td.TeamID);
                        }
                    }
                    if (td == null)
                    {
                        result = false;
                    }
                    else if (td.KFServerId != this.ThisServerId)
                    {
                        result = false;
                    }
                    else if (td.StartTime <= 0L)
                    {
                        result = false;
                    }
                    else if (!td.TeamRoles.Exists((CopyTeamMemberData _role) => _role.RoleID == data.RoleId))
                    {
                        result = false;
                    }
                    else
                    {
                        if (td.FuBenSeqID <= 0)
                        {
                            td.FuBenSeqID = GameCoreInterface.getinstance().GetNewFuBenSeqId();
                        }
                        data.FuBenSeqId = td.FuBenSeqID;
                        this.FuBenSeq2TeamId[td.FuBenSeqID] = td.TeamID;
                        result = true;
                    }
                }
            }
            return result;
        }

        
        public bool HandleKuaFuInitGame(GameClient client)
        {
            bool result;
            if (client == null)
            {
                result = false;
            }
            else
            {
                lock (this.Mutex)
                {
                    CopyTeamData td = null;
                    if (!this.CopyTeamDict.TryGetValue(client.ClientSocket.ClientKuaFuServerLoginData.GameId, out td))
                    {
                        result = false;
                    }
                    else
                    {
                        SystemXmlItem systemFuBenItem = null;
                        if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(td.FuBenId, out systemFuBenItem))
                        {
                            result = false;
                        }
                        else
                        {
                            int mapCode = systemFuBenItem.GetIntValue("MapCode", -1);
                            int destX;
                            int destY;
                            if (!this.GetBirthPoint(mapCode, out destX, out destY))
                            {
                                LogManager.WriteLog(LogTypes.Error, string.Format("rolename={0} 跨服登录副本copyid={1}, 找不到出生点", client.ClientData.RoleName, td.FuBenId), null, true);
                                result = false;
                            }
                            else
                            {
                                client.ClientData.MapCode = mapCode;
                                client.ClientData.PosX = destX;
                                client.ClientData.PosY = destY;
                                client.ClientData.FuBenSeqID = client.ClientSocket.ClientKuaFuServerLoginData.FuBenSeqId;
                                this.RoleId2JoinedTeam[client.ClientData.RoleID] = td.TeamID;
                                result = true;
                            }
                        }
                    }
                }
            }
            return result;
        }

        
        private bool GetBirthPoint(int mapCode, out int toPosX, out int toPosY)
        {
            toPosX = -1;
            toPosY = -1;
            GameMap gameMap = null;
            bool result;
            if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
            {
                result = false;
            }
            else
            {
                int defaultBirthPosX = gameMap.DefaultBirthPosX;
                int defaultBirthPosY = gameMap.DefaultBirthPosY;
                int defaultBirthRadius = gameMap.BirthRadius;
                Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, mapCode, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
                toPosX = (int)newPos.X;
                toPosY = (int)newPos.Y;
                result = true;
            }
            return result;
        }

        
        public void OnCopyRemove(int FuBenSeqId)
        {
            long teamId = -1L;
            lock (this.Mutex)
            {
                if (this.FuBenSeq2TeamId.TryGetValue(FuBenSeqId, out teamId))
                {
                    this.FuBenSeq2TeamId.Remove(FuBenSeqId);
                    CopyTeamData td;
                    if (this.CopyTeamDict.TryGetValue(teamId, out td))
                    {
                        this.OnTeamDestroy(new CopyTeamDestroyData
                        {
                            TeamId = teamId
                        });
                        if (this.IsKuaFuCopy(td.FuBenId))
                        {
                            KFCopyRpcClient.getInstance().KFCopyTeamRemove(teamId);
                        }
                    }
                }
            }
        }

        
        private void OnTeamCreate(CopyTeamCreateData data)
        {
            if (data != null)
            {
                bool isKuaFuCopy = this.IsKuaFuCopy(data.CopyId);
                CopyTeamData td = new CopyTeamData();
                td.TeamID = data.TeamId;
                td.LeaderRoleID = data.Member.RoleID;
                td.RoleSex = data.Member.RoleSex;
                td.Occupation = data.Member.Occupation;
                td.FuBenId = data.CopyId;
                td.MinZhanLi = data.MinCombat;
                td.AutoStart = (data.AutoStart > 0);
                td.TeamRoles.Add(data.Member);
                td.TeamRoles[0].IsReady = true;
                td.TeamName = td.TeamRoles[0].RoleName;
                td.MemberCount = td.TeamRoles.Count;
                td.AutoKick = data.AutoKick;
                lock (this.Mutex)
                {
                    this.CopyTeamDict[td.TeamID] = td;
                    HashSet<long> teams = null;
                    if (this.FuBenId2Teams.TryGetValue(td.FuBenId, out teams) && !teams.Contains(td.TeamID))
                    {
                        teams.Add(td.TeamID);
                    }
                    if (data.Member.ServerId == this.ThisServerId)
                    {
                        this.RoleId2JoinedTeam[data.Member.RoleID] = td.TeamID;
                        GameClient client = GameManager.ClientMgr.FindClient(data.Member.RoleID);
                        if (client != null)
                        {
                            this.NotifyTeamCmd(client, CopyTeamErrorCodes.Success, 1, td.TeamID, td.TeamName, -1, -1, -1);
                        }
                    }
                    this.NotifyTeamData(td);
                    this.NotifyTeamListChange(td);
                }
            }
        }

        
        private void OnTeamJoin(CopyTeamJoinData data)
        {
            if (data != null)
            {
                lock (this.Mutex)
                {
                    CopyTeamData td;
                    if (this.CopyTeamDict.TryGetValue(data.TeamId, out td))
                    {
                        if (td.TeamRoles.Count < ConstData.CopyRoleMax(td.FuBenId))
                        {
                            td.TeamRoles.Add(data.Member);
                            td.MemberCount = td.TeamRoles.Count<CopyTeamMemberData>();
                            if (data.Member.ServerId == this.ThisServerId)
                            {
                                this.RoleId2JoinedTeam[data.Member.RoleID] = td.TeamID;
                                GameClient client = GameManager.ClientMgr.FindClient(data.Member.RoleID);
                                if (client != null)
                                {
                                    this.NotifyTeamCmd(client, CopyTeamErrorCodes.Success, 4, td.TeamID, td.TeamName, -1, -1, -1);
                                }
                            }
                            this.NotifyTeamData(td);
                            this.NotifyTeamListChange(td);
                        }
                    }
                }
            }
        }

        
        private void OnTeamKickout(CopyTeamKickoutData data)
        {
            if (data != null)
            {
                lock (this.Mutex)
                {
                    CopyTeamData td = null;
                    if (this.CopyTeamDict.TryGetValue(data.TeamId, out td))
                    {
                        CopyTeamMemberData member = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == data.ToRoleId);
                        if (member != null)
                        {
                            td.TeamRoles.Remove(member);
                            td.MemberCount = td.TeamRoles.Count;
                            if (member.ServerId == this.ThisServerId)
                            {
                                this.RoleId2JoinedTeam.Remove(member.RoleID);
                                GameClient client = GameManager.ClientMgr.FindClient(member.RoleID);
                                if (client != null)
                                {
                                    this.NotifyTeamStateChanged(client, -12L, member.RoleID, 0);
                                }
                            }
                            this.NotifyTeamData(td);
                            this.NotifyTeamListChange(td);
                        }
                    }
                }
            }
        }

        
        private void OnTeamLeave(CopyTeamLeaveData data)
        {
            if (data != null)
            {
                lock (this.Mutex)
                {
                    CopyTeamData td = null;
                    if (this.CopyTeamDict.TryGetValue(data.TeamId, out td))
                    {
                        CopyTeamMemberData member = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == data.RoleId);
                        if (member != null)
                        {
                            td.TeamRoles.Remove(member);
                            td.MemberCount = td.TeamRoles.Count;
                            if (td.MemberCount <= 0)
                            {
                                td.LeaderRoleID = -1;
                                this.OnTeamDestroy(new CopyTeamDestroyData
                                {
                                    TeamId = td.TeamID
                                });
                            }
                            else if (td.LeaderRoleID == member.RoleID)
                            {
                                td.LeaderRoleID = td.TeamRoles[0].RoleID;
                                td.TeamRoles[0].IsReady = true;
                                td.TeamName = td.TeamRoles[0].RoleName;
                            }
                            if (member.ServerId == this.ThisServerId)
                            {
                                this.RoleId2JoinedTeam.Remove(member.RoleID);
                                GameClient client = GameManager.ClientMgr.FindClient(member.RoleID);
                                if (client != null)
                                {
                                    this.NotifyTeamStateChanged(client, -11L, member.RoleID, 0);
                                }
                            }
                            this.NotifyTeamData(td);
                            this.NotifyTeamListChange(td);
                        }
                    }
                }
            }
        }

        
        private void OnTeamSetReady(CopyTeamReadyData data)
        {
            if (data != null)
            {
                lock (this.Mutex)
                {
                    CopyTeamData td = null;
                    if (this.CopyTeamDict.TryGetValue(data.TeamId, out td))
                    {
                        CopyTeamMemberData member = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == data.RoleId);
                        if (member != null)
                        {
                            member.IsReady = (data.Ready > 0);
                            if (!member.IsReady)
                            {
                                member.NoReadyTicks = TimeUtil.NOW();
                            }
                            if (member.ServerId == this.ThisServerId)
                            {
                                GameClient client = GameManager.ClientMgr.FindClient(member.RoleID);
                                if (client != null)
                                {
                                    this.NotifyTeamStateChanged(client, td.TeamID, member.RoleID, data.Ready);
                                }
                            }
                            this.NotifyTeamData(td);
                            bool flag2;
                            if (member.IsReady && td.AutoStart && td.MemberCount >= ConstData.CopyRoleMax(td.FuBenId))
                            {
                                flag2 = !td.TeamRoles.All((CopyTeamMemberData _role) => _role.IsReady);
                            }
                            else
                            {
                                flag2 = true;
                            }
                            if (!flag2)
                            {
                                CopyTeamMemberData leader = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == td.LeaderRoleID);
                                if (leader != null && leader.ServerId == this.ThisServerId)
                                {
                                    GameClient client = GameManager.ClientMgr.FindClient(leader.RoleID);
                                    if (client != null)
                                    {
                                        this.NotifyTeamCmd(client, CopyTeamErrorCodes.Success, 14, 0L, "", -1, -1, -1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        
        private void OnTeamSetFlag(CopyTeamFlagData data)
        {
            if (data != null)
            {
                lock (this.Mutex)
                {
                    CopyTeamData td = null;
                    if (this.CopyTeamDict.TryGetValue(data.TeamId, out td))
                    {
                        if ((td.AutoStart ? 1 : 0) != data.AutoStart)
                        {
                            td.AutoStart = (data.AutoStart > 0);
                            this.NotifyTeamCmds(td, CopyTeamErrorCodes.Success, 16, td.AutoStart ? 1L : 0L, "");
                        }
                        if (td.AutoKick != data.AutoKick)
                        {
                            td.AutoKick = data.AutoKick;
                            this.NotifyTeamCmds(td, CopyTeamErrorCodes.Success, 15, (long)td.AutoKick, "");
                        }
                    }
                }
            }
        }

        
        private void OnTeamDestroy(CopyTeamDestroyData data)
        {
            if (data != null)
            {
                lock (this.Mutex)
                {
                    CopyTeamData td = null;
                    if (this.CopyTeamDict.TryGetValue(data.TeamId, out td))
                    {
                        this.CopyTeamDict.Remove(data.TeamId);
                        this.FuBenSeq2TeamId.Remove(td.FuBenSeqID);
                        HashSet<long> teamList = null;
                        if (this.FuBenId2Teams.TryGetValue(td.FuBenId, out teamList))
                        {
                            teamList.Remove(td.TeamID);
                        }
                        foreach (CopyTeamMemberData member in td.TeamRoles)
                        {
                            this.RoleId2JoinedTeam.Remove(member.RoleID);
                            if (member.ServerId == this.ThisServerId)
                            {
                                GameClient client = GameManager.ClientMgr.FindClient(member.RoleID);
                                if (client != null)
                                {
                                    this.NotifyTeamStateChanged(client, -11L, member.RoleID, 0);
                                }
                            }
                        }
                        td.LeaderRoleID = -1;
                        this.NotifyTeamData(td);
                        td.TeamRoles.Clear();
                        td.MemberCount = td.TeamRoles.Count;
                        this.NotifyTeamListChange(td);
                    }
                }
            }
        }

        
        private void OnTeamStart(CopyTeamStartData data)
        {
            if (data != null)
            {
                lock (this.Mutex)
                {
                    CopyTeamData td = null;
                    if (this.CopyTeamDict.TryGetValue(data.TeamId, out td))
                    {
                        td.StartTime = data.StartMs;
                        td.KFServerId = data.ToServerId;
                        td.FuBenSeqID = data.FuBenSeqId;
                        bool isKuaFuCopy = this.IsKuaFuCopy(td.FuBenId);
                        string toServerIp = string.Empty;
                        int toServerPort = 0;
                        if (isKuaFuCopy)
                        {
                            if (!KFCopyRpcClient.getInstance().GetKuaFuGSInfo(data.ToServerId, out toServerIp, out toServerPort))
                            {
                                LogManager.WriteLog(LogTypes.Error, string.Format("跨服副本CopyType={0}, RoomId={1}被分配到服务器ServerId={2}, 但是找不到该跨服活动服务器", td.FuBenId, data.TeamId, data.ToServerId), null, true);
                                return;
                            }
                        }
                        else
                        {
                            this.FuBenSeq2TeamId[td.FuBenSeqID] = td.TeamID;
                        }
                        foreach (CopyTeamMemberData member in td.TeamRoles)
                        {
                            if (member.ServerId == this.ThisServerId)
                            {
                                GameClient client = GameManager.ClientMgr.FindClient(member.RoleID);
                                if (client != null)
                                {
                                    if (isKuaFuCopy)
                                    {
                                        client.ClientSocket.ClientKuaFuServerLoginData.RoleId = member.RoleID;
                                        client.ClientSocket.ClientKuaFuServerLoginData.GameId = td.TeamID;
                                        client.ClientSocket.ClientKuaFuServerLoginData.GameType = 8;
                                        client.ClientSocket.ClientKuaFuServerLoginData.EndTicks = 0L;
                                        client.ClientSocket.ClientKuaFuServerLoginData.ServerId = this.ThisServerId;
                                        client.ClientSocket.ClientKuaFuServerLoginData.ServerIp = toServerIp;
                                        client.ClientSocket.ClientKuaFuServerLoginData.ServerPort = toServerPort;
                                        client.ClientSocket.ClientKuaFuServerLoginData.FuBenSeqId = data.FuBenSeqId;
                                    }
                                    GameManager.ClientMgr.NotifyTeamMemberFuBenEnterMsg(client, td.LeaderRoleID, td.FuBenId, td.FuBenSeqID);
                                }
                            }
                        }
                        this.NotifyTeamListChange(td);
                    }
                }
            }
        }

        
        private CopyTeamManager()
        {
        }

        
        public bool initialize()
        {
            TCPCmdDispatcher.getInstance().registerProcessor(621, 6, SingletonTemplate<CopyTeamManager>.Instance());
            TCPCmdDispatcher.getInstance().registerProcessor(624, 4, SingletonTemplate<CopyTeamManager>.Instance());
            TCPCmdDispatcher.getInstance().registerProcessor(620, 4, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource.getInstance().registerListener(13, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource.getInstance().registerListener(12, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource.getInstance().registerListener(14, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().registerListener(10006, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().registerListener(10007, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().registerListener(10008, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().registerListener(10009, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().registerListener(10010, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().registerListener(10011, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().registerListener(10012, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().registerListener(10013, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            this.ThisServerId = GameManager.ServerId;
            foreach (SystemXmlItem systemFuBenItem in GameManager.systemFuBenMgr.SystemXmlItemDict.Values)
            {
                int copyType = systemFuBenItem.GetIntValue("CopyType", -1);
                int FubenID = systemFuBenItem.GetIntValue("ID", -1);
                if (1 == copyType)
                {
                    this.FuBenId2Watchers.Add(FubenID, new HashSet<int>());
                    this.FuBenId2Teams.Add(FubenID, new HashSet<long>());
                }
                int RecordDamage = systemFuBenItem.GetIntValue("RecordDamage", -1);
                if (1 == RecordDamage)
                {
                    this.RecordDamagesFuBenIDHashSet.Add(FubenID);
                }
            }
            List<FuBenMapItem> fubenMapItemList = FuBenManager.GetAllFubenMapItem();
            foreach (FuBenMapItem fubenMapItem in fubenMapItemList)
            {
                int copyType = Global.GetFuBenCopyType(fubenMapItem.FuBenID);
                if (1 == copyType)
                {
                    this.MapCode2ToFubenId.Add(fubenMapItem.MapCode, fubenMapItem.FuBenID);
                    if (!this.FuBenId2MapCodes.ContainsKey(fubenMapItem.FuBenID))
                    {
                        this.FuBenId2MapCodes.Add(fubenMapItem.FuBenID, new List<int>());
                    }
                    this.FuBenId2MapCodes[fubenMapItem.FuBenID].Add(fubenMapItem.MapCode);
                }
            }
            SingletonTemplate<UniqueTeamId>.Instance().Init();
            return true;
        }

        
        public bool startup()
        {
            return true;
        }

        
        public bool showdown()
        {
            return true;
        }

        
        public bool destroy()
        {
            GlobalEventSource.getInstance().removeListener(13, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource.getInstance().removeListener(12, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource.getInstance().removeListener(14, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().removeListener(10006, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().removeListener(10007, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().removeListener(10008, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().removeListener(10009, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().removeListener(10010, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().removeListener(10011, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().removeListener(10012, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            GlobalEventSource4Scene.getInstance().removeListener(10013, 10001, SingletonTemplate<CopyTeamManager>.Instance());
            return true;
        }

        
        public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            switch (nID)
            {
                case 620:
                    return this.HandleNetCmd_GetRoomList(client, nID, bytes, cmdParams);
                case 621:
                    return this.HandleNetCmd_CopyTeam(client, nID, bytes, cmdParams);
                case 624:
                    return this.HandleNetCmd_RegRoomNotify(client, nID, bytes, cmdParams);
            }
            return true;
        }

        
        public bool processCmd(GameClient client, string[] cmdParams)
        {
            return true;
        }

        
        public void processEvent(EventObject eventObject)
        {
            if (eventObject.getEventType() == 13)
            {
                PlayerLeaveFuBenEventObject eventObj = (PlayerLeaveFuBenEventObject)eventObject;
                this.RoleLeaveFuBen(eventObj.getPlayer());
            }
            else if (eventObject.getEventType() == 14)
            {
                PlayerInitGameEventObject eventObj2 = (PlayerInitGameEventObject)eventObject;
                this.OnPlayerLogin(eventObj2.getPlayer());
            }
            else if (eventObject.getEventType() == 12)
            {
                PlayerLogoutEventObject eventObj3 = (PlayerLogoutEventObject)eventObject;
                this.OnPlayerLogout(eventObj3.getPlayer());
            }
        }

        
        public void processEvent(EventObjectEx eventObject)
        {
            switch (eventObject.EventType)
            {
                case 10006:
                    this.OnTeamCreate((eventObject as KFCopyRoomCreateEvent).Data);
                    break;
                case 10007:
                    this.OnTeamJoin((eventObject as KFCopyRoomJoinEvent).Data);
                    break;
                case 10008:
                    this.OnTeamSetReady((eventObject as KFCopyRoomReadyEvent).Data);
                    break;
                case 10009:
                    this.OnTeamKickout((eventObject as KFCopyRoomKickoutEvent).Data);
                    break;
                case 10010:
                    this.OnTeamLeave((eventObject as KFCopyRoomLeaveEvent).Data);
                    break;
                case 10011:
                    this.OnTeamDestroy((eventObject as KFCopyTeamDestroyEvent).Data);
                    break;
                case 10012:
                    this.OnTeamStart((eventObject as KFCopyRoomStartEvent).Data);
                    break;
                case 10013:
                    this.OnTeamSetFlag((eventObject as KFCopyRoomSetFlagEvent).Data);
                    break;
            }
            eventObject.Handled = true;
        }

        
        private bool HandleNetCmd_CopyTeam(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            int teamType = Convert.ToInt32(cmdParams[1]);
            if (teamType == 1)
            {
                int copyId = Convert.ToInt32(cmdParams[2]);
                int minCombat = Convert.ToInt32(cmdParams[3]);
                int autoStart = Convert.ToInt32(cmdParams[4]);
                int kickNoReady = Convert.ToInt32(cmdParams[5]);
                this.HandleCreateCopyTeam(client, copyId, minCombat, autoStart, kickNoReady);
            }
            else if (teamType == 4)
            {
                long teamId = Convert.ToInt64(cmdParams[2]);
                this.HandleApplyCopyTeam(client, teamId);
            }
            else if (teamType == 8)
            {
                int otherRoleId = Convert.ToInt32(cmdParams[2]);
                this.HandleKickoutCopyTeam(client, otherRoleId);
            }
            else if (teamType == 9)
            {
                this.HandleQuitFromTeam(client, true);
            }
            else if (teamType == 12)
            {
                int ready = Convert.ToInt32(cmdParams[2]);
                this.HandleSetReady(client, ready);
            }
            else if (teamType == 13)
            {
                int copyId = Convert.ToInt32(cmdParams[2]);
                this.HandleQuickJoinTeam(client, copyId);
            }
            else if (teamType == 15)
            {
                int flag = Convert.ToInt32(cmdParams[5]);
                this.HandleModKickFlag(client, flag);
            }
            else if (teamType == 16)
            {
                int flag = Convert.ToInt32(cmdParams[4]);
                this.HandleModAutoStart(client, flag);
            }
            return true;
        }

        
        private bool HandleNetCmd_RegRoomNotify(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            int copyId = Convert.ToInt32(cmdParams[1]);
            int ready = Convert.ToInt32(cmdParams[2]);
            if (ready > 0)
            {
                this.RegisterCopyTeamListNotify(client, copyId);
            }
            else
            {
                this.UnRegisterCopyTeamListNotify(client);
            }
            return true;
        }

        
        private bool HandleNetCmd_GetRoomList(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            throw new NotImplementedException();
        }

        
        private void HandleCreateCopyTeam(GameClient client, int copyId, int minCombat, int autoStart, int kickNoReady)
        {
            if (!client.ClientSocket.IsKuaFuLogin)
            {
                SystemXmlItem copyItem = null;
                if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyId, out copyItem))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("HandleCreateCopyTeam Faild copyId={0}", copyId), null, true);
                }
                else if (!FuBenChecker.HasFinishedPreTask(client, copyItem))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("HandleCreateCopyTeam Faild !HasFinishedPreTask copyId={0}", copyId), null, true);
                }
                else if (!FuBenChecker.HasPassedPreCopy(client, copyItem))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("HandleCreateCopyTeam Faild !HasPassedPreCopy copyId={0}", copyId), null, true);
                }
                else if (!FuBenChecker.IsInCopyLevelLimit(client, copyItem))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("HandleCreateCopyTeam Faild !IsInCopyLevelLimit copyId={0}", copyId), null, true);
                }
                else if (!FuBenChecker.IsInCopyTimesLimit(client, copyItem))
                {
                    this.NotifyTeamCmd(client, CopyTeamErrorCodes.TimesNotEnough, 1, 0L, "", -1, -1, -1);
                }
                else
                {
                    lock (this.Mutex)
                    {
                        long oldTeamId;
                        if (this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out oldTeamId))
                        {
                            this.NotifyTeamCmd(client, CopyTeamErrorCodes.AllreadyHasTeam, 1, 0L, "", -1, -1, -1);
                            return;
                        }
                    }
                    if (this.FuBenId2Watchers.ContainsKey(copyId))
                    {
                        if (!this.IsKuaFuCopy(copyId))
                        {
                            this.OnTeamCreate(new CopyTeamCreateData
                            {
                                Member = this.ClientDataToTeamMemberData(client.ClientData),
                                MinCombat = minCombat,
                                TeamId = SingletonTemplate<UniqueTeamId>.Instance().Create(),
                                CopyId = copyId,
                                AutoStart = autoStart,
                                AutoKick = kickNoReady
                            });
                            this.HandleSetReady(client, 1);
                        }
                        else
                        {
                            KFCopyTeamCreateReq req = new KFCopyTeamCreateReq();
                            req.Member = this.ClientDataToTeamMemberData(client.ClientData);
                            req.Member.RoleName = Global.FormatNameWithZoneId(req.Member.ZoneId, req.Member.RoleName);
                            req.CopyId = copyId;
                            req.MinCombat = minCombat;
                            req.AutoStart = autoStart;
                            req.AutoKick = kickNoReady;
                            req.TeamId = SingletonTemplate<UniqueTeamId>.Instance().Create();
                            KFCopyTeamCreateRsp rsp = KFCopyRpcClient.getInstance().CreateTeam(req);
                            if (rsp == null)
                            {
                                LogManager.WriteLog(LogTypes.Error, string.Format("KF 创建队伍RPC调用失败 roleid={0}, rolename={1}, copyid={2}", client.ClientData.RoleID, client.ClientData.RoleName, copyId), null, true);
                                this.NotifyTeamCmd(client, CopyTeamErrorCodes.ServerException, 1, 0L, "", -1, -1, -1);
                            }
                            else if (rsp.ErrorCode == CopyTeamErrorCodes.Success)
                            {
                                this.OnTeamCreate(rsp.Data);
                            }
                            else
                            {
                                this.NotifyTeamCmd(client, rsp.ErrorCode, 1, 0L, "", -1, -1, -1);
                                LogManager.WriteLog(LogTypes.Error, string.Format("KF 创建队伍失败 roleid={0}, rolename={1}, copyid={2}, errorcode={3}", new object[]
                                {
                                    client.ClientData.RoleID,
                                    client.ClientData.RoleName,
                                    copyId,
                                    rsp.ErrorCode
                                }), null, true);
                            }
                        }
                    }
                }
            }
        }

        
        public void HandleApplyCopyTeam(GameClient client, long teamId)
        {
            if (!client.ClientSocket.IsKuaFuLogin)
            {
                lock (this.Mutex)
                {
                    long oldTeamId;
                    if (this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out oldTeamId))
                    {
                        this.NotifyTeamCmd(client, CopyTeamErrorCodes.AllreadyHasTeam, 4, 0L, "", -1, -1, -1);
                    }
                    else
                    {
                        CopyTeamData td = null;
                        if (!this.CopyTeamDict.TryGetValue(teamId, out td))
                        {
                            this.NotifyListTeamRemove(client, teamId, -1);
                        }
                        else
                        {
                            SystemXmlItem copyItem = null;
                            if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(td.FuBenId, out copyItem))
                            {
                                LogManager.WriteLog(LogTypes.Error, string.Format("HandleApplyCopyTeam Faild copyId={0}", td.FuBenId), null, true);
                            }
                            else if (!FuBenChecker.HasFinishedPreTask(client, copyItem))
                            {
                                LogManager.WriteLog(LogTypes.Error, string.Format("HandleApplyCopyTeam Faild !HasFinishedPreTask copyId={0}", td.FuBenId), null, true);
                            }
                            else if (!FuBenChecker.HasPassedPreCopy(client, copyItem))
                            {
                                LogManager.WriteLog(LogTypes.Error, string.Format("HandleApplyCopyTeam Faild !HasPassedPreCopy copyId={0}", td.FuBenId), null, true);
                            }
                            else if (!FuBenChecker.IsInCopyLevelLimit(client, copyItem))
                            {
                                LogManager.WriteLog(LogTypes.Error, string.Format("HandleApplyCopyTeam Faild !IsInCopyLevelLimit copyId={0}", td.FuBenId), null, true);
                            }
                            else if (!FuBenChecker.IsInCopyTimesLimit(client, copyItem))
                            {
                                this.NotifyTeamCmd(client, CopyTeamErrorCodes.TimesNotEnough, 4, 0L, "", -1, -1, -1);
                            }
                            else if (td.StartTime > 0L)
                            {
                                this.NotifyTeamCmd(client, CopyTeamErrorCodes.TeamAlreadyStart, 4, 0L, "", -1, -1, -1);
                            }
                            else if (td.TeamRoles.Count >= ConstData.CopyRoleMax(td.FuBenId))
                            {
                                this.NotifyTeamCmd(client, CopyTeamErrorCodes.TeamIsFull, 4, 0L, "", -1, -1, -1);
                            }
                            else if (client.ClientData.CombatForce < td.MinZhanLi)
                            {
                                this.NotifyTeamCmd(client, CopyTeamErrorCodes.ZhanLiLow, 4, 0L, "", -1, -1, -1);
                            }
                            else if (!this.IsKuaFuCopy(td.FuBenId))
                            {
                                var member = this.ClientDataToTeamMemberData(client.ClientData);
                                member.NoReadyTicks = TimeUtil.NOW();
                                this.OnTeamJoin(new CopyTeamJoinData
                                {
                                    Member = member,
                                    TeamId = teamId,
                                });
                            }
                            else
                            {
                                KFCopyTeamJoinReq req = new KFCopyTeamJoinReq();
                                req.Member = this.ClientDataToTeamMemberData(client.ClientData);
                                req.Member.RoleName = Global.FormatNameWithZoneId(req.Member.ZoneId, req.Member.RoleName);
                                req.CopyId = td.FuBenId;
                                req.TeamId = td.TeamID;
                                KFCopyTeamJoinRsp rsp = KFCopyRpcClient.getInstance().JoinTeam(req);
                                if (rsp == null)
                                {
                                    LogManager.WriteLog(LogTypes.Error, string.Format("KF 加入队伍RPC调用失败 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, teamId), null, true);
                                    this.NotifyTeamCmd(client, CopyTeamErrorCodes.ServerException, 4, 0L, "", -1, -1, -1);
                                }
                                else if (rsp.ErrorCode == CopyTeamErrorCodes.Success)
                                {
                                    this.OnTeamJoin(rsp.Data);
                                }
                                else
                                {
                                    LogManager.WriteLog(LogTypes.Error, string.Format("KF 加入队伍失败 roleid={0}, rolename={1}, teamid={2}, errorcode={3}", new object[]
                                    {
                                        client.ClientData.RoleID,
                                        client.ClientData.RoleName,
                                        teamId,
                                        rsp.ErrorCode
                                    }), null, true);
                                    this.NotifyTeamCmd(client, rsp.ErrorCode, 4, 0L, "", -1, -1, -1);
                                    if (rsp.ErrorCode == CopyTeamErrorCodes.TeamIsDestoryed)
                                    {
                                        LogManager.WriteLog(LogTypes.Error, string.Format("KF 加入队伍, 队伍在中心已销毁 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, teamId), null, true);
                                        this.OnTeamDestroy(new CopyTeamDestroyData
                                        {
                                            TeamId = req.TeamId
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        
        public void HandleKickoutCopyTeam(GameClient client, int otherRoleId)
        {
            if (!client.ClientSocket.IsKuaFuLogin)
            {
                lock (this.Mutex)
                {
                    long teamId;
                    CopyTeamData td;
                    if (!this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out teamId))
                    {
                        this.NotifyTeamCmd(client, CopyTeamErrorCodes.NoTeam, 8, 0L, "", -1, -1, -1);
                    }
                    else if (!this.CopyTeamDict.TryGetValue(teamId, out td))
                    {
                        this.RoleId2JoinedTeam.Remove(client.ClientData.RoleID);
                        this.NotifyTeamCmd(client, CopyTeamErrorCodes.TeamIsDestoryed, 8, 0L, "", -1, -1, -1);
                    }
                    else if (td.LeaderRoleID != client.ClientData.RoleID || client.ClientData.RoleID == otherRoleId)
                    {
                        this.NotifyTeamCmd(client, CopyTeamErrorCodes.NotTeamLeader, 8, 0L, "", -1, -1, -1);
                    }
                    else if (!this.IsKuaFuCopy(td.FuBenId))
                    {
                        this.OnTeamKickout(new CopyTeamKickoutData
                        {
                            FromRoleId = client.ClientData.RoleID,
                            ToRoleId = otherRoleId,
                            TeamId = td.TeamID
                        });
                    }
                    else
                    {
                        KFCopyTeamKickoutReq req = new KFCopyTeamKickoutReq();
                        req.FromRoleId = client.ClientData.RoleID;
                        req.ToRoleId = otherRoleId;
                        req.TeamId = td.TeamID;
                        KFCopyTeamKickoutRsp rsp = KFCopyRpcClient.getInstance().KickoutTeam(req);
                        if (rsp == null)
                        {
                            LogManager.WriteLog(LogTypes.Error, string.Format("KF 队伍踢人RPC调用失败 roleid={0}, rolename={1}, otherrid={2}", client.ClientData.RoleID, client.ClientData.RoleName, otherRoleId), null, true);
                            this.NotifyTeamCmd(client, CopyTeamErrorCodes.ServerException, 8, 0L, "", -1, -1, -1);
                        }
                        else if (rsp.ErrorCode == CopyTeamErrorCodes.Success)
                        {
                            this.OnTeamKickout(rsp.Data);
                        }
                        else
                        {
                            this.NotifyTeamCmd(client, rsp.ErrorCode, 8, 0L, "", -1, -1, -1);
                            LogManager.WriteLog(LogTypes.Error, string.Format("KF 队伍踢人失败 roleid={0}, rolename={1}, teamid={2}, errorcode={3}", new object[]
                            {
                                client.ClientData.RoleID,
                                client.ClientData.RoleName,
                                req.TeamId,
                                rsp.ErrorCode
                            }), null, true);
                            if (rsp.ErrorCode == CopyTeamErrorCodes.TeamIsDestoryed)
                            {
                                LogManager.WriteLog(LogTypes.Error, string.Format("KF 队伍踢人, 队伍在中心已销毁 roleid={0}, rolename={1}, otherrid={2}", client.ClientData.RoleID, client.ClientData.RoleName, otherRoleId), null, true);
                                this.OnTeamDestroy(new CopyTeamDestroyData
                                {
                                    TeamId = req.TeamId
                                });
                            }
                        }
                    }
                }
            }
        }

        
        public void HandleQuitFromTeam(GameClient client, bool notifyOther = true)
        {
            lock (this.Mutex)
            {
                long teamId;
                if (this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out teamId))
                {
                    CopyTeamData td;
                    if (!this.CopyTeamDict.TryGetValue(teamId, out td))
                    {
                        this.RoleId2JoinedTeam.Remove(client.ClientData.RoleID);
                    }
                    else
                    {
                        CopyTeamMemberData member = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == client.ClientData.RoleID);
                        if (member != null)
                        {
                            if (!this.IsKuaFuCopy(td.FuBenId))
                            {
                                this.OnTeamLeave(new CopyTeamLeaveData
                                {
                                    TeamId = td.TeamID,
                                    RoleId = client.ClientData.RoleID
                                });
                            }
                            else
                            {
                                KFCopyTeamLeaveReq req = new KFCopyTeamLeaveReq();
                                req.ReqServerId = this.ThisServerId;
                                req.RoleId = client.ClientData.RoleID;
                                req.TeamId = td.TeamID;
                                KFCopyTeamLeaveRsp rsp = KFCopyRpcClient.getInstance().LeaveTeam(req);
                                if (rsp == null)
                                {
                                    LogManager.WriteLog(LogTypes.Error, string.Format("KF 离开队伍RPC调用失败 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, req.TeamId), null, true);
                                    this.NotifyTeamCmd(client, CopyTeamErrorCodes.ServerException, 9, 0L, "", -1, -1, -1);
                                }
                                else if (rsp.ErrorCode == CopyTeamErrorCodes.Success)
                                {
                                    this.OnTeamLeave(rsp.Data);
                                }
                                else
                                {
                                    this.NotifyTeamCmd(client, rsp.ErrorCode, 9, 0L, "", -1, -1, -1);
                                    LogManager.WriteLog(LogTypes.Error, string.Format("KF 离开队伍失败 roleid={0}, rolename={1}, teamid={2}, errorcode={3}", new object[]
                                    {
                                        client.ClientData.RoleID,
                                        client.ClientData.RoleName,
                                        req.TeamId,
                                        rsp.ErrorCode
                                    }), null, true);
                                    if (rsp.ErrorCode == CopyTeamErrorCodes.TeamIsDestoryed)
                                    {
                                        LogManager.WriteLog(LogTypes.Error, string.Format("KF 离开队伍, 队伍在中心已销毁 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, req.TeamId), null, true);
                                        this.OnTeamDestroy(new CopyTeamDestroyData
                                        {
                                            TeamId = req.TeamId
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        
        public void HandleSetReady(GameClient client, int ready)
        {
            if (!client.ClientSocket.IsKuaFuLogin)
            {
                lock (this.Mutex)
                {
                    long teamId;
                    CopyTeamData td;
                    if (!this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out teamId))
                    {
                        this.NotifyTeamStateChanged(client, -1L, client.ClientData.RoleID, 0);
                    }
                    else if (this.CopyTeamDict.TryGetValue(teamId, out td))
                    {
                        if (!this.IsKuaFuCopy(td.FuBenId))
                        {
                            this.OnTeamSetReady(new CopyTeamReadyData
                            {
                                RoleId = client.ClientData.RoleID,
                                TeamId = td.TeamID,
                                Ready = ready
                            });
                        }
                        else
                        {
                            KFCopyTeamSetReadyReq req = new KFCopyTeamSetReadyReq();
                            req.RoleId = client.ClientData.RoleID;
                            req.TeamId = td.TeamID;
                            req.Ready = ready;
                            KFCopyTeamSetReadyRsp rsp = KFCopyRpcClient.getInstance().SetReady(req);
                            if (rsp == null)
                            {
                                LogManager.WriteLog(LogTypes.Error, string.Format("KF 设置准备状态RPC调用失败 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, req.TeamId), null, true);
                                this.NotifyTeamStateChanged(client, -13L, client.ClientData.RoleID, 0);
                            }
                            else if (rsp.ErrorCode == CopyTeamErrorCodes.Success)
                            {
                                this.OnTeamSetReady(rsp.Data);
                            }
                            else
                            {
                                this.NotifyTeamStateChanged(client, (long)rsp.ErrorCode, client.ClientData.RoleID, 0);
                                LogManager.WriteLog(LogTypes.Error, string.Format("KF 设置准备状态失败 roleid={0}, rolename={1}, teamid={2}, errorcode={3}", new object[]
                                {
                                    client.ClientData.RoleID,
                                    client.ClientData.RoleName,
                                    req.TeamId,
                                    rsp.ErrorCode
                                }), null, true);
                                if (rsp.ErrorCode == CopyTeamErrorCodes.TeamIsDestoryed)
                                {
                                    LogManager.WriteLog(LogTypes.Error, string.Format("KF 设置准备状态, 队伍在中心已销毁 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, req.TeamId), null, true);
                                    this.OnTeamDestroy(new CopyTeamDestroyData
                                    {
                                        TeamId = req.TeamId
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        
        public void HandleQuickJoinTeam(GameClient client, int copyId)
        {
            if (!client.ClientSocket.IsKuaFuLogin)
            {
                SystemXmlItem copyItem = null;
                if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyId, out copyItem))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("HandleQuickJoinTeam Faild copyId={0}", copyId), null, true);
                }
                else if (!FuBenChecker.HasFinishedPreTask(client, copyItem))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("HandleQuickJoinTeam Faild !HasFinishedPreTask copyId={0}", copyId), null, true);
                }
                else if (!FuBenChecker.HasPassedPreCopy(client, copyItem))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("HandleQuickJoinTeam Faild !HasPassedPreCopy copyId={0}", copyId), null, true);
                }
                else if (!FuBenChecker.IsInCopyLevelLimit(client, copyItem))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("HandleQuickJoinTeam Faild !IsInCopyLevelLimit copyId={0}", copyId), null, true);
                }
                else if (!FuBenChecker.IsInCopyTimesLimit(client, copyItem))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("HandleQuickJoinTeam Faild !IsInCopyTimesLimit copyId={0}", copyId), null, true);
                }
                else
                {
                    lock (this.Mutex)
                    {
                        if (this.RoleId2JoinedTeam.ContainsKey(client.ClientData.RoleID))
                        {
                            this.NotifyTeamCmd(client, CopyTeamErrorCodes.AllreadyHasTeam, 13, 0L, "", -1, -1, -1);
                        }
                        else
                        {
                            int zhanLi = client.ClientData.CombatForce;
                            HashSet<long> teamIdList = null;
                            if (!this.FuBenId2Teams.TryGetValue(copyId, out teamIdList) || teamIdList.Count <= 0)
                            {
                                this.NotifyTeamCmd(client, CopyTeamErrorCodes.NoAcceptableTeam, 13, -1L, "", -1, -1, -1);
                            }
                            else
                            {
                                CopyTeamData selectTd = null;
                                foreach (long teamId in teamIdList.ToList<long>())
                                {
                                    CopyTeamData tmpTd = null;
                                    if (!this.CopyTeamDict.TryGetValue(teamId, out tmpTd))
                                    {
                                        teamIdList.Remove(teamId);
                                    }
                                    else if (tmpTd.StartTime <= 0L && zhanLi >= tmpTd.MinZhanLi && tmpTd.MemberCount < ConstData.CopyRoleMax(tmpTd.FuBenId))
                                    {
                                        selectTd = tmpTd;
                                        break;
                                    }
                                }
                                if (selectTd == null)
                                {
                                    this.NotifyTeamCmd(client, CopyTeamErrorCodes.NoAcceptableTeam, 13, -1L, "", -1, -1, -1);
                                }
                                else
                                {
                                    this.HandleApplyCopyTeam(client, selectTd.TeamID);
                                }
                            }
                        }
                    }
                }
            }
        }

        
        public void HandleModKickFlag(GameClient client, int flag)
        {
            if (!client.ClientSocket.IsKuaFuLogin)
            {
                lock (this.Mutex)
                {
                    long teamId;
                    CopyTeamData td;
                    if (!this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out teamId))
                    {
                        this.NotifyTeamStateChanged(client, -1L, client.ClientData.RoleID, 0);
                    }
                    else if (this.CopyTeamDict.TryGetValue(teamId, out td))
                    {
                        if (!this.IsKuaFuCopy(td.FuBenId))
                        {
                            this.OnTeamSetFlag(new CopyTeamFlagData
                            {
                                RoleId = client.ClientData.RoleID,
                                TeamId = td.TeamID,
                                AutoKick = flag,
                                AutoStart = (td.AutoStart ? 1 : 0)
                            });
                        }
                        else
                        {
                            KFCopyTeamSetFlagReq req = new KFCopyTeamSetFlagReq();
                            req.RoleId = client.ClientData.RoleID;
                            req.TeamId = td.TeamID;
                            req.AutoStart = (td.AutoStart ? 1 : 0);
                            req.AutoKick = flag;
                            KFCopyTeamSetFlagRsp rsp = KFCopyRpcClient.getInstance().SetFlag(req);
                            if (rsp == null)
                            {
                                LogManager.WriteLog(LogTypes.Error, string.Format("KF 设置准备状态RPC调用失败 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, req.TeamId), null, true);
                                this.NotifyTeamStateChanged(client, -13L, client.ClientData.RoleID, 0);
                            }
                            else if (rsp.ErrorCode == CopyTeamErrorCodes.Success)
                            {
                                this.OnTeamSetFlag(rsp.Data);
                            }
                            else
                            {
                                this.NotifyTeamCmd(client, rsp.ErrorCode, 15, (long)td.AutoKick, "", -1, -1, -1);
                                LogManager.WriteLog(LogTypes.Error, string.Format("KF 设置准备状态失败 roleid={0}, rolename={1}, teamid={2}, errorcode={3}", new object[]
                                {
                                    client.ClientData.RoleID,
                                    client.ClientData.RoleName,
                                    req.TeamId,
                                    rsp.ErrorCode
                                }), null, true);
                                if (rsp.ErrorCode == CopyTeamErrorCodes.TeamIsDestoryed)
                                {
                                    LogManager.WriteLog(LogTypes.Error, string.Format("KF 设置准备状态, 队伍在中心已销毁 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, req.TeamId), null, true);
                                    this.OnTeamDestroy(new CopyTeamDestroyData
                                    {
                                        TeamId = req.TeamId
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        
        public void HandleModAutoStart(GameClient client, int flag)
        {
            if (!client.ClientSocket.IsKuaFuLogin)
            {
                lock (this.Mutex)
                {
                    long teamId;
                    CopyTeamData td;
                    if (!this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out teamId))
                    {
                        this.NotifyTeamStateChanged(client, -1L, client.ClientData.RoleID, 0);
                    }
                    else if (this.CopyTeamDict.TryGetValue(teamId, out td))
                    {
                        if (!this.IsKuaFuCopy(td.FuBenId))
                        {
                            this.OnTeamSetFlag(new CopyTeamFlagData
                            {
                                RoleId = client.ClientData.RoleID,
                                TeamId = td.TeamID,
                                AutoKick = td.AutoKick,
                                AutoStart = flag
                            });
                        }
                        else
                        {
                            KFCopyTeamSetFlagReq req = new KFCopyTeamSetFlagReq();
                            req.RoleId = client.ClientData.RoleID;
                            req.TeamId = td.TeamID;
                            req.AutoStart = flag;
                            req.AutoKick = td.AutoKick;
                            KFCopyTeamSetFlagRsp rsp = KFCopyRpcClient.getInstance().SetFlag(req);
                            if (rsp == null)
                            {
                                LogManager.WriteLog(LogTypes.Error, string.Format("KF 设置准备状态RPC调用失败 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, req.TeamId), null, true);
                                this.NotifyTeamStateChanged(client, -13L, client.ClientData.RoleID, 0);
                            }
                            else if (rsp.ErrorCode == CopyTeamErrorCodes.Success)
                            {
                                this.OnTeamSetFlag(rsp.Data);
                            }
                            else
                            {
                                this.NotifyTeamCmd(client, rsp.ErrorCode, 16, td.AutoStart ? 1L : 0L, "", -1, -1, -1);
                                LogManager.WriteLog(LogTypes.Error, string.Format("KF 设置准备状态失败 roleid={0}, rolename={1}, teamid={2}, errorcode={3}", new object[]
                                {
                                    client.ClientData.RoleID,
                                    client.ClientData.RoleName,
                                    req.TeamId,
                                    rsp.ErrorCode
                                }), null, true);
                                if (rsp.ErrorCode == CopyTeamErrorCodes.TeamIsDestoryed)
                                {
                                    LogManager.WriteLog(LogTypes.Error, string.Format("KF 设置准备状态, 队伍在中心已销毁 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, req.TeamId), null, true);
                                    this.OnTeamDestroy(new CopyTeamDestroyData
                                    {
                                        TeamId = req.TeamId
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        
        public void HandleClickStart(GameClient client, int fubenSeqId)
        {
            lock (this.Mutex)
            {
                CopyTeamData td = null;
                if (!this.CanEnterScene(client, out td))
                {
                    client.sendCmd(253, string.Format("{0}:{1}", -100, client.ClientData.RoleID), false);
                }
                else if (!this.IsKuaFuCopy(td.FuBenId))
                {
                    this.OnTeamStart(new CopyTeamStartData
                    {
                        TeamId = td.TeamID,
                        StartMs = TimeUtil.NOW(),
                        ToServerId = 0,
                        FuBenSeqId = fubenSeqId
                    });
                }
                else
                {
                    SystemXmlItem copyItem = null;
                    FuBenMapItem mapItem = null;
                    if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(td.FuBenId, out copyItem) && (mapItem = FuBenManager.FindMapCodeByFuBenID(td.FuBenId, copyItem.GetIntValue("MapCode", -1))) != null)
                    {
                        KFCopyTeamStartReq req = new KFCopyTeamStartReq();
                        req.RoleId = client.ClientData.RoleID;
                        req.TeamId = td.TeamID;
                        req.LastMs = mapItem.MaxTime * 60 * 1000;
                        KFCopyTeamStartRsp rsp = KFCopyRpcClient.getInstance().StartGame(req);
                        if (rsp == null)
                        {
                            LogManager.WriteLog(LogTypes.Error, string.Format("KF 开始游戏RPC调用失败 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, req.TeamId), null, true);
                        }
                        else if (rsp.ErrorCode == CopyTeamErrorCodes.Success)
                        {
                            this.OnTeamStart(rsp.Data);
                        }
                        else
                        {
                            LogManager.WriteLog(LogTypes.Error, string.Format("KF 开始游戏 roleid={0}, rolename={1}, teamid={2}, errorcode={3}", new object[]
                            {
                                client.ClientData.RoleID,
                                client.ClientData.RoleName,
                                req.TeamId,
                                rsp.ErrorCode
                            }), null, true);
                            if (rsp.ErrorCode == CopyTeamErrorCodes.TeamIsDestoryed)
                            {
                                LogManager.WriteLog(LogTypes.Error, string.Format("KF 开始游戏, 队伍在中心已销毁 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, req.TeamId), null, true);
                                this.OnTeamDestroy(new CopyTeamDestroyData
                                {
                                    TeamId = req.TeamId
                                });
                            }
                        }
                        client.sendCmd(253, string.Format("{0}:{1}", 1000, client.ClientData.RoleID), false);
                    }
                }
            }
        }

        
        private void RegisterCopyTeamListNotify(GameClient client, int copyId)
        {
            lock (this.Mutex)
            {
                int roleId = client.ClientData.RoleID;
                foreach (KeyValuePair<int, HashSet<int>> kvp in this.FuBenId2Watchers)
                {
                    int _copyId = kvp.Key;
                    HashSet<int> _watchers = kvp.Value;
                    if (_copyId == copyId)
                    {
                        if (!_watchers.Contains(roleId))
                        {
                            _watchers.Add(roleId);
                        }
                    }
                    else
                    {
                        _watchers.Remove(roleId);
                    }
                }
            }
            this.SendTeamList(client, 0, copyId);
        }

        
        public void UnRegisterCopyTeamListNotify(GameClient client)
        {
            lock (this.Mutex)
            {
                foreach (KeyValuePair<int, HashSet<int>> kvp in this.FuBenId2Watchers)
                {
                    HashSet<int> watchers = kvp.Value;
                    watchers.Remove(client.ClientData.RoleID);
                }
            }
        }

        
        public bool CanEnterScene(GameClient client, out CopyTeamData td)
        {
            td = null;
            MarriageInstance FubenInstance = MarryFuBenMgr.getInstance().GetMarriageInstanceEX(client);
            if (FubenInstance != null)
            {
                if (MapTypes.MarriageCopy == Global.GetMapType(FubenInstance.nHusband_FuBenID))
                {
                    if (MarryFuBenMgr.getInstance().CanEnterSceneEX(client))
                    {
                        td = null;
                        return true;
                    }
                    td = null;
                    return false;
                }
            }
            bool result;
            lock (this.Mutex)
            {
                long teamId;
                if (!this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out teamId))
                {
                    this.NotifyTeamStateChanged(client, -1L, client.ClientData.RoleID, 0);
                    result = false;
                }
                else if (this.CopyTeamDict.TryGetValue(teamId, out td) && td.LeaderRoleID == client.ClientData.RoleID)
                {
                    if (this.IsKuaFuCopy(td.FuBenId))
                    {
                        result = true;
                    }
                    else
                    {
                        foreach (CopyTeamMemberData member in td.TeamRoles)
                        {
                            if (!member.IsReady)
                            {
                                return false;
                            }
                            GameClient gc = GameManager.ClientMgr.FindClient(member.RoleID);
                            if (gc == null)
                            {
                                member.IsReady = false;
                                member.NoReadyTicks = TimeUtil.NOW();
                                this.NotifyTeamData(td);
                                return false;
                            }
                        }
                        result = true;
                    }
                }
                else
                {
                    td = null;
                    result = false;
                }
            }
            return result;
        }

        
        public bool CanEnterOtherScene(GameClient client)
        {
            lock (this.Mutex)
            {
                long teamId;
                if (this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out teamId))
                {
                    return false;
                }
            }
            return true;
        }

        
        public void Update()
        {
            long nowMs = TimeUtil.NOW();
            lock (this.Mutex)
            {
                if (nowMs >= this.TimeLimitMemberNoReadyMs + 1000L)
                {
                    foreach (CopyTeamData copyTeam in this.CopyTeamDict.Values)
                    {
                        if (copyTeam.AutoKick > 0)
                        {
                            List<CopyTeamLeaveData> needDelList = new List<CopyTeamLeaveData>();
                            foreach (CopyTeamMemberData member in copyTeam.TeamRoles)
                            {
                                if (!member.IsReady && nowMs > member.NoReadyTicks + 30000L)
                                {
                                    needDelList.Add(new CopyTeamLeaveData
                                    {
                                        TeamId = copyTeam.TeamID,
                                        RoleId = member.RoleID
                                    });
                                }
                            }
                            foreach (CopyTeamLeaveData i in needDelList)
                            {
                                this.OnTeamLeave(i);
                            }
                        }
                    }
                }
            }
        }

        
        public List<CopyTeamData> GetTeamDataList(int startIndex, int count, int sceneIndex, int zhanLi)
        {
            int index = 0;
            List<CopyTeamData> teamDataList = new List<CopyTeamData>();
            lock (this.CopyTeamDict)
            {
                foreach (CopyTeamData teamData in this.CopyTeamDict.Values)
                {
                    if (index >= startIndex && sceneIndex == teamData.FuBenId && teamData.StartTime == 0L && zhanLi >= teamData.MinZhanLi && teamData.MemberCount < ConstData.CopyRoleMax(sceneIndex))
                    {
                        teamDataList.Add(teamData.SimpleClone());
                        if (teamDataList.Count >= count)
                        {
                            break;
                        }
                    }
                    index++;
                }
            }
            return teamDataList;
        }

        
        public List<CopyTeamData> GetTeamDataListInCopyMap(int sceneIndex = -1)
        {
            List<CopyTeamData> teamDataList = new List<CopyTeamData>();
            lock (this.CopyTeamDict)
            {
                foreach (CopyTeamData teamData in this.CopyTeamDict.Values)
                {
                    if (sceneIndex < 0 || sceneIndex == teamData.FuBenId)
                    {
                        if (teamData.StartTime > 0L && teamData.FuBenSeqID > 0)
                        {
                            teamDataList.Add(teamData);
                        }
                    }
                }
            }
            return teamDataList;
        }

        
        public CopyTeamMemberData ClientDataToTeamMemberData(SafeClientData clientData)
        {
            return new CopyTeamMemberData
            {
                RoleID = clientData.RoleID,
                RoleName = Global.FormatRoleName2(clientData, clientData.RoleName),
                RoleSex = clientData.RoleSex,
                Level = clientData.Level,
                Occupation = clientData.Occupation,
                RolePic = clientData.RolePic,
                MapCode = clientData.MapCode,
                OnlineState = 1,
                MaxLifeV = clientData.LifeV,
                CurrentLifeV = clientData.CurrentLifeV,
                MaxMagicV = clientData.MagicV,
                CurrentMagicV = clientData.CurrentMagicV,
                PosX = clientData.PosX,
                PosY = clientData.PosY,
                CombatForce = clientData.CombatForce,
                ChangeLifeLev = clientData.ChangeLifeCount,
                ServerId = this.ThisServerId,
                ZoneId = clientData.ZoneID
            };
        }

        
        public void RoleLeaveFuBen(GameClient client)
        {
            this.HandleQuitFromTeam(client, true);
        }

        
        public void OnPlayerLogin(GameClient client)
        {
            if (client != null)
            {
                lock (this.Mutex)
                {
                    long teamId;
                    if (this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out teamId))
                    {
                        CopyTeamData td;
                        if (this.CopyTeamDict.TryGetValue(teamId, out td))
                        {
                            if (!this.IsKuaFuCopy(td.FuBenId))
                            {
                                this.HandleQuitFromTeam(client, true);
                            }
                            else if (td.StartTime > 0L && (td.KFServerId == 0 || td.KFServerId != this.ThisServerId))
                            {
                                this.HandleQuitFromTeam(client, false);
                            }
                        }
                    }
                }
            }
        }

        
        public void OnPlayerLogout(GameClient client)
        {
            if (client != null)
            {
                this.UnRegisterCopyTeamListNotify(client);
                lock (this.Mutex)
                {
                    long teamId;
                    if (this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out teamId))
                    {
                        CopyTeamData td;
                        if (this.CopyTeamDict.TryGetValue(teamId, out td))
                        {
                            if (!this.IsKuaFuCopy(td.FuBenId))
                            {
                                this.HandleQuitFromTeam(client, true);
                            }
                            else if (td.StartTime <= 0L)
                            {
                                this.HandleQuitFromTeam(client, true);
                            }
                        }
                    }
                }
            }
        }

        
        public bool IsTeamCopyMapCode(int mapCode)
        {
            return this.MapCode2ToFubenId.ContainsKey(mapCode);
        }

        
        public List<int> GetTeamCopyMapCodes(int fubenId)
        {
            List<int> mapCodes = null;
            List<int> result;
            if (!this.FuBenId2MapCodes.TryGetValue(fubenId, out mapCodes))
            {
                result = null;
            }
            else
            {
                result = mapCodes;
            }
            return result;
        }

        
        public bool NeedRecordDamageInfoFuBenID(int fuBenID)
        {
            return this.RecordDamagesFuBenIDHashSet.Contains(fuBenID) || GameManager.GuildCopyMapMgr.IsGuildCopyMap(fuBenID);
        }

        
        public bool IsInRoleId2JoinedTeam(int nRid)
        {
            bool result;
            lock (this.Mutex)
            {
                result = this.RoleId2JoinedTeam.ContainsKey(nRid);
            }
            return result;
        }

        
        public void NotifyTeamListChange(CopyTeamData td)
        {
            if (td != null)
            {
                lock (this.Mutex)
                {
                    HashSet<int> watchers = null;
                    if (this.FuBenId2Watchers.TryGetValue(td.FuBenId, out watchers))
                    {
                        List<int> watcherList = watchers.ToList<int>();
                        if (watcherList != null && watcherList.Count<int>() > 0)
                        {
                            foreach (int rid in watcherList)
                            {
                                GameClient client = GameManager.ClientMgr.FindClient(rid);
                                if (client == null)
                                {
                                    watchers.Remove(rid);
                                }
                                else if (td.MemberCount <= 0 || td.MinZhanLi <= client.ClientData.CombatForce || td.StartTime > 0L)
                                {
                                    this.NotifyListTeamData(client, td);
                                }
                            }
                        }
                    }
                }
            }
        }

        
        public void NotifyListTeamData(GameClient client, CopyTeamData ctd)
        {
            int memberCount = (ctd.StartTime > 0L) ? 0 : ctd.MemberCount;
            string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
            {
                ctd.FuBenId,
                ctd.TeamID,
                ctd.TeamName,
                memberCount,
                ctd.MinZhanLi
            });
            client.sendCmd(625, strcmd, false);
        }

        
        public void NotifyListTeamRemove(GameClient client, long teamID, int sceneIndex = -1)
        {
            string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
            {
                sceneIndex,
                teamID,
                "",
                0,
                0
            });
            client.sendCmd(625, strcmd, false);
        }

        
        public void NotifyTeamCmd(GameClient client, CopyTeamErrorCodes status, int teamType, long extTag1, string extTag2, int nOccu = -1, int nLev = -1, int nChangeLife = -1)
        {
            string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
            {
                (int)status,
                client.ClientData.RoleID,
                teamType,
                extTag1,
                extTag2,
                nOccu,
                nLev,
                nChangeLife
            });
            client.sendCmd(621, strcmd, false);
        }

        
        public void NotifyTeamData(CopyTeamData td)
        {
            if (td != null)
            {
                lock (this.Mutex)
                {
                    foreach (CopyTeamMemberData member in td.TeamRoles)
                    {
                        if (member.ServerId == this.ThisServerId)
                        {
                            GameClient client = GameManager.ClientMgr.FindClient(member.RoleID);
                            if (null != client)
                            {
                                client.sendCmd<CopyTeamData>(622, td, false);
                            }
                        }
                    }
                }
            }
        }

        
        public void NotifyTeamCmds(CopyTeamData td, CopyTeamErrorCodes status, int teamType, long extTag1, string extTag2)
        {
            if (td != null)
            {
                lock (this.Mutex)
                {
                    foreach (CopyTeamMemberData member in td.TeamRoles)
                    {
                        if (member.ServerId == this.ThisServerId)
                        {
                            GameClient client = GameManager.ClientMgr.FindClient(member.RoleID);
                            if (null != client)
                            {
                                this.NotifyTeamCmd(client, status, teamType, extTag1, extTag2, -1, -1, -1);
                            }
                        }
                    }
                }
            }
        }

        
        public void NotifyTeamStateChanged(GameClient client, long teamID, int roleID, int isReady)
        {
            string strcmd = string.Format("{0}:{1}:{2}", roleID, teamID, isReady);
            client.sendCmd(623, strcmd, false);
        }

        
        public void NotifyTeamFuBenEnterMsg(List<int> roleIDsList, int minLevel, int maxLevel, int leaderMapCode, int leaderRoleID, int fuBenID, int fuBenSeqID, int enterNumber, int maxFinishNum, bool igoreNumLimit = false)
        {
            if (roleIDsList != null && roleIDsList.Count > 0)
            {
                for (int i = 0; i < roleIDsList.Count; i++)
                {
                    GameClient otherClient = GameManager.ClientMgr.FindClient(roleIDsList[i]);
                    if (null != otherClient)
                    {
                        int unionLevel = Global.GetUnionLevel(otherClient.ClientData.ChangeLifeCount, otherClient.ClientData.Level, false);
                        if (unionLevel >= minLevel && unionLevel <= maxLevel)
                        {
                            if (!igoreNumLimit)
                            {
                                FuBenData fuBenData = Global.GetFuBenData(otherClient, fuBenID);
                                int nFinishNum;
                                int haveEnterNum = Global.GetFuBenEnterNum(fuBenData, out nFinishNum);
                                if ((enterNumber >= 0 && haveEnterNum >= enterNumber) || (maxFinishNum >= 0 && nFinishNum >= maxFinishNum))
                                {
                                    goto IL_D4;
                                }
                            }
                            GameManager.ClientMgr.NotifyTeamMemberFuBenEnterMsg(otherClient, leaderRoleID, fuBenID, fuBenSeqID);
                        }
                    }
                    IL_D4:;
                }
            }
        }

        
        public void SendTeamList(GameClient client, int startIndex, int copyId)
        {
            CopySearchTeamData searchData = new CopySearchTeamData
            {
                StartIndex = startIndex,
                TotalTeamsCount = 0,
                PageTeamsCount = 100,
                TeamDataList = null
            };
            lock (this.Mutex)
            {
                searchData.TotalTeamsCount = this.CopyTeamDict.Count<KeyValuePair<long, CopyTeamData>>();
                startIndex = ((startIndex >= this.CopyTeamDict.Count) ? 0 : startIndex);
                if (this.CopyTeamDict.Count > 0)
                {
                    searchData.TeamDataList = new List<CopyTeamData>();
                    int _index = 0;
                    foreach (CopyTeamData td in this.CopyTeamDict.Values)
                    {
                        if (_index >= startIndex && copyId == td.FuBenId && td.StartTime == 0L && client.ClientData.CombatForce >= td.MinZhanLi && td.MemberCount < ConstData.CopyRoleMax(copyId))
                        {
                            searchData.TeamDataList.Add(td.SimpleClone());
                            if (searchData.TeamDataList.Count<CopyTeamData>() >= searchData.PageTeamsCount)
                            {
                                break;
                            }
                        }
                        _index++;
                    }
                }
            }
            client.sendCmd<CopySearchTeamData>(620, searchData, false);
        }

        
        public void OnLeaveFuBen(GameClient client, SceneUIClasses sceneType)
        {
            CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(client.ClientData.MapCode, client.ClientData.FuBenSeqID);
            if (!copyMap.CopyMapPassAwardFlag)
            {
                KuaFuManager.getInstance().SetCannotJoinKuaFu_UseAutoEndTicks(client);
            }
        }

        
        public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
        {
            int fuBenSeqId = copyMap.FuBenSeqID;
            int mapCode = copyMap.MapCode;
            FuBenManager.AddFuBenSeqID(client.ClientData.RoleID, copyMap.FuBenSeqID, 0, copyMap.FubenMapID);
            return true;
        }

        
        public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
        {
            return true;
        }

        
        public void TimerProc()
        {
        }

        
        public const int ConstCopyType = 1;

        
        private HashSet<int> RecordDamagesFuBenIDHashSet = new HashSet<int>();

        
        private Dictionary<int, HashSet<int>> FuBenId2Watchers = new Dictionary<int, HashSet<int>>();

        
        private Dictionary<int, HashSet<long>> FuBenId2Teams = new Dictionary<int, HashSet<long>>();

        
        private Dictionary<int, int> MapCode2ToFubenId = new Dictionary<int, int>();

        
        private Dictionary<int, List<int>> FuBenId2MapCodes = new Dictionary<int, List<int>>();

        
        private Dictionary<long, CopyTeamData> CopyTeamDict = new Dictionary<long, CopyTeamData>();

        
        public long TimeLimitMemberNoReadyMs = 0L;

        
        private Dictionary<int, long> CreatKuaFuCopyLinkIntervalDic = new Dictionary<int, long>();

        
        private int ThisServerId;

        
        private Dictionary<int, long> RoleId2JoinedTeam = new Dictionary<int, long>();

        
        private Dictionary<int, long> FuBenSeq2TeamId = new Dictionary<int, long>();

        
        private object Mutex = new object();
    }
}
