using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic.NewBufferExt;
using Server.Data;

namespace GameServer.Logic
{
    // Token: 0x02000620 RID: 1568
    public class DBMonsterBuffer
    {
        // Token: 0x06001FCC RID: 8140 RVA: 0x001B7B4C File Offset: 0x001B5D4C
        public static void ProcessDSTimeAddLifeNoShow(Monster monster)
        {
            if (monster.VLife > 0.0)
            {
                double lifeV = 0.0;
                BufferData bufferData = Global.GetMonsterBufferDataByID(monster, 40);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        int timeSlotSecs = (int)((ulong)bufferData.BufferVal >> 32 & 0xffff_ffffUL);
                        int addLiefV = (int)(bufferData.BufferVal & (long)0xffff_ffffUL);
                        if (nowTicks - monster.DSStartDSAddLifeNoShowTicks >= (long)(timeSlotSecs * 1000))
                        {
                            monster.DSStartDSAddLifeNoShowTicks = nowTicks;
                            lifeV = (double)addLiefV;
                        }
                    }
                    else
                    {
                        Global.RemoveMonsterBufferData(monster, 40);
                    }
                }
                if (monster.VLife < monster.MonsterInfo.VLifeMax && lifeV > 0.0)
                {
                    lifeV += monster.VLife;
                    monster.VLife = Global.GMin(monster.MonsterInfo.VLifeMax, lifeV);
                    List<object> listObjs = Global.GetAll9Clients(monster);
                    GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.SafeDirection, monster.VLife, monster.VMana, 120, listObjs, 0);
                }
            }
        }

        // Token: 0x06001FCD RID: 8141 RVA: 0x001B7CD8 File Offset: 0x001B5ED8
        public static int ProcessHuZhaoSubLifeV(Monster monster, int subLifeV)
        {
            if (monster.VLife > 0.0)
            {
                if (Global.CanMapUseBuffer(monster.CurrentMapCode, 97))
                {
                    BufferData bufferData = Global.GetMonsterBufferDataByID(monster, 97);
                    if (null != bufferData)
                    {
                        if (bufferData.BufferVal > 0L)
                        {
                            long needLifeV = (long)(monster.MonsterInfo.VLifeMax - monster.VLife);
                            HuZhaoBufferItem huZhaoBufferItem = monster.MyBufferExtManager.FindBufferItem(97) as HuZhaoBufferItem;
                            if (huZhaoBufferItem != null)
                            {
                                needLifeV = Global.GMin(needLifeV, (long)huZhaoBufferItem.InjuredV);
                                needLifeV = (long)((int)Global.GMin(needLifeV, (long)((int)bufferData.BufferVal)));
                                bufferData.BufferVal -= (long)Global.GMin((int)bufferData.BufferVal, huZhaoBufferItem.InjuredV);
                                subLifeV = (int)Global.GMin(needLifeV, (long)subLifeV);
                            }
                        }
                        else
                        {
                            Global.RemoveMonsterBufferData(monster, 97);
                            monster.MyBufferExtManager.RemoveBufferItem(97);
                            bufferData.BufferSecs = 0;
                            bufferData.StartTime = 0L;
                            GameManager.ClientMgr.NotifyOtherBufferData(monster, bufferData);
                        }
                    }
                }
            }
            return subLifeV;
        }

        // Token: 0x06001FCE RID: 8142 RVA: 0x001B7E0C File Offset: 0x001B600C
        public static double ProcessHuZhaoRecoverPercent(Monster monster)
        {
            double percent = 0.0;
            if (monster.VLife > 0.0)
            {
                if (Global.CanMapUseBuffer(monster.CurrentMapCode, 97))
                {
                    BufferData bufferData = Global.GetMonsterBufferDataByID(monster, 97);
                    if (null != bufferData)
                    {
                        if (bufferData.BufferVal > 0L)
                        {
                            HuZhaoBufferItem huZhaoBufferItem = monster.MyBufferExtManager.FindBufferItem(97) as HuZhaoBufferItem;
                            if (huZhaoBufferItem != null)
                            {
                                percent = huZhaoBufferItem.RecoverLifePercent;
                            }
                        }
                        else
                        {
                            Global.RemoveMonsterBufferData(monster, 97);
                            monster.MyBufferExtManager.RemoveBufferItem(97);
                        }
                    }
                }
            }
            return percent;
        }

        // Token: 0x06001FCF RID: 8143 RVA: 0x001B7ED0 File Offset: 0x001B60D0
        public static int ProcessWuDiHuZhaoNoInjured(Monster monster, int subLifeV)
        {
            if (monster.VLife > 0.0)
            {
                if (Global.CanMapUseBuffer(monster.CurrentMapCode, 98))
                {
                    BufferData bufferData = Global.GetMonsterBufferDataByID(monster, 98);
                    if (null != bufferData)
                    {
                        long nowTicks = TimeUtil.NOW();
                        if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                        {
                            subLifeV = 0;
                        }
                        else
                        {
                            Global.RemoveMonsterBufferData(monster, 98);
                        }
                    }
                }
            }
            return subLifeV;
        }

        // Token: 0x06001FD0 RID: 8144 RVA: 0x001B7F60 File Offset: 0x001B6160
        public static int ProcessMarriageFubenInjured(Monster monster, int subLifeV)
        {
            if (monster.VLife > 0.0 && subLifeV > 0)
            {
                if (Global.CanMapUseBuffer(monster.CurrentMapCode, 2000808))
                {
                    BufferData bufferData = Global.GetMonsterBufferDataByID(monster, 2000808);
                    if (null != bufferData)
                    {
                        subLifeV = (int)((double)subLifeV * ((double)bufferData.BufferVal / 100.0));
                    }
                }
            }
            return subLifeV;
        }

        // Token: 0x06001FD1 RID: 8145 RVA: 0x001B7FE0 File Offset: 0x001B61E0
        public static void ProcessDSTimeSubLifeNoShow(Monster monster)
        {
            if (monster.VLife > 0.0)
            {
                double lifeV = 0.0;
                BufferData bufferData = Global.GetMonsterBufferDataByID(monster, 42);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        int timeSlotSecs = (int)(bufferData.BufferVal >> 32 & (long)0xffff_ffffUL);
                        int SubLiefV = (int)(bufferData.BufferVal & (long)0xffff_ffffUL);
                        if (nowTicks - monster.DSStartDSSubLifeNoShowTicks >= (long)(timeSlotSecs * 1000))
                        {
                            monster.DSStartDSSubLifeNoShowTicks = nowTicks;
                            lifeV = (double)SubLiefV;
                        }
                    }
                    else
                    {
                        Global.RemoveMonsterBufferData(monster, 42);
                    }
                }
                if (lifeV > 0.0)
                {
                    GameClient enemyClient = GameManager.ClientMgr.FindClient(monster.FangDuRoleID);
                    if (null != enemyClient)
                    {
                        int nOcc = Global.CalcOriginalOccupationID(enemyClient);
                        GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, enemyClient, monster, 0, (int)lifeV, 1.0, nOcc, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
                        if (monster.VLife <= 0.0)
                        {
                            Global.RemoveMonsterBufferData(monster, 42);
                        }
                    }
                }
            }
        }

        // Token: 0x06001FD2 RID: 8146 RVA: 0x001B8170 File Offset: 0x001B6370
        private static void ProcessTimeSubLifeNoShow(Monster monster, int id)
        {
            if (monster.VLife > 0.0)
            {
                double lifeV = 0.0;
                DelayInjuredBufferItem delayInjuredBufferItem = null;
                BufferData bufferData = Global.GetMonsterBufferDataByID(monster, id);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        delayInjuredBufferItem = (monster.MyBufferExtManager.FindBufferItem(id) as DelayInjuredBufferItem);
                        if (null != delayInjuredBufferItem)
                        {
                            if (nowTicks - delayInjuredBufferItem.StartSubLifeNoShowTicks >= (long)(delayInjuredBufferItem.TimeSlotSecs * 1000))
                            {
                                delayInjuredBufferItem.StartSubLifeNoShowTicks = nowTicks;
                                lifeV = (double)delayInjuredBufferItem.SubLifeV;
                            }
                        }
                    }
                    else
                    {
                        Global.RemoveMonsterBufferData(monster, id);
                        monster.MyBufferExtManager.RemoveBufferItem(id);
                    }
                }
                if (lifeV > 0.0 && null != delayInjuredBufferItem)
                {
                    GameClient enemyClient = GameManager.ClientMgr.FindClient(delayInjuredBufferItem.ObjectID);
                    if (null != enemyClient)
                    {
                        int nOcc = Global.CalcOriginalOccupationID(enemyClient);
                        GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, enemyClient, monster, 0, (int)lifeV, 1.0, nOcc, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
                        if (monster.VLife <= 0.0)
                        {
                            Global.RemoveMonsterBufferData(monster, id);
                            monster.MyBufferExtManager.RemoveBufferItem(id);
                        }
                    }
                    else
                    {
                        Global.RemoveMonsterBufferData(monster, id);
                        monster.MyBufferExtManager.RemoveBufferItem(id);
                    }
                }
            }
        }

        // Token: 0x06001FD3 RID: 8147 RVA: 0x001B8348 File Offset: 0x001B6548
        public static void ProcessAllTimeSubLifeNoShow(Monster monster)
        {
            for (int id = 93; id <= 96; id++)
            {
                DBMonsterBuffer.ProcessTimeSubLifeNoShow(monster, id);
            }
        }
    }
}
