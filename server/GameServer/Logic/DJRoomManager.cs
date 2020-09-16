using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
    
    public class DJRoomManager
    {
        
        
        public object Mutex
        {
            get
            {
                return this.mutex;
            }
        }

        
        public int GetNewRoomID()
        {
            int id = 1;
            lock (this.mutex)
            {
                id = this.BaseRoomID++;
            }
            return id;
        }

        
        public List<DJRoomData> CloneRoomDataList()
        {
            List<DJRoomData> roomDataList = null;
            lock (this.mutex)
            {
                roomDataList = this.DJRoomDataList.GetRange(0, this.DJRoomDataList.Count);
            }
            return roomDataList;
        }

        
        public DJRoomData FindRoomData(int roomID)
        {
            DJRoomData djRoomData = null;
            lock (this.mutex)
            {
                this.DJRoomDict.TryGetValue(roomID, out djRoomData);
            }
            return djRoomData;
        }

        
        public void AddRoomData(DJRoomData roomData)
        {
            lock (this.mutex)
            {
                this.DJRoomDict[roomData.RoomID] = roomData;
                this.DJRoomDataList.Add(roomData);
            }
        }

        
        public void RemoveRoomData(int roomID)
        {
            lock (this.mutex)
            {
                DJRoomData roomData = null;
                if (this.DJRoomDict.TryGetValue(roomID, out roomData))
                {
                    this.DJRoomDict.Remove(roomID);
                    this.DJRoomDataList.Remove(roomData);
                }
            }
        }

        
        public DJRoomData GetNextDJRoomData(int index)
        {
            DJRoomData djRoomData = null;
            lock (this.mutex)
            {
                if (index < this.DJRoomDataList.Count)
                {
                    djRoomData = this.DJRoomDataList[index];
                }
            }
            return djRoomData;
        }

        
        public DJRoomRolesData FindRoomRolesData(int roomID)
        {
            DJRoomRolesData djRoomRolesData = null;
            lock (this.mutex)
            {
                this.DJRoomRolesDict.TryGetValue(roomID, out djRoomRolesData);
            }
            return djRoomRolesData;
        }

        
        public void AddRoomRolesData(DJRoomRolesData djRoomRolesData)
        {
            lock (this.mutex)
            {
                this.DJRoomRolesDict[djRoomRolesData.RoomID] = djRoomRolesData;
            }
        }

        
        public void RemoveRoomRolesData(int roomID)
        {
            lock (this.mutex)
            {
                if (this.DJRoomRolesDict.ContainsKey(roomID))
                {
                    this.DJRoomRolesDict.Remove(roomID);
                }
            }
        }

        
        public void SetRoomRolesDataRoleState(int roomID, int roleID, int state)
        {
            DJRoomRolesData djRoomRolesData = this.FindRoomRolesData(roomID);
            if (null != djRoomRolesData)
            {
                lock (this.mutex)
                {
                    int oldState = 0;
                    djRoomRolesData.RoleStates.TryGetValue(roleID, out oldState);
                    if (state > oldState)
                    {
                        djRoomRolesData.RoleStates[roleID] = state;
                    }
                }
            }
        }

        
        public void ProcessFighting()
        {
            int index = 0;
            DJRoomData djRoomData = this.GetNextDJRoomData(index);
            while (null != djRoomData)
            {
                this.ProcessRoomFighting(djRoomData);
                index++;
                djRoomData = this.GetNextDJRoomData(index);
            }
        }

        
        private bool CanGameOver(DJRoomRolesData djRoomRolesData)
        {
            bool team1Over = true;
            for (int i = 0; i < djRoomRolesData.Team1.Count; i++)
            {
                int state = 0;
                djRoomRolesData.RoleStates.TryGetValue(djRoomRolesData.Team1[i].RoleID, out state);
                if (state == 1)
                {
                    team1Over = false;
                    break;
                }
            }
            bool team2Over = true;
            for (int i = 0; i < djRoomRolesData.Team2.Count; i++)
            {
                int state = 0;
                djRoomRolesData.RoleStates.TryGetValue(djRoomRolesData.Team2[i].RoleID, out state);
                if (state == 1)
                {
                    team2Over = false;
                    break;
                }
            }
            return team1Over || team2Over;
        }

        
        private int GetLoseTeam(DJRoomRolesData djRoomRolesData)
        {
            bool team1Over = true;
            for (int i = 0; i < djRoomRolesData.Team1.Count; i++)
            {
                int state = 0;
                djRoomRolesData.RoleStates.TryGetValue(djRoomRolesData.Team1[i].RoleID, out state);
                if (state == 1)
                {
                    team1Over = false;
                    break;
                }
            }
            bool team2Over = true;
            for (int i = 0; i < djRoomRolesData.Team2.Count; i++)
            {
                int state = 0;
                djRoomRolesData.RoleStates.TryGetValue(djRoomRolesData.Team2[i].RoleID, out state);
                if (state == 1)
                {
                    team2Over = false;
                    break;
                }
            }
            int result;
            if (team1Over)
            {
                result = 1;
            }
            else if (team2Over)
            {
                result = 2;
            }
            else
            {
                result = 0;
            }
            return result;
        }

        
        private void ProcessRoomFighting(DJRoomData djRoomData)
        {
            int djFightState = 0;
            long startFightTicks = 0L;
            DJRoomData obj;
            lock (djRoomData)
            {
                if (djRoomData.PKState <= 0)
                {
                    return;
                }
                djFightState = djRoomData.DJFightState;
                startFightTicks = djRoomData.StartFightTicks;
            }
            long ticks = TimeUtil.NOW();
            if (djFightState == 0)
            {
                GameManager.ClientMgr.NotifyDianJiangFightCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djRoomData, 1, ticks.ToString(), null);
                lock (djRoomData)
                {
                    djRoomData.DJFightState = 1;
                    djRoomData.StartFightTicks = ticks;
                }
            }
            else if (djFightState == 1)
            {
                if (ticks >= startFightTicks + 30000L)
                {
                    GameManager.ClientMgr.NotifyDianJiangFightCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djRoomData, 2, ticks.ToString(), null);
                    lock (djRoomData)
                    {
                        djRoomData.PKState = 2;
                        djRoomData.DJFightState = 2;
                        djRoomData.StartFightTicks = ticks;
                    }
                }
            }
            else if (djFightState == 2)
            {
                bool gameOver = false;
                DJRoomRolesData djRoomRolesData = this.FindRoomRolesData(djRoomData.RoomID);
                if (null != djRoomRolesData)
                {
                    gameOver = this.CanGameOver(djRoomRolesData);
                }
                if (gameOver || ticks >= startFightTicks + 90000L)
                {
                    GameManager.ClientMgr.NotifyDianJiangFightCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djRoomData, 3, ticks.ToString(), null);
                    lock (djRoomData)
                    {
                        djRoomData.PKState = 3;
                        djRoomData.DJFightState = 3;
                        djRoomData.StartFightTicks = ticks;
                    }
                    GameManager.ClientMgr.NotifyDianJiangData(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djRoomData);
                }
            }
            else if (djFightState == 3)
            {
                this.ProcessDJFightAwards(djRoomData);
                GameManager.ClientMgr.NotifyDianJiangFightCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djRoomData, 4, ticks.ToString(), null);
                lock (djRoomData)
                {
                    djRoomData.DJFightState = 4;
                    djRoomData.StartFightTicks = ticks;
                }
            }
            else if (djFightState == 4)
            {
                if (ticks >= startFightTicks + 60000L)
                {
                    GameManager.ClientMgr.NotifyDJFightRoomLeaveMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djRoomData);
                    GameManager.ClientMgr.RemoveDianJiangRoom(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djRoomData);
                }
            }
        }

        
        private int GetTeamAvgDJPoint(List<DJRoomRoleData> team)
        {
            int result;
            if (team.Count <= 0)
            {
                result = 0;
            }
            else
            {
                int totalDJPoint = 0;
                for (int i = 0; i < team.Count; i++)
                {
                    totalDJPoint += team[i].DJPoint;
                }
                result = totalDJPoint / team.Count;
            }
            return result;
        }

        
        private int GetDJPointClass(int djPoint)
        {
            int result;
            if (djPoint <= 100)
            {
                result = 0;
            }
            else if (djPoint <= 200)
            {
                result = 1;
            }
            else if (djPoint <= 300)
            {
                result = 2;
            }
            else
            {
                result = 3;
            }
            return result;
        }

        
        private int GetRetPoint(int pointClass, bool isWinner)
        {
            int retPoint;
            if (0 == pointClass)
            {
                if (isWinner)
                {
                    retPoint = 10;
                }
                else
                {
                    retPoint = -4;
                }
            }
            else if (1 == pointClass)
            {
                if (isWinner)
                {
                    retPoint = 9;
                }
                else
                {
                    retPoint = -5;
                }
            }
            else if (2 == pointClass)
            {
                if (isWinner)
                {
                    retPoint = 8;
                }
                else
                {
                    retPoint = -6;
                }
            }
            else if (isWinner)
            {
                retPoint = 7;
            }
            else
            {
                retPoint = -7;
            }
            return retPoint;
        }

        
        private int GetTeamRolePoint(DJRoomRoleData djRoomRoleData, int otherTeamAvgDJPoint, bool isWinner)
        {
            int selfPointClass = this.GetDJPointClass(djRoomRoleData.DJPoint);
            int otherPointClass = this.GetDJPointClass(otherTeamAvgDJPoint);
            int absDJPoint = Math.Abs(selfPointClass - otherPointClass);
            int retPoint = this.GetRetPoint(selfPointClass, isWinner);
            if (0 != absDJPoint)
            {
                if (1 == absDJPoint)
                {
                    if (selfPointClass > otherPointClass)
                    {
                        if (!isWinner)
                        {
                            retPoint -= 10;
                        }
                    }
                    else if (isWinner)
                    {
                        retPoint += 10;
                    }
                }
                else if (2 == absDJPoint)
                {
                    if (selfPointClass > otherPointClass)
                    {
                        if (!isWinner)
                        {
                            retPoint -= 15;
                        }
                    }
                    else if (isWinner)
                    {
                        retPoint += 15;
                    }
                }
                else if (3 == absDJPoint)
                {
                    if (selfPointClass > otherPointClass)
                    {
                        if (!isWinner)
                        {
                            retPoint -= 20;
                        }
                    }
                    else if (isWinner)
                    {
                        retPoint += 20;
                    }
                }
            }
            return retPoint;
        }

        
        private void ProcessDJFightAwards(DJRoomData djRoomData)
        {
            DJRoomRolesData djRoomRolesData = this.FindRoomRolesData(djRoomData.RoomID);
            if (null != djRoomRolesData)
            {
                DJRoomRolesPoint djRoomRolesPoint = new DJRoomRolesPoint
                {
                    RoomID = djRoomData.RoomID,
                    RoomName = djRoomData.RoomName,
                    RolePoints = new List<DJRoomRolePoint>()
                };
                lock (djRoomRolesData)
                {
                    int loseTeam = this.GetLoseTeam(djRoomRolesData);
                    int team1AvgDJPoint = this.GetTeamAvgDJPoint(djRoomRolesData.Team1);
                    int team2AvgDJPoint = this.GetTeamAvgDJPoint(djRoomRolesData.Team2);
                    for (int i = 0; i < djRoomRolesData.Team1.Count; i++)
                    {
                        djRoomRolesPoint.RolePoints.Add(new DJRoomRolePoint
                        {
                            RoleID = djRoomRolesData.Team1[i].RoleID,
                            RoleName = djRoomRolesData.Team1[i].RoleName,
                            FightPoint = ((loseTeam > 0) ? this.GetTeamRolePoint(djRoomRolesData.Team1[i], team2AvgDJPoint, loseTeam != 1) : 0)
                        });
                    }
                    for (int i = 0; i < djRoomRolesData.Team2.Count; i++)
                    {
                        djRoomRolesPoint.RolePoints.Add(new DJRoomRolePoint
                        {
                            RoleID = djRoomRolesData.Team2[i].RoleID,
                            RoleName = djRoomRolesData.Team2[i].RoleName,
                            FightPoint = ((loseTeam > 0) ? this.GetTeamRolePoint(djRoomRolesData.Team1[i], team1AvgDJPoint, loseTeam != 2) : 0)
                        });
                    }
                }
                for (int i = 0; i < djRoomRolesPoint.RolePoints.Count; i++)
                {
                    if (djRoomRolesPoint.RolePoints[i].FightPoint != 0)
                    {
                        GameManager.DBCmdMgr.AddDBCmd(10023, string.Format("{0}:{1}", djRoomRolesPoint.RolePoints[i].RoleID, djRoomRolesPoint.RolePoints[i].FightPoint), null, 0);
                    }
                }
                GameManager.ClientMgr.NotifyDianJiangRoomRolesPoint(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djRoomRolesPoint);
            }
        }

        
        private object mutex = new object();

        
        private int BaseRoomID = 1;

        
        private Dictionary<int, DJRoomData> DJRoomDict = new Dictionary<int, DJRoomData>(100);

        
        private List<DJRoomData> DJRoomDataList = new List<DJRoomData>(100);

        
        private Dictionary<int, DJRoomRolesData> DJRoomRolesDict = new Dictionary<int, DJRoomRolesData>(100);
    }
}
