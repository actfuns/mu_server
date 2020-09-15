using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
    // Token: 0x02000627 RID: 1575
    public class DJRoomManager
    {
        // Token: 0x17000214 RID: 532
        // (get) Token: 0x0600202C RID: 8236 RVA: 0x001BC2D4 File Offset: 0x001BA4D4
        public object Mutex
        {
            get
            {
                return this.mutex;
            }
        }

        // Token: 0x0600202D RID: 8237 RVA: 0x001BC2EC File Offset: 0x001BA4EC
        public int GetNewRoomID()
        {
            int id = 1;
            lock (this.mutex)
            {
                id = this.BaseRoomID++;
            }
            return id;
        }

        // Token: 0x0600202E RID: 8238 RVA: 0x001BC350 File Offset: 0x001BA550
        public List<DJRoomData> CloneRoomDataList()
        {
            List<DJRoomData> roomDataList = null;
            lock (this.mutex)
            {
                roomDataList = this.DJRoomDataList.GetRange(0, this.DJRoomDataList.Count);
            }
            return roomDataList;
        }

        // Token: 0x0600202F RID: 8239 RVA: 0x001BC3B8 File Offset: 0x001BA5B8
        public DJRoomData FindRoomData(int roomID)
        {
            DJRoomData djRoomData = null;
            lock (this.mutex)
            {
                this.DJRoomDict.TryGetValue(roomID, out djRoomData);
            }
            return djRoomData;
        }

        // Token: 0x06002030 RID: 8240 RVA: 0x001BC418 File Offset: 0x001BA618
        public void AddRoomData(DJRoomData roomData)
        {
            lock (this.mutex)
            {
                this.DJRoomDict[roomData.RoomID] = roomData;
                this.DJRoomDataList.Add(roomData);
            }
        }

        // Token: 0x06002031 RID: 8241 RVA: 0x001BC480 File Offset: 0x001BA680
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

        // Token: 0x06002032 RID: 8242 RVA: 0x001BC4F8 File Offset: 0x001BA6F8
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

        // Token: 0x06002033 RID: 8243 RVA: 0x001BC56C File Offset: 0x001BA76C
        public DJRoomRolesData FindRoomRolesData(int roomID)
        {
            DJRoomRolesData djRoomRolesData = null;
            lock (this.mutex)
            {
                this.DJRoomRolesDict.TryGetValue(roomID, out djRoomRolesData);
            }
            return djRoomRolesData;
        }

        // Token: 0x06002034 RID: 8244 RVA: 0x001BC5CC File Offset: 0x001BA7CC
        public void AddRoomRolesData(DJRoomRolesData djRoomRolesData)
        {
            lock (this.mutex)
            {
                this.DJRoomRolesDict[djRoomRolesData.RoomID] = djRoomRolesData;
            }
        }

        // Token: 0x06002035 RID: 8245 RVA: 0x001BC624 File Offset: 0x001BA824
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

        // Token: 0x06002036 RID: 8246 RVA: 0x001BC68C File Offset: 0x001BA88C
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

        // Token: 0x06002037 RID: 8247 RVA: 0x001BC714 File Offset: 0x001BA914
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

        // Token: 0x06002038 RID: 8248 RVA: 0x001BC750 File Offset: 0x001BA950
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

        // Token: 0x06002039 RID: 8249 RVA: 0x001BC814 File Offset: 0x001BAA14
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

        // Token: 0x0600203A RID: 8250 RVA: 0x001BC8F0 File Offset: 0x001BAAF0
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

        // Token: 0x0600203B RID: 8251 RVA: 0x001BCCBC File Offset: 0x001BAEBC
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

        // Token: 0x0600203C RID: 8252 RVA: 0x001BCD10 File Offset: 0x001BAF10
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

        // Token: 0x0600203D RID: 8253 RVA: 0x001BCD54 File Offset: 0x001BAF54
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

        // Token: 0x0600203E RID: 8254 RVA: 0x001BCDE8 File Offset: 0x001BAFE8
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

        // Token: 0x0600203F RID: 8255 RVA: 0x001BCF28 File Offset: 0x001BB128
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

        // Token: 0x04002CDB RID: 11483
        private object mutex = new object();

        // Token: 0x04002CDC RID: 11484
        private int BaseRoomID = 1;

        // Token: 0x04002CDD RID: 11485
        private Dictionary<int, DJRoomData> DJRoomDict = new Dictionary<int, DJRoomData>(100);

        // Token: 0x04002CDE RID: 11486
        private List<DJRoomData> DJRoomDataList = new List<DJRoomData>(100);

        // Token: 0x04002CDF RID: 11487
        private Dictionary<int, DJRoomRolesData> DJRoomRolesDict = new Dictionary<int, DJRoomRolesData>(100);
    }
}
